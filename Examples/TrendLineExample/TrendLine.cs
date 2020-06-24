using Runtime.Script;
using TradeApi.Trading;
using System.Collections.Generic;
using TradeApi.Indicators;
using Runtime.Script.Charts;
using System.Drawing;
using System;

namespace TestEnv
{
    public class TrendLine : IndicatorBuilder
    {
        [InputParameter(InputType.Numeric, "Price change limit, %:", 0)]
        [SimpleNumeric(0D, 10D, 2, 0.01)]
        public double _priceChangeLimit = 0.03;

        private Pen _pen = new Pen(Brushes.Yellow);

        public TrendLine()
        : base()
        {
            #region Initialization
            Credentials.ProjectName = "TrendLineExample";
            #endregion
        }

        public override void Init()
        {

        }

        public override void OnPaintChart(object sender, PaintChartEventArgs args)
        {
            if (HistoryDataSeries.Count == 0)
                return;

            int winNumber = ChartSource.FindWindow(this);
            var winRect = ChartSource.GetAllWindows()[winNumber].WindowRectangle;
            args.Graphics.SetClip(winRect);

            var leftPoint = winRect.Location;
            var rightPoint = new Point(leftPoint.X + winRect.Width, leftPoint.Y);

            var leftTime = ChartSource.GetTimeValue(leftPoint, winNumber).TimeUtc;
            var rightTime = ChartSource.GetTimeValue(rightPoint, winNumber).TimeUtc;

            int leftOffset = HistoryDataSeries.FindInterval(leftTime);
            int rightOffset = HistoryDataSeries.FindInterval(rightTime);

            if (leftOffset == -1)
                leftOffset = HistoryDataSeries.Count - 1;

            if (rightOffset == -1)
                rightOffset = 0;

            var barWidth = ChartSource.BarsWidth;

            double previousClose = double.NaN;
            var previousDt = DateTime.MinValue;

            for (int i = rightOffset; i <= leftOffset; i++)
            {
                double close = HistoryDataSeries.GetValue(TradeApi.History.PriceType.Close, i);
                var dt = HistoryDataSeries.GetTimeUtc(i);
                if (double.IsNaN(previousClose))
                {
                    previousClose = close;
                    previousDt = dt;
                    continue;
                }

                if (previousDt == dt || Math.Abs((previousClose - close) / close) < _priceChangeLimit / 100)
                    continue;


                var previousClosePoint = ChartSource.GetChartPoint(new TimeValue(previousDt, previousClose), winNumber);
                var nextClosePoint = ChartSource.GetChartPoint(new TimeValue(dt, close), winNumber);

                args.Graphics.DrawLine(_pen, new PointF(previousClosePoint.X + barWidth / 2, previousClosePoint.Y), new PointF(nextClosePoint.X + barWidth / 2, nextClosePoint.Y));
                previousClose = close;
                previousDt = dt;
            }

        }

        public override void Update(TickStatus args)
        {


        }
    }
}