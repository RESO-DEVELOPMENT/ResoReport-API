using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoReportDataService.Commons.ExcelUtils;
using ResoReportDataService.RequestModels;
using ResoReportDataService.Services;
using ResoReportDataService.ViewModels;

namespace ResoReport.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/export")]
    [ApiController]
    public class ExportReportController : ControllerBase
    {
        private readonly IReportService _reportService;


        public ExportReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [MapToApiVersion("1.0")]
        [HttpGet("systemreport")]
        public ActionResult GetStoreReport([FromQuery] DateFilter filter)
        {
            var result = _reportService.GetStoreReport(filter);
            return _reportService.ExportStoreReportToExcel(filter, result);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("paymentreport")]
        public ActionResult GetPaymentReportDetail([FromQuery] DateFilter filter)
        {
            var result = _reportService.GetPaymentReport(filter);
            return _reportService.ExportPaymentReportToExcel(filter, result);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("productreport")]
        public ActionResult GetProductReport([FromQuery] DateFilter filter)
        {
            var result = _reportService.GetProductReport(filter);
            return _reportService.ExportStoreReportToExcel(filter, result);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("kybill")]
        public ActionResult GetPromotionReport([FromQuery] DateFilter filter)
        {
            var result = _reportService.GetPromotionReport(filter);
            return _reportService.ExportPromotionReportToExel(filter, result);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("test-export")]
        public ActionResult TestExport([FromQuery] DateFilter filter)
        {
            var result = _reportService.GetPromotionReport(filter);
            return ExcelUtils.ExportExcel(new ExcelModel<PromotionReportViewModel>()
            {
                SheetTitle = "Test",
                ColumnConfigs = new List<ColumnConfig<PromotionReportViewModel>>()
                {
                    new ColumnConfig<PromotionReportViewModel>()
                    {
                        Title = "Customer Name",
                        DataIndex = "CustomerName",
                        ValueType = "string"
                    },
                    new ColumnConfig<PromotionReportViewModel>()
                    {
                        Title = "Store Name",
                        DataIndex = "StoreName",
                        ValueType = "string"
                    },
                    new ColumnConfig<PromotionReportViewModel>()
                    {
                        Title = "Sum Amount",
                        DataIndex = "SumAmount",
                        ValueType = "currency"
                    },
                  
                },
                DataSources = result
            });
        }

        [MapToApiVersion("1.0")]
        [HttpGet("orderDetailReport/storeId")]
        public ActionResult<List<OrderDetailReportViewModel>> GetOrderDetailReportForStore([FromQuery] Guid storeId, [FromQuery] DateFilter filter)
        {
            var result =  _reportService.ExportOrderDetailReportToExcel(storeId , filter);
            return result;
        }

    }
}