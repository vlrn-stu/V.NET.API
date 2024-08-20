using System.ComponentModel.DataAnnotations;

namespace V.NET.API.Models
{
    public class UrlMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string ShortCode { get; set; } = default!;

        [Required]
        public string OriginalUrl { get; set; } = default!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string RequesterIp { get; set; } = default!;

        public ICollection<RequestLog> RequestLogs { get; set; } = new List<RequestLog>();
    }
}
