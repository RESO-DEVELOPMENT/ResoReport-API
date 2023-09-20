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
            PagingModel paging, Guid? storeId);

        List<ProductReportViewModel> GetProductReport(DateFilter filter, Guid? storeId);

        FileStreamResult ExportProductReport(DateFilter filter, Guid? storeId);
    }

    public class ProductReportService : IProductReportService
    {
        private readonly DataWareHouseReportingContext _context;
        private readonly PosSystemContext _posSystemContext;

        public ProductReportService(DataWareHouseReportingContext context, PosSystemContext posSystemContext)
        {
            _context = context;
            _posSystemContext = posSystemContext;
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
           PagingModel paging, Guid? storeId)
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

            var orderReport = _posSystemContext.Orders
                .Where(x => DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                            DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0);

            if (storeId != null)
            {
                orderReport = _posSystemContext.Orders
                    .Include(x => x.Session)
                    .Where(x => x.Session.StoreId.Equals(storeId) &&
                                DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                                DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0);
            }

            var orderDetails = orderReport.SelectMany(x => x.OrderDetails);

            var productsReport = orderDetails.GroupBy(x => new
            {
                ProductId = x.MenuProduct.Product.Id,
                ProductName = x.MenuProduct.Product.Name,
                ProductCode = x.MenuProduct.Product.Code,
                CategoryName = x.MenuProduct.Product.Category.Name,
                Size = x.MenuProduct.Product.Size,
                Type = x.MenuProduct.Product.Type,
            })
                .Select(x => new ProductReportViewModel
                {
                    ProductId = x.Key.ProductId,
                    ProductName = x.Key.ProductName,
                    ProductCode = x.Key.ProductCode,

                    CateName = x.Key.CategoryName,

                    UnitPrice = 0,
                    UnitPriceNoVat = 0,

                    Unit = "VND",

                    TotalPriceBeforeVat = 0,
                    Vat = 0,
                    Percent = 0,

                    Quantity = x.Sum(orderDetail => orderDetail.Quantity),
                    Discount = x.Sum(orderDetail => orderDetail.Discount),
                    TotalBeforeDiscount = x.Sum(orderDetail => orderDetail.TotalAmount),
                    TotalAfterDiscount = x.Sum(orderDetail => orderDetail.FinalAmount)
                })
                .OrderByDescending(x => x.TotalAfterDiscount).ToList();

            var (total, data) = productsReport
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

        public List<ProductReportViewModel> GetProductReport(DateFilter filter, Guid? storeId)
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

            var orderReport = _posSystemContext.Orders
                .Where(x => DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                            DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0);

            if (storeId != null)
            {
                orderReport = _posSystemContext.Orders
                    .Include(x => x.Session)
                    .Where(x => x.Session.StoreId.Equals(storeId) &&
                                DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                                DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0);
            }

            var orderDetails = orderReport.SelectMany(x => x.OrderDetails);

            var productsReport = orderDetails.GroupBy(x => new
            {
                ProductId = x.MenuProduct.Product.Id,
                ProductName = x.MenuProduct.Product.Name,
                ProductCode = x.MenuProduct.Product.Code,
                CategoryName = x.MenuProduct.Product.Category.Name,
                Size = x.MenuProduct.Product.Size,
                Type = x.MenuProduct.Product.Type,
            })
                .Select(x => new ProductReportViewModel
                {
                    ProductId = x.Key.ProductId,
                    ProductName = x.Key.ProductName,
                    ProductCode = x.Key.ProductCode,

                    CateName = x.Key.CategoryName,

                    UnitPrice = 0,
                    UnitPriceNoVat = 0,

                    Unit = "VND",

                    TotalPriceBeforeVat = 0,
                    Vat = 0,
                    Percent = 0,

                    Quantity = x.Sum(orderDetail => orderDetail.Quantity),
                    Discount = x.Sum(orderDetail => orderDetail.Discount),
                    TotalBeforeDiscount = x.Sum(orderDetail => orderDetail.TotalAmount),
                    TotalAfterDiscount = x.Sum(orderDetail => orderDetail.FinalAmount)
                })
                .OrderByDescending(x => x.TotalAfterDiscount).ToList();

            return productsReport;
        }

        public FileStreamResult ExportProductReport(DateFilter filter, Guid? storeId)
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