using Microsoft.AspNetCore.Mvc;
using V.NET.API.Models;
using V.NET.API.Database;
using Microsoft.EntityFrameworkCore;

namespace V.NET.API.Controllers
{
    [Route("s")]
    [ApiController]
    public class UrlShortenerController(UrlShortenerDbContext context) : ControllerBase
    {
        private readonly UrlShortenerDbContext _context = context;
        #region Helpers

        // Helper method to get the client's IP address
        private string GetRequesterIp()
        {
            // Check for the Cloudflare header CF-Connecting-IP first
            var cloudflareIp = HttpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault();

            if (!string.IsNullOrEmpty(cloudflareIp))
            {
                return cloudflareIp;
            }

            // If Cloudflare header isn't present, check X-Forwarded-For header
            var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For may contain multiple IPs, the first one is the original client IP
                var ipAddress = forwardedFor.Split(',').First().Trim();
                return ipAddress;
            }

            // Fallback to the remote IP address if no headers are present
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
        {
            // Generate a short code
            var shortCode = GenerateShortCode();

            // Save the URL mapping with the requester's IP
            var urlMapping = new UrlMapping
            {
                ShortCode = shortCode,
                OriginalUrl = originalUrl,
                RequesterIp = GetRequesterIp()
            };

            _context.UrlMappings.Add(urlMapping);
            await _context.SaveChangesAsync();

            return Ok(new { shortCode });
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var urlMapping = await _context.UrlMappings
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (urlMapping == null)
            {
                return NotFound();
            }

            var requestLog = new RequestLog
            {
                UrlMappingId = urlMapping.Id,
                RequesterIp = GetRequesterIp(),
                UserAgent = Request.Headers.UserAgent.FirstOrDefault()
            };

            _context.RequestLogs.Add(requestLog);
            await _context.SaveChangesAsync();

            return Redirect(urlMapping.OriginalUrl);
        }

        [HttpGet("{shortCode}/logs")]
        public async Task<IActionResult> ViewLogs(string shortCode)
        {
            var urlMapping = await _context.UrlMappings
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (urlMapping == null)
            {
                return NotFound();
            }

            var logs = await _context.RequestLogs
                .Where(rl => rl.UrlMappingId == urlMapping.Id)
                .ToListAsync();

            return Ok(logs);
        }

        private static string GenerateShortCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
