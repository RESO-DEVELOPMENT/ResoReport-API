//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Reso.Sdk.Core.Custom;
//using ResoReportDataService.Commons;
//using ResoReportDataService.Models;
//using ResoReportDataService.RequestModels;
//using ResoReportDataService.ViewModels;

//namespace ResoReportDataService.Services
//{
//    public interface IRevenueReportService
//    {
//        OverviewDashboard GetOverviewDashboard(DateFilter filter, int brandId, int storeId);
//        OverviewPaymentDashboard GetOverviewPaymentDashboard(DateFilter filter, int brandId, int storeId);
//    }

//    public class RevenueReportService : IRevenueReportService
//    {
//        private readonly ProdPassioContext _context;

//        public RevenueReportService(ProdPassioContext context)
//        {
//            _context = context;
//        }

//        public OverviewPaymentDashboard GetOverviewPaymentDashboard(DateFilter filter, int brandId, int storeId)
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

//            from = ((DateTime)from).GetStartOfDate();
//            to = ((DateTime)to).GetEndOfDate();
           
//            var resultPayment = new OverviewPaymentDashboard();
//            var listStoreIds = _context.Stores.Where(x => x.BrandId == brandId && x.IsAvailable == true)
//                .Select(s => s.Id)
//                .ToList();
//            var orderPayments = _context.Orders
//                .Include(i => i.Payments)
//                .Where(x => x.OrderStatus == (int)OrderStatusEnum.Finish &&
//                            listStoreIds.Contains(x.StoreId ?? -1) &&
//                            x.CheckInDate >= from &&
//                            x.CheckInDate < to)
//                .ToList();

//            //Tổng lượt thanh toán
//            resultPayment.TotalTransactionPayment = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type != (int)PaymentTypeEnum.ExchangeCash);

//            //Số lượt thanh toán tiền mặt bán hàng
//            resultPayment.TotalTransactionPaymentForSales = orderPayments
//                .Where(q => q.OrderType != (int)OrderTypeEnum.OrderCard).SelectMany(s => s.Payments)
//                .Count(q => (q.Type == (int)PaymentTypeEnum.Cash));

//            //Số lượt thanh toán tiền mặt nạp thẻ                
//            resultPayment.TotalTransactionPaymentCard = orderPayments
//                .Where(q => q.OrderType == (int)OrderTypeEnum.OrderCard).SelectMany(s => s.Payments)
//                .Count(q => (q.Type == (int)PaymentTypeEnum.Cash));

//            //Số lượt thanh toán thẻ thành viên bán hàng
//            resultPayment.TotalTransactionPaymentBuyCard = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.MemberPayment);

//            //Số lượt thanh toán thanh toán ví điện tử, momo, grabpay, grabfood, baemin        
//            resultPayment.TotalTransactionPaymentE_Wallet_Momo = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.MoMo);
//            resultPayment.TotalTransactionPaymentE_Wallet_GrabPay = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.GrabPay);
//            resultPayment.TotalTransactionPaymentE_Wallet_GrabFood = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.GrabFood);
//            resultPayment.TotalTransactionPaymentE_Wallet_VnPay = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.VNPay);
//            resultPayment.TotalTransactionPaymentE_Wallet_Baemin = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.BaeMin);
//            resultPayment.TotalTransactionPaymentE_Wallet_Shopeepay = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.Shopeepay);
//            resultPayment.TotalTransactionPaymentE_Wallet_ZaloPay = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.ZaloPay);

//            resultPayment.TotalTransactionPaymentE_Wallet = resultPayment.TotalTransactionPaymentE_Wallet_Momo
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_GrabPay
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_GrabFood
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_VnPay
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_Baemin
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_Shopeepay
//                                                            + resultPayment.TotalTransactionPaymentE_Wallet_ZaloPay
//                ;
//            //Số lượt thanh toán thanh toán ngân hàng
//            resultPayment.TotalTransactionPaymentBank = orderPayments.SelectMany(s => s.Payments).Count(q =>
//                q.Type == (int)PaymentTypeEnum.MasterCard || q.Type == (int)PaymentTypeEnum.VisaCard);
//            //Số lượt thanh toán thanh toán khác
//            resultPayment.TotalTransactionPaymentOther = orderPayments.SelectMany(s => s.Payments)
//                .Count(q => q.Type == (int)PaymentTypeEnum.Other);

//            //Tổng thanh toán

