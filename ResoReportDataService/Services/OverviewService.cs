//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Net.WebSockets;
//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Reso.Sdk.Core.Custom;
//using Reso.Sdk.Core.Utilities;
//using ResoReportDataAccess.Models;
//using ResoReportDataService.Commons;
//using ResoReportDataService.Commons.ExcelUtils;
//using ResoReportDataService.Models;
//using ResoReportDataService.RequestModels;
//using ResoReportDataService.ViewModels;

//namespace ResoReportDataService.Services
//{
//    public interface IOverviewService
//    {
//        BaseResponsePagingViewModel<TopStoreRevenueViewModel> GetTopStoreRevenueWithPaging(int? storeId, int brandId,
//            DateRange dateRange, PagingModel paging, TopStoreRevenueViewModel modelFilter);

//        FileStreamResult ExportTopStoreRevenue(DateRange dateRange, int? storeId);

//        OverviewDashboard GetOverviewDashboard(DateFilter filter, int brandId, int? storeId);
//        OverviewPaymentDashboard GetOverviewPaymentDashboard(DateFilter filter, int brandId, int? storeId);
//    }

//    public class OverviewService : IOverviewService
//    {
//        private readonly DataWareHouseReportingContext _context;
//        private readonly ProdPassioContext _passioContext;
//        private readonly IMapper _mapper;

//        public OverviewService(DataWareHouseReportingContext context, IMapper mapper, ProdPassioContext passioContext)
//        {
//            _context = context;
//            _mapper = mapper;
//            _passioContext = passioContext;
//        }

//        public OverviewDashboard GetOverviewDashboard(DateFilter filter, int brandId, int? storeId)
//        {
//            #region Check Date range

//            var from = filter?.FromDate;
//            var to = filter?.ToDate;

//            if (from == null && to == null)
//            {
//                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
//                to = Utils.GetLastAndFirstDateInCurrentMonth().Item2;
//            }

//            from ??= Utils.GetCurrentDate();
//            to ??= Utils.GetCurrentDate();

//            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
//            {
//                throw new ErrorResponse(400, "The datetime is invalid!");
//            }

//            from = ((DateTime)from).GetStartOfDate();
//            to = ((DateTime)to).GetEndOfDate();

//            #endregion

//            //OverviewDashboard result = new OverviewDashboard();
//            //var dateReport =
//            //    storeId != null
//            //        ? _context.DateReports
//            //            .Where(x => x.StoreId == storeId && x.Active == true &&
//            //                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//            //                        DateTime.Compare(x.Date, (DateTime)to) <= 0)
//            //        : _context.DateReports
//            //            .Where(x => x.Active == true &&
//            //                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//            //                        DateTime.Compare(x.Date, (DateTime)to) <= 0);

//            //var dateProduct =
//            //    storeId != null
//            //        ? _context.DateProducts
//            //            .Where(x => x.StoreId == storeId && x.Active == true &&
//            //                        x.ProductType != (int?)ProductTypeEnum.CardPayment &&
//            //                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//            //                        DateTime.Compare(x.Date, (DateTime)to) <= 0)
//            //        : _context.DateProducts
//            //            .Where(x => x.Active == true &&
//            //                        x.ProductType != (int?)ProductTypeEnum.CardPayment &&
//            //                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//            //                        DateTime.Compare(x.Date, (DateTime)to) <= 0);

//            OverviewDashboard result = new OverviewDashboard();

//            bool isToday = (DateTime)from <= Utils.GetCurrentDate() && (DateTime)to >= Utils.GetCurrentDate();

//            //Check DateTime Data is Today
//            if (isToday)
//            {
//                //Take orders made today
//                var rawTodayOrders =
//                    storeId != null ?
//                    _passioContext.Orders
//                    .Where(x => x.StoreId == storeId && x.OrderType != (int)OrderTypeEnum.DropProduct &&
//                                (x.OrderStatus == (int)OrderStatusEnum.Finish ||
//                                 x.OrderStatus == (int)OrderStatusEnum.Cancel ||
//                                 x.OrderStatus == (int)OrderStatusEnum.PreCancel) &&
//                                 DateTime.Compare((DateTime)x.CheckInDate, (DateTime)from) >= 0 &&
//                                 DateTime.Compare((DateTime)x.CheckInDate, (DateTime)to) <= 0)

//                    :
//                    _passioContext.Orders
//                    .Where(x => x.OrderType != (int)OrderTypeEnum.DropProduct &&
//                                (x.OrderStatus == (int)OrderStatusEnum.Finish ||
//                                 x.OrderStatus == (int)OrderStatusEnum.Cancel ||
//                                 x.OrderStatus == (int)OrderStatusEnum.PreCancel) &&
//                                 DateTime.Compare((DateTime)x.CheckInDate, (DateTime)from) >= 0 &&
//                                 DateTime.Compare((DateTime)x.CheckInDate, (DateTime)to) <= 0);

//                var todayOrders = rawTodayOrders.Select(x => new
//                {
//                    x.Att1,
//                    x.OrderStatus,
//                    x.OrderType,
//                    x.TotalAmount,
//                    x.FinalAmount,
//                    x.OrderDetailsTotalQuantity
//                }).ToList();

//                var FinishOrders = todayOrders.Where(x => x.OrderStatus == (int)OrderStatusEnum.Finish);
//                var CancleAndPreCancleOrders = todayOrders
//                    .Where(x => x.OrderStatus == (int)OrderStatusEnum.Cancel ||
//                                x.OrderStatus == (int)OrderStatusEnum.PreCancel);


