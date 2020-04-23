using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CSIT314BCE.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [StringLength(256)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Suspended Till")]
        public DateTime? LockoutEndDateUtc { get; set; }

        [Display(Name = "Suspended")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Role")]
        public string Discriminator { get; set; }
        [Display(Name = "Ratings")]
        public int? Ratings { get; set; }
    }

    public class ResetUserPasswordViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class CreateUserViewModel
    {
    }
}