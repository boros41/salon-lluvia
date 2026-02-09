using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Mvc.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a client")]
        public int? ClientId { get; set; }
        
        [ValidateNever]
        public Client Client { get; set; } = null!;
        
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(200)]
        public string DesiredService { get; set; } = string.Empty;
    }
}