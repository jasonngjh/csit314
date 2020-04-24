using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSIT314BCE.Models
{
    public class Student : ApplicationUser
    {
        public int? Ratings { get; set; }
    }
}