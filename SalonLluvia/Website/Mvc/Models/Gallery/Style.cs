using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.Gallery;

public class Style
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter a gender.")]
    [StringLength(6, ErrorMessage = "Gender must be 6 characters or less.")]
    public string Gender { get; set; } = string.Empty;

    public List<StyleType> Type { get; set; } = []; // collection navigational property

    [Required(ErrorMessage = "Please enter a hair color.")]
    [StringLength(20, ErrorMessage = "Color must be 20 characters or less.")]
    public string Color { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Description must be 50 characters or less.")]
    public string? Description { get; set; }
}