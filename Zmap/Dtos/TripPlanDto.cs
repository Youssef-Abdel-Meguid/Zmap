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
        public int Adults { get; set; }
        public int Child { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public List<int> Ages { get; set; }
        public string HomeName { get; set; }
        public string DestinationName { get; set; }
    }
}