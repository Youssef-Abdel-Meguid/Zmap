using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.DTOs;

namespace Zmap.Dtos
{
    public class RoomDataDto
    {
        public int? HotelId { get; set; }
        public List<RoomDto> Rooms { get; set; }
    }
}