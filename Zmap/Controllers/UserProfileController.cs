using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zmap.Dtos;
using Zmap.Models;

namespace Zmap.Controllers
{
    public class UserProfileController : Controller
    {
        private ZmapEntities db = new ZmapEntities();

        private int? userType = 0;
        private int? userId = 0;

        private void SetIdenitiy()
        {
            if (Session["UserType"] != null)
            {
                string type = Session["UserType"] == null ? null : Session["UserType"].ToString();
                userType = int.Parse(type);

                string id = Session["UserId"] == null ? null : Session["UserId"].ToString();
                userId = int.Parse(id);
            }
        }

        public async Task<ActionResult> UserProfile(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            ProfileDto profileDto = new ProfileDto();

            try
            {
                var user = await db.Users.FindAsync(id);
                var userReservations = await db.UserReservations.Where(r => r.Active == true && r.UserId == id).ToListAsync();
                var userTrips = await db.UserTrips.Where(t => t.Active == true && t.UserId == id).ToListAsync();
                var userPayments = await db.UserPayments.Where(p => p.Active == true && p.UserId == id).ToListAsync();

                List<BookingsDto> bookingsDtos = new List<BookingsDto>();
                List<TripsDto> tripsDtos = new List<TripsDto>();

                foreach (var item in userReservations)
                {
                    var hotel = await db.Hotels.FindAsync(item.HotelId);
                    var transportationComapny = await db.Companies.Where(c => c.IsExternal == true && c.Id == item.TransportationCompanyId).FirstOrDefaultAsync();
                    var userPayment = await db.UserPayments.FindAsync(item.UserPaymentId);
                    var paymentStatus = await db.PaymentStatus.FindAsync(userPayment.PaymentStatusId);

                    bookingsDtos.Add(new BookingsDto()
                    {
                        IsPaid = paymentStatus.Id == 1,
                        Id = item.Id,
                        UserId = id,
                        DateFrom = item.DateFrom,
                        DateTo = item.DateTo,
                        HotelId = item.HotelId,
                        TotalCost = item.TotalCost,
                        TransportationCompanyId = item.TransportationCompanyId,
                        NumberOfSeats = item.NumberOfSeats,
                        PaymentStatus = paymentStatus.PaymentStatus,
                        HotelName = hotel.Name,
                        TransportationCompanyName = transportationComapny == null ? "" : transportationComapny.Name
                    });
                }

                foreach (var item in userTrips)
                {
                    tripsDtos.Add(new TripsDto()
                    {
                        UserId = id,
                        Home = item.Home,
                        Destination = item.Destination,
                        TotalCost = item.Cost,
                        DateFrom = item.DateFrom,
                        DateTo = item.DateTo,
                        TripDays = item.TripDays,
                        TripDescription = item.TripDescription,
                        TripNights = item.TripNights,
                        TripTitle = item.TripTitle,
                    });
                }

                profileDto = new ProfileDto()
                {
                    UserId = id,
                    Username = user.UserName,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    CountOfBookings = userReservations != null ? userReservations.Count() : 0,
                    CountOfTrips = userTrips != null ? userTrips.Count() : 0,
                    CountOfPayments = userPayments != null ? userPayments.Count() : 0,
                    Bookings = bookingsDtos,
                    Trips = tripsDtos,
                    ProfilePhotoUrl = user.ProfilePhotoUrl
                };

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }

            return View(profileDto);
        }

        public async Task<ActionResult> MyBookings(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            UserBookingsDto userBooking = new UserBookingsDto();

            try
            {
                var user = await db.Users.FindAsync(id);

                if(user == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "user is null", UserId = userId });
                }

                var userReservations = await db.UserReservations.Where(r => r.Active == true && r.UserId == id).ToListAsync();
                var userPayments = await db.UserPayments.Where(p => p.Active == true && p.UserId == id).ToListAsync();

