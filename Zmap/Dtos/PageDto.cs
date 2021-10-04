using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class PageDto
    {
        public ContactU ContactUs { get; set; }
        public AboutU AboutUs { get; set; }
        public OurService OurService { get; set; }

    }
}