using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Web.Services;

namespace ShellMgmt.Web.Controllers;

[Route("Account")]
public class AccountController(AuditLogService auditLogService) : Controller
{
    [HttpGet("Login")]
    public IActionResult Login([FromQuery] string? returnUrl)
    {
        var redirectUri = returnUrl ?? "/";
        if (User.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(redirectUri);
        }
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri });
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout([FromQuery] string? returnUrl)
    {
        var redirectUri = returnUrl ?? "/";
        if (User.Identity?.IsAuthenticated != true)
        {
            return LocalRedirect(redirectUri);
        }

        // Capture logout before signing out
        var userId = User.FindFirst("sub")?.Value;
        var username = User.Identity?.Name
            ?? User.FindFirst("preferred_username")?.Value
            ?? userId;
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        await auditLogService.LogAsync(userId, username, "Logout", "Logout", "Success",
            description: $"User {username} logged out",
            ipAddress: ip, userAgent: userAgent);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        return LocalRedirect(redirectUri);
    }
}
