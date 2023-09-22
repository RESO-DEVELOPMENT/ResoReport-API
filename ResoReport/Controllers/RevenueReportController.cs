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
//    [ApiVersion("1.0")]
//    [Route("api/v{version:apiVersion}/revenue-report")]
//    [ApiController]
//    public class RevenueReportController : ControllerBase
//    {
//        private readonly IRevenueReportService _reportService;

//        public RevenueReportController(IRevenueReportService reportService)
//        {
//            _reportService = reportService;
//        }

//        [MapToApiVersion("1.0")]
//        [HttpGet("overview")]
//        public OverviewDashboard GetOverviewDashboard([FromQuery] DateFilter filter, [FromQuery] int brandId = 1,
//            [FromQuery] int storeId = 0)
//        {
//            return _reportService.GetOverviewDashboard(filter, brandId, storeId);
//        }
        
//        [MapToApiVersion("1.0")]
//        [HttpGet("overview-payment")]
//        public OverviewPaymentDashboard GetOverviewPaymentDashboard([FromQuery] DateFilter filter, [FromQuery] int brandId = 1,
//            [FromQuery] int storeId = 0)
//        {
//            return _reportService.GetOverviewPaymentDashboard(filter, brandId, storeId);
//        }
//    }
//}