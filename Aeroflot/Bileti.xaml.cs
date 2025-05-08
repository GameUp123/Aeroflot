using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Protobuf.WellKnownTypes;
using XAct.Users;
using static Aeroflot.Bileti;

namespace Aeroflot
{
    /// <summary>
    /// Логика взаимодействия для Bileti.xaml
    /// </summary>
    public partial class Bileti : UserControl
    {
        
        public Bileti()
        {
            InitializeComponent();
          
            LoadPassengerData();
           

        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            System.Windows.Window parentWindow = System.Windows.Window.GetWindow(this);
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
            System.Windows.Window parentWindow = System.Windows.Window.GetWindow(this);
            parentWindow.WindowState = WindowState.Minimized;
        }
        private void HamburgerButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Collapsed;
        }
        private void HamburgerButton_Checked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Visible;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
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
        private void BuyButton_Click(object parameter)
        {
            
        }
        
        private void LoadPassengerData()
        {
            int userId = Registration.Session.UserId;

            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
            string query = "SELECT Id, UserId, PurchaseDate, FinalPrice, FlightNumber, DepartureCity, ArrivalCity, PlaneType, Duration, FlightClass, TicketDiscountType, DepartureTime  FROM PurchasedTickets WHERE UserId = @userId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);  //user_id для фильтрации

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            bool flightpanel = false;
            // Используйте while, чтобы обрабатывать все записи
            while (reader.Read())
            {
                string flightNumber = reader["FlightNumber"].ToString();
                string departureCity = reader["DepartureCity"].ToString();
                string arrivalCity = reader["ArrivalCity"].ToString();
                string planeType = reader["PlaneType"].ToString();
                string Duration = reader["Duration"].ToString();
                string flightClass = reader["FlightClass"].ToString();
                string TicketDiscountType = reader["TicketDiscountType"].ToString();
                string PurchaseDate = reader["PurchaseDate"].ToString();
                string FinalPrice = reader["FinalPrice"].ToString();
                string DepartureTime = reader["DepartureTime"].ToString();

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
                    
                };

                Run boldFlight = new Run
                {
                    Text = "Рейс: ",
                    FontSize = 16,
                    
                };

                Run regularFlight = new Run
                {
                    Text = $"{arrivalCity} - {departureCity}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };

                flightInfo.Inlines.Add(boldFlight);
                flightInfo.Inlines.Add(regularFlight);

                TextBlock FlightNumber = new TextBlock
                {
                    
                };
                Run boldFlight1 = new Run
                {
                    Text = $"Номер рейса: ",
                    FontSize = 16,
                };
                Run regularFlight1 = new Run
                {
                    Text = $"{flightNumber}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                    
                };
                FlightNumber.Inlines.Add(boldFlight1);
                FlightNumber.Inlines.Add(regularFlight1);
                TextBlock departureTime = new TextBlock
                {
                   
                };
                Run boldFlight2 = new Run
                {
                    Text = $"Время вылета: ",
                    FontSize = 16,
                };
                Run regularFlight2 = new Run
                {
                    Text = $"{DepartureTime}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };
                departureTime.Inlines.Add(boldFlight2);
                departureTime.Inlines.Add(regularFlight2);
                TextBlock PlaneType = new TextBlock
                {
                   
                };
                Run boldFlight3 = new Run
                {
                    Text = $"Самолет: ",
                    FontSize = 16
                };
                Run regularFlight3 = new Run
                {
                    Text = $"{planeType}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };
                PlaneType.Inlines.Add(boldFlight3);
                PlaneType.Inlines.Add(regularFlight3);

                TextBlock priceText = new TextBlock
                {
                    
                };
                Run boldFlight4 = new Run
                {
                    Text = $"Цена: ",
                    FontSize = 16
                };
                Run regularFlight4 = new Run
                {
                    Text = $"{FinalPrice} руб.",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };
                priceText.Inlines.Add(boldFlight4);
                priceText.Inlines.Add(regularFlight4);

                TextBlock classText = new TextBlock
                {
                   
                };
                Run boldFlight5 = new Run
                {
                    Text = $"Класс: ",
                    FontSize = 16
                };
                Run regularFlight5 = new Run
                {
                    Text = $"{flightClass}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };
                classText.Inlines.Add(boldFlight5);
                classText.Inlines.Add(regularFlight5);

                TextBlock durationText = new TextBlock
                {
                  
                };
                Run boldFlight6 = new Run
                {
                    Text = $"Время в пути: ",
                    FontSize = 16
                };
                Run regularFlight6 = new Run
                {
                    Text = $"{Duration}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold

                };
                durationText.Inlines.Add(boldFlight6);
                durationText.Inlines.Add(regularFlight6);
                DockPanel buttonPanel = new DockPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 970
                };

                int flightId = reader["Id"] as int? ?? 0; // извлечение ID билета из reader
                Button ExitBilet = new Button
                {
                    Content = "Отменить билет",
                    FontSize = 14,
                    Padding = new Thickness(10, 5, 10, 5),
                    Background = System.Windows.Media.Brushes.Red,
                    Tag = flightId,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderBrush = System.Windows.Media.Brushes.Transparent,
                    Command = new RelayCommand((param) => ExitBilet_Click(flightId)),
                };

                DockPanel.SetDock(ExitBilet, Dock.Right);

                buttonPanel.Children.Add(ExitBilet);
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
               
                DoubleAnimation scaleUpX = new DoubleAnimation(1.0, 1.03, TimeSpan.FromMilliseconds(100));
                Storyboard.SetTargetProperty(scaleUpX, new PropertyPath("RenderTransform.ScaleX"));
                enterStoryboard.Children.Add(scaleUpX);
                mouseEnterTrigger.Actions.Add(new BeginStoryboard { Storyboard = enterStoryboard });
                ExitBilet.RenderTransform = new ScaleTransform();
                ExitBilet.RenderTransformOrigin = new Point(0.5, 0.5);

                EventTrigger mouseLeaveTrigger = new EventTrigger(Button.MouseLeaveEvent);
                Storyboard leaveStoryboard = new Storyboard();

                DoubleAnimation scaleDownX = new DoubleAnimation(1.03, 1.0, TimeSpan.FromMilliseconds(100));
                Storyboard.SetTargetProperty(scaleDownX, new PropertyPath("RenderTransform.ScaleX"));
                leaveStoryboard.Children.Add(scaleDownX);
                mouseLeaveTrigger.Actions.Add(new BeginStoryboard { Storyboard = leaveStoryboard });

                buttonStyle.Triggers.Add(mouseEnterTrigger);
                buttonStyle.Triggers.Add(mouseLeaveTrigger);


               
                ExitBilet.RenderTransform = new ScaleTransform();
                ExitBilet.Style = buttonStyle;
               

                flightPanel.Children.Add(flightInfo);
                flightPanel.Children.Add(FlightNumber);
                flightPanel.Children.Add(departureTime);
                flightPanel.Children.Add(PlaneType);
                flightPanel.Children.Add(classText);
                flightPanel.Children.Add(durationText);
                flightPanel.Children.Add(priceText);
                flightPanel.Children.Add(buttonPanel);

                flightBorder.Child = flightPanel;
                flightsStackPanel.Children.Add(flightBorder);
                flightpanel = true;
            }
            if(!flightpanel)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = "✈️Билетов нет, но мы уже готовим для вас место!",
                    FontWeight = FontWeights.Bold,
                    FontSize = 20,
                    FontFamily = new FontFamily("Century Gothic"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                };
                flightsStackPanel.Children.Add(textBlock);
                Button button = new Button
                {
                    Content = "Найти билет",
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Padding = new Thickness(15, 7, 15, 7),
                    FontFamily = new FontFamily("Century Gothic"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = System.Windows.Media.Brushes.Orange,
                    
                };
                button.Click += ButtonBileti_Click;

                flightsStackPanel.Children.Add(button);
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

                DoubleAnimation scaleUpX = new DoubleAnimation(1.0, 1.03, TimeSpan.FromMilliseconds(100));
                Storyboard.SetTargetProperty(scaleUpX, new PropertyPath("RenderTransform.ScaleX"));
                enterStoryboard.Children.Add(scaleUpX);
                mouseEnterTrigger.Actions.Add(new BeginStoryboard { Storyboard = enterStoryboard });
                button.RenderTransform = new ScaleTransform();
                button.RenderTransformOrigin = new Point(0.5, 0.5);

                EventTrigger mouseLeaveTrigger = new EventTrigger(Button.MouseLeaveEvent);
                Storyboard leaveStoryboard = new Storyboard();

                DoubleAnimation scaleDownX = new DoubleAnimation(1.03, 1.0, TimeSpan.FromMilliseconds(100));
                Storyboard.SetTargetProperty(scaleDownX, new PropertyPath("RenderTransform.ScaleX"));
                leaveStoryboard.Children.Add(scaleDownX);
                mouseLeaveTrigger.Actions.Add(new BeginStoryboard { Storyboard = leaveStoryboard });

                buttonStyle.Triggers.Add(mouseEnterTrigger);
                buttonStyle.Triggers.Add(mouseLeaveTrigger);



                button.RenderTransform = new ScaleTransform();
                button.Style = buttonStyle;
            }

                reader.Close();
        }
        private void ButtonBileti_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Poisk();
            MainFrame.Visibility = Visibility.Visible;
            
        }
        private void ExitBilet_Click(int flightId)
        {
            int userId = Registration.Session.UserId;
            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
            string query = "SELECT FlightNumber FROM PurchasedTickets WHERE Id = @flightId AND UserId = @userId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@flightId", flightId);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string flightNumber = reader["FlightNumber"].ToString();
                    reader.Close();

                    MessageBoxResult result = System.Windows.MessageBox.Show($"Вы уверены то что хотите отменить билет - {flightNumber} \nСумма за билет возвращена не будет ", "Отмена билета", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        string deleteQuery = "DELETE FROM PurchasedTickets WHERE Id = @flightId";
                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                        deleteCommand.Parameters.AddWithValue("@flightId", flightId);

                        try
                        {
                            deleteCommand.ExecuteNonQuery();
                            // обновить UI после удаления
                            flightsStackPanel.Children.Clear();
                            LoadPassengerData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при удалении билета: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}

