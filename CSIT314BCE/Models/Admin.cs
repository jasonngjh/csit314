using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSIT314BCE.Models
{
    public class Admin : ApplicationUser
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public List<ApplicationUser> GetUserList() {
            return context.Users.ToList();
        }

        public async Task<bool> CreateUser(CreateUserViewModel model) {
            if (model.Role == "Student") 
            {
                var userManager = new UserManager<Student>(new UserStore<Student>(context));
                var user = new Student { UserName = model.UserName, Email = model.Email, FullName = model.Fullname, Ratings = 0 };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user.Id,"Student");
                    return true;
                }
            }
            else if(model.Role == "Admin") 
            {
                var userManager = new UserManager<Admin>(new UserStore<Admin>(context));
                var user = new Admin { UserName = model.UserName, Email = model.Email, FullName = model.Fullname};
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user.Id, "Admin");
                    return true;
                }
            }
            else if(model.Role == "Moderator")
            {
                var userManager = new UserManager<Moderator>(new UserStore<Moderator>(context));
                var user = new Moderator { UserName = model.UserName, Email = model.Email, FullName = model.Fullname };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user.Id, "Moderator");
                    return true;
                }
            }
            return false;
        }

        public EditUserViewModel EditUser(ApplicationUser user) {
            EditUserViewModel editUserViewModel = new EditUserViewModel();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var role = userManager.GetRoles(user.Id);

            if (role.FirstOrDefault() == "Student")
            {
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

        public async Task<bool> EditUser(EditUserViewModel model)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var role = userManager.GetRoles(model.Id);

            if (role.FirstOrDefault() == "Student")
            {
                var studentStore = new UserStore<Student>(context);
                var studentManager = new UserManager<Student>(new UserStore<Student>(context));
                Student user = studentManager.FindById(model.Id);

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.LockoutEnabled = model.LockoutEnabled;
                user.LockoutEndDateUtc = model.LockoutEndDateUtc;
                user.Ratings = model.Ratings;

                var result = await studentManager.UpdateAsync(user);
                var ctx = studentStore.Context;
                ctx.SaveChanges();

                if (result.Succeeded) {
                    return true;
                }
            }
            else if (role.FirstOrDefault() == "Admin")
            {
                var adminStore = new UserStore<Admin>(context);
                var adminManager = new UserManager<Admin>(new UserStore<Admin>(context));
                Admin user = adminManager.FindById(model.Id);

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.LockoutEnabled = model.LockoutEnabled;
                user.LockoutEndDateUtc = model.LockoutEndDateUtc;

                var result = await adminManager.UpdateAsync(user);
                var ctx = adminStore.Context;
                ctx.SaveChanges();

                if (result.Succeeded)
                {
                    return true;
                }
            }
            else if (role.FirstOrDefault() == "Moderator")
            {
                var moderatorStore = new UserStore<Moderator>(context);
                var moderatorManager = new UserManager<Moderator>(new UserStore<Moderator>(context));
                Moderator user = moderatorManager.FindById(model.Id);

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.LockoutEnabled = model.LockoutEnabled;
                user.LockoutEndDateUtc = model.LockoutEndDateUtc;

                var result = await moderatorManager.UpdateAsync(user);
                var ctx = moderatorStore.Context;
                ctx.SaveChanges();

                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}