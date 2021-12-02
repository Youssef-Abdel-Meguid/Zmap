using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class UserTripsDto
    {
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public int? UserTripId { get; set; }
        public List<TripsDto> Trips { get; set; }
    }
}