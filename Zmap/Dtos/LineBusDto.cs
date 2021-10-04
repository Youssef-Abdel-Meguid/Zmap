using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class LineBusDto
    {
        public int Id { get; set; }
        public int LineId { get; set; }
        public int BusId { get; set; }
        public string BusName { get; set; }
        public string BusNumber { get; set; }
        public double SeatPrice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public DateTime? DepartureTime { get; set; }
    }
}