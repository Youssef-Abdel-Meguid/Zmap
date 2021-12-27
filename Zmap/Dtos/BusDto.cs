using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class BusDto
    {
        public int Id { get; set; }
        public int LineId { get; set; }
        public string Name { get; set; }
        public string BusNumber { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int BusSeatMapId { get; set; }
        public string BusSeatMap { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Gallery Photo { get; set; }
    }
}