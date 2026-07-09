using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace ShellMgmt.Web.Controllers;

[Route("Account")]
public class AccountController : Controller
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

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        return LocalRedirect(redirectUri);
    }
}
