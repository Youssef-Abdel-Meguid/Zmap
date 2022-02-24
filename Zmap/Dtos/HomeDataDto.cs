using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class HomeDataDto
    {
        public List<ReservationsDto> ReservationsDtos { get; set; }
        public List<HomeBlogsDto> HomeBlogsDtos { get; set; }
    }
}