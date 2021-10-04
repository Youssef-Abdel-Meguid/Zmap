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

namespace Zmap.Controllers
{
    public class TransportationController : Controller
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

        // GET: Transportation
        public async Task<ActionResult> Index()
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            var companies = new List<Company>();

            try
            {
                if (userType == 1)
                    companies = await db.Companies.Where(c => c.IsExternal == true && c.Active == true).ToListAsync();
                else
                    companies = await db.Companies.Where(c => c.IsExternal == true && c.Active == true && c.CreatedByUserId == userId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(companies);
        }

        // GET: Transportation/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var companyDto = new TransportationDetailsDto();

            try
            {
                var company = await db.Companies.FindAsync(id);

                if (company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                var lines = await db.TransportationCompanyLines.Where(l => l.Active == true && l.CompanyId == company.Id).ToListAsync();
                var companyPhotos = await db.Galleries.Where(g => g.Active == true && g.CompanyId == company.Id).ToListAsync();
                var companyContacts = await db.Contacts.Where(c => c.Active == true && c.CompanyId == company.Id).ToListAsync();
                var faq = await db.FAQs.Where(f => f.Active == true && f.CompanyId == company.Id).ToListAsync();
                var buses = await db.Buses.Where(b => b.Active == true && b.CompanyId == company.Id).ToListAsync();
                var stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == company.Id).ToListAsync();

                var busDto = new List<BusDto>();

                foreach (var bus in buses)
                {
                    var cat = await db.BusCategories.FindAsync(bus.CategoryId);
                    busDto.Add(new BusDto()
                    {
                        BusNumber = bus.BusNumber,
                        CreatedDate = bus.CreatedDate,
                        Id = bus.Id,
                        Name = bus.Name,
                        NumberOfSeats = bus.NumberOfSeats,
                        CategoryId = bus.CategoryId,
                        Category = cat.CategoryName,
                        BusSeatMapId = bus.SeatsMapId,
                        CompanyId = bus.CompanyId
                    });
                }

                var stationDto = new List<StationDto>();

                foreach (var station in stations)
                {
                    var city = await db.Cities.FindAsync(station.CityId);
                    stationDto.Add(new StationDto()
                    {
                        Address = station.Address,
                        Id = station.id,
                        Name = station.Name,
                        GoogleMapLocation = station.GoogleMapLocation,
                        CreatedDate = station.CreatedDate,
                        CityId = station.CityId,
                        City = city.ArabicCityName,
                        CompanyId = station.CompanyId
                    });
                }

                var lineDto = new List<LineDto>();

                foreach (var line in lines)
                {
                    var home = await db.Cities.FindAsync(line.HomeCityId);
                    var destination = await db.Cities.FindAsync(line.DestinationCityId);
                    var lineStart = await db.Stations.FindAsync(line.LineStartStationId);
                    var lineEnd = await db.Stations.FindAsync(line.LineEndStationId);

                    lineDto.Add(new LineDto()
                    {
                        CompanyId = line.CompanyId,
                        HomeCityId = line.HomeCityId,
                        CreatedDate = line.CreatedDate,
                        Destination = destination.ArabicCityName,
                        Home = home.ArabicCityName,
                        Id = line.Id,
                        LineStart = lineStart.Name,
                        DestinationCityId = line.DestinationCityId,
                        LineEnd = lineEnd.Name,
                        LineName = line.LineName
                    });
                }

                companyDto = new TransportationDetailsDto()
                {
                    About = company.About,
                    Contacts = companyContacts,
                    CreatedDate = company.CreatedDate,
                    FAQs = faq,
                    Id = company.Id,
                    Name = company.Name,
                    PrivacyPolicy = company.PrivacyPolicy,
                    Buses = busDto,
                    Photos = companyPhotos,
                    Stations = stationDto,
                    Lines = lineDto,
                    IsConfirmed = company.IsConfirmed,
                    Attachments = await db.Attachments.Where(a => a.Active == true && a.CompanyId == company.Id).ToListAsync(),
                    ManagerEmail = company.ManagerEmail,
                    ManagerPhonenumber = company.ManagerPhonenumber
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(companyDto);
        }

        public async Task<ActionResult> LineDetails(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var lineDto = new LineDto();

            try
            {
                var line = await db.TransportationCompanyLines.FindAsync(id);

                if (line == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                var home = await db.Cities.FindAsync(line.HomeCityId);
                var destination = await db.Cities.FindAsync(line.DestinationCityId);
                var lineBuses = await db.LineBuses.Where(l => l.Active == true && l.LineId == line.Id).ToListAsync();
                var lineStations = await db.LineStations.Where(s => s.Active == true && s.LineId == line.Id).ToListAsync();

                var lineBusesDto = new List<LineBusDto>();
                var lineStationsDto = new List<LineStationDto>();

                foreach (var lineBus in lineBuses)
                {
                    var bus = await db.Buses.FindAsync(lineBus.BusId);

                    lineBusesDto.Add(new LineBusDto()
                    {
                        ArrivalTime = lineBus.ArrivalTime,
                        Id = lineBus.Id,
                        SeatPrice = lineBus.SeatPrice,
                        LineId = lineBus.LineId,
                        CreatedDate = lineBus.CreatedDate,
                        DepartureTime = lineBus.DepartureTime,
                        BusId = lineBus.BusId,
                        BusName = bus.Name,
                        BusNumber = bus.BusNumber
                    });
                }

                foreach (var lineStation in lineStations)
                {
                    var station = await db.Stations.FindAsync(lineStation.StationId);
                    lineStationsDto.Add(new LineStationDto()
                    {
                        Id = lineStation.Id,
                        LineId = line.Id,
                        StoppingByOrder = lineStation.StoppinByOrder,
                        StationId = lineStation.StationId,
                        StationName = station.Name
                    });
                }

                var lineStart = await db.Stations.FindAsync(line.LineStartStationId);
                var lineEnd = await db.Stations.FindAsync(line.LineEndStationId);

                lineDto = new LineDto()
                {
                    CompanyId = line.CompanyId,
                    HomeCityId = line.HomeCityId,
                    CreatedDate = line.CreatedDate,
                    Destination = destination.ArabicCityName,
                    Home = home.ArabicCityName,
                    Id = line.Id,
                    LineStart = lineStart.Name,
                    DestinationCityId = line.DestinationCityId,
                    LineEnd = lineEnd.Name,
                    LineName = line.LineName,
                    LineBuses = lineBusesDto,
                    LineStations = lineStationsDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }


            return View(lineDto);
        }

        public async Task<ActionResult> StationDetails(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var stationDto = new StationDto();

            try
            {
                var station = await db.Stations.FindAsync(id);

                if (station == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                var city = await db.Cities.FindAsync(station.CityId);

                stationDto = new StationDto()
                {
                    Address = station.Address,
                    Id = station.id,
                    Photos = await db.Galleries.Where(a => a.Active == true && a.StationId == station.id).ToListAsync(),
                    Name = station.Name,
                    GoogleMapLocation = station.GoogleMapLocation,
                    CreatedDate = station.CreatedDate,
                    Contacts = await db.Contacts.Where(c => c.Active == true && c.StationId == station.id).ToListAsync(),
                    CityId = station.CityId,
                    City = city.ArabicCityName,
                    CompanyId = station.CompanyId
                };

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(stationDto);
        }

        // GET: Transportation/Create
        public ActionResult Create()
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Company company)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    company.IsExternal = true;
                    company.IsActivity = false;
                    company.IsInternal = false;
                    company.CreatedDate = DateTime.Now;
                    company.CreatedByUserId = userId;
                    company.Active = true;
                    company.IsConfirmed = userType == 1;
                    db.Companies.Add(company);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(company);
        }

        // GET: Transportation/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            Company company = new Company();

            try
            {
                company = await db.Companies.FindAsync(id);
                if (company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(company);
        }

        // POST: Transportation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Company company)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    company.ModifiedDate = DateTime.Now;
                    company.ModifiedByUserId = userId;
                    db.Entry(company).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(company);
        }

        // GET: Transportation/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            Company company = new Company();

            try
            {
                company = await db.Companies.FindAsync(id);
                if (company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }
                company.Active = false;
                company.ModifiedDate = DateTime.Now;
                company.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Index", "Transportation");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePhoto(Gallery gallery, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                if (uploadFile == null)
                    return View();

                uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Transportation/Companies/" + uploadFile.FileName));
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = gallery.CompanyId });
        }

        public ActionResult CreatePhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            return View(new Gallery() { CompanyId = Id });
        }

        public async Task<ActionResult> DeletePhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var galley = new Gallery();

            try
            {
                galley = await db.Galleries.FindAsync(Id);
                if (galley == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                galley.Active = false;
                galley.ModifiedDate = DateTime.Now;
                galley.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = galley.CompanyId });
        }

