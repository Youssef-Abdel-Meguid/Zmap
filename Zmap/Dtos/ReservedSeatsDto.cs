using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class ReservedSeatsDto
    {
        public int? BusId { get; set; }
        public int? BusTripId { get; set; }
        public int? NumberOfSeats { get; set; }
        public List<string> SeatsNumber { get; set; }
        public List<ReservedSeat> ReservedSeats { get; set; }
    }
}