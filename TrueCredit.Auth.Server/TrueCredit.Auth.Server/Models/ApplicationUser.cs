using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCredit.Auth.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreateTime { get; set; }
    }
}
