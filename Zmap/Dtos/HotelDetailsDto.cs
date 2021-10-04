using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Zmap.DTOs;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class HotelDetailsDto
    {
        public int Id { get; set; }
        [Display(Name = "اسم الفندق")]
        public string Name { get; set; }
        [Display(Name = "الموقع")]
        public string WebsiteUrl { get; set; }
        [Display(Name = "خريطة جوجل")]
        public string GoogleMapLocation { get; set; }
        [Display(Name = "وصف الفندق")]
        public string Description { get; set; }
        [Display(Name = "شروط الفندق")]
        public string Conditions { get; set; }
        [Display(Name = "المدينة")]
        public string City { get; set; }
        [Display(Name = "تاريخ الاضافة")]
        public DateTime CreatedDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerPhonenumber { get; set; }
        public List<Gallery> Photos { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<FAQ> FAQs { get; set; }
        public List<AddOnDto> HotelAddsOn { get; set; }
        public List<RoomDto> Rooms { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}