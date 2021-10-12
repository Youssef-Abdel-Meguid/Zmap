using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class FilesDetailsDto
    {
        public int? HotelId { get; set; }
        public int? CompanyId { get; set; }
        public List<Attachment> Files { get; set; }
    }
}