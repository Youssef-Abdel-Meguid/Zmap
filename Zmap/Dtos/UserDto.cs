using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserType { get; set; }
        public int UserTypeId { get; set; }
        public string Username { get; set; }
    }
}