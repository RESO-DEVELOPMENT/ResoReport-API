using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoReportDataService.Commons;
using ResoReportDataService.Commons.ExcelUtils;
using ResoReportDataService.RequestModels;
using ResoReportDataService.Services;
using ResoReportDataService.ViewModels;

namespace ResoReport.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/product-report")]
    [ApiController]
    public class ProductReportController : ControllerBase
    {
        private readonly IProductReportService _service;

        public ProductReportController(IProductReportService service)
        {
            _service = service;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public ActionResult<BaseResponsePagingViewModel<ProductReportViewModel>> GetProductReports(
            [FromQuery] ProductReportViewModel modelFilter,
            [FromQuery] DateFilter dateFilter, [FromQuery] PagingModel paging,
            [FromQuery] Guid? storeId)
        {
            return _service.GetStoreProductProgress(modelFilter, dateFilter, paging, storeId);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("export")]
        public ActionResult ProductExport([FromQuery] DateFilter filter, Guid? storeId)
        {
            var result = _service.ExportProductReport(filter, storeId);
            return result;
        }
    }
}