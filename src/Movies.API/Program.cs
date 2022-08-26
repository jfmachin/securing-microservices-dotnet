using Microsoft.EntityFrameworkCore;
using Movies.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MoviesContext>(x => x.UseInMemoryDatabase("Movies"));

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
app.UseAuthorization();
app.MapControllers();
app.Run();