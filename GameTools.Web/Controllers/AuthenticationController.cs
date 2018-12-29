using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTools.Authentication.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameTools.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Authentication")]
    public class AuthenticationController : Controller
    {
        private IAuthenticationService _auth;
        public AuthenticationController(IAuthenticationService service)
        {
            _auth = service;
        }

        [HttpPost]
        public OkObjectResult ValidateGoogleAuthenticationToken(string idtoken)
        {
            
            return Ok(idtoken);
        }
    }
}