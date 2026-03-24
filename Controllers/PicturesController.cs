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

        using var stream = file.OpenReadStream();
        var record = await _pictureServices.AddPictureAsync(stream, file.FileName);

        return Ok(record);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPictures()
    {
        var pictures = await _pictureServices.GetAllPicturesAsync();
        return Ok(pictures);
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