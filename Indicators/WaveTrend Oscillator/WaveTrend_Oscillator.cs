using Runtime.Script;
using System;
using System.Collections.Generic;
using System.Drawing;
using TradeApi.History;
using TradeApi.Indicators;

namespace WaveTrend_Oscillator
{
    public class WaveTrend_Oscillator : IndicatorBuilder
    {
        public WaveTrend_Oscillator()
        : base()
        {
            #region Initialization
            Credentials.ProjectName = "WaveTrend Oscillator";
            #endregion

            Lines.Set("Channel Length");
            Lines["Channel Length"].Color = Color.Red;
            Lines["Channel Length"].Style = LineStyle.Dot;

            Lines.Set("Average Length");
            Lines["Average Length"].Color = Color.LimeGreen;

            Thresholds.Set("Over Bought Level 1");
            Thresholds["Over Bought Level 1"].Level = 60;
            Thresholds["Over Bought Level 1"].Color = Color.Gray;

            Thresholds.Set("Over Bought Level 2");
            Thresholds["Over Bought Level 2"].Level = 53;
            Thresholds["Over Bought Level 2"].Color = Color.Gray;

            Thresholds.Set("Over Sold Level 1");
            Thresholds["Over Sold Level 1"].Level = -60;
            Thresholds["Over Sold Level 1"].Color = Color.Gray;

            Thresholds.Set("Over Sold Level 2");
            Thresholds["Over Sold Level 2"].Level = -53;
            Thresholds["Over Sold Level 2"].Color = Color.Gray;

            Clouds.Set("Cloud", GradientMode.Simple, Color.CadetBlue, Color.CadetBlue);


            SeparateWindow = true;
        }


        [InputParameter(InputType.Numeric, "Channel Length", 0)]
        [SimpleNumeric(1D, 9999)]
        public int ChannelPeriod = 10;

        [InputParameter(InputType.Numeric, "Average Length", 1)]
        [SimpleNumeric(1D, 9999)]
        public int AveragePeriod = 21;

        BuiltInIndicator esa, d, wt2;

        List<double> ci = new List<double>();
        List<double> hlc3 = new List<double>();
        List<double> absD = new List<double>();

        public override void Init()
        {
            esa = IndicatorsManager.BuildIn.MA((int x )=> { 
                return HLC3(x);
            }, ChannelPeriod, MAMode.EMA);

            d = IndicatorsManager.BuildIn.MA((int x) => {
                return absD[x];
            }, ChannelPeriod, MAMode.EMA);

            wt2 = IndicatorsManager.BuildIn.MA((int x) => {
                return Lines[0].GetValue(x);
            }, 4, MAMode.SMA);
        }

        private double HLC3(int x) {
            return (HistoryDataSeries.GetValue(PriceType.Close, x) + HistoryDataSeries.GetValue(PriceType.Low, x) + HistoryDataSeries.GetValue(PriceType.High, x)) / 3;
        }

        private void EMA(int x, List<double> collection, int period) {
            if (collection.Count < period) {
                collection[x] = 0;
                return;
            }
            
            if (collection.Count == period)
            {
                double sumPrice = 0d;
                for (int i = 0; i < period; i++)
                    sumPrice += collection[i];
                collection[x] = (sumPrice / period);
            }
            else
            {
                double k = 2.0 / (period + 1);
                collection[x] = k * collection[x] + (1 - k) * collection[x+1];
            }
        }

        private double CI(int x)
        {
            return (HLC3(x) - esa.GetValue(x)) / (0.015 * d.GetValue(x));
        }
        public override void Update(TickStatus args)
        {
            if (args != TickStatus.IsQuote)
            {
                ci.Insert(0, 0);
                hlc3.Insert(0, 0);
                absD.Insert(0, 0);

                hlc3[0] = HLC3(0);

                absD[0] = Math.Abs(HLC3(0) - esa.GetValue(0));
                ci[0] = CI(0);
                EMA(0, ci, AveragePeriod);
                Lines[0].SetValue(ci[0]);
                Lines[1].SetValue(wt2.GetValue());
                Clouds[0].Set(ci[0] - wt2.GetValue(), 0.0001);

                if (Lines[0].GetValue() < Lines[1].GetValue() && Lines[0].GetValue(1) >= Lines[1].GetValue(1) && Lines[0].GetValue() > Thresholds[0].Level) {
                    BarStamps.Set(new BarStampPrimitiveFigure(Color.Red, 0, Lines[0].GetValue(), PrimitiveFigure.TriangleDown));
                }                   

                if (Lines[0].GetValue() > Lines[1].GetValue() && Lines[0].GetValue(1) <= Lines[1].GetValue(1) && Lines[0].GetValue() < Thresholds[2].Level) {
                    BarStamps.Set(new BarStampPrimitiveFigure(Color.LimeGreen, 0, Lines[0].GetValue(), PrimitiveFigure.TriangleUp));
                }                    
            }
        }

        public override void Complete()
        {
            esa = null;
            d = null;
            wt2 = null;
        }
    }
}
