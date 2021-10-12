using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class LineStationDetailsDto
    {
        public int? LineId { get; set; }
        public List<LineStationDto> Stations { get; set; }
    }
}