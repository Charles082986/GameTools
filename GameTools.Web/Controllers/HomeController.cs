using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameTools.Web.Models;
using Microsoft.AspNetCore.Authorization;
using GameTools.Authentication.Interfaces.Services;
using GameTools.Authentication.Interfaces.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace GameTools.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private Authentication.Interfaces.Services.IAuthenticationService _auth;
        public HomeController(Authentication.Interfaces.Services.IAuthenticationService auth)
        {
            _auth = auth;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LogIn(string authProvider, string token)
        {
            ThirdPartyAuthenticationProvider provider = ThirdPartyAuthenticationProvider.None;
            Enum.TryParse(authProvider, out provider);
            ClaimsPrincipal principal = _auth.Authenticate(provider, token) as ClaimsPrincipal;
            if (principal != null && principal.Identities.Any())
            {
                HttpContext.SignInAsync("Cookie", principal
                            , new AuthenticationProperties()
                            {
                                ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                                IsPersistent = true,
                                AllowRefresh = true
                            });
                if (principal.Identities.FirstOrDefault(i => i.AuthenticationType == "GameTools") != null)
                {
                    if (principal.HasClaim(c => string.Equals(c.Type, "Active", StringComparison.CurrentCultureIgnoreCase)
                         && string.Equals(c.Value, "Active", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        return Json(new { SignInStatus = "SUCCESS" });
                    }
                    else
                    {
                        return Json(new { SignInStatus = "ACCOUNT_LOCKED" });
                    }
                }
                else
                {
                    return Json(new { SignInStatus = "CREATE_ACCOUNT" });
                }
            }
            else
            {
                return Json(new { SignInStatus = "FAIL" });
            }
        }

        public IActionResult CreateAccount(string authProvider, string token)
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
