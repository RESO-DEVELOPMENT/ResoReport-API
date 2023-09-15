namespace ResoReportDataService.Commons
{
    public class CommonConstants
    {
        public const long MAX_UPLOAD_FILE_SIZE = 25000000; //in bytes
        public const int DefaultPaging = 50;
        public const int LimitPaging = 500;

        public class Duration
        {
            public const string PastWeek = "PREV_WEEK";
            public const string PastMonth = "PREV_MONTH";
            public const string Past90Days = "90_DAYS";
            public const string Past7Days = "7_DAYS";
            public const string Past30Days = "30_DAYS";
        }

        public class ErrorMessage
        {
            public const string InvalidProduct = "Sản phẩm không tồn tại!";
            public const string InvalidQuantity = "Số lượng không phải kiểu số!";
        }
    }
}