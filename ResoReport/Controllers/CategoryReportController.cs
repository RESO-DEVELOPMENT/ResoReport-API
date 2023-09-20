using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoReportDataService.Commons;
using ResoReportDataService.RequestModels;
using ResoReportDataService.Services;
using ResoReportDataService.ViewModels;

namespace ResoReport.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/category-report")]
    [ApiController]
    public class CategoryReportController : ControllerBase
    {
        private readonly ICategoryReportService _categoryReportService;

        public CategoryReportController(ICategoryReportService categoryReportService)
        {
            _categoryReportService = categoryReportService;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public ActionResult<BaseResponsePagingViewModel<CategoryReportViewModel>> GetCategoryReport(
            [FromQuery] DateFilter filter,
            [FromQuery] PagingModel paging, [FromQuery] Guid? brandId = null,
            [FromQuery] string checkDeal = "beforeDeal", [FromQuery] Guid? storeId = null)
        {
            if (storeId == null)
            {
                return _categoryReportService.GetCategoryReportAllStore(filter, paging, brandId, checkDeal);
            }

            return _categoryReportService.GetCategoryReportOneStore(filter, paging, brandId, checkDeal, storeId);
            ;
        }
    }
}