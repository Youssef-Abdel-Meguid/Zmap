using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Zmap.Models;
using Zmap.Dtos;
using Microsoft.AspNet.Identity;
using Zmap.DTOs;

namespace Zmap.Controllers
{
    public class HotelsController : Controller
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

        public async Task<ActionResult> Index()
        {
            SetIdenitiy();
            if(userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            var hotels = new List<Hotel>();

            try
            {
                if (userType == 1)
                    hotels = await db.Hotels.Where(h => h.Active == true).ToListAsync();
                else
                    hotels = await db.Hotels.Where(h => h.Active == true && h.CreateByUserId == userId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return View(hotels);
        }

        public async Task<ActionResult> Details(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            HotelDetailsDto hotelDetailsDto = new HotelDetailsDto();

            try
            {
                Hotel hotel = await db.Hotels.FindAsync(id);
                if (hotel == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home",
                       new ErrorLogger() { ActionName = "Error in hotel", Error = "hotel not found", UserId = userId });
                }

                hotelDetailsDto = new HotelDetailsDto()
                {
                    Name = hotel.Name,
                    Id = hotel.Id,
                    IsConfirmed = hotel.IsConfirmed,
                };

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel details", Error = e.Message.ToString() , UserId = userId });
            }


            return View(hotelDetailsDto);
        }

        public async Task<ActionResult> Gallery(int? id, bool isHotel = true)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var photoDetails = new PhotoDetailsDto();

            try
            {
               if(isHotel == true)
                {
                    photoDetails = new PhotoDetailsDto()
                    {
                        HotelId = id,
                        Photos = await db.Galleries.Where(g => g.Active == true && g.HotelId == id).ToListAsync()
                    };
                }
                else
                {
                    photoDetails = new PhotoDetailsDto()
                    {
                        RoomId = id,
                        Photos = await db.Galleries.Where(g => g.Active == true && g.RoomId == id).ToListAsync()
                    };
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = e.Message.ToString(), UserId = userId });
            }

            return View(photoDetails);
        }

        public async Task<ActionResult> Contacts(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var contacts = new ContactDetailsDto();

            try
            {
                contacts = new ContactDetailsDto() 
                {
                    Contacts = await db.Contacts.Where(c => c.Active == true && c.HotelId == id).ToListAsync(),
                    HotelId = id
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = e.Message.ToString(), UserId = userId });
            }

