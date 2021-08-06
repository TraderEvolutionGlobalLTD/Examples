using Runtime.Script;
using Runtime.Script.Charts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TradeApi.History;
using TradeApi.Indicators;

namespace TradingSessions
{
    public class TradingSessions : IndicatorBuilder
    {
        public TradingSessions()
            : base()
        {
            #region Initialization
            Credentials.ProjectName = "Trading Sessions";           

            SeparateWindow = false;
            settings = new Settings();

            ExchangeName_1 = settings.defaultExchanges[0].Name;
            ExchangeName_2 = settings.defaultExchanges[1].Name;
            ExchangeName_3 = settings.defaultExchanges[2].Name;
            ExchangeName_4 = settings.defaultExchanges[3].Name;
            ExchangeName_5 = settings.defaultExchanges[4].Name;
            ExchangeName_6 = settings.defaultExchanges[5].Name;

            TimeZoneInfo_1 = settings.defaultExchanges[0].Tzi;
            TimeZoneInfo_2 = settings.defaultExchanges[1].Tzi;
            TimeZoneInfo_3 = settings.defaultExchanges[2].Tzi;
            TimeZoneInfo_4 = settings.defaultExchanges[3].Tzi;
            TimeZoneInfo_5 = settings.defaultExchanges[4].Tzi;
            TimeZoneInfo_6 = settings.defaultExchanges[5].Tzi;

            ExchangeTimeInterval_1 = new TimeInterval(from: settings.defaultExchanges[0].BeginTime, to: settings.defaultExchanges[0].EndTime);
            ExchangeTimeInterval_2 = new TimeInterval(from: settings.defaultExchanges[1].BeginTime, to: settings.defaultExchanges[1].EndTime);
            ExchangeTimeInterval_3 = new TimeInterval(from: settings.defaultExchanges[2].BeginTime, to: settings.defaultExchanges[2].EndTime);
            ExchangeTimeInterval_4 = new TimeInterval(from: settings.defaultExchanges[3].BeginTime, to: settings.defaultExchanges[3].EndTime);
            ExchangeTimeInterval_5 = new TimeInterval(from: settings.defaultExchanges[4].BeginTime, to: settings.defaultExchanges[4].EndTime);
            ExchangeTimeInterval_6 = new TimeInterval(from: settings.defaultExchanges[5].BeginTime, to: settings.defaultExchanges[5].EndTime);

            #endregion
        }
        #region exchange 1
        [InputParameter(InputType.Checkbox, "Enable exchange 1", 1)]
        public bool ExchangeEnable_1 = true;

        [InputParameter(InputType.String, "Name of exchange 1", 2)]
        public string ExchangeName_1 = String.Empty;

        [InputParameter(InputType.TimeInterval, "Start/End day time of exchange 1", 3)]
        public TimeInterval ExchangeTimeInterval_1 = new TimeInterval();

        [InputParameter(InputType.TimeZoneInfo, "TimeZone of exchange 1", 4)]
        public TimeZoneInfo TimeZoneInfo_1;
        #endregion

        #region exchange 2
        [InputParameter(InputType.Checkbox, "Enable exchange 2", 5)]
        public bool ExchangeEnable_2 = true;

        [InputParameter(InputType.String, "Name of exchange 2", 6)]
        public string ExchangeName_2 = String.Empty;

        [InputParameter(InputType.TimeInterval, "Start/End day time of exchange 2", 7)]
        public TimeInterval ExchangeTimeInterval_2 = new TimeInterval();

        [InputParameter(InputType.TimeZoneInfo, "TimeZone of exchange 2", 8)]
        public TimeZoneInfo TimeZoneInfo_2;
        #endregion

        #region exchange 3
        [InputParameter(InputType.Checkbox, "Enable exchange 3", 9)]
        public bool ExchangeEnable_3 = true;

        [InputParameter(InputType.String, "Name of exchange 3", 10)]
        public string ExchangeName_3 = String.Empty;

        [InputParameter(InputType.TimeInterval, "Start/End day time of exchange 3", 11)]
        public TimeInterval ExchangeTimeInterval_3 = new TimeInterval();

        [InputParameter(InputType.TimeZoneInfo, "TimeZone of exchange 3", 12)]
        public TimeZoneInfo TimeZoneInfo_3;
        #endregion