//                //Tổng số hóa đơn
//                result.TotalOrder = FinishOrders.Count(x => x.OrderType != (int)OrderTypeEnum.OrderCard);
//                result.TotalOrderAtStore = FinishOrders.Count(x => x.OrderType == (int)OrderTypeEnum.AtStore);
//                result.TotalOrderDelivery = FinishOrders.Count(x => x.OrderType == (int)OrderTypeEnum.Delivery);
//                result.TotalOrderTakeAway = FinishOrders.Count(x => x.OrderType == (int)OrderTypeEnum.TakeAway);
//                result.TotalOrderCard = FinishOrders.Count(x => x.OrderType == (int)OrderTypeEnum.OrderCard);

//                result.TotalOrderAfterCancel = CancleAndPreCancleOrders.Count(x => x.OrderStatus == (int)OrderStatusEnum.Cancel);
//                result.TotalOrderPreCancel = CancleAndPreCancleOrders.Count(x => x.OrderStatus == (int)OrderStatusEnum.PreCancel);

//                //Tổng doanh thu trước giảm giá
//                result.TotalRevenueWithDiscount = FinishOrders.Where(x => x.OrderType != (int)OrderTypeEnum.OrderCard).Sum(x => x.TotalAmount);

//                //Tổng doanh thu thực tế
//                result.TotalRevenueWithoutDiscountAndCard = FinishOrders.Where(x => x.OrderType != (int)OrderTypeEnum.OrderCard).Sum(x => x.FinalAmount);

//                //Tổng giảm giá 
//                result.TotalDiscount = result.TotalRevenueWithDiscount - result.TotalRevenueWithoutDiscountAndCard;

//                //Tổng doanh thu
//                result.TotalRevenue = result.TotalRevenueWithoutDiscountAndCard;

//                //Tổng doanh thu nạp thẻ
//                result.TotalRevenueOrderCard = (int)FinishOrders.Where(x => x.OrderType == (int)OrderTypeEnum.OrderCard).Sum(x => x.FinalAmount);

//                result.TotalRevenueAtStore = (int)FinishOrders.Where(x => x.OrderType == (int)OrderTypeEnum.AtStore).Sum(x => x.FinalAmount);
//                result.TotalRevenueDelivery = (int)FinishOrders.Where(x => x.OrderType == (int)OrderTypeEnum.Delivery).Sum(x => x.FinalAmount);
//                result.TotalRevenueTakeAway = (int)FinishOrders.Where(x => x.OrderType == (int)OrderTypeEnum.TakeAway).Sum(x => x.FinalAmount);
//                result.TotalRevenuePreCancel = (int)CancleAndPreCancleOrders.Where(x => x.OrderStatus == (int)OrderStatusEnum.PreCancel).Sum(x => x.FinalAmount);
//                result.TotalRevenueAfterCancel = (int)CancleAndPreCancleOrders.Where(x => x.OrderStatus == (int)OrderStatusEnum.Cancel).Sum(x => x.FinalAmount);

//                result.AvgRevenueOrder = result.TotalOrder == 0 ? 0 : result.TotalRevenueWithoutDiscountAndCard / result.TotalOrder;
//                result.AvgProductOrder = result.TotalOrder == 0 ? 0 : Math.Round((double)FinishOrders.Sum(q => q.OrderDetailsTotalQuantity ?? 0) / result.TotalOrder, 2);
//                result.AvgProductOrderAtStore = result.TotalOrderAtStore == 0 ? 0 : Math.Round((double)FinishOrders.Where(q => q.OrderType == (int)OrderTypeEnum.AtStore).Sum(q => q.OrderDetailsTotalQuantity ?? 0) / result.TotalOrderAtStore, 2);
//                result.AvgProductOrderTakeAway = result.TotalOrderTakeAway == 0 ? 0 : Math.Round((double)FinishOrders.Where(q => q.OrderType == (int)OrderTypeEnum.TakeAway).Sum(q => q.OrderDetailsTotalQuantity ?? 0) / result.TotalOrderTakeAway, 2);
//                result.AvgProductOrderDelivery = result.TotalOrderDelivery == 0 ? 0 : Math.Round((double)FinishOrders.Where(q => q.OrderType == (int)OrderTypeEnum.Delivery).Sum(q => q.OrderDetailsTotalQuantity ?? 0) / result.TotalOrderDelivery, 2);

//                result.AvgOrderAtStore = result.TotalOrderAtStore == 0
//                        ? 0
//                        : Math.Round((double)result.TotalRevenueAtStore / result.TotalOrderAtStore, 0);
//                result.AvgOrderDelivery = result.TotalOrderDelivery == 0
//                    ? 0
//                    : Math.Round((double)result.TotalRevenueDelivery / result.TotalOrderDelivery, 0);
//                result.AvgOrderTakeAway = result.TotalOrderTakeAway == 0
//                    ? 0
//                    : Math.Round((double)result.TotalRevenueTakeAway / result.TotalOrderTakeAway, 0);
//            }
//            else
//            {
//                var dateReport =
//                storeId != null
//                    ? _passioContext.DateReports
//                        .Where(x => x.StoreId == storeId && x.Status == 1 &&
//                                    DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                    DateTime.Compare(x.Date, (DateTime)to) <= 0)
//                    : _passioContext.DateReports
//                        .Where(x => x.Status == 1 &&
//                                    DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                    DateTime.Compare(x.Date, (DateTime)to) <= 0);

