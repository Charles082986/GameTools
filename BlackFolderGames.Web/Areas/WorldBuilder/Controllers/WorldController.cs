using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldBuilder.Data;

namespace BlackFolderGames.Web.Areas.WorldBuilder.Controllers
{
    [Authorize]
    public class WorldController : Controller
    {
        public WorldController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}