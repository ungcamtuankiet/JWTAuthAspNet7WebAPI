﻿using JWTAuthAspNet7WebAPI.Core.Dtos;
using JWTAuthAspNet7WebAPI.Core.Entities;
using JWTAuthAspNet7WebAPI.Core.Interfaces;
using JWTAuthAspNet7WebAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthAspNet7WebAPI.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null)
                return new AuthServiceResponseDto()
                {
                    IsSuccess = false,
                    Message = "Invalid Credentials"
                };

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
                return new AuthServiceResponseDto()
                {
                    IsSuccess = false,
                    Message = "Invalid Credentials"
                };

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateNewJsonWebToken(authClaims);

            return new AuthServiceResponseDto()
            {
                IsSuccess = true,
                Message = token
            };
        }

        public async Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
                return new AuthServiceResponseDto()
                {
                    IsSuccess = false,
                    Message = "Invalid User Name !!!!!!!!!"
                };

            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return new AuthServiceResponseDto()
            {
                IsSuccess = true,
                Message = "User is now an ADMIN"
            };
        }

        public async Task<AuthServiceResponseDto> MakeCreatorAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
                return new AuthServiceResponseDto()
                {
                    IsSuccess = false,
                    Message = "Invalid User Name !!!!!!!!!"
                };

            await _userManager.AddToRoleAsync(user, StaticUserRoles.CREATOR);

            return new AuthServiceResponseDto()
            {
                IsSuccess = true,
                Message = "User is now an Creator"
            };
        }

        public async Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto)    
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);

            if (isExistsUser != null)
            {
                return new AuthServiceResponseDto() 
                {
                    IsSuccess = false,
                    Message = "UserName Already Exist"
                };
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User Creation Failed Because: ";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return new AuthServiceResponseDto()
                {
                    IsSuccess = false,
                    Message = errorString
                };
            }

            //Add default CUSTOMER Role to all user
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.CUSTOMER);

            return new AuthServiceResponseDto()
            {
                IsSuccess = true,
                Message = "UserName Created Successfully"
            };
        }

        public async Task<AuthServiceResponseDto> SeedRoleAsync()
        {
            bool isCustomerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.CUSTOMER);
            bool isCreatorRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.CREATOR);
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);

            if (isCustomerRoleExists && isCreatorRoleExists && isAdminRoleExists)
                return new AuthServiceResponseDto()
                {
                    IsSuccess = true,
                    Message = "Roles Seeding is Already Done"
                };

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.CUSTOMER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.CREATOR));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));

            return new AuthServiceResponseDto()
            {
                IsSuccess = true,
                Message = "Role Seeding Done Successfully"
            };
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }
    }
}
