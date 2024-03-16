﻿namespace GraduationProject.DTOs
{
    public class EmployeeAttendenceDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public string attend { get; set; }
        public string leave { get; set; }
        public string? date { get; set; }

        public string? OriginalAttend { get; set; }
        public string? OriginalLeave { get; set; }

        public double ExtraHours { get; set; }
        public double EarlyDepartureHours { get; set; }

        //public int Id { get; set; }
        //public string Name { get; set; }
        //public string Department { get; set; }
        //public TimeSpan Attend { get; set; }
        //public TimeSpan Leave { get; set; }
        //public DateTime Date { get; set; }

    }
}
