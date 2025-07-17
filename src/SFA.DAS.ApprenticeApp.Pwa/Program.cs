using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.ApprenticeApp.Pwa.AppStart;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticePortal.SharedUi.GoogleAnalytics;
using System.Diagnostics.CodeAnalysis;
using WebEssentials.AspNetCore.Pwa;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var rootConfiguration = builder.Configuration.LoadConfiguration(builder.Services);
var applicationConfiguration = rootConfiguration.Get<ApplicationConfiguration>();
builder.Services.AddSingleton(applicationConfiguration!);

// Add services to the container.
builder.Services.AddControllersWithViews();

var environment = builder.Environment;
builder.Services.AddServiceRegistration(environment, rootConfiguration, applicationConfiguration);

// Add outerapi
builder.Services.AddOuterApi(applicationConfiguration.ApprenticeAppApimApi);

builder.Services.AddDataProtection(applicationConfiguration);
builder.Services.AddHealthChecks();
builder.Services.AddProgressiveWebApp(new PwaOptions { RegisterServiceWorker = true });

builder.Services.AddSession(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddLogging(builder =>
{
    builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
    builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
});

// Add the OpenTelemetry telemetry service to the application.
builder.Services.AddOpenTelemetryRegistration(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

builder.Services.AddAntiforgery(
    options =>
    {
        options.Cookie.Name = "qa-x-csrf";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.HeaderName = "X-XSRF-TOKEN";
    }
);

// configure google analytics
builder.Services.Configure<MvcOptions>(options =>
    options.Filters.Add(new EnableGoogleAnalyticsAttribute(
        applicationConfiguration.GoogleAnalytics)
    ));

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
app.UseHealthChecks("/ping");

app.UseStatusCodePagesWithReExecute("/ErrorPage/{0}");

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=CookieStart}/{id?}");
    
    endpoints.MapGet("/home/keepalive", async context =>
    {
        context.Response.StatusCode = context.User.Identity?.IsAuthenticated == true 
            ? StatusCodes.Status204NoContent 
            : StatusCodes.Status401Unauthorized;
    });
    
});

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (context.Response.Headers.ContainsKey("X-Powered-By"))
        {
            context.Response.Headers.Remove("X-Powered-By");
        }

        if (context.Response.Headers.ContainsKey("Server"))
        {
            context.Response.Headers.Remove("Server");
        }

        return Task.CompletedTask;
    });

    if (context.Response.Headers.ContainsKey("X-Frame-Options"))
    {
        context.Response.Headers.Remove("X-Frame-Options");
    }
    
    context.Response.Headers!.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers!.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers!.Append("X-Xss-Protection", "1");
    context.Response.Headers!.Append("X-Permitted-Cross-Domain-Policies", "none");
    context.Response.Headers!.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    
    await next();
});



app.Run();

[ExcludeFromCodeCoverage]
public static partial class Program { }