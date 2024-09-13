using GraduationProject.DTOs;
using GraduationProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public DashboardController(ApplicationDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }
        [HttpGet]
        public Dashboard GetDashboardData()
        {
            var numberOfEmployees = dbContext.Employees.Count();
            var numberOfDepartments = dbContext.Departments.Count();
            var averageSalary = dbContext.Salaries.Average(s => s.NetSalary);
            var genderPercentage = CalculateGenderPercentage();
            var attendanceRatio = CalculateDailyAttendanceRatio();
            var attendanceLateCount = CalculateDailyLateAttendanceRatio();


            var dashboard = new Dashboard
            {
                NumberOfEmployees = numberOfEmployees,
                NumberOfDepartments = numberOfDepartments,
                AverageSalary = averageSalary,
                GenderPercentage = genderPercentage,
                //MaleFemaleRatio = maleFemaleRatio,
                DailyAttendanceRatio = attendanceRatio,
                DailyLateAttendanceCount = attendanceLateCount
            };

            return dashboard;
        }
        private Dictionary<string, int> CalculateGenderPercentage()
        {
            var maleCount = dbContext.Employees.Count(e => e.gender.Name == "Male");
            var femaleCount = dbContext.Employees.Count(e => e.gender.Name == "Female");

            var totalEmployees = maleCount + femaleCount;

            var malePercentage = totalEmployees > 0 ? (int)((double)maleCount / totalEmployees * 100) : 0;
            var femalePercentage = totalEmployees > 0 ? (int)((double)femaleCount / totalEmployees * 100) : 0;

            var genderPercentage = new Dictionary<string, int>
    {
        { "Male", malePercentage },
        { "Female", femalePercentage }
    };

            return genderPercentage;
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private Dictionary<DateTime, int> CalculateDailyAttendanceRatio()
        {
            var attendanceRatioByDate = new Dictionary<DateTime, int>();
            var startOfWeek = GetStartOfWeek(DateTime.Today);
            var endOfWeek = startOfWeek.AddDays(6);

            var distinctDates = dbContext.EmployeeAttendances
                .Where(a => a.Attendence.Date >= startOfWeek && a.Attendence.Date <= endOfWeek)
                .Select(a => a.Attendence.Date)
                .Distinct()
                .ToList();

            foreach (var date in distinctDates)
            {
                var totalEmployees = dbContext.Employees.Count();
                var totalAttendance = dbContext.EmployeeAttendances
                    .Count(a => a.Attendence.Date == date);
                var attendanceRatio = totalEmployees > 0
                    ? (int)Math.Round((double)totalAttendance / totalEmployees * 100)
                    : 0;
                attendanceRatioByDate[date] = attendanceRatio;
            }
            return attendanceRatioByDate;
        }

        private Dictionary<DateTime, int> CalculateDailyLateAttendanceRatio()
        {
            var attendanceRatioByDate = new Dictionary<DateTime, int>();
            var startOfWeek = GetStartOfWeek(DateTime.Today);
            var endOfWeek = startOfWeek.AddDays(6);

            var distinctDates = dbContext.EmployeeAttendances
                .Where(a => a.Attendence.Date >= startOfWeek && a.Attendence.Date <= endOfWeek)
                .Select(a => a.Attendence.Date)
                .Distinct()
                .ToList();

            foreach (var date in distinctDates)
            {
                var totalLateEmployees = dbContext.EmployeeAttendances.AsEnumerable()
                .Count(a => a.Attendence.Date == date
                     && a.Attendence.TimeOfDay > TimeSpan.Parse(a.Employee != null ? a.Employee.AttendanceTime : "00:00:00"));
                attendanceRatioByDate[date] = totalLateEmployees;
            }
            return attendanceRatioByDate;
        }
    }
}

