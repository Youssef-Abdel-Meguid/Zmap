using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class HomeDataDto
    {
        public List<ReservationsDto> ReservationsDtos { get; set; }
        public List<HomeBlogsDto> HomeBlogsDtos { get; set; }
        public int DestinationId { get; set; }
        public int HomeId { get; set; }
        public int Adults { get; set; }
        public int Child { get; set; }
        public DateTime to { get; set; }
        public DateTime from { get; set; }
    }
}