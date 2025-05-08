using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Aeroflot.Registration;

namespace Aeroflot
{
    /// <summary>
    /// Логика взаимодействия для Glav.xaml
    /// </summary>
    
    public partial class Glav : Window
    {
        
       

        public Glav()
        {
            InitializeComponent();
            
            string nickname = Session.Nickname;
            if (nickname.Length > 10)
                nickname = nickname.Substring(0, 7) + "...";

            ProfileUser.Text = $"Здравствуйте, {nickname}!";
            TextAccount.Text = $"{nickname}";
           

        }
        private void Account_Logout(object sender, RoutedEventArgs e)
        {
            this.Close();
            Registration registration = new Registration();
        }
        public System.Windows.Forms.DockStyle Dock { get; internal set; }

        
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            MenuStack.Width = e.NewSize.Width;
            MenuStack.Height = e.NewSize.Height;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void Info_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Logout_Click(Object sender, RoutedEventArgs e)
        {

        }
        private void ProfilePopup_Opened(object sender, EventArgs e)
        {
            
        }
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HamburgerButton_Checked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Visible;
        }
        private void HamburgerButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Collapsed;
        }
        private void ProfileUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            Account account = new Account();
            account.Logout += Account_Logout;
            MainFrame.Content = account;
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized; 
            }
            else
            {
                this.WindowState = WindowState.Normal; 
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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
                ErrorPoisk.Visibility = Visibility.Visible;
                reader.Close();
                connection.Close();
                return;
            }
            else
            {
                MainFrame.Navigate(new Poisk());
                
            }
        }

        private void TelezkOzero_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Poisk();
            MainFrame.Visibility = Visibility.Visible;
        }
        private void OstrovOlhon_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Poisk();
            MainFrame.Visibility = Visibility.Visible;
        }

        private void SkyPark_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Poisk();
            MainFrame.Visibility = Visibility.Visible;
        }
        private void Bileti_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Bileti();
            MainFrame.Visibility = Visibility.Visible;
        }
        private void Otchet_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Otchet();
            MainFrame.Visibility = Visibility.Visible;
        }
        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Account account = new Account();
            account.Logout += Account_Logout;
            MainFrame.Content = account;
            ErrorPoisk.Visibility = Visibility.Collapsed;
        }
    }
}
