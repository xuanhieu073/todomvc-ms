using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Application.Queries
{
    public class GetTodoHandler : IRequestHandler<GetTodoQuery, IResult>
    {
        private readonly TodoApiClient _apiClient;

        public GetTodoHandler(TodoApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<IResult> Handle(GetTodoQuery request, CancellationToken cancellationToken)
        {
            var response = await _apiClient.GetTodoAsync(request.Id);
            return response.ToHttpResponse();
        }
    }
}
