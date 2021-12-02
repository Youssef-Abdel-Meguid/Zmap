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
using System.Security.Cryptography;
using System.Text;

namespace Zmap.Controllers
{
    public class UsersController : Controller
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

            if (userType != 1 )
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "not authorized", UserId = userId });
            }

            var usersDto = new List<UserDto>();

            try
            {
                var users = await db.Users.Where(u => u.Active == true).ToListAsync();
                foreach (var user in users)
                {
                    var userType = await db.UserTypes.FindAsync(user.UserTypeId);
                    usersDto.Add(new UserDto()
                    {
                        CreatedDate = user.CreatedDate,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        Id = user.Id,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        UserTypeId = user.UserTypeId,
                        Username = user.UserName,
                        UserType = userType == null ? null : userType.Name
                    });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = e.Message.ToString() , UserId = userId });
            }

            return View(usersDto);
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
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "id is null", UserId = userId });
            }

            User user = new User();

            try
            {
                user = await db.Users.FindAsync(id);
                if (user == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "user is null", UserId = userId });
                }
                ViewBag.UserTypes = await db.UserTypes.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = e.Message.ToString() , UserId = userId });
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User user)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    user.ModifiedDate = DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.UserTypes = await db.UserTypes.ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = e.Message.ToString() , UserId = userId });
            }

            return View(user);
        }

        public async Task<ActionResult> ChangeAdminPassword(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "id is null", UserId = userId });
            }

            ChangeAdminPasswordDto changeAdminPasswordDto = new ChangeAdminPasswordDto();

            try
            {
                var user = await db.Users.FindAsync(id);
                if (user == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "user is null", UserId = userId });
                }
                changeAdminPasswordDto.UserId = user.Id;
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = e.Message.ToString(), UserId = userId });
            }

            return View(changeAdminPasswordDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeAdminPassword(ChangeAdminPasswordDto changeAdminPasswordDto)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await db.Users.FindAsync(changeAdminPasswordDto.UserId);

                    if(user == null)
                    {
                        return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "user is null", UserId = userId });

                    }

                    var hmac = new HMACSHA512();

                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changeAdminPasswordDto.Password));
                    user.PasswordSalt = hmac.Key;
                    user.ModifiedDate = DateTime.Now;

                    db.Entry(user).State = EntityState.Modified;

                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = e.Message.ToString(), UserId = userId });
            }

            return View(changeAdminPasswordDto);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "not authorized", UserId = userId });
            }
            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "id is null", UserId = userId });
            }

            User user = new User();

            try
            {
                user = await db.Users.FindAsync(id);
                if (user == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "user is null", UserId = userId });
                }
                user.Active = false;
                user.ModifiedDate = DateTime.Now;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = e.Message.ToString() , UserId = userId });
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Create()
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "not authorized", UserId = userId });
            }

            try
            {
                ViewBag.Types = await db.UserTypes.Where(u => u.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = e.Message.ToString() , UserId = userId });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewUserDto registerDto)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = "not authorized", UserId = userId });
            }

            try
            {
                if(!ModelState.IsValid)
                {
                    ViewBag.Types = await db.UserTypes.Where(u => u.Active == true).ToListAsync();
                    return View(registerDto);
                }
                var isUsernameExists = await db.Users.Where(u => u.Active == true && u.UserName == registerDto.Username).AnyAsync();
                if (isUsernameExists)
                {
                    ViewBag.Types = await db.UserTypes.Where(u => u.Active == true).ToListAsync();
                    ModelState.AddModelError("Username", "Username is taken");
                    return View(registerDto);
                }
                var isEmailExists = await db.Users.Where(u => u.Active == true && u.Email == registerDto.Email).AnyAsync();
                if (isEmailExists)
                {
                    ViewBag.Types = await db.UserTypes.Where(u => u.Active == true).ToListAsync();
                    ModelState.AddModelError("Email", "Email is taken");
                    return View(registerDto);
                }
                var isPhoneNumberExists = await db.Users.Where(u => u.Active == true && u.PhoneNumber == registerDto.Phonenumber).AnyAsync();
                if (isPhoneNumberExists)
                {
                    ViewBag.Types = await db.UserTypes.Where(u => u.Active == true).ToListAsync();
                    ModelState.AddModelError("Phonenumber", "Phonenumber is taken");
                    return View(registerDto);
                }

                var hmac = new HMACSHA512();

                var user = new User()
                {
                    Active = true,
                    CreatedDate = DateTime.Now,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PhoneNumber = registerDto.Phonenumber,
                    UserName = registerDto.Username,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                    PasswordSalt = hmac.Key,
                    BirthDate = registerDto.BirthOfDate,
                    UserTypeId = registerDto.UserTypeId
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in users", Error = e.Message.ToString(), UserId = userId });
            }

            return RedirectToAction("Index", "Users");
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