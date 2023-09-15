using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Reso.Sdk.Core.Custom;
using Reso.Sdk.Core.Utilities;
using ResoReportDataAccess.Models;
using ResoReportDataService.Commons;
using ResoReportDataService.Commons.ExcelUtils;
using ResoReportDataService.Models;
using ResoReportDataService.RequestModels;
using ResoReportDataService.ViewModels;
using AutoMapper;

namespace ResoReportDataService.Services
{
    public interface ISystemReportService
    {
        BaseResponsePagingViewModel<StoreReportViewModel> GetStoreReports(int? storeId, DateFilter filter, PagingModel paging, StoreReportViewModel modelFilter);
        FileStreamResult ExportStoreReport(int? storeId, DateFilter filter);
    }

    public class SystemReportService : ISystemReportService
    {
        private readonly DataWareHouseReportingContext _context;
        private readonly ProdPassioContext _passioContext;
        private readonly IMapper _mapper;

        public SystemReportService(DataWareHouseReportingContext context, IMapper mapper, ProdPassioContext passioContext)
        {
            _context = context;
            _mapper = mapper;
            _passioContext = passioContext;
        }

        private List<StoreReportViewModel> GetListStoreReports(int? storeId, DateFilter filter)
        {
            #region Check Date range

            var from = filter?.FromDate;
            var to = filter?.ToDate;

            if (from == null && to == null)
            {
                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
                to = Utils.GetCurrentDate().AddDays(-1);
            }

            from ??= Utils.GetCurrentDate();
            to ??= Utils.GetCurrentDate();

            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
            {
                throw new ErrorResponse(400, "The datetime is invalid!");
            }

            from = ((DateTime)from).GetStartOfDate();
            to = ((DateTime)to).GetEndOfDate();

            #endregion

            var dateReportsAllStore = _passioContext.DateReports
                .Where(x => x.Date <= to && x.Date >= from && x.Status == 1);

            var dateReportsOneStore = _passioContext.DateReports
                .Where(x => x.Date <= to && x.Date >= from && x.Status == 1 && x.StoreId == storeId);

            IQueryable<ResoReportDataService.Models.DateReport> dateReports;

            if (storeId == null)
            {
                dateReports = dateReportsAllStore;
            }
            else
            {
                dateReports = dateReportsOneStore;
            }


            var result = dateReports
                .Include(x => x.Store)
                .Where(x => x.Date <= to && x.Date >= from && x.Status == 1)
                .GroupBy(x => new
                {
                    StoreId = x.StoreId,
                    StoreName = x.Store.Name,
                })
                .Select(x => new StoreReportViewModel()
                {
                    StoreName = x.Key.StoreName,
                    TotalOrderAtStore = x.Sum(w => w.TotalOrderAtStore),
                    TotalOrderTakeAway = x.Sum(w => w.TotalOrderTakeAway),
                    TotalOrderDelivery = x.Sum(w => w.TotalOrderDelivery),
                    FinalAmountAtStore = x.Sum(w => w.FinalAmountAtStore),
                    FinalAmountTakeAway = x.Sum(w => w.FinalAmountTakeAway),
                    FinalAmountDelivery = x.Sum(w => w.FinalAmountDelivery),
                    TotalBills = x.Sum(w => w.TotalOrderAtStore + w.TotalOrderTakeAway + w.TotalOrderDelivery),
                    TotalSales = x.Sum(w => w.TotalAmount) - x.Sum(w => w.FinalAmountCard),
                    TotalDiscount = x.Sum(w => w.Discount + w.DiscountOrderDetail),
                    TotalSalesAfterDiscount = x.Sum(w => w.FinalAmount) - x.Sum(w => w.FinalAmountCard)
                }).ToList();

            return result;
        }

        public BaseResponsePagingViewModel<StoreReportViewModel> GetStoreReports(int? storeId, DateFilter filter, PagingModel paging, StoreReportViewModel modelFilter)
        {
            var result = GetListStoreReports(storeId, filter);

            var (total, data) = result
                .AsQueryable()
                .DynamicFilter(modelFilter)
                .DynamicSort(modelFilter)
                .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

            return new BaseResponsePagingViewModel<StoreReportViewModel>()
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

        public FileStreamResult ExportStoreReport(int? storeId, DateFilter filter)
        {
            #region Check Date range

            var from = filter?.FromDate;
            var to = filter?.ToDate;

            if (from == null && to == null)
            {
                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
                to = Utils.GetCurrentDate().AddDays(-1);
            }

            from ??= Utils.GetCurrentDate();
            to ??= Utils.GetCurrentDate();

            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
            {
                throw new ErrorResponse(400, "The datetime is invalid!");
            }

            from = ((DateTime)from).GetStartOfDate();
            to = ((DateTime)to).GetEndOfDate();

            #endregion


            filter.FromDate = from;
            filter.ToDate = to;

            var sheetName = filter.FromDate?.Date == filter.ToDate?.Date ? filter.FromDate?.ToString("dd/MM/yyyy") : (filter.FromDate?.ToString("dd/MM/yyyy") + "-" + filter.ToDate?.ToString("dd/MM/yyyy"));

            var result = GetListStoreReports(storeId, filter);

            return ExcelUtils.ExportExcel(new ExcelModel<StoreReportViewModel>()
            {
                SheetTitle = "BaoCaoKinhDoanh-" + sheetName,
                ColumnConfigs = new List<ColumnConfig<StoreReportViewModel>>()
                {
                     new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Tên cửa hàng",
                        DataIndex = "StoreName",
                        ValueType = "string"
                    },
                      new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Số lượng (Mang đi)",
                        DataIndex = "TotalOrderTakeAway",
                        ValueType = "int"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Doanh thu (Mang đi)",
                        DataIndex = "FinalAmountTakeAway",
                        ValueType = "currency"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Số lượng (Tại Store)",
                        DataIndex = "TotalOrderAtStore",
                        ValueType = "int"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Doanh thu (Tại Store)",
                        DataIndex = "FinalAmountAtStore",
                        ValueType = "currency"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Số lượng (Giao Hàng)",
                        DataIndex = "TotalOrderDelivery",
                        ValueType = "int"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Doanh Thu (Giao Hàng)",
                        DataIndex = "FinalAmountDelivery",
                        ValueType = "currency"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Tổng số bill",
                        DataIndex = "TotalBills",
                        ValueType = "int"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Tổng doanh thu",
                        DataIndex = "TotalSales",
                        ValueType = "currency"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Tiền giảm giá",
                        DataIndex = "TotalDiscount",
                        ValueType = "currency"
                    },
                    new ColumnConfig<StoreReportViewModel>()
                    {
                        Title = "Tổng doanh thu sau giảm giá",
                        DataIndex = "TotalSalesAfterDiscount",
                        ValueType = "currency"
                    },
                },
                DataSources = result
            });
        }
    }
}