//                var dateProduct =
//                    storeId != null
//                        ? _passioContext.DateProducts
//                            .Where(x => x.StoreId == storeId &&
//                                        x.CategoryId != (int?)ProductTypeEnum.CardPayment &&
//                                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                        DateTime.Compare(x.Date, (DateTime)to) <= 0)
//                        : _passioContext.DateProducts
//                            .Where(x =>
//                                        x.CategoryId != (int?)ProductTypeEnum.CardPayment &&
//                                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                        DateTime.Compare(x.Date, (DateTime)to) <= 0);
//                if (dateReport.Any())
//                {
//                    result.TotalOrder = dateReport.Sum(q => q.TotalOrder) - dateReport.Sum(q => q.TotalOrderCard);
//                    result.TotalOrderDelivery = dateReport.Sum(q => q.TotalOrderDelivery);
//                    result.TotalOrderAtStore = dateReport.Sum(q => q.TotalOrderAtStore);
//                    result.TotalOrderTakeAway = dateReport.Sum(q => q.TotalOrderTakeAway);
//                    result.TotalOrderCard = dateReport.Sum(q => q.TotalOrderCard);
//                    result.TotalOrderAfterCancel = dateReport.Where(q => q.TotalOrderCanceled.HasValue)
//                        .Sum(q => q.TotalOrderCanceled.Value);
//                    result.TotalOrderPreCancel = dateReport.Where(q => q.TotalOrderPreCanceled.HasValue)
//                        .Sum(q => q.TotalOrderPreCanceled.Value);
//                    result.TotalOrderCancel = result.TotalOrderAfterCancel + result.TotalOrderPreCancel;

//                    result.TotalRevenueOrderCard = (int)dateReport.Sum(q => q.FinalAmountCard.GetValueOrDefault());
//                    result.TotalRevenueWithDiscount = dateReport.Sum(q => q.TotalAmount.GetValueOrDefault()) -
//                                                      result.TotalRevenueOrderCard;
//                    result.TotalRevenueWithoutDiscountAndCard = dateReport.Sum(q => q.FinalAmount.GetValueOrDefault()) -
//                                                                result.TotalRevenueOrderCard;
//                    result.TotalRevenue = result.TotalRevenueWithoutDiscountAndCard;
//                    result.TotalDiscount = result.TotalRevenueWithDiscount - result.TotalRevenueWithoutDiscountAndCard;

//                    result.TotalRevenueAtStore = (int)dateReport.Sum(q => q.FinalAmountAtStore.GetValueOrDefault());
//                    result.TotalRevenueTakeAway = (int)dateReport.Sum(q => q.FinalAmountTakeAway.GetValueOrDefault());
//                    result.TotalRevenueDelivery = (int)dateReport.Sum(q => q.FinalAmountDelivery.GetValueOrDefault());
//                    result.TotalRevenuePreCancel = dateReport.Sum(q => q.FinalAmountPreCanceled.GetValueOrDefault());
//                    result.TotalRevenueAfterCancel = dateReport.Sum(q => q.FinalAmountCanceled.GetValueOrDefault());

//                    result.AvgRevenueOrder = result.TotalOrder == 0
//                        ? 0
//                        : result.TotalRevenueWithoutDiscountAndCard / result.TotalOrder;
//                    if (dateProduct.Any())
//                    {
//                        result.AvgProductOrder = result.TotalOrder == 0
//                            ? 0
//                            : Math.Round(
//                                (double)(dateProduct?.Sum(q => q.OrderQuantity.GetValueOrDefault()) / result.TotalOrder),
//                                2);
//                        result.AvgProductOrderAtStore = result.TotalOrderAtStore == 0
//                            ? 0
//                            : Math.Round(
//                                (double)(dateProduct?.Sum(q => q.QuantityAtStore.GetValueOrDefault()) /
//                                         result.TotalOrderAtStore), 2);
//                        result.AvgProductOrderTakeAway = result.TotalOrderTakeAway == 0
//                            ? 0
//                            : Math.Round(
//                                (double)(dateProduct?.Sum(q => q.QuantityTakeAway.GetValueOrDefault()) /
//                                         result.TotalOrderTakeAway), 2);
//                        result.AvgProductOrderDelivery = result.TotalOrderDelivery == 0
//                            ? 0
//                            : Math.Round(
//                                (double)(dateProduct?.Sum(q => q.QuantityDelivery.GetValueOrDefault()) /
//                                         result.TotalOrderDelivery), 2);
//                    }

//                    result.AvgOrderAtStore = result.TotalOrderAtStore == 0
//                        ? 0
//                        : Math.Round((double)result.TotalRevenueAtStore / result.TotalOrderAtStore, 0);
//                    result.AvgOrderDelivery = result.TotalOrderDelivery == 0
//                        ? 0
//                        : Math.Round((double)result.TotalRevenueDelivery / result.TotalOrderDelivery, 0);
//                    result.AvgOrderTakeAway = result.TotalOrderTakeAway == 0
//                        ? 0
//                        : Math.Round((double)result.TotalRevenueTakeAway / result.TotalOrderTakeAway, 0);
//                }
//            }

//            //Take the OrderPassio100
//            var listOrderPassio100 =
//                storeId != null ?
//                _passioContext.Orders.Where(x => x.StoreId == storeId &&
//                                                DateTime.Compare((DateTime)x.CheckInDate, (DateTime)from) >= 0 &&
//                                                DateTime.Compare((DateTime)x.CheckInDate, (DateTime)to) <= 0 &&
//                                                x.OrderStatus == (int)OrderStatusEnum.Finish &&
//                                                !string.IsNullOrEmpty(x.Att1) && x.Att1.Contains("passio-100"))
//                :
//                _passioContext.Orders.Where(x => DateTime.Compare((DateTime)x.CheckInDate, (DateTime)from) >= 0 &&
//                                                DateTime.Compare((DateTime)x.CheckInDate, (DateTime)to) <= 0 &&
//                                                x.OrderStatus == (int)OrderStatusEnum.Finish &&
//                                                !string.IsNullOrEmpty(x.Att1) && x.Att1.Contains("passio-100"));


