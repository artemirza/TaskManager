using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }  
        public string UserName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.Date.Add(DateTime.UtcNow.TimeOfDay);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.Date.Add(DateTime.UtcNow.TimeOfDay);
        public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    }
}
