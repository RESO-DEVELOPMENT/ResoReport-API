using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
    public interface IPaymentReportService
    {
        BaseResponsePagingViewModel<PaymentReportViewModel> GetPaymentReports(PaymentReportViewModel modelFilter,
            DateFilter filter,
            PagingModel paging, int? storeId);

        FileStreamResult ExportPaymentReport(DateFilter filter, int? storeId);

        BaseResponsePagingViewModel<PaymentReportOneStoreViewModel> GetPaymentReportsOneStore(
            PaymentReportOneStoreViewModel modelFilter,
            DateFilter filter,
            PagingModel paging, int? storeId);
    }

    public class PaymentReportService : IPaymentReportService
    {
        private readonly DataWareHouseReportingContext _dataWareHouseReportingContext;
        private readonly ProdPassioContext _prodPassioContext;

        public PaymentReportService(DataWareHouseReportingContext dataWareHouseReportingContext, ProdPassioContext prodPassioContext)
        {
            _dataWareHouseReportingContext = dataWareHouseReportingContext;
            _prodPassioContext = prodPassioContext;
        }

        public BaseResponsePagingViewModel<PaymentReportViewModel> GetPaymentReports(
            PaymentReportViewModel modelFilter,
            DateFilter filter,
            PagingModel paging, int? storeId)
        {
            #region Check Date range

            var from = filter?.FromDate;
            var to = filter?.ToDate;

            if (from == null && to == null)
            {
                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
                to = Utils.GetLastAndFirstDateInCurrentMonth().Item2;
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

            var (total, data) =
                GetPaymentReports((DateTime)from, (DateTime)to, storeId)
                    .OrderByDescending(x => x.Cash)
                    .DynamicFilter(modelFilter)
                    .DynamicSort(modelFilter)
                    .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging,
                        CommonConstants.DefaultPaging);
            return new BaseResponsePagingViewModel<PaymentReportViewModel>()
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


        public BaseResponsePagingViewModel<PaymentReportOneStoreViewModel> GetPaymentReportsOneStore(
            PaymentReportOneStoreViewModel modelFilter,
            DateFilter filter,
            PagingModel paging, int? storeId)
        {
            #region Check Date range

            var from = filter?.FromDate;
            var to = filter?.ToDate;

            if (from == null && to == null)
            {
                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
                to = Utils.GetLastAndFirstDateInCurrentMonth().Item2;
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

            var (total, data) =
                GetPaymentReportsOneStore((DateTime)from, (DateTime)to, storeId)
                    .OrderByDescending(x => x.Date)
                    .DynamicFilter(modelFilter)
                    .DynamicSort(modelFilter)
                    .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging,
                        CommonConstants.DefaultPaging);
            return new BaseResponsePagingViewModel<PaymentReportOneStoreViewModel>()
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

        //private IQueryable<PaymentReportViewModel> GetPaymentReports(DateTime from, DateTime to, int? storeId)
        //{
        //    var dateReports = storeId != null
        //        ? _dataWareHouseReportingContext.DateReports
        //            .Where(x => x.Active == true && x.StoreId == storeId &&
        //                        DateTime.Compare(x.Date, from) >= 0 &&
        //                        DateTime.Compare(x.Date, to) <= 0)
        //        : _dataWareHouseReportingContext.DateReports
        //            .Where(x => x.Active == true &&
        //                        DateTime.Compare(x.Date, from) >= 0 &&
        //                        DateTime.Compare(x.Date, to) <= 0);

        //    var dateReportGroup = dateReports
        //        .GroupBy(x => new
        //        {
        //            x.StoreId,
        //            x.StoreName
        //        })
        //        .Select(x => new
        //        {
        //            x.Key.StoreId,
        //            FinalAmountCard = x.Sum(dateReport => dateReport.FinalAmountCard),
        //        });

        //    var paymentReports =
        //        storeId != null
        //            ? _dataWareHouseReportingContext.PaymentReports
        //                .Where(x => x.Active == true && x.StoreId == storeId &&
        //                            DateTime.Compare(x.Date, from) >= 0 &&
        //                            DateTime.Compare(x.Date, to) <= 0)
        //            : _dataWareHouseReportingContext.PaymentReports
        //                .Where(x => x.Active == true &&
        //                            DateTime.Compare(x.Date, from) >= 0 &&
        //                            DateTime.Compare(x.Date, to) <= 0);

        //    var result = paymentReports
        //        .AsEnumerable()
        //        .GroupBy(x =>
        //            new
        //            {
        //                x.StoreId,
        //                x.StoreName
        //            })
        //        .Select(x => new PaymentReportViewModel()
        //        {
        //            StoreName = x.Key.StoreName,
        //            Cash = x.Where(paymentReport =>
        //                        paymentReport.PaymentType == (int)PaymentTypeEnum.Cash ||
        //                        paymentReport.PaymentType == (int)PaymentTypeEnum.ExchangeCash)
        //                    .Sum(paymentReport => paymentReport.Amount) -
        //                dateReportGroup
        //                    .FirstOrDefault(d => d.StoreId == x.Key.StoreId)?.FinalAmountCard ?? 0,
        //            CreditCard = dateReportGroup
        //                .FirstOrDefault(d => d.StoreId == x.Key.StoreId)
        //                ?.FinalAmountCard ?? 0,
        //            CreditCardUse = x.Where(paymentReport =>
        //                    paymentReport.PaymentType == (int)PaymentTypeEnum.MemberPayment)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            Bank = x.Where(paymentReport =>
        //                    paymentReport.PaymentType == (int)PaymentTypeEnum.MasterCard ||
        //                    paymentReport.PaymentType == (int)PaymentTypeEnum.VisaCard)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            Baemin = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.BaeMin)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            GrabPay = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.GrabPay)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            GrabFood = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.GrabFood)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            VnPay = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.VNPay)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            ShopeePay = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.Shopeepay)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            Momo = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.MoMo)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            ZaloPay = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.ZaloPay)
        //                .Sum(paymentReport => paymentReport.Amount)
        //        })
        //        .AsQueryable();

        //    return result;
        //}
        private IQueryable<PaymentReportViewModel> GetPaymentReports(DateTime from, DateTime to, int? storeId)
        {
            var stores = _prodPassioContext.Stores
                .Where(q => q.IsAvailable.Value)
                .Select(q => new
                {
                    q.Id,
                    q.Name
                });

            var storeIds = _prodPassioContext.Stores.Select(s => s.Id).ToList();
            var paymentInDateRange =
            storeId != null ?
                _prodPassioContext.Payments.Where(q => q.PayTime >= from && q.PayTime <= to
                                     && q.ToRent.StoreId == storeId
                                    && q.ToRent.OrderStatus == (int)OrderStatusEnum.Finish)
                .Where(q => q.ToRent.OrderType != (int)OrderTypeEnum.OrderCard && q.ToRent.StoreId.HasValue && storeIds.Contains(q.ToRent.StoreId.Value))
                    .Select(q => new
                    {
                        q.ToRent.StoreId,
                        q.Amount,
                        q.Type,
                        q.ToRent.Store.Name,
                        q.PayTime
                    }).AsEnumerable() :

                _prodPassioContext.Payments.Where(q => q.PayTime >= from && q.PayTime <= to
                                    && q.ToRent.OrderStatus == (int)OrderStatusEnum.Finish).
                Where(q => q.ToRent.OrderType != (int)OrderTypeEnum.OrderCard && q.ToRent.StoreId.HasValue && storeIds.Contains(q.ToRent.StoreId.Value))
                    .Select(q => new
                    {
                        q.ToRent.StoreId,
                        q.Amount,
                        q.Type,
                        q.ToRent.Store.Name,
                        q.PayTime
                    }).AsEnumerable();


            var orderInDateRange =
            storeId == null ?
                _prodPassioContext.Orders.Where(q => q.StoreId == storeId && q.OrderType == (int)OrderTypeEnum.OrderCard && q.CheckInDate >= from && q.CheckInDate <= to && q.OrderStatus == (int)OrderStatusEnum.Finish)
                .GroupBy(q => q.StoreId)
                 .Select(q => new
                 {
                     q.Key,
                     FinalAmount = q.Sum(a => a.FinalAmount),
                 }) :
                _prodPassioContext.Orders.Where(q => q.OrderType == (int)OrderTypeEnum.OrderCard && q.CheckInDate >= from && q.CheckInDate <= to && q.OrderStatus == (int)OrderStatusEnum.Finish)
                .GroupBy(q => q.StoreId)
                 .Select(q => new
                 {
                     q.Key,
                     FinalAmount = q.Sum(a => a.FinalAmount),
                 });



            var displayResult = paymentInDateRange.GroupBy(q => q.StoreId).OrderBy(q => q.Key);
            var result = displayResult
           .Select(q => new PaymentReportViewModel
           {
               StoreName = stores.FirstOrDefault(a => a.Id == q.Key).Name.Contains("\r\n") ?
               stores.FirstOrDefault(a => a.Id == q.Key).Name.Replace("\r\n", "") :
               stores.FirstOrDefault(a => a.Id == q.Key).Name,
               Cash = q.Where(a => a.Type == (int)PaymentTypeEnum.Cash).Sum(a => a.Amount)
                   + q.Where(a => a.Type == (int)PaymentTypeEnum.ExchangeCash).Sum(a => a.Amount),
               CreditCard = orderInDateRange.Any(a => a.Key == q.Key) ? orderInDateRange.Where(a => a.Key == q.Key).Sum(a => a.FinalAmount) : 0,
               Momo = q.Where(a => a.Type == (int)PaymentTypeEnum.MoMo).Sum(a => a.Amount),
               Bank = q.Where(a => a.Type == (int)PaymentTypeEnum.MasterCard || a.Type == (int)PaymentTypeEnum.VisaCard).Sum(a => a.Amount),
               CreditCardUse = q.Where(a => a.Type == (int)PaymentTypeEnum.MemberPayment).Sum(a => a.Amount),
               GrabPay = q.Where(a => a.Type == (int)PaymentTypeEnum.GrabPay).Sum(a => a.Amount),
               GrabFood = q.Where(a => a.Type == (int)PaymentTypeEnum.GrabFood).Sum(a => a.Amount),
               VnPay = q.Where(a => a.Type == (int)PaymentTypeEnum.VNPay).Sum(a => a.Amount),
               Baemin = q.Where(a => a.Type == (int)PaymentTypeEnum.BaeMin).Sum(a => a.Amount),
               ShopeePay = q.Where(a => a.Type == (int)PaymentTypeEnum.Shopeepay).Sum(a => a.Amount),
               ZaloPay = q.Where(a => a.Type == (int)PaymentTypeEnum.ZaloPay).Sum(a => a.Amount)
           }).AsQueryable();
            return result;
        }



        //private IQueryable<PaymentReportOneStoreViewModel> GetPaymentReportsOneStore(DateTime from, DateTime to,
        //    int? storeId)
        //{
        //    var dateReports = storeId != null
        //        ? _dataWareHouseReportingContext.DateReports
        //            .Where(x => x.Active == true && x.StoreId == storeId &&
        //                        DateTime.Compare(x.Date, from) >= 0 &&
        //                        DateTime.Compare(x.Date, to) <= 0)
        //        : _dataWareHouseReportingContext.DateReports
        //            .Where(x => x.Active == true &&
        //                        DateTime.Compare(x.Date, from) >= 0 &&
        //                        DateTime.Compare(x.Date, to) <= 0);

        //    var dateReportGroup = dateReports
        //        .GroupBy(x => new
        //        {
        //            x.StoreId,
        //            x.StoreName,
        //            x.Date
        //        })
        //        .Select(x => new
        //        {
        //            x.Key.StoreId,
        //            x.Key.Date,
        //            FinalAmountCard = x.Sum(dateReport => dateReport.FinalAmountCard),
        //        });

        //    var paymentReports =
        //        storeId != null
        //            ? _dataWareHouseReportingContext.PaymentReports
        //                .Where(x => x.Active == true && x.StoreId == storeId &&
        //                            DateTime.Compare(x.Date, from) >= 0 &&
        //                            DateTime.Compare(x.Date, to) <= 0)
        //            : _dataWareHouseReportingContext.PaymentReports
        //                .Where(x => x.Active == true &&
        //                            DateTime.Compare(x.Date, from) >= 0 &&
        //                            DateTime.Compare(x.Date, to) <= 0);

        //    var result = paymentReports
        //        .AsEnumerable()
        //        .GroupBy(x =>
        //            new
        //            {
        //                x.StoreId,
        //                x.StoreName,
        //                x.Date
        //            })
        //        .Select(x => new PaymentReportOneStoreViewModel()
        //        {
        //            Date = x.Key.Date,
        //            Cash = x.Where(paymentReport =>
        //                        paymentReport.PaymentType == (int)PaymentTypeEnum.Cash ||
        //                        paymentReport.PaymentType == (int)PaymentTypeEnum.ExchangeCash)
        //                    .Sum(paymentReport => paymentReport.Amount) -
        //                dateReportGroup
        //                    .FirstOrDefault(d => d.StoreId == x.Key.StoreId)?.FinalAmountCard ?? 0,
        //            CreditCard = dateReportGroup
        //                .FirstOrDefault(d => d.StoreId == x.Key.StoreId && x.Key.Date == d.Date)
        //                ?.FinalAmountCard ?? 0,
        //            CreditCardUse = x.Where(paymentReport =>
        //                    paymentReport.PaymentType == (int)PaymentTypeEnum.MemberPayment)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            Bank = x.Where(paymentReport =>
        //                    paymentReport.PaymentType == (int)PaymentTypeEnum.MasterCard ||
        //                    paymentReport.PaymentType == (int)PaymentTypeEnum.VisaCard)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            Baemin = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.BaeMin)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            GrabPay = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.GrabPay)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            GrabFood = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.GrabFood)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            VnPay = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.VNPay)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            ShopeePay = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.Shopeepay)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            Momo = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.MoMo)
        //                .Sum(paymentReport => paymentReport.Amount),
        //            ZaloPay = x.Where(paymentReport => paymentReport.PaymentType == (int)PaymentTypeEnum.ZaloPay)
        //                .Sum(paymentReport => paymentReport.Amount)
        //        })
        //        .AsQueryable();

        //    return result;
        //}
        private IQueryable<PaymentReportOneStoreViewModel> GetPaymentReportsOneStore(DateTime from, DateTime to,
            int? storeId)
        {
            var stores = _prodPassioContext.Stores
                .Where(q => q.IsAvailable.Value)
                .Select(q => new
                {
                    q.Id,
                    q.Name
                });

            var paymentInDateRange =
            //storeId != null ?
            //    _prodPassioContext.Payments.Where(q => q.PayTime >= from && q.PayTime <= to
            //                         && q.ToRent.StoreId == storeId
            //                        && q.ToRent.OrderStatus == (int)OrderStatusEnum.Finish)
            //    .Where(q => q.ToRent.OrderType != (int)OrderTypeEnum.OrderCard && q.ToRent.StoreId.HasValue && q.ToRent.StoreId.Value == storeId)
            //        .Select(q => new
            //        {
            //            q.ToRent.StoreId,
            //            q.Amount,
            //            q.Type,
            //            q.ToRent.Store.Name,
            //            q.PayTime
            //        }).AsEnumerable() :

                _prodPassioContext.Payments.Where(q => q.PayTime >= from && q.PayTime <= to
                                    && q.ToRent.OrderStatus == (int)OrderStatusEnum.Finish).
                Where(q => q.ToRent.OrderType != (int)OrderTypeEnum.OrderCard && q.ToRent.StoreId.HasValue && q.ToRent.StoreId.Value == storeId)
                    .Select(q => new
                    {
                        q.ToRent.StoreId,
                        q.Amount,
                        q.Type,
                        q.ToRent.Store.Name,
                        q.PayTime
                    }).AsEnumerable();


            var orderInDateRange =
            //storeId == null ?
            //    _prodPassioContext.Orders.Where(q => q.StoreId == storeId && q.OrderType == (int)OrderTypeEnum.OrderCard && q.CheckInDate >= from && q.CheckInDate <= to && q.OrderStatus == (int)OrderStatusEnum.Finish)
            //    .GroupBy(q => q.StoreId)
            //     .Select(q => new
            //     {
            //         q.Key,
            //         FinalAmount = q.Sum(a => a.FinalAmount),
            //     }) :
                _prodPassioContext.Orders.Where(q => q.OrderType == (int)OrderTypeEnum.OrderCard && q.CheckInDate >= from && q.CheckInDate <= to && q.OrderStatus == (int)OrderStatusEnum.Finish)
                .GroupBy(q => q.StoreId)
                 .Select(q => new
                 {
                     q.Key,
                     FinalAmount = q.Sum(a => a.FinalAmount),
                 });



            var displayResult = paymentInDateRange.GroupBy(q => q.StoreId).OrderBy(q => q.Key);
            var result = displayResult
           .Select(q => new PaymentReportOneStoreViewModel
           {
               Date = from,
               Cash = q.Where(a => a.Type == (int)PaymentTypeEnum.Cash).Sum(a => a.Amount)
                   + q.Where(a => a.Type == (int)PaymentTypeEnum.ExchangeCash).Sum(a => a.Amount),
               CreditCard = orderInDateRange.Any(a => a.Key == q.Key) ? orderInDateRange.Where(a => a.Key == q.Key).Sum(a => a.FinalAmount) : 0,
               Momo = q.Where(a => a.Type == (int)PaymentTypeEnum.MoMo).Sum(a => a.Amount),
               Bank = q.Where(a => a.Type == (int)PaymentTypeEnum.MasterCard || a.Type == (int)PaymentTypeEnum.VisaCard).Sum(a => a.Amount),
               CreditCardUse = q.Where(a => a.Type == (int)PaymentTypeEnum.MemberPayment).Sum(a => a.Amount),
               GrabPay = q.Where(a => a.Type == (int)PaymentTypeEnum.GrabPay).Sum(a => a.Amount),
               GrabFood = q.Where(a => a.Type == (int)PaymentTypeEnum.GrabFood).Sum(a => a.Amount),
               VnPay = q.Where(a => a.Type == (int)PaymentTypeEnum.VNPay).Sum(a => a.Amount),
               Baemin = q.Where(a => a.Type == (int)PaymentTypeEnum.BaeMin).Sum(a => a.Amount),
               ShopeePay = q.Where(a => a.Type == (int)PaymentTypeEnum.Shopeepay).Sum(a => a.Amount),
               ZaloPay = q.Where(a => a.Type == (int)PaymentTypeEnum.ZaloPay).Sum(a => a.Amount)
           }).AsQueryable();
            return result;
        }

        public FileStreamResult ExportPaymentReport(DateFilter filter, int? storeId)
        {
            #region Check Date range

            var from = filter?.FromDate;
            var to = filter?.ToDate;

            if (from == null && to == null)
            {
                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
                to = Utils.GetLastAndFirstDateInCurrentMonth().Item2;
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

            var data = GetPaymentReports((DateTime)from, (DateTime)to, storeId);

            return ExcelUtils.ExportExcel(new ExcelModel<PaymentReportViewModel>()
            {
                SheetTitle = "Báo cáo thanh toán - " + from + " - " + to,
                DataSources = data.ToList(),
                ColumnConfigs = new List<ColumnConfig<PaymentReportViewModel>>()
                {
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "Cửa hàng",
                        DataIndex = "StoreName",
                        ValueType = "string"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "Tiền mặt bán hàng",
                        DataIndex = "Cash",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "Tiền mặt nạp thẻ",
                        DataIndex = "CreditCard",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "Tiền sử dụng TTV",
                        DataIndex = "CreditCardUse",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "Ngân hàng",
                        DataIndex = "Bank",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "Momo",
                        DataIndex = "Momo",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "GrabPay",
                        DataIndex = "GrabPay",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "GrabFood",
                        DataIndex = "GrabFood",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "VnPay",
                        DataIndex = "VnPay",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "Baemin",
                        DataIndex = "Baemin",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "ShopeePay",
                        DataIndex = "ShopeePay",
                        ValueType = "number"
                    },
                    new ColumnConfig<PaymentReportViewModel>()
                    {
                        Title = "ZaloPay",
                        DataIndex = "ZaloPay",
                        ValueType = "number"
                    }
                }
            });
        }
    }
}