using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _dbContext;

        public UsersRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(UserEntity user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserEntity?> GetUserByNameAsync(string userName)
        {
            return await _dbContext.Users
                .Include(u => u.Tasks)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<UserEntity?> GetUserByIdAsync(Guid userId)
        {
            return await _dbContext.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
