using MTSTCKWrapper;
using MTSTCKWrapper.Core;
using Runtime.Script;
using System.Drawing;
using TradeApi.Indicators;

namespace MTSTKWrapperLibExampleV2
{
    public class MTSTKWrapperLibExampleV2 : MTSTCKWrapperBuilder
    {
        public MTSTKWrapperLibExampleV2()
        : base()
        {
            #region Initialization
            Credentials.ProjectName = "MTSTKWrapperLibExample V2";
            #endregion

            Lines.Set("momentum1");
            Lines["momentum1"].Color = Color.CornflowerBlue;
            Lines.Set("momentum2");
            Lines["momentum2"].Color = Color.Red;
            SeparateWindow = true;
        }

        [InputParameter(InputType.Numeric, "Moment period")]
        [SimpleNumeric(1D, 10D)]
        public int momPeriod = 3;

        public BuiltInIndicator momentum, ema1, ema2;

        public CUSTOM MTST_momentum;
        public override void Init()
        {
            // native TE build-in indicator Momentum;
            momentum = IndicatorsManager.BuildIn.Momentum(HistoryDataSeries, momPeriod);

            // custom wrapper for build-in indicator Momentum;
            MTST_momentum = AddCustom("MTST_momentum", (value) => {
                return value;
            }) as CUSTOM;
        }


        public override void OnUpdate(TickStatus args)
        {
            //using generic custom entity, which could be used for any build-in TE indicator with proper dataseries synchronization
            MTST_momentum.GetValue(momentum.GetValue());            

            var mov = Mov(MTST_momentum.GetValueByIndex(0) - Ref(MTST_momentum, 1).Value, 66, MAMode.EMA);

            var mov1 = Mov(mov.GetValueByIndex(0), 3, MAMode.SMA);

            Lines["momentum1"].SetValue(mov1.GetValueByIndex(0));

            //using Mom class, only for momentum calculation
            var mom = Mom(momPeriod);

            var movTest = Mov(mom.GetValueByIndex(0) - Ref(mom, 1).Value, 66, MAMode.EMA);

            var movTest1 = Mov(movTest.GetValueByIndex(0), 3, MAMode.SMA);

            Lines["momentum2"].SetValue(movTest1.GetValueByIndex(0));

        }
    }
}
