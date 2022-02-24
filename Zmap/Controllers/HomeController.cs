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

        private async Task<Categories> GetBlogCategoriesAsync()
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

        private async Task<Categories> GetActivityCategoriesAsync()
        {
            var catList = new List<CategoryListDto>();
            var allCats = await db.ActivityCategories.Where(p => p.Active == true).ToListAsync();
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
                    Categories = await GetBlogCategoriesAsync()
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

                if (allPosts.Count > 0)
                    maxIndex = allPosts[allPosts.Count - 1].Id;

                bool[] visited = new bool[maxIndex + 1];

                for (int i = 0; i < visited.Length; i++)
                    visited[i] = false;

                List<BlogDetailsDto> blogDetails = new List<BlogDetailsDto>();
                foreach (var item in categories.CategorList)
                {
                    if (item.IsChecked)
                    {
                        isIn = true;
                        var catPosts = await db.PostsCategories.Where(p => p.Active == true && p.CategoryId == item.Id).ToListAsync();

                        foreach (var catPost in catPosts)
                        {
                            if (visited[(int)catPost.PostId] == false)
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

                if (isIn)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home blog categories", Error = e.Message.ToString() });
            }

            return View(blogsDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Activities(Categories categories)
        {
            AllActivitiesDto allActivitiesDto = new AllActivitiesDto();

            try
            {
                bool isIn = false;
                var activities = await GetActiviesAsync();
                List<ListAllActivitiesDto> selectedList = new List<ListAllActivitiesDto>();

                foreach (var item in categories.CategorList)
                {
                    if (item.IsChecked)
                    {
                        isIn = true;
                        foreach (var item2 in activities)
                        {
                            if (item.Id == item2.ActivityCategoryId)
                            {
                                selectedList.Add(item2);
                            }
                        }
                    }
                }

                if (isIn == false)
                {
                    allActivitiesDto = new AllActivitiesDto()
                    {
                        Activities = await GetActiviesAsync(),
                        Categories = await GetActivityCategoriesAsync()
                    };
                }
                else
                {
                    allActivitiesDto = new AllActivitiesDto()
                    {
                        Activities = selectedList,
                        Categories = await GetActivityCategoriesAsync()
                    };
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activity categories", Error = e.Message.ToString() });
            }

            return View(allActivitiesDto);
        }

        public async Task<List<ListAllActivitiesDto>> GetActiviesAsync()
        {
            var activityCategories = await db.ActivityCategories.Where(a => a.Active == true).ToListAsync();
            var activites = await db.Activities.Where(a => a.Active == true).ToListAsync();

            List<ListAllActivitiesDto> listAllActivitiesDtos = new List<ListAllActivitiesDto>();

            foreach (var item in activites)
            {
                var area = await db.SubAreas.FindAsync(item.SubAreaId);
                var city = await db.Cities.FindAsync(area.CityId);
                var company = await db.Companies.FindAsync(item.CompanyId);
                var category = await db.ActivityCategories.FindAsync(item.ActivityCategoryId);
                var gallery = await db.Galleries.Where(g => g.Active == true && g.ActivityId == item.Id).FirstOrDefaultAsync();

                listAllActivitiesDtos.Add(new ListAllActivitiesDto()
                {
                    ActivityCategoryName = category == null ? "" : category.CategoryName,
                    ActivityId = item.Id,
                    Area = area == null ? "" : area.Name,
                    City = city == null ? "" : city.ArabicCityName,
                    CompanyId = item.CompanyId,
                    PhotoUrl = gallery == null ? "" : gallery.PhotoUrl,
                    CompanyName = company == null ? "" : company.Name,
                    ActivityCategoryId = category == null ? 0 : category.Id
                });
            }

            return listAllActivitiesDtos;
        }

        public async Task<ActionResult> Activities()
        {
            AllActivitiesDto activitiesDtos = new AllActivitiesDto();
            try
            {
                activitiesDtos = new AllActivitiesDto()
                {
                    Categories = await GetActivityCategoriesAsync(),
                    Activities = await GetActiviesAsync()
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in list all activities", Error = e.InnerException.Message.ToString() });
            }

            return View(activitiesDtos);
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
                    if (user != null)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home trips", Error = e.Message.ToString() });
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

                if (user != null)
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

                if (trip == null)
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

        public async Task<ActionResult> Index()
        {
            SetIdenitiy();

            var homeDto = new HomeDataDto();

            try
            {
                if (userType != 0 && userType != 5)
                {
                    var userReservations = await db.UserReservations.Where(u => u.Active == true).ToListAsync();
                    List<ReservationsDto> reservationsDtos = new List<ReservationsDto>();

                    foreach (var item in userReservations)
                    {
                        var user = await db.Users.FindAsync(item.UserId);

                        var hotel = new Hotel();
                        var room = new Room();
                        var roomType = new RoomType();
                        var roomView = new RoomView();
                        var transportationCompany = new Company();
                        var station = new Station();
                        var activityCompany = new Company();

                        if (userType == 2 || userType == 1)
                        {
                            if (userType == 1)
                            {
                                hotel = await db.Hotels.FindAsync(item.HotelId);
                                room = await db.Rooms.FindAsync(item.RoomId);
                            }
                            else
                            {
                                hotel = await db.Hotels.Where(h => h.Id == item.Id && h.CreateByUserId == userId).FirstOrDefaultAsync();
                                room = await db.Rooms.Where(r => r.Id == item.RoomId && r.CreatedByUserId == userId).FirstOrDefaultAsync();
                            }

                            if (room != null)
                            {
                                roomType = await db.RoomTypes.FindAsync(room.RoomTypeId);
                                roomView = await db.RoomViews.FindAsync(room.RoomViewId);
                            }
                        }

                        if (userType == 3 || userType == 1)
                        {
                            if (userType == 1)
                            {
                                transportationCompany = await db.Companies.FindAsync(item.TransportationCompanyId);
                                station = await db.Stations.FindAsync(item.StationId);
                            }
                            else
                            {
                                transportationCompany = await db.Companies.Where(c => c.Id == item.TransportationCompanyId && c.CreatedByUserId == userId).FirstOrDefaultAsync();
                                station = await db.Stations.Where(s => s.id == item.StationId && s.CreatedByUserId == userId).FirstOrDefaultAsync();
                            }
                        }

                        if (userId == 4 || userType == 1)
                        {
                            if (userType == 1)
                            {
                                activityCompany = await db.Companies.FindAsync(item.ActivityCompanyId);
                            }
                            else
                            {
                                activityCompany = await db.Companies.Where(a => a.Id == item.ActivityCompanyId && a.CreatedByUserId == userId).FirstOrDefaultAsync();
                            }
                        }

                        var userPayment = await db.UserPayments.FindAsync(item.UserPaymentId);
                        var paymentStatus = new PaymentStatu();

                        if (userPayment != null)
                        {
                            paymentStatus = await db.PaymentStatus.FindAsync(userPayment.PaymentStatusId);
                        }

                        reservationsDtos.Add(new ReservationsDto
                        {
                            UserId = item.UserId,
                            UserFirstName = user != null ? user.FirstName : "",
                            UserLastName = user != null ? user.LastName : "",
                            UserEmail = user != null ? user.Email : "",
                            UserPhoneNumber = user != null ? user.PhoneNumber : "",
                            AccommodationCost = item.AccommodationCost,
                            ActivityCompanyId = item.ActivityCompanyId,
                            ActivityCost = item.ActivityCost,
                            BusId = item.BusId,
                            CreatedDate = item.CreatedDate,
                            DateFrom = item.DateFrom,
                            DateTo = item.DateTo,
                            HotelId = item.HotelId,
                            NumberOfAdults = item.NumberOfAdults,
                            NumberOfChilds = item.NumberOfAdults,
                            NumberOfSeats = item.NumberOfSeats,
                            RoomId = item.RoomId,
                            UserPaymentId = item.UserPaymentId,
                            TransportationCost = item.TransportationCost,
                            TransportationComapnyId = item.TransportationCompanyId,
                            RoomAvailibilityId = item.RoomAvailabilityId,
                            Id = item.Id,
                            TotalCost = item.TotalCost,
                            StationId = item.StationId,
                            RoomType = roomType != null ? roomType.ArabicName : "",
                            RoomView = roomView != null ? roomView.ArabicName : "",
                            HotelName = hotel != null ? hotel.Name : "",
                            TransportationComapnyName = transportationCompany != null ? transportationCompany.Name : "",
                            PaymentStatus = paymentStatus != null ? paymentStatus.PaymentStatus : "",
                            ActivityCompanyName = activityCompany != null ? activityCompany.Name : ""
                        });
                    }

                    homeDto.ReservationsDtos = reservationsDtos;
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in home index", Error = e.Message.ToString(), UserId = userId });
            }

            return View(homeDto);
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

                if (TempData["TripPlan"] != null)
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
        public async Task<ActionResult> TripPlanData(TripPlanDto tripPlanDto, bool isSelected)
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
                                var busPhoto = await db.Galleries.Where(g => g.Active == true && g.BusId == bus.Id).FirstOrDefaultAsync();

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
                                    CompanyId = company.Id,
                                    BusPhotoUrl = busPhoto == null ? "" : busPhoto.PhotoUrl
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
                            var photo = await db.Galleries.Where(g => g.Active == true && g.CompanyId == company.Id).FirstOrDefaultAsync();
                            tripActivityDtos.Add(new TripActivityDto()
                            {
                                ActivityName = activity.ActivityName,
                                Area = area.Name,
                                CompanyId = company.Id,
                                ActivtyId = activity.Id,
                                PhotoUrl = photo == null ? "" : photo.PhotoUrl
                            });
                        }
                    }

                    foreach (var item in roomAvas)
                    {
                        var room = await db.Rooms.Where(r => r.Active == true && r.Id == item.RoomId && r.RoomTypeId == 18 || r.RoomTypeId == 17).FirstOrDefaultAsync();
                        var hotel = await db.Hotels.FindAsync(room.HotleId);

                        if (hotel.CityId == tripPlanDto.DestinationId)
                        {
                            var acco = await db.Accommodations.FindAsync(item.AccommodationId);
                            var roomType = await db.RoomTypes.FindAsync(room.RoomTypeId);
                            var roomView = await db.RoomViews.FindAsync(room.RoomViewId);
                            var photo = await db.Galleries.Where(g => g.HotelId == hotel.Id && g.Active == true).FirstOrDefaultAsync();

                            tripHotelDtos.Add(new TripHotelDto()
                            {
                                TotalCost = item.PricePerNght * numberOfDays,
                                RoomAccommodation = acco.ArabicName,
                                RoomType = roomType.ArabicName,
                                RoomView = roomView.ArabicName,
                                HotelName = hotel.Name,
                                HotelId = hotel.Id,
                                RoomId = room.Id,
                                PhotoUrl = photo == null ? "" : photo.PhotoUrl
                            });
                        }
                    }

                    tripPlanDto.TripActivities = tripActivityDtos;
                    tripPlanDto.TripHotels = tripHotelDtos;
                    tripPlanDto.TripTransportations = tripTransportationDtos;
                    tripPlanDto.IsSelected = false;

                    TempData["TripPlan"] = tripPlanDto;
                }
                else if (isSelected == true)
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
                                var busPhoto = await db.Galleries.Where(g => g.Active == true && g.BusId == bus.Id).FirstOrDefaultAsync();

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
                                    CompanyId = company.Id,
                                    BusPhotoUrl = busPhoto == null ? "" : busPhoto.PhotoUrl
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
                            var photo = await db.Galleries.Where(g => g.Active == true && g.CompanyId == company.Id).FirstOrDefaultAsync();
                            tripActivityDtos.Add(new TripActivityDto()
                            {
                                ActivityName = activity.ActivityName,
                                Area = area.Name,
                                CompanyId = company.Id,
                                ActivtyId = activity.Id,
                                PhotoUrl = photo == null ? "" : photo.PhotoUrl
                            });
                        }
                    }

                    foreach (var item in roomAvas)
                    {
                        var room = await db.Rooms.Where(r => r.Active == true && r.Id == item.RoomId && r.RoomTypeId == 18 || r.RoomTypeId == 17).FirstOrDefaultAsync();
                        var hotel = await db.Hotels.FindAsync(room.HotleId);

                        if (hotel.CityId == tripPlanDto.DestinationId)
                        {
                            var acco = await db.Accommodations.FindAsync(item.AccommodationId);
                            var roomType = await db.RoomTypes.FindAsync(room.RoomTypeId);
                            var roomView = await db.RoomViews.FindAsync(room.RoomViewId);
                            var photo = await db.Galleries.Where(g => g.HotelId == hotel.Id && g.Active == true).FirstOrDefaultAsync();

                            tripHotelDtos.Add(new TripHotelDto()
                            {
                                TotalCost = item.PricePerNght * numberOfDays,
                                RoomAccommodation = acco.ArabicName,
                                RoomType = roomType.ArabicName,
                                RoomView = roomView.ArabicName,
                                HotelName = hotel.Name,
                                HotelId = hotel.Id,
                                RoomId = room.Id,
                                PhotoUrl = photo == null ? "" : photo.PhotoUrl
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

        public async Task<ActionResult> Hotels()
        {

            List<ListAllHotelsDto> hotelsDtos = new List<ListAllHotelsDto>();

            try
            {
                var hotels = await db.Hotels.Where(h => h.Active == true).ToListAsync();

                foreach (var item in hotels)
                {
                    var photo = await db.Galleries.Where(g => g.Active == true && g.HotelId == item.Id).FirstOrDefaultAsync();

                    hotelsDtos.Add(new ListAllHotelsDto()
                    {
                        Name = item.Name,
                        HotelId = item.Id,
                        City = item.City,
                        PhotoUrl = photo == null ? "" : photo.PhotoUrl
                    });
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.InnerException.Message.ToString() });
            }

            return View(hotelsDtos);
        }

        public async Task<ActionResult> HotelDetails(int? hotelId)
        {
            if (hotelId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel details", Error = "hotel id is null", UserId = userId });
            }

            ClientHotelDetailsDto hotelDetailsDto = new ClientHotelDetailsDto();
            try
            {
                var hotel = await db.Hotels.FindAsync(hotelId);
                var photos = await db.Galleries.Where(g => g.Active == true && g.HotelId == hotelId).ToListAsync();
                var hotelAddsOn = await db.HotelAddsOns.Where(a => a.Active == true && a.HotelId == hotelId).ToListAsync();
                var rooms = await db.Rooms.Where(r => r.Active == true && r.HotleId == hotelId).ToListAsync();

                List<string> photoUrls = new List<string>();
                List<string> addsOnName = new List<string>();
                List<ClientListAllRoomsDto> roomsDtos = new List<ClientListAllRoomsDto>();

                if (photos != null)
                {
                    foreach (var item in photos)
                    {
                        photoUrls.Add(item.PhotoUrl);
                    }
                }
                else
                {
                    photoUrls.Add("bg.jpg");
                }

                foreach (var item in hotelAddsOn)
                {
                    var addOn = await db.AddsOns.FindAsync(item.AddOnId);

                    if (addOn != null)
                        addsOnName.Add(addOn.ArabicName);
                }

                foreach (var item in rooms)
                {
                    var roomView = await db.RoomViews.FindAsync(item.RoomViewId);
                    var roomType = await db.RoomTypes.FindAsync(item.RoomTypeId);
                    var photo = await db.Galleries.Where(g => g.Active == true && g.RoomId == item.Id).FirstOrDefaultAsync();
                    var roomAva = await db.RoomAvailabilities.Where(r => r.Active == true && r.RoomId == item.Id).ToListAsync();
                    var roomAddsOn = await db.RoomAddsOns.Where(r => r.Active == true && r.RoomId == item.Id).ToListAsync();
                    List<string> roomAddOnNames = new List<string>();

                    for (int i = 0; i < roomAddsOn.Count; i++)
                    {
                        var addOn = await db.AddsOns.FindAsync(roomAddsOn[i].AddOnId);
                        if (addOn != null)
                        {
                            roomAddOnNames.Add(addOn.ArabicName);
                            int tmp = i + 1;
                            if (tmp < roomAddsOn.Count)
                            {
                                roomAddOnNames.Add(", ");
                            }
                        }
                    }

                    foreach (var item2 in roomAva)
                    {
                        var roomAcc = await db.Accommodations.FindAsync(item2.AccommodationId);
                        roomsDtos.Add(new ClientListAllRoomsDto()
                        {
                            RoomView = roomView == null ? "" : roomView.ArabicName,
                            RoomType = roomType == null ? "" : roomType.ArabicName,
                            Photo = photo == null ? "" : photo.PhotoUrl,
                            RoomId = item.Id,
                            PricePerNight = item2.PricePerNght,
                            Accommodation = roomAcc.ArabicName,
                            AddsOn = roomAddOnNames,
                            DateFrom = item2.DateFrom,
                            DateTo = item2.DateTo,
                            RoomAvaId = item2.Id
                        });
                    }
                }


                hotelDetailsDto = new ClientHotelDetailsDto()
                {
                    HotelId = (int)hotelId,
                    Description = hotel.Description,
                    Name = hotel.Name,
                    AddsOn = addsOnName,
                    Photos = photoUrls,
                    Rooms = roomsDtos
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel details", Error = e.InnerException.Message.ToString() });
            }

            return View(hotelDetailsDto);
        }

        public ActionResult RoomBooking(int? hotelId, int? roomId, int? roomAvaId)
        {
            if (hotelId == null || roomId == null || roomAvaId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in room booking", Error = "hotel id is null or room id is null", UserId = userId });
            }

            return View(new UserReservation() { HotelId = hotelId, RoomId = roomId, RoomAvailabilityId = roomAvaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoomBooking(UserReservation userReservation)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 5)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in room booking", Error = "not authorized", UserId = userId });
            }

            try
            {
                UserPayment userPayment = new UserPayment()
                {
                    Active = true,
                    CreatedDate = DateTime.Now,
                    UserId = (int)userId,
                    PaymentStatusId = 2
                };

                db.UserPayments.Add(userPayment);
                await db.SaveChangesAsync();

                userReservation.UserId = (int)userId;
                userReservation.CreatedDate = DateTime.Now;
                userReservation.Active = true;
                userReservation.ApprovedByAdmin = false;
                userReservation.UserPaymentId = userPayment.Id;
                //userReservation.NumberOfSeats = userReservation.NumberOfAdults + userReservation.NumberOfChilderLessThanSix + userReservation.NumberOfChildernBetweenSixAndTwelve;

                db.UserReservations.Add(userReservation);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in room booking", Error = e.InnerException.Message.ToString() });
            }

            return RedirectToAction("MyBookings", "UserProfile", new { id = userId });
        }

        public async Task<ActionResult> ActivityDetails(int? companyId, int? activityId)
        {
            if (companyId == null || activityId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activity details", Error = "company id or activity id is null", UserId = userId });
            }

            ClientActivityDetails clientActivityDetails = new ClientActivityDetails();

            try
            {
                var activity = await db.Activities.FindAsync(activityId);

                if(activity == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activity details", Error = "company id or activity id is null", UserId = userId });
                }

                var company = await db.Companies.FindAsync(companyId);

                if(company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activity details", Error = "company id or activity id is null", UserId = userId });
                }

                var gallery = await db.Galleries.Where(g => g.Active == true && g.ActivityId == activityId).ToListAsync();
                var activityAva = await db.ActivityAvailabilities.Where(a => a.Active == true && a.ActivityId == activityId).ToListAsync();
                var activityCategory = await db.ActivityCategories.FindAsync(activity.ActivityCategoryId);

                List<string> photos = new List<string>();

                foreach (var item in gallery)
                {
                    photos.Add(item.PhotoUrl);
                }

                if(photos.Count <= 0) 
                {
                    photos.Add("bg.png");
                }

                clientActivityDetails = new ClientActivityDetails()
                {
                    ActivityId = (int)activityId,
                    CompanyId = (int)companyId,
                    Photos = photos,
                    CompanyName = company.Name,
                    Sefty = activity.Safety,
                    Description = activity.Description,
                    ActivityAvailabilities = activityAva,
                    ActivityCategoryId = activityCategory.Id,
                    ActivityCategoryName = activityCategory.CategoryName
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activity details", Error = e.InnerException.Message.ToString() });
            }

            return View(clientActivityDetails);
        }
    }
}