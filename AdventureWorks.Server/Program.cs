using System.Net;
using AdventureWorks.Server.Data;
using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var endpoint = builder.Configuration.GetConnectionString("AppConfig")
    ?? throw new InvalidOperationException("The connection string 'AppConfig' was not found.");
   
TokenCredential credential = new DefaultAzureCredential();

try
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(endpoint), credential);

        options.ConfigureKeyVault(keyVaultOptions =>
        {
            keyVaultOptions.SetCredential(credential);
        });
    });
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}

var connectionString = builder.Environment.IsProduction() ? builder.Configuration.GetSection("SQLDb").Value : builder.Configuration.GetConnectionString("LocalDb");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();