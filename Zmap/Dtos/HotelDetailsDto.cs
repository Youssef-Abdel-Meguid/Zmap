using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Zmap.DTOs;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class HotelDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
        public string GoogleMapLocation { get; set; }
        public string Description { get; set; }
        public string Conditions { get; set; }
        public string City { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerPhonenumber { get; set; }
        public Gallery Photo { get; set; }
    }
}