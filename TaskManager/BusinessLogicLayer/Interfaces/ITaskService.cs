using BusinessLogicLayer.DTO;
using DataAccessLayer.DTO;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITaskService
    {
        Task AddTaskAsync(CreateTask model);
        Task<IEnumerable<TaskDisplay>> GetFilteredTasksAsync(TaskFilter taskFilter);
        Task<TaskDisplay?> GetTaskByIdAsync(Guid taskId);
        Task UpdateTaskInfoAsync(UpdateTask model);
        Task DeleteTaskAsync(Guid taskId);
    }
}
