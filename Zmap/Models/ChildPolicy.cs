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
    
    public partial class ChildPolicy
    {
        public int Id { get; set; }
        public Nullable<int> HotelId { get; set; }
        public Nullable<int> AgeFrom { get; set; }
        public Nullable<int> AgeTo { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedByUserId { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> AccommodationPercentage { get; set; }
        public Nullable<int> ChildPolicyCategoryId { get; set; }
    }
}
