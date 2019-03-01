using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlackFolderGames.Application;
using BlackFolderGames.Web.Areas.WorldBuilder.Models;
using BlackFolderGames.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldBuilder.Data;

namespace BlackFolderGames.Web.Areas.WorldBuilder.Controllers
{
    [Authorize]
    [Area("WorldBuilder")]
    public class WorldController : Controller
    {
        private IWorldService _worldService;
        private IUserRepository _userRepository;

        public WorldController(IWorldService worldService, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _worldService = worldService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            RegionSet model = new RegionSet() { OwnerId = User.GetUserId() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("RegionSetId,Name,OwnerId,ImageURL,Description,IsPublic")]RegionSet model)
        {
            if(ModelState.IsValid)
            {
                model.OwnerId = User.GetUserId();
                var regionSet = _worldService.CreateRegionSet(model);
                return RedirectToAction("RegionSet",new { regionSetId =  regionSet.RegionSetId });
            }
            return View(model);
        }

        public IActionResult RegionSet(string regionSetId)
        {
            var userId = User.GetUserId();
            if (_worldService.CanViewRegionSet(userId,regionSetId))
            {
                RegionSet model = _worldService.GetRegionSet(regionSetId);
                ViewBag.CanEdit = _worldService.CanEditRegionSet(userId,regionSetId);
                return View(model);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(string regionSetId)
        {
            if (_worldService.CanEditRegionSet(User.GetUserId(), regionSetId))
            {
                RegionSet model = _worldService.GetRegionSet(regionSetId);
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit([Bind("RegionSetId,Name,OwnerId,ImageURL,Description,IsPublic")]RegionSet model)
        {
            if(ModelState.IsValid)
            {
                model = _worldService.UpdateRegionSet(model);
                return RedirectToAction("RegionSet",model);
            }
            return View(model);
        }


    }
}