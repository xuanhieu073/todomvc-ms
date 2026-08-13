using Carter;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints;

public partial class TodoEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    var apiGroup = app.MapGroup("/api/todos").RequireAuthorization().AddEndpointFilter<UserBindingFilter>();
    AddFilterTodoRoute(apiGroup);
    AddGetTodoRoute(apiGroup);
    AddCreateTodoRoute(apiGroup);
    AddUpateTodoRoute(apiGroup);
    AddToggleCompletedRoute(apiGroup);
    AddDeleteTodoRoute(apiGroup);
    AddClearCompletedRoute(apiGroup);
  }
}