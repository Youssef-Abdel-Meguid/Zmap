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
    
    public partial class Service
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string PhotoUrl { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreateByUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedByUserId { get; set; }
        public Nullable<int> OurServiceId { get; set; }
    }
}