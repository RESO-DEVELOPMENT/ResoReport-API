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
using ResoReportDataService.Commons;
using ResoReportDataService.Models;
using ResoReportDataService.RequestModels;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.Services
{
    public interface IPromotionReportService
    {
        BaseResponsePagingViewModel<PromotionReportViewModel> GetPromotionReports(DateFilter filter, int storeId,
            PagingModel paging);

        FileStreamResult ExportPromotionReportToExel(DateFilter filter);
    }

    public class PromotionReportService : IPromotionReportService
    {
        private readonly ProdPassioContext _passioContext;

        public PromotionReportService(ProdPassioContext passioContext)
        {
            _passioContext = passioContext;
        }

        public BaseResponsePagingViewModel<PromotionReportViewModel> GetPromotionReports(DateFilter filter, int storeId,
            PagingModel paging)
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


            var result = _passioContext.Orders
                .Include(x => x.Customer)
                .Include(x => x.Store)
                .Where(x => x.OrderStatus == 2 &&
                            x.CustomerId != null &&
                            x.StoreId == storeId &&
                            x.Att1.Contains("passio-100") &&
                            DateTime.Compare((DateTime)x.CheckInDate, (DateTime)from) >= 0 &&
                            DateTime.Compare((DateTime)x.CheckInDate, (DateTime)to) <= 0
                )
                .GroupBy(x => new
                {
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.Name,
                    StoreId = x.StoreId,
                    StoreName = x.Store.Name
                })
                .Select(x => new PromotionReportViewModel()
                {
                    CustomerName = x.Key.CustomerName,
                    SumAmount = x.Sum(order => order.TotalAmount),
                    StoreName = x.Key.StoreName
                })
                .PagingIQueryable(paging.Page, paging.Size, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);

            return new BaseResponsePagingViewModel<PromotionReportViewModel>()
            {
                Metadata = new PagingMetadata()
                {
                    Page = paging.Page,
                    Size = paging.Size,
                    Total = result.Item1
                },
                Data = result.Item2.ToList()
            };
        }

        private List<PromotionReportViewModel> GetListPromotionReports(DateFilter filter)
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

            var result = _passioContext.Orders
                .Include(x => x.Customer)
                .Include(x => x.Store)
                .Where(x => x.OrderStatus == 2 &&
                            x.CustomerId != null &&
                            x.Att1.Contains("passio-100") &&
                            DateTime.Compare((DateTime)x.CheckInDate, (DateTime)from) >= 0 &&
                            DateTime.Compare((DateTime)x.CheckInDate, (DateTime)to) <= 0
                )
                .GroupBy(x => new
                {
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.Name,
                    StoreId = x.StoreId,
                    StoreName = x.Store.Name
                })
                .Select(x => new PromotionReportViewModel()
                {
                    CustomerName = x.Key.CustomerName,
                    SumAmount = x.Sum(order => order.TotalAmount),
                    StoreName = x.Key.StoreName
                }).ToList();
            return result;
        }

        public FileStreamResult ExportPromotionReportToExel(DateFilter filter)
        {
            var result = GetListPromotionReports(filter);
            #region Export to Excel

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath ?? "");
            var resourcePath = Path.Combine(path, @"Resources");
            var filePath = Path.Combine(resourcePath, @"PromotionReportTemplate.xlsx");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            string storeName = "AllStore";
            var startDate = filter?.FromDate.ToString().Replace("/", "-");
            var endDate = filter?.ToDate.ToString().Replace("/", "-");
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
    }
}