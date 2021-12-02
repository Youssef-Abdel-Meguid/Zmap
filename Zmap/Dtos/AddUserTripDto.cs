using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class AddUserTripDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Home { get; set; }
        public string Destination { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal TotalCost { get; set; }
        public int NumberOfDays { get; set; }
        public int NumberOfNights { get; set; }
    }
}