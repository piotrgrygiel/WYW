#if TEST || DEBUG

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WYW.Data;

namespace WYW.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("tests")]
    [ApiController]
    public class TestsController : Controller
    {
        private readonly IApiResponseService apiResponseService;
        private readonly IDateTime dateTimeService;

        public TestsController(IApiResponseService apiResponseService, IDateTime dateTimeService)
        {
            this.apiResponseService = apiResponseService;
            this.dateTimeService = dateTimeService;
        }

        [HttpPost("currentResponse")]
        public async Task<IActionResult> ChangeCurrentApiResponse(FlightInfo[] currentResponse)
        {
            apiResponseService.RecentResponse = new RecentResponse() { LastResponse = currentResponse, LastResponseDT = DateTime.Now };
            return StatusCode(200);
        }

        [HttpGet("resetTime")]
        public async Task<IActionResult> ResetTime()
        {
            (dateTimeService as MockDateTime).ResetTime(new DateTime(2021, 6, 13, 9, 0, 0)); //13-06-2021 godz 9:00
            return StatusCode(200);
        }
    }
}

#endif