                List<BookingsDto> bookingsDtos = new List<BookingsDto>();
                foreach (var item in userReservations)
                {
                    var hotel = await db.Hotels.FindAsync(item.HotelId);
                    var transportationComapny = await db.Companies.Where(c => c.IsExternal == true && c.Id == item.TransportationCompanyId).FirstOrDefaultAsync();
                    var userPayment = await db.UserPayments.FindAsync(item.UserPaymentId);
                    var paymentStatus = await db.PaymentStatus.FindAsync(userPayment.PaymentStatusId);

                    bookingsDtos.Add(new BookingsDto()
                    {
                        IsPaid = paymentStatus.Id == 1,
                        Id = item.Id,
                        UserId = id,
                        DateFrom = item.DateFrom,
                        DateTo = item.DateTo,
                        HotelId = item.HotelId,
                        TotalCost = item.TotalCost,
                        TransportationCompanyId = item.TransportationCompanyId,
                        NumberOfSeats = item.NumberOfSeats,
                        PaymentStatus = paymentStatus.PaymentStatus,
                        HotelName = hotel.Name,
                        TransportationCompanyName = transportationComapny == null ? "" : transportationComapny.Name
                    });
                }

                userBooking = new UserBookingsDto()
                {
                    UserId = id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Bookings = bookingsDtos,
                    ProfilePhotoUrl = user.ProfilePhotoUrl
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }
            return View(userBooking);
        }

        public async Task<ActionResult> MyProfile(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            UserProfileDto userProfileDto = new UserProfileDto();

            try
            {
                var user = await db.Users.FindAsync(id);

                if (user == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "user is null", UserId = userId });
                }

                int age = DateTime.Now.Year - user.BirthDate.Year;
                if (DateTime.Now < user.BirthDate.AddYears(age))
                    age--;

                userProfileDto = new UserProfileDto()
                {
                    UserId = id,
                    Age = age,
                    DateOfBirth = user.BirthDate.Date,
                    CreatedDate = user.CreatedDate,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    ProfilePhotoUrl = user.ProfilePhotoUrl
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }
            return View(userProfileDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(UserProfileDto userDto, HttpPostedFileBase uploadFile)
        {

            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || userDto.UserId != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            try
            {
                var user = await db.Users.FindAsync(userDto.UserId);
                
                if (uploadFile != null)
                {
                    var formattedFileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), uploadFile.FileName);
                    uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/users/" + formattedFileName));
                    user.ProfilePhotoUrl = formattedFileName;
                }

                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Email = userDto.Email;
                user.PhoneNumber = userDto.PhoneNumber;
                user.ModifiedDate = DateTime.Now;
                user.BirthDate = userDto.DateOfBirth;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("MyProfile", "UserProfile", new { id = userDto.UserId });
        }

        public async Task<ActionResult> MyTrips(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            UserTripsDto userTrip = new UserTripsDto();

            try
            {
                var user = await db.Users.FindAsync(id);

                if (user == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "user is null", UserId = userId });
                }
                var userTrips = await db.UserTrips.Where(t => t.Active == true && t.UserId == id).ToListAsync();

                List<TripsDto> tripsDtos = new List<TripsDto>();
                foreach (var item in userTrips)
                {
                    tripsDtos.Add(new TripsDto()
                    {
                        UserId = id,
                        UserTripId = item.Id,
                        Home = item.Home,
                        Destination = item.Destination,
                        TotalCost = item.Cost,
                        DateFrom = item.DateFrom,
                        DateTo = item.DateTo,
                        TripDays = item.TripDays,
                        TripDescription = item.TripDescription,
                        TripNights = item.TripNights,
                        TripTitle = item.TripTitle
                    });
                }

