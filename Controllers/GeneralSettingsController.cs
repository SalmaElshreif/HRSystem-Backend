using GraduationProject.DTOs;
using GraduationProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralSettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GeneralSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public ActionResult GetGeneralSettings()
        //{
        //    var settings = _context.generalSettings.FirstOrDefault();
        //    if (settings == null)
        //    {
        //        return Ok(new GeneralSettings() { method = "", selectedFirstWeekendDay = "", selectedSecondWeekendDay = "" });
        //    }
        //    else
        //    {
        //        return Ok(settings);
        //    }
        //}

        //[HttpPost]
        //public ActionResult SaveSettings(GeneralSettings generalSettings)
        //{
        //    var setting = _context.generalSettings.FirstOrDefault();
        //    if (setting == null)
        //    {
        //        GeneralSettings general = new GeneralSettings()
        //        {
        //            method = generalSettings.method,
        //            selectedFirstWeekendDay = generalSettings.selectedFirstWeekendDay,
        //            selectedSecondWeekendDay = generalSettings.selectedSecondWeekendDay,
        //            ExtraHourRate = generalSettings.ExtraHourRate,  
        //            DiscountHourRate = generalSettings.DiscountHourRate  
        //        };
        //        _context.generalSettings.Add(general);
        //        _context.SaveChanges();
        //        return Ok();
        //    }
        //    else
        //    {
        //        setting.method = generalSettings.method;
        //        setting.selectedFirstWeekendDay = generalSettings.selectedFirstWeekendDay;
        //        setting.selectedSecondWeekendDay= generalSettings.selectedSecondWeekendDay;
        //        setting.ExtraHourRate = generalSettings.ExtraHourRate; 
        //        setting.DiscountHourRate = generalSettings.DiscountHourRate;
        //        _context.Entry(setting).State = EntityState.Modified;
        //        _context.SaveChanges(); 
        //        return Ok();
        //    }
        //}


        //[HttpPost]
        //public ActionResult SaveSettings(GeneralSettingDTO generalSettingDTO)
        //{
        //    GeneralSettings general = new GeneralSettings()
        //    {
        //        ExtraHourRate = generalSettingDTO.ExtraHourRate,
        //        DiscountHourRate = generalSettingDTO.DiscountHourRate,
        //        selectedFirstWeekendDay = generalSettingDTO.selectedFirstWeekendDay,
        //        selectedSecondWeekendDay = generalSettingDTO.selectedSecondWeekendDay
        //    };

        //    _context.generalSettings.Add(general);
        //    _context.SaveChanges();

        //    return Ok();
        //}


        //[HttpGet]
        //public ActionResult GetGeneralSettingsList()
        //{
        //    List<GeneralSettings> settingsList = _context.generalSettings.ToList();
        //    if (!settingsList.Any()) // Check if list is empty
        //    {
        //        return NotFound(); // Return not found status if no settings exist
        //    }
        //    return Ok(settingsList);
        //}


        ////////////////////////////////////
        ///
        [HttpGet]
        public ActionResult<GeneralSettings> GetWeekendDays()
        {


            // For simplicity, assuming there is only one record in the database
            GeneralSettings generalSettings = _context.generalSettings.OrderByDescending(x => x.id) .FirstOrDefault();

            if (generalSettings == null)
            {
                generalSettings = new GeneralSettings
                {
                    selectedFirstWeekendDay = "Saturday",
                    selectedSecondWeekendDay = "Sunday",
                    ExtraHourRate = 0,
                    DiscountHourRate = 0
                };
            }

            return Ok(generalSettings);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateWeekendDays(GeneralSettings generalSettings)
        {
            try
            {
                await _context.generalSettings.AddAsync(generalSettings);
                await _context.SaveChangesAsync();

                return Ok();
            }catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
