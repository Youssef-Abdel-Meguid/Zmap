using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class NewUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Phonenumber { get; set; }
        public string Password { get; set; }
        public DateTime BirthOfDate { get; set; }
        public int UserTypeId { get; set; }
    }
}