        #region exchange 4
        [InputParameter(InputType.Checkbox, "Enable exchange 4", 13)]
        public bool ExchangeEnable_4 = true;

        [InputParameter(InputType.String, "Name of exchange 4", 14)]
        public string ExchangeName_4 = String.Empty;

        [InputParameter(InputType.TimeInterval, "Start/End day time of exchange 4", 15)]
        public TimeInterval ExchangeTimeInterval_4 = new TimeInterval();

        [InputParameter(InputType.TimeZoneInfo, "TimeZone of exchange 4", 17)]
        public TimeZoneInfo TimeZoneInfo_4;
        #endregion

        #region exchange 5
        [InputParameter(InputType.Checkbox, "Enable exchange 5", 18)]
        public bool ExchangeEnable_5 = true;

        [InputParameter(InputType.String, "Name of exchange 5", 19)]
        public string ExchangeName_5 = String.Empty;

        [InputParameter(InputType.TimeInterval, "Start/End day time of exchange 5", 20)]
        public TimeInterval ExchangeTimeInterval_5 = new TimeInterval();

        [InputParameter(InputType.TimeZoneInfo, "TimeZone of exchange 5", 21)]
        public TimeZoneInfo TimeZoneInfo_5;
        #endregion

        #region exchange 6
        [InputParameter(InputType.Checkbox, "Enable exchange 6", 22)]
        public bool ExchangeEnable_6 = true;

        [InputParameter(InputType.String, "Name of exchange 6", 23)]
        public string ExchangeName_6 = String.Empty;

        [InputParameter(InputType.TimeInterval, "Start/End day time of exchange 6", 24)]
        public TimeInterval ExchangeTimeInterval_6 = new TimeInterval();

        [InputParameter(InputType.TimeZoneInfo, "TimeZone of exchange 6", 25)]
        public TimeZoneInfo TimeZoneInfo_6;
        #endregion

        private Settings settings;

        private int totalMinutesInBar;

        private Pen boxPen = new Pen(Color.FromArgb(150, Color.LightGray), 2);

        private Brush boxBrush = new SolidBrush(Color.LightGray);

        private Font font = new Font("Arial", 8);

        private Brush linebrush = new SolidBrush(Color.LimeGreen);

        private List<CustomExchange> customExchanges = new List<CustomExchange>();

        private bool isDrawable = true;

