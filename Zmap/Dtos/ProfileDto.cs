using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ProfileDto
    {
        public int? UserId { get; set; }
        public int CountOfBookings { get; set; }
        public int CountOfTrips { get; set; }
        public int CountOfPayments { get; set; }
        public List<BookingsDto> Bookings { get; set; }
        public List<TripsDto> Trips { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhotoUrl { get; set; }
    }
}