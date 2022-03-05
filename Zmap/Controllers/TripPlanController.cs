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
    public class TripPlanController : Controller
    {
        private readonly ZmapEntities db = new ZmapEntities();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(TripPlanDto tripPlan)
        {
            try
            {
                if(tripPlan != null)
                {
                    var x = await db.Cities.FindAsync(tripPlan.DestinationId);
                    var y = await db.Cities.FindAsync(tripPlan.HomeId);
                    tripPlan.DestinationName = x.ArabicCityName;
                    tripPlan.HomeName = y.ArabicCityName;
                    tripPlan.Ages = new List<int>();
                    for (int i = 0; i < tripPlan.Child; i++)
                    {
                        tripPlan.Ages.Add(i);
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in trip paln", Error = e.Message.ToString() });
            }
            return View(tripPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TripPlanData(TripPlanDto tripPlanDto)
        {
            if(tripPlanDto == null)
            {
                return RedirectToAction("Index", "TripPlan");
            }
            var TripPlanDataDto = new TripPlanDataDto();
            try
            {
                #region Hotels
                var hotels = await db.Hotels.Where(h => h.Active == true && h.CityId == tripPlanDto.DestinationId).ToListAsync();
                List<TripHotelDto> tripHotelDtos = new List<TripHotelDto>();
                foreach (var hotel in hotels)
                {
                    var rooms = await db.Rooms.Where(r => r.Active == true && r.HotleId == hotel.Id).ToListAsync();
                    foreach (var room in rooms)
                    {
                        var roomsAva = await db.RoomAvailabilities.Where(r => r.Active == true && r.RoomId == room.Id &&
                        tripPlanDto.from >= r.DateFrom && tripPlanDto.to <= r.DateTo).ToListAsync();
                        var lockedDates = await db.LockedRooms.Where(r => r.Active == true && r.RoomId == room.Id &&
                        tripPlanDto.from >= r.DateFrom && tripPlanDto.to <= r.DateTo).ToListAsync();
                        foreach (var ava in roomsAva)
                        {
                            var locked = lockedDates.Where(l => ava.DateFrom >= l.DateFrom && ava.DateTo <= l.DateTo).FirstOrDefault();
                            if(locked == null)
                            {
                                var galleries = await db.Galleries.Where(g => g.Active == true && g.HotelId == hotel.Id).ToListAsync();
                                List<string> photoUrls = new List<string>();
                                foreach (var gallery in galleries)
                                {
                                    photoUrls.Add(gallery.PhotoUrl);
                                }
                                var roomType = await db.RoomTypes.FindAsync(room.RoomTypeId);
                                var roomView = await db.RoomViews.FindAsync(room.RoomViewId);
                                var roomAccommodation = await db.Accommodations.FindAsync(ava.AccommodationId);
                                decimal? totalCost = ava.PricePerNght;
                                if(tripPlanDto.Child != 0)
                                {
                                    var childPolicy = await db.ChildPolicies.Where(c => c.Active == true && c.RoomId == room.Id).ToListAsync();
                                    for (int i = 0; i < tripPlanDto.Ages.Count; i++)
                                    {
                                        var getPolicy = childPolicy.Where(c => tripPlanDto.Ages[i] >= c.AgeFrom 
                                        && tripPlanDto.Ages[i] <= c.AgeTo).FirstOrDefault();
                                        if(getPolicy != null)
                                            totalCost += (totalCost * getPolicy.PricePerNight) / 100;
                                    }
                                }
                                
                                tripHotelDtos.Add(new TripHotelDto()
                                {
                                    HotelId = hotel.Id,
                                    HotelName = hotel.Name,
                                    RoomAccommodation = roomAccommodation != null ? roomAccommodation.ArabicName : "",
                                    RoomId = room.Id,
                                    RoomType = roomType != null ? roomType.ArabicName : "",
                                    RoomView = roomView != null ? roomView.ArabicName : "",
                                    TotalCost = totalCost,
                                    PhotoUrl = photoUrls,
                                    RoomAvailibilityId = ava.Id
                                });
                            }
                        }
                    }
                }
                #endregion
                
                #region Activities
                var subareas = await db.SubAreas.Where(s => s.Active == true && s.CityId == tripPlanDto.DestinationId).ToListAsync();
                List<int> activitiesIds = new List<int>();
                List<TripActivityDto> tripActivityDtos = new List<TripActivityDto>();
                foreach (var item in subareas)
                {
                    var activities = await db.Activities.Where(a => a.Active == true && a.SubAreaId == item.Id).ToListAsync();
                    foreach (var item2 in activities)
                    {
                        activitiesIds.Add(item2.Id);
                    }
                }
                for (int i = 0; i < activitiesIds.Count; i++)
                {
                    var activityAva = await db.ActivityAvailabilities
                        .Where(a => a.Active == true && a.ActivityId == activitiesIds[i] && tripPlanDto.from >= a.DateFrom && tripPlanDto.to <= a.DateTo)
                        .ToListAsync();
                    var activityLockDates = await db.ActivityLockedDates
                        .Where(a => a.Active == true && a.ActivityId == activitiesIds[i] && tripPlanDto.from >= a.DateFrom && tripPlanDto.to <= a.DateTo)
                        .ToListAsync();

                    foreach (var item in activityAva)
                    {
                        var locked = activityLockDates.Where(a => item.DateFrom >= a.DateFrom && item.DateTo <= a.DateTo).FirstOrDefault();
                        if(locked == null)
                        {
                            var activiy = await db.Activities.FindAsync(item.ActivityId);
                            var activityCategory = await db.ActivityCategories.FindAsync(activiy.ActivityCategoryId);
                            var area = await db.SubAreas.FindAsync(activiy.SubAreaId);
                            var gallaries = await db.Galleries.Where(g => g.Active == true && g.ActivityId == item.ActivityId).ToListAsync();
                            List<string> photoUrls = new List<string>();
                            foreach (var gallery in gallaries)
                            {
                                photoUrls.Add(gallery.PhotoUrl);
                            }
                            tripActivityDtos.Add(new TripActivityDto()
                            {
                                ActivityCategory = activityCategory.CategoryName,
                                ActivityName = activiy.ActivityName,
                                ActivtyId = activiy.Id,
                                TotalCost = item.CostWithoutTrasnportation,
                                CompanyId = activiy.CompanyId,
                                DateFrom = item.DateFrom,
                                DateTo = item.DateTo,
                                Area = area.Name,
                                PhotoUrl = photoUrls
                            });
                        }
                    }
                }
                #endregion

                #region Transportations
                var transportationComapnyLines = await db.TransportationCompanyLines
                    .Where(t => t.Active == true && t.HomeCityId == tripPlanDto.HomeId 
                    && t.DestinationCityId == tripPlanDto.DestinationId).ToListAsync();
                List<TripTransportationDto> tripTransportationDtos = new List<TripTransportationDto>();
                foreach (var companyLines in transportationComapnyLines)
                {
                    var lineBuses = await db.LineBuses.Where(l => l.Active == true && l.LineId == companyLines.Id).ToListAsync();

                    foreach (var lineBus in lineBuses)
                    {
                        var bus = await db.Buses.FindAsync(lineBus.BusId);
                        var busTripSchedules = await db.BusTripSchedules
                            .Where(b => b.Active == true && b.BusId == lineBus.Id
                            && b.DepartureDate == tripPlanDto.from && b.ArrivalDate == tripPlanDto.to).ToListAsync();

                        foreach (var busTripSchedule in busTripSchedules)
                        {
                            var busSeatMap = await db.BusSeatsMaps.FindAsync(bus.SeatsMapId);
                            var reservedSeats = await db.ReservedSeats.Where(r => r.Active == true && r.BusTripId == busTripSchedule.Id).ToListAsync();
                            int remainingSeats = (int)busSeatMap.NumberOfSeats - reservedSeats.Count();
                            if(remainingSeats >= tripPlanDto.Adults + tripPlanDto.Child)
                            {
                                var company = await db.Companies.FindAsync(companyLines.CompanyId);
                                var busCategory = await db.BusCategories.FindAsync(bus.CategoryId);
                                var stationStart = await db.Stations.FindAsync(companyLines.LineStartStationId);
                                var stationEnd = await db.Stations.FindAsync(companyLines.LineEndStationId);
                                double totalCost = lineBus.SeatPrice * (tripPlanDto.Adults + tripPlanDto.Child);
                                var galleries = await db.Galleries.Where(g => g.Active == true && 
                                g.StationId == stationStart.id || g.StationId == stationEnd.id || g.BusId == busTripSchedule.BusId).ToListAsync();
                                List<string> photoUrls = new List<string>();
                                foreach (var gallery in galleries)
                                {
                                    photoUrls.Add(gallery.PhotoUrl);
                                }
                                tripTransportationDtos.Add(new TripTransportationDto()
                                {
                                    BusId = busTripSchedule.BusId,
                                    BusName = bus.BusNumber,
                                    CompanyId = companyLines.CompanyId,
                                    CompanyName = company.Name,
                                    NumberOfSeats = tripPlanDto.Adults + tripPlanDto.Child,
                                    LineName = companyLines.LineName,
                                    LineId = companyLines.Id,
                                    BusCategory = busCategory.CategoryName,
                                    From = (DateTime)tripPlanDto.from,
                                    To = (DateTime)tripPlanDto.to,
                                    StationFrom = stationStart.Name,
                                    StationTo = stationEnd.Name,
                                    StationFromId = stationStart.id,
                                    StationToId = stationEnd.id,
                                    TotalCost = totalCost,
                                    PhotoUrl = photoUrls
                                });
                            }
                        }
                    }
                }
                #endregion

                TripPlanDataDto = new TripPlanDataDto()
                {
                    TripActivities = tripActivityDtos,
                    TripHotels = tripHotelDtos,
                    TripTransportations = tripTransportationDtos,
                    Adults = tripPlanDto.Adults,
                    Ages = tripPlanDto.Ages,
                    Child = tripPlanDto.Child,
                    DestinationId = tripPlanDto.DestinationId,
                    DestinationName = tripPlanDto.DestinationName,
                    from = tripPlanDto.from,
                    HomeId = tripPlanDto.HomeId,
                    HomeName = tripPlanDto.HomeName,
                    to = tripPlanDto.to
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in trip paln", Error = e.Message.ToString() });
            }

            return View(TripPlanDataDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TripReservation(SelectedTripPlanDto selectedTripPlan)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in trip plan", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (selectedTripPlan == null) 
                {
                    return RedirectToAction("TripPlanData", "TripPlan");
                }

                #region ReserveHotels
                foreach (var hotels in selectedTripPlan.TripHotels)
                {
                    UserPayment userPayment = new UserPayment() 
                    {
                        Active = true,
                        CreatedDate = DateTime.Now,
                        UserId = userId,
                        PaymentStatusId = 2
                    };

                    db.UserPayments.Add(userPayment);
                    db.SaveChanges();

                    UserReservation userReservation = new UserReservation()
                    {
                        AccommodationCost = (double?)hotels.TotalCost,
                        RoomAvailabilityId = hotels.RoomAvailibilityId,
                        Active = true,
                        ApprovedByAdmin = false,
                        CreatedDate = DateTime.Now,
                        UserId = (int)userId,
                        DateFrom = selectedTripPlan.from,
                        DateTo = selectedTripPlan.to,
                        NumberOfAdults = selectedTripPlan.Adults,
                        RoomId = hotels.RoomId,
                        UserPaymentId = userPayment.Id,
                        UserConfirmation = false,
                        HotelId = hotels.HotelId,
                        NumberOfSeats = selectedTripPlan.Adults + selectedTripPlan.Child
                    };

                    db.UserReservations.Add(userReservation);
                    db.SaveChanges();
                }
                #endregion

                #region ReserveActivities
                foreach (var activity in selectedTripPlan.TripActivities)
                {
                    UserPayment userPayment = new UserPayment()
                    {
                        Active = true,
                        CreatedDate = DateTime.Now,
                        UserId = userId,
                        PaymentStatusId = 2
                    };

                    db.UserPayments.Add(userPayment);
                    db.SaveChanges();

                    UserReservation userReservation = new UserReservation()
                    {
                        Active = true,
                        ApprovedByAdmin = false,
                        CreatedDate = DateTime.Now,
                        UserId = (int)userId,
                        DateFrom = selectedTripPlan.from,
                        DateTo = selectedTripPlan.to,
                        NumberOfAdults = selectedTripPlan.Adults,
                        UserPaymentId = userPayment.Id,
                        UserConfirmation = false,
                        NumberOfSeats = selectedTripPlan.Adults + selectedTripPlan.Child,
                        ActivityCompanyId = activity.CompanyId,
                        ActivityCost =(double?)activity.TotalCost,
                    };

                    db.UserReservations.Add(userReservation);
                    db.SaveChanges();
                }
                #endregion

                #region ReserveTransportation
                foreach (var transportation in selectedTripPlan.TripTransportations)
                {
                    UserPayment userPayment = new UserPayment()
                    {
                        Active = true,
                        CreatedDate = DateTime.Now,
                        UserId = userId,
                        PaymentStatusId = 2
                    };

                    db.UserPayments.Add(userPayment);
                    db.SaveChanges();

                    UserReservation userReservation = new UserReservation()
                    {
                        Active = true,
                        ApprovedByAdmin = false,
                        CreatedDate = DateTime.Now,
                        UserId = (int)userId,
                        DateFrom = selectedTripPlan.from,
                        DateTo = selectedTripPlan.to,
                        NumberOfAdults = selectedTripPlan.Adults,
                        UserPaymentId = userPayment.Id,
                        UserConfirmation = false,
                        NumberOfSeats = selectedTripPlan.Adults + selectedTripPlan.Child,
                        TransportationCost = (double?) transportation.TotalCost,
                        TransportationCompanyId = transportation.CompanyId,
                        StationId = transportation.StationFromId,
                        BusId = transportation.BusId,
                    };

                    db.UserReservations.Add(userReservation);
                    db.SaveChanges();
                }
                #endregion
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in trip paln", Error = e.InnerException.Message.ToString() });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}