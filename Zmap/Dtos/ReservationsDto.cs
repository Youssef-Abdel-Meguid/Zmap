using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ReservationsDto
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? HotelId { get; set; }
        public int? RoomId { get; set; }
        public int? TransportationComapnyId { get; set; }
        public int? ActivityCompanyId { get; set; }
        public int? BusId { get; set; }
        public int? StationId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? RoomAvailibilityId { get; set; }
        public int? UserPaymentId { get; set; }
        public int? NumberOfAdults { get; set; }
        public int? NumberOfChilds { get; set; }
        public int? NumberOfSeats { get; set; }
        public double? TotalCost { get; set; }
        public double? AccommodationCost { get; set; }
        public double? TransportationCost { get; set; }
        public double? ActivityCost { get; set; }
        public int? UserId { get; set; }
        public string RoomView { get; set; }
        public string RoomType { get; set; }
        public string TransportationComapnyName { get; set; }
        public string HotelName { get; set; }
        public string Home { get; set; }
        public string Destination { get; set; }
        public string PaymentStatus { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserEmail { get; set; }
        public string ActivityCompanyName { get; set; }
    }
}