using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopOnline.Api.BackgroundServices;
using ShopOnline.Api.Data;
using ShopOnline.Api.Repositories;
using ShopOnline.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

// Add controllers
builder.Services.AddControllers();

// Add Swagger + Bearer Token support
builder.Services.AddEndpointsApiExplorer();

var identityUrl = builder.Configuration["BaseURLSettings:ShopOnline_IdentityServerProvider_Url"];

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ShopOnlineApi", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{identityUrl}/connect/authorize"),
                TokenUrl = new Uri($"{identityUrl}/connect/token"),
                //Scopes = new Dictionary<string, string> { { "shop_online_api", "Access API" } }
                Scopes = new Dictionary<string, string>()
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "shop_online_api" }
        }
    });


});

// Add EF Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add DI for services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

// Add Authentication using Duende IdentityServer (OAuth2/OIDC)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration.GetSection("BaseURLSettings")["ShopOnline_IdentityServerProvider_Url"]; // URL IdentityServer        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "shop_online_api"
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Token invalid: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "shop_online_api");
    });

    options.AddPolicy("RequireAdmin", policy =>
    {
        policy.RequireRole("Admin");
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllClients", policy =>
    {
        policy.WithOrigins(
            builder.Configuration["BaseURLSettings:ShopOnline_MvcClient_Url"],
            builder.Configuration["BaseURLSettings:ShopOnline_AngularClient_Url"],
            builder.Configuration["BaseURLSettings:ShopOnline_ReactClient_Url"]
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Register the RabbitMQ background consumer
builder.Services.AddHostedService<CategoryConsumerService>();
builder.Services.AddHostedService<CategorySearchService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next.Invoke();
});

// Middlewares
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopOnlineApi v1");
    options.OAuthClientId("shop_online_swagger_client");
    options.OAuthUsePkce();
    options.OAuthScopes("openid", "profile", "shop_online_api");
});

app.UseHttpsRedirection();

app.UseCors("AllowAllClients");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
