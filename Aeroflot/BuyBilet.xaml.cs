using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aeroflot
{
    /// <summary>
    /// Логика взаимодействия для BuyBilet.xaml
    /// </summary>
    public partial class BuyBilet : Window
    {
        public event Action UpdateFlightListEvent;
        decimal finalprice = 0;
        private TicketInfo _ticket;
        public BuyBilet(TicketInfo ticket)
        {
            InitializeComponent();

            _ticket = ticket;


            decimal basePrice = _ticket.TicketPrice;

            string GetTicketType()
            {
                DateTime birthDate = Session1.BirthDate;

                if (birthDate == default)
                {
                    int userId = Registration.Session.UserId;
                    using (SqlConnection conn = new DB().getConnection())
                    {
                        conn.Open();
                        string query = "SELECT date_birth FROM Passengers WHERE user_id = @userId";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                birthDate = Convert.ToDateTime(result);
                                Session1.BirthDate = birthDate;
                            }
                        }
                    }
                }

                int age = DateTime.Today.Year - birthDate.Year;
                if (birthDate > DateTime.Today.AddYears(-age)) age--;

                if (age < 12)
                    return "Детский";
                else if (age < 21)
                    return "Студенческий";
                else
                    return "Взрослый";
            }

            string ticketType = GetTicketType(); // определяем тип билета
            decimal discount = GetDiscount(ticketType); //скидку
            finalprice = basePrice * (1 - discount);

            departureTextBlock.Text = $"Рейс: {_ticket.ArrivalCity} - {_ticket.DepartureCity}";
            FlightClassTextBlock.Inlines.Clear();
            FlightClassTextBlock.Inlines.Add(new Run { Text = "Номер рейса:  " });
            FlightClassTextBlock.Inlines.Add(new Run { Text = _ticket.FlightNumber.ToString(), FontWeight = FontWeights.Bold });
            PlaneTypeTextBlock.Inlines.Add(new Run { Text = "Ваш самолет:  " });
            PlaneTypeTextBlock.Inlines.Add(new Run { Text = _ticket.PlaneType, FontWeight = FontWeights.Bold });
            DepartureTimeTextBlock.Inlines.Add(new Run { Text = "Время вылета: " });
            DepartureTimeTextBlock.Inlines.Add(new Run { Text = _ticket.DepartureTime,FontWeight = FontWeights.Bold });
            classTextBlock.Inlines.Add(new Run { Text = "Класс:  " });
            classTextBlock.Inlines.Add(new Run { Text = _ticket.FlightClass, FontWeight = FontWeights.Bold });
            priceTextBlock.Inlines.Add(new Run { Text = "Цена (с персональной скидкой):  " });
            priceTextBlock.Inlines.Add(new Run { Text = $"{finalprice:0.00} ₽ ({ticketType})", FontWeight = FontWeights.Bold });
            durationTextBlock.Inlines.Add(new Run { Text = "Время в пути:  " });
            durationTextBlock.Inlines.Add(new Run { Text = _ticket.Duration, FontWeight = FontWeights.Bold });


        }
        private decimal GetDiscount(string ticketType)
        {
            switch (ticketType)
            {
                case "Детский":
                    return 0.5m;
                case "Студенческий":
                    return 0.25m;
                default:
                    return 0.0m;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window parentWindow = System.Windows.Window.GetWindow(this);
            parentWindow.WindowState = WindowState.Minimized;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            int userId = Registration.Session.UserId;
            decimal currentBalance;
            int passengerId = -1;

            DB dB = new DB();
            using (SqlConnection connection = dB.getConnection())
            {
                connection.Open();

                // Получаем passenger_id и Balance из таблицы Passengers
                string getPassengerQuery = "SELECT passenger_id, Balance FROM Passengers WHERE user_id = @user_id";
                using (SqlCommand cmd = new SqlCommand(getPassengerQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            passengerId = Convert.ToInt32(reader["passenger_id"]);
                            currentBalance = Convert.ToDecimal(reader["Balance"]);
                        }
                        else
                        {
                            MessageBox.Show("Пассажир не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
                // Проверка наличия средств
                if (currentBalance < finalprice)
                {
                    MessageBox.Show("Недостаточно средств для оплаты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                decimal newBalance = currentBalance - finalprice;

                // Обновляем баланс пассажира
                string updateQuery = "UPDATE Passengers SET Balance = @newBalance WHERE passenger_id = @passenger_id";
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@newBalance", newBalance);
                    command.Parameters.AddWithValue("@passenger_id", passengerId);
                    command.ExecuteNonQuery();
                }

                // Запрос для получения ID билета из таблицы Tickets по flight_id
                string getTicketIdQuery = "SELECT ticket_id FROM Tickets WHERE flight_id = @flight_id";
                int ticketId = 0;

                using (SqlCommand getTicketCmd = new SqlCommand(getTicketIdQuery, connection))
                {
                    getTicketCmd.Parameters.AddWithValue("@flight_id", _ticket.FlightId);
                    object ticketResult = getTicketCmd.ExecuteScalar();

                    if (ticketResult != null)
                    {
                        ticketId = Convert.ToInt32(ticketResult);
                    }
                    else
                    {
                        MessageBox.Show("Билет для указанного рейса не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Вставляем информацию о покупке в таблицу PurchasedTickets
                string insertPurchasedQuery = @"INSERT INTO PurchasedTickets 
                                        (UserId,  FinalPrice, FlightNumber, DepartureCity, ArrivalCity, PlaneType, Duration, FlightClass, PurchaseDate, TicketDiscountType, DepartureTime)
                                        VALUES 
                                        (@UserId,  @FinalPrice, @FlightNumber, @DepartureCity, @ArrivalCity, @PlaneType, @Duration, @FlightClass, @PurchaseDate, @DiscountType, @DepartureTime)";
                using (SqlCommand insertCmd = new SqlCommand(insertPurchasedQuery, connection))
                {
                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                    insertCmd.Parameters.AddWithValue("@FinalPrice", finalprice);
                    insertCmd.Parameters.AddWithValue("@FlightNumber", _ticket.FlightNumber);
                    insertCmd.Parameters.AddWithValue("@DepartureCity", _ticket.DepartureCity);
                    insertCmd.Parameters.AddWithValue("@ArrivalCity", _ticket.ArrivalCity);
                    insertCmd.Parameters.AddWithValue("@PlaneType", _ticket.PlaneType);
                    insertCmd.Parameters.AddWithValue("@Duration", _ticket.Duration);
                    insertCmd.Parameters.AddWithValue("@FlightClass", _ticket.FlightClass);
                    insertCmd.Parameters.AddWithValue("@PurchaseDate", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@DiscountType", _ticket.TypeBilet);
                    DateTime departureTime = DateTime.Parse(_ticket.DepartureTime);
                    insertCmd.Parameters.AddWithValue("@DepartureTime", departureTime);

                    insertCmd.ExecuteNonQuery();
                }

                // Сообщаем пользователю о успешной покупке
                MessageBox.Show($"Билет успешно куплен! Ваш новый баланс: {newBalance:0.00} ₽", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем список рейсов
                UpdateFlightListEvent?.Invoke();
                this.Close();
            }

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

    }
}

