using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskEntity = DataAccessLayer.Models.TaskEntity;

namespace DataAccessLayer.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .IsRequired();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200); 

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.HasIndex(t => t.DueDate);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.HasIndex(t => t.Status);

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasConversion<string>();

            builder.HasIndex(t => t.Priority);
        }
    }
}
