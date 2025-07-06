using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class VendorProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string StoreName { get; set; } = string.Empty;
    }
} 