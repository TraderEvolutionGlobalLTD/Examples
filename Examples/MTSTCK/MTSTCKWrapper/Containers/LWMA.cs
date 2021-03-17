using System.Collections.Generic;

namespace MTSTCKWrapper.Containers
{
    internal class LWMA : DataSeries<double, double>
    {
        public LWMA(int _period) : base("LWMA")
        {
            this.period = _period;
        }

        internal override double GetCustomValue(double price, int index = 0)
        {
            if (LWMAList.Count <= period)
            {
                LWMAList.Add(price);
                oldPrice = price;
                return oldPrice;
            }
            if (base.IsNewBar)
            {
                LWMAList.Add(oldPrice);
            }
            double num = (double)period * price;
            double num2 = period;
            for (int i = 1; i < period; i++)
            {
                int num3 = period - i;
                num += (double)num3 * LWMAList[LWMAList.Count - i];
                num2 += (double)num3;
            }
            lwma = num / num2;
            oldPrice = price;
            return lwma;
        }

        private List<double> LWMAList = new List<double>();

        private double oldPrice;

        private double lwma;

        private int period;
    }
}
