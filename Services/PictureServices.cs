using System.IO.Enumeration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PictureDatabaseAPI.Data;
using PictureDatabaseAPI.Entities;

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

    public async Task<PictureRecord> AddPictureAsync(Stream fileStream, string originalName)
    {
        var id = Guid.NewGuid();
        var extension = Path.GetExtension(originalName);
        var fileName = $"{id}{extension}";
        var fullPath = Path.Combine(_storagePath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        var record = new PictureRecord
        {
            Id = id,
            FileName = fileName,
            OriginalFileName = originalName,
            FilePath = fullPath,
            Created = DateTime.UtcNow,
        };
        _dbContext.Pictures.Add(record);
        await _dbContext.SaveChangesAsync();
        return record;
    }

    public async Task<List<PictureRecord>> GetAllPicturesAsync()
    {
        return await _dbContext.Pictures.ToListAsync();
    }

    public async Task<bool> DeletePictureAsync(Guid id)
    {
      var record = await _dbContext.Pictures.FindAsync(id);
      if (record == null) return false;
      if (File.Exists(record.FilePath)) File.Delete(record.FilePath);
      _dbContext.Pictures.Remove(record);
      await _dbContext.SaveChangesAsync();
      return true;
    }
}