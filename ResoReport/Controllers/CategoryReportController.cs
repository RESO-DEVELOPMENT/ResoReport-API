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
            [FromQuery] PagingModel paging, [FromQuery] int brandId = 1,
            [FromQuery] string checkDeal = "beforeDeal", [FromQuery] int storeId = 0)
        {
            if (storeId == 0)
            {
                return _categoryReportService.GetCategoryReportAllStore(filter, paging, brandId, checkDeal);
            }

            return _categoryReportService.GetCategoryReportOneStore(filter, paging, brandId, checkDeal, storeId);
            ;
        }
    }
}