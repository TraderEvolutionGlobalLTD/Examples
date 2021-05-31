using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Runtime.Script;
using Runtime.Script.Charts;
using TradeApi;
using TradeApi.History;
using TradeApi.Indicators;
using TradeApi.Instruments;
using TradeApi.ToolBelt;


namespace CustomIndicator
{
	/// <summary>
	/// CustomIndicator
	/// 
	/// </summary>
	public class CustomIndicator : IndicatorBuilder
	{
		public CustomIndicator()
			: base()
		{
			#region Initialization
			Credentials.Author = "";
			Credentials.Company = "";
			Credentials.Copyrights = "";
			Credentials.DateOfCreation = new DateTime(2021, 5, 31);
			Credentials.ExpirationDate = DateTime.MinValue;
			Credentials.Version = "";
			Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
			Credentials.ProjectName = "CustomIndicator";
			#endregion

			Lines.Set("MinMax");
			Lines["MinMax"].Color = Color.Blue;

			SeparateWindow = false;
		}

		[InputParameter(InputType.Color, "Color for minimum:", 0)]
		public Color minColor = Color.Blue;

		[InputParameter(InputType.Color, "Color for maximum:", 1)]
		public Color maxColor = Color.Red;


		double minValue = double.NaN;
		double maxValue = double.NaN;
		Brush minBrush;
		Brush maxBrush;

		/// <summary>
		/// This function will be called after creating
		/// </summary>
		public override void Init()
		{
			minValue = double.NaN;
			maxValue = double.NaN;

			minBrush = new SolidBrush(minColor);
			maxBrush = new SolidBrush(maxColor);
		}

		/// <summary>
		/// Entry point. This function is called when new quote comes or new bar created
		/// </summary>
		public override void Update(TickStatus args)
		{
			double high = HistoryDataSeries.GetValue(PriceType.High);
			double low = HistoryDataSeries.GetValue(PriceType.Low);

			int val = 0;

			if (double.IsNaN(maxValue) || high >= maxValue)
			{
				maxValue = high;
				val = 1;
			}

			if (double.IsNaN(minValue) || low <= minValue)
			{
				minValue = low;
				val = -1;
			}
			Lines["MinMax"].SetValue(val);

			//Clear previuse min/max
			if (val != 0 && HistoryDataSeries.Count > 1)
			{
				for (int i = 1; i < HistoryDataSeries.Count; i++)
				{
					double prevValue = Lines["MinMax"].GetValue(i);
					double prevHigh = HistoryDataSeries.GetValue(PriceType.High, i);
					double prevLow = HistoryDataSeries.GetValue(PriceType.Low, i);

					if (prevValue == -1 && prevLow > minValue)
						Lines["MinMax"].SetValue(0, i);

					if (prevValue == 1 && prevHigh < maxValue)
						Lines["MinMax"].SetValue(0, i);
				}
			}
		}

		/// <summary>
		/// This function will be called before removing
		/// </summary>
		public override void Complete()
		{

		}

		public override void OnPaintChart(object sender, PaintChartEventArgs args)
		{
			if (HistoryDataSeries.Count == 0)
				return;

			//draw circle on min max

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

			for (int i = rightOffset; i <= leftOffset; i++)
			{
				double val = Lines["MinMax"].GetValue(i);

				if (val == 0)
					continue;

				var dt = HistoryDataSeries.GetTimeUtc(i);
				var price = val == 1
					? HistoryDataSeries.GetValue(PriceType.High, i)
					: HistoryDataSeries.GetValue(PriceType.Low, i);

				var curPoint = ChartSource.GetChartPoint(new TimeValue(dt, price), winNumber);
				var brush = val == 1 ? maxBrush : minBrush;

				args.Graphics.FillEllipse(brush, curPoint.X, curPoint.Y - barWidth / 2,
					  barWidth, barWidth);

			}
		}
	}
}
