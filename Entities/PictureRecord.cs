using System.ComponentModel.DataAnnotations;

namespace PictureDatabaseAPI.Entities;

public class PictureRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string FileName { get; set; } = string.Empty;
    [Required]
    public string OriginalFileName { get; set; } = string.Empty;
    [Required]
    public string FilePath { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
}