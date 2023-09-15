using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using ResoReportDataService.Commons;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.Services
{
    public interface IRawQueryService
    {
        List<PaymentReportViewModel> GetPaymentReportRawSqlQuery<T>(DateTime? from, DateTime? to);
    }

    public class RawQueryService : IRawQueryService
    {
        public List<PaymentReportViewModel> GetPaymentReportRawSqlQuery<T>(DateTime? from, DateTime? to)
        {
            List<PaymentReportViewModel> list = new List<PaymentReportViewModel>();
            string sqlConnection =
                "Data Source=13.239.27.24;Initial Catalog=ProdPassio;User ID=dev-team;Password=zaQ@1234;MultipleActiveResultSets=true";
            var connection = new SqlConnection(sqlConnection);
            connection.Open();
            using (DbCommand command = connection.CreateCommand())
            {
                if (from != null) from = ((DateTime)from).GetStartOfDate();
                if (to != null) to = ((DateTime)to).GetEndOfDate();
                command.CommandText =
                    string.Format(
                        "select s.Name as StoreName, SUM(CASE WHEN(p.type = {0} or p.type = {1}) and o.OrderType != {2}  THEN p.Amount ELSE 0 END) Cash, "
                        + "SUM(CASE WHEN  o.OrderType = {3} THEN p.Amount ELSE 0 END) CreditCard, "
                        + "SUM(CASE WHEN  p.type = {4} THEN p.Amount ELSE 0 END) Momo, "
                        + "SUM(CASE WHEN(p.type = {5} or p.type = {6}) THEN p.Amount ELSE 0 END) Bank, "
                        + "SUM(CASE WHEN(p.type = {7}) THEN p.Amount ELSE 0 END) CreditCardUse, "
                        + "SUM(CASE WHEN(p.type = {8}) THEN p.Amount ELSE 0 END) Grabpay, "
                        + "SUM(CASE WHEN(p.type = {9}) THEN p.Amount ELSE 0 END) GrabFood, "
                        + "SUM(CASE WHEN(p.type = {10}) THEN p.Amount ELSE 0 END) Vnpay, "
                        + "SUM(CASE WHEN(p.type = {11}) THEN p.Amount ELSE 0 END) Baemin, "
                        + "SUM(CASE WHEN(p.type = {12}) THEN p.Amount ELSE 0 END) Shopeepay from [order] o "
                        + "join payment p on o.rentid = p.ToRentID "
                        + "join store s on o.storeid = s.id where p.paytime >= '{13}' and p.paytime <= '{14}' and o.OrderStatus = {15} group by s.Name",
                        (int)PaymentTypeEnum.Cash, //0
                        (int)PaymentTypeEnum.ExchangeCash, //1
                        (int)OrderTypeEnum.OrderCard, //2
                        (int)OrderTypeEnum.OrderCard, //3
                        (int)PaymentTypeEnum.MoMo, //4
                        (int)PaymentTypeEnum.MasterCard, //5
                        (int)PaymentTypeEnum.VisaCard, //6
                        (int)PaymentTypeEnum.MemberPayment, //7
                        (int)PaymentTypeEnum.GrabPay, //8
                        (int)PaymentTypeEnum.GrabFood, //9
                        (int)PaymentTypeEnum.VNPay, //10
                        (int)PaymentTypeEnum.BaeMin, //11
                        (int)PaymentTypeEnum.Shopeepay, //12
                        from, //13
                        to, //14
                        (int)OrderStatusEnum.Finish //15
                    );
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new PaymentReportViewModel()
                    {
                        StoreName = reader["StoreName"].ToString(),
                        Cash = Convert.ToDouble(reader["Cash"]),
                        CreditCard = Convert.ToDouble(reader["CreditCard"]),
                        Momo = Convert.ToDouble(reader["Momo"]),
                        Bank = Convert.ToDouble(reader["Bank"]),
                        CreditCardUse = Convert.ToDouble(reader["CreditCardUse"]),
                        GrabPay = Convert.ToDouble(reader["Grabpay"]),
                        GrabFood = Convert.ToDouble(reader["Grabfood"]),
                        VnPay = Convert.ToDouble(reader["Vnpay"]),
                        Baemin = Convert.ToDouble(reader["Baemin"]),
                        ShopeePay = Convert.ToDouble(reader["Shopeepay"]),
                    });
                }
            }

            var test = list;
            connection.Close();
            return list;
        }
    }
}