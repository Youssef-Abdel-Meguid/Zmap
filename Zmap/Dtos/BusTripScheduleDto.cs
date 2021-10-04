using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class BusTripScheduleDto
    {
        public int Id { get; set; }
        public int StationFromId { get; set; }
        public int StationToId { get; set; }
        public string StationFromName { get; set; }
        public string StationToName { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public int BusId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}