using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zmap.Dtos
{
    public class ChildPolicyDetailsDto
    {
        public int? RoomId { get; set; }
        public List<ChildPolicyDto> ChildPolicies { get; set; }
    }
}