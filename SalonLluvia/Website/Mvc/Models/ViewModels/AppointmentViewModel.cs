using Mvc.Validation;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Models.ViewModels;

public class AppointmentViewModel
{
    [Required(ErrorMessage = "Please enter a name")]
    [StringLength(100, ErrorMessage = "Name must be 100 characters or less")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a phone number")]
    [StringLength(20, ErrorMessage = "Phone number must be 20 characters or less")]
    [PhoneNumber]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a date")]
    public DateTime? Date { get; set; }

    // TODO: Add email regex
    [Required(ErrorMessage = "Please enter an email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a desired service")]
    [StringLength(200, ErrorMessage = "Service must be 200 characters or less")]
    public string DesiredService { get; set; } = string.Empty;
}
