using System.Net;
using AdventureWorks.Server.Data;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var endpoint = builder.Configuration.GetConnectionString("AppConfig")
    ?? throw new InvalidOperationException("The connection string 'AppConfig' was not found.");
   
TokenCredential credential = new DefaultAzureCredential();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddOpenIdConnect(options =>
{
    var oidcConfig = builder.Configuration.GetSection("OpenIDConnectSettings");

    options.Authority = oidcConfig["Authority"] ?? throw new InvalidOperationException("Authority not found in configuration.");
    options.ClientId = oidcConfig["ClientId"] ?? throw new InvalidOperationException("ClientId not found in configuration.");
    options.ClientSecret = oidcConfig["ClientSecret"] ?? throw new InvalidOperationException("ClientSecret not found in configuration.");
    options.Scope.Add(oidcConfig["Scope"] ?? throw new InvalidOperationException("Scope not found in configuration."));
    options.ResponseType = oidcConfig["ResponseType"] ?? OpenIdConnectResponseType.Code;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = false;

}).AddCookie();

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
app.UseRouting();
app.UseAuthentication();
// Authorization is applied for middleware after the UseAuthorization method
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();