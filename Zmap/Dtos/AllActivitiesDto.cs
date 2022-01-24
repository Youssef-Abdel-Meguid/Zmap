using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class AllActivitiesDto
    {
        public List<ListAllActivitiesDto> Activities { get; set; }
        public Categories Categories { get; set; }
    }
}