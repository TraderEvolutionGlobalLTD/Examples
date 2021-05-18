using MTSTCKWrapper;
using MTSTCKWrapper.Core;
using Runtime.Script;
using System.Drawing;
using TradeApi.History;
using TradeApi.Indicators;

namespace MTSTCKWrapperLibExample
{
    public class MTSTCKWrapperLibExample : MTSTCKWrapperBuilder
    {
        public MTSTCKWrapperLibExample() 
        : base()
        {
            #region Initialization
            Credentials.ProjectName = "MTSTCKWrapperLibExample";
            #endregion

            Lines.Set("a1");
            Lines["a1"].Color = Color.CornflowerBlue;

            Lines.Set("s2");
            Lines["s2"].Color = Color.OrangeRed;
            SeparateWindow = true;
        }

        [InputParameter(InputType.Numeric, "% stop")]
        [SimpleNumeric(1D, 10D)]
        public int percentage = 2;

        [InputParameter(InputType.Numeric, "period")]
        [SimpleNumeric(1D, 100D)]
        public int period = 3;

        public double a2, a3;

        public CUSTOM b1, b2;

        public ICustomData<double, double> a1;
        public override void Init()
        {
            b1 = AddCustom("b1", (value) => {
                return value < PREV["a1"] ? a2 : a2 > PREV["a1"] ? a2 : PREV["a1"];
            }) as CUSTOM;

            b2 = AddCustom("b2", (value) => {
                return value > PREV["a1"] ? a3 : a3 < PREV["a1"] ? a3 : PREV["a1"];
            }) as CUSTOM;
        }

        public override void OnUpdate(TickStatus args)
        {
            /*if (HistoryDataSeries.Count - 1 < period)
                return;*/

            a1 = Mov(PriceType.Close, period, MAMode.EMA);

            a2 = a1.GetValueByIndex(0) - (a1.GetValueByIndex(0) * percentage / 100);

            a3 = a1.GetValueByIndex(0) + (a1.GetValueByIndex(0) * percentage / 100);

            // Both custom series is required to be supplied by value for the algorithm
            b1.GetValue(a1.GetValueByIndex(0));

            b2.GetValue(a1.GetValueByIndex(0));

            var k1 = Cross(a1, Ref(b2, 1));

            var k2 = Cross(Ref(b1, 1), a1);

            //Search index example of a logical cross over condition based on operation input and variable relations

            //BarsSince(a1, InstrumentsManager.Current.DayInfo.PrevClose, Operation.Less<double>());

            var s1 = BarsSince(k1) < BarsSince(k2);
            var s2 = s1 ? b1 : b2;

            Lines["a1"].SetValue(a1.GetValueByIndex(0));

            Lines["s2"].SetValue(s2.GetValueByIndex(0));
        }
    }

}
