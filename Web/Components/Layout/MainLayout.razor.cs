using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using ShellMgmt.Domain.MenuItemModels;
using ShellMgmt.Domain.UserModels;
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
    [Inject] protected ApiService ApiService { get; set; } = default!;

    private bool sidebarExpanded = true;
    private bool isUserAdmin = false;
    private List<MenuItemDto>? menuItems;
    private string? userDisplayName;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Logger.LogInformation("OnInitializedAsync Starting");
            if (Context.HttpContext?.User.Identity?.IsAuthenticated == true)
            {
                Logger.LogInformation("User is authenticated");

                // Get access token
                try
                {
                    AppConfig.AccessToken = await Context.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken) ?? string.Empty;
                    Logger.LogInformation("Access token retrieved, length: {Length}", AppConfig.AccessToken.Length);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to get access token");
                }

                // Resolve user display name from claims first (instant)
                userDisplayName = Context.HttpContext?.User.Identity?.Name
                    ?? Context.HttpContext?.User.FindFirst("name")?.Value
                    ?? Context.HttpContext?.User.FindFirst("preferred_username")?.Value
                    ?? Context.HttpContext?.User.FindFirst("given_name")?.Value
                    ?? Context.HttpContext?.User.FindFirst("email")?.Value
                    ?? "User";

                // Fetch user profile from API to get real name and username
                try
                {
                    var sub = Context.HttpContext?.User.FindFirst("sub")?.Value;
                    if (!string.IsNullOrEmpty(sub) && !string.IsNullOrEmpty(AppConfig.AccessToken))
                    {
                        var user = await ApiService.GetAsync<UserDto>($"user/Get/{sub}", AppConfig.AccessToken);
                        if (user != null)
                        {
                            var firstName = user.FirstName ?? string.Empty;
                            var lastName = user.LastName ?? string.Empty;
                            var name = $"{firstName} {lastName}".Trim();
                            if (!string.IsNullOrEmpty(name))
                                userDisplayName = name;
                            else if (!string.IsNullOrEmpty(user.UserName))
                                userDisplayName = user.UserName;

                            // Store the SCIM username for menu assignment lookup
                            AppConfig.CurrentUserName = user.UserName ?? string.Empty;
                            Logger.LogInformation("User profile loaded: {Name}, UserName: {UserName}", userDisplayName, user.UserName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to fetch user profile for header");
                }

                // Load tenant (non-critical)
                try
                {
                    if (!string.IsNullOrEmpty(AppConfig.AccessToken) && !IsTokenExpired(AppConfig.AccessToken))
                    {
                        AppConfig.TenantName = GetTenantName(AppConfig.AccessToken);
                        var tenant = await TenantService.GetTenantAsync(AppConfig.TenantName, AppConfig.AccessToken);
                        if (tenant != null)
                        {
                            AppConfig.TenantId = tenant.Id?.ToString() ?? string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to load tenant, continuing...");
                }

                // Load menus from API
                try
                {
                    Logger.LogInformation("Loading menus from API...");
                    var menuItemList = await MenuService.GetMenuItemsAsync(AppConfig.AccessToken);
                    var allMenus = menuItemList ?? new List<MenuItemDto>();
                    Logger.LogInformation("Loaded {Count} menus", allMenus.Count);

                    var sub = Context.HttpContext?.User.FindFirst("sub")?.Value ?? string.Empty;
                    var adminUser = Configuration["WSO2:AdminUser"] ?? "admin";

                    Logger.LogInformation("Menu filter: sub={Sub}, adminUser={AdminUser}, match={Match}",
                        sub, adminUser, string.Equals(sub, adminUser, StringComparison.OrdinalIgnoreCase));

                    if (string.Equals(sub, adminUser, StringComparison.OrdinalIgnoreCase))
                    {
                        // Admin gets all menus
                        isUserAdmin = true;
                        menuItems = allMenus;
                        Logger.LogInformation("Admin user - showing all {Count} menus", allMenus.Count);
                    }
                    else
                    {
                        // Non-admin: filter by UserMenuAssignment
                        isUserAdmin = false;

                        // Try matching by sub (UUID) first, then by SCIM username
                        var userMenuIds = await GetUserAssignedMenuIds(sub);
                        if ((userMenuIds == null || userMenuIds.Count == 0)
                            && !string.IsNullOrEmpty(AppConfig.CurrentUserName)
                            && !string.Equals(AppConfig.CurrentUserName, sub, StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.LogInformation("No menus for sub={Sub}, trying SCIM username={Username}", sub, AppConfig.CurrentUserName);
                            userMenuIds = await GetUserAssignedMenuIds(AppConfig.CurrentUserName);
                        }

                        if (userMenuIds != null && userMenuIds.Count > 0)
                        {
                            var allowedIds = new HashSet<Guid>(userMenuIds);

                            foreach (var menuId in userMenuIds)
                            {
                                var menu = allMenus.FirstOrDefault(m => m.Id == menuId);
                                if (menu != null)
                                {
                                    // Add parent menu
                                    if (menu.ParentId != null && menu.ParentId != Guid.Empty)
                                        allowedIds.Add(menu.ParentId.Value);

                                    // Add child menus
                                    var children = allMenus.Where(m => m.ParentId == menu.Id).ToList();
                                    foreach (var child in children)
                                    {
                                        allowedIds.Add(child.Id ?? Guid.Empty);
                                    }
                                }
                            }

                            menuItems = allMenus.Where(m => allowedIds.Contains(m.Id ?? Guid.Empty)).ToList();
                            Logger.LogInformation("Non-admin user - showing {Count} assigned menus", menuItems.Count);
                        }
                        else
                        {
                            menuItems = new List<MenuItemDto>();
                            Logger.LogWarning("Non-admin user sub={Sub} - no menus assigned in UserMenuAssignment", sub);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to load menus");
                    menuItems = new List<MenuItemDto>();
                }

                StateHasChanged();
            }
            else
            {
                Logger.LogWarning("User is NOT authenticated, redirecting to login");
                NavigationManager.NavigateTo("/Account/Login", true);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Main Layout Error");
            menuItems = new List<MenuItemDto>();
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

    private void OnMenuItemClick(MenuItemDto item)
    {
        if (!string.IsNullOrEmpty(item.Url))
        {
            NavigationManager.NavigateTo(item.Url);
        }
    }

    private static string GetMenuIcon(string? name)
    {
        return name?.ToLower() switch
        {
            "dashboard" => "dashboard",
            "administration" => "admin_panel_settings",
            "settings" => "settings",
            "users" => "people",
            "claims" => "security",
            "institutions" => "account_balance",
            "organization" => "corporate_fare",
            "apps" => "apps",
            "menu items" => "menu",
            "user groups" => "group_add",
            "resources" => "key",
            "tenants" => "business",
            _ => "folder"
        };
    }

    private async Task<List<Guid>?> GetUserAssignedMenuIds(string wso2UserName)
    {
        try
        {
            // Get all mappings and filter by username
            var allMappings = await ApiService.GetAsync<List<UserMenuAssignmentDto>>("usermenu/GetAllMappings", AppConfig.AccessToken);
            if (allMappings == null) return null;

            var userMappings = allMappings.Where(m =>
                string.Equals(m.Wso2UserId, wso2UserName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(m.Wso2UserName, wso2UserName, StringComparison.OrdinalIgnoreCase)).ToList();

            return userMappings.Select(m => m.MenuItemId).ToList();
        }
        catch
        {
            return null;
        }
    }

    private async Task Logout()
    {
        NavigationManager.NavigateTo($"{NavigationManager.BaseUri}Account/Logout", true);
    }
}
