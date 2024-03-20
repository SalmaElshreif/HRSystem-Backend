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

                // Check Username
                if (await CheckUserNameExistAsync(user.UserName))
                    return BadRequest(new { Message = "Username Already Exists" });

                // Check Email
                if (await CheckEmailExistAsync(user.Email))
                    return BadRequest(new { Message = "Email Already Exists" });

                user.Password = PasswordHasher.HashPassword(user.Password);
                user.Token = "";
                _context.Users.Add(user);
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

        //[HttpGet("GetAllUsers")]
        //public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        //{
        //    try
        //    {
        //        List<User> userDtoList = await _context.Users
        //            .Select(user => new User
        //            {
        //                Id = user.Id,
        //                User_Name = user.UserName,
        //                Email = user.Email,
        //                Role_Id = user.role.Id,
        //            })
        //            .ToListAsync();

        //        if (userDtoList.Count > 0)
        //            return Ok(userDtoList);
        //        else
        //            return NotFound();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest();
        //    }
        //}


        [HttpGet("{id}")]
        public async Task<ActionResult<UserReq>> GetUserById(int id)
        {
            try
            {
                UserReq? usersDto = await _context.Users
                    .Where(user => user.Id == id)
                    .Select(user => new UserReq
                    {
                        Id = user.Id,
                        Name = user.Name,
                        User_Name = user.UserName,
                        Email = user.Email,
                        Password = user.Password,
                        Role_Id = user.role.Id,
                        Role_Name = user.role.Name,

                    })
                    .FirstOrDefaultAsync();

                if (usersDto != null)
                {
                    return Ok(usersDto);
                }
                else
                    return NotFound();
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



    }
}
