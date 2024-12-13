using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            await _usersService.RegisterAsync(model);
            return Ok("User has been registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var token = await _usersService.LoginAsync(request);
            if (token == null)
            {
                return Unauthorized("Login failed");
            }
            return Ok(new { Token = token });
        }
    }
}
