using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ListAllHotelsDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
    }
}