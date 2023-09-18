using Microsoft.EntityFrameworkCore;
using RinhaBackend.BackgroundServices;
using RinhaBackend.Cache;
using RinhaBackend.Endpoints;
using RinhaBackend.Filters;
using RinhaBackend.Persistence;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<RequestErrorCaptureMiddleware>();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    s => ConnectionMultiplexer.Connect("cache"));
builder.Services.AddScoped<IRedisCacheSevice, RedisCacheService>();
builder.Services.AddHostedService<PersonDatabaseWorker>();
builder.Services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())); 

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

app.UseMiddleware<RequestErrorCaptureMiddleware>();

app.UsePessoasEndpoints();

app.InitializeDatabase();
app.Run();