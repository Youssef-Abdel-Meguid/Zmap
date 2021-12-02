using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class AddUserTripDetailDto
    {
        public int? UserId { get; set; }
        public int? UserTripId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}