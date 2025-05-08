using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для UserReg.xaml
    /// </summary>
    public partial class UserReg : Window
    {
        public UserReg()
        {
            InitializeComponent();
            ErrorPass.Visibility = Visibility.Collapsed;
            ErrorPass1.Visibility = Visibility.Collapsed;
            ErrorPass2.Visibility = Visibility.Collapsed;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var sb = this.Resources["TextBoxAppear"] as Storyboard;
            Storyboard.SetTarget(sb, textBox);
            sb.Begin();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
           
            if (string.IsNullOrEmpty(Name.Text) || string.IsNullOrEmpty(Password.Password) || string.IsNullOrEmpty(PasswordRename.Password))
            {
               ErrorPass1.Visibility = Visibility.Visible;
                ErrorPass2.Visibility= Visibility.Collapsed;
                ErrorPass.Visibility= Visibility.Collapsed;
                return;
            }

            // Проверяем, что пароли совпадают
            if (Password.Password != PasswordRename.Password)
            {
                ErrorPass2.Visibility = Visibility.Visible;
                ErrorPass1.Visibility = Visibility.Collapsed;
                ErrorPass.Visibility = Visibility.Collapsed;
                return;
            }
            
            using (System.Data.SqlClient.SqlConnection conn = new DB().getConnection()) 
            {
                try
                {
                    conn.Open();

                   
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE name = @name";
                    using (System.Data.SqlClient.SqlCommand checkCmd = new System.Data.SqlClient.SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@name", Name.Text);
                        int userCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (userCount > 0)
                        {
                            
                            ErrorPass.Visibility = Visibility.Visible;
                            ErrorPass1.Visibility = Visibility.Collapsed;
                            ErrorPass2.Visibility = Visibility.Collapsed;
                            return; 
                        }
                    }
                    MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    // SQL запрос на добавление нового пользователя
                    string query = "INSERT INTO users (name, password) VALUES (@name, @password)";
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@name", Name.Text);
                        cmd.Parameters.AddWithValue("@password", Password.Password);

 
                        cmd.ExecuteNonQuery();
                    }

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении пользователя в базу данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            this.WindowState = WindowState.Minimized;
        }

    }
}
