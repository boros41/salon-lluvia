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
    public int? StyleId { get; set; }

    [ValidateNever]
    public Style Style { get; set; } = null!;

}