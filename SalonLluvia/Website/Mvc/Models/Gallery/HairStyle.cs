using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.Gallery;

public class HairStyle
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter a gender.")]
    [StringLength(6, ErrorMessage = "Gender must be 6 characters or less.")]
    public string Gender { get; set; } = string.Empty;

    // collection navigational properties
    public List<HairType> HairTypes { get; set; } = []; // a hairstyle can have many hair types (e.g., peinado, trenzas...)
    public List<HairColor> HairColors { get; set; } = []; // a hairstyle can have many colors (e.g., black, brown...)

    [StringLength(50, ErrorMessage = "Description must be 50 characters or less.")]
    public string? Description { get; set; }
}