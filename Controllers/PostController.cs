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
    public class PostController : ControllerBase
    {
        private readonly CrudtestDbContext _context;

        public PostController(CrudtestDbContext context)
        {
            _context = context;
        }

        // POST api/post
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] TblPost post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newPost = new TblPost
                {
                    PostText = post.PostText,
                    UserId = post.UserId,
                };

                _context.TblPosts.Add(newPost);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPostById), new { id = newPost.PostId }, newPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // GET api/post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblPost>>> GetPosts()
        {
            return await _context.TblPosts
                .ToListAsync();
        }

        // GET api/post/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TblPost>> GetPostById(int id)
        {
            var post = await _context.TblPosts
                .Include(p => p.TblComments)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // DELETE api/post/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.TblPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.TblPosts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT api/post/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id,[FromBody] TblPost updatedPost)
        {
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var post = await _context.TblPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.PostText = updatedPost.PostText; 

            try{
                 await _context.SaveChangesAsync();

                 return Ok(post);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }
    }
}
