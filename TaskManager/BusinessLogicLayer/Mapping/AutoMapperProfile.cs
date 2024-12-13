using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UpdateTask, TaskEntity>();
            CreateMap<TaskEntity, TaskDisplay>()
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"))); 
        }     
    }
}
