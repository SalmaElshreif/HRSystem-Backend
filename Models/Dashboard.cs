namespace GraduationProject.Models
{
    public class Dashboard
    {
        public int NumberOfEmployees { get; set; }
        public int NumberOfDepartments { get; set; }
        public double? AverageSalary { get; set; }
        public Dictionary<string, int> GenderPercentage { get; set; }
        public Dictionary<DateTime, int> DailyAttendanceRatio { get; set; }
        public Dictionary<DateTime, int> DailyLateAttendanceCount { get; set; }
    }
}
