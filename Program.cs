using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PictureDatabaseAPI.Services;
using PictureDatabaseAPI.Data;

string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string pictureDataPath = Path.Combine(homePath, "picture_data");

var builder = WebApplication.CreateBuilder(args);


//Safety protocol
if (!Directory.Exists(pictureDataPath)) Directory.CreateDirectory(pictureDataPath);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PictureServices>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}



app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
Console.WriteLine("Start App");

app.MapGet("/pictureStorageApi/docs", () => 
{
    var htmlPath = Path.Combine(AppContext.BaseDirectory, "docs.html");
    if (!File.Exists(htmlPath)) return Results.NotFound("Documentation file not found.");
    
    var htmlContent = File.ReadAllText(htmlPath);
    return Results.Content(htmlContent, "text/html");
});

app.Run();