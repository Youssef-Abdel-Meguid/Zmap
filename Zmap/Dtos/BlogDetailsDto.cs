using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class BlogDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string Username { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<PostCategory> Categories { get; set; }
    }
}