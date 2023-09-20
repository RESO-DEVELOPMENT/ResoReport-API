using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using ResoReportDataAccess.Models;
using ResoReportDataService.Commons;
using ResoReportDataService.Models;
using ResoReportDataService.RequestModels;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.Services
{
    public interface IHomeService
    {
        SummaryViewModel GetSummary(Guid? storeId);
        BusinessInsightViewModel GetBusinessInsights(string duration, Guid? storeId);
    }

    public class HomeService : IHomeService
    {
        private readonly PosSystemContext _passioContext;
        private readonly DataWareHouseReportingContext _dataWareHouseReportingContext;

        public HomeService(PosSystemContext passioContext, DataWareHouseReportingContext dataWareHouseReportingContext)
        {
            _passioContext = passioContext;
            _dataWareHouseReportingContext = dataWareHouseReportingContext;
        }

        public SummaryViewModel GetSummary(Guid? storeId)
        {
            var orders =
                storeId == null
                    ? _passioContext.Orders
                        .Include(x => x.Session)
                        .Where(x =>
                            x.Status.Equals(OrderStatus.PAID.GetDisplayName())
                    &&
                    DateTime.Compare((DateTime)x.CheckInDate, Utils.GetCurrentDate().GetStartOfDate()) >= 0 &&
                    DateTime.Compare((DateTime)x.CheckInDate, Utils.GetCurrentDate().GetEndOfDate()) <= 0)
                    : _passioContext.Orders
                        .Include(x => x.Session)
                        .Where(x =>
                            x.Session.StoreId.Equals(storeId) &&
                            x.Status.Equals(OrderStatus.PAID.GetDisplayName())
                            &&
                            DateTime.Compare((DateTime)x.CheckInDate, Utils.GetCurrentDate().GetStartOfDate()) >= 0 &&
                            DateTime.Compare((DateTime)x.CheckInDate, Utils.GetCurrentDate().GetEndOfDate()) <= 0);

            return new SummaryViewModel()
            {
                TotalOrders = (int)orders.Sum(x => x.TotalAmount),
                LastUpdatedTime = Utils.GetCurrentDate()
            };
        }

        public BusinessInsightViewModel GetBusinessInsights(string duration, Guid? storeId)
        {
            (DateTime, DateTime) durationDateTime = Utils.GetPast7Days();
            (DateTime, DateTime) durationPreviousDateTime = Utils.GetPreviousPast7Days();


            #region Get duration

            switch (duration)
            {
                case CommonConstants.Duration.Past7Days:
                    durationDateTime = Utils.GetPast7Days();
                    durationPreviousDateTime = Utils.GetPreviousPast7Days();
                    break;
                case CommonConstants.Duration.Past30Days:
                    durationDateTime = Utils.GetPast30Days();
                    durationPreviousDateTime = Utils.GetPreviousPast30Days();
                    break;
                case CommonConstants.Duration.Past90Days:
                    durationDateTime = Utils.GetPast90Days();
                    durationPreviousDateTime = Utils.GetPreviousPast90Days();
                    break;
                case CommonConstants.Duration.PastMonth:
                    durationDateTime = Utils.GetPastMonth();
                    durationPreviousDateTime = Utils.GetPreviousPastMonth();
                    break;
                case CommonConstants.Duration.PastWeek:
                    durationDateTime = Utils.GetPastWeek();
                    durationPreviousDateTime = Utils.GetPreviousPastWeek();
                    break;
            }

            #endregion


            #region Get reports

            var dateReports = storeId == null
                ? _passioContext.Orders
                    .Where(x => x.Status.Equals(OrderStatus.PAID.GetDisplayName()))
                : _passioContext.Orders
                    .Where(x => x.Status.Equals(OrderStatus.PAID.GetDisplayName()) && x.Session.StoreId.Equals(storeId));

            var dateReportsInDuration = dateReports
                .Where(x =>
                    DateTime.Compare(x.CheckInDate, durationDateTime.Item1) >= 0 &&
                    DateTime.Compare(x.CheckInDate, durationDateTime.Item2) <= 0);

            var dateReportsInPreviousDuration = dateReports
                .Where(x =>
                    DateTime.Compare(x.CheckInDate, durationPreviousDateTime.Item1) >= 0 &&
                    DateTime.Compare(x.CheckInDate, durationPreviousDateTime.Item2) <= 0);

            //var dateProducts = storeId == null
            //    ? _passioContext.DateProducts
            //        .Where(x => x.Status == 1 &&
            //                    DateTime.Compare(x.Date, durationDateTime.Item1) >= 0 &&
            //                    DateTime.Compare(x.Date, durationDateTime.Item2) <= 0)
            //    : _passioContext.DateProducts
            //        .Where(x => x.Status == 1 && x.StoreId == storeId &&
            //                    DateTime.Compare(x.Date, durationDateTime.Item1) >= 0 &&
            //                    DateTime.Compare(x.Date, durationDateTime.Item2) <= 0);

            var dateProducts = storeId == null
                ? _passioContext.Orders
                .Include(x => x.Session).Include(x => x.OrderDetails).ThenInclude(x => x.MenuProduct).ThenInclude(x => x.Product)
                    .Where(x =>
                                DateTime.Compare(x.CheckInDate, durationDateTime.Item1) >= 0 &&
                                DateTime.Compare(x.CheckInDate, durationDateTime.Item2) <= 0)
                : _passioContext.Orders
                .Include(x => x.Session).Include(x => x.OrderDetails).ThenInclude(x => x.MenuProduct).ThenInclude(x => x.Product)
                    .Where(x => x.Session.StoreId.Equals(storeId) &&
                                DateTime.Compare(x.CheckInDate, durationDateTime.Item1) >= 0 &&
                                DateTime.Compare(x.CheckInDate, durationDateTime.Item2) <= 0);

            //List<Models.OrderDetail> myorder = new List<Models.OrderDetail>();

            //foreach(var orderdetail in dateProducts)
            //{
            //    foreach (var item in orderdetail.OrderDetails)
            //    {
            //        myorder.Add(item);
            //    }
            //}

            var myorder = dateProducts.SelectMany(x => x.OrderDetails);

            #endregion

            #region Number of transactions dashboard

            var TotalOrder = storeId == null
                ? _passioContext.Orders.Count()
                : _passioContext.Orders.Where(x => x.Session.StoreId.Equals(storeId)).Count();

            var TotalOrderRID = storeId == null
                ? dateReportsInDuration.Count()
                : dateReportsInDuration.Where(x => x.Session.StoreId.Equals(storeId)).Count();

            var TotalOrderRIPD = storeId == null
                ? dateReportsInPreviousDuration.Count()
                : dateReportsInPreviousDuration.Where(x => x.Session.StoreId.Equals(storeId)).Count();

            var orderInsight = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight
                    ()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Sum(r => r.TotalAmount)
                });

            #endregion


            #region TrendInsights

            var grossSales = new TrendInsight()
            {
                Value = dateReportsInDuration.Sum(x => x.FinalAmount),
                Trend = dateReportsInPreviousDuration.Sum(x => x.FinalAmount) != 0
                    ? (dateReportsInDuration.Sum(x => x.FinalAmount) -
                       dateReportsInPreviousDuration.Sum(x => x.FinalAmount)) /
                    dateReportsInPreviousDuration.Sum(x => x.FinalAmount) * 100
                    : 0
            };

            var netSales = new TrendInsight()
            {
                Value = dateReportsInDuration.Sum(x => x.TotalAmount),
                Trend = dateReportsInPreviousDuration.Sum(x => x.TotalAmount) != 0
                    ? (dateReportsInDuration.Sum(x => x.TotalAmount) -
                       dateReportsInPreviousDuration.Sum(x => x.TotalAmount)) /
                    dateReportsInPreviousDuration.Sum(x => x.TotalAmount) * 100
                    : 0
            };

            var totalTransaction = new TrendInsight()
            {
                Value = TotalOrderRID,
                Trend = TotalOrderRIPD != 0
                    ? (TotalOrderRID -
                       TotalOrderRIPD) /
                    (double)TotalOrderRIPD * 100
                    : 0
            };

            var avgTransactionAmount = new TrendInsight()
            {
                Value =
                    dateReportsInDuration.Count() != 0
                        ? dateReportsInDuration.Sum(x => x.TotalAmount) / TotalOrderRID
                        : 0,
                Trend = (dateReportsInPreviousDuration.Count() != 0
                    ? dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Count()
                    : 0) != 0
                    ? ((dateReportsInDuration.Sum(x => x.TotalAmount) / TotalOrderRID -
                        dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / TotalOrderRIPD) /
                       dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / TotalOrderRIPD) *
                      100
                    : 0
            };

            #endregion


            return new BusinessInsightViewModel()
            {
                TopPerformingStore = storeId == null
                    ? dateReportsInDuration
                        .OrderByDescending(x => x.TotalAmount)
                        .First().Session.Store.Name
                    : "",
                TopSellingItem = myorder
                    .GroupBy(x => new
                    {
                        x.MenuProduct.Product.Id,
                        x.MenuProduct.Product.Name,
                    })
                    .Select(x => new
                    {
                        FinalAmount = x.Sum(d => d.TotalAmount),
                        ProductName = x.Key.Name
                    }).OrderByDescending(x => x.FinalAmount).First().ProductName,
                Orders = orderInsight.ToList(),
                TotalTransaction = totalTransaction,
                GrossSales = grossSales,
                NetSales = netSales,
                // TotalCustomers = ,
                AvgTransactionAmount = avgTransactionAmount
            };
        }
    }
}