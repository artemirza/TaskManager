﻿using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    public record TaskDisplay(Guid Id, string Title, 
        string Description, string DueDate, Status Status,
        Priority Priority, string CreatedAt, string UpdatedAt);
}
