using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reso.Sdk.Core.Custom;
using Reso.Sdk.Core.Utilities;
using ResoReportDataAccess.Models;
using ResoReportDataService.Commons;
using ResoReportDataService.Commons.ExcelUtils;
using ResoReportDataService.Models;
using ResoReportDataService.RequestModels;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.Services
{
    public interface IProductReportService
    {
        BaseResponsePagingViewModel<ProductReportViewModel> GetStoreProductProgress(
            ProductReportViewModel modelFilter,
            DateFilter dateFilter,
            PagingModel paging, int? storeId);

        List<ProductReportViewModel> GetProductReport(DateFilter filter, int? storeId);

        FileStreamResult ExportProductReport(DateFilter filter, int? storeId);
    }

    public class ProductReportService : IProductReportService
    {
        private readonly DataWareHouseReportingContext _context;
        private readonly ProdPassioContext _prodPassioContext;

        public ProductReportService(DataWareHouseReportingContext context, ProdPassioContext prodPassioContext)
        {
            _context = context;
            _prodPassioContext = prodPassioContext;
        }

        //public BaseResponsePagingViewModel<ProductReportViewModel> GetStoreProductProgress(
        //    ProductReportViewModel modelFilter, DateFilter dateFilter,
        //    PagingModel paging, int? storeId)
        //{
        //    #region Check Date range

        //    var from = dateFilter?.FromDate;
        //    var to = dateFilter?.ToDate;
        //    if (from == null && to == null)
        //    {
        //        from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
        //        to = Utils.GetCurrentDate().AddDays(-1);
        //    }

        //    if (from == null)
        //    {
        //        from = Utils.GetCurrentDate().AddDays(-1);
        //    }

        //    if (to == null)
        //    {
        //        to = Utils.GetCurrentDate().AddDays(-1);
        //    }

        //    if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
        //    {
        //        throw new ErrorResponse(400, "The datetime is invalid!");
        //    }

        //    if (DateTime.Compare((DateTime)to, Utils.GetCurrentDate()) >= 0)
        //    {
        //        throw new ErrorResponse(400, "The datetime must be earlier than today!");
        //    }

        //    #endregion

        //    from = ((DateTime)from).GetStartOfDate();
        //    to = ((DateTime)to).GetEndOfDate();

        //    var resultProductReport = _context.DateProducts
        //        .Where(x =>
        //            x.Active == true &&
        //            DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
        //            DateTime.Compare(x.Date, (DateTime)to) <= 0
        //        );

        //    if (storeId != null)
        //    {
        //        resultProductReport = _context.DateProducts
        //            .Where(x =>
        //                x.Active == true &&
        //                x.StoreId == storeId &&
        //                DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
        //                DateTime.Compare(x.Date, (DateTime)to) <= 0);
        //    }

        //    var resultReport = resultProductReport.GroupBy(x => new
        //        {
        //            ProductId = x.ProductId,
        //            ProductName = x.ProductName,
        //            CateName = x.CategoryName,
        //            ProductCode = x.ProductCode,
        //            UnitPrice = x.UnitPrice
        //        })
        //        .Select(x => new ProductReportViewModel()
        //        {
        //            ProductName = x.Key.ProductName,
        //            ProductId = x.Key.ProductId,
        //            CateName = x.Key.CateName,
        //            Quantity = x.Sum(dateProduct => dateProduct.Quantity),
        //            ProductCode = x.Key.ProductCode,
        //            UnitPriceNoVat = 0,
        //            UnitPrice = x.Key.UnitPrice,
        //            Percent = 0,
        //            TotalBeforeDiscount = x.Sum(dateProduct => dateProduct.TotalAmount),
        //            TotalAfterDiscount = x.Sum(dateProduct => dateProduct.FinalAmount),
        //        }).OrderByDescending(x => x.TotalAfterDiscount).ToList();

        //    var (total, data) = resultReport
        //        .AsQueryable()
        //        .DynamicFilter(modelFilter)
        //        .DynamicSort(modelFilter)
        //        .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

        //    return new BaseResponsePagingViewModel<ProductReportViewModel>()
        //    {
        //        Metadata = new PagingMetadata()
        //        {
        //            Page = paging.Page,
        //            Size = paging.Size,
        //            Total = total
        //        },
        //        Data = data.ToList()
        //    };
        //}

        public BaseResponsePagingViewModel<ProductReportViewModel> GetStoreProductProgress(
           ProductReportViewModel modelFilter, DateFilter dateFilter,
           PagingModel paging, int? storeId)
        {
            #region Check Date range

            var from = dateFilter?.FromDate;
            var to = dateFilter?.ToDate;
            if (from == null && to == null)
            {
                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
                to = Utils.GetCurrentDate().AddDays(-1);
            }

            if (from == null)
            {
                from = Utils.GetCurrentDate().AddDays(-1);
            }

            if (to == null)
            {
                to = Utils.GetCurrentDate().AddDays(-1);
            }

            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
            {
                throw new ErrorResponse(400, "The datetime is invalid!");
            }

            if (DateTime.Compare((DateTime)to, Utils.GetCurrentDate()) >= 0)
            {
                throw new ErrorResponse(400, "The datetime must be earlier than today!");
            }

            #endregion

            from = ((DateTime)from).GetStartOfDate();
            to = ((DateTime)to).GetEndOfDate();

            var resultProductReport = _prodPassioContext.DateProducts
                .Where(x =>
                    //x.Active == true &&
                    DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
                    DateTime.Compare(x.Date, (DateTime)to) <= 0
                );

            if (storeId != null)
            {
                resultProductReport = _prodPassioContext.DateProducts
                    .Where(x =>
                        x.StoreId == storeId &&
                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
                        DateTime.Compare(x.Date, (DateTime)to) <= 0);
            }

            var resultReport = resultProductReport.GroupBy(x => new
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                CateName = x.Product.Cat.CateName,
                ProductCode = x.Product.Code,
                UnitPrice = x.Product.Price
            })
                .Select(x => new ProductReportViewModel()
                {
                    ProductName = x.Key.ProductName,
                    ProductId = x.Key.ProductId,
                    CateName = x.Key.CateName,
                    Quantity = x.Sum(dateProduct => dateProduct.Quantity),
                    ProductCode = x.Key.ProductCode,
                    UnitPriceNoVat = 0,
                    UnitPrice = x.Key.UnitPrice,
                    Percent = 0,
                    TotalBeforeDiscount = x.Sum(dateProduct => dateProduct.TotalAmount),
                    TotalAfterDiscount = x.Sum(dateProduct => dateProduct.FinalAmount),
                }).OrderByDescending(x => x.TotalAfterDiscount).ToList();

            var (total, data) = resultReport
                .AsQueryable()
                .DynamicFilter(modelFilter)
                .DynamicSort(modelFilter)
                .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

            return new BaseResponsePagingViewModel<ProductReportViewModel>()
            {
                Metadata = new PagingMetadata()
                {
                    Page = paging.Page,
                    Size = paging.Size,
                    Total = total
                },
                Data = data.ToList()
            };
        }

        public List<ProductReportViewModel> GetProductReport(DateFilter filter, int? storeId)
        {
            #region Check Date range

            var from = filter?.FromDate;
            var to = filter?.ToDate;
            if (from == null && to == null)
            {
                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
                to = Utils.GetLastAndFirstDateInCurrentMonth().Item2;
            }

            if (from == null)
            {
                from = Utils.GetCurrentDate();
            }

            if (to == null)
            {
                to = Utils.GetCurrentDate();
            }

            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
            {
                throw new ErrorResponse(400, "The datetime is invalid!");
            }

            #endregion

            var resultProductReport = _context.DateProducts
                .Where(x =>
                    x.Active == true &&
                    DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
                    DateTime.Compare(x.Date, (DateTime)to) <= 0
                );

            if (storeId != null)
            {
                resultProductReport = _context.DateProducts
                    .Where(x =>
                        x.Active == true &&
                        x.StoreId == storeId &&
                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
                        DateTime.Compare(x.Date, (DateTime)to) <= 0);
            }

            var resultReport = resultProductReport.GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    CateName = x.CategoryName,
                    ProductCode = x.ProductCode,
                    UnitPrice = x.UnitPrice,
                })
                .Select(x => new ProductReportViewModel()
                {
                    ProductName = x.Key.ProductName,
                    ProductId = x.Key.ProductId,
                    CateName = x.Key.CateName,
                    Quantity = x.Sum(dateProduct => dateProduct.Quantity),
                    ProductCode = x.Key.ProductCode,
                    UnitPriceNoVat = 0,
                    UnitPrice = x.Key.UnitPrice,
                    Percent = 0,
                    TotalBeforeDiscount = 0,
                    TotalAfterDiscount = x.Sum(dateProduct => dateProduct.FinalAmount),
                }).OrderByDescending(x => x.TotalAfterDiscount).ToList();
            return resultReport;
        }

        public FileStreamResult ExportProductReport(DateFilter filter, int? storeId)
        {
            var today = Utils.GetCurrentDate();
            if (filter.FromDate == null) filter.FromDate = today;
            if (filter.ToDate == null) filter.ToDate = today;
            var sheetName = filter.FromDate == filter.ToDate
                ? filter.FromDate?.ToString("dd/MM/yyyy")
                : (filter.FromDate?.ToString("dd/MM/yyyy") + "-" + filter.ToDate?.ToString("dd/MM/yyyy"));
            var data = GetProductReport(filter, storeId);

            return ExcelUtils.ExportExcel(new ExcelModel<ProductReportViewModel>()
            {
                SheetTitle = "Báo cáo doanh thu sản phẩm " + sheetName,
                ColumnConfigs = new List<ColumnConfig<ProductReportViewModel>>()
                {
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Mã sản phẩm",
                        DataIndex = "ProductCode",
                        ValueType = "string"
                    },
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Tên sản phẩm",
                        DataIndex = "ProductName",
                        ValueType = "string"
                    },
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Danh Mục",
                        DataIndex = "CateName",
                        ValueType = "string"
                    },
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Đơn vị tính",
                        DataIndex = "Unit",
                        ValueType = "string"
                    },
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Số lượng bán ra",
                        DataIndex = "Quantity",
                        ValueType = "int"
                    },
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Đơn giá (chưa VAT)",
                        DataIndex = "UnitPrice",
                        ValueType = "double"
                    },
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Đơn giá (đã VAT)",
                        DataIndex = "UnitPriceNoVat",
                        ValueType = "double"
                    },
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Doanh thu (VAT)",
                        DataIndex = "TotalBeforeDiscount",
                        ValueType = "double"
                    },
                    new ColumnConfig<ProductReportViewModel>()
                    {
                        Title = "Doanh thu (đã VAT)",
                        DataIndex = "TotalAfterDiscount",
                        ValueType = "double"
                    },
                },
                DataSources = data
            });
        }
    }
}