using Microsoft.AspNetCore.Mvc;
using Football.Models;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;
        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("settings/season")]
        [ProducesResponseType(typeof(Season), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public Season GetSeason() => _settingsService.GetSeason();

        [HttpGet("settings/replacements")]
        [ProducesResponseType(typeof(ReplacementLevels), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ReplacementLevels GetReplacementLevels() => _settingsService.GetReplacementLevels();

        [HttpGet("settings/scoring")]
        [ProducesResponseType(typeof(FantasyScoring), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public FantasyScoring GetFantasyScoring() => _settingsService.GetFantasyScoring();

        [HttpGet("settings/limits")]
        [ProducesResponseType(typeof(ProjectionLimits), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ProjectionLimits GetProjectionLimits() => _settingsService.GetProjectionLimits();

        [HttpGet("settings/starters")]
        [ProducesResponseType(typeof(Starters), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public Starters GetStarters() => _settingsService.GetStarters();

        [HttpGet("settings/tunings")]
        [ProducesResponseType(typeof(Tunings), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public Tunings GetTunings() => _settingsService.GetTunings();

        [HttpGet("settings/weeklytunings")]
        [ProducesResponseType(typeof(WeeklyTunings), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public WeeklyTunings GetWeeklyTunings() => _settingsService.GetWeeklyTunings();
    }
}
