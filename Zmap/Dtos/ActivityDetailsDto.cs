using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ActivityDetailsDto
    {
        public int? CompanyId { get; set; }
        public List<ActivityDto> Activities { get; set; }
    }
}