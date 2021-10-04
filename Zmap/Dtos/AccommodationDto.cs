using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class RoomAvailabilityDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int AccommodationId { get; set; }
        public string ArabicName { get; set; }
        public string OtherValue { get; set; }
        public decimal? PricePerNight { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}