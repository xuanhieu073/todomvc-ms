using AutoMapper;
using Todo.Api.Features.Reminders;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.MapingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateTodoRequest, TodoItem>();
            CreateMap<UpdateTodoRequest, TodoItem>();
            CreateMap<TodoItem, TodoDto>().ForMember(dest => dest.Id, o => o.MapFrom(o => o.ID));
            CreateMap<Reminder, ReminderDto>().ForMember(dest => dest.Id, o => o.MapFrom(o => o.ID));
        }
    }
}
