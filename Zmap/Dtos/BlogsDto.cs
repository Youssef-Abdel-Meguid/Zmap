using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class BlogsDto
    {
        public List<BlogDetailsDto> BlogDetails { get; set; }
        public Categories Categories { get; set; }
    }
}