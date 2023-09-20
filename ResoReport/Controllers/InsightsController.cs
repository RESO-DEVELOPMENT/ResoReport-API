//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ResoReportDataService.RequestModels;
//using ResoReportDataService.Services;
//using ResoReportDataService.ViewModels;

//namespace ResoReport.Controllers
//{
//    [Route("api/v1/insights")]
//    [ApiController]
//    public class InsightsController : ControllerBase
//    {
//        private readonly IInsightService _insightService;

//        public InsightsController(IInsightService insightService)
//        {
//            _insightService = insightService;
//        }

//        [HttpGet]
//        public ActionResult<SalesInsightViewModel> GetSalesInsights([FromQuery] DateFilter dateFilter,
//            [FromQuery] string duration, [FromQuery] int? storeId = null)
//        {
//            return Ok(_insightService.GetSalesInsights(dateFilter, duration, storeId));
//        }
//    }
//}