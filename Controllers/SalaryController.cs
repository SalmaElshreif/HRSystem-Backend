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

        [HttpGet]
        public IActionResult GetAllSalaries()
        {
            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            var firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

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

            var absenceDaysToDate = new Dictionary<int, int>();

            var holidays = _context.Holidays
        .Where(h => h.Date >= firstDayOfMonth && h.Date <= lastDayOfMonth)
        .ToList();
            int HolidaysCount = holidays?.Count() ?? 0;

            var salaries = new List<SalaryResponseDto>();

            foreach (var employee in employees)
            {
                if (employee.IsResigned ?? false)
                {
                    continue;
                }

                var absenceDays = CalculateAbsenceDaysToDate(employee, holidays, settings);
                absenceDaysToDate.Add(employee.Id, absenceDays);

                var firstWeekDay = settings != null ? settings.selectedFirstWeekendDay : null;
                var secondWeekDay = settings != null ? settings.selectedSecondWeekendDay : null;

                int firstWeekDaysCount = !string.IsNullOrEmpty(firstWeekDay) ? CountWeekdaysInMonth(DateTime.Now.Year, DateTime.Now.Month, firstWeekDay) : 0;
                int secondWeekDaysCount = !string.IsNullOrEmpty(secondWeekDay) ? CountWeekdaysInMonth(DateTime.Now.Year, DateTime.Now.Month, secondWeekDay) : 0;

                int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                //double DayPrice = employee.salary.NetSalary / daysInCurrentMonth;

                double DayPrice = employee.salary.NetSalary / 22;


                DateTime leaveTime = DateTime.Parse(employee.LeaveTime);
                DateTime attendanceTime = DateTime.Parse(employee.AttendanceTime);
                double timeDifferenceInHours = (leaveTime - attendanceTime).TotalHours;

                double HourPrice = timeDifferenceInHours > 0 ? DayPrice / timeDifferenceInHours : 0;


                var attendances = _context.EmployeeAttendances.Where(z => z.EmployeeId == employee.Id && z.Attendence >= firstDayOfMonth && z.Attendence <= lastDayOfMonth).ToList();



                double extraHours = 0;
                double lossHours = 0;

                foreach (var attendance in attendances)
                {
                    if (attendance.Departure != null)
                    {
                        TimeSpan timeDifference = new TimeSpan();
                        double attendanceTimeEmp = 0.0;

                        timeDifference = attendanceTime.TimeOfDay - attendance.Attendence.TimeOfDay;

                        attendanceTimeEmp = timeDifference.TotalHours;

                        TimeSpan timeDifference2 = new TimeSpan();
                        double departureTimeEmp = 0.0;

                        timeDifference2 = attendance.Departure.Value.TimeOfDay - leaveTime.TimeOfDay;

                        departureTimeEmp = timeDifference2.TotalHours;


                        var resetHours = attendanceTimeEmp + departureTimeEmp;

                        if (attendanceTimeEmp > 0)
                        {
                            extraHours += attendanceTimeEmp;
                        }
                        else
                        {
                            lossHours += attendanceTimeEmp;
                        }

                        if (departureTimeEmp > 0)
                        {
                            extraHours += departureTimeEmp;
                        }
                        else
                        {
                            lossHours += departureTimeEmp;
                        }
                    }
                }


                double extraHoursAdjustment = settings.ExtraHourRate ?? 0;
                double discountHoursAdjustment = settings.DiscountHourRate ?? 0;

                if (settings.Method == "hour")
                {
                    extraHours *= extraHoursAdjustment;
                    lossHours *= discountHoursAdjustment;
                }
                int weekendDaysUntilToday = CountWeekendsUntilToday(DateTime.Now.Year, DateTime.Now.Month, selectedWeekendDays);


                var salary = new SalaryResponseDto
                {
                    id = employee.Id,
                    empName = employee.Name,
                    NetSalary = employee.salary.NetSalary,
                    deptName = employee?.dept?.Name,
                    attendanceDays = attendances?.Where(e=> e.Departure != null).Count() ?? 0,
                    //absenceDays = absenceDayss,
                    exrtaHours = extraHours,
                    discountHours = lossHours,
                    //extraSalary = extraHours * HourPrice,
                    //discountSalary = lossHours * HourPrice,
                    extraSalary = (double)(settings.Method == "hour" ? (extraHours * HourPrice) : (extraHours * settings.ExtraHourRate)),
                    discountSalary = (double)(settings.Method == "hour" ? (lossHours * HourPrice) : (lossHours * settings.DiscountHourRate)),
                    HourlyRate = HourPrice,
                    DailyRate = DayPrice,
                    WeekendDays = CountWeekendsUntilToday(DateTime.Now.Year, DateTime.Now.Month, selectedWeekendDays),
                    AbsenceDaysToDate = absenceDaysToDate[employee.Id],
                    Month = currentMonth,
                    Year = currentYear,
                };

                double weekendDaysSalary = weekendDaysUntilToday * salary.DailyRate;


                double totalSalary = employee.salary.NetSalary + salary.extraSalary + salary.discountSalary ;

                salary.totalSalary = totalSalary - (DayPrice * (22 - attendances.Where(e=> e.Departure != null).Count()));

                salaries.Add(salary);
            }

            return Ok(salaries);
        }



        private int CalculateAbsenceDaysToDate(Employee employee, List<Holiday> holidays, GeneralSettings generalSettings)
        {
            int absenceDaysToDate = 0;
            int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            for (int day = 1; day <= DateTime.Now.Day; day++)
            {
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);

                if (IsWeekend(currentDate.DayOfWeek, generalSettings) || holidays.Any(h => h.Date.Date == currentDate.Date))
                {
                    continue;
                }

                var attendance = _context.EmployeeAttendances
                    .FirstOrDefault(z => z.EmployeeId == employee.Id && z.Attendence.Date == currentDate.Date);

                if (attendance == null)
                {
                    absenceDaysToDate++;
                }
            }

            return absenceDaysToDate;
        }

        private bool IsWeekend(DayOfWeek dayOfWeek, GeneralSettings generalSettings)
        {
            return dayOfWeek.ToString() == generalSettings.selectedFirstWeekendDay ||
                   dayOfWeek.ToString() == generalSettings.selectedSecondWeekendDay;
        }

        private int CountWeekendsUntilToday(int year, int month, List<string> selectedWeekendDays)
        {
            int count = 0;
            DateTime currentDate = DateTime.Now.Date;

            for (int day = 1; day <= currentDate.Day; day++)
            {
                DateTime date = new DateTime(year, month, day);
                if (selectedWeekendDays.Contains(date.DayOfWeek.ToString()))
                {
                    count++;
                }
            }

            return count;
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

            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var holidays = _context.Holidays
                .Where(h => h.Date >= firstDayOfMonth && h.Date <= lastDayOfMonth)
                .ToList();


            var employeesForMonthYear = employees.Where(e => _context.EmployeeAttendances.Any(a => a.EmployeeId == e.Id && a.Attendence.Month == month && a.Attendence.Year == year)).ToList();

            if (employeesForMonthYear.Count == 0)
            {
                return NotFound("No employees found for the selected month and year.");
            }

            foreach (var employee in employeesForMonthYear)
            {
                if (employee.IsResigned ?? false)
                {
                    continue;
                }

                var firstWeekDay = settings != null ? settings.selectedFirstWeekendDay : null;
                var secondWeekDay = settings != null ? settings.selectedSecondWeekendDay : null;

                int firstWeekDaysCount = !string.IsNullOrEmpty(firstWeekDay) ? CountWeekdaysInMonth(year, month, firstWeekDay) : 0;
                int secondWeekDaysCount = !string.IsNullOrEmpty(secondWeekDay) ? CountWeekdaysInMonth(year, month, secondWeekDay) : 0;

                int daysInCurrentMonth = DateTime.DaysInMonth(year, month);

                double DayPrice = employee.salary.NetSalary / 22;

                DateTime leaveTime = DateTime.Parse(employee.LeaveTime);
                DateTime attendanceTime = DateTime.Parse(employee.AttendanceTime);
                double timeDifferenceInHours = (leaveTime - attendanceTime).TotalHours;

                double HourPrice = timeDifferenceInHours > 0 ? DayPrice / timeDifferenceInHours : 0;

                int HolidaysCount = holidays?.Count() ?? 0;



                var selectedWeekendDays = new List<string> { generalSettingDTO.selectedFirstWeekendDay, generalSettingDTO.selectedSecondWeekendDay };
                int weekendsInMonth = CountWeekendsInMonth(year, month, selectedWeekendDays);

                var selectedVacationDaysAsStrings = generalSettingDTO.SelectedVacationDays?.Select(day => ((DayOfWeek)day).ToString()).ToList() ?? new List<string>();

                int totalOfficialDaysInThisMonth = daysInCurrentMonth - (firstWeekDaysCount + secondWeekDaysCount + HolidaysCount);

                var attendances = _context.EmployeeAttendances
                    .Where(z => z.EmployeeId == employee.Id && z.Attendence.Month == month && z.Attendence.Year == year)
                    .ToList();


                int absenceDays = totalOfficialDaysInThisMonth - attendances?.Count() ?? 0;

                var holidaysAndWeekends = holidays.Select(h => h.Date.DayOfWeek.ToString())
                    .Concat(selectedWeekendDays)
                    .Distinct();

                double extraHours = 0;
                double lossHours = 0;

                foreach (var attendance in attendances)
                {
                    if (attendance.Departure != null)
                    {
                        TimeSpan timeDifference = new TimeSpan();
                        double attendanceTimeEmp = 0.0;

                        timeDifference = attendanceTime.TimeOfDay - attendance.Attendence.TimeOfDay;

                        attendanceTimeEmp = timeDifference.TotalHours;

                        TimeSpan timeDifference2 = new TimeSpan();
                        double departureTimeEmp = 0.0;

                        timeDifference2 = attendance.Departure.Value.TimeOfDay - leaveTime.TimeOfDay;

                        departureTimeEmp = timeDifference2.TotalHours;


                        var resetHours = attendanceTimeEmp + departureTimeEmp;

                        if (attendanceTimeEmp > 0)
                        {
                            extraHours += attendanceTimeEmp;
                        }
                        else
                        {
                            lossHours += attendanceTimeEmp;
                        }

                        if (departureTimeEmp > 0)
                        {
                            extraHours += departureTimeEmp;
                        }
                        else
                        {
                            lossHours += departureTimeEmp;
                        }
                    }
                }

                double extraHoursAdjustment = settings.ExtraHourRate ?? 0;
                double discountHoursAdjustment = settings.DiscountHourRate ?? 0;

                if (settings.Method == "hour")
                {
                    extraHours *= extraHoursAdjustment;
                    lossHours *= discountHoursAdjustment;
                }


                var salary = new SalaryResponseDto
                {
                    id = employee.Id,
                    empName = employee.Name,
                    NetSalary = employee.salary.NetSalary,
                    deptName = employee?.dept?.Name,
                    attendanceDays = attendances?.Where(e=> e.Departure != null).Count() ?? 0,
                    AbsenceDaysToDate = absenceDays,
                    exrtaHours = extraHours,
                    discountHours = lossHours,
                    //extraSalary = extraHours * HourPrice,
                    //discountSalary = lossHours * HourPrice,
                    extraSalary = (double)(settings.Method == "hour" ? (extraHours * HourPrice) : (extraHours * settings.ExtraHourRate)),
                    discountSalary = (double)(settings.Method == "hour" ? (lossHours * HourPrice) : (lossHours * settings.DiscountHourRate)),
                    HourlyRate = HourPrice,
                    DailyRate = DayPrice,
                    WeekendDays = weekendsInMonth,
                    Month = month,
                    Year = year,
                };

                double weekendDaysSalary = weekendsInMonth * salary.DailyRate;

                double totalSalarry = employee.salary.NetSalary + salary.extraSalary + salary.discountSalary ;

                salary.totalSalary = totalSalarry - (DayPrice * (22 - attendances.Where(e => e.Departure != null).Count()));

                filteredSalaries.Add(salary);
            }

            return Ok(filteredSalaries);
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
                Method = settings.Method,    /////////////////////////////////////////// New
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

            DateTime leaveTime = DateTime.Parse(employee.LeaveTime);
            DateTime attendanceTime = DateTime.Parse(employee.AttendanceTime);



            foreach (var attendance in attendances)
            {
                if (attendance.Departure != null)
                {
                    TimeSpan timeDifference = new TimeSpan();
                    double attendanceTimeEmp = 0.0;

                    timeDifference = attendanceTime.TimeOfDay - attendance.Attendence.TimeOfDay;

                    attendanceTimeEmp = timeDifference.TotalHours;

                    TimeSpan timeDifference2 = new TimeSpan();
                    double departureTimeEmp = 0.0;

                    timeDifference2 = attendance.Departure.Value.TimeOfDay - leaveTime.TimeOfDay;

                    departureTimeEmp = timeDifference2.TotalHours;


                    var resetHours = attendanceTimeEmp + departureTimeEmp;

                    double extraHours = 0;
                    double lossHours = 0;

                    if (attendanceTimeEmp > 0)
                    {
                        extraHours += attendanceTimeEmp;
                    }
                    else
                    {
                        lossHours += attendanceTimeEmp;
                    }

                    if (departureTimeEmp > 0)
                    {
                        extraHours += departureTimeEmp;
                    }
                    else
                    {
                        lossHours += departureTimeEmp;
                    }

                    var attendanceDetail = new EmployeeAttendenceDTO
                    {
                        id = attendance.Id,
                        name = employee.Name,
                        department = employee?.dept?.Name,
                        attend = attendance.Attendence.ToString("HH:mm"),
                        leave = attendance.Departure.Value.ToString("HH:mm"),
                        date = attendance.Attendence.Date.ToString("yyyy-MM-dd"),
                        OriginalAttend = employee.AttendanceTime,
                        OriginalLeave = employee.LeaveTime,

                        ExtraHours = extraHours,
                        EarlyDepartureHours = lossHours,
                    };

                    attendanceDetails.Add(attendanceDetail);

                }
            }

            return Ok(attendanceDetails);
        }



    }
}
