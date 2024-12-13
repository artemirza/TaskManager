using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Settings;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public TokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string? Generate(UserEntity user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Email, user.Email!),                 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!)),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
