using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.ViewModels;

public class AppointmentViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    // TODO: Add custom validation attribute for regex (ch.11)
    // TODO: Add unique validation attribute
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(200)]
    public string DesiredService { get; set; } = string.Empty;
}