        public override void Init()
        {
            customExchanges.Add(new CustomExchange(ExchangeName_1, ExchangeEnable_1, ExchangeTimeInterval_1, TimeZoneInfo_1));
            customExchanges.Add(new CustomExchange(ExchangeName_2, ExchangeEnable_2, ExchangeTimeInterval_2, TimeZoneInfo_2));
            customExchanges.Add(new CustomExchange(ExchangeName_3, ExchangeEnable_3, ExchangeTimeInterval_3, TimeZoneInfo_3));
            customExchanges.Add(new CustomExchange(ExchangeName_4, ExchangeEnable_4, ExchangeTimeInterval_4, TimeZoneInfo_4));
            customExchanges.Add(new CustomExchange(ExchangeName_5, ExchangeEnable_5, ExchangeTimeInterval_5, TimeZoneInfo_5));
            customExchanges.Add(new CustomExchange(ExchangeName_6, ExchangeEnable_6, ExchangeTimeInterval_6, TimeZoneInfo_6));
            foreach (var item in settings.defaultExchanges)
            {
                item.Settings = settings;
            }

            totalMinutesInBar = getTotalMinutes();

            if ((HistoryDataSeries.HistoricalRequest as TimeHistoricalRequest).Period == Period.Second) {
                Notification.Alert("This timeframe is not valid");

                isDrawable = false;
            }

            var index = 0;
            settings.defaultExchanges.ForEach(x => {
                var _customExchange = customExchanges[index];
                if (_customExchange.IsEnable)
                {
                    x.Name = _customExchange.Name;
                    x.BeginTime = _customExchange.TimeInterval.From;
                    x.EndTime = _customExchange.TimeInterval.To;
                    x.Tzi = _customExchange.TimeZoneInfo;
                    x.IsEnable = _customExchange.IsEnable;
                }
                index++;
            });
        }
        public override void Update(TickStatus args)
        {
        }
        public int getTotalMinutes() {
            int minutesInPeriod;
            var periodType = (HistoryDataSeries.HistoricalRequest as TimeHistoricalRequest).Period;
            var periodValue = (HistoryDataSeries.HistoricalRequest as TimeHistoricalRequest).Value;
            switch (periodType)
            {
                case Period.Minute:
                    minutesInPeriod = (int)TimeSpan.FromMinutes(1).TotalMinutes;
                    break;

                case Period.Hour:
                    minutesInPeriod = (int)TimeSpan.FromHours(1).TotalMinutes;
                    break;

                case Period.Day:
                    minutesInPeriod = (int)TimeSpan.FromDays(1).TotalMinutes;
                    break;

                case Period.Week:
                    minutesInPeriod = (int)TimeSpan.FromDays(7).TotalMinutes;
                    break;

                case Period.Month:
                    minutesInPeriod = (int)TimeSpan.FromDays(30).TotalMinutes;
                    break;

                case Period.Year:
                    minutesInPeriod = (int)TimeSpan.FromDays(365).TotalMinutes;
                    break;

                default:
                    minutesInPeriod = (int)TimeSpan.FromHours(1).TotalMinutes;
                    break;
            }

            int minutesInMajorPeriod = minutesInPeriod * periodValue;

            return minutesInMajorPeriod;
        }
        public int getClossesIndex(DateTime time) 
        {
            var resultDesc = -1;
            var _time = RoundMinutes(time);

            var resultAsc = -1;
            var _totalMinutesInBar = totalMinutesInBar;
            while (resultDesc == -1 && resultAsc ==-1) {
                resultDesc = HistoryDataSeries.FindInterval(_time.AddMinutes(-_totalMinutesInBar));
                resultAsc = HistoryDataSeries.FindInterval(_time.AddMinutes(+_totalMinutesInBar));
                _totalMinutesInBar += totalMinutesInBar;
            };

            return resultDesc == -1 ? resultAsc : resultDesc;
        }
        private DateTime RoundMinutes(DateTime Input)
        {
            int Minute;
            if ((Input.Minute <= 15) || ((Input.Minute > 30) && (Input.Minute <= 45)))
                Minute = -1;
            else
                Minute = +1;

            while ((Input.Minute != 0))
                Input = Input.AddMinutes(Minute);

            return Input;
        }
        private Point Midpoint(RectangleF r, int pastOffset)
        {
            RectangleF a = (RectangleF)r;
            float x = a.Location.X;
            float y = a.Location.Y;
            float h = a.Height;
            float w = a.Width;
            Point pp = new Point();
            pp.X = Convert.ToInt32(x + w / 2);
            var offset = (pastOffset % 2 == 0) ? h / 8 : h - h / 8;
            pp.Y = Convert.ToInt32(y + offset);
            return pp;
        }
        private bool isLeftBorder(Exchange exchange, int i, DateTime date)
        {
            return exchange.ExchangeDateTimeUTC(date, exchange.BeginTime) <= HistoryDataSeries.GetTimeUtc(i) && exchange.ExchangeDateTimeUTC(date, exchange.BeginTime) > HistoryDataSeries.GetTimeUtc(i + 1);
        }
        private bool isRightBorder(Exchange exchange, int i, DateTime date)
        {
            return exchange.ExchangeDateTimeUTC(date, exchange.EndTime) <= HistoryDataSeries.GetTimeUtc(i) && exchange.ExchangeDateTimeUTC(date, exchange.EndTime) > HistoryDataSeries.GetTimeUtc(i + 1);
        }
        private bool isFirstOnBorder(Exchange exchange, DateTime date)
        {
            return exchange.ExchangeDateTimeUTC(date, exchange.EndTime) >= HistoryDataSeries.GetTimeUtc(0);
        }
        private bool isBetweenBorder(Exchange exchange, DateTime date, int rightOffset, int leftOffset)
        {
            return exchange.ExchangeDateTimeUTC(date, exchange.EndTime) >= HistoryDataSeries.GetTimeUtc(leftOffset) && exchange.ExchangeDateTimeUTC(date, exchange.BeginTime) <= HistoryDataSeries.GetTimeUtc(rightOffset);
        }
        private List<BoxContainer> fillBoxes(int rightOffset, int leftOffset, List<Exchange> properExchanges, int winNumber)
        {
            var listToDraw = new List<BoxContainer>();
            for (int i = rightOffset; i <= leftOffset - 1; i++)
            {
               var date = HistoryDataSeries.GetTimeUtc(i);
               var exchanges = properExchanges.FindAll(exchange => exchange.IsEnable && isLeftBorder(exchange, i, date) 
               || exchange.IsEnable && isRightBorder(exchange, i, date)
               || exchange.IsEnable && isFirstOnBorder(exchange, date)
               || exchange.IsEnable && isBetweenBorder(exchange, date, rightOffset, leftOffset));

                exchanges.ForEach(exchange =>
                {
                    var newBox = new BoxContainer();
                    var beginDateTime = exchange.ExchangeDateTimeUTC(date, exchange.BeginTime);
                    var endDateTime = exchange.ExchangeDateTimeUTC(date, exchange.EndTime);
                    newBox.Name = exchange.Name;
                    var beginDateTimeIndex = HistoryDataSeries.FindInterval(beginDateTime) == -1 ? getClossesIndex(beginDateTime) : HistoryDataSeries.FindInterval(beginDateTime);
                    var endDateTimeIndex = HistoryDataSeries.FindInterval(endDateTime) == -1 ? getClossesIndex(endDateTime) : HistoryDataSeries.FindInterval(endDateTime);
                    var min = HistoryDataSeries.GetTimeUtc(beginDateTimeIndex) < HistoryDataSeries.GetTimeUtc(endDateTimeIndex) ? HistoryDataSeries.GetTimeUtc(beginDateTimeIndex) : HistoryDataSeries.GetTimeUtc(endDateTimeIndex);
                    var max = HistoryDataSeries.GetTimeUtc(beginDateTimeIndex) > HistoryDataSeries.GetTimeUtc(endDateTimeIndex) ? HistoryDataSeries.GetTimeUtc(beginDateTimeIndex) : HistoryDataSeries.GetTimeUtc(endDateTimeIndex);
                    var highest = HistoryDataSeries.GetValue(PriceType.High, HistoryDataSeries.GetHighest(PriceType.High, new Interval(min, max))[0]);
                    var lowest = HistoryDataSeries.GetValue(PriceType.Low, HistoryDataSeries.GetLowest(PriceType.Low, new Interval(min, max))[0]);
                    newBox.LeftPoint = ChartSource.GetChartPoint(new TimeValue(beginDateTime, highest), winNumber);
                    newBox.RightPoint = ChartSource.GetChartPoint(new TimeValue(endDateTime, lowest), winNumber);
                    newBox.Color = beginDateTime < HistoryDataSeries.GetTimeUtc(0) && endDateTime
                    > HistoryDataSeries.GetTimeUtc(0) ? Color.FromArgb(100, Color.CornflowerBlue) : Color.Transparent;
                    listToDraw.Add(newBox);
                });
            }
            var result = listToDraw.Distinct(new Comparer()).ToList();
            return result;
        }

