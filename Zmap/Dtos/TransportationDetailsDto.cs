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
        public Gallery Photo { get; set; }
    }
}