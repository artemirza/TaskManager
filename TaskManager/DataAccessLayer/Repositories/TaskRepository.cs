using DataAccessLayer.Data;
using DataAccessLayer.DTO;
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
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _dbContext;

        public TaskRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddTaskAsync(TaskEntity taskEntity)
        {
            await _dbContext.AddAsync(taskEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetFilteredTasksAsync(TaskFilter filter, Guid userId)
        {

            var query = _dbContext.Tasks.AsQueryable();

            query = query.Where(t => t.UserId == userId);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status);

            if (!string.IsNullOrEmpty(filter.DueDate))
            {
                if (DateTime.TryParse(filter.DueDate, out DateTime parsedDueDate))
                {
                    parsedDueDate = DateTime.SpecifyKind(parsedDueDate, DateTimeKind.Utc);
                    query = query.Where(t => t.DueDate >= parsedDueDate && t.DueDate < parsedDueDate.AddDays(1));
                }
                else
                {
                    throw new ArgumentException("Invalid DueDate format.");
                }
            }

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority); 

            return await query.ToListAsync();
        }

        public async Task<TaskEntity?> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            return await _dbContext.Tasks
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task UpdateTaskInfoAsync(TaskEntity taskEntity, Guid userId)
        {
            var currentUtcTime = DateTime.UtcNow.Date.Add(DateTime.UtcNow.TimeOfDay).AddHours(2);

            await _dbContext.Tasks
                .Where(t => t.UserId == userId && t.Id == taskEntity.Id)
                .ExecuteUpdateAsync(t => t
                .SetProperty(i => i.Title, taskEntity.Title)
                .SetProperty(i => i.Description, taskEntity.Description)
                .SetProperty(i => i.DueDate, taskEntity.DueDate)
                .SetProperty(i => i.Status, taskEntity.Status)
                .SetProperty(i => i.Priority, taskEntity.Priority)
                .SetProperty(i => i.UpdatedAt, currentUtcTime));
        }

        public async Task DeleteTaskAsync(Guid taskId, Guid userId)
        {
            await _dbContext.Tasks
                .Where(t => t.UserId == userId && t.Id == taskId)
                .ExecuteDeleteAsync();
        }
    }
}
