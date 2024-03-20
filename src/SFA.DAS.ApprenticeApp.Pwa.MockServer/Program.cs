var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<SFA.DAS.ApprenticeApp.Pwa.MockServer.Interfaces.IUserRepository, SFA.DAS.ApprenticeApp.Pwa.MockServer.Services.UserRepository>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
