using JWTAuthAspNet7WebAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthAspNet7WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
            return Ok(Summaries);
        }

        [HttpGet]
        [Route("GetAdminsRole")]
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        public IActionResult GetAdminsRole()
        {
            return Ok(Summaries);
        }

        [HttpGet]
        [Route("GetCreatorsRole")]
        [Authorize(Roles = StaticUserRoles.CREATOR)]
        public IActionResult GetCreatorsRole()
        {
            return Ok(Summaries);
        }

        [HttpGet]
        [Route("GetCustomersRole")]
        [Authorize(Roles = StaticUserRoles.CUSTOMER)]
        public IActionResult GetCustomersRole()
        {
            return Ok(Summaries);
        }
    }
}
