using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeroflot
{
    internal class AuthModel
    {
        public int Id { get; set; }  // Идентификатор записи в базе
        public string Username { get; set; }  // Имя пользователя (nvarchar)
        public string Password { get; set; }  // Пароль (nvarchar)
    }
    
}
