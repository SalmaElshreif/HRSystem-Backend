using GraduationProject.Models;
using GraduationProject_ITI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject_ITI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public DepartmentController(ApplicationDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }
        [HttpGet("GetAllDepartments")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllDepartments()
        {
            try
            {
                List<DepartmentDto> deptDtoList = await dbContext.Departments
                    .GroupBy(dept => new { dept.Id, dept.Name })
                    .Select(group => new DepartmentDto
                    {
                        DepartmentId = group.Key.Id,
                        DepartmentName = group.Key.Name,
                        EmployeeCount = dbContext.Employees.Count(emp => emp.deptid == group.Key.Id)
                    })
                    .ToListAsync();

                if (deptDtoList.Count > 0)
                    return Ok(deptDtoList);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("dept/{deptId}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int deptId)
        {
            try
            {
                DepartmentDto? departmentDto = await dbContext.Departments
                    .Where(dept => dept.Id == deptId)
                    .Select(dept => new DepartmentDto
                    {
                        DepartmentId = dept.Id,
                        DepartmentName = dept.Name,

                    })
                    .FirstOrDefaultAsync();

                if (departmentDto != null)
                {
                    return Ok(departmentDto);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("AddDepartment")]
        public async Task<ActionResult<DepartmentDto>> AddDepartment(DepartmentDto newDepartmentDto)
        {
            try
            {
                if (await DepartmentExists(newDepartmentDto.DepartmentName))
                {
                    return Conflict("A department with the same name already exists.");
                }
                Department newDepartment = new Department
                {
                    Name = newDepartmentDto.DepartmentName
                };

                dbContext.Departments.Add(newDepartment);
                await dbContext.SaveChangesAsync();

                newDepartmentDto.DepartmentId = newDepartment.Id;

                var departments = await GetAllDepartments();

                return Ok(new { Department = newDepartmentDto, Departments = departments.Value });

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private async Task<bool> DepartmentExists(string departmentName)
        {
            return await dbContext.Departments.AnyAsync(d => d.Name == departmentName);
        }

        [HttpPut("deptId/{_id}")]
        public async Task<ActionResult<Department>> EditDepartment(int _id, DepartmentDto updatedDepartmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Department? department = await dbContext.Departments.Include(e => e.company)
                                                                    .Include(e => e.Employees)
                                                                    .FirstOrDefaultAsync(x => x.Id == _id);

                if (department == null)
                {
                    return NotFound();
                }

                if (await DepartmentExists(updatedDepartmentDto.DepartmentName, _id))
                {
                    return Conflict("A department with the same name already exists.");
                }

                department.Name = updatedDepartmentDto.DepartmentName;

                await dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private async Task<bool> DepartmentExists(string departmentName, int departmentId)
        {
            return await dbContext.Departments.AnyAsync(d => d.Name == departmentName && d.Id != departmentId);
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            try
            {
                Department dept = await dbContext.Departments
                    .Include(e => e.company)
                    .Include(e => e.Employees)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (dept == null)
                    return NotFound("Department not found");

                dbContext.Employees.RemoveRange(dept.Employees);
                dbContext.Departments.Remove(dept);
                await dbContext.SaveChangesAsync();

                return Ok(dept);
            }
            catch (Exception ex)
            {
                return BadRequest("Error deleting department");
            }
        }
    }
}
