using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Aeroflot
{
    /// <summary>
    /// Логика взаимодействия для Poisk.xaml
    /// </summary>
    public partial class Poisk : UserControl
    {
        public string FlightNumber { get; set; }
        public decimal TicketPrice { get; set; }
        public string Duration { get; set; }
        public Poisk()
        {
            InitializeComponent();
            LoadData();
            LoadDepartureCities();
            LoadFlightDetails();
           
        }
        private void LoadData()
        {

        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            // Fix: Access the parent window of the UserControl to modify its WindowState
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                if (parentWindow.WindowState == WindowState.Normal)
                {
                    parentWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    parentWindow.WindowState = WindowState.Normal;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            parentWindow.WindowState = WindowState.Minimized;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click_Glav(object sender, RoutedEventArgs e)
        {
            var glav = Window.GetWindow(this) as Glav;
            if (glav != null)
            {
                glav.MainFrame.Navigate(null); 
            }
        }
        private void HamburgerButton_Checked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Visible;
        }
        private void HamburgerButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Collapsed;
        }
        private void ExitButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (ExitButton.IsEnabled == true)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }


        }
        private void SearchButton_Click(object sender, EventArgs e)
        {

        }
        private void LoadDepartureCities()
        {


            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
                try
                {
                    connection.Open();
                    string query = "SELECT DISTINCT city FROM Airport ORDER BY city";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<string> cities = new List<string>();
                    while (reader.Read())
                    {
                        cities.Add(reader["city"].ToString());
                    }

                DepartureCityComboBox.ItemsSource = new List<string>{"Москва","Невинномысск"};
               
                    ArrivalCityComboBox.ItemsSource = new List<string>(cities);

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки городов: " + ex.Message);
                }
            
        }

        private void BuyButton_Click(object parameter)
        {
            int userId = Registration.Session.UserId;
            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
            string query = "SELECT surname, name, middle, document_number FROM Passengers WHERE user_id = @userId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read() || string.IsNullOrEmpty(reader["surname"].ToString()) || string.IsNullOrEmpty(reader["name"].ToString()) || string.IsNullOrEmpty(reader["middle"].ToString()) || string.IsNullOrEmpty(reader["document_number"].ToString()))
            {
                MessageBox.Show("Пожалуйста, заполните персональные данные перед покупкой билета \nПерейдите в Главном меню в свой Профиль и заполните все пункты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                reader.Close();
                connection.Close();
                return;
            }

            reader.Close();
            connection.Close();

            var ticket = parameter as TicketInfo;
            if (ticket == null)
                return;

            var buyBiletWindow = new BuyBilet(ticket);
            buyBiletWindow.ShowDialog();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            int userId = Registration.Session.UserId;
            string departureCity = DepartureCityComboBox.SelectedItem?.ToString();
            string arrivalCity = ArrivalCityComboBox.SelectedItem?.ToString();
            DateTime? departureDate = DepartureDatePicker.SelectedDate;

            string query = @"
         SELECT f.flight_number, airport_destination,departure_time, airport_origin,class, t.ticket_price, f.duration,f.plane_type,f.flight_id,p.ticket_discount_type
         FROM Flights f
         JOIN Tickets t ON f.flight_id = t.flight_id
         JOIN Passengers p ON p.user_id = @userId";

            if (!string.IsNullOrEmpty(departureCity))
                query += " AND airport_destination = @departureCity";
            if (!string.IsNullOrEmpty(arrivalCity))
                query += " AND airport_origin = @arrivalCity";
            if (departureDate.HasValue)
                query += " AND CAST(departure_time AS DATE) = @departureDate";
            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();

            flightsStackPanel.Children.Clear(); // очистить старые результаты
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);

                // ***ADD THIS LINE***
                cmd.Parameters.AddWithValue("@userId", userId);  

                if (!string.IsNullOrEmpty(departureCity))
                    cmd.Parameters.AddWithValue("@departureCity", departureCity);
                if (!string.IsNullOrEmpty(arrivalCity))
                    cmd.Parameters.AddWithValue("@arrivalCity", arrivalCity);
                if (departureDate.HasValue)
                    cmd.Parameters.AddWithValue("@departureDate", departureDate.Value.Date);

                SqlDataReader reader = cmd.ExecuteReader();
                bool flightsFound = false;
                while (reader.Read())
                {
                    string flightNumber = reader["flight_number"].ToString();
                    decimal ticketPrice = Convert.ToDecimal(reader["ticket_price"]);
                    string duration = reader["duration"].ToString();
                    string departureCity1 = reader["airport_origin"].ToString();
                    string arrivalCity1 = reader["airport_destination"].ToString();
                    string flightClass = reader["class"].ToString();
                    string planeType = reader["plane_type"].ToString();
                    string flightId = reader["flight_id"].ToString();
                    string DepartureTime = reader["departure_time"].ToString();
                    string TicketDiscount = reader["ticket_discount_type"].ToString();


                    TextBlock flightInfo = new TextBlock
                    {
                        Text = $"Рейс: {arrivalCity1} - {departureCity1}",
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    TextBlock priceText = new TextBlock
                    {
                        Text = $"Цена: {ticketPrice} руб.",
                        FontSize = 14,

                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    TextBlock durationText = new TextBlock
                    {
                        Text = $"Время в пути: {duration}",
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 10)
                    };

                    TextBlock classText = new TextBlock
                    {
                        Text = $"Класс: {flightClass}",
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    Button buyButton = new Button
                    {
                        Content = "Купить билет",
                        FontSize = 14,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Padding = new Thickness(10, 5, 10, 5),
                        Background = System.Windows.Media.Brushes.Orange,
                        BorderBrush = System.Windows.Media.Brushes.Transparent,
                        Command = new RelayCommand(BuyButton_Click),
                    };
                    var ticket = new TicketInfo
                    {
                        DepartureCity = departureCity1,
                        ArrivalCity = arrivalCity1,
                        Duration = duration,
                        TicketPrice = ticketPrice,
                        FlightClass = flightClass,
                        PlaneType = planeType,
                        FlightNumber = flightNumber,
                        FlightId = flightId,
                        TypeBilet = TicketDiscount,
                        DepartureTime = DepartureTime
                    };

                    buyButton.Command = new RelayCommand(BuyButton_Click);
                    buyButton.CommandParameter = ticket;
                    Style buttonStyle = new Style(typeof(Button));


                    var controlTemplate = new ControlTemplate(typeof(Button));


                    var borderFactory = new FrameworkElementFactory(typeof(Border));
                    borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
                    borderFactory.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
                    borderFactory.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
                    borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(15));
                    borderFactory.SetValue(Border.PaddingProperty, new TemplateBindingExtension(Button.PaddingProperty)); // важный момент



                    var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                    contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);


                    borderFactory.AppendChild(contentPresenterFactory);


                    controlTemplate.VisualTree = borderFactory;


                    buttonStyle.Setters.Add(new Setter(Button.TemplateProperty, controlTemplate));

                    buttonStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
                    buttonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, Brushes.Transparent));

                    EventTrigger mouseEnterTrigger = new EventTrigger(Button.MouseEnterEvent);
                    Storyboard enterStoryboard = new Storyboard();
                    buyButton.RenderTransform = new ScaleTransform();
                    buyButton.RenderTransformOrigin = new Point(0.5, 0.5);
                    DoubleAnimation scaleUpX = new DoubleAnimation(1.0, 1.03, TimeSpan.FromMilliseconds(100));
                    Storyboard.SetTargetProperty(scaleUpX, new PropertyPath("RenderTransform.ScaleX"));
                    enterStoryboard.Children.Add(scaleUpX);
                    mouseEnterTrigger.Actions.Add(new BeginStoryboard { Storyboard = enterStoryboard });


                    EventTrigger mouseLeaveTrigger = new EventTrigger(Button.MouseLeaveEvent);
                    Storyboard leaveStoryboard = new Storyboard();

                    DoubleAnimation scaleDownX = new DoubleAnimation(1.03, 1.0, TimeSpan.FromMilliseconds(100));
                    Storyboard.SetTargetProperty(scaleDownX, new PropertyPath("RenderTransform.ScaleX"));
                    leaveStoryboard.Children.Add(scaleDownX);
                    mouseLeaveTrigger.Actions.Add(new BeginStoryboard { Storyboard = leaveStoryboard });

                    buttonStyle.Triggers.Add(mouseEnterTrigger);
                    buttonStyle.Triggers.Add(mouseLeaveTrigger);


                    buyButton.RenderTransform = new ScaleTransform();
                    buyButton.Style = buttonStyle;

                    StackPanel flightPanel = new StackPanel();
                    flightPanel.Children.Add(flightInfo);
                    flightPanel.Children.Add(priceText);
                    flightPanel.Children.Add(classText);
                    flightPanel.Children.Add(durationText);
                    flightPanel.Children.Add(buyButton);

                    Border flightBorder = new Border
                    {
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(5),
                        Margin = new Thickness(10),
                        Background = System.Windows.Media.Brushes.White,
                        Child = flightPanel
                    };

                    flightsStackPanel.Children.Add(flightBorder);
                    flightsFound = true;
                }
                if(!flightsFound)
                {
                    TextBlock noFlightsText = new TextBlock
                    {
                        Text = "Рейсов нет... Может, Вселенная намекает на отпуск дома?",
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Century Gothic"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10)
                    };

                    flightsStackPanel.Children.Add(noFlightsText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске рейсов: " + ex.Message);
            }
        }



        private void LoadFlightDetails()
        {
            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();

            try
            {
                connection.Open();
                string query = @"
    SELECT f.flight_id, f.flight_number, airport_destination, airport_origin, class, t.ticket_price, f.duration, plane_type, ' ' as ticket_discount_type, f.departure_time
    FROM Flights f
    JOIN Tickets t ON f.flight_id = t.flight_id";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@userId", Registration.Session.UserId); 


                SqlDataReader reader = cmd.ExecuteReader();

                // Чтение всех рейсов из базы данных
                if (!reader.HasRows)
                {
                    MessageBox.Show("Рейсы не найдены.");
                    return;
                }

                while (reader.Read())
                {
                    string flightNumber = reader["flight_number"].ToString();
                    decimal ticketPrice = Convert.ToDecimal(reader["ticket_price"]);
                    string duration = reader["duration"].ToString();
                    string departureCity = reader["airport_origin"].ToString();
                    string arrivalCity = reader["airport_destination"].ToString();
                    string flightClass = reader["class"].ToString();
                    string planeType = reader["plane_type"].ToString();
                    string flightId = reader["flight_id"].ToString();
                    string TicketDiscount = reader["ticket_discount_type"].ToString();
                    string DepartureTime = reader["departure_time"].ToString();
                    Border flightBorder = new Border
                    {
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(5),
                        Margin = new Thickness(10),
                        Background = System.Windows.Media.Brushes.White,

                    };

                    StackPanel flightPanel = new StackPanel();

                    TextBlock flightInfo = new TextBlock
                    {
                        Text = $"Рейс: {arrivalCity} - {departureCity}",
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 0, 5)
                    };

                    TextBlock priceText = new TextBlock
                    {
                        Text = $"Цена: {ticketPrice} руб.",
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    TextBlock classText = new TextBlock
                    {
                        Text = $"Класс: {flightClass}",
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    TextBlock durationText = new TextBlock
                    {
                        Text = $"Время в пути: {duration}",
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 10)
                    };

                    var ticket = new TicketInfo
                    {
                        DepartureCity = departureCity,
                        ArrivalCity = arrivalCity,
                        Duration = duration,
                        TicketPrice = ticketPrice,
                        FlightClass = flightClass,
                        PlaneType = planeType,
                        FlightNumber = flightNumber,
                        FlightId = flightId,
                        TypeBilet = TicketDiscount,
                        DepartureTime = DepartureTime
                    };

                    Button buyButton = new Button
                    {
                        Content = "Купить билет",
                        FontSize = 14,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Padding = new Thickness(10, 5, 10, 5),
                        Background = System.Windows.Media.Brushes.Orange,
                        BorderBrush = System.Windows.Media.Brushes.Transparent,
                        Command = new RelayCommand(BuyButton_Click),
                        CommandParameter = ticket
                    };
                    buyButton.Command = new RelayCommand(BuyButton_Click);
                    buyButton.CommandParameter = ticket;
                    Style buttonStyle = new Style(typeof(Button));


                    var controlTemplate = new ControlTemplate(typeof(Button));


                    var borderFactory = new FrameworkElementFactory(typeof(Border));
                    borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
                    borderFactory.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
                    borderFactory.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
                    borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(15));
                    borderFactory.SetValue(Border.PaddingProperty, new TemplateBindingExtension(Button.PaddingProperty));



                    var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                    contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);


                    borderFactory.AppendChild(contentPresenterFactory);


                    controlTemplate.VisualTree = borderFactory;


                    buttonStyle.Setters.Add(new Setter(Button.TemplateProperty, controlTemplate));

                    buttonStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
                    buttonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, Brushes.Transparent));

                    EventTrigger mouseEnterTrigger = new EventTrigger(Button.MouseEnterEvent);
                    Storyboard enterStoryboard = new Storyboard();
                    buyButton.RenderTransform = new ScaleTransform();
                    buyButton.RenderTransformOrigin = new Point(0.5, 0.5);
                    DoubleAnimation scaleUpX = new DoubleAnimation(1.0, 1.03, TimeSpan.FromMilliseconds(100));
                    Storyboard.SetTargetProperty(scaleUpX, new PropertyPath("RenderTransform.ScaleX"));
                    enterStoryboard.Children.Add(scaleUpX);
                    mouseEnterTrigger.Actions.Add(new BeginStoryboard { Storyboard = enterStoryboard });


                    EventTrigger mouseLeaveTrigger = new EventTrigger(Button.MouseLeaveEvent);
                    Storyboard leaveStoryboard = new Storyboard();

                    DoubleAnimation scaleDownX = new DoubleAnimation(1.03, 1.0, TimeSpan.FromMilliseconds(100));
                    Storyboard.SetTargetProperty(scaleDownX, new PropertyPath("RenderTransform.ScaleX"));
                    leaveStoryboard.Children.Add(scaleDownX);
                    mouseLeaveTrigger.Actions.Add(new BeginStoryboard { Storyboard = leaveStoryboard });

                    buttonStyle.Triggers.Add(mouseEnterTrigger);
                    buttonStyle.Triggers.Add(mouseLeaveTrigger);


                    buyButton.RenderTransform = new ScaleTransform();
                    buyButton.Style = buttonStyle;

                    flightPanel.Children.Add(flightInfo);
                    flightPanel.Children.Add(priceText);
                    flightPanel.Children.Add(classText);
                    flightPanel.Children.Add(durationText);
                    flightPanel.Children.Add(buyButton);

                    flightBorder.Child = flightPanel;
                    flightsStackPanel.Children.Add(flightBorder);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private void DepartureCityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ArrivalCityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class TicketInfo
    {
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public string Duration { get; set; }
        public decimal TicketPrice { get; set; }
        public string FlightClass { get; set; }
        public string PlaneType { get; set; }
        public string FlightNumber { get; set; }
        public string FlightId { get; set; }
        public string TypeBilet {  get; set; }  
        public string DepartureTime { get; set; }

    }

    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }

}

