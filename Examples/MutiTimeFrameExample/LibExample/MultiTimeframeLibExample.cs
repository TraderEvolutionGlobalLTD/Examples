
using IndicatorBuilderMTF;
using Runtime.Script;
using TradeApi.History;

namespace LibExample
{
    public class MultiTimeframeLibExample : IndicatorBuilder_MTF
    {
        public MultiTimeframeLibExample()
        : base()
        {
            #region Initialization
            Credentials.ProjectName = "Multi timeframe lib example";
            #endregion
            SeparateWindow = true;
        }
        [InputParameter(InputType.Numeric, "value", 0)]
        [SimpleNumeric(1D, 99999D)]
        public int value = 4;

        public bool DoOnce = false;

        BarData data;

        public override void Init()
        {

        }

        public override void Update(TickStatus args)
        {
            if (args != TickStatus.IsQuote)
            {
                if (!DoOnce)
                {
                    data = TimeFrameRequest<BarData>(new IntervalPeriod(Period.Hour, value));

                    var price = TimeFrameGetPrice(new IntervalPeriod(Period.Day, 1), PriceType.High, 0);

                    Notification.Comment($"New time frame bar count: {data.Count}" + $"Day's high {price}");

                    DoOnce = true;

                }
            }

        }
    }
}
