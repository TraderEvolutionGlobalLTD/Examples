using Runtime.Script;
using Runtime.Script.Charts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeApi.Indicators;
using TradeApi.History;
using System.Drawing.Drawing2D;

namespace DrawTextOnCandle
{
    public class DrawTextOnCandle : IndicatorBuilder
    {
        Color textColor;
        SolidBrush drawBrush;

        public DrawTextOnCandle() : base()
        {
            #region Initialization
            Credentials.Author = "";
            Credentials.Company = "";
            Credentials.Copyrights = "";
            Credentials.DateOfCreation = new DateTime(2021, 9, 6);
            Credentials.ExpirationDate = DateTime.MinValue;
            Credentials.Version = "";
            Credentials.ProjectName = "DrawTextOnCandle";
            #endregion
        }
        public override void OnPaintChart(object sender, PaintChartEventArgs args)
        {
            var winNumber = ChartSource.FindWindow(this);
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
            int fontSize = barWidth / 3;
            Font drawFont = new Font("Arial", fontSize);

            int counter = 1;

            for (int i = rightOffset; i <= leftOffset; i++)
            {
                double open = HistoryDataSeries.GetValue(PriceType.Open, i);
                double close = HistoryDataSeries.GetValue(PriceType.Close, i);
                double high = HistoryDataSeries.GetValue(PriceType.High, i);
                double low = HistoryDataSeries.GetValue(PriceType.Low, i);
                var dt = HistoryDataSeries.GetTimeUtc(i);

                var openPoint = ChartSource.GetChartPoint(new TimeValue(dt, open), winNumber);
                var closePoint = ChartSource.GetChartPoint(new TimeValue(dt, close), winNumber);
                var highPoint = ChartSource.GetChartPoint(new TimeValue(dt, high), winNumber);
                var lowPoint = ChartSource.GetChartPoint(new TimeValue(dt, low), winNumber);

                var lowerPoint = open < close ? openPoint : closePoint;
                var upperPoint = open > close ? openPoint : closePoint;


                var candleBodyRect = new Rectangle((int)upperPoint.X + 6, (int)upperPoint.Y, barWidth - 10, (int)lowerPoint.Y - (int)upperPoint.Y);
                var candleFullRect = new Rectangle((int)highPoint.X, (int)highPoint.Y, barWidth, (int)lowPoint.Y - (int)highPoint.Y);

                string drawString = string.Empty;

                int x = 0;
                int y = 0;
                float angle = 0;

                switch (counter)
                {
                    case 1:
                        //left vertical
                        x = candleBodyRect.Left - (fontSize + (fontSize / 2)); // from the left edge of the candle body
                        y = candleFullRect.Bottom; // from the bottom of the whole candle
                        angle = -90; //draw from bottom to top
                        drawString = "left vertical";
                        break;
                    case 5:
                        //top horizontal from center
                        x = candleBodyRect.Left + (candleBodyRect.Width / 2); // from the left edge of the candle body, taking into account the font size
                        y = candleFullRect.Top - (fontSize + (fontSize / 2)); // from the bottom of the whole candle
                        drawString = "top horizontal";
                        break;
                    case 9:
                        //bottom horizontal from left edge of the whole candle
                        x = candleFullRect.Left; // from the left edge of the whole candle body
                        y = candleFullRect.Bottom; // from the bottom of the whole candle
                        drawString = "bottom horizontal";
                        break;
                    case 13:
                        //center vertical from top to bootom under the whole candle
                        x = candleBodyRect.Left + (candleBodyRect.Width / 2) + ((fontSize + (fontSize / 2)) / 2);
                        y = candleFullRect.Bottom;
                        angle = 90;//from top to bottom
                        drawString = "bottom center horizontal";
                        break;

                }
                if (++counter > 13)
                    counter = 0;
                if (string.IsNullOrEmpty(drawString))
                    continue;
                if (angle != 0)
                    DrawRotatedTextAt(args.Graphics, angle, drawString, drawFont, drawBrush, x, y);
                else
                    args.Graphics.DrawString(drawString, drawFont, drawBrush, x, y);
            }
        }

        public override void Init()
        {
            textColor = Color.Green;
            drawBrush = new SolidBrush(textColor);
        }

        public override void Update(TickStatus args)
        {
        }

        public override void Complete()
        {
        }
        private void DrawRotatedTextAt(Graphics gr, float angle, string text, Font font, Brush brush, int x, int y)
        {
            GraphicsState state = gr.Save();
            gr.ResetTransform();
            gr.RotateTransform(angle);
            gr.TranslateTransform(x, y, MatrixOrder.Append);
            gr.DrawString(text, font, brush, 0, 0);
            gr.Restore(state);
        }
    }
}
