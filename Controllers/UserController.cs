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
    public class UserController : ControllerBase
    {
        private readonly CrudtestDbContext _context;

        public UserController(CrudtestDbContext context)
        {
            _context = context;
        }

        // POST api/user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] TblUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newUser = new TblUser
                {
                    Username = user.Username,
                    Password = user.Password,
                    IsActive = user.IsActive,
                    Name = user.Name
                };

                _context.TblUsers.Add(newUser);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // GET api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblUser>>> GetUsers()
        {
            return await _context.TblUsers
                .ToListAsync();
        }

        // GET api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TblUser>> GetUserById(int id)
        {
            var user = await _context.TblUsers
                .Include(u => u.TblPosts)
                .Include(u => u.TblComments)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // DELETE api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.TblUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.TblUsers.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] TblUser updatedUser)
        {
            if (id != updatedUser.Id){
                return BadRequest("User ID mismatch");
            }
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var user = await _context.TblUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password; 
            user.IsActive = updatedUser.IsActive;
            user.Name = updatedUser.Name;
            try{
                 await _context.SaveChangesAsync();

                 return Ok(user);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }
    }
}
