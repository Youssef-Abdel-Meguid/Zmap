using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zmap.Models;
using Zmap.Dtos;
using System.Text;
using System.Security.Cryptography;
using System.Data.Entity;
using System.Web;
using System.Collections.Generic;

namespace Zmap.Controllers
{

    public class AccountController : Controller
    {
        private ZmapEntities db = new ZmapEntities();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {

            if(!ModelState.IsValid)
                return View(registerDto);

            try
            {
                var isUsernameExists = await db.Users.Where(u => u.Active == true && u.UserName == registerDto.Username).AnyAsync();
                if (isUsernameExists)
                {
                    ModelState.AddModelError("Username", "Username is taken");
                    return View(registerDto);
                }
                var isEmailExists = await db.Users.Where(u => u.Active == true && u.Email == registerDto.Email).AnyAsync();
                if (isEmailExists)
                {
                    ModelState.AddModelError("Email", "Email is taken");
                    return View(registerDto);
                }
                var isPhoneNumberExists = await db.Users.Where(u => u.Active == true && u.PhoneNumber == registerDto.Phonenumber).AnyAsync();
                if (isPhoneNumberExists)
                {
                    ModelState.AddModelError("Phonenumber", "Phonenumber is taken");
                    return View(registerDto);
                }
                if (registerDto.Password != registerDto.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password not match");
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
                    UserTypeId = 5
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                    new ErrorLogger() { ActionName = "Error in regestration", Error = e.Message.ToString() });
            }

            return RedirectToAction("Login", "Account");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var user = new User();

            try
            {
                user = await db.Users.Where(u => u.UserName == loginDto.Username).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                    new ErrorLogger() { ActionName = "Error in login", Error = e.Message.ToString() });
            }

            if (user == null)
            {
                ModelState.AddModelError("Username", "Username is invalid");
                return View(loginDto);
            }

            var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != user.PasswordHash[i])
                {
                    ModelState.AddModelError("Password", "Password is invalid");
                    return View(loginDto);
                }

            try
            {
                var userType = await db.UserTypes.FindAsync(user.UserTypeId);
                string s = userType.Name.Trim().ToLower();

                if (userType.Name.Trim().ToLower() == "super admin")
                    Session["UserType"] = 1;
                else if (userType.Name.Trim().ToLower() == "hotel admin")
                    Session["UserType"] = 2;
                else if (userType.Name.Trim().ToLower() == "transportation admin")
                    Session["UserType"] = 3;
                else if (userType.Name.Trim().ToLower() == "activity admin")
                    Session["UserType"] = 4;
                else
                    Session["UserType"] = 5;

                Session.Timeout = 60;
                Session["UserId"] = user.Id;
            }
            catch (Exception e)
            {
                return RedirectToAction("TechnicalSupport", "Home",
                     new ErrorLogger() { ActionName = "Error in login", Error = e.Message.ToString() });
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("UserType");
            Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
        }
    }
}