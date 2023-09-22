//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ResoReportDataService.Commons;
//using ResoReportDataService.Models;
//using ResoReportDataService.RequestModels;
//using ResoReportDataService.Services;
//using ResoReportDataService.ViewModels;

//namespace ResoReport.Controllers
//{
//    [ApiVersion("1.0")]
//    [Route("api/v{version:apiVersion}/system-report")]
//    [ApiController]
//    public class SystemReportController : ControllerBase
//    {
//        private readonly ISystemReportService _service;

//        public SystemReportController(ISystemReportService service)
//        {
//            _service = service;
//        }

//        [MapToApiVersion("1.0")]
//        [HttpGet]
//        public ActionResult<BaseResponsePagingViewModel<StoreReportViewModel>> GetStoreReports([FromQuery] int? storeId,
//            [FromQuery] DateFilter filter, [FromQuery] PagingModel paging, [FromQuery] StoreReportViewModel modelFilter)
//        {
//            return _service.GetStoreReports(storeId, filter, paging, modelFilter);
//        }

//        [MapToApiVersion("1.0")]
//        [HttpGet("export")]
//        public ActionResult GetStoreReport([FromQuery] int? storeId, [FromQuery] DateFilter filter)
//        {
//            return _service.ExportStoreReport(storeId, filter);
//        }
//    }
//}