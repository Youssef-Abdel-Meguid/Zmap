using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class FAQDetailsDto
    {
        public int? HotelId { get; set; }
        public int? CompanyId { get; set; }
        public List<FAQ> FAQs { get; set; }
    }
}