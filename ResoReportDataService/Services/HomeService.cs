//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using Microsoft.EntityFrameworkCore;
//using ResoReportDataAccess.Models;
//using ResoReportDataService.Commons;
//using ResoReportDataService.Models;
//using ResoReportDataService.RequestModels;
//using ResoReportDataService.ViewModels;

//namespace ResoReportDataService.Services
//{
//    public interface IHomeService
//    {
//        SummaryViewModel GetSummary(int? storeId);
//        BusinessInsightViewModel GetBusinessInsights(string duration, int? storeId);
//    }

//    public class HomeService : IHomeService
//    {
//        private readonly ProdPassioContext _passioContext;
//        private readonly DataWareHouseReportingContext _dataWareHouseReportingContext;

//        public HomeService(ProdPassioContext passioContext, DataWareHouseReportingContext dataWareHouseReportingContext)
//        {
//            _passioContext = passioContext;
//            _dataWareHouseReportingContext = dataWareHouseReportingContext;
//        }

//        public SummaryViewModel GetSummary(int? storeId)
//        {
//            var orders =
//                storeId == null
//                    ? _passioContext.Orders
//                        .Include(x => x.Store)
//                        .Where(x =>
//                            x.CheckInDate.HasValue &&
//                            x.Store.RunReport == true &&
//                            x.Store.IsAvailable == true &&
//                            x.Store.Type != (int)StoreTypeEnum.Hostel &&
//                            x.OrderStatus == (int)OrderStatusEnum.Finish &&
//                            DateTime.Compare((DateTime)x.CheckInDate, Utils.GetCurrentDate().GetStartOfDate()) >= 0 &&
//                            DateTime.Compare((DateTime)x.CheckInDate, Utils.GetCurrentDate().GetEndOfDate()) <= 0)
//                    : _passioContext.Orders
//                        .Include(x => x.Store)
//                        .Where(x =>
//                            x.StoreId == storeId &&
//                            x.CheckInDate.HasValue &&
//                            x.Store.RunReport == true &&
//                            x.Store.IsAvailable == true &&
//                            x.Store.Type != (int)StoreTypeEnum.Hostel &&
//                            x.OrderStatus == (int)OrderStatusEnum.Finish &&
//                            DateTime.Compare((DateTime)x.CheckInDate, Utils.GetCurrentDate().GetStartOfDate()) >= 0 &&
//                            DateTime.Compare((DateTime)x.CheckInDate, Utils.GetCurrentDate().GetEndOfDate()) <= 0);

//            return new SummaryViewModel()
//            {
//                NetSales = orders.Sum(x => x.TotalAmount) -
//                           orders.Where(x => x.OrderType == (int)OrderTypeEnum.OrderCard).Sum(x => x.TotalAmount),
//                TotalOrders = orders.Count() - orders.Count(x => x.OrderType == (int)OrderTypeEnum.OrderCard),
//                LastUpdatedTime = Utils.GetCurrentDate()
//            };
//        }

//        public BusinessInsightViewModel GetBusinessInsights(string duration, int? storeId)
//        {
//            (DateTime, DateTime) durationDateTime = Utils.GetPast7Days();
//            (DateTime, DateTime) durationPreviousDateTime = Utils.GetPreviousPast7Days();


//            #region Get duration

//            switch (duration)
//            {
//                case CommonConstants.Duration.Past7Days:
//                    durationDateTime = Utils.GetPast7Days();
//                    durationPreviousDateTime = Utils.GetPreviousPast7Days();
//                    break;
//                case CommonConstants.Duration.Past30Days:
//                    durationDateTime = Utils.GetPast30Days();
//                    durationPreviousDateTime = Utils.GetPreviousPast30Days();
//                    break;
//                case CommonConstants.Duration.Past90Days:
//                    durationDateTime = Utils.GetPast90Days();
//                    durationPreviousDateTime = Utils.GetPreviousPast90Days();
//                    break;
//                case CommonConstants.Duration.PastMonth:
//                    durationDateTime = Utils.GetPastMonth();
//                    durationPreviousDateTime = Utils.GetPreviousPastMonth();
//                    break;
//                case CommonConstants.Duration.PastWeek:
//                    durationDateTime = Utils.GetPastWeek();
//                    durationPreviousDateTime = Utils.GetPreviousPastWeek();
//                    break;
//            }

//            #endregion


//            #region Get reports

//            var dateReports = storeId == null
//                ? _passioContext.DateReports
//                    .Where(x => x.Status == 1)
//                : _passioContext.DateReports
//                    .Where(x => x.Status == 1 && x.StoreId == storeId);

//            var dateReportsInDuration = dateReports
//                .Where(x =>
//                    DateTime.Compare(x.Date, durationDateTime.Item1) >= 0 &&
//                    DateTime.Compare(x.Date, durationDateTime.Item2) <= 0);

//            var dateReportsInPreviousDuration = dateReports
//                .Where(x =>
//                    DateTime.Compare(x.Date, durationPreviousDateTime.Item1) >= 0 &&
//                    DateTime.Compare(x.Date, durationPreviousDateTime.Item2) <= 0);

