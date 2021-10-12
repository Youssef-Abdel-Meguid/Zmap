using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class BusDetailsDto
    {
        public int? CompanyId { get; set; }
        public List<BusDto> Buses { get; set; }
    }
}