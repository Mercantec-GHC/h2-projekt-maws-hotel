using Blazor.Components;
using Blazor.Services;
using DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Get the connection string from appsettings or environment variable
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DefaultConnection");

        // Configure ApplicationDbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.AddScoped<HttpClient>();
        builder.Services.AddMudServices();

        // Register DatabaseService with both connectionString and HttpClient
        builder.Services.AddScoped<DatabaseService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            return new DatabaseService(connectionString, httpClient);
        });

        // AuthenticationService Registration
        builder.Services.AddAuthentication("Cookies")
            .AddCookie("Cookies", options =>
            {
                // Cookie options configuration
            });

        // Register AppState
        builder.Services.AddScoped<AppState>();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddScoped<EmailService>();

        builder.Services.AddHttpClient("API", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7207/");
        });

        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
