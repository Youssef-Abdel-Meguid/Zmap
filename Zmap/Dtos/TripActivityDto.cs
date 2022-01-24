using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class TripActivityDto
    {
        public string ActivityName { get; set; }
        public double? TotalCost { get; set; }
        public string Area { get; set; }
        public int ActivtyId { get; set; }
        public int CompanyId { get; set; }
        public string PhotoUrl { get; set; }
    }
}