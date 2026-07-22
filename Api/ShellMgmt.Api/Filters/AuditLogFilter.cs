using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShellMgmt.Application.UserlogService.Create;

namespace ShellMgmt.Api.Filters;

public class AuditLogFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Skip logging for the UserlogController itself to avoid infinite loops
        if (context.Controller.GetType().Name == "UserlogController")
        {
            await next();
            return;
        }

        var httpContext = context.HttpContext;
        var user = httpContext.User;
        var userId = user.FindFirst("sub")?.Value ?? string.Empty;
        var username = user.Identity?.Name
            ?? user.FindFirst("preferred_username")?.Value
            ?? user.FindFirst("email")?.Value
            ?? userId;

        var controllerName = context.Controller.GetType().Name.Replace("Controller", "");
        var actionName = context.ActionDescriptor.RouteValues["action"] ?? "Unknown";
        var httpMethod = httpContext.Request.Method;
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        // Execute the action
        var resultContext = await next();

        // Determine result
        var result = "Success";
        var description = $"{httpMethod} {controllerName}/{actionName}";

        if (resultContext.Exception != null && !resultContext.ExceptionHandled)
        {
            result = "Failed";
            description += $" - Error: {resultContext.Exception.Message}";
        }
        else if (resultContext.Result is ObjectResult objectResult)
        {
            if (objectResult.StatusCode >= 400)
            {
                result = "Failed";
                description += $" - Status: {objectResult.StatusCode}";
            }
        }

        // Log to database
        try
        {
            var mediator = httpContext.RequestServices.GetRequiredService<IMediator>();
            await mediator.Send(new CreateUserlogCommand(
                UserId: userId,
                Username: username,
                Category: "Action",
                Action: $"{httpMethod} {actionName}",
                Application: "ShellManagementPortal",
                Module: controllerName,
                Description: description,
                Result: result,
                IPAddress: ipAddress,
                UserAgent: userAgent
            ));
        }
        catch
        {
            // Don't let logging failures break the request
        }
    }
}