//            //Tổng thanh toán tiền mặt bán hàng
//            resultPayment.TotalPaymentForSales = orderPayments.Where(q => q.OrderType != (int)OrderTypeEnum.OrderCard)
//                .SelectMany(s => s.Payments)
//                .Where(q => (q.Type == (int)PaymentTypeEnum.Cash || q.Type == (int)PaymentTypeEnum.ExchangeCash))
//                .Sum(q => q.Amount);

//            //Tổng thanh toán tiền mặt nạp thẻ
//            resultPayment.TotalPaymentCard = orderPayments.Where(q => q.OrderType == (int)OrderTypeEnum.OrderCard)
//                .SelectMany(s => s.Payments)
//                .Where(q => (q.Type == (int)PaymentTypeEnum.Cash || q.Type == (int)PaymentTypeEnum.ExchangeCash))
//                .Sum(q => q.Amount);

//            //Tổng thanh toán thẻ thành viên bán hàng
//            resultPayment.TotalPaymentBuyCard = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.MemberPayment).Sum(q => q.Amount);

//            //Tổng thanh toán thanh toán ví điện tử, momo, gift talk, grabpay, grabfood
//            resultPayment.TotalPaymentE_Wallet_Momo = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.MoMo).Sum(q => q.Amount);
//            resultPayment.TotalPaymentE_Wallet_GrabPay = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.GrabPay).Sum(q => q.Amount);
//            resultPayment.TotalPaymentE_Wallet_GrabFood = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.GrabFood).Sum(q => q.Amount);
//            resultPayment.TotalPaymentE_Wallet_VNPay = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.VNPay).Sum(q => q.Amount);
//            resultPayment.TotalPaymentE_Wallet_Baemin = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.BaeMin).Sum(q => q.Amount);
//            resultPayment.TotalPaymentE_Wallet_Shopeepay = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.Shopeepay).Sum(q => q.Amount);
//            resultPayment.TotalPaymentE_Wallet_ZaloPay = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.ZaloPay).Sum(q => q.Amount);

//            resultPayment.TotalPaymentE_Wallet = resultPayment.TotalPaymentE_Wallet_Momo
//                                                 + resultPayment.TotalPaymentE_Wallet_GrabPay
//                                                 + resultPayment.TotalPaymentE_Wallet_GrabFood
//                                                 + resultPayment.TotalPaymentE_Wallet_VNPay
//                                                 + resultPayment.TotalPaymentE_Wallet_Baemin
//                                                 + resultPayment.TotalPaymentE_Wallet_Shopeepay
//                                                 + resultPayment.TotalPaymentE_Wallet_ZaloPay
//                ;
//            //Tổng thanh toán ngân hàng
//            resultPayment.TotalPaymentBank = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.MasterCard || q.Type == (int)PaymentTypeEnum.VisaCard)
//                .Sum(q => q.Amount);
//            //Tổng thanh toán khác
//            resultPayment.TotalPaymentOther = orderPayments.SelectMany(s => s.Payments)
//                .Where(q => q.Type == (int)PaymentTypeEnum.Other).Sum(q => q.Amount);

//            resultPayment.TotalPayment = resultPayment.TotalPaymentForSales
//                                         + resultPayment.TotalPaymentCard
//                                         + resultPayment.TotalPaymentBuyCard
//                                         + resultPayment.TotalPaymentE_Wallet
//                                         + resultPayment.TotalPaymentBank
//                                         + resultPayment.TotalPaymentOther
//                ;

//            return resultPayment;
//        }

//        public OverviewDashboard GetOverviewDashboard(DateFilter filter, int brandId, int storeId)
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

//            from = ((DateTime)from).GetStartOfDate();
//            to = ((DateTime)to).GetEndOfDate();
//            var result = new OverviewDashboard();
//            var listStoreIds = _context.Stores.Where(x => x.BrandId == brandId && x.IsAvailable == true)
//                .Select(s => s.Id)
//                .ToList();

//            if (storeId > 0)
//            {
//                listStoreIds = listStoreIds.Where(q => q == storeId).ToList();
//            }

//            var dateReport = _context.DateReports
//                .Where(x => listStoreIds.Contains(x.StoreId) &&
//                            DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                            DateTime.Compare(x.Date, (DateTime)to) <= 0).ToList();

