using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json.Schema;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Slicer.Style;
using Reso.Sdk.Core.Custom;
using ResoReportDataAccess.Models;
using ResoReportDataService.Commons;
using ResoReportDataService.Commons.ExcelUtils;
using ResoReportDataService.Models;
using ResoReportDataService.RequestModels;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.Services
{
    public interface IReportService
    {
        List<StoreReportViewModel> GetStoreReport(DateFilter filter);
        List<PaymentReportViewModel> GetPaymentReport(DateFilter filter);
        List<PromotionReportViewModel> GetPromotionReport(DateFilter filter);
        List<StoreReportViewModel> GetProductReport(DateFilter filter);
        List<OrderDetailReportViewModel> GetOrderDetailReport(Guid storeId , DateFilter filter);

        FileStreamResult ExportPromotionReportToExel(DateFilter filter, List<PromotionReportViewModel> orders);
        FileStreamResult ExportStoreReportToExcel(DateFilter filter, List<StoreReportViewModel> result);


        FileStreamResult ExportPaymentReportToExcel(DateFilter filter, List<PaymentReportViewModel> result);
        FileStreamResult ExportOrderDetailReportToExcel(Guid storeId, DateFilter filter);

    }

    public class ReportService : IReportService
    {
        private readonly PosSystemContext _posSystemContext;
        private readonly IRawQueryService _rawQueryService;


        public ReportService(PosSystemContext posSystemContext, IRawQueryService rawQueryService)
        {
            _posSystemContext = posSystemContext;
            _rawQueryService = rawQueryService;
        }

        public List<StoreReportViewModel> GetStoreReport(DateFilter filter)
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

            var result = _posSystemContext.Orders
                .Include(x => x.Session)
                .Where(x => x.Status.Equals(OrderStatus.PAID) &&
                            DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                            DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0)
                .GroupBy(x => new
                {
                    StoreId = x.Session.Store.Id,
                    StoreName = x.Session.Store.Name,
                })
                .Select(x => new StoreReportViewModel
                {
                    StoreName = x.Key.StoreName,

                    TotalOrderAtStore = x.Count(y => y.OrderType.Equals(OrderType.EAT_IN)),
                    TotalOrderTakeAway = x.Count(y => y.OrderType.Equals(OrderType.TAKE_AWAY)),
                    TotalOrderDelivery = x.Count(y => y.OrderType.Equals(OrderType.DELIVERY)),

                    FinalAmountAtStore = x.Where(y => y.OrderType.Equals(OrderType.EAT_IN)).Sum(y => y.FinalAmount),
                    FinalAmountTakeAway = x.Where(y => y.OrderType.Equals(OrderType.TAKE_AWAY)).Sum(y => y.FinalAmount),
                    FinalAmountDelivery = x.Where(y => y.OrderType.Equals(OrderType.DELIVERY)).Sum(y => y.FinalAmount),

                    TotalBills = x.Count(),
                    TotalSales = x.Sum(y => y.TotalAmount),
                    TotalDiscount = x.Sum(y => y.Discount),
                    TotalSalesAfterDiscount = x.Sum(y => y.FinalAmount)
                }).ToList();

            return result;
        }

        public List<StoreReportViewModel> GetProductReport(DateFilter filter)
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

            var result = _posSystemContext.Orders
                .Include(x => x.Session)
                .Where(x => x.Status == OrderStatus.PAID.GetDisplayName() &&
                            DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                            DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0)
                .GroupBy(x => new
                {
                    StoreId = x.Session.Store.Id,
                    StoreName = x.Session.Store.Name,
                })
                .Select(x => new StoreReportViewModel
                {
                    StoreName = x.Key.StoreName,

                    TotalOrderAtStore = x.Count(y => y.OrderType.Equals(OrderType.EAT_IN)),
                    TotalOrderTakeAway = x.Count(y => y.OrderType.Equals(OrderType.TAKE_AWAY)),
                    TotalOrderDelivery = x.Count(y => y.OrderType.Equals(OrderType.DELIVERY)),

                    FinalAmountAtStore = x.Where(y => y.OrderType.Equals(OrderType.EAT_IN)).Sum(y => y.FinalAmount),
                    FinalAmountTakeAway = x.Where(y => y.OrderType.Equals(OrderType.TAKE_AWAY)).Sum(y => y.FinalAmount),
                    FinalAmountDelivery = x.Where(y => y.OrderType.Equals(OrderType.DELIVERY)).Sum(y => y.FinalAmount),

                    TotalBills = x.Count(),
                    TotalSales = x.Sum(y => y.TotalAmount),
                    TotalDiscount = x.Sum(y => y.Discount),
                    TotalSalesAfterDiscount = x.Sum(y => y.FinalAmount)
                }).ToList();

            return result;
        }

        public List<PaymentReportViewModel> GetPaymentReport(DateFilter filter)
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

            return _rawQueryService.GetPaymentReportRawSqlQuery<PaymentReportViewModel>(from, to);
        }

        public List<PromotionReportViewModel> GetPromotionReport(DateFilter filter)
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

            var result = _posSystemContext.Orders
                .Include(x => x.Session)
                .ThenInclude(x => x.Store)
                .Where(x => x.Status == OrderStatus.PAID.GetDisplayName() &&
                            DateTime.Compare((DateTime)x.CheckInDate, ((DateTime)from).GetStartOfDate()) >= 0 &&
                            DateTime.Compare((DateTime)x.CheckInDate, ((DateTime)to).GetEndOfDate()) <= 0
                )
                .GroupBy(x => new
                {
                    StoreId = x.Session.Store.Id,
                    StoreName = x.Session.Store.Name
                })
                .Select(x => new PromotionReportViewModel()
                {
                    CustomerName = "",
                    SumAmount = x.Sum(order => order.TotalAmount),
                    StoreName = x.Key.StoreName
                }).ToList();
            return result;
        }
        public List<OrderDetailReportViewModel> GetOrderDetailReport(Guid storeId, DateFilter filter)
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
                                .Include(x => x.Session)
                                .Where(x => x.Session.StoreId.Equals(storeId) &&
                                            DateTime.Compare(x.CheckInDate, (DateTime)from) >= 0 &&
                                            DateTime.Compare(x.CheckInDate, (DateTime)to) <= 0);


            //var orders = _posSystemContext.Orders
            //        .Include(x => x.Session)
            //        .Where(x => x.Status == OrderStatus.PAID.GetDisplayName() &&
            //                x.Session.StoreId == storeId &&
            //                DateTime.Compare((DateTime)x.CheckInDate, ((DateTime)from).GetStartOfDate()) >= 0 &&
            //                DateTime.Compare((DateTime)x.CheckInDate, ((DateTime)to).GetEndOfDate()) <= 0)
            //        .Select(x => new
            //        {
            //            x.CheckInDate,
            //            x.InvoiceId,
            //            x.Payments,
            //            x.RentId,
            //            x.Discount
            //        })
            //        .OrderBy(x => x.CheckInDate.Value.Date)
            //        .ToList();

            //var listOrderDetail = _posSystemContext.OrderDetails
            //        .Where(x => 
            //                x.StoreId.Equals(storeId) &&
            //                DateTime.Compare((DateTime)x.OrderDate, ((DateTime)from).GetStartOfDate()) >= 0 &&
            //                DateTime.Compare((DateTime)x.OrderDate, ((DateTime)to).GetEndOfDate()) <= 0)
            //        .ToList();

            //List<OrderDetailReportViewModel> listData = new List<OrderDetailReportViewModel>();
           
            //foreach (var order in orders)
            //{
            //    var detailsOfOrder = listOrderDetail.Where(x => x.RentId == order.RentId).ToList();
            //    foreach (var detail in detailsOfOrder)
            //    {
            //        var orderDetail = new OrderDetailReportViewModel()
            //        {
            //            CheckInDate = detail.OrderDate.ToString("dd/MM/yyyy hh:mm tt"),
            //            InvoiceId = order.InvoiceId,
            //            ProductCode = detail.Product.Code,
            //            ProductName = detail.Product.ProductName,
            //            UnitPrice = detail.UnitPrice,
            //            Quantity = detail.Quantity,
            //            TotalAmount = detail.TotalAmount,
            //            DiscountOrderDetail = detail.Discount,
            //        };
            //        var totalQuantity = detailsOfOrder.Select(x => x.ItemQuantity).Sum();
            //        orderDetail.Discount = order.Discount / totalQuantity * detail.Quantity;
            //        orderDetail.FinalAmount = (detail.TotalAmount - detail.Discount - orderDetail.Discount);
            //        if (order.Payments.Count() != 0)
            //        {
            //            var orderPayment = order.Payments.FirstOrDefault(x => x.ToRentId == order.RentId).Type;
            //            orderDetail.PaymentName = _posSystemContext.PaymentTypes.FirstOrDefault(x => x.Id == orderPayment).Name;
            //        }
            //        else
            //        {
            //            orderDetail.PaymentName = "";
            //        }
            //        listData.Add(orderDetail);
            //    }
            //}
            //return listData;
        }


        public FileStreamResult ExportStoreReportToExcel(DateFilter filter, List<StoreReportViewModel> result)
        {
            #region Export to Excel

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath ?? "");
            var resourcePath = Path.Combine(path, @"Resources");
            var filePath = Path.Combine(resourcePath, @"StoreReportTemplate.xlsx");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            string storeName = "AllStore";
            if (filter.FromDate != null) filter.FromDate = ((DateTime)filter.FromDate).GetStartOfDate();
            if (filter.ToDate != null) filter.ToDate = ((DateTime)filter.ToDate).GetEndOfDate();
            var startDate = filter.FromDate.ToString().Replace("/", "-");
            var endDate = filter.ToDate.ToString().Replace("/", "-");
            var dateRange = "(" + startDate + (startDate == endDate ? "" : " - " + endDate) + ")";
            string fileName = "BaoCaoTheoNgay_" + "Store_" + storeName + "_" + dateRange + ".xlsx";
            using (ExcelPackage package = new ExcelPackage(fileStream))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets[0];
                char startHeaderChar = 'A';
                int startHeaderNumber = 6;

                ws.Cells["B3"].Value = dateRange;
                ws.Cells["G3"].Value = storeName;

                #region Set values for cells

                foreach (var data in result)
                {
                    startHeaderChar = 'A';
                    ws.Cells["" + (startHeaderChar++) + (++startHeaderNumber)].Value = data.StoreName;
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.TotalOrderTakeAway);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.FinalAmountTakeAway);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.TotalOrderAtStore);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.FinalAmountAtStore);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.TotalOrderDelivery);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.FinalAmountDelivery);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.TotalBills);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.TotalSales);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.TotalDiscount);
                    ws.Cells["" + (startHeaderChar) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.TotalSalesAfterDiscount);
                }

                var endHeaderChar = startHeaderChar;
                var endHeaderNumber = startHeaderNumber;
                for (char j = 'B'; j <= endHeaderChar; j++)
                {
                    for (int k = 7; k <= endHeaderNumber; k++)
                    {
                        ws.Cells["" + (j) + (k)].Value = Convert.ToDecimal(ws.Cells["" + (j) + (k)].Value);
                    }
                }

                #endregion

                MemoryStream ms = new MemoryStream();
                package.SaveAs(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileStream.Close();
                return new FileStreamResult(ms, contentType)
                {
                    FileDownloadName = fileName
                };
            }

            #endregion
        }

        public FileStreamResult ExportPaymentReportToExcel(DateFilter filter, List<PaymentReportViewModel> result)
        {
            #region Export to Excel

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath ?? "");
            var resourcePath = Path.Combine(path, @"Resources");
            var filePath = Path.Combine(resourcePath, @"PaymentReportTemplate.xlsx");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            string storeName = "AllStore";
            if (filter.FromDate != null) filter.FromDate = ((DateTime)filter.FromDate).GetStartOfDate();
            if (filter.ToDate != null) filter.ToDate = ((DateTime)filter.ToDate).GetEndOfDate();
            var startDate = filter.FromDate.ToString().Replace("/", "-");
            var endDate = filter.ToDate.ToString().Replace("/", "-");
            var dateRange = "(" + startDate + (startDate == endDate ? "" : " - " + endDate) + ")";
            string fileName = "BaoCaoTheoPhuongThucThanhToan_" + "Store_" + storeName + "_" + dateRange + ".xlsx";
            using (ExcelPackage package = new ExcelPackage(fileStream))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets[0];
                char startHeaderChar = 'A';
                int startHeaderNumber = 6;

                ws.Cells["B3"].Value = dateRange;
                ws.Cells["F3"].Value = storeName;

                #region Set values for cells

                foreach (var data in result)
                {
                    startHeaderChar = 'A';
                    ws.Cells["" + (startHeaderChar++) + (++startHeaderNumber)].Value = data.StoreName;
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.Cash);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.CreditCard);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.CreditCardUse);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.Bank);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.Momo);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.GrabPay);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.GrabFood);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.VnPay);
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.Baemin);
                    ws.Cells["" + (startHeaderChar) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.ShopeePay);
                }

                var endHeaderChar = startHeaderChar;
                var endHeaderNumber = startHeaderNumber;
                for (char j = 'B'; j <= endHeaderChar; j++)
                {
                    for (int k = 7; k <= endHeaderNumber; k++)
                    {
                        ws.Cells["" + (j) + (k)].Value = Convert.ToDecimal(ws.Cells["" + (j) + (k)].Value);
                    }
                }

                #endregion

                MemoryStream ms = new MemoryStream();
                package.SaveAs(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileStream.Close();
                return new FileStreamResult(ms, contentType)
                {
                    FileDownloadName = fileName
                };
            }

            #endregion
        }

        public FileStreamResult ExportPromotionReportToExel(DateFilter filter, List<PromotionReportViewModel> result)
        {
            #region Export to Excel

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath ?? "");
            var resourcePath = Path.Combine(path, @"Resources");
            var filePath = Path.Combine(resourcePath, @"PromotionReportTemplate.xlsx");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            string storeName = "AllStore";
            if (filter.FromDate != null) filter.FromDate = ((DateTime)filter.FromDate).GetStartOfDate();
            if (filter.ToDate != null) filter.ToDate = ((DateTime)filter.ToDate).GetEndOfDate();
            var startDate = filter.FromDate.ToString().Replace("/", "-");
            var endDate = filter.ToDate.ToString().Replace("/", "-");
            var dateRange = "(" + startDate + (startDate == endDate ? "" : " - " + endDate) + ")";
            string fileName = "BaoCaoKhuyenMai_" + "Store_" + storeName + "_" + dateRange + ".xlsx";
            using (ExcelPackage package = new ExcelPackage(fileStream))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets[0];
                char startHeaderChar = 'A';
                int startHeaderNumber = 6;
                int no = 1;
                ws.Cells["B3"].Value = dateRange;
                ws.Cells["F3"].Value = storeName;

                #region Set values for cells

                foreach (var data in result)
                {
                    startHeaderChar = 'A';
                    ws.Cells["" + (startHeaderChar++) + (++startHeaderNumber)].Value = no++;
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value = data.CustomerName;
                    ws.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value = data.StoreName;
                    ws.Cells["" + (startHeaderChar) + (startHeaderNumber)].Value =
                        string.Format(CultureInfo.InvariantCulture, "{0:0,0}", data.SumAmount);
                }

                var endHeaderChar = startHeaderChar;
                var endHeaderNumber = startHeaderNumber;
                for (char j = 'D'; j <= endHeaderChar; j++)
                {
                    for (int k = 7; k <= endHeaderNumber; k++)
                    {
                        ws.Cells["" + (j) + (k)].Value = Convert.ToDecimal(ws.Cells["" + (j) + (k)].Value);
                    }
                }

                #endregion

                MemoryStream ms = new MemoryStream();
                package.SaveAs(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileStream.Close();
                return new FileStreamResult(ms, contentType)
                {
                    FileDownloadName = fileName
                };
            }

            #endregion
        }

        public FileStreamResult ExportOrderDetailReportToExcel(Guid storeId, DateFilter filter)
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

            var sheetName = storeId + "-(" + (filter.FromDate?.Date == filter.ToDate?.Date ? filter.FromDate?.ToString("dd/MM/yyyy") : (filter.FromDate?.ToString("dd/MM/yyyy") + "-" + filter.ToDate?.ToString("dd/MM/yyyy")) + ")");
            var result = GetOrderDetailReport(storeId, filter);

            return ExcelUtils.ExportExcel(new ExcelModel<OrderDetailReportViewModel>()
            {
                SheetTitle = "BaoCao-" + sheetName,
                ColumnConfigs = new List<ColumnConfig<OrderDetailReportViewModel>>()
                    {

                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Ngày",
                            DataIndex = "CheckInDate",
                            ValueType = "string"
                        },

                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Mã hóa đơn",
                            DataIndex = "InvoiceId",
                            ValueType = "string"
                        },

                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Mã sản phẩm",
                            DataIndex = "ProductCode",
                            ValueType = "String"
                        },
                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Tên sản phẩm",
                            DataIndex = "ProductName",
                            ValueType = "String"
                        },
                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Đơn giá",
                            DataIndex = "UnitPrice",
                            ValueType = "int"
                        },
                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Số lượng",
                            DataIndex = "Quantity",
                            ValueType = "int"
                        },
                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Thành tiền",
                            DataIndex = "TotalAmount",
                            ValueType = "int"
                        },
                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Chiết khấu",
                            DataIndex = "DiscountOrderDetail",
                            ValueType = "int"
                        },
                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Giảm giá",
                            DataIndex = "Discount",
                            ValueType = "int"
                        },
                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Tổng thanh toán",
                            DataIndex = "FinalAmount",
                            ValueType = "int"
                        },
                        new ColumnConfig<OrderDetailReportViewModel>()
                        {
                            Title = "Phương thức thanh toán",
                            DataIndex = "PaymentName",
                            ValueType = "string"
                        }
                    },
                DataSources = result
            });
        }
    }
}