//            if (listOrderPassio100.Count() > 0)
//            {
//                result.TotalDiscount100 = listOrderPassio100.Sum(x => x.TotalAmount);
//            }
//            else
//            {
//                result.TotalDiscount100 = 0;
//            }

//            return result;
//        }

//        public OverviewPaymentDashboard GetOverviewPaymentDashboard(DateFilter filter, int brandId, int? storeId)
//        {
//            #region Check Date range

//            var from = filter?.FromDate;
//            var to = filter?.ToDate;

//            if (from == null && to == null)
//            {
//                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
//                to = Utils.GetLastAndFirstDateInCurrentMonth().Item2;
//            }

//            from ??= Utils.GetCurrentDate();
//            to ??= Utils.GetCurrentDate();

//            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
//            {
//                throw new ErrorResponse(400, "The datetime is invalid!");
//            }

//            from = ((DateTime)from).GetStartOfDate();
//            to = ((DateTime)to).GetEndOfDate();

//            #endregion

//            OverviewPaymentDashboard resultPayment = new OverviewPaymentDashboard();

//            //var dateReport =
//            //    storeId != null
//            //        ? _context.DateReports
//            //            .Where(x => x.StoreId == storeId && x.Active == true &&
//            //                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//            //                        DateTime.Compare(x.Date, (DateTime)to) <= 0)
//            //        : _context.DateReports
//            //            .Where(x => x.Active == true &&
//            //                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//            //                        DateTime.Compare(x.Date, (DateTime)to) <= 0);

//            //var paymentReport =
//            //    storeId != null
//            //        ? _context.PaymentReports
//            //            .Where(x => x.StoreId == storeId && x.Active == true &&
//            //                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//            //                        DateTime.Compare(x.Date, (DateTime)to) <= 0)
//            //        : _context.PaymentReports
//            //            .Where(x => x.Active == true &&
//            //                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//            //                        DateTime.Compare(x.Date, (DateTime)to) <= 0);

//            var dateReport =
//              storeId != null
//                  ? _passioContext.DateReports
//                      .Where(x => x.StoreId == storeId && x.Status == 1 &&
//                                  DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                  DateTime.Compare(x.Date, (DateTime)to) <= 0)
//                  : _passioContext.DateReports
//                      .Where(x => x.Status == 1 &&
//                                  DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                  DateTime.Compare(x.Date, (DateTime)to) <= 0);

//            var paymentReport =
//                storeId != null
//                    ? _passioContext.PaymentReports
//                        .Where(x => x.StoreId == storeId && x.Status == 1 &&
//                                    DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                    DateTime.Compare(x.Date, (DateTime)to) <= 0)
//                    : _passioContext.PaymentReports
//                        .Where(x => x.Status == 1 &&
//                                    DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                    DateTime.Compare(x.Date, (DateTime)to) <= 0)
//                                    ;
//            ;

//            var orderPayments =
//                storeId != null
//                ? _passioContext.Orders
//                .Where(x => x.StoreId == storeId &&
//                            x.OrderStatus == (int)OrderStatusEnum.Finish &&
//                            DateTime.Compare((DateTime)x.CheckInDate, (DateTime)from) >= 0 &&
//                            DateTime.Compare((DateTime)x.CheckInDate, (DateTime)to) <= 0)
//                .Include(x => x.Payments)
//                : _passioContext.Orders
//                .Where(x => x.OrderStatus == (int)OrderStatusEnum.Finish &&
//                            DateTime.Compare((DateTime)x.CheckInDate, (DateTime)from) >= 0 &&
//                            DateTime.Compare((DateTime)x.CheckInDate, (DateTime)to) <= 0)
//                .Include(x => x.Payments);

//            #region Tổng lượt thanh toán

//            #region OldCode
//            ////Số lượt thanh toán tiền mặt nạp thẻ                
//            //resultPayment.TotalTransactionPaymentCard = dateReport.Sum(x => x.TotalOrderCard);

//            ////Số lượt thanh toán tiền mặt bán hàng
//            //resultPayment.TotalTransactionPaymentForSales = (paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.Cash)
//            //    .Sum(x => x.TotalTransaction) - resultPayment.TotalTransactionPaymentCard) ?? 0;

//            ////Số lượt thanh toán thẻ thành viên bán hàng
//            //resultPayment.TotalTransactionPaymentBuyCard = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.MemberPayment)
//            //    .Sum(x => x.TotalTransaction) ?? 0;

//            ////Số lượt thanh toán thanh toán ví điện tử, momo, grabpay, grabfood, baemin        
//            //resultPayment.TotalTransactionPaymentE_Wallet_Momo = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.MoMo)
//            //    .Sum(x => x.TotalTransaction) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_GrabPay = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.GrabPay)
//            //    .Sum(x => x.TotalTransaction) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_GrabFood = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.GrabFood)
//            //    .Sum(x => x.TotalTransaction) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_VnPay = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.VNPay)
//            //    .Sum(x => x.TotalTransaction) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_Baemin = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.BaeMin)
//            //    .Sum(x => x.TotalTransaction) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_Shopeepay = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.Shopeepay)
//            //    .Sum(x => x.TotalTransaction) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_ZaloPay = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.ZaloPay)
//            //    .Sum(x => x.TotalTransaction) ?? 0;

//            //resultPayment.TotalTransactionPaymentE_Wallet = resultPayment.TotalTransactionPaymentE_Wallet_Momo
//            //                                                + resultPayment.TotalTransactionPaymentE_Wallet_GrabPay
//            //                                                + resultPayment.TotalTransactionPaymentE_Wallet_GrabFood
//            //                                                + resultPayment.TotalTransactionPaymentE_Wallet_VnPay
//            //                                                + resultPayment.TotalTransactionPaymentE_Wallet_Baemin
//            //                                                + resultPayment.TotalTransactionPaymentE_Wallet_Shopeepay
//            //                                                + resultPayment.TotalTransactionPaymentE_Wallet_ZaloPay;

