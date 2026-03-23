using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.Gallery;

public class HairType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter a hair type (e.g., peinado).")]
    [StringLength(20, ErrorMessage = "Hair type must be 20 characters or less.")]
    public string Type { get; set; } = string.Empty;
}