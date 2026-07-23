using AutoMapper;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.DTOs
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles() {
            CreateMap<CreateToDoRequest, TodoItem>();
            CreateMap<UpdateTodoRequest, TodoItem>();
            CreateMap<TodoItem, TodoDto>().ForMember(dest => dest.Id, o => o.MapFrom(o => o.ID));
        }
    }
}
