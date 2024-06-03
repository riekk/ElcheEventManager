using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElcheEventManager.Models.dto
{
    public class UserWithRoles
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}