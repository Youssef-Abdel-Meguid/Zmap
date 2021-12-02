using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ChangeAdminPasswordDto
    {
        public int? UserId { get; set; }
        public string Password { get; set; }
    }
}