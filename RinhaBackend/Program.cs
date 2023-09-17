using Microsoft.EntityFrameworkCore;
using RinhaBackend.Aot;
using RinhaBackend.Endpoints;
using RinhaBackend.Persistence;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//builder.Services.ConfigureHttpJsonOptions(option =>
//{
//    option.SerializerOptions.TypeInfoResolver = new ApiJsonSerializerContext();
//});

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