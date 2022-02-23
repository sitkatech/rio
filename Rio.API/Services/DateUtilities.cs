using System;
using System.Collections.Generic;
using System.Linq;
using Rio.EFModels.Entities;

namespace Rio.API.Services
{
    public static class DateUtilities
    {
        public const int MinimumYear = 2017;

        public enum Month
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public static string ShortMonthName(this Month month)
        {
            // "Jan", "Feb", etc.
            return month.ToString().Substring(0, 3);
        }

        public static DateTime GetEpoch()
        {
            return DateTime.Parse("1754-01-01 00:00:00.000");
        }

        public static DateTime GetLastDateInMonth(this DateTime dateInMonth)
        {
            return new DateTime(dateInMonth.Year, dateInMonth.Month,
                DateTime.DaysInMonth(dateInMonth.Year, dateInMonth.Month));
        }

        public static DateTime GetEarliestDate(DateTime firstDate, DateTime secondDate)
        {
            return Convert.ToDateTime((firstDate.IsDateBefore(secondDate)) ? firstDate : secondDate);
        }

        public static DateTime GetLatestDate(DateTime firstDate, DateTime secondDate)
        {
            return Convert.ToDateTime((firstDate.IsDateAfter(secondDate)) ? firstDate : secondDate);
        }

        public static int GetLatestWaterYear()
        {
            var today = DateTime.Today;
            return today.Month >= 11 ? today.Year + 1 : today.Year;
        }


        public static bool IsTodayOnOrBeforeDate(this DateTime dateToCheck)
        {
            return DateTime.Today.IsDateOnOrBefore(dateToCheck);
        }

        public static bool IsDateOnOrBefore(this DateTime dateToCheck, DateTime dateToCheckAgainst)
        {
            return dateToCheck.Date.CompareTo(dateToCheckAgainst.Date) < 1;
        }

        public static bool IsDateOnOrAfter(this DateTime dateToCheck, DateTime dateToCheckAgainst)
        {
            return dateToCheck.Date.CompareTo(dateToCheckAgainst.Date) > -1;
        }

        public static bool IsDateBefore(this DateTime dateToCheck, DateTime dateToCheckAgainst)
        {
            return dateToCheck.Date.CompareTo(dateToCheckAgainst.Date) < 0;
        }

        public static bool IsDateAfter(this DateTime dateToCheck, DateTime dateToCheckAgainst)
        {
            return dateToCheck.Date.CompareTo(dateToCheckAgainst.Date) > 0;
        }

        public static bool IsDateInRange(this DateTime dateToCheck, DateTime startOfRange, DateTime endOfRange)
        {
            return dateToCheck.IsDateOnOrAfter(startOfRange) && dateToCheck.IsDateOnOrBefore(endOfRange);
        }

        public static string GetDifferenceInEnglish(DateTime firstDate, DateTime secondDate, bool showMinutes)
        {
            var age = firstDate.Subtract(secondDate);

            var days = Convert.ToInt32(Math.Floor(age.TotalDays));
            var hours = Convert.ToInt32(Math.Floor(age.TotalHours - (days * 24)));
            var minutes = Convert.ToInt32(Math.Floor(age.TotalMinutes - (days * 24 * 60) - (hours * 60)));

            if (showMinutes)
            {
                return
                    $"{days} day{(days == 1 ? "" : "s")}, {hours} hour{(hours == 1 ? "" : "s")}, {minutes} minute{(minutes == 1 ? "" : "s")}";
            }

            // No minutes
            return $"{days} day{(days == 1 ? "" : "s")}, {hours} hour{(hours == 1 ? "" : "s")}";

        }

        /// <summary>
        /// Tests if two given periods overlap each other.
        /// </summary>
        /// <param name="startDate1">Base period start</param>
        /// <param name="endDate1">Base period end</param>
        /// <param name="startDate2">Test period start</param>
        /// <param name="endDate2">Test period end</param>
        /// <returns>
        /// 	<c>true</c> if the periods overlap; otherwise, <c>false</c>.
        /// </returns>
        public static bool DateRangesOverlap(DateTime startDate1, DateTime endDate1, DateTime startDate2,
            DateTime endDate2)
        {
            // More simple?
            // return !((TS < BS && TE < BS) || (TS > BE && TE > BE));

            // The version below, without comments 
            //
            //return (
            //    (TS >= BS && TS < BE) || (TE <= BE && TE > BS) || (TS <= BS && TE >= BE)
            //);

            return (
                // 1. Case:
                //
                //       TS-------TE
                //    BS------BE 
                //
                // TS is after BS but before BE
                (startDate2 >= startDate1 && startDate2 < endDate1)
                || // or

                // 2. Case
                //
                //    TS-------TE
                //        BS---------BE
                //
                // TE is before BE but after BS
                (endDate2 <= endDate1 && endDate2 > startDate1)
                || // or

                // 3. Case
                //
                //  TS----------TE
                //     BS----BE
                //
                // TS is before BS and TE is after BE
                (startDate2 <= startDate1 && endDate2 >= endDate1)
            );
        }

