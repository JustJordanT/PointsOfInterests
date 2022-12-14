using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        private class CityInfoUser
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Country { get; set; }
            
            public CityInfoUser(int id, string userName, string email, string firstName, string lastName, string country)
            {
                Id = id;
                UserName = userName;
                Email = email;
                FirstName = firstName;
                LastName = lastName;
                Country = country;
            }
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authentication)
        {
            var user = ValidateUserCredentials(
                authentication.UserName,
                authentication.Password);
            
            if (user == null)
            {
                return BadRequest();
            }

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding
                        .ASCII
                        .GetBytes(_configuration["Authentication:SecretForKey"]));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            
            var claimsForToken = new List<Claim>
            {
                new("sub", user.Id.ToString()),
                new("email", user.Email),
                new("given_name", user.FirstName),
                new("family_name", user.LastName),
                new("country", user.Country)
            };

            var token = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? authenticationUserName, string? authenticationPassword) =>
            new CityInfoUser(
               1,
               "jt",
               "jt@cityInfoRepository.com",
               "jordan",
               "taylor",
               "USA"
               );
    }
}
