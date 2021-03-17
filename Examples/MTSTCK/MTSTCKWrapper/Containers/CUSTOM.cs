using System;

namespace MTSTCKWrapper
{
    public class CUSTOM : DataSeries<double, double>
    {
        public CUSTOM(string name, Func<double, double> algo) : base(name)
        {
            Algo = algo;
        }

        public Func<double, double> Algo
        {
            get;
        }

        internal override double GetCustomValue(double value, int index = 0)
        {
            return Algo.Invoke(value);
        }
    }
}
