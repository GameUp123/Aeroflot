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
using DocumentFormat.OpenXml.Wordprocessing;

namespace Aeroflot
{
    /// <summary>
    /// Логика взаимодействия для Popolnenie.xaml
    /// </summary>
    public partial class Popolnenie : Window
    {
        public event EventHandler PopolnenieSuccessful;
        public Popolnenie()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string currentDigits = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Если уже 16 цифр — блокируем ввод
            if (currentDigits.Length >= 16)
            {
                e.Handled = true;
                return;
            }
            // Разрешаем только цифры
            e.Handled = !char.IsDigit(e.Text, 0);
        }
        private void Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Сохраняем текущую позицию курсора
            int selectionStart = textBox.SelectionStart;

            // Удаляем все символы кроме цифр
            string rawText = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Форматируем — каждые 4 цифры добавляем пробел
            string formatted = string.Join(" ", Enumerable.Range(0, (rawText.Length + 3) / 4)
                .Select(i => rawText.Skip(i * 4).Take(4))
                .Where(group => group.Any())
                .Select(group => new string(group.ToArray())));

            if (textBox.Text != formatted)
            {
                // Считаем, сколько цифр было до текущей позиции курсора
                int digitsBeforeCursor = textBox.Text
                    .Take(selectionStart)
                    .Count(char.IsDigit);

                textBox.Text = formatted;

                // Считаем, где должен быть курсор в новом тексте
                int newCaretIndex = 0;
                int digitsCount = 0;
                while (newCaretIndex < textBox.Text.Length && digitsCount < digitsBeforeCursor)
                {
                    if (char.IsDigit(textBox.Text[newCaretIndex]))
                        digitsCount++;
                    newCaretIndex++;
                }

                textBox.SelectionStart = newCaretIndex;
            }
        }
        private void Srok_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string currentDigits = new string(textBox.Text.Where(char.IsDigit).ToArray());

          
            if (currentDigits.Length >= 4)
            {
                e.Handled = true;
                return;
            }
            // Разрешаем только цифры
            e.Handled = !char.IsDigit(e.Text, 0);
        }
        private void Srok_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Сохраняем текущую позицию курсора
            int selectionStart = textBox.SelectionStart;

            // Удаляем все символы кроме цифр
            string rawText = new string(textBox.Text.Where(char.IsDigit).ToArray());

           
            string formatted = string.Join("/", Enumerable.Range(0, (rawText.Length + 3) / 2)
                .Select(i => rawText.Skip(i * 2).Take(2))
                .Where(group => group.Any())
                .Select(group => new string(group.ToArray())));

            if (textBox.Text != formatted)
            {
                // Считаем, сколько цифр было до текущей позиции курсора
                int digitsBeforeCursor = textBox.Text
                    .Take(selectionStart)
                    .Count(char.IsDigit);

                textBox.Text = formatted;

                // Считаем, где должен быть курсор в новом тексте
                int newCaretIndex = 0;
                int digitsCount = 0;
                while (newCaretIndex < textBox.Text.Length && digitsCount < digitsBeforeCursor)
                {
                    if (char.IsDigit(textBox.Text[newCaretIndex]))
                        digitsCount++;
                    newCaretIndex++;
                }

                textBox.SelectionStart = newCaretIndex;
            }
        }
        private async void PopolnenieButton_Click(object sender, RoutedEventArgs e)
        {
            

            if (string.IsNullOrWhiteSpace(Name.Text) ||
        string.IsNullOrWhiteSpace(Number.Text) ||
        string.IsNullOrWhiteSpace(Srok.Text) ||
        string.IsNullOrWhiteSpace(CVV.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Number.Text.Length != 19 ||
    Srok.Text.Length != 5 ||
    CVV.Text.Length != 3)
            {
                MessageBox.Show("Проверьте правильность ввода: одно или несколько полей заполнены не полностью.");
                return;
            }
            int userId = Registration.Session.UserId;
            decimal currentBalance;
            decimal popolnenieAmount;

            // Проверка, что введенная сумма является числом
            if (!decimal.TryParse(Balance.Text, out popolnenieAmount))
            {
             
                MessageBox.Show("Введите корректную сумму");
                return;
            }

            if (popolnenieAmount <= 0)
            {
               
                MessageBox.Show("Сумма пополнения должна быть больше нуля");
                return;
            }
            PaymentBorder.Visibility = Visibility.Visible;
            StatusText.Text = "Обработка платежа...";
            StatusText.Foreground = Brushes.Black;
            LoadingBar.Visibility = Visibility.Visible;

            // Эмуляция "обработки"
            await Task.Delay(2000); // 2 секунды ожидания

            LoadingBar.Visibility = Visibility.Collapsed;

            StatusText.Text = "Платеж успешно выполнен!";
            StatusText.Foreground = Brushes.Green;
            await Task.Delay(2000);
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

            decimal newBalance = currentBalance + popolnenieAmount;
            string updateQuery = "UPDATE Passengers SET Balance = @balance WHERE user_id = @user_id";
            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
            {
                updateCommand.Parameters.AddWithValue("@balance", newBalance);
                updateCommand.Parameters.AddWithValue("@user_id", userId);
                updateCommand.ExecuteNonQuery();
            }
            PopolnenieSuccessful?.Invoke(this, EventArgs.Empty);
            this.Close();
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Balance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
            else
            {
                string text = Balance.Text + e.Text;
                if (text.Length > 7)
                {
                    e.Handled = true;
                }
                else if (decimal.TryParse(text, out decimal amount) && amount > 999999)
                {
                    e.Handled = true;
                }
            }
        }
        private void Balance_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Удаляем все символы кроме цифр
            string rawText = new string(textBox.Text.Where(char.IsDigit).ToArray());
            string formatted = string.Join("", Enumerable.Range(0, (rawText.Length + 3) / 2)
                .Select(i => rawText.Skip(i * 2).Take(2))
                .Where(group => group.Any())
                .Select(group => new string(group.ToArray())));
            textBox.Text = formatted;
        }

        private void CVV_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Удаляем все символы кроме цифр
            string rawText = new string(textBox.Text.Where(char.IsDigit).ToArray());
            string formatted = string.Join("", Enumerable.Range(0, (rawText.Length + 3) / 2)
                .Select(i => rawText.Skip(i * 2).Take(2))
                .Where(group => group.Any())
                .Select(group => new string(group.ToArray())));
            textBox.Text = formatted;
        }
        private void CVV_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string currentDigits = new string(textBox.Text.Where(char.IsDigit).ToArray());


            if (currentDigits.Length >= 3)
            {
                e.Handled = true;
                return;
            }

            // Разрешаем только цифры
            e.Handled = !char.IsDigit(e.Text, 0);
        }
        private void Name_PreviewTextInput(Object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string currentDigits = new string(textBox.Text.Where(char.IsDigit).ToArray());
            if (currentDigits.Length >= 25)
            {
                e.Handled = true;
                return;
            }
        }

    }
}
