//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ResoReportDataService.Commons;
//using ResoReportDataService.RequestModels;
//using ResoReportDataService.Services;
//using ResoReportDataService.ViewModels;
//using static ResoReportDataAccess.Models.DateReport;

//namespace ResoReport.Controllers
//{
//    [Route("api/v1/overview-dashboard")]
//    [ApiController]
//    public class OverviewController : ControllerBase
//    {
//        private readonly IOverviewService _overviewService;

//        public OverviewController(IOverviewService overviewService)
//        {
//            _overviewService = overviewService;
//        }


//        [HttpGet("top-store-revenue")]
//        public ActionResult<BaseResponsePagingViewModel<TopStoreRevenueViewModel>> GetTopStoreRevenueWithPaging([FromQuery] int? storeId, [FromQuery] int brandId, [FromQuery] DateRange dateRange, [FromQuery] PagingModel paging, [FromQuery] TopStoreRevenueViewModel modelFilter)
//        {
//            return _overviewService.GetTopStoreRevenueWithPaging(storeId, brandId, dateRange, paging, modelFilter);
//        }

//        [HttpGet("top-store-revenue/export")]
//        public ActionResult ExportTopStoreRevenue([FromQuery] DateRange dateRange, [FromQuery] int? storeId)
//        {
//            return _overviewService.ExportTopStoreRevenue(dateRange, storeId);
//        }

//        [HttpGet("revenue")]
//        public ActionResult<OverviewDashboard> GetOverviewDashboard([FromQuery] DateFilter filter,
//            [FromQuery] int brandId, [FromQuery] int? storeId = null)
//        {
//            return _overviewService.GetOverviewDashboard(filter, brandId, storeId);
//        }

//        [HttpGet("payment")]
//        public ActionResult<OverviewPaymentDashboard> GetOverviewDashboardPayment([FromQuery] DateFilter filter,
//            [FromQuery] int brandId, [FromQuery] int? storeId = null)
//        {
//            return _overviewService.GetOverviewPaymentDashboard(filter, brandId, storeId);
//        }
//    }
//}