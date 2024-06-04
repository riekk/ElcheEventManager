using ElcheEventManager.Models;
using ElcheEventManager.Models.db;
using ElcheEventManager.Models.dto;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ElcheEventManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private EntitiesEM db = new EntitiesEM();

        public ActionResult Index()
        {
            var roles = context.Roles.ToList();
            return View(roles);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new IdentityRole { Name = collection["RoleName"] });
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(string RoleName)
        {
            var thisRole = context.Roles.FirstOrDefault(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase));
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Users
        public ActionResult UserList()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            var users = db.AspNetUsers.Where(u => u.Email != "admin@elche.es").ToList();

            var usersWithRoles = users.Select(user => new UserWithRoles
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = userManager.GetRoles(user.Id).ToList()
            }).ToList();


            return View(usersWithRoles);
        }

        // GET: Users/EditRole/5
        public ActionResult EditRole(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = context.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            var roles = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
                new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var userRoles = userManager.GetRoles(user.Id);

            foreach (var role in roles)
            {
                if (userRoles.Contains(role.Value))
                {
                    role.Selected = true;
                }
            }

            ViewBag.Roles = roles;
            ViewBag.userId = userId;

            return View(user);
        }

        // POST: Users/EditRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(string userId, string roleName)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var user = userManager.FindById(userId);
                if (user == null)
                {
                    return HttpNotFound();
                }

                var currentRoles = userManager.GetRoles(user.Id);
                userManager.RemoveFromRoles(user.Id, currentRoles.ToArray());

                userManager.AddToRole(user.Id, roleName);

                return RedirectToAction("UserList");
            }

            return View();
        }
    }

    
}