using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class LockedRoomDetailsDto
    {
        public int? RoomId { get; set; }
        public List<LockedRoomDto> LockedRooms { get; set; }
    }
}