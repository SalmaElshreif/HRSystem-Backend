using GraduationProject.DTOs;
using GraduationProject.Helpers;
using GraduationProject.Models;
using GraduationProject_ITI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<ActionResult> AddUser([FromBody]UserReq userReq)
        {
            try
            {
                User user = new User();
                user.Email = userReq.Email;
                user.UserName = userReq.User_Name;
                user.Name = userReq.Name;
                user.Role_Id = userReq.Role_Id;
                user.Password = userReq.Password;

                if (await CheckUserNameExistAsync(user.UserName))
                    return BadRequest(new { Message = "Username Already Exists" });

                if (await CheckEmailExistAsync(user.Email))
                    return BadRequest(new { Message = "Email Already Exists" });

                user.Password = PasswordHasher.HashPassword(user.Password);
                user.Token = "";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var temporaryPassword = new TemporaryPasswords
                {
                    UserID = user.Id,
                    OriginalPassword = userReq.Password
                };
                _context.temporaryPasswords.Add(temporaryPassword);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        private Task<bool> CheckUserNameExistAsync(string username)
        => _context.Users.AnyAsync(x => x.UserName == username);

        private Task<bool> CheckEmailExistAsync(string email)
        => _context.Users.AnyAsync(x => x.Email == email);

        [HttpGet("GetRolesDropdown")]
        public async Task<ActionResult> GetRolesDropdown() 
        {
            var roles = await _context.Roles.ToListAsync();
            
            List<RoleDropdownResponse> rolesDropdown = new List<RoleDropdownResponse>();
            foreach (var role in roles)
            {
                rolesDropdown.Add(new RoleDropdownResponse
                {
                    id = role.Id,
                    name = role.Name,
                }); 
            }
            return Ok(rolesDropdown);
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReq>> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == id)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound();

                var temporaryPassword = await _context.temporaryPasswords
                    .Where(tp => tp.UserID == id)
                    .Select(tp => tp.OriginalPassword)
                    .FirstOrDefaultAsync();

                var userDto = new UserReq
                {
                    User_Name = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    Role_Id = (int)user.Role_Id
                };


                if (!string.IsNullOrEmpty(temporaryPassword))
                {
                    userDto.Password = temporaryPassword;
                }

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<UserReq>> UpdateUser(int id, UserReq updatedUser)
        {
            try
            {
                var userToUpdate = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

                if (userToUpdate == null)
                {
                    return NotFound();
                }

                userToUpdate.Name = updatedUser.Name;
                userToUpdate.UserName = updatedUser.User_Name;
                userToUpdate.Email = updatedUser.Email;
                //userToUpdate.Password = updatedUser.Password;
                userToUpdate.Password = PasswordHasher.HashPassword(updatedUser.Password);
                userToUpdate.Role_Id = updatedUser.Role_Id;


                await _context.SaveChangesAsync();


                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            var associatedPasswords = _context.temporaryPasswords.Where(p => p.UserID == id);
            _context.temporaryPasswords.RemoveRange(associatedPasswords);

            _context.Users.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }



    }
}
