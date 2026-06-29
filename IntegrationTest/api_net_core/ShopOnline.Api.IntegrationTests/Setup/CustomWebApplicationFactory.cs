using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopOnline.Api.Data;
using ShopOnline.Api.Models;
using System.Net;

namespace ShopOnline.Api.IntegrationTests.Setup
{
    /// <summary>
    /// Custom HttpMessageHandler to intercept reCAPTCHA verification requests
    /// and always return a successful response JSON: { "success": true }
    /// </summary>
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Intercept Google reCAPTCHA siteverify endpoint
            if (request.RequestUri != null && request.RequestUri.AbsoluteUri.Contains("recaptcha/api/siteverify"))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{ \"success\": true }", System.Text.Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            }

            // Fallback for any other unexpected HTTP requests during integration test
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // 1. Remove SqlServer DbContext and its Options comprehensively
                var entityFrameworkServices = services
                    .Where(d => d.ServiceType.Namespace != null &&
                                d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore"))
                    .ToList();

                foreach (var service in entityFrameworkServices)
                {
                    services.Remove(service);
                }

                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

                // 2. Register AppDbContext to use a shared In-Memory database
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });

                // 3. Mock HttpClient to bypass Google reCAPTCHA verification logic
                // Remove the real HttpClient registration if any exists
                var httpClientDescriptors = services.Where(d => d.ServiceType == typeof(HttpClient)).ToList();
                foreach (var descriptor in httpClientDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Inject our custom HttpClient configured with the FakeHttpMessageHandler
                services.AddSingleton<HttpClient>(sp => new HttpClient(new FakeHttpMessageHandler()));

                // 4. Inject Fake Authentication Scheme for testing bypass
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestAuthScheme";
                    options.DefaultChallengeScheme = "TestAuthScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuthScheme", options => { });

                // 5. Create a temporary scope to seed data into the shared In-Memory DB
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    db.Database.EnsureCreated();

                    if (!db.Categories.Any(c => c.Name.StartsWith("Test_")))
                    {
                        db.Categories.AddRange(
                            new Category { Name = "Test_Laptop" },
                            new Category { Name = "Test_Smartphone" }
                        );
                        db.SaveChanges();
                    }
                }
            });
        }
    }
}