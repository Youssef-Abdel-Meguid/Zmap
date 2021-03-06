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
        private ZmapEntities logger = new ZmapEntities();

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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(companies);
        }

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

                companyDto = new TransportationDetailsDto()
                {
                    Id = company.Id,
                    Name = company.Name,
                    IsConfirmed = company.IsConfirmed
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(companyDto);
        }

        public async Task<ActionResult> Buses(int? id)
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

            var busDetails = new BusDetailsDto();

            try
            {
                var buses = await db.Buses.Where(b => b.Active == true && b.CompanyId == id).ToListAsync();
                var busDto = new List<BusDto>();
                foreach (var bus in buses)
                {
                    var seatMap = await db.BusSeatsMaps.FindAsync(bus.SeatsMapId);
                    var cat = await db.BusCategories.FindAsync(bus.CategoryId);
                    busDto.Add(new BusDto()
                    {
                        BusNumber = bus.BusNumber,
                        CreatedDate = bus.CreatedDate,
                        Id = bus.Id,
                        Name = bus.Name,
                        CategoryId = bus.CategoryId,
                        Category = cat.CategoryName,
                        BusSeatMapId = bus.SeatsMapId,
                        BusSeatMap = seatMap.MapName,
                        CompanyId = bus.CompanyId
                    });
                }

                busDetails = new BusDetailsDto()
                {
                    CompanyId = id,
                    Buses = busDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(busDetails);
        }

        public async Task<ActionResult> Stations(int? id)
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

            var stationDetails = new StationDetailsDto();

            try
            {
                var stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == id).ToListAsync();
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

                stationDetails = new StationDetailsDto()
                {
                    CompanyId = id,
                    Stations = stationDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(stationDetails);
        }

        public async Task<ActionResult> Lines(int? id)
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

            var lineDetails = new LineDetailsDto();

            try
            {
                var lines = await db.TransportationCompanyLines.Where(l => l.Active == true && l.CompanyId == id).ToListAsync();
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

                lineDetails = new LineDetailsDto()
                {
                    CompanyId = id,
                    Lines = lineDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(lineDetails);
        }

        public async Task<ActionResult> Gallery(int? id, int flag = 0)
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

            var photoDetails = new PhotoDetailsDto();

            try
            {
                if (flag == 0)
                {
                    photoDetails = new PhotoDetailsDto()
                    {
                        CompanyId = id,
                        Photos = await db.Galleries.Where(g => g.Active == true && g.CompanyId == id).ToListAsync()
                    };
                }
                else if (flag == 1)
                {
                    photoDetails = new PhotoDetailsDto()
                    {
                        BusId = id,
                        Photos = await db.Galleries.Where(g => g.Active == true && g.BusId == id).ToListAsync()
                    };
                }
                else if (flag == 2)
                {
                    photoDetails = new PhotoDetailsDto()
                    {
                        StationId = id,
                        Photos = await db.Galleries.Where(g => g.Active == true && g.StationId == id).ToListAsync()
                    };
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(photoDetails);
        }

        public async Task<ActionResult> Contacts(int? id, bool isCompany = true)
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

            var contacts = new ContactDetailsDto();

            try
            {
                if (isCompany == true)
                {
                    contacts = new ContactDetailsDto()
                    {
                        Contacts = await db.Contacts.Where(c => c.Active == true && c.CompanyId == id).ToListAsync(),
                        CompanyId = id
                    };
                }
                else
                {
                    contacts = new ContactDetailsDto()
                    {
                        Contacts = await db.Contacts.Where(c => c.Active == true && c.StationId == id).ToListAsync(),
                        StationId = id
                    };
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
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

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var files = new FilesDetailsDto();

            try
            {
                files = new FilesDetailsDto()
                {
                    Files = await db.Attachments.Where(a => a.Active == true && a.CompanyId == id).ToListAsync(),
                    CompanyId = id
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
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

            if (userType != 1 && userType != 3)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var faqs = new FAQDetailsDto();

            try
            {
                faqs = new FAQDetailsDto()
                {
                    FAQs = await db.FAQs.Where(f => f.Active == true && f.CompanyId == id).ToListAsync(),
                    CompanyId = id
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(faqs);
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
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }


            return View(lineDto);
        }

        public async Task<ActionResult> LineBuses(int? id)
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

            var lineBusDetails = new LineBusDetailsDto();

            try
            {
                var lineBusesDto = new List<LineBusDto>();
                var lineBuses = await db.LineBuses.Where(l => l.Active == true && l.LineId == id).ToListAsync();
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

                lineBusDetails = new LineBusDetailsDto()
                {
                    LineId = id,
                    Buses = lineBusesDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(lineBusDetails);
        }

        public async Task<ActionResult> LineStations(int? id)
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

            var lineStationDetails = new LineStationDetailsDto();

            try
            {
                var lineStationsDto = new List<LineStationDto>();
                var lineStations = await db.LineStations.Where(s => s.Active == true && s.LineId == id).ToListAsync();

                foreach (var lineStation in lineStations)
                {
                    var station = await db.Stations.FindAsync(lineStation.StationId);
                    lineStationsDto.Add(new LineStationDto()
                    {
                        Id = lineStation.Id,
                        LineId = id,
                        StoppingByOrder = lineStation.StoppinByOrder,
                        StationId = lineStation.StationId,
                        StationName = station.Name
                    });
                }

                lineStationDetails = new LineStationDetailsDto()
                {
                    LineId = id,
                    Stations = lineStationsDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(lineStationDetails);
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
                    Photo = await db.Galleries.Where(a => a.Active == true && a.StationId == station.id).FirstOrDefaultAsync(),
                    Name = station.Name,
                    GoogleMapLocation = station.GoogleMapLocation,
                    CreatedDate = station.CreatedDate,
                    CityId = station.CityId,
                    City = city.ArabicCityName,
                    CompanyId = station.CompanyId
                };

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(stationDto);
        }

        public ActionResult Create(bool? added)
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
                    return RedirectToAction("Create", "Trasnportation", new { added = true });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(company);
        }

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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(company);
        }

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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(company);
        }

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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreatePhoto", "Transportation", new { Id = gallery.CompanyId, added = true });
        }

        public ActionResult CreatePhoto(int? Id, bool? added)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Gallery", "Transportation", new { Id = galley.CompanyId });
        }

        public ActionResult CreateContact(int? Id, bool? added)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateContact", "Transportation", new { Id = contact.CompanyId, added = true });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }
            return RedirectToAction("Contacts", "Transportation", new { Id = contact.CompanyId });
        }

        public ActionResult CreateFAQ(int? id, bool? added)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateFAQ", "Transportation", new { Id = fAQ.CompanyId, added = true });
        }

        public async Task<ActionResult> CreateBus(int? id, bool? added)
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
                ViewBag.BusCategory = await db.BusCategories.Where(c => c.Active == true).ToListAsync();
                ViewBag.SeatMap = await db.BusSeatsMaps.Where(s => s.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateBus", "Transportation", new { Id = bus.CompanyId, added = true });
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

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                ViewBag.Cities = await db.Cities.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateStation", "Transportation", new { Id = station.CompanyId, added = true});
        }

        public async Task<ActionResult> CreateLine(int? id, bool? added)
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
                ViewBag.Stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == id).ToListAsync();
                ViewBag.Cities = await db.Cities.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
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

                if (!ModelState.IsValid)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateLine", "Transportation", new { Id = line.CompanyId, added = true });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("FAQs", "Transportation", new { Id = faq.CompanyId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Buses", "Transportation", new { Id = bus.CompanyId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Stations", "Transportation", new { Id = station.CompanyId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Lines", "Transportation", new { Id = line.CompanyId });
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
               
                busDto = new BusDto()
                {
                    Id = bus.Id,
                    Name = bus.Name,
                    CompanyId = bus.CompanyId
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(busDto);
        }

        public async Task<ActionResult> BusAddsOn(int? id)
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

            var addOnDetails = new AddOnDetailsDto();

            try
            {
                var busAdds = await db.BusAddOns.Where(b => b.Active == true && b.BusId == id).ToListAsync();
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

                addOnDetails = new AddOnDetailsDto()
                {
                    BusId = id,
                    AddOns = busAddDtos
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(addOnDetails);
        }

        public async Task<ActionResult> BusTrips(int? busId, int? lineId)
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
            if (busId == null || lineId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var busTripDetails = new BusTripDetailsDto();

            try
            {
                var bus = await db.Buses.FindAsync(busId);

                if(bus == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "bus is null", UserId = userId });
                }

                var line = await db.TransportationCompanyLines.FindAsync(lineId);

                if (line == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "line is null", UserId = userId });
                }

                var trips = await db.BusTripSchedules.Where(b => b.Active == true && b.BusId == busId && b.LineId == lineId).ToListAsync();
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

                busTripDetails = new BusTripDetailsDto()
                {
                    BusId = busId,
                    LineId = lineId,
                    BusName = bus.Name,
                    BusTrips = tripsDto
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(busTripDetails);
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateBusPhoto", "Transportation", new { Id = gallery.BusId, added = true });
        }

        public ActionResult CreateBusPhoto(int? Id, bool? added)
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

            return View(new Gallery() { BusId = Id });
        }

        public ActionResult CreateBusAddOn(int? id, bool? added)
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
                ViewBag.AddsOn = db.AddsOns.Where(a => a.Active == true && a.IsBus == true).ToList();
            }
            catch (Exception e)
            {

                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateBusAddOn", "Transportation", new { Id = busAddOn.BusId, added = true });
        }

        public async Task<ActionResult> CreateBusTrip(int? busId, int? lineId, bool? added)
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

            if (busId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            try
            {
                var bus = await db.Buses.FindAsync(busId);
                ViewBag.Stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == bus.CompanyId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(new BusTripSchedule() { BusId = (int)busId, LineId = lineId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateBusTrip", "Transportation", new { busId = trip.BusId, lineId = trip.LineId, added = true });
        }

        public async Task<ActionResult> EditBusTrip(int? busId, int? id)
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

            if (busId == null || id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            var busTrip = new BusTripSchedule();

            try
            {
                var bus = await db.Buses.FindAsync(busId);
                ViewBag.Stations = await db.Stations.Where(s => s.Active == true && s.CompanyId == bus.CompanyId).ToListAsync();
                busTrip = await db.BusTripSchedules.FindAsync(id);
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(busTrip);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBusTrip(BusTripSchedule busTripSchedule)
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
                if(ModelState.IsValid)
                {
                    busTripSchedule.ModifiedDate = DateTime.Now;
                    busTripSchedule.ModifiedByUserId = userId;
                    db.Entry(busTripSchedule).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("BusTrips", "Transportation", new { busId = busTripSchedule.BusId, lineId = busTripSchedule.LineId });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(busTripSchedule);
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Gallery", "Transportation", new { Id = station.BusId, flag = 1 });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("BusAddsOn", "Transportation", new { Id = station.BusId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("BusTrips", "Transportation", new { busId = station.BusId, lineId = station.LineId });
        }

        public ActionResult CreateStationContact(int? Id, bool? added)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateStationContact", "Transportation", new { Id = contact.StationId, added = true });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Contacts", "Transportation", new { Id = contact.StationId, isCompany = false });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateStationPhoto", "Transportation", new { Id = gallery.StationId, added = true });
        }

        public ActionResult CreateStationPhoto(int? Id, bool? added)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }
            return RedirectToAction("Gallery", "Transportation", new { Id = station.StationId, flag = 2 });
        }

        public async Task<ActionResult> CreateLineBus(int? Id, bool? added)
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

            try
            {
                var line = await db.TransportationCompanyLines.FindAsync(Id);
                ViewBag.Buses = await db.Buses.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }
            return View(new LineBus() { LineId = (int)Id });
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
                if (!ModelState.IsValid)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("CreateLineBus", "Transportation", new { id = lineBus.LineId, added = true });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("LineBuses", "Transportation", new { Id = station.LineId });
        }

        public async Task<ActionResult> CreateLineStation(int? Id, bool? added)
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

            try
            {
                var line = await db.TransportationCompanyLines.FindAsync(Id);
                ViewBag.Stations = await db.Stations.Where(b => b.Active == true && b.CompanyId == line.CompanyId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
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
                else if (len <= 0 && lineBus.StoppinByOrder > 1)
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }


            return RedirectToAction("CreateLineStation", "Transportation", new { id = lineBus.LineId, added = true });
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
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "station is null", UserId = userId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("LineStations", "Transportation", new { Id = station[0].LineId });
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

            return RedirectToAction("CreateAttachment", "Transportation", new { Id = attachment.CompanyId, added = true });
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

            return RedirectToAction("Files", "Transportation", new { id = att.CompanyId });
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

        public async Task<ActionResult> BusTripReservations(int? id)
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

            var reservedSeatsDto = new ReservedSeatsDto();

            try
            {
                var busTrip = await db.BusTripSchedules.FindAsync(id);
                var bus = await db.Buses.FindAsync(busTrip.BusId);
                var map = await db.BusSeatsMaps.FindAsync(bus.SeatsMapId);

                if(busTrip == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "bus trip is null", UserId = userId });
                }

                var reservedSeats = await db.ReservedSeats.Where(r => r.Active == true && r.BusTripId == id).ToListAsync();
                var seatsNumber = new List<string>();

                foreach (var item in reservedSeats)
                {
                    seatsNumber.Add(item.SeatNumber);
                }


                reservedSeatsDto = new ReservedSeatsDto()
                {
                    ReservedSeats = reservedSeats,
                    BusTripId = id,
                    BusId = bus.Id,
                    NumberOfSeats = map.NumberOfSeats,
                    SeatsNumber = seatsNumber,
                    LineId = busTrip.LineId
                };
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return View(reservedSeatsDto);
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<bool> UpdateBusTripReservations([System.Web.Http.FromUri]int? id, [System.Web.Http.FromBody]List<string> seatsNumber)
        //{
        //    SetIdenitiy();

        //    try
        //    {

        //        foreach (var item in seatsNumber)
        //        {

        //            var reservedSeat = new ReservedSeat()
        //            {
        //                Active = true,
        //                BusTripId = id,
        //                ByAdmin = true,
        //                CreatedByUserId = userId,
        //                CreatedDate = DateTime.Now,
        //                SeatNumber = item
        //            };

        //            db.ReservedSeats.Add(reservedSeat);
        //        }

        //        await db.SaveChangesAsync();


        //    }
        //    catch (Exception e)
        //    {

        //        logger.ErrorLoggers.Add(new ErrorLogger()
        //        {
        //            ActionName = "update seats map",
        //            CreatedDate = DateTime.Now,
        //            Error = e.Message.ToString(),
        //            UserId = userId
        //        });

        //        await logger.SaveChangesAsync();
        //        return false;
        //    }

        //    return true;

        //}

        public ActionResult LineBusDetails(int? lineId, int? busId)
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

            if (lineId == null || busId == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = "id is null", UserId = userId });
            }

            return View(new BusDto() { Id = (int)busId, LineId = (int)lineId });
        }

        public async Task<ActionResult> CopyBusTrip(int? id)
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

            var busTrip = new BusTripSchedule();

            try
            {
                busTrip = await db.BusTripSchedules.FindAsync(id);
                var newBusTrip = new BusTripSchedule()
                {
                    Active = true,
                    ArrivalTime = busTrip.ArrivalTime,
                    ArrivalDate = busTrip.ArrivalDate,
                    BusId = busTrip.BusId,
                    CreatedByUserId = userId,
                    CreatedDate = DateTime.Now,
                    DepartureDate = busTrip.DepartureDate,
                    DepartureTime = busTrip.DepartureTime,
                    IsCompleted = busTrip.IsCompleted,
                    LineId = busTrip.LineId,
                    StationFromId = busTrip.StationFromId,
                    TripName = busTrip.TripName,
                    StationToId = busTrip.StationToId
                };

                db.BusTripSchedules.Add(newBusTrip);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in transportations", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("BusTrips", "Transportation", new { busId = busTrip.BusId, lineId = busTrip.LineId });
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