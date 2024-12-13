using DataAccessLayer.DTO;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ITaskRepository
    {
        Task AddTaskAsync(TaskEntity taskEntity);
        Task<IEnumerable<TaskEntity>> GetFilteredTasksAsync(TaskFilter filter, Guid userId);
        Task<TaskEntity?> GetTaskByIdAsync(Guid taskId, Guid userId);
        Task UpdateTaskInfoAsync(TaskEntity taskEntity, Guid userId);
        Task DeleteTaskAsync(Guid taskId, Guid userId);
    }
}
