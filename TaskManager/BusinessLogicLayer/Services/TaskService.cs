using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.DTO;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IUsersRepository _usersRepository;

        public TaskService(ITaskRepository taskRepository, IUsersRepository usersRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<TaskService> logger)
        {
            _usersRepository = usersRepository;
            _taskRepository = taskRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task AddTaskAsync(CreateTask model)
        {
            var userId = GetUserIdFromJwtToken();

            var user = await _usersRepository.GetUserByIdAsync(userId);

            if (!DateTime.TryParse(model.DueDate, out DateTime parsedDueDate))
            {
                throw new ArgumentException("Uncorrect format of the date");
            }

            parsedDueDate = DateTime.SpecifyKind(parsedDueDate, DateTimeKind.Utc);

            var taskEntity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Description = model.Description,
                DueDate = parsedDueDate,
                Status = model.Status,
                Priority = model.Priority,
                UserId = userId,
                User = user,
            };

            user.Tasks.Add(taskEntity);

            await _taskRepository.AddTaskAsync(taskEntity);
        }

        public async Task<IEnumerable<TaskDisplay>> GetFilteredTasksAsync(TaskFilter taskFilter)
        {
            var userId = GetUserIdFromJwtToken();

            var result = await _taskRepository.GetFilteredTasksAsync(taskFilter, userId);

            return _mapper.Map<IEnumerable<TaskDisplay>>(result);
        }

        public async Task<TaskDisplay?> GetTaskByIdAsync(Guid taskId)
        {
            var userId = GetUserIdFromJwtToken();

            var task = await _taskRepository.GetTaskByIdAsync(taskId, userId);   
            
            return _mapper.Map<TaskDisplay>(task);
        }

        public async Task UpdateTaskInfoAsync(UpdateTask model)
        {
            if (!DateTime.TryParse(model.DueDate, out DateTime parsedDueDate))
            {
                throw new ArgumentException("Uncorrect format of the date");
            }

            parsedDueDate = DateTime.SpecifyKind(parsedDueDate, DateTimeKind.Utc);

            var taskEntity = _mapper.Map<TaskEntity>(model);

            taskEntity.DueDate = parsedDueDate;

            var userId = GetUserIdFromJwtToken();

            await _taskRepository.UpdateTaskInfoAsync(taskEntity, userId);
        }

        public async Task DeleteTaskAsync(Guid taskId)
        {
            var userId = GetUserIdFromJwtToken();

            await _taskRepository.DeleteTaskAsync(taskId, userId);
        }

        private Guid GetUserIdFromJwtToken()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new ArgumentException("User ID not found in JWT token");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new ArgumentException("Invalid GUID format in JWT token");
            }

            return userId;
        }
    }
}           
    