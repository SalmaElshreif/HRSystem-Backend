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

        [HttpGet]
        public ActionResult<GeneralSettings> GetWeekendDays()
        {


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
