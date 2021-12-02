using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class TripsDto
    {
        public int? UserId { get; set; }
        public int UserTripId { get; set; }
        public int UserTripDetailId { get; set; }
        public string TripTitle { get; set; }
        public string TripDescription { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateFrom { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateTo { get; set; }
        public decimal? TotalCost { get; set; }
        public string Home { get; set; }
        public string Destination { get; set; }
        public int? TripDays { get; set; }
        public int? TripNights { get; set; }

    }
}