using AutoMapper;
using Todo.Api.Entities;

namespace Todo.Api.DTOs
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
