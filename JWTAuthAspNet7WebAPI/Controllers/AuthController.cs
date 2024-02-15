using JWTAuthAspNet7WebAPI.Core.Dtos;
using JWTAuthAspNet7WebAPI.Core.Entities;
using JWTAuthAspNet7WebAPI.Core.Interfaces;
using JWTAuthAspNet7WebAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthAspNet7WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //Route for seeding my roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            var seedRoles = await  _authService.SeedRoleAsync();

            return Ok(seedRoles);
        }

        //Route -> Register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var registerResult = await _authService.RegisterAsync(registerDto);

            if(registerResult.IsSuccess) 
                return Ok(registerResult);

            return BadRequest(registerResult);
        }

        //Route -> Login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);

            if (loginResult.IsSuccess)
                return Ok(loginResult);

            return Unauthorized(loginResult);
        }

        //Route -> make customer -> creator
        [HttpPost]
        [Route("make-creator")]
        public async Task<IActionResult> MakeCreator([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.MakeCreatorAsync(updatePermissionDto);

            if(operationResult.IsSuccess) 
                return Ok(operationResult);

            return BadRequest(operationResult);
        }

        //Route -> make customer -> admin
        [HttpPost]
        [Route("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.MakeAdminAsync(updatePermissionDto);

            if (operationResult.IsSuccess)
                return Ok(operationResult);

            return BadRequest(operationResult);
        }
    }
}