//            List<DateProduct> dateProduct;
//            if (storeId > 0)
//            {
//                dateProduct = _context.DateProducts
//                    .Include(x => x.Store)
//                    .Where(x => x.StoreId == storeId &&
//                                (bool)x.Store.IsAvailable &&
//                                DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                                DateTime.Compare(x.Date, (DateTime)to) <= 0 &&
//                                x.Product.ProductType != (int)ProductTypeEnum.CardPayment)
//                    .ToList();
//            }
//            else
//            {
//                dateProduct = _context.DateProducts
//                    .Where(x =>
//                        (bool)x.Store.IsAvailable &&
//                        x.Store.BrandId == brandId &&
//                        DateTime.Compare(x.Date, (DateTime)from) >= 0 &&
//                        DateTime.Compare(x.Date, (DateTime)to) <= 0 &&
//                        x.Product.ProductType != (int)ProductTypeEnum.CardPayment &&
//                        listStoreIds.Contains(x.StoreId))
//                    .ToList();
//            }

//            if (dateReport.Any())
//            {
//                result.TotalOrder = dateReport.Sum(q => q.TotalOrder) - dateReport.Sum(q => q.TotalOrderCard);
//                result.TotalOrderDelivery = dateReport.Sum(q => q.TotalOrderDelivery);
//                result.TotalOrderAtStore = dateReport.Sum(q => q.TotalOrderAtStore);
//                result.TotalOrderTakeAway = dateReport.Sum(q => q.TotalOrderTakeAway);
//                result.TotalOrderCard = dateReport.Sum(q => q.TotalOrderCard);
//                result.TotalOrderAfterCancel = dateReport.Where(q => q.TotalOrderCanceled.HasValue)
//                    .Sum(q => q.TotalOrderCanceled.Value);
//                result.TotalOrderPreCancel = dateReport.Where(q => q.TotalOrderPreCanceled.HasValue)
//                    .Sum(q => q.TotalOrderPreCanceled.Value);

//                result.TotalRevenueOrderCard = (int)dateReport.Sum(q => q.FinalAmountCard.GetValueOrDefault());
//                result.TotalRevenueWithDiscount = dateReport.Sum(q => q.TotalAmount.GetValueOrDefault()) -
//                                                  result.TotalRevenueOrderCard;
//                result.TotalRevenueWithoutDiscountAndCard = dateReport.Sum(q => q.FinalAmount.GetValueOrDefault()) -
//                                                            result.TotalRevenueOrderCard;
//                result.TotalRevenue = result.TotalRevenueWithoutDiscountAndCard;
//                result.TotalDiscount = result.TotalRevenueWithDiscount - result.TotalRevenueWithoutDiscountAndCard;

//                result.TotalRevenueAtStore = (int)dateReport.Sum(q => q.FinalAmountAtStore.GetValueOrDefault());
//                result.TotalRevenueTakeAway = (int)dateReport.Sum(q => q.FinalAmountTakeAway.GetValueOrDefault());
//                result.TotalRevenueDelivery = (int)dateReport.Sum(q => q.FinalAmountDelivery.GetValueOrDefault());
//                result.TotalRevenuePreCancel = dateReport.Sum(q => q.FinalAmountPreCanceled.GetValueOrDefault());
//                result.TotalRevenueAfterCancel = dateReport.Sum(q => q.FinalAmountCanceled.GetValueOrDefault());

//                result.AvgRevenueOrder = result.TotalOrder == 0
//                    ? 0
//                    : result.TotalRevenueWithoutDiscountAndCard / result.TotalOrder;
//                if (dateProduct.Any())
//                {
//                    result.AvgProductOrder = result.TotalOrder == 0
//                        ? 0
//                        : Math.Round(
//                            (double)(dateProduct?.Sum(q => q.OrderQuantity.GetValueOrDefault()) / result.TotalOrder),
//                            2);
//                    result.AvgProductOrderAtStore = result.TotalOrderAtStore == 0
//                        ? 0
//                        : Math.Round(
//                            (double)(dateProduct?.Sum(q => q.QuantityAtStore.GetValueOrDefault()) /
//                                     result.TotalOrderAtStore), 2);
//                    result.AvgProductOrderTakeAway = result.TotalOrderTakeAway == 0
//                        ? 0
//                        : Math.Round(
//                            (double)(dateProduct?.Sum(q => q.QuantityTakeAway.GetValueOrDefault()) /
//                                     result.TotalOrderTakeAway), 2);
//                    result.AvgProductOrderDelivery = result.TotalOrderDelivery == 0
//                        ? 0
//                        : Math.Round(
//                            (double)(dateProduct?.Sum(q => q.QuantityDelivery.GetValueOrDefault()) /
//                                     result.TotalOrderDelivery), 2);
//                }
//            }

//            return result;
//        }
//    }
//}