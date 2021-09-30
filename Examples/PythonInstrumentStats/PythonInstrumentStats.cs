using Runtime.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeApi.History;
using TradeApi.Indicators;
using TradeApi.Instruments;

namespace PythonInstrumentStats
{
    public class PythonInstrumentStats : IndicatorBuilder
    {
        public PythonInstrumentStats()
            : base()
        {
            #region Initialization
            Credentials.ProjectName = "Instrument Stats";
            #endregion;
        }

        private InstrumentStats instrumentStat;

        private string module_name = "instrumentStats";

        private string class_name = "InstrumentStatsModel";

        private string def_name = "getStats";

        ColData resultJson;
        public override void Init()
        {
            var methodArguments = new PythonCallerArgs();

            instrumentStat = new InstrumentStats(InstrumentsManager.Current);

            methodArguments.SetArg(instrumentStat);

            methodArguments.SetArg(instrumentStat);

            var pyCaller = new PythonCaller(module_name);

            resultJson = pyCaller.CallClassMethod(class_name, def_name, methodArguments);
        }

        public override void Update(TickStatus args)
        {
            if (resultJson != null) {

                var str = "";
                
                for (int i = 0; i < resultJson.columns.Count; i++)
                {
                    string row = resultJson.columns[i].Trim();
                    string data = resultJson.data[0][i].Trim();

                    str += "\n" + row + ":  " + data;
                }
                Notification.Comment(str);
            }
            
        }
    }
    class InstrumentStats
    {
        public InstrumentStats(Instrument instrument)
        {
            symbol = instrument.Symbol;
        }
        public string symbol;

    }
}
