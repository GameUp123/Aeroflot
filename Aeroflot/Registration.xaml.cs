using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
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
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;
using XAct.Library.Settings;
using System.Data.SqlClient;
using XAct.Users;
using System.Windows.Media.Animation;
using static Mysqlx.Notice.SessionStateChanged.Types;
namespace Aeroflot
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public string Nickname { get; private set; }
        
        public Registration()
        {
            InitializeComponent();
            ErrorPass.Visibility = Visibility.Collapsed;
            
        }
        public static class Session
        {
            public static string Nickname { get; set; }
            public static int UserId { get; set; }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            int userId = Registration.Session.UserId;
            using (System.Data.SqlClient.SqlConnection conn = new DB().getConnection())
            {
                conn.Open();
                string query = "SELECT date_birth FROM Passengers WHERE user_id = @userId";
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        Session1.BirthDate = Convert.ToDateTime(result); // Сохраняем актуальную дату рождения
                    }
                }
            }

            string nickname = Name.Text;
            string password = Password.Password.ToString();

           
            DB dB = new DB();
            System.Data.SqlClient.SqlConnection connection = dB.getConnection();

            try
            {
                connection.Open();

                //  существует ли уже пользователь с таким именем
                string checkQuery = "SELECT COUNT(*) FROM users WHERE name = @name";
                System.Data.SqlClient.SqlCommand checkCommand = new System.Data.SqlClient.SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@name", nickname);

                int userCount = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (userCount > 0)
                {
                    //   проверка пароля
                    string loginQuery = "SELECT id, password FROM users WHERE name = @name";
                    System.Data.SqlClient.SqlCommand loginCommand = new System.Data.SqlClient.SqlCommand(loginQuery, connection);
                    loginCommand.Parameters.AddWithValue("@name", nickname);

                    System.Data.SqlClient.SqlDataReader reader = loginCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        string storedPassword = reader["password"].ToString();

                        if (storedPassword == password)
                        {
                       
                            userId = Convert.ToInt32(reader["id"]);
                            Session.UserId = userId;
                            Session.Nickname = nickname;
                            this.DialogResult = true;  
                            this.Close();
                        }
                        else
                        {
                            ErrorPass.Visibility = Visibility.Visible;
                        }
                    }
                    reader.Close();
                }
                else
                {
                    ErrorPass.Visibility = Visibility.Visible;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connection.Close();
            }
        }




        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
           
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void ErrorPass_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            
            var UserReg = new UserReg();

            if (UserReg.ShowDialog() == true)
            {

                this.Show();
            }
        }
    }
}