                userTrip = new UserTripsDto()
                {
                    UserId = id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Trips = tripsDtos,
                    ProfilePhotoUrl = user.ProfilePhotoUrl
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }
            return View(userTrip);
        }

        public async Task<ActionResult> MyTripDetails(int? id, int? userTripId)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if (id == null || userTripId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            UserTripsDto userTrip = new UserTripsDto();

            try
            {
                var user = await db.Users.FindAsync(id);
                var userTripDetails = await db.UserTripDetails.Where(d => d.Active == true && d.UserTripId == userTripId).ToListAsync();

                List<TripsDto> tripsDtos = new List<TripsDto>();
                foreach (var item in userTripDetails)
                {
                    tripsDtos.Add(new TripsDto()
                    {
                        UserId = id,
                        UserTripDetailId = item.Id,
                        TripDescription = item.Description,
                        TripTitle = item.Title,
                    });
                }

                userTrip = new UserTripsDto()
                {
                    UserId = id,
                    UserTripId = userTripId,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Trips = tripsDtos,
                    ProfilePhotoUrl = user.ProfilePhotoUrl
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }

            return View(userTrip);
        }

        public async Task<ActionResult> AddTripDetails(int? id, int? userTripId)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if (id == null || userTripId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            var user = new User();

            try
            {
                user = await db.Users.FindAsync(id);

                if (user == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "user is null", UserId = userId });
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }

            return View(
                new AddUserTripDetailDto() 
                { 
                    UserTripId = userTripId, 
                    UserId = id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    ProfilePhotoUrl = user.ProfilePhotoUrl
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddTripDetails(AddUserTripDetailDto userTripDetailDto)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || userTripDetailDto.UserId != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            try
            {
                if(!ModelState.IsValid)
                {
                    return View(userTripDetailDto);
                }

                var userTrip = await db.UserTrips.FindAsync(userTripDetailDto.UserTripId);

                if(userTrip == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "user trip is null", UserId = userId });
                }

                UserTripDetail userTripDetail = new UserTripDetail()
                {
                    Active = true,
                    CreatedDate = DateTime.Now,
                    Description = userTripDetailDto.Description,
                    Title = userTripDetailDto.Title,
                    UserTripId = userTripDetailDto.UserTripId
                };

                db.UserTripDetails.Add(userTripDetail);
                await db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("MyTripDetails", "UserProfile", new { id = userId, userTripId = userTripDetailDto.UserTripId });
        }

        public async Task<ActionResult> AddTrip(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            var user = new User();

            try
            {
                 user = await db.Users.FindAsync(id);

                if(user == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "user is null", UserId = userId });
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }
            return View(
                new AddUserTripDto() 
                { 
                    UserId = (int)id, 
                    FirstName = user.FirstName, 
                    LastName = user.LastName, 
                    UserName = user.UserName, 
                    ProfilePhotoUrl = user.ProfilePhotoUrl 
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddTrip(AddUserTripDto userTripDto, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || userTripDto.UserId != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            try
            {
                if(!ModelState.IsValid)
                {
                    return View(userTripDto);
                }

                UserTrip userTrip = new UserTrip()
                {
                    UserId = userTripDto.UserId,
                    Active = true,
                    Cost = userTripDto.TotalCost,
                    CreatedDate = DateTime.Now,
                    DateFrom = userTripDto.DateFrom,
                    DateTo = userTripDto.DateTo,
                    Destination = userTripDto.Destination,
                    Home = userTripDto.Home,
                    TripDays = userTripDto.NumberOfDays,
                    TripDescription = userTripDto.Description,
                    TripNights = userTripDto.NumberOfNights,
                    TripTitle = userTripDto.Title
                };

                if (uploadFile != null)
                {
                    var formattedFileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), uploadFile.FileName);
                    uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/UserTrips/" + formattedFileName));
                    userTrip.PhotoUrl = formattedFileName;
                }

                db.UserTrips.Add(userTrip);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("MyTrips", "UserProfile", new { id = userTripDto.UserId });
        }

        public async Task<ActionResult> DeleteUserTrip(int? id, int? userTripId)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if(id == null || userTripId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            try
            {
                var userTrip = await db.UserTrips.FindAsync(userTripId);

                if(userTrip == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
                }

                userTrip.ModifiedDate = DateTime.Now;
                userTrip.Active = false;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("MyTrips", "UserProfile", new { id = id });
        }

        public async Task<ActionResult> DeleteUserTripDetail(int? id, int? userTripDetailId)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5 || id != userId)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            if (id == null || userTripDetailId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
            }

            var userTripDetail = new UserTripDetail();

            try
            {
                userTripDetail = await db.UserTripDetails.FindAsync(userTripDetailId);

                if(userTripDetail == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "id is null", UserId = userId });
                }

                userTripDetail.Active = false;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("MyTripDetails", "UserProfile", new { id = id, userTripId = userTripDetail.UserTripId });
        }
    }
}