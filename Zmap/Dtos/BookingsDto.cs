using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class BookingsDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? HotelId { get; set; }
        public string HotelName { get; set; }
        public int? TransportationCompanyId { get; set; }
        public string TransportationCompanyName { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsPaid { get; set; }
        public double? TotalCost { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? NumberOfSeats { get; set; }
    }
}