//            ////Số lượt thanh toán thanh toán ngân hàng
//            //resultPayment.TotalTransactionPaymentBank = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.VisaCard ||
//            //                x.PaymentType == (int)PaymentTypeEnum.MasterCard)
//            //    .Sum(x => x.TotalTransaction) ?? 0;
//            ////Số lượt thanh toán thanh toán khác
//            //resultPayment.TotalTransactionPaymentOther = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.Other)
//            //    .Sum(x => x.TotalTransaction) ?? 0;

//            //resultPayment.TotalTransactionPayment = (int)(resultPayment.TotalTransactionPaymentE_Wallet +
//            //                                              resultPayment.TotalTransactionPaymentOther +
//            //                                              resultPayment.TotalTransactionPaymentBank +
//            //                                              resultPayment.TotalTransactionPaymentForSales +
//            //                                              resultPayment.TotalTransactionPaymentCard +
//            //                                              resultPayment.TotalTransactionPaymentBuyCard);

//            //#endregion


//            //#region MyRegion

//            ////Tổng thanh toán tiền mặt nạp thẻ
//            //resultPayment.TotalPaymentCard = dateReport.Sum(x => x.FinalAmountCard) ?? 0;

//            ////Tổng thanh toán tiền mặt bán hàng
//            //resultPayment.TotalPaymentForSales = (paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.Cash ||
//            //                x.PaymentType == (int)PaymentTypeEnum.ExchangeCash)
//            //    .Sum(x => x.Amount) - dateReport.Sum(x => x.FinalAmountCard)) ?? 0;

//            ////Tổng thanh toán thẻ thành viên bán hàng
//            //resultPayment.TotalPaymentBuyCard = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.MemberPayment)
//            //    .Sum(x => x.Amount);

//            ////Tổng thanh toán thanh toán ví điện tử, momo, gift talk, grabpay, grabfood
//            //resultPayment.TotalPaymentE_Wallet_Momo = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.MoMo)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_GrabPay = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.GrabPay)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_GrabFood = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.GrabFood)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_VNPay = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.VNPay)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_Baemin = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.BaeMin)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_Shopeepay = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.Shopeepay)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_ZaloPay = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.ZaloPay)
//            //    .Sum(x => x.Amount);

//            //resultPayment.TotalPaymentE_Wallet = resultPayment.TotalPaymentE_Wallet_Momo
//            //                                     + resultPayment.TotalPaymentE_Wallet_GrabPay
//            //                                     + resultPayment.TotalPaymentE_Wallet_GrabFood
//            //                                     + resultPayment.TotalPaymentE_Wallet_VNPay
//            //                                     + resultPayment.TotalPaymentE_Wallet_Baemin
//            //                                     + resultPayment.TotalPaymentE_Wallet_Shopeepay
//            //                                     + resultPayment.TotalPaymentE_Wallet_ZaloPay;


//            ////Tổng thanh toán ngân hàng
//            //resultPayment.TotalPaymentBank = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.MasterCard ||
//            //                x.PaymentType == (int)PaymentTypeEnum.VisaCard)
//            //    .Sum(x => x.Amount);
//            ////Tổng thanh toán khác
//            //resultPayment.TotalPaymentOther = paymentReport
//            //    .Where(x => x.PaymentType == (int)PaymentTypeEnum.Other)
//            //    .Sum(x => x.Amount);

//            //resultPayment.TotalPayment = resultPayment.TotalPaymentForSales
//            //                             + resultPayment.TotalPaymentCard
//            //                             + resultPayment.TotalPaymentBuyCard
//            //                             + resultPayment.TotalPaymentE_Wallet
//            //                             + resultPayment.TotalPaymentBank
//            //                             + resultPayment.TotalPaymentOther;
//            #endregion

//            //Số lượt thanh toán tiền mặt nạp thẻ                
//            resultPayment.TotalTransactionPaymentCard = orderPayments.Where(x => x.OrderType == (int)OrderTypeEnum.OrderCard)
//                             .SelectMany(x => x.Payments)
//                             .Count(x => x.Type == (int)PaymentTypeEnum.Cash);

//            //Số lượt thanh toán tiền mặt bán hàng
//            //resultPayment.TotalTransactionPaymentForSales = ((int?)paymentReport
//            //    .Where(x => x.CashAmount > 0)
//            //    .Sum(x => x.CashAmount) - resultPayment.TotalTransactionPaymentCard) ?? 0;
//            resultPayment.TotalTransactionPaymentForSales =
//                orderPayments.Where(x => x.OrderType != (int)OrderTypeEnum.OrderCard)
//                             .SelectMany(x => x.Payments)
//                             .Count(x => x.Type == (int)PaymentTypeEnum.Cash);

//            //Số lượt thanh toán thẻ thành viên bán hàng
//            //resultPayment.TotalTransactionPaymentBuyCard = (int?)paymentReport  
//            //    .Where(x => x.MemberCardAmount > 0)
//            //    .Sum(x => x.MemberCardAmount) ?? 0;
//            resultPayment.TotalTransactionPaymentBuyCard =
//                orderPayments.SelectMany(x => x.Payments)
//                             .Count(x => x.Type == (int)PaymentTypeEnum.MemberPayment);


//            //Số lượt thanh toán thanh toán ví điện tử, momo, grabpay, grabfood, baemin
//            //var paymentTypeReport = paymentReport
//            //                        .Join(_passioContext.Payments,
//            //                        report => report.Date,
//            //                        payment => payment.PayTime,
//            //                        (report, payment) => new
//            //                        {
//            //                            Amount = payment.Amount,
//            //                            Type = payment.Type
//            //                        });

