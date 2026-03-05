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

    // TODO: Add custom range validation to prevent client-side issues
    // TODO: Add custom validation to check if selected date is on an available day in a calendar service
    [Required(ErrorMessage = "Please enter a date")]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "Please enter a desired service")]
    [StringLength(200, ErrorMessage = "Service must be 200 characters or less")]
    public string DesiredService { get; set; } = string.Empty;
}
