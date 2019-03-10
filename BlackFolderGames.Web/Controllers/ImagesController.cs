using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using BlackFolderGames.Application;
using BlackFolderGames.Application.Utilities;
using BlackFolderGames.Application.Interfaces;

namespace BlackFolderGames.Web.Controllers
{
    public class ImagesController : ControllerBase
    {
        private IImageService _images;
        private IUserRepository _users;
        public ImagesController(IImageServiceFactory factory, IUserRepository users)
        {
            _images = factory.Create();
            _users = users;
        }

        [HttpGet]
        public ActionResult GetByFriendlyName(string name)
        {
            byte[] image = _images.GetByFriendlyName(name);
            string contentType = string.Empty;
            if(image != null && image.Length > 0 && ImageHelper.TryGetImageContentType(image,out contentType))
            {
                return base.File(image, contentType);
            }
            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
        }

        [HttpGet]
        public ActionResult GetById(string id)
        {
            byte[] image = _images.GetById(id);
            string contentType = string.Empty;
            if (image != null && image.Length > 0 && ImageHelper.TryGetImageContentType(image, out contentType))
            {
                return base.File(image, contentType);
            }
            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
        }

        [HttpPut]
        public ActionResult UploadImage([FromBody]byte[] image, [FromBody]string friendlyName)
        {
            string userId = User.GetUserId();
            string contentType = string.Empty;
            if(image != null && image.Length > 0 && ImageHelper.TryGetImageContentType(image, out contentType))
            {
                if (_images.IsFriendlyNameSanitary(friendlyName))
                {
                    if (_images.IsFriendlyNameAvailable(friendlyName) || _images.IsOwner(friendlyName, userId))
                    {
                        if(_images.UploadImage(image, friendlyName, userId))
                        {
                            return Ok();
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    }
                    else
                    {
                        return BadRequest(new ArgumentException("Friendly name is already taken."));
                    }
                }
                else
                {
                    return BadRequest(new ArgumentException("The friendlyName value does not meet the requirements."));
                }
            }
            else
            {
                return BadRequest(new ArgumentNullException("image"));
            }
        }

        [HttpGet]
        public ActionResult CheckImageFriendlyNameAvailability(string friendlyName)
        {
            return base.Ok(_images.IsFriendlyNameSanitary(friendlyName) && _images.IsFriendlyNameAvailable(friendlyName));
        }
    }
}