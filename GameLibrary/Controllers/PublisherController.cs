using GameDataLibrary;
using GameLibrary.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Controllers
{
    public class PublisherController : Controller
    {
        private readonly ILogger<PublisherController> logger;
        private readonly IPublisherRepository repository;

        public PublisherController(ILogger<PublisherController> logger, IPublisherRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
            logger.LogInformation($"PublisherController started");
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogInformation($"{nameof(Index)}");
            return View(await repository.GetAllAsync(cancellationToken));
        }

        // GET: Publisher/Create
        public IActionResult Create()
        {
            logger.LogInformation($"{nameof(Create)}");
            PublisherModel pub = new PublisherModel();
            return View(pub);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] PublisherModel publisherModel)
        {
            logger.LogInformation($"{nameof(Create)} Save model");
            if (ModelState.IsValid)
            {
                await repository.AddAsync(publisherModel);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                logger.LogInformation($"{nameof(Create)} invalid model");
            }
            return View(publisherModel);
        }

    }
}
