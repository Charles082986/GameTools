using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlackFolderGames.Application;
using BlackFolderGames.Web.Areas.WorldBuilder.Models;
using BlackFolderGames.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldBuilder.Data;

namespace BlackFolderGames.Web.Areas.WorldBuilder.Controllers
{
    [Area("WorldBuilder")]
    public class WorldApiController : ControllerBase
    {
        IWorldService _worldService;
        IUserRepository _userRepository;

        public WorldApiController(IWorldService worldService, IUserRepository userRepository)
        {
            _worldService = worldService;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<RegionSet>> GetOwnedRegionSets(string searchTerm = null)
        {
            var userId = User.GetUserId();
            var ownedRegionSets = _worldService.GetOwnedRegionSets(userId, searchTerm);
            if(ownedRegionSets != null && ownedRegionSets.Any())
            {
                ownedRegionSets = ownedRegionSets.OrderByDescending(ors => ors.LastEdit).ToList();
            }
            return Ok(ownedRegionSets);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<RegionSet>> GetSharedRegionSets(string searchTerm = null)
        {
            var userId = User.GetUserId();
            var sharedRegionSets = _worldService.GetEditableRegionSets(userId, searchTerm);
            if(sharedRegionSets != null && sharedRegionSets.Any())
            {
                sharedRegionSets = sharedRegionSets.OrderByDescending(srs => srs.LastEdit).ToList();
            }
            return Ok(sharedRegionSets);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<RegionSet>> GetBrowseableRegionSets(string searchTerm = null)
        {
            var userId = User.GetUserId();
            var browseableRegionSets = _worldService.GetViewableRegionSets(userId, searchTerm);
            if (browseableRegionSets != null && browseableRegionSets.Any())
            {
                browseableRegionSets = browseableRegionSets.OrderByDescending(srs => srs.LastEdit).ToList();
            }
            return Ok(browseableRegionSets);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Region>> GetContainedRegions(string containerId)
        {
            var userId = User.GetUserId();
            return Ok(null);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<PointOfInterest>> GetContainedPointsOfInterest(string containerId)
        {
            var userId = User.GetUserId();
            return Ok(null);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Region>> GetOverlappingRegions(string regionId)
        {
            var userId = User.GetUserId();
            return Ok(null);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Region>> GetContainingRegions(string containedId)
        {
            var userId = User.GetUserId();
            return Ok(null);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Region>> GetOrphanedRegions(string regionSetId)
        {
            var userId = User.GetUserId();
            return Ok(null);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<PointOfInterest>> GetOrphanedPointsOfInterest(string regionSetId)
        {
            var userId = User.GetUserId();
            return Ok(null);
        }



        [HttpGet]
        public ActionResult<WorldIndexViewModel> RegionSetSearch(string searchTerm = null)
        {
            var userId = User.GetUserId();
            WorldIndexViewModel model = new WorldIndexViewModel();
            IEnumerable<UserModel> users = _userRepository.GetUsers();
            UserModel defaultUser = new UserModel(new Microsoft.AspNetCore.Identity.IdentityUser("UNKNOWN"), new List<Claim>());
            model.OwnedRegionSets = _worldService.GetOwnedRegionSets(userId, searchTerm);
            var editableRegionSets = _worldService.GetEditableRegionSets(userId, searchTerm);
            if (editableRegionSets != null)
            {
                model.EditableRegionSets = editableRegionSets.GroupBy(rs =>
                    (users.FirstOrDefault(u => u.IdentityUser.Id == rs.OwnerId) ?? defaultUser)
                        .IdentityUser.UserName)
                .Select(g => new RegionSetCollectionModel(g)).ToList();
            }
            var viewableRegionSets = _worldService.GetViewableRegionSets(userId, searchTerm);
            if (viewableRegionSets != null)
            {
                model.ViewableRegionSets = viewableRegionSets.Where(vrs => vrs.OwnerId != userId).GroupBy(rs =>
                        (users.FirstOrDefault(u => u.IdentityUser.Id == rs.OwnerId) ?? defaultUser)
                            .IdentityUser.UserName)
                    .Select(g => new RegionSetCollectionModel(g)).ToList();
            }
            return model;
        }
    }
}