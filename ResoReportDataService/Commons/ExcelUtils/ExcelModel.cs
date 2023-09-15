using System;
using System.Collections.Generic;

namespace ResoReportDataService.Commons.ExcelUtils
{
    public class ExcelModel<T>
    {
        public string SheetTitle { get; set; }
        public List<ColumnConfig<T>> ColumnConfigs { get; set; }
        public List<T> DataSources { get; set; }
    }

    public class ColumnConfig<T>
    {
        public string Title { get; set; }
        public string ValueType { get; set; }
        public string DataIndex { get; set; }
        public Func<object,T> Render { get; set; }
    }
}