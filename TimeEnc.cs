using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class LDateTime
    {
        
        
        //LDateTime has a  584,554,530,872.543 (Billion) year range, spanning from -292,277,265,436 to +292,277,265,436 years, precise to the second.
        public long TimePoint; // range: -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807
        *

        public static const int secondsInHour = 3600;
        
        public static const int secondsInDay = 86400;
        
        public static const int leapSeconds = 126230400;//How many seonds in leap quad (3 normal years and 1 leap year) (365 * 3 * SecondsInDay)+(366*SecondsInDay)

        private long accumulatedSecondsFromMonths(long year, int month)
        {
            long secondsOfTargetMonth = 0;
            //get the cumulative seconds of the prior month
            for (int i = 1; i <= month; i++)
            {
                secondsOfTargetMonth += DaysInMonth(year, i) * secondsInDay;
            }
            return secondsOfTargetMonth;
        }
        private long accumlatedYearsInSeconds
        {
            get
            {
                return ((modLeapSeconds >= 366 * secondsInDay)
                    ? ((long)Math.Floor(modLeapSeconds / (double)secondsInYear)
                       * (365 * secondsInDay)) + (secondsInDay)
                    : 0);
            }
        }
        private long leapSecondOccurrences
        {
            get { return TimePoint / leapSeconds; }
        }
        public long totalLeapSeconds
        {
            get { return (long)(leapSecondOccurrences / (double)leapSeconds); }
        }
        private long modLeapSeconds
        {
            get
            {
                return Modulo(TimePoint, leapSeconds);
            }
        }
        private long secondsInYear
        {
            get { return (modLeapSeconds >= (366 * secondsInDay)) ? 365 * secondsInDay : 366 * secondsInDay; }
        }
        public long SecondsOfModYear
        {
            get
            {
                //long a = Modulo(modLeapSeconds, secondsInYear);
                //long b = Modulo((modLeapSeconds + (3 * secondsInDay)), secondsInYear);

                return (modLeapSeconds >= (366 * secondsInDay))
                    ? Modulo(modLeapSeconds - secondsInDay, secondsInYear)
                    : Modulo(modLeapSeconds, secondsInYear);
            }
        }
        private long modMinuteOfYear
        {
            get { return (long)Math.Floor(SecondsOfModYear / 60d); }
        }
        private long modHourOfYear
        {
            get { return (long)Math.Floor(modMinuteOfYear / 60d); }
        }
        private long dayOfYear
        {
            get { return (long)Math.Floor(modHourOfYear / 24d) + 1; }
        }
        public long modYear
        {
            get
            {
                return (modLeapSeconds >= (366 * secondsInDay)) ?
                        (long)Math.Floor((modLeapSeconds - secondsInDay) /
                                 (double)secondsInYear)
                    : 0;
            }
        }
        public byte DaysInMonth(long year, int month)
        {
            //year to timepoint
            switch (month)
            {
                case 0 | 1://possibly bad attempt to compensate for future errors?
                    return 31;
                case 2:
                    return (byte)((year / 4 == 0) ? 28 : 29);//((modLeapSeconds >= 366 * secondsInDay) ? 28 : 29);
                case 3:
                    return 31;
                case 4:
                    return 30;
                case 5:
                    return 31;
                case 6:
                    return 30;
                case 7:
                    return 31;
                case 8:
                    return 31;
                case 9:
                    return 30;
                case 10:
                    return 31;
                case 11:
                    return 30;
                case 12:
                    return 31;
                default:
                    return 0;
            }
        }


        public long Year
        {
            get
            {
                return (modLeapSeconds >= 366 * secondsInDay) ?
                        (leapSecondOccurrences * 4L) + (long)Math.Floor((modLeapSeconds - secondsInDay) / (double)(secondsInYear))
                    : (leapSecondOccurrences * 4L);
            }
            set
            {
                TimePoint = AccumulatedSecondsFromYear(value) +
                            accumulatedSecondsFromMonths(value, Month) +
                            (DayOfMonth * secondsInDay) +
                            (Hour * secondsInHour) +
                            (Minutes * 60) +
                            Seconds;
            }
        }
        public int Day
        {
            get { return (int)dayOfYear; }
            set
            {
                value = (int)Modulo(value, DaysInYear);
                TimePoint = AccumulatedSecondsFromYear(Year) +
                            ((value - 1) * secondsInDay) +
                            (Hour * secondsInHour) +
                            (Minutes * 60) +
                            Seconds;
            }
        }
        public int Hour
        {
            get { return (int)Modulo(modHourOfYear, 24) + 1; }
            set
            {
                value = (int)Modulo(value, 24);
                TimePoint = AccumulatedSecondsFromYear(value) +
                            (Day * secondsInDay) +
                            ((value - 1) * secondsInHour) +
                            (Minutes * 60) +
                            Seconds;
            }

        }
        public int Minutes
        {
            get { return (int)Modulo(modMinuteOfYear, 60); }
            set
            {
                value = (int)Modulo(value, 60);
                TimePoint = AccumulatedSecondsFromYear(value) +
                            (Day * secondsInDay) +
                            ((Hour - 1) * secondsInHour) +
                            (value * 60) +
                            Seconds;
            }
        }
        public int Seconds
        {
            get { return (int)Modulo(SecondsOfModYear, 60); }
            set
            {
                value = (int)Modulo(value, 60);
                TimePoint = AccumulatedSecondsFromYear(value) +
                            (Day * secondsInDay) +
                            ((Hour - 1) * secondsInHour) +
                            (Minutes * 60) +
                            value;
            }
        }

        public int Month
        {
            get
            {
                int dOy = (int)dayOfYear;
                int TotDays = 0;
                for (byte i = 1; i <= 12; i++)
                {
                    TotDays += DaysInMonth(Year, i);
                    if (TotDays >= dOy)
                    {
                        return i;
                    }
                }
                return 0;
            }
            set
            {
                value = (int)Modulo(value, 12);
                TimePoint = totalLeapSeconds + accumlatedYearsInSeconds +
                            accumulatedSecondsFromMonths(Year, value) +
                            (DayOfMonth * secondsInDay) +
                            ((Hour - 1) * secondsInHour) +
                            (Minutes * 60) +
                            Seconds;
            }
        }
        public int DayOfMonth
        {
            get
            {
                int _dayOfMonth = (int)dayOfYear;
                int culmDay = 0;
                for (int i = 1; i <= 12; i++)
                {
                    culmDay += DaysInMonth(Year, i);
                    if (culmDay >= dayOfYear)
                    {
                        return (short)(_dayOfMonth + 0);
                    }

                    _dayOfMonth -= DaysInMonth(Year, i);
                }
                return 0;
            }
            set
            {
                /*
                 * Setting the day of the month means taking the current day of the month (clamping between 1-number of days in target month)
                 * Finding the CUMULATIVE SECONDS of the TARGET MONTH + DAY OF MONTH, and adding that to the following:
                 * (Cumulative Leap Seconds) + (Accumulated Years In Seconds) + (Time of current day) ...(Hour(not of year):Minutes(not of year):Seconds(not of year))
                 * Basically we disregard the current day of the year value and assign a new day of year to the current time based off the month/day combo
                 */
                long secondsExcludingCurrentDayOfYear =
                    totalLeapSeconds + accumlatedYearsInSeconds + ((Hour - 1) * secondsInHour) + (Minutes * 60) + Seconds;
                long secondsOfTargetMonth = 0;
                long year = Year;
                //get the cumulative seconds of the prior month
                for (int i = 1; i < 12; i++)
                {
                    byte dIm = DaysInMonth(year, i);
                    secondsOfTargetMonth += dIm;
                    if (secondsOfTargetMonth >= modLeapSeconds)
                    {
                        secondsOfTargetMonth -= dIm;
                        break;
                    }
                }

                long cumulativeSecondsOfTargetDay =
                    Math.Min(Math.Max(0, (value - 1) * secondsInDay), (DaysInMonth(year, Month) - 1) * secondsInDay);

                //Final recoded point
                TimePoint = secondsExcludingCurrentDayOfYear + secondsOfTargetMonth + cumulativeSecondsOfTargetDay;
            }
        }

        public int DaysInYear
        {
            get { return (modLeapSeconds >= 366 * secondsInDay) ? 365 : 366; }
        }

        public LDateTime()
        {
            TimePoint = 0;
        }

        public LDateTime(long setTime)
        {
            TimePoint = setTime;
        }

        public LDateTime(long year, int month, int day, int hours, int minutes, int seconds)
        {
            //clamp day to between 1 and however many days are in the target month for the given year
            day = Math.Max(1, day);
            day = Math.Min(day, DaysInMonth(year, month));
            day--;
            //day = Math.Min(Math.Max(0, (day - 1) * secondsInDay), (DaysInMonth(year, month) - 1) * secondsInDay);
            long sfy = AccumulatedSecondsFromYear(year);
            long sfm = accumulatedSecondsFromMonths(year, (month - 1));
            long combined = sfy + sfm;
            TimePoint = AccumulatedSecondsFromYear(year) +
                        accumulatedSecondsFromMonths(year, (month - 1)) +
                        (day * secondsInDay) +
                        ((hours - 1) * secondsInHour) +
                        (minutes * 60) +
                        seconds;
        }
        public long ToTimePoint()
        {
            return totalLeapSeconds + accumlatedYearsInSeconds + ((dayOfYear - 1) * secondsInDay) +
                   ((Hour - 1) * secondsInHour) + (Minutes * 60) + Seconds;
        }

        public long AccumulatedSecondsFromYear(long year)
        {
            //long g = (Modulo(3, 4) * 365 * secondsInDay);
            return ((long)Math.Floor(year / 4.0d) * leapSeconds) + (Modulo(year, 4) == 0 ? 0 : (366 * secondsInDay) + ((Modulo(year, 4) - 1) * (365 * secondsInDay)));
        }
        public long Modulo(long a, long b)
        {
            return a - b * (long)Math.Floor(a / (double)b);
        }

        public string ToString()
        {
            return Year + "/" + Month + "/" + DayOfMonth + " " + Hour + ":" + Minutes + ":" + Seconds + "(Day of year: " +
                   Day + ")\t\t(TimePoint: " + TimePoint + ")";
        }
    }
}
