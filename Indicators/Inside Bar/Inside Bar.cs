using System;
using System.Drawing;
using Runtime.Script;
using Runtime.Script.Charts;
using TradeApi.History;
using TradeApi.Indicators;

namespace InsideBar
{
    public class InsideBar : IndicatorBuilder
    {
        public InsideBar()
            : base()
        {
            Lines.Set("bullish bar");
            Lines["bullish bar"].Color = Color.Green;
            Lines["bullish bar"].Width = 5;

            Lines.Set("bearish bar");
            Lines["bearish bar"].Color = Color.Red;
            Lines["bearish bar"].Width = 5;

            SeparateWindow = false;
        }

        private int bullishBar = 1;
        private int bearishBar = -1;
        private double offsetPrimitive = 0D;
        private Brush boxBrush = new SolidBrush(Color.LightGray);
        public override void Init()
        {
            Lines["bullish bar"].Visible = false;
            Lines["bearish bar"].Visible = false;
        }
        public override void Update(TickStatus args)
        {
            if (args != TickStatus.IsQuote && HistoryDataSeries.Count - 1 > 0)
            {

                var message = "";
                var _isInside = isInside();
                var isInsideBarMade = _isInside == bullishBar || _isInside == bearishBar;

                if (isInsideBarMade)
                {
                    message = "Inside Bar came up!";
                }

                if (_isInside == bullishBar)
                {
                    DrawBarStamp(Lines["bullish bar"].Color, PrimitiveFigure.TriangleUp, false, 20);
                }

                if (_isInside == bearishBar)
                {
                    DrawBarStamp(Lines["bearish bar"].Color, PrimitiveFigure.TriangleDown, true, 20);
                }

                Notification.Print(message);
            }
        }

        private void DrawBarStamp(Color color, PrimitiveFigure primitiveFigure, bool up = false, double offset = 1)
        {
            var tickSize = HistoryDataSeries.HistoricalRequest.Instrument.MinimalTickSize;
            offset = offset * tickSize;
            BarStamps.Set(new BarStampPrimitiveFigure(color, 0, up ? HistoryDataSeries.GetValue(PriceType.High) + offsetPrimitive + offset
                : HistoryDataSeries.GetValue(PriceType.Low) - offsetPrimitive - offset, primitiveFigure));
        }

        private int isInside()
        {
            var bodyStatus = (HistoryDataSeries.GetValue(PriceType.Close) >= HistoryDataSeries.GetValue(PriceType.Open)) ? 1 : -1;
            var isInsidePattern = HistoryDataSeries.GetValue(PriceType.High) < HistoryDataSeries.GetValue(PriceType.High, 1)
                && HistoryDataSeries.GetValue(PriceType.Low) > HistoryDataSeries.GetValue(PriceType.Low, 1);


            return isInsidePattern ? bodyStatus : 0;
        }

        public override void OnPaintChart(object sender, PaintChartEventArgs args)
        {
            var gr = args.Graphics;

            int winNumber = ChartSource.FindWindow(this);

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
        }
    }
}