        class Comparer : IEqualityComparer<BoxContainer>
        {
            public bool Equals(BoxContainer x, BoxContainer y)
            {
                return x.LeftPoint.Y == y.LeftPoint.Y && x.RightPoint.Y == x.RightPoint.Y && x.Name == y.Name;
            }

            public int GetHashCode(BoxContainer obj)
            {
                return (obj.Name).GetHashCode();
            }
        }
        public override void OnPaintChart(object sender, PaintChartEventArgs args)
        {
            if (!isDrawable)
            {
                return;
            }

            var gr = args.Graphics;
            var winNumber = ChartSource.FindWindow(this);

            if (winNumber != 0)
            {
                using (StringFormat format = new StringFormat())
                {
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    gr.DrawString("The indicator operates only on the main chart", new Font("Arial", 24), boxBrush, args.Rectangle, format);
                }
                return;
            }

            var winRect = ChartSource.GetAllWindows()[winNumber].WindowRectangle;
            args.Graphics.SetClip(winRect);

            var leftPoint = winRect.Location;
            var rightPoint = new Point(leftPoint.X + winRect.Width, leftPoint.Y);
            var leftTime = ChartSource.GetTimeValue(leftPoint, winNumber).TimeUtc;
            var rightTime = ChartSource.GetTimeValue(rightPoint, winNumber).TimeUtc;
            var leftOffset = HistoryDataSeries.FindInterval(leftTime);
            var rightOffset = HistoryDataSeries.FindInterval(rightTime);

            if (leftOffset == -1) 
            {
                leftOffset = HistoryDataSeries.Count - 1;
            }

            if (rightOffset == -1)
            {
                rightOffset = 0;
            }               

            List<Exchange> properExchanges = settings.defaultExchanges;
            List<BoxContainer> listToDraw = fillBoxes(rightOffset, leftOffset, settings.defaultExchanges, winNumber);

            listToDraw.ForEach(el =>
            {
                var rect = new RectangleF(el.LeftPoint.X, el.LeftPoint.Y, Math.Abs(el.LeftPoint.X - el.RightPoint.X), Math.Abs(el.LeftPoint.Y - el.RightPoint.Y));

                boxPen.DashPattern = new float[] { 2, 2 };
                gr.DrawRectangle(boxPen, rect.X, rect.Y, rect.Width, rect.Height);

                using (Brush activeBrush = new SolidBrush(el.Color)) {
                    gr.FillRectangle(activeBrush, rect);
                }                

            });

            var offset = 0;
            listToDraw.ForEach(el =>
            {                
                var rect = new RectangleF(el.LeftPoint.X, el.LeftPoint.Y, Math.Abs(el.LeftPoint.X - el.RightPoint.X), Math.Abs(el.LeftPoint.Y - el.RightPoint.Y));
                var midp = Midpoint(rect, offset);              

                using (StringFormat format = new StringFormat()) {
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    gr.DrawString(el.Name, font, boxBrush, midp.X, midp.Y, format);
                }
                offset++;
            });

            var todayDate = HistoryDataSeries.GetTimeUtc(0).Date;
            var tomorrowDate = HistoryDataSeries.GetTimeUtc(0).Date.AddDays(1).Date;
            var todayExchanges = properExchanges.FindAll(x => x.ExchangeDateTimeUTC(todayDate, x.BeginTime) > HistoryDataSeries.GetTimeUtc(0));
            var tomorrowExchanges = properExchanges.FindAll(x => x.ExchangeDateTimeUTC(tomorrowDate, x.BeginTime) > HistoryDataSeries.GetTimeUtc(0));
            var valueTop = ChartSource.GetTimeValue(new PointF(0, 0), winNumber).Value;
            var valueBottom = ChartSource.GetTimeValue(new PointF(args.Rectangle.Height, args.Rectangle.Width), winNumber).Value;

            offset = 0;
            todayExchanges.ForEach(el =>
            {
                if (el.IsEnable) 
                {
                    var poinTop = ChartSource.GetChartPoint(new TimeValue(el.ExchangeDateTimeUTC(todayDate, el.BeginTime), valueTop), winNumber);
                    var poinBottom = ChartSource.GetChartPoint(new TimeValue(el.ExchangeDateTimeUTC(todayDate, el.BeginTime), valueBottom), winNumber);

                    gr.DrawLine(boxPen, poinTop, poinBottom);
                    gr.DrawString(el.Name, font, boxBrush, poinTop.X, offset % 2 == 0 ? poinTop.Y : poinTop.Y + 20);
                    offset++;
                }
            });

            offset = 0;
            tomorrowExchanges.ForEach(el =>
            {
                if (el.IsEnable)
                {
                    var poinTop = ChartSource.GetChartPoint(new TimeValue(el.ExchangeDateTimeUTC(tomorrowDate, el.BeginTime), valueTop), winNumber);
                    var poinBottom = ChartSource.GetChartPoint(new TimeValue(el.ExchangeDateTimeUTC(tomorrowDate, el.BeginTime), valueBottom), winNumber);

                    gr.DrawLine(new Pen(linebrush), poinTop, poinBottom);
                    gr.DrawString(el.Name, font, linebrush, poinTop.X, offset % 2 == 0 ? poinTop.Y + args.Rectangle.Height - 40 : poinTop.Y + args.Rectangle.Height - 60);
                    offset++;
                }
            });
        }
    }
}