//            //var dateProducts = storeId == null
//            //    ? _passioContext.DateProducts
//            //        .Where(x => x.Status == 1 &&
//            //                    DateTime.Compare(x.Date, durationDateTime.Item1) >= 0 &&
//            //                    DateTime.Compare(x.Date, durationDateTime.Item2) <= 0)
//            //    : _passioContext.DateProducts
//            //        .Where(x => x.Status == 1 && x.StoreId == storeId &&
//            //                    DateTime.Compare(x.Date, durationDateTime.Item1) >= 0 &&
//            //                    DateTime.Compare(x.Date, durationDateTime.Item2) <= 0);

//            var dateProducts = storeId == null
//                ? _passioContext.DateProducts
//                    .Where(x => 
//                                DateTime.Compare(x.Date, durationDateTime.Item1) >= 0 &&
//                                DateTime.Compare(x.Date, durationDateTime.Item2) <= 0)
//                : _passioContext.DateProducts
//                    .Where(x =>  x.StoreId == storeId &&
//                                DateTime.Compare(x.Date, durationDateTime.Item1) >= 0 &&
//                                DateTime.Compare(x.Date, durationDateTime.Item2) <= 0);

//            #endregion

//            #region Number of transactions dashboard

//            var orderInsight = dateReportsInDuration
//                .GroupBy(x => new
//                {
//                    x.Date
//                })
//                .Select(x => new  DashboardInsight
//                    ()
//                {
//                    Date = x.Key.Date,
//                    Value = x.Sum(r => r.TotalOrder)
//                });

//            #endregion


//            #region TrendInsights

//            var totalTransaction = new TrendInsight()
//            {
//                Value = dateReportsInDuration.Sum(r => r.TotalOrder),
//                Trend = dateReportsInPreviousDuration.Sum(r => r.TotalOrder) != 0
//                    ? (dateReportsInDuration.Sum(r => r.TotalOrder) -
//                       dateReportsInPreviousDuration.Sum(r => r.TotalOrder)) /
//                    (double)dateReportsInPreviousDuration.Sum(r => r.TotalOrder) * 100
//                    : 0
//            };

//            var grossSales = new TrendInsight()
//            {
//                Value = dateReportsInDuration.Sum(x => x.FinalAmount) ?? 0,
//                Trend = dateReportsInPreviousDuration.Sum(x => x.FinalAmount) != 0
//                    ? (dateReportsInDuration.Sum(x => x.FinalAmount) -
//                       dateReportsInPreviousDuration.Sum(x => x.FinalAmount)) /
//                    dateReportsInPreviousDuration.Sum(x => x.FinalAmount) * 100
//                    : 0
//            };

//            var netSales = new TrendInsight()
//            {
//                Value = dateReportsInDuration.Sum(x => x.TotalAmount) ?? 0,
//                Trend = dateReportsInPreviousDuration.Sum(x => x.TotalAmount) != 0
//                    ? (dateReportsInDuration.Sum(x => x.TotalAmount) -
//                       dateReportsInPreviousDuration.Sum(x => x.TotalAmount)) /
//                    dateReportsInPreviousDuration.Sum(x => x.TotalAmount) * 100
//                    : 0
//            };

//            var avgTransactionAmount = new TrendInsight()
//            {
//                Value =
//                    dateReportsInDuration.Count() != 0
//                        ? dateReportsInDuration.Sum(x => x.TotalAmount) / dateReportsInDuration.Sum(x=>x.TotalOrder)
//                        : 0,
//                Trend = (dateReportsInPreviousDuration.Count() != 0
//                    ? dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Count()
//                    : 0) != 0
//                    ? ((dateReportsInDuration.Sum(x => x.TotalAmount) / dateReportsInDuration.Sum(x=>x.TotalOrder) -
//                        dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Sum(x=>x.TotalOrder)) /
//                       dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Sum(x=>x.TotalOrder)) *
//                      100
//                    : 0
//            };

//            #endregion


//            return new BusinessInsightViewModel()
//            {
//                TopPerformingStore = storeId == null
//                    ? dateReportsInDuration
//                        .OrderByDescending(x => x.TotalAmount)
//                        .First().Store.Name
//                    : "",
//                TopSellingItem = dateProducts
//                    .GroupBy(x => new
//                    {
//                        x.ProductId,
//                        x.ProductName
//                    })
//                    .Select(x => new
//                    {
//                        FinalAmount = x.Sum(d => d.TotalAmount),
//                        ProductName = x.Key.ProductName
//                    }).OrderByDescending(x => x.FinalAmount).First().ProductName,
//                Orders = orderInsight.ToList(),
//                TotalTransaction = totalTransaction,
//                GrossSales = grossSales,
//                NetSales = netSales,
//                // TotalCustomers = ,
//                AvgTransactionAmount = avgTransactionAmount
//            };
//        }
//    }
//}