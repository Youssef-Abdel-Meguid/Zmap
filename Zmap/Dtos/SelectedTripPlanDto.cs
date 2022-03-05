using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class SelectedTripPlanDto
    {
        public List<TripHotelDto> TripHotels { get; set; }
        public List<TripTransportationDto> TripTransportations { get; set; }
        public List<TripActivityDto> TripActivities { get; set; }
        public int HomeId { get; set; }
        public int DestinationId { get; set; }
        public int Adults { get; set; }
        public int Child { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public List<int> Ages { get; set; }
        public string HomeName { get; set; }
        public string DestinationName { get; set; }
    }
}