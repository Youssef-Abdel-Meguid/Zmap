using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class LineDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string LineName { get; set; }
        public string LineStart { get; set; }
        public string LineEnd { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? HomeCityId { get; set; }
        public int? DestinationCityId { get; set; }
        public string Home { get; set; }
        public string Destination { get; set; }
        public List<LineBusDto> LineBuses { get; set; }
        public List<LineStationDto> LineStations { get; set; }
    }
}