//            //resultPayment.TotalTransactionPaymentE_Wallet_Momo = (int?)paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.MoMo)
//            //    .Sum(x => x.Amount) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_GrabPay = (int?)paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.GrabPay)
//            //    .Sum(x => x.Amount) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_GrabFood = (int?)paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.GrabFood)
//            //    .Sum(x => x.Amount) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_VnPay = (int?)paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.VNPay)
//            //    .Sum(x => x.Amount) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_Baemin = (int?)paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.BaeMin)
//            //    .Sum(x => x.Amount) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_Shopeepay = (int?)paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.Shopeepay)
//            //    .Sum(x => x.Amount) ?? 0;
//            //resultPayment.TotalTransactionPaymentE_Wallet_ZaloPay = (int?)paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.ZaloPay)
//            //    .Sum(x => x.Amount) ?? 0;

//            resultPayment.TotalTransactionPaymentE_Wallet_Momo = orderPayments
//                .SelectMany(x => x.Payments).Count(x => x.Type == (int)PaymentTypeEnum.MoMo);

//            resultPayment.TotalTransactionPaymentE_Wallet_GrabPay = orderPayments
//                .SelectMany(x => x.Payments).Count(x => x.Type == (int)PaymentTypeEnum.GrabPay);

//            resultPayment.TotalTransactionPaymentE_Wallet_GrabFood = orderPayments
//                .SelectMany(x => x.Payments).Count(x => x.Type == (int)PaymentTypeEnum.GrabFood);

//            resultPayment.TotalTransactionPaymentE_Wallet_VnPay = orderPayments
//                .SelectMany(x => x.Payments).Count(x => x.Type == (int)PaymentTypeEnum.VNPay);

//            resultPayment.TotalTransactionPaymentE_Wallet_Baemin = orderPayments
//                .SelectMany(x => x.Payments).Count(x => x.Type == (int)PaymentTypeEnum.BaeMin);

//            resultPayment.TotalTransactionPaymentE_Wallet_Shopeepay = orderPayments
//                .SelectMany(x => x.Payments).Count(x => x.Type == (int)PaymentTypeEnum.Shopeepay);

//            resultPayment.TotalTransactionPaymentE_Wallet_ZaloPay = orderPayments
//                .SelectMany(x => x.Payments).Count(x => x.Type == (int)PaymentTypeEnum.ZaloPay);

//            resultPayment.TotalTransactionPaymentE_Wallet = resultPayment.TotalTransactionPaymentE_Wallet_Momo
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_GrabPay
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_GrabFood
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_VnPay
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_Baemin
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_Shopeepay
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_ZaloPay;

//            //Số lượt thanh toán thanh toán ngân hàng
//            resultPayment.TotalTransactionPaymentBank = orderPayments
//                .SelectMany(x => x.Payments)
//                .Count(x => x.Type == (int)PaymentTypeEnum.MasterCard || x.Type == (int)PaymentTypeEnum.VisaCard);

//            //Số lượt thanh toán thanh toán khác
//            resultPayment.TotalTransactionPaymentOther = orderPayments
//                .SelectMany(x => x.Payments).Count(x => x.Type == (int)PaymentTypeEnum.Other);

//            resultPayment.TotalTransactionPayment = (int)(resultPayment.TotalTransactionPaymentE_Wallet +
//                                                          resultPayment.TotalTransactionPaymentOther +
//                                                          resultPayment.TotalTransactionPaymentBank +
//                                                          resultPayment.TotalTransactionPaymentForSales +
//                                                          resultPayment.TotalTransactionPaymentCard +
//                                                          resultPayment.TotalTransactionPaymentBuyCard);

//            #endregion


//            #region MyRegion

//            //Tổng thanh toán tiền mặt nạp thẻ
//            resultPayment.TotalPaymentCard = orderPayments
//                .Where(x => x.OrderType == (int)OrderTypeEnum.OrderCard)
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.Cash || x.Type == (int)PaymentTypeEnum.ExchangeCash))
//                .Sum(x => x.Amount);

//            //Tổng thanh toán tiền mặt bán hàng
//            //resultPayment.TotalPaymentForSales = ((int?)paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.Cash ||
//            //                x.Type == (int)PaymentTypeEnum.ExchangeCash)
//            //    .Sum(x => x.Amount) - dateReport.Sum(x => x.FinalAmountCard)) ?? 0;

//            resultPayment.TotalPaymentForSales = orderPayments
//                .Where(x => x.OrderType != (int)OrderTypeEnum.OrderCard)
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.Cash || x.Type == (int)PaymentTypeEnum.ExchangeCash))
//                .Sum(x => x.Amount);

//            //Tổng thanh toán thẻ thành viên bán hàng
//            //resultPayment.TotalPaymentBuyCard = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.MemberPayment)
//            //    .Sum(x => x.Amount);
//            resultPayment.TotalPaymentBuyCard = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.MemberPayment))
//                .Sum(x => x.Amount);

//            //Tổng thanh toán thanh toán ví điện tử, momo, gift talk, grabpay, grabfood
//            //resultPayment.TotalPaymentE_Wallet_Momo = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.MoMo)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_GrabPay = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.GrabPay)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_GrabFood = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.GrabFood)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_VNPay = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.VNPay)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_Baemin = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.BaeMin)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_Shopeepay = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.Shopeepay)
//            //    .Sum(x => x.Amount);
//            //resultPayment.TotalPaymentE_Wallet_ZaloPay = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.ZaloPay)
//            //    .Sum(x => x.Amount);

