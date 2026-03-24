using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.Gallery;

public class Image
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter a filepath.")]
    [StringLength(260, ErrorMessage = "Filepath must be 260 characters or less.")]
    public string FilePath { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a hair style.")]
    public int? HairProfileId { get; set; } // foreign key linking to HairProfile

    // one-to-many: One hair profile can have many images (multiple photos of the same person's hair), but one image can have only one hair profile.
    [ValidateNever]
    public HairProfile HairProfile { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Description must be 50 characters or less.")]
    public string? Description { get; set; }
}