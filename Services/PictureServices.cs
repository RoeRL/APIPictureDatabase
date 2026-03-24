using Microsoft.Extensions.Configuration;
using PictureDatabaseAPI.Data;

namespace PictureDatabaseAPI.Services;

public class PictureServices
{
    private readonly AppDbContext _dbContext;
    private readonly string _storagePath;

    public PictureServices(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _storagePath = Path.Combine(home, "picture_data");
        
        if (!Directory.Exists(_storagePath))Directory.CreateDirectory(_storagePath);
    }
}