//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ResoReportDataService.Caches;
//using ResoReportDataService.Services;
//using ResoReportDataService.ViewModels;

//namespace ResoReport.Controllers
//{
//    [Route("api/v1/home")]
//    [ApiController]
//    public class HomeController : ControllerBase
//    {
//        private readonly IHomeService _homeService;

//        public HomeController(IHomeService homeService)
//        {
//            _homeService = homeService;
//        }

//        [HttpGet("summary")]
//        [Cache(300000)]
//        public ActionResult<SummaryViewModel> GetSummary([FromQuery] int? storeId = null)
//        {
//            return Ok(_homeService.GetSummary(storeId));
//        }

//        [HttpGet("business-insights")]
//        public ActionResult<BusinessInsightViewModel> GetBusinessInsights([FromQuery] string duration,
//            [FromQuery] int? storeId = null)

//        {
//            return Ok(_homeService.GetBusinessInsights(duration, storeId));
//        }
//    }
//}