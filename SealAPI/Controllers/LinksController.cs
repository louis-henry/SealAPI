using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SealAPI.Context;
using SealAPI.Models;
using SealAPI.Resources;
using System.Net;

namespace SealAPI.Controllers.Links
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinksController : BaseApiController<Link>
    {
        private readonly FileContext _context;
        private readonly Config _config;
        private readonly ILogger<Link> _logger;

        public LinksController(FileContext context, ILogger<Link> logger, IOptions<Config> config) : base(context, logger, config)
        {
            _context = context;
            _config = config.Value;
            _logger = logger;
        }

        [HttpGet("Download/{guid}")]
        public async Task<IActionResult> DownloadViaLink(string guid)
        {
            var result = await _context.Links.FirstOrDefaultAsync(e => e.GUID == guid);
            
            // Compare expiry time via the saved value, not the appsettings value as it 
            // may have been updated
            if (result == null || DateTime.UtcNow > result.Cts.AddMinutes(result.ExpiryMins))
            {
                return BadRequest(HttpStatusCode.Gone);
            }

            var file = await _context.Files.FirstOrDefaultAsync(e => e.Id == result.FileId);
            if (file == null || file.Data == null)
            {
                return NotFound();
            }

            // We must manually update the download count as the UI does not have the file here,
            // only a link containing a guid
            try
            {
                file.DownloadCount++;
                _context.Files.Update(file);
                await _context.SaveChangesAsync();
                return File(file.Data, "application/octet-stream", file.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(Helper.LogException(ex));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Link>> PostLink(Link link)
        {
            // Setup uniue GUID
            Guid g = Guid.NewGuid();
            while (LinkGUIDExists(g))
            {
                g = Guid.NewGuid();
            }
            link.GUID = g.ToString();

            // Set expiry time from appsettings.json
            link.ExpiryMins = _config.LinkExpiryMins;
            return await Post(link);
        }

        private bool LinkGUIDExists(Guid guid)
        {
            return _context.Links != null && _context.Links.Any(e => e.GUID == guid.ToString());
        }
    }
}
