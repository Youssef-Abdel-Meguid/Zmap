using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class LineDetailsDto
    {
        public int? CompanyId { get; set; }
        public List<LineDto> Lines { get; set; }
    }
}