        /// <summary>
        /// Gets a simplified version that removes two things:
        /// - Values with zero values that aren't significant digits: (0 days, 0 hours, 36 minutes) => 36 minutes
        /// - Less significant values as net time increases. 36 minutes => 3 hours => 1 day
        /// </summary>
        /// <param name="firstDate"></param>
        /// <param name="secondDate"></param>
        /// <returns></returns>
        public static string GetDifferenceInEnglishSimplified(DateTime firstDate, DateTime secondDate)
        {
            var age = firstDate.Subtract(secondDate);

            var days = Convert.ToInt32(Math.Floor(age.TotalDays));
            var hours = Convert.ToInt32(Math.Floor(age.TotalHours - (days * 24)));
            var minutes = Convert.ToInt32(Math.Floor(age.TotalMinutes - (days * 24 * 60) - (hours * 60)));
            var seconds =
                Convert.ToInt32(Math.Floor(age.TotalSeconds - (days * 24 * 60) - (hours * 60) - (minutes * 60)));

            if (days > 0) return $"{days} day{(days == 1 ? "" : "s")}";
            if (hours > 0) return $"{hours} hour{(hours == 1 ? "" : "s")}";
            if (minutes > 0) return $"{minutes} minute{(minutes == 1 ? "" : "s")}";
            return $"{seconds} second{(seconds == 1 ? "" : "s")}";
        }

        public static int GetDifferenceInDays(DateTime firstDate, DateTime secondDate)
        {
            return firstDate.Date.Subtract(secondDate.Date).Days;
        }

        public static int GetDaysSinceToday(this DateTime dateToCheck)
        {
            return GetDifferenceInDays(DateTime.Today, dateToCheck);
        }

        public static int GetDaysFromToday(this DateTime dateToCheck)
        {
            return GetDifferenceInDays(dateToCheck, DateTime.Today);
        }

        public static DateTime GetFirstDateInYear(int calendarYear)
        {
            const int firstDayInMonth = 1;
            const int firstCalendarMonth = (int) Month.January;
            return new DateTime(calendarYear, firstCalendarMonth, firstDayInMonth);
        }

        public static DateTime GetFirstDateInYear(this DateTime date)
        {
            return GetFirstDateInYear(date.Year);
        }

        public static DateTime GetLastDateInYear(int calendarYear)
        {
            const int lastDayInMonth = 31;
            const int lastCalendarMonth = (int) Month.December;
            return new DateTime(calendarYear, lastCalendarMonth, lastDayInMonth);
        }

        /// <summary>
        /// Indicates if string would parse as a date
        /// </summary>
        public static bool IsValidDateFormat(this string stringToCheck)
        {
            DateTime dummy;
            return DateTime.TryParse(stringToCheck, out dummy);
        }

        public static DateTime GetFirstDateInMonth(this DateTime dateInMonth)
        {
            return new DateTime(dateInMonth.Year, dateInMonth.Month, 1);
        }

        public static DateTime SubtractMonths(this DateTime date, int months)
        {
            return date.AddMonths(months * -1);
        }

        public static List<int> GetRangeOfYears(int startYear, int endYear)
        {
            return Enumerable.Range(startYear, (endYear - startYear) + 1).OrderByDescending(x => x).ToList();
        }

        public static List<int> GetWaterYears(bool includeCurrentYear)
        {
            var latestWaterYear = includeCurrentYear ? DateTime.Today.Year + 1 : GetLatestWaterYear();
            return GetRangeOfYears(DateUtilities.MinimumYear, latestWaterYear);
        }

        public static int GetDefaultWaterYearToDisplay(RioDbContext dbContext)
        {
            return DateTime.Today.Year;
        }
    }
}