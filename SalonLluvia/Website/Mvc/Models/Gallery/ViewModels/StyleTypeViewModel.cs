using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.Gallery.ViewModels;

public class StyleTypeViewModel
{
    public bool IsChecked { get; set; }

    [Required(ErrorMessage = "Please enter a style type (e.g., peinado).")]
    [StringLength(20, ErrorMessage = "Style type must be 20 characters or less.")]
    public string Type { get; set; } = string.Empty;
}