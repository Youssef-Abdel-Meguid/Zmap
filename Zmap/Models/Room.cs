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
    
    public partial class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public int HotleId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> CreatedByUserId { get; set; }
        public Nullable<int> ModifiedByUserId { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> RoomTypeId { get; set; }
        public Nullable<int> RoomViewId { get; set; }
    }
}