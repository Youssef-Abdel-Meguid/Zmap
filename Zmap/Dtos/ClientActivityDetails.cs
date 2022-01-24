using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class ClientActivityDetails
    {
        public int CompanyId { get; set; }
        public int ActivityId { get; set; }
        public int ActivityCategoryId { get; set; }
        public string CompanyName { get; set; }
        public string ActivityCategoryName { get; set; }
        public string Description { get; set; }
        public string Sefty { get; set; }
        public List<string> Photos { get; set; }
        public List<ActivityAvailability> ActivityAvailabilities { get; set; }
    }
}