using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace CSIT314BCE.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { set; get; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<SignInStatus> Login(LoginViewModel model, ApplicationSignInManager SignInManager) {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            return result;
        }

        public async Task<bool> Register(RegisterViewModel model, ApplicationUserManager userManager, ApplicationSignInManager signInManager) {
            var user = new Student { UserName = model.UserName, Email = model.Email, FullName = model.Fullname, Ratings = 0 };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                await userManager.AddToRoleAsync(user.Id, "Student");
                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return true;
            }
            return false;
        }

        public void LogOff(IAuthenticationManager authenticationManager) {
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}