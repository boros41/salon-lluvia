using System.ComponentModel.DataAnnotations;

namespace Mvc.Models;

public class Client
{
    public int Id { get; set; }

    [Display(Name = "Client")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    [Required]
    [StringLength(20)]
    // TODO: Add custom validation attribute for regex (ch.11)
    // TODO: Add unique validation attribute
    public string PhoneNumber { get; set; } = string.Empty;
}