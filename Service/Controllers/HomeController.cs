using CCG.AspNetCore.Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queries;
using System.Threading.Tasks;

namespace SapService.Controllers
{
    [Route("api/home")]
    public class HomeController : Controller
    {
        private readonly IQueryProcessor _queryProcessor;
        public HomeController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> GetProfile()
        {
            var results = await _queryProcessor.ProcessAsync(new GetCostCentresQuery());
            return Ok(results);
        }

    }
}
