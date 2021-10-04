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
    public class ActivitesController : Controller
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
        // GET: Activites
        public async Task<ActionResult> Index()
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            List<Company> companies = new List<Company>();

            try
            {
                if (userType == 1)
                    companies = await db.Companies.Where(c => c.IsActivity == true && c.Active == true).ToListAsync();

                else
                    companies = await db.Companies.Where(c => c.IsActivity == true && c.Active == true && c.CreatedByUserId == userId).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return View(companies);
        }

        // GET: Activites/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            var copnayDto = new ActivityCompanyDto();

            try
            {
                var company = await db.Companies.FindAsync(id);

                if (company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "company is null", UserId = userId });
                }

                var activities = await db.Activities.Where(a => a.Active == true && a.CompanyId == company.Id).ToListAsync();
                var activitiesDto = new List<ActivityDto>();

                foreach (var item in activities)
                {
                    var cat = await db.ActivityCategories.FindAsync(item.ActivityCategoryId);
                    var subArea = await db.SubAreas.FindAsync(item.SubAreaId);
                    var city = await db.Cities.FindAsync(subArea.CityId);

                    activitiesDto.Add(new ActivityDto()
                    {
                        ActicityName = item.ActivityName,
                        ActivityCategory = cat == null ? null : cat.CategoryName,
                        CompanyId = company.Id,
                        Id = item.Id,
                        CreatedDate = item.CreatedDate,
                        CityId = city.Id,
                        CostWithoutTrans = item.CostWithoutTrasnportation,
                        CostWithTrans = item.CostWithTransportation,
                        Evening = item.Evening,
                        Morning = item.Morning,
                        City = city.ArabicCityName,
                        SubArea = subArea.Name,
                        SubAreaId = subArea.Id
                    });
                }

                copnayDto = new ActivityCompanyDto()
                {
                    About = company.About,
                    Id = company.Id,
                    PrivacyPolicy = company.PrivacyPolicy,
                    CreatedDate = company.CreatedDate,
                    Name = company.Name,
                    Contacts = await db.Contacts.Where(c => c.Active == true && c.CompanyId == company.Id).ToListAsync(),
                    Photos = await db.Galleries.Where(c => c.Active == true && c.CompanyId == company.Id).ToListAsync(),
                    FAQs = await db.FAQs.Where(c => c.Active == true && c.CompanyId == company.Id).ToListAsync(),
                    Activities = activitiesDto,
                    IsConfirmed = company.IsConfirmed,
                    Attachments = await db.Attachments.Where(a => a.Active == true && a.CompanyId == company.Id).ToListAsync(),
                    ManagerEmail = company.ManagerEmail,
                    ManagerPhonenumber = company.ManagerPhonenumber
                };

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }


            return View(copnayDto);
        }

        // GET: Activites/Create
        public ActionResult Create()
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            return View();
        }

        // POST: Activites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Company company)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    company.IsExternal = false;
                    company.IsActivity = true;
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return View(company);
        }

        // GET: Activites/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            Company company = new Company();

            try
            {
                company = await db.Companies.FindAsync(id);
                if (company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "company is null", UserId = userId });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
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

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return View(company);
        }

        // GET: Activites/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            Company company = new Company();

            try
            {
                company = await db.Companies.FindAsync(id);
                if (company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "company is null", UserId = userId });
                }
                company.Active = false;
                company.ModifiedDate = DateTime.Now;
                company.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Index", "Activites");
        }

        public ActionResult CreatePhoto(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
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

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            var galley = new Gallery();

            try
            {
                galley = await db.Galleries.FindAsync(Id);
                if (galley == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
                }

                galley.Active = false;
                galley.ModifiedDate = DateTime.Now;
                galley.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }
            return RedirectToAction("Details", "Activites", new { Id = galley.CompanyId });
        }

        public ActionResult CreateContact(int? Id)
        {
            SetIdenitiy();
            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if(Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
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

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { Id = contact.CompanyId });
        }

        public async Task<ActionResult> DeleteContact(int? Id)
        {
            SetIdenitiy();
            if (userId == null || userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            var contact = new Contact();

            try
            {
                contact = await db.Contacts.FindAsync(Id);
                if (contact == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
                }

                contact.Active = false;
                contact.ModifiedDate = DateTime.Now;
                contact.ModifiedByUserId = userId;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }
           
            return RedirectToAction("Details", "Activites", new { Id = contact.CompanyId });
        }

        public ActionResult CreateFAQ(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
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

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { Id = fAQ.CompanyId });
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

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                if (uploadFile == null)
                    return View();

                uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Activities/" + uploadFile.FileName));
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { Id = gallery.CompanyId });
        }

        public async Task<ActionResult> DeleteFAQ(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            var faq = new FAQ();

            try
            {
                faq = await db.FAQs.FindAsync(Id);
                if (faq == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
                }

                faq.Active = false;
                faq.ModifiedDate = DateTime.Now;
                faq.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }


            return RedirectToAction("Details", "Activites", new { Id = faq.CompanyId });
        }

        public async Task<ActionResult> CreateActivity(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            try
            {
                ViewBag.Cat = await db.ActivityCategories.Where(a => a.Active == true).ToListAsync();
                ViewBag.SubAreas = await db.SubAreas.Where(s => s.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return View(new Activity { CompanyId = (int)id, Morning = false, Evening = false});
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateActivity(Activity activity)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Cat = await db.ActivityCategories.Where(a => a.Active == true).ToListAsync();
                    ViewBag.SubAreas = await db.SubAreas.Where(s => s.Active == true).ToListAsync();
                    return View(activity);
                }

                if (activity.Morning == null)
                    activity.Morning = false;

                if (activity.Evening == null)
                    activity.Evening = false;

                activity.Active = true;
                activity.CreatedByUserId = userId;
                activity.CreatedDate = DateTime.Now;
                db.Activities.Add(activity);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { id = activity.CompanyId});
        }

        public async Task<ActionResult> DeleteActivity(int? Id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (Id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            var faq = new Activity();

            try
            {
                faq = await db.Activities.FindAsync(Id);
                if (faq == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
                }

                faq.Active = false;
                faq.ModifiedDate = DateTime.Now;
                faq.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {

                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { Id = faq.CompanyId });
        }

        public async Task<ActionResult> EditActivity(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            var company = new Activity();

            try
            {
                company = await db.Activities.FindAsync(id);
                if (company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
                }
                ViewBag.Cat = await db.ActivityCategories.Where(a => a.Active == true).ToListAsync();
                ViewBag.SubAreas = await db.SubAreas.Where(s => s.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditActivity(Activity activity)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    activity.ModifiedDate = DateTime.Now;
                    activity.ModifiedByUserId = userId;
                    db.Entry(activity).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Activites", new { id = activity.CompanyId});
                }
                else
                {
                    ViewBag.Cat = await db.ActivityCategories.Where(a => a.Active == true).ToListAsync();
                    ViewBag.SubAreas = await db.SubAreas.Where(s => s.Active == true).ToListAsync();
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { Id = activity.CompanyId });
        }

        public ActionResult CreateAttachment(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            return View(new Attachment() { CompanyId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAttachment(Attachment attachment ,HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
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

                var formattedFileName = string.Format("{0}-{1}",Guid.NewGuid().ToString(), uploadFile.FileName);
                uploadFile.SaveAs(HttpContext.Server.MapPath("~/Attachments/ActivityCompany/" + formattedFileName));

                attachment.SavedFileName = formattedFileName;

                db.Attachments.Add(attachment);
                await db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Details", "Activites", new { Id = attachment.CompanyId});
        }

        public ActionResult DownloadAttachment(string filePath)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            string fullName = Server.MapPath("~/Attachments/ActivityCompany/" + filePath);
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

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            var att = new Attachment();

            try
            {
                att = await db.Attachments.FindAsync(id);

                if(att == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "attachement is null", UserId = userId });
                }

                att.Active = false;
                att.ModifiedByUserId = userId;
                att.ModifiedDate = DateTime.Now;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString(), UserId = userId });
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

            if (userType != 1 && userType != 4)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "id is null", UserId = userId });
            }

            try
            {
                var company = await db.Companies.FindAsync(id);

                if(company == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = "company is null", UserId = userId });
                }

                company.IsConfirmed = isConfirm;
                company.ModifiedDate = DateTime.Now;
                company.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in activities", Error = e.Message.ToString(), UserId = userId });
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