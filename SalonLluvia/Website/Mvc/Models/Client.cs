using System.ComponentModel.DataAnnotations;

namespace Mvc.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required] 
        [StringLength(20)]
        // TODO: Add custom validation attribute for regex (ch.11)
        // TODO: Add unique validation attribute
        public string PhoneNumber { get; set; } = string.Empty;
    }
}