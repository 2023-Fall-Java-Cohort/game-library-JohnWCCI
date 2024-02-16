using Microsoft.AspNetCore.Mvc;
using GameDataLibrary;
using GameService.Services;
using System.Linq.Expressions;

namespace GameService.Controllers
{
    /// <summary>
    /// Takes care of Publishers
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
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
                 publisherModel = await this.publisher.GetAsync(id, cancellationToken);
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
                

        /// <summary>
        /// Puts the publisher model.
        /// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="publisherModel">The publisher model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <example>PUT: api/Publishers/5</example>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<PublisherModel>> PutPublisherModel(int id, PublisherModel publisherModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id != publisherModel.Id)
            {
                return BadRequest("Invalid ID");
            }
            try
            {
               return await publisher.UpdateAsync(publisherModel,cancellationToken);
            }
            catch (Exception ex)
            {
               return BadRequest( new Exception($"Unable to update Publisher record: {publisherModel}", ex));
            }
        }

      
        /// <summary>
        /// Posts the publisher model.
        /// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754 
        /// </summary>
        /// <param name="publisherModel">The publisher model.</param>
        /// <param name="">The .</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <example>POST: api/Publishers</example>
        /// <returns></returns>
      [HttpPost]
        public async Task<ActionResult<PublisherModel>> PostPublisherModel(PublisherModel publisherModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await publisher.AddAsync(publisherModel, cancellationToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new Exception($"Unable to add Publisher record: {publisherModel}", ex));
            }
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisherModel(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            PublisherModel? publisherModel = await publisher.GetAsync(id, cancellationToken);
            if (publisherModel is null)
            {
                return NotFound();
            }
            try
            {
                await publisher.DeleteAsync(publisherModel, cancellationToken);
            }
            catch (Exception ex)
            {

                return BadRequest(new Exception($"Unable to Delete Publisher record: {id}", ex));
            }
          
            return NoContent();
        }
        [HttpGet("{pageIndex}, {pageSize}, {orderBy}")]   
        public async Task<ActionResult<IEnumerable<PublisherModel>>> GetAllPageAsync(int pageIndex, int pageSize = 10, string orderBy = "Id", CancellationToken cancellationToken = default(CancellationToken))
       {
            try
            {
                List<PublisherModel> p =  await publisher.GetAllPageAsync(pageIndex, pageSize, orderBy, cancellationToken);
                if(p.Count == 0)
                {
                    return NotFound();
                }
                return p;
            }
            catch (Exception ex)
            {

                return BadRequest($"{ex.Message}\n Unable to get any publisher using pageIndex {pageIndex}, pageSize {pageSize} orderby {orderBy}");
            }
        }
    }
}

//Task<List<TEntity>> GetAllPageAsync(int pageIndex, int pageSize = 10, string orderBy = "Id", CancellationToken cancellationToken = default(CancellationToken));
//Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
//Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
//Task<List<TEntity>> GetSearchEntityAsync(Expression<Func<TEntity, bool>> predicate, int PageIndex = 1, int PageSize = 20, string