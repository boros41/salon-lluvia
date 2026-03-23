using Mvc.Models.Gallery.ViewModels.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.Gallery.ViewModels;

// Used by Gallery.cshtml to iterate and generate @Html.CheckBoxesFor for each hair type
public class HairTypeViewModel : ICheckbox
{
    public bool IsChecked { get; set; }

    [Required(ErrorMessage = "Please enter a hair type (e.g., peinado).")]
    [StringLength(20, ErrorMessage = "Hair type must be 20 characters or less.")]
    public string Type { get; set; } = string.Empty;
}