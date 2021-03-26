
namespace MTSTCKWrapper.Containers
{
    internal class EMA : DataSeries<double, double>
    {
        public EMA(int _emaPeriod, string name) : base(name)
        {
            this.emaPeriod = _emaPeriod;
            alpha = 2.0 / ((double)this.emaPeriod + 1.0);
            isFirstStart = true;
        }

        internal override double GetCustomValue(double calcPrice, int index = 0)
        {
            if (isFirstStart)
            {
                ema = calcPrice;
                isFirstStart = false;
            }
            if (base.IsNewBar)
            {
                oldEma = ema;
            }
            ema = oldEma + alpha * (calcPrice - oldEma);
            return ema;
        }

        private int emaPeriod;

        private double alpha;

        private double ema;

        private double oldEma;

        private bool isFirstStart;
    }
}
