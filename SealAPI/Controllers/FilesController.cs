using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SealAPI.Context;
using SealAPI.Models;
using SealAPI.Resources;

namespace SealAPI.Controllers.Files
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : BaseApiController<Models.File>
    {
        private readonly FileContext _context;
        private readonly Config _config;
        private readonly ILogger<Models.File> _logger;

        public FilesController(FileContext context, ILogger<Models.File> logger, IOptions<Config> config) : base(context, logger, config)
        {
            _context = context;
            _config = config.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.File>>> GetFiles(bool ignoreData = true)
        {
            _logger.LogInformation(LogMessage.ApiRequest, nameof(this.Get), typeof(Models.File), "ALL");
            if (_context.Files == null)
            {
                _logger.LogError(LogMessage.NotFound, nameof(Models.File));
                return NotFound();
            }

            // Return without data to increase performance. Will be retrieved seperately (when required)
            return ignoreData ? await _context.Files
                .Select(e => new Models.File
                {
                    Id = e.Id,
                    Name = e.Name,
                    Cts = e.Cts,
                    Size = e.Size,
                    Type = e.Type,
                    DownloadCount = e.DownloadCount
                }).ToListAsync() : await Get();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.File?>> GetFile(int id)
        {
            return await GetById(id);
        }

        [HttpGet("Download/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var file = await GetById(id);
            if (file == null || file.Value == null || file.Value.Data == null)
            {
                return NotFound();
            }

            // Return the file as a download response
            return File(file.Value.Data, "application/octet-stream", file.Value.Name);
        }

        [HttpGet("Preview/{id}")]
        public async Task<IActionResult> PreviewFile(int id)
        {
            var file = await GetById(id);
            if (file == null || file.Value == null || file.Value.Data == null)
            {
                return NotFound();
            }

            // For word documents, we will convert to HTML for display
            if (file.Value.Type == FileType.DOC || file.Value.Type == FileType.DOCX)
            {
                file.Value.Data = Converter.ConvertDocToPng(_logger, file.Value.Data);
            }

            // For Excel documents, we will convert to HTML for display
            if (file.Value.Type == FileType.XLS || file.Value.Type == FileType.XLSX)
            {
                file.Value.Data = Converter.ConvertExcelToPng(_logger, file.Value.Data);
            }

            // Return the file as a download response
            return File(file.Value.Data, "application/octet-stream", file.Value.Name);
        }

        [HttpPut]
        public async Task<ActionResult<Models.File>> PutFile(Models.File file)
        {
            // We don't always pass the byte[] data to the front end. We also only need this endpoint
            // to increment the download counter. Retrieve the file from DB instead of using front 
            // end data
            var fileActual = await GetById(file.Id);
            if (fileActual == null || fileActual.Value == null || fileActual.Value.Data == null)
            {
                return NotFound();
            }
            fileActual.Value.DownloadCount = file.DownloadCount;
            return await Put(fileActual.Value.Id, fileActual.Value);
        }

        [HttpPost]
        public async Task<ActionResult<Models.File>> PostFile(Models.File file)
        {
            file.DownloadCount = 0;
            file.Type = Helper.ResolveFileType(file.Name);
            return await Post(file);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Models.File>> DeleteFile(int id)
        {
            return await Delete(id);
        }
    }
}
