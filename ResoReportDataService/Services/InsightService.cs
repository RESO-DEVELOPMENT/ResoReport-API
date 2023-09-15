using System;
using System.Linq;
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
        private readonly DataWareHouseReportingContext _dataWareHouseReportingContext;
        private readonly ProdPassioContext _context;

        public InsightService(DataWareHouseReportingContext dataWareHouseReportingContext, ProdPassioContext context)
        {
            _dataWareHouseReportingContext = dataWareHouseReportingContext;
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
                ? _context.DateReports
                    .Where(x => x.Status == 1 &&
                                DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
                                DateTime.Compare(x.Date, (DateTime)to) <= 0)
                : _context.DateReports
                    .Where(x => x.Status == 1 && x.StoreId == storeId &&
                                DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
                                DateTime.Compare(x.Date, (DateTime)to) <= 0);

            #endregion

            #region TrendInsights

            var totalTransaction = new TrendInsight()
            {
                Value = dateReports.Sum(r => r.TotalOrder),
                Trend = 0
            };

            var grossSales = new TrendInsight()
            {
                Value = dateReports.Sum(x => x.FinalAmount) ?? 0,
                Trend = 0
            };

            var netSales = new TrendInsight()
            {
                Value = dateReports.Sum(x => x.TotalAmount) ?? 0,
                Trend = 0
            };

            var avgTransactionAmount = new TrendInsight()
            {
                Value =
                    dateReports.Count() != 0
                        ? dateReports.Sum(x => x.TotalAmount) / dateReports.Sum(x=>x.TotalOrder)
                        : 0,
                Trend = 0
            };

            #endregion

            #region Dashboard

            var numberOfTransactionsDashboard = dateReports
                .GroupBy(x => new
                {
                    x.Date
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.Date,
                    Value = x.Sum(r => r.TotalOrder)
                });

            var grossSalesDashboard = dateReports
                .GroupBy(x => new
                {
                    x.Date
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.Date,
                    Value = x.Sum(r => r.FinalAmount)
                });

            var netSalesDashboard = dateReports
                .GroupBy(x => new
                {
                    x.Date
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.Date,
                    Value = x.Sum(r => r.TotalAmount)
                });

            var avgTransactionsDashboard = dateReports
                .GroupBy(x => new
                {
                    x.Date
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.Date,
                    Value = x.Sum(r => r.TotalAmount) / x.Sum(r => r.TotalOrder)
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


        private SalesInsightViewModel GetSalesInsightsInDuration(string duration, int? storeId)
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

            #region Get reports

            var dateReports = storeId == null
                ? _context.DateReports
                    .Where(x => x.Status == 1)
                : _context.DateReports
                    .Where(x => x.Status == 1 && x.StoreId == storeId);

            var dateReportsInDuration = dateReports
                .Where(x =>
                    DateTime.Compare(x.Date, durationDateTime.Item1) >= 0 &&
                    DateTime.Compare(x.Date, durationDateTime.Item2) <= 0);

            var dateReportsInPreviousDuration = dateReports
                .Where(x =>
                    DateTime.Compare(x.Date, durationPreviousDateTime.Item1) >= 0 &&
                    DateTime.Compare(x.Date, durationPreviousDateTime.Item2) <= 0);

            #endregion

            #region TrendInsights

            var totalTransaction = new TrendInsight()
            {
                Value = dateReportsInDuration.Sum(r => r.TotalOrder),
                Trend = dateReportsInPreviousDuration.Sum(r => r.TotalOrder) != 0
                    ? (dateReportsInDuration.Sum(r => r.TotalOrder) -
                       dateReportsInPreviousDuration.Sum(r => r.TotalOrder)) /
                    (double)dateReportsInPreviousDuration.Sum(r => r.TotalOrder) * 100
                    : 0
            };

            var grossSales = new TrendInsight()
            {
                Value = dateReportsInDuration.Sum(x => x.FinalAmount) ?? 0,
                Trend = dateReportsInPreviousDuration.Sum(x => x.FinalAmount) != 0
                    ? (dateReportsInDuration.Sum(x => x.FinalAmount) -
                       dateReportsInPreviousDuration.Sum(x => x.FinalAmount)) /
                    dateReportsInPreviousDuration.Sum(x => x.FinalAmount) * 100
                    : 0
            };

            var netSales = new TrendInsight()
            {
                Value = dateReportsInDuration.Sum(x => x.TotalAmount) ?? 0,
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
                        ? dateReportsInDuration.Sum(x => x.TotalAmount) / dateReportsInDuration.Sum(x=>x.TotalOrder)
                        : 0,
                Trend = (dateReportsInPreviousDuration.Sum(x=>x.TotalOrder) != 0
                    ? dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Sum(x=>x.TotalOrder)
                    : 0) != 0
                    ? ((dateReportsInDuration.Sum(x => x.TotalAmount) / dateReportsInDuration.Sum(x=>x.TotalOrder) -
                        dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Sum(x=>x.TotalOrder)) /
                       dateReportsInPreviousDuration.Sum(x => x.TotalAmount) / dateReportsInPreviousDuration.Sum(x=>x.TotalOrder)) *
                      100
                    : 0
            };

            #endregion

            #region Dashboard

            var numberOfTransactionsDashboard = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.Date
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.Date,
                    Value = x.Sum(r => r.TotalOrder)
                });

            var grossSalesDashboard = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.Date
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.Date,
                    Value = x.Sum(r => r.FinalAmount)
                });

            var netSalesDashboard = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.Date
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.Date,
                    Value = x.Sum(r => r.TotalAmount)
                });

            var avgTransactionsDashboard = dateReportsInDuration
                .GroupBy(x => new
                {
                    x.Date
                })
                .Select(x => new DashboardInsight()
                {
                    Date = x.Key.Date,
                    Value = x.Sum(r => r.TotalAmount) / x.Sum(r => r.TotalOrder)
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