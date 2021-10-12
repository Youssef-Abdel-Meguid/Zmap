using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class StationDetailsDto
    {
        public int? CompanyId { get; set; }
        public List<StationDto> Stations { get; set; }
    }
}