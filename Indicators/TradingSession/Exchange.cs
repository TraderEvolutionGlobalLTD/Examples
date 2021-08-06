using Runtime.Script;
using System;

namespace TradingSessions
{
    /// <summary>
    /// Helper class for Exchange entity
    /// </summary>
    class Exchange
    {
        #region Properties
        /// <summary>
        /// Exchange name in Upper case
        /// </summary>
        public string UpperCaseName { get { return Name.ToUpper(); } }

        /// <summary>
        /// Exchange's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Exchange's group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Exchange's begin time with UTC offset
        /// </summary>
        public TimeSpan BeginTime
        {
            get
            {
                return beginTime;
            }
            set { beginTime = value; }
        }
        private TimeSpan beginTime;
        /// <summary>
        /// Exchange's end time with UTC offset
        /// </summary>
        public TimeSpan EndTime
        {
            get
            {
                return endTime;
            }
            set { endTime = value; }
        }
        private TimeSpan endTime;

        /// <summary>
        /// Exchange's time zone info
        /// </summary>
        public TimeZoneInfo Tzi
        {
            get
            {
                return tzi;
            }
            set { tzi = value; }
        }
        private TimeZoneInfo tzi;
        public bool IsEnable { get; set; }
        public DateTime ExchangeDateTimeUTC(DateTime date, TimeSpan _exchangeDatetime, TimeZoneInfo _tzi = null) 
        {
            var newDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(date, DateTimeKind.Unspecified), _tzi != null ? _tzi : tzi);

            var offsetUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(newDate.Date + _exchangeDatetime, DateTimeKind.Unspecified), _tzi != null ? _tzi : tzi);

            return offsetUTC;
        }

        /// <summary>
        /// Total hours of Exchanges's work time
        /// </summary>
        public double DurationInHours { get { return (BeginTime > EndTime) ? (EndTime - BeginTime).TotalHours + 24 : (EndTime - BeginTime).TotalHours; } }

        /// <summary>
        /// Exchanges's settings
        /// </summary>
        public Settings Settings { get; set; }

        #endregion
        public Exchange(string group, string name, string beginTime, string endTime, string timezone)
        {
            Group = group;
            Name = name;
            BeginTime = TimeSpan.Parse(beginTime);
            EndTime = TimeSpan.Parse(endTime);
            tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }

        public override string ToString()
        {
            return UpperCaseName;
        }
    }

    class CustomExchange
    {
        public CustomExchange(string name, bool isEnable, TimeInterval timeInterval, TimeZoneInfo timeZoneInfo) {
            Name = name;
            IsEnable = isEnable;
            TimeInterval = timeInterval;
            TimeZoneInfo = timeZoneInfo;
        }
        public string Name { get; set; }
        public bool IsEnable { get; set; }
        public TimeInterval TimeInterval { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}
