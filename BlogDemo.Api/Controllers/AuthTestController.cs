using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogDemo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AuthTestController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var claims = User.Claims;
            return claims.Select(x=>x.Value);
        }
    }
}