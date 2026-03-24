using System.Security.AccessControl;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
Console.WriteLine("Start App");

app.Run();