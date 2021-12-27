using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zmap.Models;

namespace Zmap.Dtos
{
    public class ActivityLockedDateDto
    {
        public int? ActivityId { get; set; }
        public List<ActivityLockedDate> activityLockedDates { get; set; }
    }
}