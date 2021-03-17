using System;
using System.Collections.Generic;

namespace MTSTCKWrapper.Containers
{
    internal class SMA : DataSeries<double, double>
    {
        public SMA(int _smaPeriod) : base("SMA")
        {
            this.smaPeriod = _smaPeriod;
        }

        internal override double GetCustomValue(double price, int index = 0)
        {
            int num = Math.Min(smaList.Count, smaPeriod);
            double num2 = price;
            if (base.IsNewBar)
            {
                smaList.Add(oldPrice);
            }
            for (int num3 = smaList.Count - 1; num3 > smaList.Count - num; num3--)
            {
                num2 += smaList[num3];
            }
            double result = num2 / (double)smaPeriod;
            oldPrice = price;
            return result;
        }

        private List<double> smaList = new List<double>();

        private int smaPeriod;

        private double oldPrice;

    }
}
