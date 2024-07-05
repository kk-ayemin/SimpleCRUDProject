using ApiTest.DTOs;
using ApiTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly CrudtestDbContext _context;

        public CommentController(CrudtestDbContext context)
        {
            _context = context;
        }

        // POST api/comment
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentDTO commentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userExists = await _context.TblUsers.AnyAsync(u => u.Id == commentDTO.UserId);
                var postExists = await _context.TblPosts.AnyAsync(p => p.PostId == commentDTO.PostId);
                if (!userExists){
                    return NotFound($"User ID {commentDTO.UserId} was not found");
                }
                if (!postExists){
                    return NotFound($"Post ID {commentDTO.PostId} was not found");
                }
                var newComment = new TblComment
                {
                    CommentText = commentDTO.CommentText,
                    UserId = commentDTO.UserId,
                    PostId = commentDTO.PostId,
                };

                _context.TblComments.Add(newComment);
                await _context.SaveChangesAsync();

                var createdComment = new{
                    newComment.CommentId,
                    commentDTO.CommentText,
                    commentDTO.PostId,
                    commentDTO.UserId,
                };
                return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.CommentId }, createdComment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/comment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments()
        {
            var comments = await _context.TblComments
                .Select(c => new CommentDTO
                {
                    CommentText = c.CommentText,
                    UserId = c.UserId,
                    PostId = c.PostId
                })
                .ToListAsync();

            return comments;
        }

        // GET api/comment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDTO>> GetCommentById(int id)
        {
            var comment = await _context.TblComments
                .Where(c => c.CommentId == id)
                .Select(c => new CommentDTO
                {
                    CommentText = c.CommentText,
                    UserId = c.UserId,
                    PostId = c.PostId
                })
                .FirstOrDefaultAsync();

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
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentDTO updatedCommentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var comment = await _context.TblComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.CommentText = updatedCommentDTO.CommentText;
            updatedCommentDTO.UserId = comment.UserId;
            updatedCommentDTO.PostId = comment.PostId;    


            try 
            {
                await _context.SaveChangesAsync();
                return Ok(updatedCommentDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
            }
        }
    }
}
