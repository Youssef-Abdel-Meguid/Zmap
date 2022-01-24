using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ClientHotelDetailsDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> AddsOn { get; set; }
        public List<string> Photos { get; set; }
        public List<ClientListAllRoomsDto> Rooms { get; set; }
    }
}