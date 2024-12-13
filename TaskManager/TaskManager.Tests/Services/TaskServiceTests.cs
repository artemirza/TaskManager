using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Services;
using DataAccessLayer.DTO;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Priority = DataAccessLayer.Models.Priority;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly ITaskRepository _taskRepositoryFake;
        private readonly IUsersRepository _usersRepositoryFake;
        private readonly IHttpContextAccessor _httpContextAccessorFake;
        private readonly IMapper _mapperFake;
        private readonly ILogger<TaskService> _loggerFake;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryFake = A.Fake<ITaskRepository>();
            _usersRepositoryFake = A.Fake<IUsersRepository>();
            _httpContextAccessorFake = A.Fake<IHttpContextAccessor>();
            _mapperFake = A.Fake<IMapper>();
            _loggerFake = A.Fake<ILogger<TaskService>>();

            _taskService = new TaskService(
                _taskRepositoryFake,
                _usersRepositoryFake,
                _httpContextAccessorFake,
                _mapperFake,
                _loggerFake
            );
        }

        [Fact]
        public async Task AddTaskAsync_ShouldAddTask_WhenModelIsValid()
        {
            var userId = Guid.NewGuid();
            var createTask = new CreateTask(
                "Test Task",
                "Test Description",
                "2024-12-12 18:00:00",
                Status.Pending,
                Priority.Low
            );

            var expectedDueDate = DateTime.SpecifyKind(DateTime.Parse(createTask.DueDate), DateTimeKind.Utc);

            var user = new UserEntity { Id = userId, Tasks = new List<TaskEntity>() };
            A.CallTo(() => _usersRepositoryFake.GetUserByIdAsync(userId)).Returns(user);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            A.CallTo(() => _httpContextAccessorFake.HttpContext.User).Returns(claimsPrincipal);

            TaskEntity? capturedTask = null;
            A.CallTo(() => _taskRepositoryFake.AddTaskAsync(A<TaskEntity>.That.Matches(task =>
            task.Title == createTask.Title &&
            task.Description == createTask.Description &&
            task.Status == createTask.Status &&
            task.Priority == createTask.Priority &&
            task.DueDate == expectedDueDate &&
            task.UserId == userId &&
            task.User == user
            ))).Invokes(call => capturedTask = call.GetArgument<TaskEntity>(0));

            await _taskService.AddTaskAsync(createTask);

            A.CallTo(() => _taskRepositoryFake.AddTaskAsync(A<TaskEntity>._)).MustHaveHappenedOnceExactly();

            capturedTask.Should().NotBeNull();
            capturedTask!.Title.Should().Be(createTask.Title);
            capturedTask.Description.Should().Be(createTask.Description);
            capturedTask.Status.Should().Be(createTask.Status);
            capturedTask.Priority.Should().Be(createTask.Priority);
            capturedTask.DueDate.Should().Be(expectedDueDate);
            capturedTask.UserId.Should().Be(userId);
            capturedTask.User.Should().Be(user);
        }

        [Fact]
        public async Task GetFilteredTasksAsync_ShouldReturnMappedTasks_WhenFilterIsProvided()
        {
            var userId = Guid.NewGuid();
            var taskFilter = new TaskFilter(Status.Pending, null, Priority.Low);
            var taskEntities = new List<TaskEntity> { new TaskEntity { Id = Guid.NewGuid(), Title = "Task 1", Description = "Desc 1", DueDate = DateTime.UtcNow, Status = Status.Pending, Priority = Priority.Low, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow } };
            var taskDisplays = new List<TaskDisplay> { new TaskDisplay(Guid.NewGuid(), "Task 1", "Desc 1", DateTime.UtcNow.ToString("s"), Status.Pending, Priority.Low, DateTime.UtcNow.ToString("s"), DateTime.UtcNow.ToString("s")) };

            A.CallTo(() => _taskRepositoryFake.GetFilteredTasksAsync(taskFilter, userId)).Returns(taskEntities);
            A.CallTo(() => _mapperFake.Map<IEnumerable<TaskDisplay>>(taskEntities)).Returns(taskDisplays);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            A.CallTo(() => _httpContextAccessorFake.HttpContext.User).Returns(claimsPrincipal);

            var result = await _taskService.GetFilteredTasksAsync(taskFilter);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().ContainSingle(task => task.Title == "Task 1" && task.Description == "Desc 1");
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldCallDeleteTask_WhenTaskIdIsValid()
        {
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            A.CallTo(() => _httpContextAccessorFake.HttpContext.User).Returns(claimsPrincipal);

            await _taskService.DeleteTaskAsync(taskId);

            A.CallTo(() => _taskRepositoryFake.DeleteTaskAsync(taskId, userId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddTaskAsync_ShouldThrowArgumentException_WhenDueDateIsInvalid()
        {
            var createTask = new CreateTask(
                "Test Task",
                "Test Description",
                "Invalid Date Format", // Некорректная дата
                Status.Pending,
                Priority.Low
            );

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "InvalidGuid") // Некорректный GUID
            }));

            A.CallTo(() => _httpContextAccessorFake.HttpContext.User).Returns(claimsPrincipal);

            Func<Task> act = async () => await _taskService.AddTaskAsync(createTask);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid GUID format in JWT token");
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnMappedTask_WhenTaskExists()
        {
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var taskEntity = new TaskEntity
            {
                Id = taskId,
                Title = "Task 1",
                Description = "Description 1",
                DueDate = DateTime.UtcNow,
                Status = Status.Pending,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
                User = new UserEntity { Id = userId }
            };

            var taskDisplay = new TaskDisplay(
                taskId,
                "Task 1",
                "Description 1",
                taskEntity.DueDate.ToString("s"),
                Status.Pending,
                Priority.Low,
                taskEntity.CreatedAt.ToString("s"),
                taskEntity.UpdatedAt.ToString("s")
            );

            A.CallTo(() => _taskRepositoryFake.GetTaskByIdAsync(taskId, userId)).Returns(taskEntity);
            A.CallTo(() => _mapperFake.Map<TaskDisplay>(taskEntity)).Returns(taskDisplay);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            A.CallTo(() => _httpContextAccessorFake.HttpContext.User).Returns(claimsPrincipal);

            var result = await _taskService.GetTaskByIdAsync(taskId);

            result.Should().NotBeNull();
            result.Id.Should().Be(taskId);
            result.Title.Should().Be("Task 1");
            result.Description.Should().Be("Description 1");
            result.DueDate.Should().Be(taskDisplay.DueDate);
        }
    }
}
