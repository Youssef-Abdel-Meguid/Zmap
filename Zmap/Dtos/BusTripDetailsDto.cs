using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class BusTripDetailsDto
    {
        public int? BusId { get; set; }
        public string BusName { get; set; }
        public List<BusTripScheduleDto> BusTrips { get; set; }
    }
}