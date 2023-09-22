using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Reso.Sdk.Core.Custom;
using ResoReportDataAccess.Models;
using ResoReportDataService.Commons;
using ResoReportDataService.Models;
using ResoReportDataService.RequestModels;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.Services
{
    public interface IInsightService
    {
        SalesInsightViewModel GetSalesInsights(DateFilter dateFilter, string duration, int? storeId);
    }

    public class InsightService : IInsightService
    {
        private readonly PosSystemContext _context;

        public InsightService(PosSystemContext context)
        {
            _context = context;
        }


        public SalesInsightViewModel GetSalesInsights(DateFilter dateFilter, string duration, int? storeId)
        {
            #region Check Date range

            var from = dateFilter?.FromDate;
            var to = dateFilter?.ToDate;

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

            if (!string.IsNullOrEmpty(duration))
            {
                return GetSalesInsightsInDuration(duration, storeId);
            }

            #region Get reports

            var dateReports = storeId == null
                ? _context.Orders
                    .Where(x => x.Status.Equals(OrderStatus.PAID.ToString()) &&
                                DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                                DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0)
                : _context.Orders
                    .Where(x => x.Status.Equals(OrderStatus.PAID.ToString()) && x.Session.StoreId.Equals(storeId) 
                    && DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                                DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0);

            #endregion

            #region TrendInsights

            var totalTransaction = new TrendInsight()
            {
                Value = dateReports.Count(),
                Trend = 0
            };

            var grossSales = new TrendInsight()
            {
                Value = dateReports.Sum(x => x.FinalAmount),
                Trend = 0
            };

            var netSales = new TrendInsight()
            {
                Value = dateReports.Sum(x => x.TotalAmount),
                Trend = 0
            };

            var avgTransactionAmount = new TrendInsight()
            {
                Value =
                    dateReports.Count() != 0
                        ? dateReports.Sum(x => x.TotalAmount) / dateReports.Count()
                        : 0,
                Trend = 0
            };

            #endregion

            #region Dashboard

            var numberOfTransactionsDashboard = dateReports
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Count()
                });

            var grossSalesDashboard = dateReports
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Sum(r => r.FinalAmount)
                });

            var netSalesDashboard = dateReports
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Sum(r => r.TotalAmount)
                });

            var avgTransactionsDashboard = dateReports
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Sum(r => r.TotalAmount) / x.Count()
                });

            #endregion

            return new SalesInsightViewModel()
            {
                GrossSales = grossSales,
                NetSales = netSales,
                TotalOrders = totalTransaction,
                AverageTransactionAmount = avgTransactionAmount,
                GrossSalesDashboard = grossSalesDashboard.ToList(),
                NetSalesDashboard = netSalesDashboard.ToList(),
                AvgTransactionAmountDashboard = avgTransactionsDashboard.ToList(),
                NumberOfTransactionsDashboard = numberOfTransactionsDashboard.ToList()
            };
        }


        private SalesInsightViewModel GetSalesInsightsInDuration(string duration, int? storeid)
        {
            #region Get duration

            (DateTime, DateTime) durationDateTime = Utils.GetPast7Days();
            (DateTime, DateTime) durationPreviousDateTime = Utils.GetPreviousPast7Days();
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

            #region get reports

            var dateReports = storeid == null
                ? _context.Orders
                    .Where(x => x.Status.Equals(OrderStatus.PAID))
                : _context.Orders
                    .Where(x => x.Status.Equals(OrderStatus.PAID) && x.Session.StoreId.Equals(storeid));

            var dateReportsInDuration = dateReports
                .Where(x =>
                    DateTime.Compare(x.CheckInDate, durationDateTime.Item1) >= 0 &&
                    DateTime.Compare(x.CheckInDate, durationDateTime.Item2) <= 0);

            var dateReportsInPreviousDuration = dateReports
                .Where(x =>
                    DateTime.Compare(x.CheckInDate, durationPreviousDateTime.Item1) >= 0 &&
                    DateTime.Compare(x.CheckInDate, durationPreviousDateTime.Item2) <= 0);

            #endregion

            #region TrendInsights

            var totalTransaction = new TrendInsight()
            {
                Value = dateReportsInDuration.Count(),
                Trend = dateReportsInPreviousDuration.Count() != 0
                    ? (dateReportsInDuration.Count() -
                       dateReportsInPreviousDuration.Count()) /
                    (double)dateReportsInPreviousDuration.Count() * 100
                    : 0
            };

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

            var avgTransactionAmount = new TrendInsight()
            {
                Value =
                    dateReportsInDuration.Count() != 0
                        ? dateReportsInDuration.Sum(x => x.TotalAmount) / dateReportsInDuration.Count()
                        : 0,
                Trend = (dateReportsInPreviousDuration.Count() != 0
                    ? dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Count()
                    : 0) != 0
                    ? ((dateReportsInDuration.Sum(x => x.TotalAmount) / dateReportsInDuration.Count() -
                        dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Count()) /
                       dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Count()) *
                      100
                    : 0
            };

            #endregion

            #region Dashboard

            var numberOfTransactionsDashboard = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Count()
                });

            var grossSalesDashboard = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Sum(r => r.FinalAmount)
                });

            var netSalesDashboard = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Sum(r => r.TotalAmount)
                });

            var avgTransactionsDashboard = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.CheckInDate
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.CheckInDate,
                    Value = x.Sum(r => r.TotalAmount) / x.Count()
                });

            #endregion

            return new SalesInsightViewModel()
            {
                GrossSales = grossSales,
                NetSales = netSales,
                TotalOrders = totalTransaction,
                AverageTransactionAmount = avgTransactionAmount,
                GrossSalesDashboard = grossSalesDashboard.ToList(),
                NetSalesDashboard = netSalesDashboard.ToList(),
                AvgTransactionAmountDashboard = avgTransactionsDashboard.ToList(),
                NumberOfTransactionsDashboard = numberOfTransactionsDashboard.ToList()
            };
        }
    }
}