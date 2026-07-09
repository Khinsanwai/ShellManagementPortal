using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Radzen;
using ShellMgmt.Web.Components;
using ShellMgmt.Web.Constants;
using ShellMgmt.Web.Services;
using Serilog;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);

builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddHttpContextAccessor();

// Bypass SSL certificate validation for development (WSO2 self-signed cert)
builder.Services.AddHttpClient<Wso2Service>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddCascadingAuthenticationState();

// WSO2 OIDC Authentication
var wso2Settings = builder.Configuration.GetSection("WSO2");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Events.OnSigningOut = async e =>
    {
        e.HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
        await Task.CompletedTask;
    };
})
.AddOpenIdConnect(options =>
{
    options.Authority = wso2Settings["Authority"];
    options.ClientId = wso2Settings["ClientId"];
    options.ClientSecret = wso2Settings["ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;

    options.MapInboundClaims = false;
    options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
    options.TokenValidationParameters.RoleClaimType = "role";

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("openid");
    options.RequireHttpsMetadata = false;

    // Bypass SSL certificate validation for WSO2 self-signed cert
    options.BackchannelHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    };

    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            context.ProtocolMessage.RedirectUri = wso2Settings["RedirectUri"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build())
    .AddPolicy("AnonymousPolicy", policy => policy.RequireAssertion(_ => true));

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

var app = builder.Build();

// Load configuration
var configuration = app.Services.GetRequiredService<IConfiguration>();
AppConfig.Url = configuration["SMPConfig:Url"] ?? string.Empty;
AppConfig.IamUrl = configuration["SMPConfig:IamUrl"] ?? string.Empty;
AppConfig.BaseRouteUrl = configuration["SMPConfig:BaseRouteUrl"] ?? string.Empty;
AppConfig.ClientId = configuration["WSO2:ClientId"] ?? string.Empty;

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAntiforgery();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseSerilogRequestLogging();

app.Run();
