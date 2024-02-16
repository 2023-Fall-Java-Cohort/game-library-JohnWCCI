using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameDataLibrary;
using GameService;
using GameService.Services;

namespace GameService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService publisher;
        private readonly ILogger<PublishersController> logger;

        public PublishersController(IPublisherService publisher, ILogger<PublishersController> logger)
        {
            this.publisher = publisher;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the publishers.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <example>GET: api/Publishers</example>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherModel>>> GetPublishers(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await publisher.GetAllAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        // GET: api/Publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherModel>> GetPublisher(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            PublisherModel? publisherModel;
            try
            {
                 publisherModel = await this.publisher.GetAsync(id);
                if (publisherModel == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return publisherModel;
        }

        // PUT: api/Publishers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisherModel(int id, PublisherModel publisherModel)
        {
            if (id != publisherModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(publisherModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Publishers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PublisherModel>> PostPublisherModel(PublisherModel publisherModel)
        {
            _context.Publishers.Add(publisherModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublisherModel", new { id = publisherModel.Id }, publisherModel);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisherModel(int id)
        {
            var publisherModel = await _context.Publishers.FindAsync(id);
            if (publisherModel == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(publisherModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublisherModelExists(int id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }
    }
}
