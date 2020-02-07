﻿using Runtime.Script;
using Runtime.Script.Charts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeApi.History;
using TradeApi.Indicators;
using TradeApi.Instruments;
using TradeApi.Quotes;

namespace DiffCloseTwoSymbols
{
    public class DiffClose : IndicatorBuilder
    {
        [InputParameter(InputType.Instrument, "Second instrument:", 0)]
        public Instrument instrument2 = null;
        #region Properties
        Instrument instrument1;
        HistoricalData hd1, hd2;
        #endregion Properties

        public DiffClose() 
            : base()
        {
            #region Initialization
            Credentials.Author = "";
            Credentials.Description = "";
            Credentials.Company = "";
            Credentials.Copyrights = "";
            Credentials.DateOfCreation = new DateTime(2020, 1, 16);
            Credentials.ExpirationDate = DateTime.MinValue;
            Credentials.Version = "";
            Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
            Credentials.ProjectName = "Two_DDBB";
            #endregion


            Lines.Set("Open");
            Lines["Open"].Color = Color.Red;
            Lines["Open"].Style = LineStyle.Dot;
            Lines["Open"].Width = 3;

            Lines.Set("High");
            Lines["High"].Color = Color.Green;
            Lines["High"].Style = LineStyle.Dot;
            Lines["High"].Width = 3;

            Lines.Set("Low");
            Lines["Low"].Color = Color.Yellow;
            Lines["Low"].Style = LineStyle.Dot;
            Lines["Low"].Width = 3;

            Lines.Set("Close");
            Lines["Close"].Color = Color.Blue;
            Lines["Close"].Style = LineStyle.Dot;
            Lines["Close"].Width = 3;
        }

        public override void Init()
        {

            instrument1 = InstrumentsManager.Current;

            if (instrument2 == null || instrument1 == null)
                return;

            var timeRequest = HistoryDataSeries.HistoricalRequest as TimeHistoricalRequest;
            if (timeRequest == null)
                return;

            var dataType = timeRequest.DataType;

            InstrumentsManager.Subscribe(instrument1, dataType == DataType.Trade ? QuoteTypes.Trade : QuoteTypes.Quote);
            InstrumentsManager.Subscribe(instrument2, dataType == DataType.Trade ? QuoteTypes.Trade : QuoteTypes.Quote);

            InstrumentsManager.OnBBO += InstrumentsManager_OnBBO;
            InstrumentsManager.OnTrades += InstrumentsManager_OnTrades;
            HistoricalDataManager.OnLoaded += HistoricalDataManager_OnLoaded;
        }

        void HistoricalDataManager_OnLoaded(HistoricalData hd) 
        {
            for (int i1 = 0; i1 < hd1.Count; i1++)
                SetDiff(i1);
        }

        void SetDiff(int index1)
        {
            if (hd2 == null || hd1 == null)
                return;

            var time1 = hd1.GetTimeUtc(index1);
            var index2 = hd2.FindInterval(time1);

            if (index2 < 0)
                return;

            var open = hd1.GetValue(PriceType.Open, index1) - hd2.GetValue(PriceType.Open, index2);
            var close = hd1.GetValue(PriceType.Close, index1) - hd2.GetValue(PriceType.Close, index2);
            var high = hd1.GetValue(PriceType.High, index1) - hd2.GetValue(PriceType.High, index2);
            var low = hd1.GetValue(PriceType.Low, index1) - hd2.GetValue(PriceType.Low, index2);

            var min = Math.Min(open, close);
            var max = Math.Max(open, close);

            if (low > min)
                low = min;

            if (high < max)
                high = max;

            Lines["Open"].SetValue(open, index1);
            Lines["High"].SetValue(high, index1);
            Lines["Low"].SetValue(low, index1);
            Lines["Close"].SetValue(close, index1);
        }

        void InstrumentsManager_OnBBO(BBO bbo) 
        {
        	QuoteProccessed();
        }
        
		void InstrumentsManager_OnTrades(Trade bbo)
		{
			QuoteProccessed();
		}

        void QuoteProccessed()
        {
        	SetDiff(0);
        }

        public override void Update(TickStatus args) 
        { 
            if(HistoryDataSeries.Count == 1)
            {
                var timeRequest = HistoryDataSeries.HistoricalRequest as TimeHistoricalRequest;
                if (timeRequest == null)
                    return;

                var interval = new Interval(HistoryDataSeries.GetTimeUtc(HistoryDataSeries.Count-1), DateTime.UtcNow);
                hd1 = HistoryDataSeries;
                hd2 = HistoricalDataManager.Get(timeRequest.GetRequest(instrument2), interval);
            }
        }

        public override void Complete()
        {
            if (instrument2 == null || instrument1 == null)
                return;

            InstrumentsManager.OnBBO -= InstrumentsManager_OnBBO;
            InstrumentsManager.OnTrades -= InstrumentsManager_OnTrades;

            var dataType = HistoryDataSeries.HistoricalRequest.DataType;

            InstrumentsManager.UnSubscribe(instrument1, dataType == DataType.Trade ? QuoteTypes.Trade : QuoteTypes.Quote);
            InstrumentsManager.UnSubscribe(instrument2, dataType == DataType.Trade ? QuoteTypes.Trade : QuoteTypes.Quote);

            HistoricalDataManager.OnLoaded -= HistoricalDataManager_OnLoaded;
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

            for (int i = rightOffset; i<= leftOffset;i++ )
            {
                double open = Lines["Open"].GetValue(i);
                double high = Lines["High"].GetValue(i);
                double low = Lines["Low"].GetValue(i);
                double close = Lines["Close"].GetValue(i);
                var dt = HistoryDataSeries.GetTimeUtc(i);

                var openPoint = ChartSource.GetChartPoint(new TimeValue(dt, open), winNumber);
                var highPoint = ChartSource.GetChartPoint(new TimeValue(dt, high), winNumber);
                var lowPoint =  ChartSource.GetChartPoint(new TimeValue(dt, low), winNumber);
                var closePoint = ChartSource.GetChartPoint(new TimeValue(dt, close), winNumber);

                var lowerPoint = open < close ? openPoint : closePoint;
                var upperPoint = open > close ? openPoint : closePoint;

                float middlex = lowerPoint.X + barWidth / 2;

                args.Graphics.DrawLine(pen, new PointF(middlex, lowPoint.Y), new PointF(middlex, lowerPoint.Y));
                args.Graphics.DrawLine(pen, new PointF(middlex, highPoint.Y), new PointF(middlex, upperPoint.Y));

                float decimalPart = barWidth / 10;
                var leftTopPoint = new PointF(upperPoint.X + decimalPart, upperPoint.Y);
                var rightTopPoint = new PointF(upperPoint.X  + barWidth - decimalPart, upperPoint.Y);

                var leftBottomPoint = new PointF(lowerPoint.X + decimalPart, lowerPoint.Y);
                var rightBottomPoint = new PointF(lowerPoint.X + barWidth - decimalPart, lowerPoint.Y);

                args.Graphics.DrawLine(pen, leftTopPoint, rightTopPoint);
                args.Graphics.DrawLine(pen, leftBottomPoint, rightBottomPoint);
                args.Graphics.DrawLine(pen, leftTopPoint, leftBottomPoint);
                args.Graphics.DrawLine(pen, rightTopPoint, rightBottomPoint);

            }
        }

        Pen pen = new Pen(Color.DarkOrange, 2);

    }
}
