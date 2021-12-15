using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class TripTransportationDto
    {
        public string CompanyName { get; set; }
        public string StationFrom { get; set; }
        public string StationTo { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string BusName { get; set; }
        public string BusCategory { get; set; }
        public double TotalCost { get; set; }
        public int NumberOfSeats { get; set; }
        public int CompanyId { get; set; }
        public int BusId { get; set; }
    }
}