        public ActionResult CreateContact(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            return View(new Contact() { CompanyId = Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateContact(Contact contact)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
                return View(contact);

            try
            {
                contact.CreatedByUserId = userId;
                contact.CreatedDate = DateTime.Now;
                contact.CreatedByUserId = userId;
                contact.Active = true;
                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = contact.CompanyId });
        }

        public async Task<ActionResult> DeleteContact(int? Id)
        {
            SetIdenitiy();
            if (userId == null || userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var contact = new Contact();

            try
            {
                contact = await db.Contacts.FindAsync(Id);
                if (contact == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }
                contact.Active = false;
                contact.ModifiedDate = DateTime.Now;
                contact.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }
            return RedirectToAction("Details", "Transportation", new { Id = contact.CompanyId });
        }

        public ActionResult CreateFAQ(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            return View(new FAQ() { CompanyId = (int)id });
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

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                fAQ.Active = true;
                fAQ.CreatedDate = DateTime.Now;
                fAQ.CreatedByUserId = userId;
                db.FAQs.Add(fAQ);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = fAQ.CompanyId });
        }

        public async Task<ActionResult> CreateBus(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                ViewBag.BusCategory = await db.BusCategories.Where(c => c.Active == true).ToListAsync();
                ViewBag.SeatMap = await db.BusSeatsMaps.Where(s => s.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new Bus() { CompanyId = (int)id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBus(Bus bus)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                bus.Active = true;
                bus.CreatedDate = DateTime.Now;
                bus.CreatedByUserId = userId;
                db.Buses.Add(bus);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = bus.CompanyId });
        }

