using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class AddOnDetailsDto
    {
        public int? HotelId { get; set; }
        public int? BusId { get; set; }
        public int? RoomId { get; set; }
        public List<AddOnDto> AddOns { get; set; }
    }
}