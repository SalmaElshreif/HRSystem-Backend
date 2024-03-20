using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models
{
    public class GeneralSettings
    {
        [Key]
        public int id { get; set; }
        public string? selectedFirstWeekendDay { get; set; }
        public string? selectedSecondWeekendDay { get; set; }

        public int? ExtraHourRate { get; set; }
        public int? DiscountHourRate { get; set; }
        public string? Method { get; set; }


    }
}
