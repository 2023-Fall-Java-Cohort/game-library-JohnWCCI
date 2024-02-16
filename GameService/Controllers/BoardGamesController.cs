using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameDataLibrary;
using GameService;

namespace GameService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly GameContext _context;

        public BoardGamesController(GameContext context)
        {
            _context = context;
        }

        // GET: api/BoardGameModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardGameModel>>> GetBoards()
        {
            return await _context.Boards.ToListAsync();
        }

        // GET: api/BoardGameModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardGameModel>> GetBoardGameModel(int id)
        {
            var boardGameModel = await _context.Boards.FindAsync(id);

            if (boardGameModel == null)
            {
                return NotFound();
            }

            return boardGameModel;
        }

        // PUT: api/BoardGameModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoardGameModel(int id, BoardGameModel boardGameModel)
        {
            if (id != boardGameModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(boardGameModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardGameModelExists(id))
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

        // POST: api/BoardGameModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BoardGameModel>> PostBoardGameModel(BoardGameModel boardGameModel)
        {
            _context.Boards.Add(boardGameModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoardGameModel", new { id = boardGameModel.Id }, boardGameModel);
        }

        // DELETE: api/BoardGameModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoardGameModel(int id)
        {
            var boardGameModel = await _context.Boards.FindAsync(id);
            if (boardGameModel == null)
            {
                return NotFound();
            }

            _context.Boards.Remove(boardGameModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoardGameModelExists(int id)
        {
            return _context.Boards.Any(e => e.Id == id);
        }
    }
}
