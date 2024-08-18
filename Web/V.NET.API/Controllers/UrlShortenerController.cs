﻿using Microsoft.AspNetCore.Mvc;
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
                RequesterIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
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

            return Redirect(urlMapping.OriginalUrl);
        }

        private static string GenerateShortCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
