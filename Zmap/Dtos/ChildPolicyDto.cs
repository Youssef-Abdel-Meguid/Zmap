using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ChildPolicyDto
    {
        public int Id { get; set; }
        public decimal? PricePerNight { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public string ChildPolicyCategory { get; set; }
        public int? ChildPolicyCategoryId { get; set; }
    }
}