using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class TripHotelDto
    {
        public string HotelName { get; set; }
        public decimal? TotalCost { get; set; }
        public string RoomType { get; set; }
        public string RoomView { get; set; }
        public string RoomAccommodation { get; set; }
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public string PhotoUrl { get; set; }
    }
}