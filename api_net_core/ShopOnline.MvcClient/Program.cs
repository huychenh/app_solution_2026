using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = builder.Configuration.GetSection("BaseURLSettings")["ShopOnline_IdentityServerProvider_Url"]; // IdentityServer URL
    options.ClientId = "shop_online_mvc_client";
    options.ClientSecret = "this_is_a_long_secret";
    options.ResponseType = "code";

    options.SaveTokens = true;

    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("shop_online_api");
    options.Scope.Add("roles");

    //options.ClaimActions.MapJsonKey("name", "name");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
    options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Role, "role");


    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddOpenIdConnectAccessTokenManagement(); // <--- Required for AddUserAccessTokenHandler

builder.Services.AddHttpClient("ShopOnlineApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseURLSettings:ShopOnline_Api_Url"]);
})
.AddUserAccessTokenHandler(); // <--- Will now work


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
