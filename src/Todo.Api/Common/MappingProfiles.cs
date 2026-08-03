using AutoMapper;
using Todo.Api.Features.Reminders;
using Todo.Api.Features.Todos;
using Todo.Api.Features.Todos.Endpoints;

namespace Todo.Api.Common
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<TodoItem, TodoResponse>().ForMember(dest => dest.Id, o => o.MapFrom(o => o.ID));
            CreateMap<UpdateTodoCommand, TodoItem>();
            CreateMap<Reminder, ReminderDto>().ForMember(dest => dest.Id, o => o.MapFrom(o => o.ID));
            CreateMap<ReminderTodoDto, PendingReminderDto>()
                .ForMember(dest => dest.Id, o => o.MapFrom(src => src.Reminder.ID))
                .ForMember(dest => dest.TodoId, o => o.MapFrom(src => src.Reminder.TodoId))
                .ForMember(dest => dest.State, o => o.MapFrom(src => src.Reminder.State))
                .ForMember(dest => dest.FireAt, o => o.MapFrom(src => src.Reminder.FiredAt))
                .ForMember(dest => dest.Title, o => o.MapFrom(src => src.Todo.Title))
                .ForMember(dest => dest.DueAt, o => o.MapFrom(src => src.Todo.DueAt));
        }
    }
}
