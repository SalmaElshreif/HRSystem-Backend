namespace GraduationProject.DTOs
{
    public class GeneralSettingDTO
    {
        public int? ExtraHourRate { get; set; }

        public int? DiscountHourRate { get; set; }

        public string selectedFirstWeekendDay { get; set; }

        public string selectedSecondWeekendDay { get; set; }

        public List<int>? SelectedVacationDays { get; set; }

        public string? Method { get; set; }
    }
}
