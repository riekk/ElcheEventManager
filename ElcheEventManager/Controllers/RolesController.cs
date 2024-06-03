using ElcheEventManager.Models;
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
            // Obtener todos los usuarios
            var users = context.AspNetUsers.ToList();

            // Obtener los roles de cada usuario
            var usersWithRoles = users.Select(user => new
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

            // Buscar al usuario por su Id
            ApplicationUser user = context.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Obtener los roles disponibles
            var roles = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
                new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            // Obtener el rol actual del usuario
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var userRoles = userManager.GetRoles(user.Id);

            // Marcar el rol actual del usuario como seleccionado
            foreach (var role in roles)
            {
                if (userRoles.Contains(role.Value))
                {
                    role.Selected = true;
                }
            }

            ViewBag.Roles = roles;

            return View(user);
        }

        // POST: Users/EditRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(string userId, string roleName)
        {
            if (ModelState.IsValid)
            {
                // Encontrar al usuario por su Id
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var user = userManager.FindById(userId);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Eliminar todos los roles existentes del usuario
                var currentRoles = userManager.GetRoles(user.Id);
                userManager.RemoveFromRoles(user.Id, currentRoles.ToArray());

                // Agregar el nuevo rol al usuario
                userManager.AddToRole(user.Id, roleName);

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}