using Microsoft.AspNetCore.Mvc;
using ResoReportDataService.Commons;
using ResoReportDataService.RequestModels;
using ResoReportDataService.Services;
using ResoReportDataService.ViewModels;

namespace ResoReport.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/payment-report")]
    [ApiController]
    public class PaymentReportController : ControllerBase
    {
        private readonly IPaymentReportService _service;

        public PaymentReportController(IPaymentReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<BaseResponsePagingViewModel<PaymentReportViewModel>> GetPaymentReport(
            [FromQuery] PaymentReportViewModel modelFilter,
            [FromQuery] DateFilter filter,
            [FromQuery] PagingModel paging,
            [FromQuery] int? storeId = null)
        {
            return _service.GetPaymentReports(modelFilter, filter, paging, storeId);
        }
        
        [HttpGet("store/{storeId}")]
        public ActionResult<BaseResponsePagingViewModel<PaymentReportOneStoreViewModel>> GetPaymentReportOneStore(
            [FromQuery] PaymentReportOneStoreViewModel modelFilter,
            [FromQuery] DateFilter filter,
            [FromQuery] PagingModel paging,
            [FromRoute] int? storeId = null)
        {
            return _service.GetPaymentReportsOneStore(modelFilter, filter, paging, storeId);
        }

        [HttpGet("export")]
        public ActionResult ExportExcelPaymentReport(
            [FromQuery] DateFilter filter,
            [FromQuery] int? storeId = null)
        {
            return _service.ExportPaymentReport(filter, storeId);
        }
    }
}