using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class LineBusDetailsDto
    {
        public int? LineId { get; set; }
        public List<LineBusDto> Buses { get; set; }
    }
}