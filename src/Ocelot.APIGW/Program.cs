using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"ocelot.json", true, true);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

var authenticationProviderKey = "IdentityApiKey";

builder.Services.AddAuthentication()
    .AddJwtBearer(authenticationProviderKey, options => {
        options.Authority = "https://localhost:5005";
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = false
        };
    });

builder.Services.AddOcelot();

var app = builder.Build();
app.UseOcelot().Wait();

app.Run();