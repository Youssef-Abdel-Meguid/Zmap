using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class LineStationDto
    {
        public int Id { get; set; }
        public int StoppingByOrder { get; set; }
        public int LineId { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; }
    }
}