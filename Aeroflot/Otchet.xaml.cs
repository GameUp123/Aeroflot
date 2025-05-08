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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aeroflot
{
    /// <summary>
    /// Логика взаимодействия для Otchet.xaml
    /// </summary>
    public partial class Otchet : UserControl
    {
        private int ticketCount;
        private decimal totalRevenue;
        private Dictionary<string, int> flightCounts;
        private int userCount;

        public Otchet()
        {
            InitializeComponent();
            OtchetVisible();
            HideGlavWindow();


        }

        private void HamburgerButton_Checked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Visible;
        }
        private void ExitButton_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void HamburgerButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MenuStack.Visibility = Visibility.Collapsed;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        private void HideGlavWindow()
        {
            Window parentWindow = Window.GetWindow(this); // Получаем родительское окно
            if (parentWindow != null)
            {
                parentWindow.Visibility = Visibility.Collapsed; // Скрываем окно
            }
        }
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                // Если это главное окно, меняем его состояние
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
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            parentWindow.WindowState = WindowState.Minimized;
        }
        private void OtchetVisible()
        {
            DB dB = new DB();
            SqlConnection connection = dB.getConnection();

            string query = "SELECT p.Id, p.UserId, p.PurchaseDate, p.FinalPrice, p.FlightNumber, p.DepartureCity, p.ArrivalCity, p.PlaneType, p.Duration, p.FlightClass, p.TicketDiscountType, p.DepartureTime, u.id " +
                           "FROM PurchasedTickets p " +
                           "JOIN users u ON p.UserId = u.id";

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            ticketCount = 0;
            totalRevenue = 0;
            flightCounts = new Dictionary<string, int>();

            while (reader.Read())
            {
                ticketCount++;
                totalRevenue += Convert.ToDecimal(reader["FinalPrice"]);

                string flightNumber = reader["FlightNumber"].ToString();
                string departureCity = reader["DepartureCity"].ToString();
                string arrivalCity = reader["ArrivalCity"].ToString();
                string flightKey = $"{flightNumber} ({departureCity} - {arrivalCity})";

                if (flightCounts.ContainsKey(flightKey))
                {
                    flightCounts[flightKey]++;
                }
                else
                {
                    flightCounts[flightKey] = 1;
                }
            }

            reader.Close();

            // Сохраняем количество пользователей в поле класса
            SqlCommand userCountCommand = new SqlCommand("SELECT COUNT(*) FROM users", connection);
            userCount = (int)userCountCommand.ExecuteScalar();

            DataGrid dataGrid = new DataGrid();
            dataGrid.VerticalAlignment = VerticalAlignment.Bottom;
            dataGrid.AutoGenerateColumns = false;

            DataGridTextColumn column1 = new DataGridTextColumn
            {
                Header = "Показатель",
                Binding = new Binding("Показатель")
            };
            dataGrid.Columns.Add(column1);

            DataGridTextColumn column2 = new DataGridTextColumn
            {
                Header = "Значение",
                Binding = new Binding("Значение")
            };
            dataGrid.Columns.Add(column2);

            List<ReportItem> reportItems = new List<ReportItem>
           
            {
            new ReportItem { Показатель = "Количество проданных билетов", Значение = ticketCount.ToString() },
            new ReportItem { Показатель = "Выручка", Значение = totalRevenue.ToString() },
            new ReportItem
            {
            Показатель = "Самый популярный рейс",
            Значение = (flightCounts.Count > 0 ? flightCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key : "Нет данных")
            },
            new ReportItem { Показатель = "Количество зарегистрированных пользователей", Значение = userCount.ToString() }
            };

            dataGrid.ItemsSource = reportItems;
            flightStackPanel.Children.Clear(); // чтобы не дублировать при повторных вызовах
            flightStackPanel.Children.Add(dataGrid);

            connection.Close();
        }

        public class ReportItem
        {
            public string Показатель { get; set; }
            public string Значение { get; set; }
        }


        private void SearchButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Отчет");

            worksheet.Cell(1, 1).Value = "Показатель";
            worksheet.Cell(1, 2).Value = "Значение";

            // Используем данные, собранные в OtchetVisible
            List<ReportItem> reportItems = new List<ReportItem>
            {
            new ReportItem { Показатель = "Количество проданных билетов", Значение = ticketCount.ToString() },
            new ReportItem { Показатель = "Выручка", Значение = totalRevenue.ToString() },
            new ReportItem
            {
            Показатель = "Самый популярный рейс",
            Значение = (flightCounts.Count > 0 ? flightCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key : "Нет данных")
            },
            new ReportItem { Показатель = "Количество зарегистрированных пользователей", Значение = userCount.ToString() }
            };
            int row = 2; 
            foreach (var item in reportItems)
            {
                worksheet.Cell(row, 1).Value = item.Показатель;
                worksheet.Cell(row, 2).Value = item.Значение;
                row++;
            }

            worksheet.Columns().AdjustToContents();
            // Сохраняем на рабочий стол
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = System.IO.Path.Combine(desktopPath, "Отчет.xlsx");

            workbook.SaveAs(filePath);

            MessageBox.Show("Отчет успешно сохранен на рабочем столе!");
        }
    }
}
