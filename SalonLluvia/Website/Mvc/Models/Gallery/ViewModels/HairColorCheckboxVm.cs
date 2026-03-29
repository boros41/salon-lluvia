using Mvc.Models.Gallery.ViewModels.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.Gallery.ViewModels;

// Used by Gallery.cshtml to iterate and generate @Html.CheckBoxesFor for each hair color
public class HairColorCheckboxVm : ICheckbox
{
    public bool IsChecked { get; set; }

    [Required(ErrorMessage = "Please enter a hair color (e.g., black).")]
    [StringLength(20, ErrorMessage = "Hair color must be 20 characters or less.")]
    public string Color { get; set; } = string.Empty;
}