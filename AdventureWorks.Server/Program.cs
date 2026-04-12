using System.Net;
using AdventureWorks.Server.Data;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


var endpoint = builder.Configuration.GetConnectionString("AppConfig")
    ?? throw new InvalidOperationException("The connection string 'AppConfig' was not found.");

var credential = new DefaultAzureCredential();


    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(endpoint), credential);

        options.ConfigureKeyVault(keyVaultOptions =>
        {
            keyVaultOptions.SetCredential(credential);
        });
    });


var connectionString = builder.Environment.IsProduction() ? builder.Configuration.GetSection("ConnectionStringAAC").Value :
    builder.Configuration.GetSection("ConnectionStringACCDev").Value;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 0)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure()
            );
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();