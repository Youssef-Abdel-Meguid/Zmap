using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class ContactDetailsDto
    {
        public int? HotelId { get; set; }
        public int? CompanyId { get; set; }
        public int? StationId { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}