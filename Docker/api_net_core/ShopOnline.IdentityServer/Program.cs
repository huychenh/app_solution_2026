using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.IdentityServer.Configuration;
using ShopOnline.IdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<BaseUrlSettings>(
    builder.Configuration.GetSection("BaseURLSettings"));


// Google external login
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

var baseUrls = builder.Configuration
    .GetSection("BaseURLSettings")
    .Get<BaseUrlSettings>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger", policy =>
    {
        if (baseUrls != null && !string.IsNullOrEmpty(baseUrls.ShopOnline_Api_Url))
        {
            policy.WithOrigins(baseUrls.ShopOnline_Api_Url)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// ==================== IDENTITY SERVER CONFIGURATION ====================
builder.Services.AddIdentityServer(options =>
{
    if (baseUrls != null && !string.IsNullOrEmpty(baseUrls.ShopOnline_IdentityServerProvider_Url))
    {
        options.IssuerUri = baseUrls.ShopOnline_IdentityServerProvider_Url;
    }
})
    .AddAspNetIdentity<AppUser>()
    .AddInMemoryClients(Config.Clients(baseUrls!))
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddDeveloperSigningCredential(persistKey: true);
// =======================================================================

builder.Services.AddTransient<IProfileService, CustomProfileService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- AUTOMATIC MIGRATION AND SEED DATA PROCESS ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // STEP 1: Always apply migrations first to create the schema/tables        
        context.Database.Migrate();

        // STEP 2: Now that tables exist, safe to run seed data
        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowSwagger");
app.UseCookiePolicy();
app.UseIdentityServer();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();