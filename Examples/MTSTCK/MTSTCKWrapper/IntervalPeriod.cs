using TradeApi.History;

namespace MTSTCKWrapper
{
    public class IntervalPeriod
    {
        public IntervalPeriod(Period _period, int _value)
        {
            Value = _value;
            Period = _period;
        }

        public int Value
        {
            get;
        }
        public Period Period
        {
            get;
        }
    }
}
