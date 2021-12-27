using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zmap.Dtos;
using Zmap.Models;

namespace Zmap.Controllers
{
    public class HomeController : Controller
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

        public async Task<ActionResult> Services()
        {
            var services = new ServicesDto();

            try
            {
                var ourService = await db.OurServices.FirstOrDefaultAsync();
                services = new ServicesDto()
                {
                    id = ourService.Id,
                    Title = ourService.Title,
                    Details = ourService.Details,
                    Services = await db.Services.Where(s => s.Active == true).ToListAsync()
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home service", Error = e.Message.ToString() });
            }

            return View(services);
        }

        private async Task<List<BlogDetailsDto>> GetBlogDetailsAsync()
        {
            List<BlogDetailsDto> blogs = new List<BlogDetailsDto>();
            var posts = await db.Posts.Where(p => p.Active == true).ToListAsync();

            foreach (var post in posts)
            {
                var cats = new List<PostCategory>();


                var user = await db.Users.FindAsync(post.CreatedByUserId);
                var postsCats = await db.PostsCategories.Where(p => p.Active == true && p.PostId == post.Id).ToListAsync();

                foreach (var cat in postsCats)
                    cats.Add(await db.PostCategories.FindAsync(cat.CategoryId));


                blogs.Add(new BlogDetailsDto()
                {
                    Id = post.Id,
                    CreatedDate = post.CreatedDate,
                    Title = post.Title,
                    Details = post.Details,
                    Username = user.UserName,
                    Categories = cats,
                    PhotoUrl = post.PostPhotoUrl,
                });
            }

            return blogs;
        }

        private async Task<Categories> GetCategoriesAsync()
        {
            var catList = new List<CategoryListDto>();
            var allCats = await db.PostCategories.Where(p => p.Active == true).ToListAsync();
            foreach (var cat in allCats)
            {
                catList.Add(new CategoryListDto()
                {
                    Id = cat.Id,
                    Name = cat.CategoryName,
                    IsChecked = false
                });
            }

            return new Categories() { CategorList = catList };
        }

        public async Task<ActionResult> Blogs()
        {
            BlogsDto blogsDto = new BlogsDto();
            try
            {
                blogsDto = new BlogsDto()
                {
                    BlogDetails = await GetBlogDetailsAsync(),
                    Categories = await GetCategoriesAsync()
                };

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home blogs", Error = e.Message.ToString() });
            }

            return View(blogsDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Blogs(Categories categories)
        {
            BlogsDto blogsDto = new BlogsDto();
            try
            {
                bool isIn = false;
                var allPosts = await db.Posts.Where(p => p.Active == true).ToListAsync();
                int maxIndex = 0;

                if(allPosts.Count > 0)
                    maxIndex = allPosts[allPosts.Count - 1].Id;

                bool[] visited = new bool[maxIndex + 1];

                for (int i = 0; i < visited.Length; i++)
                    visited[i] = false;

                List<BlogDetailsDto> blogDetails = new List<BlogDetailsDto>();
                foreach (var item in categories.CategorList)
                {
                    if(item.IsChecked)
                    {
                        isIn = true;
                        var catPosts = await db.PostsCategories.Where(p => p.Active == true && p.CategoryId == item.Id).ToListAsync();

                        foreach (var catPost in catPosts)
                        {
                            if(visited[(int)catPost.PostId] == false)
                            {
                                visited[(int)catPost.PostId] = true;

                                var post = await db.Posts.FindAsync(catPost.PostId);
                                var user = await db.Users.FindAsync(post.CreatedByUserId);
                                var allPostCats = await db.PostsCategories.Where(p => p.Active == true && p.PostId == post.Id).ToListAsync();
                                var postCategories = new List<PostCategory>();

                                foreach (var cat in allPostCats)
                                    postCategories.Add(await db.PostCategories.FindAsync(cat.CategoryId));

                                blogDetails.Add(new BlogDetailsDto() 
                                {
                                    Id = post.Id,
                                    Details = post.Details,
                                    PhotoUrl = post.PostPhotoUrl,
                                    Title = post.Title,
                                    Username = user.UserName,
                                    CreatedDate = post.CreatedDate,
                                    Categories = postCategories
                                });
                            }
                        }
                    }
                }

                if(isIn)
                {
                    blogsDto = new BlogsDto()
                    {
                        BlogDetails = blogDetails,
                        Categories = categories
                    };
                }
                else
                {
                    blogsDto = new BlogsDto()
                    {
                        BlogDetails = await GetBlogDetailsAsync(),
                        Categories = categories
                    };
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home blog categories", Error = e.Message.ToString()});
            }

            return View(blogsDto);
        }

        public async Task<ActionResult> Trips()
        {
            List<UserHomeTripDto> userHomeDto = new List<UserHomeTripDto>();
            try
            {
                var trips = await db.UserTrips.Where(t => t.Active == true).ToListAsync();

                foreach (var item in trips)
                {
                    var user = await db.Users.FindAsync(item.UserId);
                    if(user != null)
                    {
                        userHomeDto.Add(new UserHomeTripDto()
                        {
                            Cost = item.Cost,
                            CreatedBy = user.UserName,
                            Description = item.TripDescription,
                            Photo = item.PhotoUrl,
                            PlaceName = item.Destination,
                            Title = item.TripTitle,
                            UserTripId = item.Id
                        });
                    }
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home trips", Error = e.Message.ToString()});
            }
            return View(userHomeDto);
        }

        public async Task<ActionResult> TripDetails(int? id)
        {
            List<AddUserTripDetailDto> tripDetailDto = new List<AddUserTripDetailDto>();
            try
            {
                var userTripDetials = await db.UserTripDetails.Where(d => d.Active == true && d.UserTripId == id).ToListAsync();
                var trip = await db.UserTrips.FindAsync(id);
                var user = await db.Users.FindAsync(trip.UserId);

                if(user != null)
                {
                    foreach (var item in userTripDetials)
                    {
                        tripDetailDto.Add(new AddUserTripDetailDto()
                        {
                            Description = item.Description,
                            Title = item.Title,
                            UserId = user.Id,
                            UserTripId = item.UserTripId
                        });
                    }
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in trip details", Error = e.Message.ToString() });
            }

            return View(tripDetailDto);
        }

        public async Task<ActionResult> CreateSameTrip(int? userTripId)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error home create same trip", Error = "not authorized", UserId = userId });
            }

            if (userTripId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error home create same trip", Error = "id is null", UserId = userId });
            }

            try
            {
                var trip = await db.UserTrips.FindAsync(userTripId);

                if(trip == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error home create same trip", Error = "trip is null", UserId = userId });
                }

                var tripDetails = await db.UserTripDetails.Where(d => d.UserTripId == userTripId && d.Active == true).ToListAsync();

                UserTrip userTrip = new UserTrip()
                {
                    Active = true,
                    Cost = trip.Cost,
                    CreatedDate = DateTime.Now,
                    DateFrom = trip.DateFrom,
                    DateTo = trip.DateTo,
                    Destination = trip.Destination,
                    Home = trip.Home,
                    PhotoUrl = trip.PhotoUrl,
                    TripDays = trip.TripDays,
                    TripDescription = trip.TripDescription,
                    UserId = (int)userId,
                    TripTitle = trip.TripTitle,
                    TripNights = trip.TripNights
                };

                db.UserTrips.Add(userTrip);
                await db.SaveChangesAsync();

                foreach (var item in tripDetails)
                {
                    UserTripDetail userTripDetail = new UserTripDetail()
                    {
                        Active = true,
                        CreatedDate = DateTime.Now,
                        Description = item.Description,
                        Title = item.Title,
                        UserTripId = userTrip.Id
                    };
                    db.UserTripDetails.Add(userTripDetail);
                }

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home create same trip", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("TripDetails", "Home", new { id = userTripId });
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var about = new AboutU();
            try
            {
                about = await db.AboutUs.FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home about", Error = e.Message.ToString() });
            }
            return View(about);
        }

        public async Task<ActionResult> Contact()
        {

            var contact = new ContactU();
            try
            {
                contact = await db.ContactUs.FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home contact", Error = e.Message.ToString() });
            }
            return View(contact);
        }

        public async Task<ActionResult> TechnicalSupport(ErrorLogger errorLogger)
        {

            if (MvcApplication.isIn)
            {
                MvcApplication.isIn = false;
                return View();
            }

            if (string.IsNullOrEmpty(errorLogger.ActionName))
                return RedirectToAction("Index", "Home");

            try
            {
                errorLogger.CreatedDate = DateTime.Now;
                db.ErrorLoggers.Add(errorLogger);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return View();
            }

            return View();
        }

        public async Task<ActionResult> TripPlan(TripPlanDto tripPlan)
        {
            try
            {
                List<int> adults = new List<int>();
                adults.Add(1);
                adults.Add(2);
                adults.Add(3);
                adults.Add(4);
                adults.Add(5);

                List<int> child = new List<int>();
                child.Add(0);
                child.Add(1);
                child.Add(2);

                ViewBag.Cities = await db.Cities.ToListAsync();
                ViewBag.Adults = new SelectList(adults.ToList());
                ViewBag.Child = new SelectList(child.ToList());

                if(TempData["TripPlan"] != null)
                    tripPlan = (TripPlanDto)TempData["TripPlan"];

                if (tripPlan == null)
                    tripPlan = new TripPlanDto();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in trip paln", Error = e.Message.ToString() });
            }
            return View(tripPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TripPlanData(TripPlanDto tripPlanDto, bool isSelected )
        {
            try
            {
                TempData["TripPlan"] = tripPlanDto;

                if (isSelected == false)
                {
                    TimeSpan t = (DateTime)tripPlanDto.to - (DateTime)tripPlanDto.from;
                    int numberOfDays = t.Days;

                    var roomAvas = await db.RoomAvailabilities.Where(
                        a => a.Active == true &&
                        a.DateFrom <= tripPlanDto.from && a.DateTo >= tripPlanDto.to).ToListAsync();

                    var activityAvas = await db.ActivityAvailabilities.Where(
                        a => a.Active == true && a.DateTo >= tripPlanDto.to && a.DateFrom <= tripPlanDto.from).ToListAsync();

                    var lines = await db.TransportationCompanyLines.Where(
                        l => l.Active == true
                        && l.DestinationCityId == tripPlanDto.DestinationId
                        && l.HomeCityId == tripPlanDto.HomeId).ToListAsync();

                    List<TripHotelDto> tripHotelDtos = new List<TripHotelDto>();
                    List<TripActivityDto> tripActivityDtos = new List<TripActivityDto>();
                    List<TripTransportationDto> tripTransportationDtos = new List<TripTransportationDto>();

                    foreach (var item in lines)
                    {
                        var lineBuses = await db.LineBuses.Where(l => l.Active == true && l.LineId == item.Id).ToListAsync();
                        foreach (var item1 in lineBuses)
                        {
                            var busShe = await db.BusTripSchedules.Where(
                                b => b.Active == true
                                && b.BusId == item1.BusId
                                && b.DepartureDate == tripPlanDto.from).ToListAsync();

                            foreach (var item2 in busShe)
                            {
                                var stationStart = await db.Stations.FindAsync(item.LineStartStationId);
                                var stationEnd = await db.Stations.FindAsync(item.LineEndStationId);
                                var bus = await db.Buses.FindAsync(item2.BusId);
                                var busCat = await db.BusCategories.FindAsync(bus.CategoryId);
                                var company = await db.Companies.FindAsync(bus.CompanyId);

                                tripTransportationDtos.Add(new TripTransportationDto()
                                {
                                    From = item2.DepartureDate,
                                    To = item2.ArrivalDate,
                                    NumberOfSeats = tripPlanDto.NumberOfAdults,
                                    TotalCost = item1.SeatPrice * tripPlanDto.NumberOfAdults,
                                    StationFrom = stationStart.Name,
                                    StationTo = stationEnd.Name,
                                    BusName = bus.Name,
                                    BusCategory = busCat.CategoryName,
                                    CompanyName = company.Name,
                                    BusId = bus.Id,
                                    CompanyId = company.Id
                                });
                            }
                        }
                    }

                    foreach (var item in activityAvas)
                    {
                        var activity = await db.Activities.FindAsync(item.ActivityId);
                        var area = await db.SubAreas.FindAsync(activity.SubAreaId);
                        var company = await db.Companies.FindAsync(activity.CompanyId);

                        if (area.CityId == tripPlanDto.DestinationId)
                        {
                            tripActivityDtos.Add(new TripActivityDto()
                            {
                                ActivityName = activity.ActivityName,
                                Area = area.Name,
                                CompanyId = company.Id,
                                ActivtyId = activity.Id
                            });
                        }
                    }

                    foreach (var item in roomAvas)
                    {
                        var room = await db.Rooms.FindAsync(item.RoomId);
                        var hotel = await db.Hotels.FindAsync(room.HotleId);

                        if (hotel.CityId == tripPlanDto.DestinationId)
                        {
                            var acco = await db.Accommodations.FindAsync(item.AccommodationId);
                            var roomType = await db.RoomTypes.FindAsync(room.RoomTypeId);
                            var roomView = await db.RoomViews.FindAsync(room.RoomViewId);

                            tripHotelDtos.Add(new TripHotelDto()
                            {
                                TotalCost = item.PricePerNght * numberOfDays,
                                RoomAccommodation = acco.ArabicName,
                                RoomType = roomType.ArabicName,
                                RoomView = roomView.ArabicName,
                                HotelName = hotel.Name,
                                HotelId = hotel.Id,
                                RoomId = room.Id
                            });
                        }
                    }

                    tripPlanDto.TripActivities = tripActivityDtos;
                    tripPlanDto.TripHotels = tripHotelDtos;
                    tripPlanDto.TripTransportations = tripTransportationDtos;
                    tripPlanDto.IsSelected = false;

                    TempData["TripPlan"] = tripPlanDto;
                }
                else if(isSelected == true)
                {
                    TimeSpan t = (DateTime)tripPlanDto.to - (DateTime)tripPlanDto.from;
                    int numberOfDays = t.Days;

                    var roomAvas = await db.RoomAvailabilities.Where(
                       a => a.Active == true &&
                       a.DateFrom <= tripPlanDto.from && a.DateTo >= tripPlanDto.to).ToListAsync();

                    var activityAvas = await db.ActivityAvailabilities.Where(
                        a => a.Active == true && a.DateTo >= tripPlanDto.to && a.DateFrom <= tripPlanDto.from).ToListAsync();

                    var lines = await db.TransportationCompanyLines.Where(
                        l => l.Active == true
                        && l.DestinationCityId == tripPlanDto.DestinationId
                        && l.HomeCityId == tripPlanDto.HomeId).ToListAsync();

                    List<TripHotelDto> tripHotelDtos = new List<TripHotelDto>();
                    List<TripActivityDto> tripActivityDtos = new List<TripActivityDto>();
                    List<TripTransportationDto> tripTransportationDtos = new List<TripTransportationDto>();

                    foreach (var item in lines)
                    {
                        var lineBuses = await db.LineBuses.Where(l => l.Active == true && l.LineId == item.Id).ToListAsync();
                        foreach (var item1 in lineBuses)
                        {
                            var busShe = await db.BusTripSchedules.Where(
                                b => b.Active == true
                                && b.BusId == item1.BusId
                                && b.DepartureDate == tripPlanDto.from).ToListAsync();

                            foreach (var item2 in busShe)
                            {
                                var stationStart = await db.Stations.FindAsync(item.LineStartStationId);
                                var stationEnd = await db.Stations.FindAsync(item.LineEndStationId);
                                var bus = await db.Buses.FindAsync(item2.BusId);
                                var busCat = await db.BusCategories.FindAsync(bus.CategoryId);
                                var company = await db.Companies.FindAsync(bus.CompanyId);

                                tripTransportationDtos.Add(new TripTransportationDto()
                                {
                                    From = item2.DepartureDate,
                                    To = item2.ArrivalDate,
                                    NumberOfSeats = tripPlanDto.NumberOfAdults,
                                    TotalCost = item1.SeatPrice * tripPlanDto.NumberOfAdults,
                                    StationFrom = stationStart.Name,
                                    StationTo = stationEnd.Name,
                                    BusName = bus.Name,
                                    BusCategory = busCat.CategoryName,
                                    CompanyName = company.Name,
                                    CompanyId = company.Id,
                                    BusId = bus.Id
                                });
                            }
                        }
                    }

                    foreach (var item in activityAvas)
                    {
                        var activity = await db.Activities.FindAsync(item.ActivityId);
                        var area = await db.SubAreas.FindAsync(activity.SubAreaId);
                        var comapny = await db.Companies.FindAsync(activity.CompanyId);

                        if (area.CityId == tripPlanDto.DestinationId)
                        {
                            tripActivityDtos.Add(new TripActivityDto()
                            {
                                ActivityName = activity.ActivityName,
                                Area = area.Name,
                                CompanyId = comapny.Id,
                                ActivtyId = activity.Id
                            });
                        }
                    }

                    foreach (var item in roomAvas)
                    {
                        var room = await db.Rooms.FindAsync(item.RoomId);
                        var hotel = await db.Hotels.FindAsync(room.HotleId);

                        if (hotel.CityId == tripPlanDto.DestinationId)
                        {
                            var acco = await db.Accommodations.FindAsync(item.AccommodationId);
                            var roomType = await db.RoomTypes.FindAsync(room.RoomTypeId);
                            var roomView = await db.RoomViews.FindAsync(room.RoomViewId);

                            tripHotelDtos.Add(new TripHotelDto()
                            {
                                TotalCost = item.PricePerNght * numberOfDays,
                                RoomAccommodation = acco.ArabicName,
                                RoomType = roomType.ArabicName,
                                RoomView = roomView.ArabicName,
                                HotelName = hotel.Name,
                                HotelId = hotel.Id,
                                RoomId = room.Id
                            });
                        }
                    }

                    tripPlanDto.TripActivities = tripActivityDtos;
                    tripPlanDto.TripHotels = tripHotelDtos;
                    tripPlanDto.TripTransportations = tripTransportationDtos;
                    tripPlanDto.IsSelected = true;
                    tripPlanDto.TripHotelData = tripPlanDto.TripHotels[tripPlanDto.HotelId];
                    tripPlanDto.TripActivityData = tripPlanDto.TripActivities[tripPlanDto.ActivityId];
                    tripPlanDto.TransportationData = tripPlanDto.TripTransportations[tripPlanDto.TransportationId];
                    TempData["TripPlan"] = tripPlanDto;
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in trip paln", Error = e.Message.ToString() });
            }

            return RedirectToAction("TripPlan", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TripReservation(TripPlanDto tripPlanDto)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in user profile", Error = "not authorized", UserId = userId });
            }

            try
            {
                var userPayment = new UserPayment()
                {
                    UserId = userId,
                    Active = true,
                    CreatedDate = DateTime.Now,
                    PaymentStatusId = 2
                };

                db.UserPayments.Add(userPayment);
                db.SaveChanges();

                var userRes = new UserReservation()
                {
                    CreatedDate = DateTime.Now,
                    AccommodationCost = (double?)tripPlanDto.TripHotelData.TotalCost,
                    Active = true,
                    ActivityCompanyId = tripPlanDto.TripActivityData.CompanyId,
                    ActivityCost = tripPlanDto.TripActivityData.TotalCost,
                    BusId = tripPlanDto.TransportationData.BusId,
                    DateFrom = tripPlanDto.from,
                    DateTo = tripPlanDto.to,
                    HotelId = tripPlanDto.TripHotelData.HotelId,
                    NumberOfSeats = tripPlanDto.TransportationData.NumberOfSeats,
                    RoomId = tripPlanDto.TripHotelData.RoomId,
                    TotalCost = (double?)tripPlanDto.TripHotelData.TotalCost + tripPlanDto.TransportationData.TotalCost + tripPlanDto.TripActivityData.TotalCost,
                    TransportationCompanyId = tripPlanDto.TransportationData.CompanyId,
                    TransportationCost = tripPlanDto.TransportationData.TotalCost,
                    UserId = (int)userId,
                    UserPaymentId = userPayment.Id,
                };

                db.UserReservations.Add(userRes);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in trip paln", Error = e.InnerException.Message.ToString() });
            }

            return RedirectToAction("TripPlan", "Home");
        }

    }
}