using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTask model)
        {
            await _taskService.AddTaskAsync(model);
            return Ok("Task has been created successfuly");
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredTasks([FromBody] TaskFilter filter)
        {
            var tasks = await _taskService.GetFilteredTasksAsync(filter);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            return Ok(task);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTaskInfo([FromBody] UpdateTask updateTask)
        {
            await _taskService.UpdateTaskInfoAsync(updateTask);
            return Ok("Task has been updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTruck(Guid id)
        {
            await _taskService.DeleteTaskAsync(id);
            return Ok("Task has been deleted");
        }
    }

}
