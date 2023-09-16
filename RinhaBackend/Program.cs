using Microsoft.EntityFrameworkCore;
using RinhaBackend.Endpoints;
using RinhaBackend.Persistence;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRedisOutputCache();
builder.Services.AddSingleton<IConnectionMultiplexer>(
        s => ConnectionMultiplexer.Connect("cache"));

builder.Services.AddDbContext<PersonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RinhaBackend"),
                builder => builder.MigrationsAssembly(typeof(PersonContext).Assembly.FullName)));




builder.Services.AddScoped<IPersonDbContext>(provider => provider.GetRequiredService<PersonContext>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOutputCache();
app.UsePessoasEndpoints();
app.InitializeDatabase();
app.Run();