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

//namespace ResoReport.Controllers
//{

//    [ApiVersion("1.0")]
//    [Route("api/v{version:apiVersion}/promotion-report")]
//    [ApiController]
//    public class PromotionReportController : ControllerBase
//    {
//        private readonly IPromotionReportService _promotionReportService;

//        public PromotionReportController(IPromotionReportService promotionReportService)
//        {
//            _promotionReportService = promotionReportService;
//        }

//        [MapToApiVersion("1.0")]
//        [HttpGet("{storeId}")]
//        public ActionResult<BaseResponsePagingViewModel<PromotionReportViewModel>> GetPromotionReports(int storeId,
//            [FromQuery] DateFilter filter, [FromQuery] PagingModel paging)
//        {
//            return _promotionReportService.GetPromotionReports(filter, storeId, paging);
//        }

//        // [MapToApiVersion("1.0")]
//        // [HttpGet("export/ky-bill")]
//        // public ActionResult GetPromotionReport([FromQuery] DateFilter filter)
//        // {
//        //     return _promotionReportService.ExportPromotionReportToExel(filter);
//        // }
//    }
//}