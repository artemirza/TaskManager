using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256); 
            builder.HasIndex(u => u.UserName)
                .IsUnique(); 

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256); 
            builder.HasIndex(u => u.Email)
                .IsUnique(); 

            builder.Property(u => u.Password)
                .IsRequired();

            builder.HasMany(t => t.Tasks)
                .WithOne(u => u.User)               
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
