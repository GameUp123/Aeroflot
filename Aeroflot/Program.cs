using Aeroflot;
using System;

internal static class Program
{
    /// <summary>  
    /// Главная точка входа для приложения.  
    /// </summary>  
    [STAThread]
    static void Main()
    {
        bool isLogged = false;
        while (!isLogged)
        {
            var regWindow = new Registration();
            bool? result = regWindow.ShowDialog(); // Показываем окно регистрации  

            // Если регистрация прошла успешно (если DialogResult == true)  
            if (result == true)
            {
                string nickname = regWindow.Nickname;
                var mainWindow = new Glav(); // Передаем никнейм и пароль в Glav  
                mainWindow.ShowDialog(); // Показываем главное окно  
            }
            
        }
    }
}