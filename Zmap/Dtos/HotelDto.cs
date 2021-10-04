using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class HotelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Conditions { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string GoogleMapLocation { get; set; }
        public int CityId { get; set; }
        [DisplayName("Upload a photo")]
        public string PhotoUrl { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerPhonenumber { get; set; }
        public string City { get; set; }
    }
}