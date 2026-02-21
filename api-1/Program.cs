using api_1;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors();
var connectionString = builder.Configuration.GetConnectionString("PetsDatabase");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/pets", async context =>
{
    var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
    var pets = await dbContext.Pets.ToListAsync();
    await context.Response.WriteAsJsonAsync(pets);
})
.WithName("GetPets");
app.UseCors(options => options.AllowAnyOrigin());
app.Run();