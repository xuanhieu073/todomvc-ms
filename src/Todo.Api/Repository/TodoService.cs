using AutoMapper;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Todo.Api.DTOs;
using Todo.Api.Entities;
using Todo.Api.Repository.Interfaces;

namespace Todo.Api.Repository
{
    public class TodoService : ITodoService
    {
        private readonly IMapper _mapper;
        public TodoService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<TodoDto>> Filter(string status)
        {
            var query = DB.Queryable<TodoItem>();
            query = status switch
            {
                "active" => query.Where(t => !t.IsCompleted),
                "completed" => query.Where(t => t.IsCompleted),
                _ => query
            };
            var todos = await query.ToListAsync();
            var result = _mapper.Map<List<TodoDto>>(todos);
            return result;
        }

        public async Task<TodoDto?> GetTodoById(string Id) {
            var todo = await this.FindOneById(Id);
            return _mapper.Map<TodoDto>(todo);
        }

        public async Task<TodoDto> CreateTodo(CreateToDoRequest createTodo)
        {
            var newTodo = _mapper.Map<CreateToDoRequest, TodoItem>(createTodo);
            newTodo.CreatedAt = DateTime.Now;
            await newTodo.SaveAsync();
            return _mapper.Map<TodoDto>(newTodo);
        }

        public async Task<TodoDto?> UpdateTodo(string Id, UpdateTodoRequest updateTodo)
        {
            var todo = await this.FindOneById(Id);
            if(todo == null)
            {
                return null;
            }
            else
            {
                _mapper.Map(updateTodo, todo);
                await todo.SaveAsync();
                return _mapper.Map<TodoDto>(todo);
            }
        }

        public async Task<TodoDto?> ToggleIsCompleted(string Id)
        {
            var todo = await this.FindOneById(Id);
            if(todo == null)
            {
                return null;
            }
            else
            {
                todo.IsCompleted = !todo.IsCompleted;
                await todo.SaveAsync();
                return _mapper.Map<TodoDto>(todo);
            }
        }

        public async Task<bool?> DeleteTodo(string Id) 
        {
            var todo = await this.FindOneById(Id);
            if(todo == null)
            {
                return null;
            }
            else
            {
                var result = await DB.DeleteAsync<TodoItem>(Id);
                return result.IsAcknowledged;
            }
        }

        public async Task<long?> ClearCompleted()
        {
            var deletedResult = await DB.DeleteAsync<TodoItem>(todo => todo.IsCompleted == true);
            return deletedResult.IsAcknowledged switch
            {
                true => deletedResult.DeletedCount,
                _ => 0,
            };
        }

        private async Task<TodoItem?> FindOneById(string Id)
        {
            return await DB.Find<TodoItem>().OneAsync(Id);
        }
    }
}
