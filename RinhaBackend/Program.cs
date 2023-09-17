using Microsoft.EntityFrameworkCore;
using RinhaBackend.Endpoints;
using RinhaBackend.Filters;
using RinhaBackend.Persistence;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<RequestErrorCaptureMiddleware>();

if (builder.Environment.IsProduction())
{
    builder.Services.AddRedisOutputCache();
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        s => ConnectionMultiplexer.Connect("cache"));
}
else
{
    builder.Services.AddOutputCache();
}

builder.Services.AddDbContext<PersonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RinhaBackend"),
            builder =>
            {
                builder.MigrationsAssembly(typeof(PersonContext).Assembly.FullName);
                builder.EnableRetryOnFailure();
            }));




var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseOutputCache();

app.UseMiddleware<RequestErrorCaptureMiddleware>();

app.UsePessoasEndpoints();

app.InitializeDatabase();
app.Run();