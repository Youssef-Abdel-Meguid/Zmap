using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class LockedRoomDto
    {
        public int Id { get; set; }
        public string RoomView { get; set; }
        public string RoomType { get; set; }
        public int? NumberOfLockedRooms { get; set; }
        public DateTime? LockedDateFrom { get; set; }
        public DateTime? LockedDateTo { get; set; }
    }
}