            return View(contacts);
        }

        public async Task<ActionResult> Files(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var files = new FilesDetailsDto();

            try
            {
                files = new FilesDetailsDto()
                {
                    Files = await db.Attachments.Where(a => a.Active == true && a.HotelId == id).ToListAsync(),
                    HotelId = id
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = e.Message.ToString(), UserId = userId });
            }

            return View(files);
        }

        public async Task<ActionResult> FAQs(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var faqs = new FAQDetailsDto();

            try
            {
                faqs = new FAQDetailsDto()
                {
                    FAQs = await db.FAQs.Where(f => f.Active == true && f.HotelId == id).ToListAsync(),
                    HotelId = id
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = e.Message.ToString(), UserId = userId });
            }

            return View(faqs);
        }

        public async Task<ActionResult> AddsOn(int? id, bool isHotel = true)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var addOnsDetials = new AddOnDetailsDto();

            try
            {
                if(isHotel == true)
                {
                    var hotelAddOnDtos = new List<AddOnDto>();
                    var hotelAddsOn = await db.HotelAddsOns.Where(a => a.Active == true && a.HotelId == id).ToListAsync();

                    foreach (var hotelAdd in hotelAddsOn)
                    {
                        var addOn = await db.AddsOns.FindAsync(hotelAdd.AddOnId);

                        hotelAddOnDtos.Add(new AddOnDto()
                        {
                            Id = hotelAdd.Id,
                            EnglishName = addOn.EnglishName,
                            ArabicName = addOn.ArabicName
                        });
                    }

                    addOnsDetials = new AddOnDetailsDto()
                    {
                        AddOns = hotelAddOnDtos,
                        HotelId = id
                    };
                }
                else
                {
                    var roomAddOnDtos = new List<AddOnDto>();
                    var roomAddsOn = await db.RoomAddsOns.Where(a => a.Active == true && a.RoomId == id).ToListAsync();

                    foreach (var roomAdd in roomAddsOn)
                    {
                        var addOn = await db.AddsOns.FindAsync(roomAdd.AddOnId);

                        roomAddOnDtos.Add(new AddOnDto()
                        {
                            Id = roomAdd.Id,
                            EnglishName = addOn.EnglishName,
                            ArabicName = addOn.ArabicName
                        });
                    }

                    addOnsDetials = new AddOnDetailsDto()
                    {
                        AddOns = roomAddOnDtos,
                        RoomId = id
                    };
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = e.Message.ToString(), UserId = userId });
            }

            return View(addOnsDetials);
        }

        public async Task<ActionResult> Rooms(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var roomsData = new RoomDataDto();

            try
            {
                var roomDtos = new List<RoomDto>();
                var rooms = await db.Rooms.Where(r => r.Active == true && r.HotleId == id).ToListAsync();

                foreach (var room in rooms)
                {
                    var view = await db.RoomViews.FindAsync(room.RoomViewId);
                    var type = await db.RoomTypes.FindAsync(room.RoomTypeId);

                    roomDtos.Add(new RoomDto()
                    {
                        CreatedDate = room.CreatedDate,
                        Id = room.Id,
                        Description = room.Description,
                        HotleId = room.HotleId,
                        RoomView = view.ArabicName,
                        NumberOfRooms = room.NumberOfRooms,
                        RoomType = type != null ? type.ArabicName : ""
                    });
                }

                roomsData = new RoomDataDto()
                {
                    HotelId = id,
                    Rooms = roomDtos
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = e.Message.ToString(), UserId = userId });
            }

            return View(roomsData);
        }

        public async Task<ActionResult> Create(bool? added)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                    new ErrorLogger() { ActionName = "Error in create hotle", Error = "not authorized", UserId = userId });
            }

            try
            {
                ViewBag.Cities = await db.Cities.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HotelDto hotelDto, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                   new ErrorLogger() { ActionName = "Error in create hotle", Error = "not authorized", UserId = userId });
            }
            if (ModelState.IsValid)
            {

                try
                {
                    var city = await db.Cities.FindAsync(hotelDto.CityId);
                    var hotel = new Hotel()
                    {
                        Active = true,
                        City = city.ArabicCityName,
                        CityId = hotelDto.CityId,
                        Conditions = hotelDto.Conditions,
                        CreatedDate = DateTime.Now,
                        Description = hotelDto.Description,
                        Name = hotelDto.Name,
                        WebsiteUrl = hotelDto.WebsiteUrl,
                        GoogleMapLocation = hotelDto.GoogleMapLocation,
                        CreateByUserId = userId,
                        IsConfirmed = userType == 1,
                        ManagerEmail = hotelDto.ManagerEmail,
                        ManagerPhonenumber = hotelDto.ManagerPhonenumber
                    };



                    db.Hotels.Add(hotel);
                    await db.SaveChangesAsync();


                    if (uploadFile != null)
                    {
                        uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Hotels/" + uploadFile.FileName));
                        var gallery = new Gallery()
                        {
                            Active = true,
                            CreatedDate = DateTime.Now,
                            IsMain = true,
                            PhotoUrl = uploadFile.FileName,
                            HotelId = hotel.Id
                        };
                        db.Galleries.Add(gallery);
                        await db.SaveChangesAsync();
                    }

                    return RedirectToAction("Create", "Hotels", new { added = true });
                }
                catch (Exception e)
                {
                    return RedirectToAction("TechnicalSupport", "Home",
                    new ErrorLogger() { ActionName = "Error in create hotel", Error = e.Message.ToString() , UserId = userId });
                }
            }

            try
            {
                ViewBag.Cities = await db.Cities.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return View(hotelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePhoto(Gallery gallery, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                 new ErrorLogger() { ActionName = "Error in create hotle photo", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (uploadFile == null)
                return View();

            try
            {
                uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Hotels/" + uploadFile.FileName));
                gallery.Active = true;
                gallery.CreatedDate = DateTime.Now;
                gallery.CreatedByUserId = userId;
                gallery.PhotoUrl = uploadFile.FileName;
                gallery.IsMain = false;

                db.Galleries.Add(gallery);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                    new ErrorLogger() { ActionName = "Error in create hotel photo", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("CreatePhoto", "Hotels",new { Id = gallery.HotelId, added = true });
        }

        public async Task<ActionResult> Edit(int? id)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                 new ErrorLogger() { ActionName = "Error in edit hotel", Error = "not authorized", UserId = userId });
            }
            ViewBag.Cities = await db.Cities.ToListAsync();
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                    new ErrorLogger() { ActionName = "Error in edit hotle", Error = "id is null", UserId = userId });
            }

            HotelDto hotelDto = new HotelDto();

            try
            {
                Hotel hotel = await db.Hotels.FindAsync(id);
                if (hotel == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home",
                        new ErrorLogger() { ActionName = "Error in edit hotle", Error = "hotel not found", UserId = userId });
                }

                var photo = await db.Galleries.Where(g => g.HotelId == hotel.Id && g.Active == true && g.IsMain == true).FirstOrDefaultAsync();

                hotelDto = new HotelDto()
                {
                    Id = hotel.Id,
                    CityId = (int)hotel.CityId,
                    City = hotel.City,
                    Conditions = hotel.Conditions,
                    Description = hotel.Description,
                    GoogleMapLocation = hotel.GoogleMapLocation,
                    Name = hotel.Name,
                    WebsiteUrl = hotel.WebsiteUrl,
                    PhotoUrl = photo == null ? null : photo.PhotoUrl,
                    ManagerPhonenumber = hotel.ManagerPhonenumber,
                    ManagerEmail = hotel.ManagerEmail
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                    new ErrorLogger() { ActionName = "Error in edit hitel", Error = e.Message.ToString() , UserId = userId});
            }

            return View(hotelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HotelDto hotelDto)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home",
              new ErrorLogger() { ActionName = "Error in edit hotel", Error = "not authorized", UserId = userId });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var hotel = await db.Hotels.FindAsync(hotelDto.Id);

                    hotel.ModifiedDate = DateTime.Now;
                    hotel.GoogleMapLocation = hotelDto.GoogleMapLocation;
                    hotel.Description = hotelDto.Description;
                    hotel.Conditions = hotelDto.Conditions;
                    hotel.Name = hotelDto.Name;
                    hotel.WebsiteUrl = hotelDto.WebsiteUrl;
                    hotel.ModifiedByUserId = userId;
                    hotel.ModifiedDate = DateTime.Now;
                    hotel.ManagerEmail = hotelDto.ManagerEmail;
                    hotel.ManagerPhonenumber = hotelDto.ManagerPhonenumber;

                    if (hotel.CityId != hotelDto.CityId)
                    {
                        var city = await db.Cities.FindAsync(hotelDto.CityId);
                        hotel.City = city.ArabicCityName;
                        hotel.CityId = city.Id;
                    }

                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Hotels");
                }
                catch (Exception e)
                {
                    return RedirectToAction("TechnicalSupport", "Home",
                    new ErrorLogger() { ActionName = "Error in edit hotel", Error = e.Message.ToString() , UserId = userId });
                }
            }
            return View(hotelDto);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }
            Hotel hotel = await db.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }
            hotel.Active = false;
            hotel.ModifiedDate = DateTime.Now;
            hotel.ModifiedByUserId = userId;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Hotels");
        }

        public ActionResult CreateAddOn(int? id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            try
            {
                ViewBag.AddsOn = db.AddsOns.Where(a => a.Active == true && a.IsHotel == true).ToList();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new HotelAddsOn() { HotelId = (int) id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAddon(HotelAddsOn hotelAddsOn)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.AddsOn = db.AddsOns.Where(a => a.Active == true && a.IsHotel == true).ToList();
                    return View();
                }

                hotelAddsOn.Active = true;
                hotelAddsOn.CreatedDate = DateTime.Now;
                hotelAddsOn.HotelId = hotelAddsOn.Id;
                db.HotelAddsOns.Add(hotelAddsOn);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("CreateAddon", "Hotels", new { Id = hotelAddsOn.HotelId, added = true });
        }

        public async Task<ActionResult> DeleteAddOn(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            HotelAddsOn hotel = new HotelAddsOn();
            try
            {
                hotel = await db.HotelAddsOns.FindAsync(Id);
                if (hotel == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
                }
                hotel.Active = false;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("AddsOn", "Hotels", new { Id = hotel.HotelId});
        }

        public async Task<ActionResult> DeletePhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var galley = new Gallery();

            try
            {
                 galley = await db.Galleries.FindAsync(Id);
                if (galley == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
                }

                galley.Active = false;
                galley.ModifiedDate = DateTime.Now;
                galley.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Hotels",new { Id = galley.HotelId });
        }

        public async Task<ActionResult> DeleteRoomPhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }
            var galley = await db.Galleries.FindAsync(Id);
            if (galley == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            galley.Active = false;
            galley.ModifiedDate = DateTime.Now;
            galley.ModifiedByUserId = userId;
            await db.SaveChangesAsync();
            return RedirectToAction("Gallery", "Hotels",new { Id = galley.RoomId, isHotel = false });
        }

        public ActionResult CreatePhoto(int? Id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            return View(new Gallery() { HotelId = Id });
        }

        public ActionResult CreateContact(int? Id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            return View(new Contact() { HotelId = Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateContact(Contact contact)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
                return View(contact);

            try
            {
                contact.CreatedByUserId = userId;
                contact.CreatedDate = DateTime.Now;
                contact.Active = true;

                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("CreateContact", "Hotels", new { Id = contact.HotelId, added = true });
        }

        public async Task<ActionResult> DeleteContact(int? Id)
        {
            SetIdenitiy();
            if (userId == null || userId == 0 )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var contact = new Contact();
            try
            {
                contact = await db.Contacts.FindAsync(Id);
                if (contact == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
                }

                contact.Active = false;
                contact.ModifiedDate = DateTime.Now;
                contact.ModifiedByUserId = userId;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Contacts", "Hotels", new { Id = contact.HotelId });
        }

        public async Task<ActionResult> CreateRoom(int? Id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }
            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }


            try
            {
                ViewBag.RoomTypes = await db.RoomTypes.Where(r => r.Active == true).ToListAsync();
                ViewBag.RoomViews = await db.RoomViews.Where(r => r.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new Room() { HotleId = (int)Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRoom(Room room)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.RoomTypes = await db.RoomTypes.Where(r => r.Active == true).ToListAsync();
                    ViewBag.RoomViews = await db.RoomViews.Where(r => r.Active == true).ToListAsync();
                    return View(room);
                }

                room.Active = true;
                room.CreatedByUserId = userId;
                room.CreatedDate = DateTime.Now;

                db.Rooms.Add(room);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("CreateRoom", "Hotels", new { Id = room.HotleId, added = true });
        }

        public async Task<ActionResult> EditRoom(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var room = new Room();

            try
            {
                room = await db.Rooms.FindAsync(id);
                if (room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "room is null", UserId = userId });
                }
                ViewBag.RoomTypes = await db.RoomTypes.Where(r => r.Active == true).ToListAsync();
                ViewBag.RoomViews = await db.RoomViews.Where(r => r.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }


            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRoom(Room room)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.RoomTypes = await db.RoomTypes.Where(r => r.Active == true).ToListAsync();
                    ViewBag.RoomViews = await db.RoomViews.Where(r => r.Active == true).ToListAsync();
                    return View(room);
                }

                room.ModifiedDate = DateTime.Now;
                room.ModifiedByUserId = userId;
                db.Entry(room).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Rooms", "Hotels", new { Id = room.HotleId });
        }

        public async Task<ActionResult> DeleteRoom(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var room = new Room();

            try
            {
                room = await db.Rooms.FindAsync(Id);
                if (room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "room is null", UserId = userId });
                }
                room.Active = false;
                room.ModifiedDate = DateTime.Now;
                room.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Rooms", "Hotels",new { Id = room.HotleId });
        }

        public async Task<ActionResult> RoomDetails(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            RoomDetailsDto roomDetailsDto = new RoomDetailsDto();

            try
            {
                var room = await db.Rooms.Where(r => r.Active == true && r.Id == Id).SingleOrDefaultAsync();

                if (room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
                }


                roomDetailsDto = new RoomDetailsDto()
                {
                    CreatedDate = room.CreatedDate,
                    Description = room.Description,
                    HotleId = room.HotleId,
                    Id = room.Id,
                    NumberOfRooms = room.NumberOfRooms
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(roomDetailsDto);
        }

        public async Task<ActionResult> RoomAvailabilities(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var roomAvaDetails = new RoomAvailabilityDetailsDto();

            try
            {
                var room = await db.Rooms.FindAsync(id);

                if(room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "room is null", UserId = userId });
                }

                var roomAvaDto = new List<RoomAvailabilityDto>();
                var roomAva = await db.RoomAvailabilities.Where(a => a.Active == true && a.RoomId == id).ToListAsync();

                foreach (var ava in roomAva)
                {
                    var acco = await db.Accommodations.FindAsync(ava.AccommodationId);
                    roomAvaDto.Add(new RoomAvailabilityDto()
                    {
                        AccommodationId = acco != null ? acco.Id : 0,
                        ArabicName = acco != null ? acco.ArabicName : null,
                        DateFrom = ava.DateFrom,
                        DateTo = ava.DateTo,
                        RoomId = ava.RoomId,
                        OtherValue = ava.OtherValue,
                        PricePerNight = ava.PricePerNght,
                        Id = ava.Id
                    });
                }

                roomAvaDetails = new RoomAvailabilityDetailsDto()
                {
                    RoomId = room.Id,
                    HotelId = room.HotleId,
                    RoomAvailabilities = roomAvaDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(roomAvaDetails);
        }

        public async Task<ActionResult> DeleteRoomAvailability(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var room = new RoomAvailability();

            try
            {
                room = await db.RoomAvailabilities.FindAsync(Id);
                if (room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
                }
                room.Active = false;
                room.ModifiedDate = DateTime.Now;
                room.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("RoomAvailabilities", "Hotels",new { Id = room.RoomId });
        }

        public async Task<ActionResult> DeleteRoomSpecial(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null )
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var room = new RoomAddsOn();

            try
            {
                room = await db.RoomAddsOns.FindAsync(Id);
                if (room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "room is null", UserId = userId });
                }
                room.Active = false;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("AddsOn", "Hotels", new { Id = room.RoomId, isHotel = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRoomPhoto(Gallery gallery, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                if (uploadFile == null)
                    return View();
                uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Rooms/" + uploadFile.FileName));
                gallery.Active = true;
                gallery.CreatedDate = DateTime.Now;
                gallery.CreatedByUserId = userId;
                gallery.IsMain = false;
                gallery.PhotoUrl = uploadFile.FileName;
                db.Galleries.Add(gallery);
                await db.SaveChangesAsync();
            }
            catch (Exception e) 
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("CreateRoomPhoto", "Hotels", new { Id = gallery.RoomId, added = true });
        }

        public ActionResult CreateRoomPhoto(int? Id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            return View(new Gallery() { RoomId = Id });
        }

        public async Task<ActionResult> CreateRoomSpecial(int? Id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            try
            {
                ViewBag.AddsOn = await db.AddsOns.Where(a => a.Active == true && a.IsRoom == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new RoomAddsOn() { RoomId = (int)Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRoomSpecial(RoomAddsOn roomSpecial)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.AddsOn = await db.AddsOns.Where(a => a.Active == true && a.IsRoom == true).ToListAsync();
                    return View(roomSpecial);
                }

                roomSpecial.Active = true;
                roomSpecial.CreatedDate = DateTime.Now;
                roomSpecial.CreatedByUserId = userId;

                db.RoomAddsOns.Add(roomSpecial);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("CreateRoomSpecial", "Hotels",new { Id = roomSpecial.RoomId, added = true });
        }

        public async Task<ActionResult> CreateRoomAvailability(int? Id, bool? added, bool? invalidDate)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });

            try
            {
                ViewBag.Accos = await db.Accommodations.Where(a => a.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(new RoomAvailability() { RoomId = (int)Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRoomAvailability(RoomAvailability roomAvailability)
        {
            SetIdenitiy();
            if (userId == 0 ||  userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Accos = await db.Accommodations.Where(a => a.Active == true).ToListAsync();
                    return View(roomAvailability);
                }

                if(roomAvailability.DateTo < roomAvailability.DateFrom)
                {
                    return RedirectToAction("CreateRoomAvailability", "Hotels", new { Id = roomAvailability.RoomId, invalidDate = true });
                }

                var roomsAva = await db.RoomAvailabilities.Where(r => r.Active == true && r.AccommodationId == roomAvailability.AccommodationId).ToListAsync();


                foreach (var item in roomsAva)
                {
                    if(item.DateFrom <= roomAvailability.DateFrom && item.DateTo >= roomAvailability.DateTo)
                    {
                        return RedirectToAction("CreateRoomAvailability", "Hotels", new { Id = roomAvailability.RoomId, invalidDate = true });
                    }
                }

                roomAvailability.Active = true;
                roomAvailability.CreatedByUserId = userId;
                roomAvailability.CreatedDate = DateTime.Now;
                db.RoomAvailabilities.Add(roomAvailability);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("CreateRoomAvailability", "Hotels", new { Id = roomAvailability.RoomId, added = true});
        }

        public ActionResult CreateFAQ(int? id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            return View(new FAQ() { HotelId = (int)id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateFAQ(FAQ fAQ)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                fAQ.Active = true;
                fAQ.CreatedDate = DateTime.Now;
                db.FAQs.Add(fAQ);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("CreateFAQ", "Hotels", new { Id = fAQ.HotelId, added = true });
        }

        public async Task<ActionResult> DeleteFAQ(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
            }

            var faq = new FAQ();

            try
            {
                faq = await db.FAQs.FindAsync(Id);

                if (faq == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotel", Error = "id is null", UserId = userId });
                }

                faq.Active = false;
                faq.ModifiedDate = DateTime.Now;
                faq.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("FAQs", "Hotels", new { Id = faq.HotelId });
        }

        public ActionResult CreateAttachment(int? id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            return View(new Attachment() { HotelId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAttachment(Attachment attachment, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
                return View(attachment);

            try
            {
                if (uploadFile == null)
                    return View(attachment);

                attachment.Active = true;
                attachment.CreatedByUserId = userId;
                attachment.CreatedDate = DateTime.Now;
                attachment.FileName = uploadFile.FileName;

                var formattedFileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), uploadFile.FileName);
                uploadFile.SaveAs(HttpContext.Server.MapPath("~/Attachments/Hotel/" + formattedFileName));

                attachment.SavedFileName = formattedFileName;

                db.Attachments.Add(attachment);
                await db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateAttachment", "Hotels", new { Id = attachment.HotelId, added = true });
        }

        public ActionResult DownloadAttachment(string filePath)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            string fullName = Server.MapPath("~/Attachments/Hotel/" + filePath);
            byte[] fileBytes = GetFile(fullName);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fullName);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        public async Task<ActionResult> DeleteAttachment(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            var att = new Attachment();

            try
            {
                att = await db.Attachments.FindAsync(id);

                if (att == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "attachement is null", UserId = userId });
                }

                att.Active = false;
                att.ModifiedByUserId = userId;
                att.ModifiedDate = DateTime.Now;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Files", "Hotels", new { id = att.HotelId });
        }

        public async Task<ActionResult> Confirm(int? id, bool? isConfirm)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            try
            {
                var hotel = await db.Hotels.FindAsync(id);

                if (hotel == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "hotel is null", UserId = userId });
                }

                hotel.IsConfirmed = isConfirm;
                hotel.ModifiedDate = DateTime.Now;
                hotel.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> HotelCustomAdds(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            var hotelCustomAdds = new HotelCustomAddDto();

            try
            {
                hotelCustomAdds = new HotelCustomAddDto()
                {
                    HotelCustomAdds = await db.HotelCustomAdds.Where(h => h.RoomId == id && h.Active == true).ToListAsync(),
                    RoomId = id
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(hotelCustomAdds);
        }

        public async Task<ActionResult> DeleteHotelCustomAdds(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            int? roomId;

            try
            {
                var hotelCustom = await db.HotelCustomAdds.FindAsync(id);

                if(hotelCustom == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "hotel custome is null", UserId = userId });
                }

                roomId = hotelCustom.RoomId;

                hotelCustom.Active = false;
                hotelCustom.ModifiedDate = DateTime.Now;
                hotelCustom.ModifiedByUserId = userId;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("HotelCustomAdds", "Hotels", new { id = roomId });
        } 

        public ActionResult CreateHotelCustomAdds(int? id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            return View(new HotelCustomAdd() { RoomId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateHotelCustomAdds(HotelCustomAdd hotelCustomAdd)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if(!ModelState.IsValid)
            {
                return View(hotelCustomAdd);
            }

            try
            {
                hotelCustomAdd.Active = true;
                hotelCustomAdd.CreatedByUserId = userId;
                hotelCustomAdd.CreatedDate = DateTime.Now;
                db.HotelCustomAdds.Add(hotelCustomAdd);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateHotelCustomAdds", "Hotels", new { id = hotelCustomAdd.RoomId, added = true });
        }

        public async Task<ActionResult> EditHotelCustomAdds(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            var hotelCustom = new HotelCustomAdd();

            try
            {
                hotelCustom = await db.HotelCustomAdds.FindAsync(id);
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(hotelCustom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditHotelCustomAdds(HotelCustomAdd hotelCustomAdd)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View(hotelCustomAdd);
            }

            try
            {
                hotelCustomAdd.Active = true;
                hotelCustomAdd.ModifiedByUserId = userId;
                hotelCustomAdd.ModifiedDate = DateTime.Now;
                db.Entry(hotelCustomAdd).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("HotelCustomAdds", "Hotels", new { id = hotelCustomAdd.RoomId });
        }

        public async Task<ActionResult> LockedRooms(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            var lockedRoomDetails = new LockedRoomDetailsDto();

            try
            {
                var lockedRooms = await db.LockedRooms.Where(l => l.Active == true && l.RoomId == id).ToListAsync();

                if(lockedRooms == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "locked rooms is null", UserId = userId });
                }

                var lockedRoomsDto = new List<LockedRoomDto>();

                foreach (var room in lockedRooms)
                {
                    var theRoom = await db.Rooms.FindAsync(room.RoomId);
                    var roomView = await db.RoomViews.FindAsync(theRoom.RoomViewId);
                    var roomType = await db.RoomTypes.FindAsync(theRoom.RoomTypeId);

                    lockedRoomsDto.Add(new LockedRoomDto()
                    {
                        Id = room.Id,
                        LockedDateFrom = room.DateFrom,
                        LockedDateTo = room.DateTo,
                        NumberOfLockedRooms = room.NumberOfLockedRooms,
                        RoomType = roomType.ArabicName,
                        RoomView = roomView.ArabicName
                    });
                }

                lockedRoomDetails = new LockedRoomDetailsDto()
                {
                    RoomId = id,
                    LockedRooms = lockedRoomsDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(lockedRoomDetails);
        }

        public async Task<ActionResult> DeleteLockedRoom(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            int? roomId;

            try
            {
                var lockedRooms = await db.LockedRooms.FindAsync(id);

                if(lockedRooms == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "locked rooms is null", UserId = userId });
                }

                roomId = lockedRooms.RoomId;
                var room = await db.Rooms.FindAsync(roomId);

                room.NumberOfRooms = room.NumberOfRooms + lockedRooms.NumberOfLockedRooms;
                room.ModifiedDate = DateTime.Now;
                room.ModifiedByUserId = userId;

                lockedRooms.Active = false;
                lockedRooms.ModifiedDate = DateTime.Now;
                lockedRooms.ModifiedByUserId = userId;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("LockedRooms", "Hotels", new { id = roomId});
        }

        public async Task<ActionResult> CreateLockedRoom(int? id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            try
            {
                var room = await db.Rooms.FindAsync(id);

                if(room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "room is null", UserId = userId });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(new LockedRoom() { RoomId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateLockedRoom(LockedRoom lockedRoom)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            try
            {
                if(!ModelState.IsValid)
                {
                    return View(lockedRoom);
                }

                if (lockedRoom.NumberOfLockedRooms <= 0)
                {
                    ViewBag.Validation = "number of locked rooms cannot be less than or equal 0";
                    return View(lockedRoom);
                }

                var room = await db.Rooms.FindAsync(lockedRoom.RoomId);

                if(room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "room is null", UserId = userId });
                }

                if(lockedRoom.NumberOfLockedRooms > room.NumberOfRooms)
                {
                    ViewBag.Validation = "number of requested rooms is greater than number of actual rooms";
                    return View(lockedRoom);
                }

                room.NumberOfRooms = room.NumberOfRooms - lockedRoom.NumberOfLockedRooms;
                room.ModifiedDate = DateTime.Now;
                room.ModifiedByUserId = userId;

                lockedRoom.CreatedByUserId = userId;
                lockedRoom.CreatedDate = DateTime.Now;
                lockedRoom.Active = true;

                db.LockedRooms.Add(lockedRoom);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateLockedRoom", "Hotels", new { id = lockedRoom.RoomId, added = true });
        }

        public async Task<ActionResult> EditLockedRoom(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            var lockedRoom = new LockedRoom();

            try
            {
                lockedRoom = await db.LockedRooms.FindAsync(id);

                if (lockedRoom == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "room is null", UserId = userId });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(lockedRoom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLockedRoom(LockedRoom lockedRoom)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(lockedRoom);
                }

                if (lockedRoom.NumberOfLockedRooms <= 0)
                {
                    ViewBag.Validation = "number of locked rooms cannot be less than or equal 0";
                    return View(lockedRoom);
                }

                var room = await db.Rooms.FindAsync(lockedRoom.RoomId);

                if (room == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "room is null", UserId = userId });
                }

                if (lockedRoom.NumberOfLockedRooms > room.NumberOfRooms)
                {
                    ViewBag.Validation = "number of requested rooms is greater than number of actual rooms";
                    return View(lockedRoom);
                }

                var tempLockedRoom = db.LockedRooms.Find(lockedRoom.Id);

                room.NumberOfRooms = tempLockedRoom.NumberOfLockedRooms + room.NumberOfRooms;
                room.NumberOfRooms = room.NumberOfRooms - lockedRoom.NumberOfLockedRooms;
                room.ModifiedDate = DateTime.Now;
                room.ModifiedByUserId = userId;

                tempLockedRoom.ModifiedByUserId = userId;
                tempLockedRoom.ModifiedDate = DateTime.Now;
                tempLockedRoom.DateTo = lockedRoom.DateTo;
                tempLockedRoom.DateFrom = lockedRoom.DateFrom;
                tempLockedRoom.NumberOfLockedRooms = lockedRoom.NumberOfLockedRooms;

                //db.Entry(lockedRoom).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("LockedRooms", "Hotels", new { id = lockedRoom.RoomId });
        }

        public async Task<ActionResult> ChildPolicy(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            var childPolicyDetailsDto = new ChildPolicyDetailsDto();

            try
            {
                var childPolicy = await db.ChildPolicies.Where(c => c.Active == true && c.HotelId == id).ToListAsync();
                var childPolicyDto = new List<ChildPolicyDto>();
                foreach (var item in childPolicy)
                {
                    var cat = await db.ChildPolicyCategories.FindAsync(item.ChildPolicyCategoryId);
                    childPolicyDto.Add(new ChildPolicyDto()
                    {
                        Id = item.Id,
                        AgeTo = item.AgeTo,
                        AgeFrom = item.AgeFrom,
                        AccommodationPercentage = item.AccommodationPercentage,
                        ChildPolicyCategoryId = cat.Id,
                        ChildPolicyCategory = cat.CategoryName
                    });
                }

                childPolicyDetailsDto = new ChildPolicyDetailsDto()
                {
                    ChildPolicies = childPolicyDto,
                    HotelId = id
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(childPolicyDetailsDto);
        }

        public async Task<ActionResult> DeleteChildPolicy(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            int? hotelId;

            try
            {
                var childPolicy = await db.ChildPolicies.FindAsync(id);

                if(childPolicy == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "child policy is null", UserId = userId });
                }

                hotelId = childPolicy.HotelId;

                childPolicy.ModifiedDate = DateTime.Now;
                childPolicy.ModifiedByUserId = userId;
                childPolicy.Active = false;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("ChildPolicy", "Hotels", new { id = hotelId });
        }

        public async Task<ActionResult> CreateChildPolicy(int? id, bool? added)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
            }

            try
            {
                ViewBag.ChildPolicyCategories = await db.ChildPolicyCategories.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return View(new ChildPolicy() { HotelId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateChildPolicy(ChildPolicy childPolicy) 
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 2)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ChildPolicyCategories = await db.ChildPolicyCategories.ToListAsync();
                    return View(childPolicy);
                }

                var childPolicies = await db.ChildPolicies.Where(c => c.Active == true && c.ChildPolicyCategoryId == childPolicy.ChildPolicyCategoryId).ToListAsync();

                if(childPolicies.Count > 0)
                {
                    ViewBag.ChildPolicyCategories = await db.ChildPolicyCategories.ToListAsync();
                    ViewBag.Invalid = "تم اضافة النوع مسبقا";
                    return View(childPolicy);
                }

                childPolicy.CreatedByUserId = userId;
                childPolicy.CreatedDate = DateTime.Now;
                childPolicy.Active = true;

                db.ChildPolicies.Add(childPolicy);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateChildPolicy", "Hotels", new { id = childPolicy.HotelId, added = true });
        }

        //public async Task<ActionResult> EditChildPolicy(int? id)
        //{
        //    SetIdenitiy();
        //    if (userId == 0 || userId == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    if (userType != 1 && userType != 2)
        //    {
        //        return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
        //    }

        //    if (id == null)
        //    {
        //        return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "id is null", UserId = userId });
        //    }

        //    var childPolicy = new ChildPolicy();

        //    try
        //    {
        //        childPolicy = await db.ChildPolicies.FindAsync(id);

        //        if(childPolicy == null)
        //        {
        //            return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "child policy is null", UserId = userId });
        //        }

        //        ViewBag.AccoPercentage = await db.ChildAccommodationPercentages.Where(c => c.Active == true).ToListAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
        //    }

        //    return View(childPolicy);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EditChildPolicy(ChildPolicy childPolicy)
        //{
        //    SetIdenitiy();
        //    if (userId == 0 || userId == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    if (userType != 1 && userType != 2)
        //    {
        //        return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = "not authorized", UserId = userId });
        //    }

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            ViewBag.AccoPercentage = await db.ChildAccommodationPercentages.Where(c => c.Active == true).ToListAsync();
        //            return View(childPolicy);
        //        }

        //        childPolicy.ModifiedByUserId = userId;
        //        childPolicy.ModifiedDate = DateTime.Now;

        //        db.Entry(childPolicy).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in hotels", Error = e.Message.ToString(), UserId = userId });
        //    }

        //    return RedirectToAction("ChildPolicy", "Hotels", new { id = childPolicy.HotelId });
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}