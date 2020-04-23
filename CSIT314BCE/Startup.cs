using CSIT314BCE.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CSIT314BCE.Startup))]
namespace CSIT314BCE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRootUserAndRoles();
        }

        public void CreateRootUserAndRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<Admin>(new UserStore<Admin>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                //create admin role
                var role = new IdentityRole("Admin");
                roleManager.Create(role);

                //create root user
                Admin user = new Admin {
                    UserName = "root",
                    Email = "root@domain.com",
                };
                string pwd = "@Abc123";

                var newuser = userManager.Create(user, pwd);
                if (newuser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            if (!roleManager.RoleExists("Student")) {
                var role = new IdentityRole("Student");
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Moderator"))
            {
                var role = new IdentityRole("Moderator");
                roleManager.Create(role);
            }
        }
    }
}
