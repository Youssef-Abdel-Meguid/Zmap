using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class TransportationDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string PrivacyPolicy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerPhonenumber { get; set; }
        public List<Gallery> Photos { get; set; }
        public List<LineDto> Lines{ get; set; }
        public List<Contact> Contacts { get; set; }
        public List<FAQ> FAQs { get; set; }
        public List<StationDto> Stations { get; set; }
        public List<BusDto> Buses { get; set; }
        public List<Attachment> Attachments { get; set; }

    }
}