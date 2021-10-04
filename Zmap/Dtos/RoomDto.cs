using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Zmap.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        [Display(Name = "اسم الغرقة")]
        public string Name { get; set; }
        [Display(Name = "رقم الغرقة")]
        public string Number { get; set; }
        [Display(Name = "وصف الغرقة")]
        public string Description { get; set; }
        public int HotleId { get; set; }
        [Display(Name = "نوع الغرقة")]
        public string RoomType { get; set; }
        [Display(Name = "نوع الاقامة")]
        public string AccommodationType { get; set; }
        [Display(Name = "فيو الغرقة")]
        public string RoomView { get; set; }
        [Display(Name = "سعر الليلة")]
        public double PricePerNight { get; set; }
        [Display(Name = "حالة الغرقة")]
        public string Status { get; set; }
        [Display(Name = "تاريخ الاضافة")]
        public DateTime? CreatedDate { get; set; }
    }
}
