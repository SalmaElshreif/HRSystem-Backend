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


        [HttpGet("GetAllEmployees")]
        public IActionResult GetAllEmployeesDropdown()
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
                        leave = item.Departure?.ToString("HH:mm"),
                        department = item.Employee?.dept?.Name,
                        day = item.Attendence.ToShortDateString(),
                    };

                    empAttendDto.Add(employeeAttendenceDTO);
                }

                return Ok(empAttendDto);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeAttendance>> GetAttendanceById(int id)
        {
            var attendance = await _context.EmployeeAttendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound(); 
            }

            return attendance;
        }


        [HttpPost]
        public IActionResult AddEmpAttendace([FromBody] SaveEmpRequest request)
        {
            EmployeeAttendance employeeAttendance = new EmployeeAttendance();
            employeeAttendance.Attendence = request.attendance;
            //employeeAttendance.Departure = request.departure;
            employeeAttendance.EmployeeId = request.EmployeeId.Value; ;

            _context.EmployeeAttendances.Add(employeeAttendance);
            _context.SaveChanges();
            return Ok();
        }


        [HttpPut]
        public IActionResult EditEmpAttendace(EditEmpRequest request)
        {
            var employeeAttendance = _context.EmployeeAttendances.Where(z => z.Id == request.id).FirstOrDefault();

            employeeAttendance.Departure = request.departure;
            employeeAttendance.Attendence = request.attendance;

            _context.Entry(employeeAttendance).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
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


        [HttpPost("GetAttendancies")]
        public IActionResult GetAttendancies([FromBody] GetAttendanceRequest attendanceRequest)
        {
            var attendacies2 = _context.EmployeeAttendances
                                    .Include(h => h.Employee)
                                        .ThenInclude(h => h.dept)
                                    .Where(h => h.Attendence >= attendanceRequest.from &&
                                               h.Departure <= attendanceRequest.to)
                                    .ToList();

            List<EmployeesAttendancesResponse> employeesAttendances = new List<EmployeesAttendancesResponse>();
            foreach (var attend in attendacies2)
            {
                employeesAttendances.Add(new EmployeesAttendancesResponse
                {
                    id = attend.Id,
                    empName = attend.Employee.Name,
                    deptName = attend.Employee.dept.Name,
                    day = attend.Attendence.ToShortDateString(),
                    attendance = attend.Attendence.ToShortTimeString(),
                    departure = attend.Departure.Value.ToShortTimeString(),
                });
            }
            return Ok(employeesAttendances);
        }


        private bool EmployeeExists(int id)
        {
            return _context.EmployeeAttendances.Any(e => e.Id == id);
        }


        [HttpGet("GetEmployeeAttendanceBetweenDates")]
        public IActionResult GetEmployeeAttendanceBetweenDates(DateTime startDate, DateTime endDate)
        {
            var empAttend = _context.EmployeeAttendances
                .Include(e => e.Employee)
                .ThenInclude(e => e.dept)
                .Where(e => e.Attendence >= startDate && e.Attendence <= endDate)
                .ToList();

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
                        leave = item.Departure.Value.ToString("HH:mm"),
                        department = item.Employee?.dept?.Name,
                        day = item.Attendence.ToShortDateString(),
                    };

                    empAttendDto.Add(employeeAttendenceDTO);
                }

                return Ok(empAttendDto);
            }
        }


    }
}
