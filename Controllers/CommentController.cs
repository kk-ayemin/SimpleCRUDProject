using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTest.Models;

namespace ApiTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CmmtController : ControllerBase
    {
        private readonly CrudtestDbContext _context;

        public CmmtController(CrudtestDbContext context)
        {
            _context = context;
        }

        // POST api/comment
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] TblComment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newComment = new TblComment
                {
                    CommentText = comment.CommentText,
                    UserId = comment.UserId,
                    PostId = comment.PostId,
                };

                _context.TblComments.Add(newComment);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCommentById), new { id = newComment.CommentId }, newComment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // GET api/comment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblComment>>> GetComments()
        {
            return await _context.TblComments
                .ToListAsync();
        }

        // GET api/comment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TblComment>> GetCommentById(int id)
        {
            var comment = await _context.TblComments
                .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // DELETE api/comment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.TblComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.TblComments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT api/comment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id,[FromBody] TblComment updatedComment)
        {
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var comment = await _context.TblComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.CommentText = updatedComment.CommentText; 

            try{
                 await _context.SaveChangesAsync();

                 return Ok(comment);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }
    }
}
