using Microsoft.AspNetCore.Identity;
using MLSCore.Models;
using System;
using System.Collections.Generic;

using System.Text;

namespace MLSCore.IdentityModel
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public TbInstructor Instructor { get; set; } // Navigation
    }
}
