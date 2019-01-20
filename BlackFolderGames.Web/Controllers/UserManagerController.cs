using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackFolderGames.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlackFolderGames.Web.Controllers
{
    public class UserManagerController : Controller
    {
        private IUserRepository _users;
        public UserManagerController(IUserRepository repo)
        {
            _users = repo;
        }

        [Authorize("CanViewUsers")]
        public IActionResult Index()
        {
            return View(_users.GetUsers());
        }

        [Authorize("CanSuspendUsers")]
        public IActionResult SuspendUser(string UserId, string Status)
        {
            var user = _users.Manager.Users.FirstOrDefault(u => u.Id == UserId);
            if(Status.IndexOf("SUSPENDED UNTIL") > -1)
            {
                _users.UnsuspendUser(user);
            }
            else
            {
                _users.SuspendUser(user);
            }
            return RedirectToAction("Index");
        }

        [Authorize("CanBanUsers")]
        public IActionResult BanUser(string UserId, string Status)
        {
            var user = _users.Manager.Users.FirstOrDefault(u => u.Id == UserId);
            if (Status == "BANNED")
            {
                _users.UnbanUser(user);
            }
            else
            {
                _users.BanUser(user);
            }
            return RedirectToAction("Index");
        }

        [Authorize("CanEditClaims")]
        public IActionResult EditClaims(string UserId)
        {
            var user = _users.Manager.Users.FirstOrDefault(u => u.Id == UserId);
            if (user != null)
            {
                var claims = _users.Manager.GetClaimsAsync(user).Result.ToList();
                var model = new EditClaimsModel(new UserModel(user, claims));
                return View(model);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}