//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zmap.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserTrip
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TripTitle { get; set; }
        public string TripDescription { get; set; }
        public string Home { get; set; }
        public string Destination { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<int> TripDays { get; set; }
        public Nullable<int> TripNights { get; set; }
        public Nullable<bool> Active { get; set; }
        public string PhotoUrl { get; set; }
    }
}
