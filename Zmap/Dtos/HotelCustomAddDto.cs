using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class HotelCustomAddDto
    {
        public int? HotelId { get; set; }
        public List<HotelCustomAdd> HotelCustomAdds { get; set; }
    }
}