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
    
    public partial class Gallery
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public Nullable<int> RoomId { get; set; }
        public Nullable<int> StationId { get; set; }
        public Nullable<int> BusId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> CreatedByUserId { get; set; }
        public Nullable<int> ModifiedByUserId { get; set; }
        public Nullable<int> HotelId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> IsMain { get; set; }
    }
}
