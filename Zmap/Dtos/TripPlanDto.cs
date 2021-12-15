using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class TripPlanDto
    {
        public int HomeId { get; set; }
        public int DestinationId { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChild { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public int NumberOfDays { get; set; }
        public TripActivityDto TripActivityData { get; set; }
        public TripHotelDto TripHotelData { get; set; }
        public TripTransportationDto TransportationData { get; set; }
        public List<TripHotelDto> TripHotels { get; set; }
        public List<TripActivityDto> TripActivities { get; set; }
        public List<TripTransportationDto> TripTransportations { get; set; }
        public int ActivityId { get; set; }
        public int HotelId { get; set; }
        public int TransportationId { get; set; }
        public bool IsSelected { get; set; }
    }
}