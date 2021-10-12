using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class PhotoDetailsDto
    {
        public int? HotelId { get; set; }
        public int? RoomId { get; set; }
        public int? BusId { get; set; }
        public int? CompanyId { get; set; }
        public int? StationId { get; set; }
        public List<Gallery> Photos { get; set; }
    }
}