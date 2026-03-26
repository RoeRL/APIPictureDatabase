using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PictureDatabaseAPI.Services;

namespace PictureDatabaseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PicturesController : ControllerBase
{
    private readonly PictureServices _pictureServices;

    public PicturesController(PictureServices pictureServices)
    {
        _pictureServices = pictureServices;
    }

    [HttpPost]
    public async Task<IActionResult> UploadPicture(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }
        
        const long maxFileSize = 20*1024*1024;
        if (file.Length > maxFileSize) return BadRequest("File is Too Large! Maximum is 120MB.");
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension)) return BadRequest($"Invalid file type '{extension}'. Only JPG, PNG, GIF, BMP, and WEBP are allowed");
        using var stream = file.OpenReadStream();
        var record = await _pictureServices.AddPictureAsync(stream, file.FileName);

        return Ok(record);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPictures([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;
        var pictures = await _pictureServices.GetAllPicturesAsync(pageNumber, pageSize);
        return Ok(pictures);
    }

    [HttpGet("view/{id}")]
    public async Task<IActionResult> GetPictureById(Guid id)
    {
        var record = await _pictureServices.GetPictureByIdAsync(id);
        if (record == null) return NotFound($"Picture not found in database with ID {id}");
        if (!System.IO.File.Exists(record.FilePath)) return NotFound($"Picture {id} not found.");
        
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(record.FilePath, out var contentType)) contentType = "application/octet-stream";
        return PhysicalFile(record.FilePath, contentType);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePicture([FromRoute] Guid id)
    {
        var success = await _pictureServices.DeletePictureAsync(id);
        
        if (!success)
        {
            return NotFound($"Picture with ID {id} not found.");
        }

        return Ok($"Picture {id} completely removed.");
    }
}