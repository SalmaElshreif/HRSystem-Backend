namespace GraduationProject.DTOs
{
    public class SaveEmpRequest
    {
        public int? id { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime attendance { get; set; }
        public DateTime? departure { get; set; }
    }


    public class EditEmpRequest
    {
        public int? id { get; set; }
        public DateTime attendance { get; set; }
        public DateTime departure { get; set; }
    }
}
