using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

public class UsersServiceTests
{
    private readonly IUsersRepository _usersRepositoryFake;
    private readonly ITokenGenerator _tokenGeneratorFake;
    private readonly ILogger<UsersService> _loggerFake;
    private readonly UsersService _usersService;

    public UsersServiceTests()
    {
        _usersRepositoryFake = A.Fake<IUsersRepository>();
        _tokenGeneratorFake = A.Fake<ITokenGenerator>();
        _loggerFake = A.Fake<ILogger<UsersService>>();

        _usersService = new UsersService(_usersRepositoryFake, _tokenGeneratorFake, _loggerFake);
    }

    [Fact]
    public async Task RegisterAsync_ShouldAddUser_WhenModelIsValid()
    {
        var model = new Register("TestUser", "test@example.com", "TestPassword");

        UserEntity capturedUser = null;
        A.CallTo(() => _usersRepositoryFake.AddAsync(A<UserEntity>.That.Matches(u => true)))
            .Invokes(call => capturedUser = call.GetArgument<UserEntity>(0));

        await _usersService.RegisterAsync(model);

        capturedUser.Should().NotBeNull();
        capturedUser.UserName.Should().Be(model.UserName);
        capturedUser.Email.Should().Be(model.Email);
        capturedUser.Password.Should().NotBeNullOrEmpty();
        A.CallTo(() => _usersRepositoryFake.AddAsync(A<UserEntity>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var model = new Login("TestUser", "TestPassword");
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = model.UserName,
            Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(model.Password)))
        };

        var expectedToken = "GeneratedToken";

        A.CallTo(() => _usersRepositoryFake.GetUserByNameAsync(model.UserName)).Returns(user);
        A.CallTo(() => _tokenGeneratorFake.Generate(user)).Returns(expectedToken);

        var result = await _usersService.LoginAsync(model);

        result.Should().NotBeNull();
        result.Should().Be(expectedToken);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        var model = new Login ("InvalidUser", "WrongPassword");

        A.CallTo(() => _usersRepositoryFake.GetUserByNameAsync(model.UserName))
            .Returns(Task.FromResult<UserEntity>(null)); 

        var result = await _usersService.LoginAsync(model);

        // Assert
        result.Should().BeNull(); 
    }
}
