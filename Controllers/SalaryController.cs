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

        //    var selectedVacationDays = generalSettingDTO.SelectedVacationDays;

        //    var salaries = new List<SalaryResponseDto>();

        //    foreach (var employee in employees)
        //    {
        //        if (employee.IsResigned ?? false)
        //        {
        //            continue;
        //        }

        //        var firstWeekDay = settings != null ? settings.selectedFirstWeekendDay : null;
        //        var secondWeekDay = settings != null ? settings.selectedSecondWeekendDay : null;

        //        int firstWeekDaysCount = !string.IsNullOrEmpty(firstWeekDay) ? CountWeekdaysInMonth(DateTime.Now.Year, DateTime.Now.Month, firstWeekDay) : 0;
        //        int secondWeekDaysCount = !string.IsNullOrEmpty(secondWeekDay) ? CountWeekdaysInMonth(DateTime.Now.Year, DateTime.Now.Month, secondWeekDay) : 0;

        //        double DayPrice = employee.salary.NetSalary / 30;

        //        double timeDifferenceInHours = (employee.LeaveTime - employee.AttendanceTime).TotalHours;
        //        double HourPrice = timeDifferenceInHours > 0 ? DayPrice / timeDifferenceInHours : 0;

        //        var holidays = _context.Holidays.Where(h => h.Date.Month == DateTime.Now.Month).ToList();
        //        int HolidaysCount = holidays?.Count() ?? 0;

        //        int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        //        var selectedWeekendDays = new List<string> { generalSettingDTO.selectedFirstWeekendDay, generalSettingDTO.selectedSecondWeekendDay };
        //        int weekendsInMonth = CountWeekendsInMonth(DateTime.Now.Year, DateTime.Now.Month, selectedWeekendDays);

        //        var selectedVacationDaysAsStrings = selectedVacationDays?.Select(day => ((DayOfWeek)day).ToString()).ToList() ?? new List<string>();

        //        int totalOfficialDaysInThisMonth = daysInCurrentMonth - (firstWeekDaysCount + secondWeekDaysCount + HolidaysCount);

        //        var attendances = _context.EmployeeAttendances.Where(z => z.EmployeeId == employee.Id).ToList();

        //        int absenceDayss = totalOfficialDaysInThisMonth - attendances?.Count() ?? 0;

        //        var holidaysAndWeekends = holidays.Select(h => h.Date.DayOfWeek.ToString())
        //            .Concat(selectedWeekendDays)
        //            //.Concat(selectedVacationDaysAsStrings);
        //            .Distinct();

        //        //foreach (var dayOfWeek in holidaysAndWeekends.Distinct())
        //        //{
        //        //    int daysToRemove = absenceDays / 5;  // Assuming 5 working days in a week
        //        //    if (absenceDays % 5 >= (int)Enum.Parse(typeof(DayOfWeek), dayOfWeek))
        //        //        daysToRemove += 1;

        //        //    absenceDays -= daysToRemove;
        //        //}

        //        //// Ensure absence days are not negative
        //        //absenceDays = Math.Max(absenceDays, 0);



        //        //absenceDays -= attendances?.Count() ?? 0;

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
        //            attendanceDays = attendances?.Count() ?? 0,
        //            absenceDays = absenceDayss,
        //            exrtaHours = extraHours,
        //            discountHours = lossHours,
        //            extraSalary = extraHours * HourPrice,
        //            discountSalary = lossHours * HourPrice,
        //            HourlyRate = HourPrice,
        //            DailyRate = DayPrice,
        //            WeekendDays = weekendsInMonth,
        //        };
        //        double totalSalary = employee.salary.NetSalary + salary.extraSalary + salary.discountSalary;

        //        salary.totalSalary = totalSalary - (DayPrice * (30 - attendances.Count()));

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

            var selectedWeekendDays = new List<string> { generalSettingDTO.selectedFirstWeekendDay, generalSettingDTO.selectedSecondWeekendDay };

            var holidays = _context.Holidays.Where(h => h.Date.Month == DateTime.Now.Month).ToList();
            int HolidaysCount = holidays?.Count() ?? 0;

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

                //double timeDifferenceInHours = (employee.LeaveTime - employee.AttendanceTime).TotalHours;

                DateTime leaveTime = DateTime.Parse(employee.LeaveTime);
                DateTime attendanceTime = DateTime.Parse(employee.AttendanceTime);
                double timeDifferenceInHours = (leaveTime - attendanceTime).TotalHours;

                double HourPrice = timeDifferenceInHours > 0 ? DayPrice / timeDifferenceInHours : 0;

                int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                // Calculate absence days excluding weekends, holidays, and selected vacation days
                int absenceDays = 0;

                for (int day = 1; day <= daysInCurrentMonth; day++)
                {
                    var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);

                    // Check if the current day is a weekend day
                    if (selectedWeekendDays.Contains(currentDate.DayOfWeek.ToString()))
                    {
                        continue;
                    }

                    // Check if the current day is a holiday
                    if (holidays.Any(h => h.Date.Date == currentDate.Date))
                    {
                        continue;
                    }

                    // If none of the above conditions is met, consider it as a working day
                    absenceDays++;
                }

                var attendances = _context.EmployeeAttendances.Where(z => z.EmployeeId == employee.Id).ToList();

                int absenceDayss = absenceDays - attendances?.Count() ?? 0;

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
                    id = employee.Id,
                    empName = employee.Name,
                    NetSalary = employee.salary.NetSalary,
                    deptName = employee?.dept?.Name,
                    attendanceDays = attendances?.Count() ?? 0,
                    absenceDays = absenceDayss,
                    exrtaHours = extraHours,
                    discountHours = lossHours,
                    extraSalary = extraHours * HourPrice,
                    discountSalary = lossHours * HourPrice,
                    HourlyRate = HourPrice,
                    DailyRate = DayPrice,
                    WeekendDays = CountWeekendsInMonth(DateTime.Now.Year, DateTime.Now.Month, selectedWeekendDays),
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

        private int CountWeekdaysInMonth(int year, int month, string? dayOfWeek)
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


        [HttpGet("SearchEmployees")]
        public IActionResult GetSalaryReport(int month, int year)
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

            var filteredSalaries = new List<SalaryResponseDto>();

            var employeesForMonthYear = employees.Where(e => _context.EmployeeAttendances.Any(a => a.EmployeeId == e.Id && a.Attendence.Month == month && a.Attendence.Year == year)).ToList();

            if (employeesForMonthYear.Count == 0)
            {
                return NotFound("No employees found for the selected month and year.");
            }

            foreach (var employee in employees)
            {
                if (employee.IsResigned ?? false)
                {
                    continue;
                }

                var firstWeekDay = settings != null ? settings.selectedFirstWeekendDay : null;
                var secondWeekDay = settings != null ? settings.selectedSecondWeekendDay : null;

                int firstWeekDaysCount = !string.IsNullOrEmpty(firstWeekDay) ? CountWeekdaysInMonth(year, month, firstWeekDay) : 0;
                int secondWeekDaysCount = !string.IsNullOrEmpty(secondWeekDay) ? CountWeekdaysInMonth(year, month, secondWeekDay) : 0;

                //double DayPrice = employee.salary.NetSalary / DateTime.DaysInMonth(year, month);
                double DayPrice = employee.salary.NetSalary / 30;

                //double timeDifferenceInHours = (employee.LeaveTime - employee.AttendanceTime).TotalHours;

                DateTime leaveTime = DateTime.Parse(employee.LeaveTime);
                DateTime attendanceTime = DateTime.Parse(employee.AttendanceTime);
                double timeDifferenceInHours = (leaveTime - attendanceTime).TotalHours;

                double HourPrice = timeDifferenceInHours > 0 ? DayPrice / timeDifferenceInHours : 0;

                var holidays = _context.Holidays.Where(h => h.Date.Month == month && h.Date.Year == year).ToList();
                int HolidaysCount = holidays?.Count() ?? 0;

                int daysInCurrentMonth = DateTime.DaysInMonth(year, month);

                //int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                var selectedWeekendDays = new List<string> { generalSettingDTO.selectedFirstWeekendDay, generalSettingDTO.selectedSecondWeekendDay };
                int weekendsInMonth = CountWeekendsInMonth(year, month, selectedWeekendDays);
                //int weekendsInMonth = CountWeekendsInMonth(DateTime.Now.Year, DateTime.Now.Month, selectedWeekendDays);

                var selectedVacationDaysAsStrings = generalSettingDTO.SelectedVacationDays?.Select(day => ((DayOfWeek)day).ToString()).ToList() ?? new List<string>();

                int totalOfficialDaysInThisMonth = daysInCurrentMonth - (firstWeekDaysCount + secondWeekDaysCount + HolidaysCount);

                var attendances = _context.EmployeeAttendances
                    .Where(z => z.EmployeeId == employee.Id && z.Attendence.Month == month && z.Attendence.Year == year)
                    .ToList();

                int absenceDayss = totalOfficialDaysInThisMonth - attendances?.Count() ?? 0;

                var holidaysAndWeekends = holidays.Select(h => h.Date.DayOfWeek.ToString())
                    .Concat(selectedWeekendDays)
                    .Distinct();

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
                    id = employee.Id,
                    empName = employee.Name,
                    NetSalary = employee.salary.NetSalary,
                    deptName = employee?.dept?.Name,
                    attendanceDays = attendances?.Count() ?? 0,
                    absenceDays = absenceDayss,
                    exrtaHours = extraHours,
                    discountHours = lossHours,
                    extraSalary = extraHours * HourPrice,
                    discountSalary = lossHours * HourPrice,
                    HourlyRate = HourPrice,
                    DailyRate = DayPrice,
                    WeekendDays = weekendsInMonth,
                    Month = month,
                    Year = year,
                };
                double totalSalarry = employee.salary.NetSalary + salary.extraSalary + salary.discountSalary;

                //salary.totalSalary = totalSalarry - (DayPrice * (totalOfficialDaysInThisMonth - attendances.Count()));

                salary.totalSalary = totalSalarry - (DayPrice * (30 - attendances.Count()));

                filteredSalaries.Add(salary);
            }

            return Ok(filteredSalaries);
        }


        // Helper method to calculate delay hours
        private double CalculateDelayHours(DateTime arrivalTime, DateTime scheduledArrivalTime)
        {
            if (arrivalTime > scheduledArrivalTime)
            {
                TimeSpan delay = arrivalTime - scheduledArrivalTime;
                return delay.TotalHours;
            }
            return 0;
        }

        // Helper method to calculate early arrival hours
        private double CalculateEarlyArrivalHours(DateTime arrivalTime, DateTime scheduledArrivalTime)
        {
            if (arrivalTime < scheduledArrivalTime)
            {
                TimeSpan earlyArrival = scheduledArrivalTime - arrivalTime;
                return earlyArrival.TotalHours;
            }
            return 0;
        }



        [HttpGet("GetEmployeeAttendanceDetails")]
        public IActionResult GetEmployeeAttendanceDetails(int employeeId, int month, int year)
        {
            var employee = _context.Employees
                .Include(e => e.dept)
                .Include(e => e.salary)
                .FirstOrDefault(e => e.Id == employeeId && e.IsResigned != true);

            if (employee == null)
            {
                return BadRequest("Employee not found or resigned.");
            }

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

            var firstWeekDay = settings != null ? settings.selectedFirstWeekendDay : null;
            var secondWeekDay = settings != null ? settings.selectedSecondWeekendDay : null;

            int firstWeekDaysCount = !string.IsNullOrEmpty(firstWeekDay) ? CountWeekdaysInMonth(year, month, firstWeekDay) : 0;
            int secondWeekDaysCount = !string.IsNullOrEmpty(secondWeekDay) ? CountWeekdaysInMonth(year, month, secondWeekDay) : 0;

            double DayPrice = employee.salary.NetSalary / DateTime.DaysInMonth(year, month);

            var attendances = _context.EmployeeAttendances
                .Where(z => z.EmployeeId == employeeId && z.Attendence.Month == month && z.Attendence.Year == year)
                .OrderBy(a => a.Attendence)
                .ToList();

            var attendanceDetails = new List<EmployeeAttendenceDTO>();

            foreach (var attendance in attendances)
            {
                var timeDifferenceInHours = (attendance.Departure - attendance.Attendence).TotalHours;
                //var originalTimeDifferenceInHours = (employee.LeaveTime - employee.AttendanceTime).TotalHours;

                DateTime leaveTime = DateTime.Parse(employee.LeaveTime);
                DateTime attendanceTime = DateTime.Parse(employee.AttendanceTime);
                double originalTimeDifferenceInHours = (leaveTime - attendanceTime).TotalHours;

                var extraHours = timeDifferenceInHours > originalTimeDifferenceInHours ? timeDifferenceInHours - originalTimeDifferenceInHours : 0;
                var earlyDepartureHours = originalTimeDifferenceInHours > timeDifferenceInHours ? originalTimeDifferenceInHours - timeDifferenceInHours : 0;

                var attendanceDetail = new EmployeeAttendenceDTO
                {
                    id = attendance.Id,
                    name = employee.Name,
                    department = employee?.dept?.Name,
                    attend = attendance.Attendence.ToString("HH:mm"), // Format as "HH:mm" for time-only string
                    leave = attendance.Departure.ToString("HH:mm"),
                    date = attendance.Attendence.Date.ToString("yyyy-MM-dd"), // Format as "yyyy-MM-dd" for date-only string
                    OriginalAttend = employee.AttendanceTime,
                    OriginalLeave = employee.LeaveTime,

                    ExtraHours = extraHours,
                    EarlyDepartureHours = earlyDepartureHours,

                };

                attendanceDetails.Add(attendanceDetail);
            }

            return Ok(attendanceDetails);
        }




    }
}
