using BusinessLogicLayer.DTO;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUsersService
    {
        Task<string> LoginAsync(Login model);
        Task RegisterAsync(Register model);
    }
}
