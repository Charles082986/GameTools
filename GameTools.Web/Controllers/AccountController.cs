using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTools.Authentication.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameTools.Web.Controllers
{
    public class AccountController : Controller
    {
        private IAuthenticationService _authentication;
        const string UserEmail = "_UserEmail";
        public AccountController(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}