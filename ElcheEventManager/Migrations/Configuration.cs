namespace ElcheEventManager.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using ElcheEventManager.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ElcheEventManager.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ElcheEventManager.Models.ApplicationDbContext";
        }

        protected override void Seed(ElcheEventManager.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Create predefine roles
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole { Name = "Admin" };
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole { Name = "User" };
                roleManager.Create(role);
            }

            // Asign Admin rol to a user(optional)
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var adminUser = userManager.FindByEmail("admin@elche.es");
            if (adminUser != null && !userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }
        }
    }
}
