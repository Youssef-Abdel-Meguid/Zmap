using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class UserHomeTripDto
    {
        public int UserTripId { get; set; }
        public string PlaceName { get; set; }
        public string CreatedBy { get; set; }
        public decimal? Cost { get; set; }
        public string Photo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}