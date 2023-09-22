//using System;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Reso.Sdk.Core.Custom;
//using Reso.Sdk.Core.Utilities;
//using ResoReportDataService.Commons;
//using ResoReportDataService.Models;
//using ResoReportDataService.RequestModels;
//using ResoReportDataService.ViewModels;

//namespace ResoReportDataService.Services
//{
//    public interface ICategoryReportService
//    {
//        BaseResponsePagingViewModel<CategoryReportViewModel> GetCategoryReportAllStore(DateFilter filter,
//            PagingModel paging, int brandId, string checkDeal);

//        BaseResponsePagingViewModel<CategoryReportViewModel> GetCategoryReportOneStore(DateFilter filter,
//            PagingModel paging, int brandId, string checkDeal, int storeId);
//    }

//    public class CategoryReportService : ICategoryReportService
//    {
//        private readonly PosSystemContext _context;

//        public CategoryReportService(PosSystemContext context)
//        {
//            _context = context;
//        }

//        public BaseResponsePagingViewModel<CategoryReportViewModel> GetCategoryReportAllStore(DateFilter filter,
//            PagingModel paging, int brandId, string checkDeal)
//        {
//            #region Check Date range

//            var from = filter?.FromDate;
//            var to = filter?.ToDate;
//            if (from == null && to == null)
//            {
//                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
//                to = Utils.GetCurrentDate().AddDays(-1);
//            }

//            if (from == null)
//            {
//                from = Utils.GetCurrentDate().AddDays(-1);
//            }

//            if (to == null)
//            {
//                to = Utils.GetCurrentDate().AddDays(-1);
//            }

//            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
//            {
//                throw new ErrorResponse(400, "The datetime is invalid!");
//            }

//            if (DateTime.Compare((DateTime)to, Utils.GetCurrentDate()) >= 0)
//            {
//                throw new ErrorResponse(400, "The datetime must be earlier than today!");
//            }

//            #endregion

//            var listProductIDs = _context.Products
//                .Include(x => x.Category)
//                .Where(x => x.IsAvailable == true && x.Cat.BrandId == 1)
//                .Select(x => x.ProductId);

//            from = ((DateTime)from).GetStartOfDate();
//            to = ((DateTime)to).GetEndOfDate();

//            var resultProductReport = _context.DateProducts
//                .Include(x => x.Product)
//                .ThenInclude(x => x.Cat)
//                .Where(x => listProductIDs.Contains(x.ProductId) &&
//                            x.Product.IsAvailable &&
//                            DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                            DateTime.Compare(x.Date, (DateTime)to) <= 0)
//                .GroupBy(x => new
//                {
//                    CategoryId = x.Product.CatId,
//                    CategoryName = x.Product.Cat.CateName
//                })
//                .Select(x => new CategoryReportViewModel()
//                {
//                    CategoryId = x.Key.CategoryId,
//                    CategoryName = x.Key.CategoryName,
//                    Percent = 0,
//                    Quantity = x.Sum(orderDetail => orderDetail.Quantity),
//                    TotalBeforeDiscount = x.Sum(orderDetail => orderDetail.TotalAmount),
//                    Discount = x.Sum(orderDetail => orderDetail.Discount),
//                    TotalAfterDiscount = x.Sum(orderDetail => orderDetail.FinalAmount),
//                }).ToList();

//            double totalProduct = 0;
//            if (checkDeal == "beforeDeal")
//            {
//                totalProduct = resultProductReport.Sum(x => x.TotalBeforeDiscount);
//                if (totalProduct == 0)
//                {
//                    totalProduct = 1;
//                }

//                resultProductReport.ForEach(
//                    x => x.Percent = Math.Round((x.TotalBeforeDiscount / totalProduct) * 100, 2));
//            }
//            else if (checkDeal == "afterDeal")
//            {
//                totalProduct = resultProductReport.Sum(x => x.TotalAfterDiscount);
//                if (totalProduct == 0)
//                {
//                    totalProduct = 1;
//                }

//                resultProductReport.ForEach(
//                    x => x.Percent = Math.Round((x.TotalAfterDiscount / totalProduct) * 100, 2));
//            }

