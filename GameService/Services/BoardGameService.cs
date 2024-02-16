using GameDataLibrary;
using Microsoft.EntityFrameworkCore;

namespace GameService.Services
{
    public class BoardGameService : Service<BoardGameModel>, IBoardGameService
    {
        public BoardGameService(GameContext dbContext, ILogger<BoardGameService> logger)
            : base(dbContext, logger) 
        {

        }
    }
}
