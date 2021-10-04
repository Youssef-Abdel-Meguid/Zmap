using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class StationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string GoogleMapLocation { get; set; }
        public int? CityId { get; set; }
        public string City { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<Gallery> Photos { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}