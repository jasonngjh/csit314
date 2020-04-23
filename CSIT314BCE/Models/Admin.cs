using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSIT314BCE.Models
{
    public class Admin : ApplicationUser
    {
        ApplicationDbContext context = new ApplicationDbContext();


        public List<ApplicationUser> GetUserList() {
            return context.Users.ToList();
        }

        public bool CreateUser() {



            return true;
        }

        public EditUserViewModel EditUser(ApplicationUser user) {
            EditUserViewModel editUserViewModel = new EditUserViewModel();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var role = userManager.GetRoles(user.Id);

            if (role.FirstOrDefault() == "Student")
            {
                var store = new UserStore<Student>(context);
                var studentManager = new UserManager<Student>(new UserStore<Student>(context));
                Student student = studentManager.FindById(user.Id);
                editUserViewModel.Id = student.Id;
                editUserViewModel.Email = student.Email;
                editUserViewModel.FullName = student.FullName;
                editUserViewModel.UserName = student.UserName;
                editUserViewModel.LockoutEnabled = student.LockoutEnabled;
                editUserViewModel.LockoutEndDateUtc = student.LockoutEndDateUtc;
                editUserViewModel.Ratings = student.Ratings;
            }
            else 
            {
                editUserViewModel.Id = user.Id;
                editUserViewModel.Email = user.Email;
                editUserViewModel.FullName = user.FullName;
                editUserViewModel.UserName = user.UserName;
                editUserViewModel.LockoutEnabled = user.LockoutEnabled;
                editUserViewModel.LockoutEndDateUtc = user.LockoutEndDateUtc;
            }
           /* As of now Admin and moderator does not have additional attributes. so we do not need to cast it.
            * else if (role.FirstOrDefault() == "Admin")
            {
            }
            else if(role.FirstOrDefault() == "Moderator") { 
            }*/

            return editUserViewModel;
        }

        /*        public bool EditUser(EditUserViewModel model) {
                    switch (model.Discriminator) {
                        case "Student": var store = new UserStore<Student>(context);
                                        var userManager = new UserManager<Student>(new UserStore<Student>(context));

                                        break;
                        case "Moderator": break;
                        case "Admin": break;
                    }
                }*/
    }
}