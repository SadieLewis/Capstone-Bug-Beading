using System;
using System.ComponentModel.DataAnnotations;

namespace BugBeading.Models
{
    public class UserInteraction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string InteractionType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ElementType { get; set; } = string.Empty;

        public int? ProductId { get; set; }

        [Range(0, 40000)]
        public int DurationMilliseconds { get; set; }

        [Required]
        [StringLength(200)]
        public string Page { get; set; } = string.Empty;

        [Required]
        public DateTime TimestampUtc { get; set; }
    }
}