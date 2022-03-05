using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class TripActivityDto
    {
        public string ActivityName { get; set; }
        public string ActivityCategory { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? TotalCost { get; set; }
        public string Area { get; set; }
        public int ActivtyId { get; set; }
        public List<string> PhotoUrl { get; set; }
        public int? CompanyId { get; set; }
    }
}