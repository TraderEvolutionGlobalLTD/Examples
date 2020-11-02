﻿using Runtime.Script;
using Runtime.Script.Charts;
using System;
using System.Drawing;
using TradeApi.History;
using TradeApi.Indicators;

namespace Autotrend
{
    public class Autotrend : IndicatorBuilder
    {
        public Autotrend()
        : base()
        {
            #region Initialization
            Credentials.ProjectName = "Autotrend";
            #endregion

            Lines.Set("Current_Support");
            Lines["Current_Support"].Color = Color.CornflowerBlue;


            Lines.Set("Current_Resistance");
            Lines["Current_Resistance"].Color = Color.Purple;

            SeparateWindow = false;
        }

        [InputParameter(InputType.Combobox, "Source", 0)]
        [ComboboxItem("Extremum & delta", LINE_TYPE.DELTA)]
        [ComboboxItem("Two extremums", LINE_TYPE.EXM)]
        public LINE_TYPE InpLineType = LINE_TYPE.DELTA;

        [InputParameter(InputType.Numeric, "Left offset (both types)", 1)]
        [SimpleNumeric(1D, 99999D)]
        public int leftExmSide = 10;

        [InputParameter(InputType.Numeric, "Right offset (Two extremums type)", 2)]
        [SimpleNumeric(1D, 99999D)]
        public int rightExmSide = 3;

        [InputParameter(InputType.Numeric, "Current offset (extremum & delta type)", 3)]
        [SimpleNumeric(1D, 99999D)]
        public int current = 3;

        [InputParameter(InputType.Checkbox, "Use of prev bar (extremum & delta type)", 4)]
        public bool InpPrevExmBar = true;

        public int minRequiredBars;

        PointForPlot curLeftSup, curRightSup, curLeftRes, curRightRes, nullPoint;

        public override void Init()
        {
            minRequiredBars = leftExmSide * 2 + Math.Max(rightExmSide, current) * 2;
            nullPoint = new PointForPlot();
        }

        public override void Update(TickStatus args)
        {
            if (args != TickStatus.IsQuote)
            {
                if (HistoryDataSeries.Count - 1 < minRequiredBars)
                {
                    return;
                }

                int leftIndex, rightIndex;
                double delta, tmpDelta;

                switch (InpLineType)
                {
                    case LINE_TYPE.DELTA:

                        //--- Support Left Point
                        leftIndex = leftExmSide + 2;
                        for (; !isLowestLow(leftIndex, leftExmSide) && leftIndex < HistoryDataSeries.Count - 1 - minRequiredBars; leftIndex++);
                            curLeftSup = new PointForPlot(getLow(leftIndex), HistoryDataSeries.GetTimeUtc(leftIndex));                       
                        
                        //--- Support Right Point
                        rightIndex = current + 2;

                        delta = (getLow(rightIndex) - getLow(leftIndex)) / (rightIndex - leftIndex);
                        if (!InpPrevExmBar)
                        {
                            leftIndex += 1;
                        }
                        for (int tmpIndex = rightIndex - 1; tmpIndex < leftIndex; tmpIndex++)
                        {
                            tmpDelta = (getLow(tmpIndex) - curLeftSup.Price) / (tmpIndex - leftIndex);
                            if (tmpDelta > delta)
                            {
                                delta = tmpDelta;
                                rightIndex = tmpIndex;
                            }
                        }
                        curRightSup = new PointForPlot(getLow(rightIndex), HistoryDataSeries.GetTimeUtc(rightIndex));

                        //--- Resistance Left Point
                        leftIndex = leftExmSide + 2;
                        for (; !isHighestHigh(leftIndex, leftExmSide) && leftIndex < HistoryDataSeries.Count - 1 - minRequiredBars; leftIndex++) ;
                            curLeftRes = new PointForPlot(getHigh(leftIndex), HistoryDataSeries.GetTimeUtc(leftIndex));

                        //--- Resistance Right Point
                        rightIndex = current + 2;
                        delta = (getHigh(leftIndex) - getHigh(rightIndex)) / (rightIndex - leftIndex);
                        if (!InpPrevExmBar)
                        {
                            leftIndex += 1;
                        }
                        for (int tmpIndex = rightIndex - 1; tmpIndex < leftIndex; tmpIndex++)
                        {
                            tmpDelta = (curLeftRes.Price - getHigh(tmpIndex)) / (tmpIndex - leftIndex);
                            if (tmpDelta > delta)
                            {
                                delta = tmpDelta;
                                rightIndex = tmpIndex;
                            }
                        }
                        curRightRes = new PointForPlot(getHigh(rightIndex), HistoryDataSeries.GetTimeUtc(rightIndex));
                        break;

                    case LINE_TYPE.EXM:
                    default:
                        //--- Support Right Point
                        rightIndex = rightExmSide + 2;
                        for (; !isLowestLow(rightIndex, rightExmSide) && rightIndex < HistoryDataSeries.Count - 1 - minRequiredBars; rightIndex++);
                            curRightSup = new PointForPlot(getLow(rightIndex), HistoryDataSeries.GetTimeUtc(rightIndex));

                        //--- Support Left Point
                        leftIndex = rightIndex + leftExmSide;
                        for (; !isLowestLow(leftIndex, leftExmSide) && leftIndex < HistoryDataSeries.Count - 1 - minRequiredBars; leftIndex++);
                            curLeftSup = new PointForPlot(getLow(leftIndex), HistoryDataSeries.GetTimeUtc(leftIndex));

                        //--- Resistance Right Point
                        rightIndex = rightExmSide + 2;
                        for (; !isHighestHigh(rightIndex, rightExmSide) && rightIndex < HistoryDataSeries.Count - 1 - minRequiredBars; rightIndex++);
                            curRightRes = new PointForPlot(getHigh(rightIndex), HistoryDataSeries.GetTimeUtc(rightIndex));

                        //--- Resistance Left Point
                        leftIndex = rightIndex + leftExmSide;
                        for (; !isHighestHigh(leftIndex, leftExmSide) && leftIndex < HistoryDataSeries.Count - 1 - minRequiredBars; leftIndex++);
                            curLeftRes = new PointForPlot(getHigh(leftIndex), HistoryDataSeries.GetTimeUtc(leftIndex));
                        //---
                        break;
                }
            }
        }
        public override void OnPaintChart(object sender, PaintChartEventArgs args)
        {
            if (curLeftSup != nullPoint && curRightSup != nullPoint)
            {
                var point1 = ChartSource.GetChartPoint(new TimeValue(curRightSup.Time, curRightSup.Price), ChartSource.FindWindow(this));
                var point2 = ChartSource.GetChartPoint(new TimeValue(curLeftSup.Time, curLeftSup.Price), ChartSource.FindWindow(this));
                draw_beam(point1, point2, Lines[1].Color, args);
            }

            if (curLeftRes != nullPoint && curRightRes != nullPoint)
            {
                var point1 = ChartSource.GetChartPoint(new TimeValue(curRightRes.Time, curRightRes.Price), ChartSource.FindWindow(this));
                var point2 = ChartSource.GetChartPoint(new TimeValue(curLeftRes.Time, curLeftRes.Price), ChartSource.FindWindow(this));
                draw_beam(point1, point2, Lines[0].Color, args);
            }           
        }

