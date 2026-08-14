using Microsoft.EntityFrameworkCore;
using ShellMgmt.Application;
using Serilog;
using ShellMgmt.Persistence.ApplicationDbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using ShellMgmt.Api.Filters;
using ShellMgmt.Api.Services;

var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager config = builder.Configuration;

builder.Host.UseSerilog((_, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));

// Database - SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("WriteDatabaseConnection")));

builder.Services.AddDbContext<ReadDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("ReadDatabaseConnection")));

builder.Services.AddScoped<ScimService>();
builder.Services.AddScoped<Wso2ApiResourceService>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogFilter>();
});

// WSO2 JWT Bearer Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(cfg =>
{
    var wso2OidcAuthority = config["WSO2:OidcAuthority"]?.TrimEnd('/')
        ?? config["WSO2:Authority"]?.TrimEnd('/');

    if (string.IsNullOrEmpty(wso2OidcAuthority))
    {
        wso2OidcAuthority = "https://localhost:9443/oauth2/oidcdiscovery";
    }

    if (wso2OidcAuthority.EndsWith("/token", StringComparison.OrdinalIgnoreCase))
    {
        wso2OidcAuthority = wso2OidcAuthority[..^"/token".Length].TrimEnd('/') + "/oidcdiscovery";
    }
    else if (wso2OidcAuthority.EndsWith("/oauth2", StringComparison.OrdinalIgnoreCase))
    {
        wso2OidcAuthority += "/oidcdiscovery";
    }

    cfg.RequireHttpsMetadata = false;
    cfg.Authority = wso2OidcAuthority;
    cfg.MetadataAddress = wso2OidcAuthority.TrimEnd('/') + "/.well-known/openid-configuration";
    cfg.IncludeErrorDetails = true;

    cfg.BackchannelHttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidIssuer = config["WSO2:Issuer"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    cfg.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = c =>
        {
            c.NoResult();
            c.Response.StatusCode = 401;
            c.Response.ContentType = "text/plain";
            return c.Response.WriteAsync(c.Exception.ToString());
        }
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShellManagementPortal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddService();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSerilogRequestLogging();

app.Run();