//            resultPayment.TotalPaymentE_Wallet_Momo = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.MoMo))
//                .Sum(x => x.Amount);

//            resultPayment.TotalPaymentE_Wallet_GrabPay = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.GrabPay))
//                .Sum(x => x.Amount);

//            resultPayment.TotalPaymentE_Wallet_GrabFood = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.GrabFood))
//                .Sum(x => x.Amount);

//            resultPayment.TotalPaymentE_Wallet_VNPay = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.VNPay))
//                .Sum(x => x.Amount);

//            resultPayment.TotalPaymentE_Wallet_Baemin = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.BaeMin))
//                .Sum(x => x.Amount);

//            resultPayment.TotalPaymentE_Wallet_Shopeepay = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.Shopeepay))
//                .Sum(x => x.Amount);

//            resultPayment.TotalPaymentE_Wallet_ZaloPay = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => (x.Type == (int)PaymentTypeEnum.ZaloPay))
//                .Sum(x => x.Amount);

//            resultPayment.TotalPaymentE_Wallet = resultPayment.TotalPaymentE_Wallet_Momo
//                                                 + resultPayment.TotalPaymentE_Wallet_GrabPay
//                                                 + resultPayment.TotalPaymentE_Wallet_GrabFood
//                                                 + resultPayment.TotalPaymentE_Wallet_VNPay
//                                                 + resultPayment.TotalPaymentE_Wallet_Baemin
//                                                 + resultPayment.TotalPaymentE_Wallet_Shopeepay
//                                                 + resultPayment.TotalPaymentE_Wallet_ZaloPay;


//            //Tổng thanh toán ngân hàng
//            //resultPayment.TotalPaymentBank = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.MasterCard ||
//            //                x.Type == (int)PaymentTypeEnum.VisaCard)
//            //    .Sum(x => x.Amount);
//            resultPayment.TotalPaymentBank = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => x.Type == (int)PaymentTypeEnum.MasterCard || x.Type == (int)PaymentTypeEnum.VisaCard)
//                .Sum(x => x.Amount);

//            //Tổng thanh toán khác
//            //resultPayment.TotalPaymentOther = paymentTypeReport
//            //    .Where(x => x.Type == (int)PaymentTypeEnum.Other)
//            //    .Sum(x => x.Amount);
//            resultPayment.TotalPaymentOther = orderPayments
//                .SelectMany(x => x.Payments)
//                .Where(x => x.Type == (int)PaymentTypeEnum.Other)
//                .Sum(x => x.Amount);

//            resultPayment.TotalPayment = resultPayment.TotalPaymentForSales
//                                         + resultPayment.TotalPaymentCard
//                                         + resultPayment.TotalPaymentBuyCard
//                                         + resultPayment.TotalPaymentE_Wallet
//                                         + resultPayment.TotalPaymentBank
//                                         + resultPayment.TotalPaymentOther;


//            #endregion

//            return resultPayment;
//        }


//        private List<TopStoreRevenueViewModel> GetTopStoreRevenue(DateRange dateRange, int? storeId)
//        {
//            #region Check Date range

//            var from = dateRange?.FromDate;
//            var to = dateRange?.ToDate;

//            if (from == null && to == null)
//            {
//                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
//                to = Utils.GetCurrentDate().AddDays(-1);
//            }

//            from ??= Utils.GetCurrentDate();
//            to ??= Utils.GetCurrentDate();

//            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
//            {
//                throw new ErrorResponse(400, "The datetime is invalid!");
//            }

//            from = ((DateTime)from).GetStartOfDate();
//            to = ((DateTime)to).GetEndOfDate();

//            #endregion

//            //var dateReportsAllStore = _context.DateReports
//            //   .Where(x => x.Date <= to && x.Date >= from && x.Active == true);

//            //var dateReportsOneStore = _context.DateReports
//            //    .Where(x => x.Date <= to && x.Date >= from && x.Active == true && x.StoreId == storeId);

//            //IQueryable<DateReport> dateReports;

//            //if (storeId == null)
//            //{
//            //    dateReports = dateReportsAllStore;
//            //}
//            //else
//            //{
//            //    dateReports = dateReportsOneStore;
//            //}

//            var dateReportsAllStore = _passioContext.DateReports
//                .Where(x => x.Date <= to && x.Date >= from && x.Status == 1);

//            var dateReportsOneStore = _passioContext.DateReports
//                .Where(x => x.Date <= to && x.Date >= from && x.Status == 1 && x.StoreId == storeId);

//            IQueryable<ResoReportDataService.Models.DateReport> dateReports;

//            if (storeId == null)
//            {
//                dateReports = dateReportsAllStore;
//            }
//            else
//            {
//                dateReports = dateReportsOneStore;
//            }

//            var resultReport = dateReports
//                .Include(x => x.Store)
//                .GroupBy(x => new
//                {
//                    StoreId = x.StoreId,
//                    StoreName = x.Store.Name,
//                    StoreCode = x.Store.StoreCode
//                })
//                .Select(x => new TopStoreRevenueViewModel()
//                {
//                    ////TotalProduct = "N/a",
//                    //StoreCode = "N/a",
//                    //StoreId = x.Key.StoreId,
//                    //StoreName = x.Key.StoreName,
//                    //TotalOrderSale = x.Sum(w => w.TotalOrderAtStore + w.TotalOrderTakeAway + w.TotalOrderDelivery),
//                    //TotalOrderCard = x.Sum(w => w.TotalOrderCard),
//                    //TotalRevenueSale = (x.Sum(w => w.FinalAmount) - x.Sum(w => w.FinalAmountCard)),
//                    //TotalRevenueCard = x.Sum(w => w.FinalAmountCard),
//                    //TotalRevenueAll = x.Sum(w => w.FinalAmount),
//                    //TotalRevenueBeforeDiscount = (x.Sum(w => w.TotalAmount) - x.Sum(w => w.FinalAmountCard)),
//                    //TotalDiscount = x.Sum(w => w.Discount + w.DiscountOrderDetail),
//                    //AvgRevenueSale = Math.Round((double)((x.Sum(w => w.FinalAmount) - x.Sum(w => w.FinalAmountCard)) / (x.Sum(w => w.TotalOrder) - x.Sum(w => w.TotalOrderCard))), 2),

