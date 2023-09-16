using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RinhaBackend.Endpoints;
using RinhaBackend.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOutputCache();

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

app.UseHttpsRedirection();

app.UsePessoasEndpoints();
app.InitializeDatabase();
app.Run();