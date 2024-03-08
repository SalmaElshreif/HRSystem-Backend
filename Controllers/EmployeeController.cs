using GraduationProject.DTOs;
using GraduationProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("saveEmployee")]
        public ActionResult saveEmployee([FromForm] EmpReq empReq)
        {
            if (empReq.Id <= 0)
            {
                // Add new Emp
                return AddEmp(empReq);
            }
            else
            {
                //Edit Emp
                return EditEmp(empReq);
            }
        }
        [HttpPost("addemp")]
        public ActionResult AddEmp([FromForm] EmpReq empReq)
        {
            try
            {
                Employee employee = new Employee();
                employee.phone = empReq.phone;
                employee.Address = empReq.Address;
                employee.Birthdate = empReq.Birthdate;
                employee.Contractdate = empReq.Contractdate;
                employee.deptid = empReq.deptid;
                employee.GId = empReq.GId;
                employee.NationalId = empReq.NationalId;
                employee.Nationality = empReq.Nationality;
                employee.Sal_ID = empReq.Sal_ID;
                employee.user_Id = empReq.user_Id;
                employee.Name = empReq.Name;
                employee.AttendanceTime = empReq.AttandaceTime;
                employee.LeaveTime = empReq.LeaveTime;
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public ActionResult EditEmp([FromForm] EmpReq empReq)
        {
            try
            {
                var employee = _context.Employees.Find(empReq.Id);
                employee.phone = empReq.phone;
                employee.Address = empReq.Address;
                employee.Birthdate = empReq.Birthdate;
                employee.Contractdate = empReq.Contractdate;
                employee.deptid = empReq.deptid;
                employee.GId = empReq.GId;
                employee.NationalId = empReq.NationalId;
                employee.Nationality = empReq.Nationality;
                employee.Sal_ID = empReq.Sal_ID;
                employee.user_Id = empReq.user_Id;
                employee.Name = empReq.Name;
                employee.AttendanceTime = empReq.AttandaceTime;
                employee.LeaveTime = empReq.LeaveTime;

                _context.Entry(employee).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet]
        //[HttpGet("GetAllEmployees")]
        //public ActionResult GetEmployeesList()
        //{
        //    var employees = _context.Employees.ToList();
        //    return Ok(employees);
        //}

        [HttpGet("GetAllEmployees")]
        public ActionResult GetEmployeesList()
        {
            var employeesWithGender = _context.Employees
                .Join(
                    _context.Genders,
                    emp => emp.GId,
                    gender => gender.Id,
                    (emp, gender) => new
                    {
                        emp.Id,
                        emp.Name,
                        emp.phone,
                        emp.Nationality,
                        emp.NationalId,
                        emp.Address,
                        emp.Birthdate,
                        emp.Contractdate,
                        emp.AttendanceTime,
                        emp.LeaveTime,
                        emp.deptid,
                        emp.GId,
                        emp.Sal_ID,
                        emp.user_Id,
                        Gender = new GenderDto
                        {
                            id = gender.Id,
                            name = gender.GName,
                            IsSelected = true
                        }
                    })
                .ToList();

            return Ok(employeesWithGender);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteEmployee(int id)
        {
            try
            {
                var Emp = _context.Employees.FirstOrDefault(z => z.Id == id);
                _context.Remove(Emp);
                _context.SaveChanges();
                return Ok();
            }
            catch { return NotFound(); }
        }   

        [HttpGet("{id}")]
        public ActionResult GetEmployeeById(int id)
        {
            try
            {
                var Emp = _context.Employees.Include(h => h.salary).FirstOrDefault(z => z.Id == id);
                GetEmpResponseDto responseDto = new GetEmpResponseDto();
                responseDto.Id = Emp.Id;
                responseDto.Name = Emp.Name;
                responseDto.phone = Emp.phone;
                responseDto.Nationality = Emp.Nationality;
                responseDto.Address = Emp.Address;
                responseDto.Birthdate = Emp.Birthdate;
                responseDto.Contractdate = Emp.Contractdate;
                responseDto.AttendanceTime = Emp.AttendanceTime;
                responseDto.LeaveTime = Emp.LeaveTime;
                responseDto.GId = Emp.GId;
                responseDto.NetSalary = Emp.salary.NetSalary;
                return Ok(Emp);
            }
            catch { return NotFound(); }
        }

        //[HttpGet("GetGenderDropdown")]
        //public ActionResult GetGenderDropdown()
        //{
        //    var genders = _context.Genders.ToList();

        //    List<GenderDto> genderDto = new List<GenderDto>();
        //    foreach (var gender in genders)
        //    {
        //        genderDto.Add(new GenderDto
        //        {
        //            id = gender.Id,
        //            name = gender.GName
        //        });
        //    }
        //    return Ok(genderDto);
        //}


        //[HttpGet("genderId")]
        //public async Task<GenderDto> GetGenderById(int id)
        //{
        //    GenderDto? genderDto = await _context.Genders
        //    .Where(gender => gender.Id == id)
        //    .Select(gender => new GenderDto
        //    {
        //        id = gender.Id,
        //        name = gender.GName,
        //        IsSelected = true
        //    })
        //    .FirstOrDefaultAsync();

        //    return genderDto;
        //}

        [HttpGet("genderId")]
        public async Task<ActionResult<GenderDto>> GetGenderById(int id)
        {
            try
            {
                GenderDto? genderDto = await _context.Genders
                    .Where(gender => gender.Id == id)
                    .Select(gender => new GenderDto
                    {
                        id = gender.Id,
                        name = gender.GName,
                        IsSelected = true
                    })
                    .FirstOrDefaultAsync();

                if (genderDto == null)
                {
                    return NotFound(); // Return 404 if gender is not found
                }

                return Ok(genderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
