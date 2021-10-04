﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ZmapEntities : DbContext
    {
        public ZmapEntities()
            : base("name=ZmapEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<ActivityCategory> ActivityCategories { get; set; }
        public virtual DbSet<AddsOn> AddsOns { get; set; }
        public virtual DbSet<BusAddOn> BusAddOns { get; set; }
        public virtual DbSet<BusCategory> BusCategories { get; set; }
        public virtual DbSet<Bus> Buses { get; set; }
        public virtual DbSet<BusSeatsMap> BusSeatsMaps { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<FAQ> FAQs { get; set; }
        public virtual DbSet<Gallery> Galleries { get; set; }
        public virtual DbSet<HotelAddsOn> HotelAddsOns { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<LineBus> LineBuses { get; set; }
        public virtual DbSet<LineStation> LineStations { get; set; }
        public virtual DbSet<PaymentStatu> PaymentStatus { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<RoomAvailability> RoomAvailabilities { get; set; }
        public virtual DbSet<RoomReservationCancellation> RoomReservationCancellations { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomStatu> RoomStatus { get; set; }
        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<TripType> TripTypes { get; set; }
        public virtual DbSet<UserHobby> UserHobbies { get; set; }
        public virtual DbSet<UserReservation> UserReservations { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserTripActivity> UserTripActivities { get; set; }
        public virtual DbSet<UserTrip> UserTrips { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<Accommodation> Accommodations { get; set; }
        public virtual DbSet<RoomAddsOn> RoomAddsOns { get; set; }
        public virtual DbSet<RoomType> RoomTypes { get; set; }
        public virtual DbSet<RoomView> RoomViews { get; set; }
        public virtual DbSet<ErrorLogger> ErrorLoggers { get; set; }
        public virtual DbSet<TransportationCompanyLine> TransportationCompanyLines { get; set; }
        public virtual DbSet<BusTripSchedule> BusTripSchedules { get; set; }
        public virtual DbSet<PostCategory> PostCategories { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<PostsCategory> PostsCategories { get; set; }
        public virtual DbSet<AboutU> AboutUs { get; set; }
        public virtual DbSet<ContactU> ContactUs { get; set; }
        public virtual DbSet<OurService> OurServices { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<SubArea> SubAreas { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
    }
}
