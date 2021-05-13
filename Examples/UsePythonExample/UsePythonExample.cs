using Runtime.Script;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TradeApi.History;
using TradeApi.Indicators;

namespace UsePythonExample
{
    public class UsePythonExample : IndicatorBuilder
    {
        public UsePythonExample()
            : base()
        {
            #region Initialization
            Credentials.ProjectName = "Python TE";
            #endregion

            Lines.Set("ma");
            Lines["ma"].Color = Color.CornflowerBlue;
            Lines["ma"].Width = 3;
        }

        [InputParameter(InputType.Numeric, "Period", 0)]
        [SimpleNumeric(1D, 99999D)]
        public int period = 14;

        BarData barData;

        private List<OHLCV> ohlc = new List<OHLCV>();

        private string module_name = "fintaMA";

        private string class_name = "MovingAvarageModel";

        private string def_name = "getMA";

        private bool loadData = false;
        public override void Init()
        {
            barData = this.HistoryDataSeries as BarData;

            HistoricalDataManager.OnLoaded = (data) =>
            {
                if (data.Count > period * 2) {

                    for (int i = period * 6; i >= 0; i--)
                    {
                        ohlc.Insert(0, new OHLCV(data as BarData, i));
                    }                   

                    var methodArguments = new PythonCallerArgs();

                    methodArguments.SetPeriodArg(period);

                    methodArguments.SetArg(ohlc);

                    var pyCaller = new PythonCaller(module_name);

                    Dictionary<string, List<double>> resultJson = pyCaller.CallClassMethod(class_name, def_name, methodArguments);

                    var result = resultJson["result"];

                    result = result.Where(x => !Double.IsNaN(x)).ToList();

                    for (int i = 0; i < result.Count; i++)
                    {
                        Lines[0].SetValue(result[i], i);
                    }
                }

            };
        }

        public override void Update(TickStatus args)
        {
            if (!loadData)
            {
                HistoricalDataManager.Get(HistoryDataSeries.HistoricalRequest, new Interval(barData.GetTimeUtc(), DateTime.UtcNow));
                loadData = true;
            }
        }
    }

    class OHLCV
    {
        public OHLCV(BarData barData, int i)
        {
            open = barData.GetOpen(i);
            close = barData.GetClose(i);
            low = barData.GetLow(i);
            high = barData.GetHigh(i);
            volume = double.IsNaN(barData.GetVolume(i))? 0: barData.GetVolume(i);
            time = barData.GetTimeUtc(i);
        }

        public double open { get; }
        public double close { get; }
        public double high { get; }
        public double low { get; }
        public double volume { get; }
        public DateTime time { get; }
    }
}
