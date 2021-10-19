using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Zmap.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int HotleId { get; set; }
        public string AccommodationType { get; set; }
        public string RoomView { get; set; }
        public int? NumberOfRooms { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
