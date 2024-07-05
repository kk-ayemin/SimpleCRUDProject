using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTest.Models;
using ApiTest.DTOs;

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
        public async Task<IActionResult> CreatePost([FromBody] PostDTO postDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userExists = await _context.TblUsers.AnyAsync(u => u.Id == postDTO.UserId);
                if (!userExists){
                    return NotFound($"User ID {postDTO.UserId} not found");
                }
                var newPost = new TblPost
                {
                    PostText = postDTO.PostText,
                    UserId = postDTO.UserId,
                };

                _context.TblPosts.Add(newPost);
                await _context.SaveChangesAsync();

                var createdPost = new 
                {
                    newPost.PostId,
                    postDTO.PostText,
                    postDTO.UserId
                };

                return CreatedAtAction(nameof(GetPostById), new { id = createdPost.PostId }, createdPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPosts()
        {
            var posts = await _context.TblPosts
                .Select(p => new PostDTO
                {
                    PostText = p.PostText,
                    UserId = p.UserId,
                })
                .ToListAsync();

            return posts;
        }

        // GET api/post/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDTO>> GetPostById(int id)
        {
            var post = await _context.TblPosts
                .Where(p => p.PostId == id)
                .Select(p => new PostDTO
                {
                    PostText = p.PostText,
                    UserId = p.UserId,
                })
                .FirstOrDefaultAsync();

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
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostDTO updatedPostDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _context.TblPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.PostText = updatedPostDTO.PostText;
            updatedPostDTO.UserId = post.UserId;

            try
            {
                await _context.SaveChangesAsync();

                return Ok(updatedPostDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }
    }
}
