using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using ShellMgmt.Domain.MenuItemModels;
using ShellMgmt.Web.Constants;
using ShellMgmt.Web.Services;

namespace ShellMgmt.Web.Components.Layout;

public partial class MainLayout
{
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected DialogService DialogService { get; set; } = default!;
    [Inject] protected TooltipService TooltipService { get; set; } = default!;
    [Inject] protected ContextMenuService ContextMenuService { get; set; } = default!;
    [Inject] protected NotificationService NotificationService { get; set; } = default!;
    [Inject] protected TenantService TenantService { get; set; } = default!;
    [Inject] protected ResourceService ResourceService { get; set; } = default!;
    [Inject] protected ILogger<MainLayout> Logger { get; set; } = default!;

    private bool sidebarExpanded = true;
    private List<MenuItemDto>? menuItems;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Logger.LogInformation("OnInitializedAsync Starting");
            if (Context.HttpContext?.User.Identity?.IsAuthenticated == true)
            {
                Logger.LogInformation("User is authenticated");

                AppConfig.AccessToken = await Context.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken) ?? string.Empty;

                if (IsTokenExpired(AppConfig.AccessToken))
                {
                    Logger.LogWarning("Access token expired, logging out.");
                    await Logout();
                    return;
                }

                AppConfig.TenantName = GetTenantName(AppConfig.AccessToken);

                var tenant = await TenantService.GetTenantAsync(AppConfig.TenantName, AppConfig.AccessToken);
                if (tenant != null)
                {
                    AppConfig.TenantId = tenant.Id?.ToString() ?? string.Empty;
                }

                var menuItemList = await MenuService.GetMenuItemsAsync(AppConfig.AccessToken);
                menuItems = menuItemList ?? [];

                Logger.LogInformation("Menu Count: {Count}", menuItems.Count);
            }
            else
            {
                NavigationManager.NavigateTo("/Account/Login", true);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Main Layout Error");
        }
    }

    void SidebarToggleClick()
    {
        sidebarExpanded = !sidebarExpanded;
    }

    protected async Task ProfileMenuClick(RadzenProfileMenuItem args)
    {
        if (args.Value?.ToString() == "Logout")
        {
            await Logout();
        }
    }

    private static string GetTenantName(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var issClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;

        if (issClaim == null)
            throw new Exception("The 'iss' claim was not found in the token.");

        Uri uri = new(issClaim);
        string[] segments = uri.Segments;
        if (segments.Length < 3)
            throw new ArgumentException("The 'iss' field does not contain a valid tenant name");

        return segments[^1].TrimEnd('/');
    }

    private static bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.ValidTo < DateTime.UtcNow;
    }

    private async Task Logout()
    {
        NavigationManager.NavigateTo($"{NavigationManager.BaseUri}Account/Logout", true);
    }
}