//                    //TotalProduct = "N/a",
//                    StoreCode = x.Key.StoreCode,
//                    StoreId = x.Key.StoreId,
//                    StoreName = x.Key.StoreName,
//                    TotalOrderSale = x.Sum(w => w.TotalOrderAtStore + w.TotalOrderTakeAway + w.TotalOrderDelivery),
//                    TotalOrderCard = x.Sum(w => w.TotalOrderCard),
//                    TotalRevenueSale = (x.Sum(w => w.FinalAmount) - x.Sum(w => w.FinalAmountCard)),
//                    TotalRevenueCard = x.Sum(w => w.FinalAmountCard),
//                    TotalRevenueAll = x.Sum(w => w.FinalAmount),
//                    TotalRevenueBeforeDiscount = (x.Sum(w => w.TotalAmount) - x.Sum(w => w.FinalAmountCard)),
//                    TotalDiscount = x.Sum(w => w.Discount + w.DiscountOrderDetail),
//                    AvgRevenueSale = Math.Round((double)((x.Sum(w => w.FinalAmount) - x.Sum(w => w.FinalAmountCard))
//                                    / (((x.Sum(w => w.TotalOrder) - x.Sum(w => w.TotalOrderCard)) != 0) ?  //
//                                    x.Sum(w => w.TotalOrder) - x.Sum(w => w.TotalOrderCard) : 1))         //    check chia cho 0
//                                    , 2),                                                                     //
//                })
//                .OrderByDescending(q => q.TotalRevenueAll).ToList();

//            return resultReport;
//        }

//        public BaseResponsePagingViewModel<TopStoreRevenueViewModel> GetTopStoreRevenueWithPaging(int? storeId,
//            int brandId, DateRange dateRange, PagingModel paging, TopStoreRevenueViewModel modelFilter)
//        {
//            var resultReport = GetTopStoreRevenue(dateRange, storeId);

//            var (total, data) = resultReport
//                .AsQueryable()
//                .DynamicFilter(modelFilter)
//                .DynamicSort(modelFilter)
//                .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

//            return new BaseResponsePagingViewModel<TopStoreRevenueViewModel>()
//            {
//                Metadata = new PagingMetadata()
//                {
//                    Page = paging.Page,
//                    Size = paging.Size,
//                    Total = total
//                },
//                Data = data.ToList()
//            };
//        }

//        public FileStreamResult ExportTopStoreRevenue(DateRange dateRange, int? storeId)
//        {
//            #region Check Date range

//            var from = dateRange?.FromDate;
//            var to = dateRange?.ToDate;

//            if (from == null && to == null)
//            {
//                from = Utils.GetLastAndFirstDateInCurrentMonth().Item1;
//                to = Utils.GetCurrentDate().AddDays(-1);
//            }

//            from ??= Utils.GetCurrentDate();
//            to ??= Utils.GetCurrentDate();

//            if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
//            {
//                throw new ErrorResponse(400, "The datetime is invalid!");
//            }

//            from = ((DateTime)from).GetStartOfDate();
//            to = ((DateTime)to).GetEndOfDate();

//            #endregion


//            dateRange.FromDate = from;
//            dateRange.ToDate = to;

//            var sheetName = dateRange.FromDate?.Date == dateRange.ToDate?.Date ? dateRange.FromDate?.ToString("dd/MM/yyyy") : (dateRange.FromDate?.ToString("dd/MM/yyyy") + "-" + dateRange.ToDate?.ToString("dd/MM/yyyy"));

//            var result = GetTopStoreRevenue(dateRange, storeId);

//            return ExcelUtils.ExportExcel(new ExcelModel<TopStoreRevenueViewModel>()
//            {
//                SheetTitle = "BaoCaoTopDoanhThuHeThong-" + sheetName,
//                ColumnConfigs = new List<ColumnConfig<TopStoreRevenueViewModel>>()
//                {
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Tên cửa hàng",
//                        DataIndex = "StoreName",
//                        ValueType = "string"
//                    },
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Tổng sản phẩm",
//                        DataIndex = "TotalProduct",
//                        ValueType = "int"
//                    },
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Hóa đơn bán hàng",
//                        DataIndex = "TotalOrderSale",
//                        ValueType = "int"
//                    },
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Trung bình bill",
//                        DataIndex = "AvgRevenueSale",
//                        ValueType = "float"
//                    },
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Doanh thu trước giảm",
//                        DataIndex = "TotalRevenueBeforeDiscount",
//                        ValueType = "currency"
//                    },
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Giảm giá",
//                        DataIndex = "TotalDiscount",
//                        ValueType = "currency"
//                    },
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Doanh thu sau giảm",
//                        DataIndex = "TotalRevenueSale",
//                        ValueType = "currency"
//                    },
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Hóa đơn nạp thẻ",
//                        DataIndex = "TotalOrderCard",
//                        ValueType = "int"
//                    },
//                    new ColumnConfig<TopStoreRevenueViewModel>()
//                    {
//                        Title = "Doanh thu nạp thẻ",
//                        DataIndex = "TotalRevenueCard",
//                        ValueType = "currency"
//                    },
//                },
//                DataSources = result
//            });
//        }
//    }
//}