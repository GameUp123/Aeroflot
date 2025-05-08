using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeroflot
{
    public class PurchasedTicketInfo
    {
        public int TicketId { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public string PlaneType { get; set; }
        public string Duration { get; set; }
        public string FlightClass { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal FinalPrice { get; set; }
        public override string ToString()
        {
            return $"Рейс {FlightNumber}: {DepartureCity} - {ArrivalCity}, Класс: {FlightClass}, Самолёт: {PlaneType}, Цена: {FinalPrice:0.00} ₽";
        }
    }

}
