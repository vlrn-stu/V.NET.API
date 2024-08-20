using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace V.NET.API.Models
{
    public class RequestLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UrlMappingId { get; set; }

        [ForeignKey("UrlMappingId")]
        public UrlMapping UrlMapping { get; set; } = default!;

        [Required]
        public string RequesterIp { get; set; } = default!;

        public string? UserAgent { get; set; } 

        [Required]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
