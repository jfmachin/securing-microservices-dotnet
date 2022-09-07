using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MoviesContext>(x => x.UseInMemoryDatabase("Movies"));
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.Authority = "https://localhost:5005";
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(x => {
    x.AddPolicy("ClientIdPolicy", p => p.RequireClaim("client_id", "movieClient", "moviesMVCClient"));
});

var app = builder.Build();

var context = app.Services.CreateScope()
    .ServiceProvider
    .GetRequiredService<MoviesContext>();
context.Database.EnsureCreated();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();