        public async Task<ActionResult> CreateStation(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                ViewBag.Cities = await db.Cities.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new Station() { CompanyId = (int)id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateStation(Station station)
        {
            SetIdenitiy();
            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var city = await db.Cities.FindAsync(station.CityId);
                station.City = city == null ? null : city.ArabicCityName;
                station.Active = true;
                station.CreatedDate = DateTime.Now;
                station.CreatedByUserId = userId;
                db.Stations.Add(station);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = station.CompanyId });
        }

        public async Task<ActionResult> CreateLine(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                ViewBag.Stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == id).ToListAsync();
                ViewBag.Cities = await db.Cities.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new TransportationCompanyLine() { CompanyId = (int)id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateLine(TransportationCompanyLine line)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            try
            {

                if(!ModelState.IsValid)
                {
                    ViewBag.Stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == line.CompanyId).ToListAsync();
                    ViewBag.Cities = await db.Cities.ToListAsync();
                    return View(line);
                }

                line.Active = true;
                line.CreatedDate = DateTime.Now;
                line.CreatedByUserId = userId;
                db.TransportationCompanyLines.Add(line);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = line.CompanyId });
        }

        public async Task<ActionResult> DeleteFAQ(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var faq = new FAQ();

            try
            {
                faq = await db.FAQs.FindAsync(Id);
                if (faq == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }
                faq.Active = false;
                faq.ModifiedDate = DateTime.Now;
                faq.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = faq.CompanyId });
        }

        public async Task<ActionResult> DeleteBus(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var bus = new Bus();

            try
            {
                bus = await db.Buses.FindAsync(Id);
                if (bus == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                bus.Active = false;
                bus.ModifiedDate = DateTime.Now;
                bus.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = bus.CompanyId });
        }

        public async Task<ActionResult> DeleteStation(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var station = new Station();

            try
            {
                station = await db.Stations.FindAsync(Id);
                if (station == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                station.Active = false;
                station.ModifiedDate = DateTime.Now;
                station.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = station.CompanyId });
        }

        public async Task<ActionResult> DeleteLine(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var line = new TransportationCompanyLine();

            try
            {
                line = await db.TransportationCompanyLines.FindAsync(Id);
                if (line == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                line.Active = false;
                line.ModifiedDate = DateTime.Now;
                line.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Transportation", new { Id = line.CompanyId });
        }

        public async Task<ActionResult> BusDetails(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var busDto = new BusDto();

            try
            {
                var bus = await db.Buses.FindAsync(id);

                if (bus == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                var cat = await db.BusCategories.FindAsync(bus.CategoryId);
                var map = await db.BusSeatsMaps.FindAsync(bus.SeatsMapId);

                var busAdds = await db.BusAddOns.Where(b => b.Active == true && b.BusId == bus.Id).ToListAsync();

                var busAddDtos = new List<AddOnDto>();

                foreach (var add in busAdds)
                {
                    var addOn = await db.AddsOns.FindAsync(add.AddOnId);
                    busAddDtos.Add(new AddOnDto()
                    {
                        Id = add.Id,
                        EnglishName = addOn.EnglishName,
                        ArabicName = addOn.ArabicName
                    });
                }

                var trips = await db.BusTripSchedules.Where(b => b.Active == true && b.BusId == bus.Id).ToListAsync();

                var tripsDto = new List<BusTripScheduleDto>();

                foreach (var trip in trips)
                {
                    var from = await db.Stations.FindAsync(trip.StationFromId);
                    var to = await db.Stations.FindAsync(trip.StationToId);
                    tripsDto.Add(new BusTripScheduleDto()
                    {
                        CreatedDate = trip.CreatedDate,
                        BusId = trip.BusId,
                        Id = trip.Id,
                        StationFromId = trip.StationFromId,
                        StationToId = trip.StationToId,
                        StationFromName = from.Name,
                        StationToName = to.Name,
                        ArrivalDate = trip.ArrivalDate.Date,
                        DepartureDate = trip.DepartureDate.Date,
                        DepartureTime = trip.DepartureTime,
                        ArrivalTime = trip.ArrivalTime
                    });
                }

                busDto = new BusDto()
                {
                    BusNumber = bus.BusNumber,
                    CreatedDate = bus.CreatedDate,
                    Id = bus.Id,
                    Name = bus.Name,
                    Photos = await db.Galleries.Where(g => g.Active == true && g.BusId == bus.Id).ToListAsync(),
                    NumberOfSeats = bus.NumberOfSeats,
                    CategoryId = bus.CategoryId,
                    Category = cat.CategoryName,
                    BusSeatMapId = bus.SeatsMapId,
                    BusSeatMap = map.MapName,
                    BusAddOns = busAddDtos,
                    CompanyId = bus.CompanyId,
                    busTripSchedules = tripsDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(busDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBusPhoto(Gallery gallery, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                if (uploadFile == null)
                    return View();

                uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Transportation/Buses/" + uploadFile.FileName));
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("BusDetails", "Transportation", new { Id = gallery.BusId });
        }

        public ActionResult CreateBusPhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            return View(new Gallery() { BusId = Id });
        }

        public ActionResult CreateBusAddOn(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                ViewBag.AddsOn = db.AddsOns.Where(a => a.Active == true && a.IsBus == true).ToList();
            }
            catch (Exception e)
            {

                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new BusAddOn() { BusId = (int)id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBusAddon(BusAddOn busAddOn)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.AddsOn = db.AddsOns.Where(a => a.Active == true && a.IsBus == true).ToList();
                    return View();
                }

                busAddOn.Active = true;
                db.BusAddOns.Add(busAddOn);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("BusDetails", "Transportation", new { Id = busAddOn.BusId });
        }

        public async Task<ActionResult> CreateBusTrip(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                var bus = await db.Buses.FindAsync(id);
                ViewBag.Stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == bus.CompanyId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new BusTripSchedule() { BusId = (int)id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBusTrip(BusTripSchedule trip)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var bus = await db.Buses.FindAsync(trip.BusId);
                    ViewBag.Stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == bus.CompanyId).ToListAsync();
                    return View();
                }
                trip.Active = true;
                trip.CreatedByUserId = userId;
                trip.CreatedDate = DateTime.Now;
                db.BusTripSchedules.Add(trip);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("BusDetails", "Transportation", new { Id = trip.BusId });
        }

        public async Task<ActionResult> DeleteBusPhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var station = new Gallery();

            try
            {
                station = await db.Galleries.FindAsync(Id);
                if (station == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                station.Active = false;
                station.ModifiedDate = DateTime.Now;
                station.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("BusDetails", "Transportation", new { Id = station.BusId });
        }

        public async Task<ActionResult> DeleteBusAddOn(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var station = new BusAddOn();

            try
            {

                station = await db.BusAddOns.FindAsync(Id);
                if (station == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }
                station.Active = false;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("BusDetails", "Transportation", new { Id = station.BusId });
        }

        public async Task<ActionResult> DeleteBusTrip(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var station = new BusTripSchedule();

            try
            {

                station = await db.BusTripSchedules.FindAsync(Id);
                if (station == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }
                station.Active = false;
                station.ModifiedDate = DateTime.Now;
                station.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("BusDetails", "Transportation", new { Id = station.BusId });
        }

        public ActionResult CreateStationContact(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }


            return View(new Contact() { StationId = Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateStationContact(Contact contact)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
                return View(contact);

            try
            {
                contact.CreatedByUserId = userId;
                contact.CreatedDate = DateTime.Now;
                contact.CreatedByUserId = userId;
                contact.Active = true;
                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("StationDetails", "Transportation", new { Id = contact.StationId });
        }

        public async Task<ActionResult> DeleteStationContact(int? Id)
        {
            SetIdenitiy();
            if (userId == null || userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var contact = new Contact();

            try
            {
                contact = await db.Contacts.FindAsync(Id);
                if (contact == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                contact.Active = false;
                contact.ModifiedDate = DateTime.Now;
                contact.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("StationDetails", "Transportation", new { Id = contact.StationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateStationPhoto(Gallery gallery, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                if (uploadFile == null)
                    return View();

                uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Transportation/Stations/" + uploadFile.FileName));


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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("StationDetails", "Transportation", new { Id = gallery.StationId });
        }

        public ActionResult CreateStationPhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            return View(new Gallery() { StationId = Id });
        }

        public async Task<ActionResult> DeleteStationPhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var station = new Gallery();

            try
            {
                station = await db.Galleries.FindAsync(Id);
                if (station == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                station.Active = false;
                station.ModifiedDate = DateTime.Now;
                station.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }
            return RedirectToAction("StationDetails", "Transportation", new { Id = station.StationId });
        }

        public async Task<ActionResult> CreateLineBus(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                var line = await db.TransportationCompanyLines.FindAsync(Id);
                ViewBag.Buses = await db.Buses.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }
            return View(new LineBus() { LineId = (int)Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateLineBus(LineBus lineBus)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            try
            {
                if(!ModelState.IsValid)
                {
                    var line = await db.TransportationCompanyLines.FindAsync(lineBus.LineId);
                    ViewBag.Buses = await db.Buses.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
                    return View(lineBus);
                }

                lineBus.Active = true;
                lineBus.CreatedByUserId = userId;
                lineBus.CreatedDate = DateTime.Now;
                db.LineBuses.Add(lineBus);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("LineDetails", "Transportation", new { id = lineBus.LineId });
        }

        public async Task<ActionResult> DeleteLineBus(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var station = new LineBus();

            try
            {
                station = await db.LineBuses.FindAsync(Id);
                if (station == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }

                station.Active = false;
                station.ModifiedDate = DateTime.Now;
                station.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("LineDetails", "Transportation", new { Id = station.LineId });
        }

        public async Task<ActionResult> CreateLineStation(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                var line = await db.TransportationCompanyLines.FindAsync(Id);
                ViewBag.Stations = await db.Stations.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }
            return View(new LineStation() { LineId = (int)Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateLineStation(LineStation lineBus)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            try
            {
                var line = await db.TransportationCompanyLines.FindAsync(lineBus.LineId);
                if (!ModelState.IsValid)
                {
                    ViewBag.Stations = await db.Stations.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
                    return View(lineBus);
                }
                if (lineBus.StoppinByOrder <= 0)
                {
                    ViewBag.Stations = await db.Stations.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
                    return View(lineBus);
                }
                var lines = await db.LineStations.Where(l => l.Active == true && l.LineId == lineBus.LineId).ToListAsync();
                foreach (var item in lines)
                {
                    if (item.StoppinByOrder == lineBus.StoppinByOrder)
                    {
                        ViewBag.Stations = await db.Stations.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
                        return View(lineBus);
                    }
                }
                int len = lines.Count;
                if (len > 0)
                {
                    int x = lineBus.StoppinByOrder - lines[len - 1].StoppinByOrder;
                    if (x > 1)
                    {
                        ViewBag.Stations = await db.Stations.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
                        return View(lineBus);
                    }
                }
                else if(len <= 0 && lineBus.StoppinByOrder > 1 ) 
                {
                    ViewBag.Stations = await db.Stations.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
                    return View(lineBus);
                }
                lineBus.Active = true;
                lineBus.CreatedByUserId = userId;
                lineBus.CreatedDate = DateTime.Now;
                db.LineStations.Add(lineBus);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }


            return RedirectToAction("LineDetails", "Transportation", new { id = lineBus.LineId });
        }

        public async Task<ActionResult> DeleteLineStation(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var station = new List<LineStation>();

            try
            {
                station = await db.LineStations.Where(l => l.Active == true && l.LineId == Id).ToListAsync();
                if (station == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
                }
                foreach (var item in station)
                {
                    item.Active = false;
                    item.ModifiedDate = DateTime.Now;
                    item.ModifiedByUserId = userId;
                }
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("LineDetails", "Transportation", new { Id = station[0].LineId });
        }

        public ActionResult CreateAttachment(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            return View(new Attachment() { CompanyId = id });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
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
                uploadFile.SaveAs(HttpContext.Server.MapPath("~/Attachments/TransportationCompany/" + formattedFileName));

                attachment.SavedFileName = formattedFileName;

                db.Attachments.Add(attachment);
                await db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { Id = attachment.CompanyId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            string fullName = Server.MapPath("~/Attachments/TransportationCompany/" + filePath);
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var att = new Attachment();

            try
            {
                att = await db.Attachments.FindAsync(id);

                if (att == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "attachement is null", UserId = userId });
                }

                att.Active = false;
                att.ModifiedByUserId = userId;
                att.ModifiedDate = DateTime.Now;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { id = att.CompanyId });
        }

        public async Task<ActionResult> Confirm(int? id, bool? isConfirm)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                var company = await db.Companies.FindAsync(id);

                if (company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "company is null", UserId = userId });
                }

                company.IsConfirmed = isConfirm;
                company.ModifiedDate = DateTime.Now;
                company.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Index");
        }

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