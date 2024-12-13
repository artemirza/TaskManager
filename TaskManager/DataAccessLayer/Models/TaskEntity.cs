using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Models
{
    public class TaskEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; } 
        public string Description { get; set; } 

        public DateTime DueDate { get; set; }
        [EnumDataType(typeof(Status))]
        public Status Status { get; set; } = Status.Pending;
        [EnumDataType(typeof(Priority))]
        public Priority Priority { get; set; } = Priority.Low; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.Date.Add(DateTime.UtcNow.TimeOfDay).AddHours(2);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.Date.Add(DateTime.UtcNow.TimeOfDay).AddHours(2);

        public Guid UserId { get; set; }

        public UserEntity User { get; set; }
    }

}