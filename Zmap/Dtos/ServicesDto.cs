using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class ServicesDto
    {
        public int id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public List<Service> Services { get; set; }
    }
}