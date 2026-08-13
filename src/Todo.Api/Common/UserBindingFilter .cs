namespace Todo.Api.Common;

public abstract record UserBoundRequest
{
    public string UserId { get; init; } = string.Empty;
}

public class UserBindingFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // 1. Loop through the method arguments to find your request record
        for (int i = 0; i < context.Arguments.Count; i++)
        {
            if (context.Arguments[i] is UserBoundRequest userRequest)
            {
                var userId = context.HttpContext.User.GetUserId();

                // 2. Use a native C# 'with' expression directly on the abstract type!
                context.Arguments[i] = userRequest with { UserId = userId };
            }
        }

        return await next(context);
    }
}