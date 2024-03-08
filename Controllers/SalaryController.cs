using GraduationProject.DTOs;
using GraduationProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SalaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet("{id}")]
        //public IActionResult GetEmployeeSalary(int id)
        //{

        //    var Emp = _context.Employees
        //                        .Include(h => h.dept)
        //                        .Include(h => h.salary)
        //                        .FirstOrDefault(h => h.Id == id);

        //    var settings = _context.generalSettings.FirstOrDefault();

        //    var firstWeekDay = settings != null ? settings.selectedFirstWeekendDay : null;
        //    var secondWeekDay = settings != null ? settings.selectedSecondWeekendDay : null;

        //    int firstWeekDaysCount = firstWeekDay != null ? 4 : 0;
        //    int secondWeekDaysCount = secondWeekDay != null ? 4 : 0;

        //    var holidays = _context.Holidays.Where(h => h.Date.Month == DateTime.Now.Month).ToList();

        //    int HolidaysCount = holidays != null ? holidays.Count() : 0;

        //    int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        //    int totalOfficialDaysInThisMonth = daysInCurrentMonth - firstWeekDaysCount - secondWeekDaysCount - HolidaysCount;

        //    double DayPrice = Emp.salary.NetSalary / totalOfficialDaysInThisMonth;

        //    double timeDifferenceInHours = (Emp.LeaveTime - Emp.AttendanceTime).TotalHours;

        //    double HourPrice = DayPrice / timeDifferenceInHours;


        //    var attendances = _context.EmployeeAttendances.Where(z => z.EmployeeId == id).ToList();

        //    double extraHours = 0;
        //    double lossHours = 0;

        //    foreach (var attendance in attendances)
        //    {
        //        var DifferenceInHours = (attendance.Departure - attendance.Attendence).TotalHours;
        //        var resetHours = DifferenceInHours - timeDifferenceInHours;
        //        if (resetHours > 0)
        //        {
        //            extraHours += resetHours;
        //        }
        //        else
        //        {
        //            lossHours += resetHours;
        //        }
        //    }

        //    //double extraHoursAdjustment = extraHoursInput;
        //    //double discountHoursAdjustment = discountHoursInput;

        //    //extraHours *= extraHoursAdjustment;
        //    //lossHours *= discountHoursAdjustment;

        //    double extraHoursAdjustment = settings.ExtraHourRate ?? 0;
        //    double discountHoursAdjustment = settings.DiscountHourRate ?? 0;

        //    extraHours *= extraHoursAdjustment;
        //    lossHours *= discountHoursAdjustment;

        //    SalaryResponseDto responseDto = new SalaryResponseDto();
        //    responseDto.empName = Emp.Name;
        //    responseDto.NetSalary = Emp.salary.NetSalary;
        //    responseDto.deptName = Emp.dept.Name;

        //    responseDto.attendanceDays = attendances != null ? attendances.Count() : 0;
        //    responseDto.absenceDays = totalOfficialDaysInThisMonth - (attendances != null ? attendances.Count() : 0);
        //    responseDto.exrtaHours = extraHours;
        //    responseDto.discountHours = lossHours;
        //    responseDto.extraSalary = extraHours * HourPrice;
        //    responseDto.discountSalary = lossHours * HourPrice;
        //    responseDto.totalSalary = responseDto.NetSalary + responseDto.extraSalary + responseDto.discountSalary;
        //    return Ok(responseDto);
        //}


        //[HttpGet]
        //public IActionResult GetAllSalaries()
        //{

        //    var salaries = _context.Employees
        //        .Include(h => h.dept)
        //        .Include(h => h.salary)
        //        .Select(emp => new SalaryResponseDto
        //        {
        //            empName = emp.Name,
        //            NetSalary = emp.salary.NetSalary,
        //            deptName = emp.dept.Name,
        //            attendanceDays = attendances != null ? attendances.Count() : 0,

        //        })
        //        .ToList();


        //    return Ok(salaries);
        //}


        //[HttpGet]
        //public IActionResult GetAllSalaries()
        //{

        //    var employees = _context.Employees
        //        .Include(e => e.dept)
        //        .Include(e => e.salary)
        //        .Where(e => e.IsResigned != true)
        //        .ToList();

        //    var settings = _context.generalSettings.OrderByDescending(s => s.id).FirstOrDefault();

        //    if (settings == null)
        //    {
        //        return BadRequest("General settings not found.");
        //    }
        //    var generalSettingDTO = new GeneralSettingDTO
        //    {
        //        DiscountHourRate = settings.DiscountHourRate,
        //        ExtraHourRate = settings.ExtraHourRate,
        //        selectedFirstWeekendDay = settings.selectedFirstWeekendDay,
        //        selectedSecondWeekendDay = settings.selectedSecondWeekendDay,
        //    };

        //    var salaries = new List<SalaryResponseDto>();

        //    foreach (var employee in employees)
        //    {
        //        if (employee.IsResigned ?? false)
        //        {
        //            continue;
        //        }

        //        var firstWeekDay = settings != null ? settings.selectedFirstWeekendDay : null;
        //        var secondWeekDay = settings != null ? settings.selectedSecondWeekendDay : null;

        //        int firstWeekDaysCount = !string.IsNullOrEmpty(firstWeekDay) ? 4 : 0;
        //        int secondWeekDaysCount = !string.IsNullOrEmpty(secondWeekDay) ? 4 : 0;

        //        var holidays = _context.Holidays.Where(h => h.Date.Month == DateTime.Now.Month).ToList();

        //        int HolidaysCount = holidays != null ? holidays.Count() : 0;

        //        int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        //        int totalOfficialDaysInThisMonth = daysInCurrentMonth - firstWeekDaysCount - secondWeekDaysCount - HolidaysCount;

        //        double DayPrice = totalOfficialDaysInThisMonth > 0 ? employee.salary.NetSalary / totalOfficialDaysInThisMonth : 0;

        //        double timeDifferenceInHours = (employee.LeaveTime - employee.AttendanceTime).TotalHours;

        //        double HourPrice = timeDifferenceInHours > 0 ? DayPrice / timeDifferenceInHours : 0;


        //        var attendances = _context.EmployeeAttendances.Where(z => z.EmployeeId == employee.Id).ToList();

        //        double extraHours = 0;
        //        double lossHours = 0;

        //        foreach (var attendance in attendances)
        //        {
        //            var DifferenceInHours = (attendance.Departure - attendance.Attendence).TotalHours;
        //            var resetHours = DifferenceInHours - timeDifferenceInHours;
        //            if (resetHours > 0)
        //            {
        //                extraHours += resetHours;
        //            }
        //            else
        //            {
        //                lossHours += resetHours;
        //            }
        //        }

        //        double extraHoursAdjustment = settings.ExtraHourRate ?? 0;
        //        double discountHoursAdjustment = settings.DiscountHourRate ?? 0;

        //        extraHours *= extraHoursAdjustment;
        //        lossHours *= discountHoursAdjustment;

        //        var salary = new SalaryResponseDto
        //        {
        //            empName = employee.Name,
        //            NetSalary = employee.salary.NetSalary,
        //            deptName = employee.dept.Name,
        //            attendanceDays = attendances != null ? attendances.Count() : 0,
        //            absenceDays = totalOfficialDaysInThisMonth - (attendances != null ? attendances.Count() : 0),
        //            exrtaHours = extraHours,
        //            discountHours = lossHours,
        //            extraSalary = extraHours * HourPrice,
        //            discountSalary = lossHours * HourPrice,
        //        };

        //        salary.totalSalary = salary.NetSalary + salary.extraSalary + salary.discountSalary;

        //        salaries.Add(salary);

        //    }

        //    return Ok(salaries);
        //}



        [HttpGet]
        public IActionResult GetAllSalaries()
        {
            var employees = _context.Employees
                .Include(e => e.dept)
                .Include(e => e.salary)
                .Where(e => e.IsResigned != true)
                .ToList();

            var settings = _context.generalSettings.OrderByDescending(s => s.id).FirstOrDefault();

            if (settings == null)
            {
                return BadRequest("General settings not found.");
            }

            var generalSettingDTO = new GeneralSettingDTO
            {
                DiscountHourRate = settings.DiscountHourRate,
                ExtraHourRate = settings.ExtraHourRate,
                selectedFirstWeekendDay = settings.selectedFirstWeekendDay,
                selectedSecondWeekendDay = settings.selectedSecondWeekendDay,
            };

            var selectedVacationDays = generalSettingDTO.SelectedVacationDays;

            var salaries = new List<SalaryResponseDto>();

            foreach (var employee in employees)
            {
                if (employee.IsResigned ?? false)
                {
                    continue;
                }

                var firstWeekDay = settings != null ? settings.selectedFirstWeekendDay : null;
                var secondWeekDay = settings != null ? settings.selectedSecondWeekendDay : null;

                int firstWeekDaysCount = !string.IsNullOrEmpty(firstWeekDay) ? CountWeekdaysInMonth(DateTime.Now.Year, DateTime.Now.Month, firstWeekDay) : 0;
                int secondWeekDaysCount = !string.IsNullOrEmpty(secondWeekDay) ? CountWeekdaysInMonth(DateTime.Now.Year, DateTime.Now.Month, secondWeekDay) : 0;

                double DayPrice = employee.salary.NetSalary / 30;

                double timeDifferenceInHours = (employee.LeaveTime - employee.AttendanceTime).TotalHours;
                double HourPrice = timeDifferenceInHours > 0 ? DayPrice / timeDifferenceInHours : 0;

                var holidays = _context.Holidays.Where(h => h.Date.Month == DateTime.Now.Month).ToList();
                int HolidaysCount = holidays?.Count() ?? 0;

                int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                var selectedWeekendDays = new List<string> { generalSettingDTO.selectedFirstWeekendDay, generalSettingDTO.selectedSecondWeekendDay };
                int weekendsInMonth = CountWeekendsInMonth(DateTime.Now.Year, DateTime.Now.Month, selectedWeekendDays);

                var selectedVacationDaysAsStrings = selectedVacationDays?.Select(day => ((DayOfWeek)day).ToString()).ToList() ?? new List<string>();

                // Calculate absence days excluding weekends, holidays, and selected vacation days

                //int absenceDays = daysInCurrentMonth - (firstWeekDaysCount + secondWeekDaysCount + HolidaysCount);

                int totalOfficialDaysInThisMonth = daysInCurrentMonth - (firstWeekDaysCount + secondWeekDaysCount + HolidaysCount);

                var attendances = _context.EmployeeAttendances.Where(z => z.EmployeeId == employee.Id).ToList();

                int absenceDayss = totalOfficialDaysInThisMonth - attendances?.Count() ?? 0;

                var holidaysAndWeekends = holidays.Select(h => h.Date.DayOfWeek.ToString())
                    .Concat(selectedWeekendDays)
                    //.Concat(selectedVacationDaysAsStrings);
                    .Distinct();

                //foreach (var dayOfWeek in holidaysAndWeekends.Distinct())
                //{
                //    int daysToRemove = absenceDays / 5;  // Assuming 5 working days in a week
                //    if (absenceDays % 5 >= (int)Enum.Parse(typeof(DayOfWeek), dayOfWeek))
                //        daysToRemove += 1;

                //    absenceDays -= daysToRemove;
                //}

                //// Ensure absence days are not negative
                //absenceDays = Math.Max(absenceDays, 0);

                

                //absenceDays -= attendances?.Count() ?? 0;



                double extraHours = 0;
                double lossHours = 0;

                foreach (var attendance in attendances)
                {
                    var DifferenceInHours = (attendance.Departure - attendance.Attendence).TotalHours;
                    var resetHours = DifferenceInHours - timeDifferenceInHours;
                    if (resetHours > 0)
                    {
                        extraHours += resetHours;
                    }
                    else
                    {
                        lossHours += resetHours;
                    }
                }

                double extraHoursAdjustment = settings.ExtraHourRate ?? 0;
                double discountHoursAdjustment = settings.DiscountHourRate ?? 0;

                extraHours *= extraHoursAdjustment;
                lossHours *= discountHoursAdjustment;

                var salary = new SalaryResponseDto
                {
                    empName = employee.Name,
                    NetSalary = employee.salary.NetSalary,
                    deptName = employee.dept.Name,
                    attendanceDays = attendances?.Count() ?? 0,
                    absenceDays = absenceDayss,
                    exrtaHours = extraHours,
                    discountHours = lossHours,
                    extraSalary = extraHours * HourPrice,
                    discountSalary = lossHours * HourPrice,
                    HourlyRate = HourPrice,
                    DailyRate = DayPrice,
                    WeekendDays = weekendsInMonth,
                };
                double totalSalary = employee.salary.NetSalary + salary.extraSalary + salary.discountSalary;

                salary.totalSalary = totalSalary - (DayPrice * (30 - attendances.Count()));

                salaries.Add(salary);
            }

            return Ok(salaries);
        }



        private int CountWeekendsInMonth(int year, int month, List<string> selectedWeekendDays)
        {
            int weekendsCount = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int i = 1; i <= daysInMonth; i++)
            {
                var currentDate = new DateTime(year, month, i);
                if (selectedWeekendDays.Contains(currentDate.DayOfWeek.ToString()))
                {
                    weekendsCount++;
                }
            }

            return weekendsCount;
        }

        private int CountWeekdaysInMonth(int year, int month, string dayOfWeek)
        {
            int count = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int i = 1; i <= daysInMonth; i++)
            {
                var currentDate = new DateTime(year, month, i);
                if (currentDate.DayOfWeek.ToString() == dayOfWeek)
                {
                    count++;
                }
            }

            return count;
        }

        [HttpGet("searchEmployees")]
        public IActionResult SearchEmployees([FromQuery] SalaryResponseDto salaryDto)
        {
            try
            {
                var employees = _context.Employees
                    .Where(e => e.Contractdate.Month == salaryDto.Month && e.Contractdate.Year == salaryDto.Year)
                    .ToList();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


    }
}
