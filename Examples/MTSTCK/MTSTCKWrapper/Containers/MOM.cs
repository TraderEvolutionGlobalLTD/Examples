using System;
using System.Collections.Generic;


namespace MTSTCKWrapper.Containers
{
    class MOM: DataSeries<double, double>
    {
        public MOM(int _momPeriod, string name) : base(name)
    {
        this.momPeriod = _momPeriod;
    }

    internal override double GetCustomValue(double price = 0, int index = 0)
    {
        if (base.IsNewBar)
        {
            momList.Add(oldValue);
        }
        double result = 0;
        if (momPeriod < momList.Count) {
            result = CurrentData.GetValue(TradeApi.History.PriceType.Close) * 100D / CurrentData.GetValue(TradeApi.History.PriceType.Close, momPeriod);
        }

        oldValue = result;
        return result;
    }

    private List<double> momList = new List<double>();

    private int momPeriod;

    private double oldValue;

    }
}
