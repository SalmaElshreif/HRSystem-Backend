namespace GraduationProject.DTOs
{
    public class UserReq
    {
        public int Id { get; set; }
        public string User_Name{ get; set; }
        public string? Name { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role_Id { get; set; }
        public string? Role_Name { get; set; }

    }
}
