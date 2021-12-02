using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class UserHomeDto
    {
        public List<UserHomeBlogDto> UserHomeBlogDtos { get; set; }
        public List<UserHomeTripDto> UserHomeTripDtos { get; set; }
    }
}