using MediatR;
using Todo.Bff.Clients;
using Todo.Bff.Features.Todos.DTOs;
using static System.Net.WebRequestMethods;

namespace Todo.Bff.Features.Todos.Application.Queries
{
    public class FilterTodoHandler : IRequestHandler<FilterTodoQuery, IResult>
    {
        private readonly TodoApiClient _apiClient;

        public FilterTodoHandler(TodoApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IResult> Handle(FilterTodoQuery request, CancellationToken cancellationToken)
        {
            var response = await _apiClient.GetTodosAsync(request.filter);
            return response.ToHttpResponse();
        }
    }
}
