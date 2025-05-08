using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Aeroflot.Registration;
using static Aeroflot.Glav;
using System.Reflection;
using System.Xml.Linq;
using XAct.Users;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using DocumentFormat.OpenXml.Spreadsheet;


namespace Aeroflot
{
    /// <summary>
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public static class Session1
    {
        public static DateTime BirthDate { get; set; }
    }
    public partial class Account : UserControl
    {
      
        public Account()
        {
            InitializeComponent();
            LoadPassengerData();
            string nickname = Session.Nickname;
            if (nickname.Length > 10)
                nickname = nickname.Substring(0, 7) + "...";
            LoadUserInfo();
            ProfileUser.Text = $"{nickname}";
            BalanceLoad();
            LoadProfileData();

        }
        private void LoadProfileData()
        {
            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
            connection.Open();

            string query = "SELECT * FROM Passengers WHERE user_id = @userId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", Session.UserId); 

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // Если данные уже есть
                {
                    // Блокируем кнопку
                    SaveButton.IsEnabled = false;
                }
                else
                {
                   
                    SaveButton.IsEnabled = true;
                }
            }
        }
        private void BalanceLoad()
        {
            int userId = Registration.Session.UserId;
            decimal currentBalance;

            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
            connection.Open();

                // текущий баланс
                string selectQuery = "SELECT Balance FROM Passengers WHERE user_id = @user_id";
                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@user_id", userId);
                    object result = selectCommand.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        currentBalance = Convert.ToDecimal(result);
                    else
                        currentBalance = 0;
                }
                Balance.Text = currentBalance.ToString("N0");
        }
        private void LoadUserInfo()
        {
            DB dB = new DB();
            using (SqlConnection connection = dB.getConnection())
            {
                connection.Open();
                string query = "SELECT surname, middle, document_number, date_birth FROM Passengers WHERE user_id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", Session.UserId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    surnameTextBox.Text = reader["surname"].ToString();
                    middleTextBox.Text = reader["middle"].ToString();
                    documentNumberTextBox.Text = reader["document_number"].ToString();
                    if (reader["date_birth"] != DBNull.Value)
                        BirthdayTextBox.SelectedDate = Convert.ToDateTime(reader["date_birth"]);
                }
                reader.Close();
            }
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
        private void Button_Click_Glav(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        private void HamburgerButton_Checked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Visible;
        }
        private void HamburgerButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Collapsed;
        }
        

        public void InsertPassenger(string name, string surname, string middle, string documentNumber, DateTime dateOfBirth, string ticketType)
        {
            int userId = Registration.Session.UserId;  

            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
            string query = "INSERT INTO Passengers (name, surname, middle, document_number, date_birth, ticket_discount_type, user_id) " +
                               "VALUES (@name, @surname, @middle, @document_number, @date_of_birth, @ticket_type, @user_id)";  

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@surname", surname);
                command.Parameters.AddWithValue("@middle", middle);
                command.Parameters.AddWithValue("@document_number", documentNumber);
                command.Parameters.AddWithValue("@date_of_birth", dateOfBirth);
                command.Parameters.AddWithValue("@ticket_type", ticketType);
                command.Parameters.AddWithValue("@user_id", userId);  

                connection.Open();
                command.ExecuteNonQuery();
        }
        private void LoadPassengerData()
        {
            int userId = Registration.Session.UserId;

            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
            string query = "SELECT surname, name, middle, document_number, date_birth FROM Passengers WHERE user_id = @userId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId); 

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    surnameTextBox.Text = reader["surname"].ToString();
                    nameTextBox.Text = reader["name"].ToString();
                    middleTextBox.Text = reader["middle"].ToString();
                    documentNumberTextBox.Text = reader["document_number"].ToString();

                    if (reader["date_birth"] != DBNull.Value)
                        BirthdayTextBox.SelectedDate = Convert.ToDateTime(reader["date_birth"]);
                        Session1.BirthDate = BirthdayTextBox.SelectedDate.Value;
                        Session1.BirthDate = Convert.ToDateTime(reader["date_birth"]);
            }

                reader.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text;
            string surname = surnameTextBox.Text;
            string middle = middleTextBox.Text;
            string documentNumber = documentNumberTextBox.Text;
            if (string.IsNullOrWhiteSpace(name) ||
               string.IsNullOrWhiteSpace(surname) ||
               string.IsNullOrWhiteSpace(middle) ||
               string.IsNullOrWhiteSpace(documentNumber))
            {
                MessageBox.Show("Ошибка: Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DateTime? dateOfBirth = BirthdayTextBox.SelectedDate;
            if (!dateOfBirth.HasValue)
            {
                MessageBox.Show("Пожалуйста, выберите дату рождения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SaveButton.IsEnabled = false;
           
            

            // Вычисляем возраст
            int age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (DateTime.Now < dateOfBirth.Value.AddYears(age)) age--;

            
            string ticketType;
            if (age < 12)
                ticketType = "Детский";
            else if (age < 21)
                ticketType = "Студенческий";
            else
                ticketType = "Взрослый";

            foreach (var item in TicketTypeComboBox.Items)
            {
                if (item is ComboBoxItem comboBoxItem && comboBoxItem.Content.ToString() == ticketType)
                {
                    TicketTypeComboBox.SelectedItem = comboBoxItem;
                    break;
                }
            }

            try
            {
                InsertPassenger(name, surname, middle, documentNumber, dateOfBirth.Value, ticketType);
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Popolnit_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text;
            string surname = surnameTextBox.Text;
            string middle = middleTextBox.Text;
            string documentNumber = documentNumberTextBox.Text;
            if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(surname) ||
            string.IsNullOrWhiteSpace(middle) ||
            string.IsNullOrWhiteSpace(documentNumber))
            {
                MessageBox.Show("Ошибка: Сначало заполните персональные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var buyBiletWindow = new Popolnenie();
            
            buyBiletWindow.ShowDialog();
            UpdateBalance();
        }
        private void UpdateBalance()
        {
            int userId = Registration.Session.UserId;
            decimal currentBalance;

            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();
            connection.Open();

            string selectQuery = "SELECT Balance FROM Passengers WHERE user_id = @user_id";
            using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
            {
                selectCommand.Parameters.AddWithValue("@user_id", userId);
                object result = selectCommand.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    currentBalance = Convert.ToDecimal(result);
                else
                    currentBalance = 0;
            }

            Balance.Text = currentBalance.ToString("N0");
        }
        public delegate void LogoutEventHandler(object sender, RoutedEventArgs e);
        public event LogoutEventHandler Logout;
        private void ExitAccount_Click(object sender, RoutedEventArgs e)
        {
                if (ExitAccount.IsEnabled == true)
                {
                MessageBoxResult result = System.Windows.MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Logout?.Invoke(this, e);
                }
               
            }
        }
        private void Balance_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            
        }

        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        
    } 
}
