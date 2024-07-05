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
    public class UserController : ControllerBase
    {
        private readonly CrudtestDbContext _context;

        public UserController(CrudtestDbContext context)
        {
            _context = context;
        }

        // POST api/user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newUser = new TblUser
                {
                    Username = userDTO.Username,
                    Password = userDTO.Password,
                    IsActive = userDTO.IsActive,
                    Name = userDTO.Name
                };

                _context.TblUsers.Add(newUser);
                await _context.SaveChangesAsync();

                var createdUser = new 
                {
                    newUser.Id,
                    userDTO.Username,
                    userDTO.Password,
                    userDTO.IsActive,
                    userDTO.Name,
                };

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.TblUsers
                .Select(u => new UserDTO
                {
                    Username = u.Username,
                    Password = u.Password,
                    IsActive = u.IsActive ?? false,
                    Name = u.Name,
                })
                .ToListAsync();

            return users;
        }

        // GET api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _context.TblUsers
                .Where(u => u.Id == id)
                .Select(u => new UserDTO
                {
                    Username = u.Username,
                    Password = u.Password,
                    IsActive = u.IsActive ?? false,
                    Name = u.Name,
                })
                .FirstOrDefaultAsync();

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
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO updatedUserDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.TblUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = updatedUserDTO.Username;
            user.Password = updatedUserDTO.Password;
            user.IsActive = updatedUserDTO.IsActive;
            user.Name = updatedUserDTO.Name;

            try
            {
                await _context.SaveChangesAsync();

                return Ok(updatedUserDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }
    }
}