        void draw_beam(PointF point1, PointF point2, Color color, PaintChartEventArgs args)
        {
            RectangleF rect = args.Rectangle;
            args.Graphics.SetClip(rect);
            PointF point3;
            PointF[] point_arr1;

            if (point1.X > point2.X)
            {
                var temp = new PointF(point1.X, point1.Y);
                point1 = new PointF(point2.X, point2.Y);
                point2 = new PointF(temp.X, temp.Y);
            }

            point3 = new PointF(Lerp(point1.X, point2.X, rect.Width - (rect.X + point2.X)),
                   Lerp(point1.Y, point2.Y, rect.Width - (rect.X + point2.X)));

            point_arr1 = new PointF[] { point1, point2, point3 };

            args.Graphics.DrawLines(new Pen(color, 2), point_arr1);

        }

        float Lerp(float value1, float value2, float amount)
        {
           return value1 + (value2 - value1) * amount;
        }

        double getLow(int index)
        {
            return HistoryDataSeries.GetValue(PriceType.Low, index);
        }

        double getHigh(int index)
        {
            return HistoryDataSeries.GetValue(PriceType.High, index);
        }

        bool isLowestLow(int bar, int side)
        {
            for (int i = 1; i <= side; i++)
            {
                if (getLow(bar) > getLow(bar - i) || getLow(bar) > getLow(bar + i))
                {
                    return false;
                }
            }
            return true;
        }
        bool isHighestHigh(int bar, int side)
        {
            for (int i = 1; i <= side; i++)
            {
                if (getHigh(bar) < getHigh(bar - i) || getHigh(bar) < getHigh(bar + i))
                {
                    return false;
                }
            }
            return true;
        }
    }


    public class PointForPlot
    {     
        private double price;
        private DateTime time;

        public PointForPlot() { }

        public PointForPlot(double _price, DateTime _time) 
        {
            this.setPoint(_price, _time);
        }

        public static bool operator == (PointForPlot lhs, PointForPlot rhs)
        {
            return rhs.price == lhs.price && rhs.time == lhs.time; 
        }

        public static bool operator != (PointForPlot lhs, PointForPlot rhs)
        {
            return !(rhs.price == lhs.price && rhs.time == lhs.time); 
        }

        public void setPoint(double _price, DateTime _time) {
            price = _price;
            time = _time;
        }

        public double Price { get { return price; } }

        public DateTime Time { get { return time; } }

    }

    public enum LINE_TYPE
    {
        EXM,    
        DELTA 
    };
}
