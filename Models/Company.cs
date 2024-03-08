﻿using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public virtual List<Department>? Dpartments { get; set; }
        public virtual List<Holiday>? Holidays { get; set; }

        [ForeignKey("GeneralSettings")]
        public int? GeneralSettingsId { get; set; }
        public virtual GeneralSettings? GeneralSettings { get; set; }
    }
}
