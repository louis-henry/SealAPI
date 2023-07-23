using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SealAPI.Context;
using SealAPI.Models;
using SealAPI.Resources;

namespace SealAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController<T> : ControllerBase where T : Base
    {
        private readonly FileContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly Config _config;
        private readonly ILogger<T> _logger;

        public BaseApiController(FileContext context, ILogger<T> logger, IOptions<Config> config)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _config = config.Value;
            _logger = logger;
        }

        protected virtual async Task<ActionResult<IEnumerable<T>>> Get()
        {
            _logger.LogInformation(LogMessage.ApiRequest, nameof(this.Get), typeof(T), "ALL");
            if (_dbSet == null)
            {
                _logger.LogError(LogMessage.NotFound, nameof(T));
                return NotFound();
            }
            return await _dbSet.ToListAsync();
        }

        protected virtual async Task<ActionResult<T?>> GetById(int id, bool includeVirtual = false)
        {
            _logger.LogInformation(LogMessage.ApiRequest, nameof(this.GetById), typeof(T), id);
            if (_dbSet == null || id < 0)
            {
                _logger.LogError(LogMessage.NotFound, nameof(T));
                return NotFound();
            }
            return await _dbSet.FindAsync(id);
        }

        protected virtual async Task<ActionResult<T>> Post(T entity)
        {
            _logger.LogInformation(LogMessage.ApiRequest, nameof(this.Post), typeof(T), "N/A");
            SetAuditProperties(entity, true);
            try
            {
                _dbSet.Add(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation(LogMessage.ApiEntity, typeof(T), "added", JsonConvert.SerializeObject(entity));
                return Ok(JsonConvert.SerializeObject(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(Helper.LogException(ex));
                return BadRequest();
            }
        }

        protected virtual async Task<ActionResult<T>> Put(int id, T entity)
        {
            _logger.LogInformation(LogMessage.ApiRequest, nameof(this.Put), typeof(T), id);

            // Check if entity exists
            if (!EntityExists(id))
            {
                _logger.LogError(LogMessage.NotFound, "Record");
                return NotFound();
            }

            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation(LogMessage.ApiEntity, typeof(T), "updated", id);
                return Ok(JsonConvert.SerializeObject(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(Helper.LogException(ex));
                return BadRequest(ex);
            }
        }

        protected virtual async Task<ActionResult<T>> Delete(int id)
        {
            _logger.LogInformation(LogMessage.ApiRequest, nameof(this.Delete), typeof(T), id);
            var target = await _dbSet.FindAsync(id);
            if (target == null)
            {
                _logger.LogError(LogMessage.NotFound, "Record");
                return NotFound();
            }

            try
            {
                _dbSet.Remove(target);
                await _context.SaveChangesAsync();
                _logger.LogInformation(LogMessage.ApiEntity, typeof(T), "deleted", id);
                return Ok(target);
            }
            catch (Exception ex)
            {
                _logger.LogError(Helper.LogException(ex));
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Check for existing entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool EntityExists(int id)
        {
            return _dbSet.Any(e => e.Id == id);
        }

        /// <summary>
        /// Set timestamp properties
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isPost"></param>
        /// <returns></returns>
        private void SetAuditProperties(T entity, bool isPost = false)
        {
            if (isPost)
            {
                entity.Cts = DateTime.UtcNow;
            }
        }
    }
}
