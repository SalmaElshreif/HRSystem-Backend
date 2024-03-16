using GraduationProject.DTOs;
using GraduationProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public IActionResult GetAttendancies(GetAttendanceRequest attendanceRequest) 
        //{
        //    //var attendacies = _context.EmployeeAttendances
        //    //                        .Where(h => h.Attendence.Day == DateTime.Now.Day && 
        //    //                                    h.Attendence.Month == DateTime.Now.Month && 
        //    //                                    h.Attendence.Year == DateTime.Now.Year)
        //    //                        .ToList();
        //    var attendacies2 = _context.EmployeeAttendances
        //                            .Where(h => h.Attendence >= attendanceRequest.from && 
        //                                        h.Departure <= attendanceRequest.to)
        //                            .ToList();
        //    return Ok(attendacies2);
        //}

        //[HttpPost]
        //public IActionResult AddEmpAttendace(SaveEmpRequest request)
        //{
        //    EmployeeAttendance employeeAttendance = new EmployeeAttendance();
        //    employeeAttendance.Attendence = request.attendance;
        //    employeeAttendance.Departure = request.departure;
        //    employeeAttendance.EmployeeId = request.EmpId;

        //    _context.EmployeeAttendances.Add(employeeAttendance);
        //    _context.SaveChanges();
        //    return Ok();
        //}

        //[HttpPut]
        //public IActionResult EditEmpAttendace(SaveEmpRequest request)
        //{
        //    var employeeAttendance = _context.EmployeeAttendances.Where(z => z.Id == request.id).FirstOrDefault();

        //    employeeAttendance.Departure = request.departure;
        //    employeeAttendance.Attendence = request.attendance;
        //    employeeAttendance.EmployeeId = request.EmpId;

        //    _context.Entry(employeeAttendance).State = EntityState.Modified;
        //    _context.SaveChanges();
        //    return Ok();
        //}

        //[HttpDelete]
        //public IActionResult SaveEmpAttendace(int id)
        //{
        //    var empAttendance = _context.EmployeeAttendances.Find(id);
        //    _context.EmployeeAttendances.Remove(empAttendance);
        //    _context.SaveChanges();
        //    return Ok();
        //}



        [HttpGet("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            var emps = _context.Employees.ToList();

            List<GetEmpsResponse> empsResponses = new List<GetEmpsResponse>();

            foreach (var emp in emps)
            {
                empsResponses.Add(new GetEmpsResponse { id = emp.Id, name = emp.Name });
            }

            return Ok(empsResponses);
        }






        [HttpGet("GetAllEmployeeAttendance")]
        public IActionResult GetAll()
        {
            List<EmployeeAttendance> empAttend = _context.EmployeeAttendances.Include(e => e.Employee).ThenInclude(e => e.dept).ToList();

            if (empAttend == null || empAttend.Count == 0)
            {
                return NotFound();
            }
            else
            {
                List<EmployeeAttendenceDTO> empAttendDto = new List<EmployeeAttendenceDTO>();

                foreach (EmployeeAttendance item in empAttend)
                {
                    EmployeeAttendenceDTO employeeAttendenceDTO = new EmployeeAttendenceDTO
                    {
                        id = item.Id,
                        name = item.Employee?.Name,
                        attend = item.Attendence.ToString("HH:mm"),
                        leave = item.Departure.ToString("HH:mm"),
                        department = item.Employee?.dept?.Name,
                    };

                    empAttendDto.Add(employeeAttendenceDTO);
                }

                return Ok(empAttendDto);
            }
        }

        [HttpDelete("DeleteEmployeeAttendance/{id}")]
        public IActionResult DeleteEmployeeAttendance(int id)
        {
            var empAttend = _context.EmployeeAttendances.Find(id);

            if (empAttend == null)
            {
                return NotFound();
            }

            _context.EmployeeAttendances.Remove(empAttend);
            _context.SaveChanges();

            return NoContent();
        }


        [HttpPut("UpdateEmployeeAttendance/{id}")]
        public IActionResult UpdateEmployeeAttendance(int id, [FromBody] EmployeeAttendenceDTO updatedEmployee)
        {
            if (id != updatedEmployee.id)
            {
                return BadRequest("Invalid ID");
            }

            var existingEmployee = _context.EmployeeAttendances.Include(e => e.Employee).FirstOrDefault(e => e.Id == id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.Employee.Name = updatedEmployee.name;
            //existingEmployee.Attendence = updatedEmployee.attend;
            string dateFormat = "HH:mm"; // Specify the format of the date string
            if (DateTime.TryParseExact(updatedEmployee.attend, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                existingEmployee.Attendence = parsedDate;
            }
            //existingEmployee.Departure = updatedEmployee.leave;
            string dateFormatLeave = "HH:mm"; // Specify the format of the date string
            if (DateTime.TryParseExact(updatedEmployee.leave, dateFormatLeave, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateLeave))
            {
                existingEmployee.Departure = parsedDateLeave;
            }

            try
            {
                _context.SaveChanges();
                return Ok(existingEmployee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool EmployeeExists(int id)
        {
            return _context.EmployeeAttendances.Any(e => e.Id == id);
        }

    }
}
