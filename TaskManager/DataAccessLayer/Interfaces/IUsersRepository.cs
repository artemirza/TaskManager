using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUsersRepository
    {
        Task AddAsync(UserEntity user);
        Task<UserEntity?> GetUserByNameAsync(string userName);
        Task<UserEntity?> GetUserByIdAsync(Guid userId);

    }
}
