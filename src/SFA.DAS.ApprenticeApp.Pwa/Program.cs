﻿using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.ApprenticeApp.Pwa.AppStart;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using SFA.DAS.ApprenticePortal.SharedUi.GoogleAnalytics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Text;
using WebEssentials.AspNetCore.Pwa;
using System.Diagnostics.CodeAnalysis;


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

builder.Services.AddDataProtection();
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

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

// Add Security Headers Middleware with environment
//app.UseMiddleware<SecurityHeadersMiddleware>(app.Environment);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

[ExcludeFromCodeCoverage]
public static partial class Program { }