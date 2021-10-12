using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Dtos;

namespace Zmap.Dtos
{
    public class RoomAvailabilityDetailsDto
    {
        public int? RoomId { get; set; }
        public int? HotelId { get; set; }
        public List<RoomAvailabilityDto> RoomAvailabilities { get; set; }
    }
}