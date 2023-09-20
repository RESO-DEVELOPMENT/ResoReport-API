//using System.Collections.Generic;
//using Microsoft.AspNetCore.Mvc;
//using ResoReportDataService.Services;
//using ResoReportDataService.ViewModels;

//namespace ResoReport.Controllers
//{
//    [ApiVersion("1.0")]
//    [Route("api/v{version:apiVersion}/stores")]
//    [ApiController]
//    public class StoresController : ControllerBase
//    {

//        private readonly IStoreService _service;

//        public StoresController(IStoreService service)
//        {
//            _service = service;
//        }

//        [MapToApiVersion("1.0")]
//        [HttpGet]
//        public ActionResult<List<StoreViewModel>> GetListStore()
//        {
//            return _service.GetListStore();
//        }
        
//        // [MapToApiVersion("1.0")]
//        // [HttpGet("store-report-day")]
//        // public ActionResult<List<StoreViewModel>> GetListStoreByReportDay()
//        // {
//        //     return _service.GetListStoreByReportDay();
//        // }
//    }
//}