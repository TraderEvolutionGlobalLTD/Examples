using System;
using System.Drawing;
using Runtime.Script;
using TradeApi.History;
using TradeApi.Indicators;
using TradeApi.Instruments;

namespace DWM
{
    public class DWM : IndicatorBuilder
    {
        public DWM()
            : base()
        {
            #region Initialization
            Credentials.ProjectName = "DWM";
            #endregion

            Lines.Set("daily");
            Lines["daily"].Color = Color.Blue;

            Lines.Set("weekly");
            Lines["weekly"].Color = Color.Red;

            Lines.Set("monthly");
            Lines["monthly"].Color = Color.Green;

            SeparateWindow = false;
        }

        [InputParameter(InputType.Numeric, "days", 0)]
        [SimpleNumeric(1D, 5000D)]
        public int d = 1;

        [InputParameter(InputType.Numeric, "week", 1)]
        [SimpleNumeric(1D, 5000D)]
        public int w = 1;

        [InputParameter(InputType.Numeric, "months", 2)]
        [SimpleNumeric(1D, 5000D)]
        public int m = 1;

        Instrument inst;

        BarData dailyBars, weeklyBars, monthlyBars;

        double dailyClose, weeklyClose, monthlyClose;

        bool historyRequested;
        bool dailyHistory, weeklyHistory, monthlyHistory;

        //int dcount, wcount, mcount;

        public override void Init()
        {
            HistoricalDataManager.OnLoaded += OnLoadedHandler;

            dailyClose = double.NaN;
            weeklyClose = double.NaN;
            monthlyClose = double.NaN;
        }


        private void OnLoadedHandler(HistoricalData data)
        {
            if ((data.HistoricalRequest as TimeHistoricalRequest).Period == Period.Day)
            {
                dailyHistory = true;
            }
            else if ((data.HistoricalRequest as TimeHistoricalRequest).Period == Period.Week)
            {
                weeklyHistory = true;
            }
            else if ((data.HistoricalRequest as TimeHistoricalRequest).Period == Period.Month)
            {
                monthlyHistory = true;
            }

            Process(HistoryDataSeries.Count - 1);
        }

        private void Process(int? count = null)
        {
            dailyClose = dailyHistory && dailyBars.Count - 1 >= d ? dailyBars.GetClose(d) : double.NaN;

            weeklyClose = weeklyHistory && weeklyBars.Count - 1 >= w ? weeklyBars.GetClose(d) : double.NaN;

            monthlyClose = monthlyHistory && monthlyBars.Count - 1 >= m ? monthlyBars.GetClose(m) : double.NaN;

            if (count == null)
            {
                this.CalcBar(0);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    this.CalcBar(i);
                }
            }
        }

        private void CalcBar(int i) {
            var currentBarTime = HistoryDataSeries.GetTimeUtc(i);
            var indexDaily = dailyBars.FindInterval(currentBarTime);
            var indexWeekly = weeklyBars.FindInterval(currentBarTime);
            var indexMonthly = monthlyBars.FindInterval(currentBarTime);

            if (indexDaily >= 0)
            {
                var multiMaValue = dailyBars.GetClose(indexDaily);
                Lines[0].SetValue(multiMaValue, i);
            }
            if (indexWeekly >= 0)
            {
                var multiMaValue = weeklyBars.GetClose(indexWeekly);
                Lines[1].SetValue(multiMaValue, i);
            }
            if (indexMonthly >= 0)
            {
                var multiMaValue = monthlyBars.GetClose(indexMonthly);
                Lines[2].SetValue(multiMaValue, i);
            }
            string notAvailable = "is not available";
            string dClose = !double.IsNaN(dailyClose)? dailyClose.ToString() : notAvailable;
            string wClose = !double.IsNaN(weeklyClose)? weeklyClose.ToString() : notAvailable;
            string mClose = !double.IsNaN(monthlyClose)? monthlyClose.ToString() : notAvailable;
            Notification.Comment($"\n\n \n\n\n          Daily Close  {dClose}\n\n          Weekly Close  {wClose}\n\n          Monthy Close  {mClose}");
        }

        public override void Update(TickStatus args)
        {
            if (!historyRequested) {
                inst = InstrumentsManager.Current;

                var dayRequest = new TimeHistoricalRequest(inst, DataType.Trade, Period.Day, 1);

                var weekRequest = new TimeHistoricalRequest(inst, DataType.Trade, Period.Week, 1);

                var monthRequest = new TimeHistoricalRequest(inst, DataType.Trade, Period.Month, 1);

                dailyBars = HistoricalDataManager.Get(dayRequest, new Interval(DateTime.UtcNow.AddDays(-252).ToLocalTime(), DateTime.UtcNow.ToLocalTime())) as BarData;
                weeklyBars = HistoricalDataManager.Get(weekRequest, new Interval(DateTime.UtcNow.AddDays(-250).ToLocalTime(), DateTime.UtcNow.ToLocalTime())) as BarData;
                monthlyBars = HistoricalDataManager.Get(monthRequest, new Interval(DateTime.UtcNow.AddMonths(-160).ToLocalTime(), DateTime.UtcNow.ToLocalTime())) as BarData;
                historyRequested = true;
            }
            if (args == TickStatus.IsBar)
            {
                Process();
            }
        }
        public override void Complete()
        {
            dailyBars = null;
            weeklyBars = null;
            monthlyBars = null;
        }
    }
}
