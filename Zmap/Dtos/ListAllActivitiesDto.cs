using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ListAllActivitiesDto
    {
        public int CompanyId { get; set; }
        public int ActivityId { get; set; }
        public int ActivityCategoryId { get; set; }
        public string CompanyName { get; set; }
        public string ActivityCategoryName { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string PhotoUrl { get; set; }
    }
}