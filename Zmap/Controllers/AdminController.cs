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
    public class AdminController : Controller
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
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            var about = await db.AboutUs.FirstOrDefaultAsync();
            var contact = await db.ContactUs.FirstOrDefaultAsync();
            var services = await db.OurServices.FirstOrDefaultAsync();

            return View(new PageDto() 
            {
                AboutUs = about,
                OurService = services,
                ContactUs = contact
            });
        }

        public async Task<ActionResult> SevicesDetails(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "id is null", UserId = userId });
            }

            var serviceDto = new ServicesDto();

            try
            {
                var service = await db.OurServices.FindAsync(id);
                if (service == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "service is null", UserId = userId });
                }

                serviceDto = new ServicesDto()
                {
                    id = service.Id,
                    Details = service.Details,
                    Title = service.Title,
                    Services = await db.Services.Where(s => s.Active == true && s.OurServiceId == service.Id).ToListAsync()
                };

            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return View(serviceDto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "id is null", Error = "not authorized", UserId = userId });
            }

            var aboutU = new AboutU();

            try
            {
                aboutU = await db.AboutUs.FindAsync(id);
                if (aboutU == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "about is null", Error = "not authorized", UserId = userId });
                }
            }
            catch (Exception e)
            {

                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return View(aboutU);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AboutU aboutU, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (uploadFile != null)
                    {
                        uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Admin/About/" + uploadFile.FileName));
                        aboutU.PhotoUrl = uploadFile.FileName;
                    }
                    db.Entry(aboutU).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }
            return View(aboutU);
        }

        public async Task<ActionResult> ContactEdit(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "id is null", UserId = userId });
            }

            var contactU = new ContactU();

            try
            {
                contactU = await db.ContactUs.FindAsync(id);
                if (contactU == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "contact is null", UserId = userId });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return View(contactU);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactEdit(ContactU contactU)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(contactU).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return View(contactU);
        }

        public async Task<ActionResult> ServiceEdit(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "id is null", UserId = userId });
            }

            var service = new OurService();

            try
            {
                service = await db.OurServices.FindAsync(id);
                if (service == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "service is null", UserId = userId });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ServiceEdit(OurService service)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(service).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return View(service);
        }

        public async Task<ActionResult> CreateService()
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            var service = new OurService();

            try
            {
                service = await db.OurServices.FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return View(new Service() { OurServiceId = service.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateService(Service service, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            if (!ModelState.IsValid)
            {
                return View(service);
            }

            try
            {
                if(uploadFile != null)
                {
                    uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Admin/Services/") + uploadFile.FileName);
                    service.CreatedDate = DateTime.Now;
                    service.Active = true;
                    service.PhotoUrl = uploadFile.FileName;
                    service.CreateByUserId = userId;
                    db.Services.Add(service);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return RedirectToAction("SevicesDetails", "Admin", new { id = service.OurServiceId });
        }

        public async Task<ActionResult> DeleteService(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "id is null", UserId = userId });
            }

            var service = new Service();

            try
            {
                service = await db.Services.FindAsync(id);

                if(service == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = "service is null", UserId = userId });
                }

                service.Active = false;
                service.ModifiedDate = DateTime.Now;
                service.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in admin", Error = e.Message.ToString() + "\n" + e.InnerException.ToString(), UserId = userId });
            }

            return RedirectToAction("SevicesDetails", "Admin", new { id = service.OurServiceId });
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