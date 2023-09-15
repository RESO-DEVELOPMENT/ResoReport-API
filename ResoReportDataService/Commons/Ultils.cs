using System;

namespace ResoReportDataService.Commons
{
    public static class Utils
    {
        public static (DateTime, DateTime) GetLastAndFirstDateInCurrentMonth()
        {
            var now = DateTime.Now;
            var first = new DateTime(now.Year, now.Month, 1);
            var last = first.AddMonths(1).AddDays(-1);
            return (first, last);
        }

        public static DateTime GetStartOfDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
        }

        public static DateTime GetEndOfDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
        }

        public static DateTime GetCurrentDate()
        {
            return DateTime.UtcNow.AddHours(7);
        }

        public static (DateTime, DateTime) GetPastWeek()
        {
            DayOfWeek currentDay = GetCurrentDate().DayOfWeek;
            int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            DateTime currentWeekStartDate = DateTime.Now.AddDays(-daysTillCurrentDay);
            return (currentWeekStartDate.AddDays(-7).GetStartOfDate(),
                currentWeekStartDate.AddDays(-7).AddDays(6).GetEndOfDate());
        }

        public static (DateTime, DateTime) GetPreviousPastWeek()
        {
            DayOfWeek currentDay = GetCurrentDate().DayOfWeek;
            int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            DateTime currentWeekStartDate = DateTime.Now.AddDays(-daysTillCurrentDay);
            return (currentWeekStartDate.AddDays(-14).GetStartOfDate(),
                currentWeekStartDate.AddDays(-14).AddDays(6).GetEndOfDate());
        }

        public static (DateTime, DateTime) GetPastMonth()
        {
            var currentDay = GetCurrentDate().Date;
            var monthStart = new DateTime(currentDay.Year, currentDay.Month, 1);
            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart.AddDays(-1);
            return (lastMonthStart.GetStartOfDate(), lastMonthEnd.GetEndOfDate());
        }

        public static (DateTime, DateTime) GetPreviousPastMonth()
        {
            var currentDay = GetCurrentDate().Date;
            var monthStart = new DateTime(currentDay.Year, currentDay.Month, 1);
            var lastMonthStart = monthStart.AddMonths(-2);
            var lastMonthEnd = monthStart.AddMonths(-1).AddDays(-1);
            return (lastMonthStart.GetStartOfDate(), lastMonthEnd.GetEndOfDate());
        }

        public static (DateTime, DateTime) GetPast90Days()
        {
            return (GetCurrentDate().AddDays(-91).GetStartOfDate(), GetCurrentDate().GetEndOfDate().AddDays(-1));
        }

        public static (DateTime, DateTime) GetPreviousPast90Days()
        {
            return (GetCurrentDate().AddDays(-181).GetStartOfDate(), GetCurrentDate().GetEndOfDate().AddDays(-91));
        }

        public static (DateTime, DateTime) GetPast7Days()
        {
            return (GetCurrentDate().AddDays(-8).GetStartOfDate(), GetCurrentDate().GetEndOfDate().AddDays(-1));
        }

        public static (DateTime, DateTime) GetPreviousPast7Days()
        {
            return (GetCurrentDate().AddDays(-15).GetStartOfDate(), GetCurrentDate().GetEndOfDate().AddDays(-8));
        }

        public static (DateTime, DateTime) GetPast30Days()
        {
            return (GetCurrentDate().AddDays(-31).GetStartOfDate(), GetCurrentDate().GetEndOfDate().AddDays(-1));
        }

        public static (DateTime, DateTime) GetPreviousPast30Days()
        {
            return (GetCurrentDate().AddDays(-61).GetStartOfDate(), GetCurrentDate().GetEndOfDate().AddDays(-31));
        }
    }
}