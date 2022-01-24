using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ClientListAllRoomsDto
    {
        public int RoomId { get; set; }
        public string RoomView { get; set; }
        public string RoomType { get; set; }
        public decimal? PricePerNight { get; set; }
        public int RoomAvaId { get; set; }
        public string Photo { get; set; }
        public string Accommodation { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<string> AddsOn { get; set; }
    }
}