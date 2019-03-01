using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlackFolderGames.Application;
using BlackFolderGames.Web.Areas.WorldBuilder.Models;
using BlackFolderGames.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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