//            var result = resultProductReport
//                .OrderByDescending(x => x.Percent).AsQueryable()
//                .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

//            return new BaseResponsePagingViewModel<CategoryReportViewModel>()
//            {
//                Metadata = new PagingMetadata()
//                {
//                    Page = paging.Page,
//                    Size = paging.Size,
//                    Total = result.Item1
//                },
//                Data = result.Item2.ToList()
//            };
//        }

//        public BaseResponsePagingViewModel<CategoryReportViewModel> GetCategoryReportOneStore(DateFilter filter,
//            PagingModel paging, int brandId, string checkDeal, int storeId)
//        {
//            #region Check Date range

//            var from = filter?.FromDate;
//            var to = filter?.ToDate;
//            if (from == null && to == null)
//            {
//                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
//                to = Utils.GetCurrentDate().AddDays(-1);
//            }

//            if (from == null)
//            {
//                from = Utils.GetCurrentDate().AddDays(-1);
//            }

//            if (to == null)
//            {
//                to = Utils.GetCurrentDate().AddDays(-1);
//            }

//            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
//            {
//                throw new ErrorResponse(400, "The datetime is invalid!");
//            }

//            if (DateTime.Compare((DateTime)to, Utils.GetCurrentDate()) >= 0)
//            {
//                throw new ErrorResponse(400, "The datetime must be earlier than today!");
//            }

//            #endregion

//            var listProductIDs = _context.Products
//                .Include(x => x.Cat)
//                .Where(x => x.IsAvailable == true && x.Cat.BrandId == 1)
//                .Select(x => x.ProductId);

//            from = ((DateTime)from).GetStartOfDate();
//            to = ((DateTime)to).GetEndOfDate();

//            var resultProductReport = _context.DateProducts
//                .Include(x => x.Product)
//                .ThenInclude(x => x.Cat)
//                .Where(x => listProductIDs.Contains(x.ProductId) &&
//                            x.StoreId == storeId &&
//                            x.Product.IsAvailable &&
//                            DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                            DateTime.Compare(x.Date, (DateTime)to) <= 0)
//                .GroupBy(x => new
//                {
//                    CategoryId = x.Product.CatId,
//                    CategoryName = x.Product.Cat.CateName
//                })
//                .Select(x => new CategoryReportViewModel()
//                {
//                    CategoryId = x.Key.CategoryId,
//                    CategoryName = x.Key.CategoryName,
//                    Percent = 0,
//                    Quantity = x.Sum(orderDetail => orderDetail.Quantity),
//                    TotalBeforeDiscount = x.Sum(orderDetail => orderDetail.TotalAmount),
//                    Discount = x.Sum(orderDetail => orderDetail.Discount),
//                    TotalAfterDiscount = x.Sum(orderDetail => orderDetail.FinalAmount),
//                }).ToList();

//            double totalProduct = 0;
//            if (checkDeal == "beforeDeal")
//            {
//                totalProduct = resultProductReport.Sum(x => x.TotalBeforeDiscount);
//                if (totalProduct == 0)
//                {
//                    totalProduct = 1;
//                }

//                resultProductReport.ForEach(
//                    x => x.Percent = Math.Round((x.TotalBeforeDiscount / totalProduct) * 100, 2));
//            }
//            else if (checkDeal == "afterDeal")
//            {
//                totalProduct = resultProductReport.Sum(x => x.TotalAfterDiscount);
//                if (totalProduct == 0)
//                {
//                    totalProduct = 1;
//                }

//                resultProductReport.ForEach(
//                    x => x.Percent = Math.Round((x.TotalAfterDiscount / totalProduct) * 100, 2));
//            }

//            var result = resultProductReport
//                .OrderByDescending(x => x.Percent).AsQueryable()
//                .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

//            return new BaseResponsePagingViewModel<CategoryReportViewModel>()
//            {
//                Metadata = new PagingMetadata()
//                {
//                    Page = paging.Page,
//                    Size = paging.Size,
//                    Total = result.Item1
//                },
//                Data = result.Item2.ToList()
//            };
//        }
//    }
//}