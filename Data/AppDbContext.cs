using Microsoft.EntityFrameworkCore;
using PictureDatabaseAPI.Entities;

namespace PictureDatabaseAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    
    public DbSet<PictureRecord> Pictures { get; set; }
}