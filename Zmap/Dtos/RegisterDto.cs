using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Phonenumber is required")]
        public string Phonenumber { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password not match")]
        [Display(Name = "Re-enter Password")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Birthdate is required")]
        [Display(Name = "Birthdate")]
        public DateTime BirthOfDate { get; set; }
    }
}