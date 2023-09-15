using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ResoReportDataService.Commons;

namespace ResoReportDataService.RequestModels
{
    public class DateFilter
    {
        [DataType(DataType.DateTime)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ToDate { get; set; }
    }
}