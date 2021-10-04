using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string ActicityName { get; set; }
        public string ActivityCategory { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Morning { get; set; }
        public bool? Evening { get; set; }
        public double? CostWithTrans { get; set; }
        public double? CostWithoutTrans { get; set; }
        public int? CityId { get; set; }
        public string City { get; set; }
        public int SubAreaId { get; set; }
        public string SubArea { get; set; }
        public int CompanyId { get; set; }
    }
}