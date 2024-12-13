using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class UsersService : IUsersService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<UsersService> _logger;
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository, ITokenGenerator tokenGenerator, ILogger<UsersService> logger)
        {
            _usersRepository = usersRepository;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        public async Task RegisterAsync(Register model)
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = model.UserName,
                Email = model.Email,
                Password = HashPassword(model.Password)
            };

            await _usersRepository.AddAsync(user);
        }

        public async Task<string> LoginAsync(Login model)
        {
            try
            {
                var user = await _usersRepository.GetUserByNameAsync(model.UserName);

                if (user is not null && VerifyPassword(model.Password, user.Password))
                {
                    var token = _tokenGenerator.Generate(user);

                    return token;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }

    }
}
