using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.ApprenticeApp.Pwa.AppStart;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using WebEssentials.AspNetCore.Pwa;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var rootConfiguration = builder.Configuration.LoadConfiguration(builder.Services);
var applicationConfiguration = rootConfiguration.Get<ApplicationConfiguration>();
builder.Services.AddSingleton(applicationConfiguration!);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddServiceRegistration(applicationConfiguration);

// Add outerapi
builder.Services.AddOuterApi(applicationConfiguration.ApprenticeAppApimApi);

builder.Services.AddDataProtection();

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

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddApplicationInsightsTelemetry(new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
});

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

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

