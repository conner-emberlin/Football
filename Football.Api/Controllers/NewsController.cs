using News.Models;
using News.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using News.Interfaces;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPost("espn-headlines")]
        [ProducesResponseType(typeof(Root), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Root>> GetEspnNews()
        {
            return Ok(await _newsService.GetEspnNews());
        }
    }
}