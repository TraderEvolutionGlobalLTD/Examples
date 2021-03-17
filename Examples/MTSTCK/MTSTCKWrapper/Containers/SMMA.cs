
namespace MTSTCKWrapper.Containers
{
    internal class SMMA : DataSeries<double, double>
    {
        public SMMA(int _smmaPeriod) : base("smma")
        {
            this.smmaPeriod = _smmaPeriod;
            isFirstStart = true;
        }

        internal override double GetCustomValue(double pfe, int index = 0)
        {
            if (isFirstStart)
            {
                oldSmma = pfe;
            }
            else if (base.IsNewBar)
            {
                oldSmma = smma;
            }
            smma = oldSmma + (pfe - oldSmma) / (double)smmaPeriod;
            isFirstStart = false;
            return smma;
        }

        private int smmaPeriod;

        private bool isFirstStart;

        private double smma;

        private double oldSmma;
    }
}
