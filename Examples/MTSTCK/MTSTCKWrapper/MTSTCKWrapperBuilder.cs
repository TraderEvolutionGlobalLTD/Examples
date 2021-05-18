using Runtime.Script;
using System;
using TradeApi.History;
using TradeApi.Indicators;
using MTSTCKWrapper.Helpers;

namespace MTSTCKWrapper.Core
{
    public class MTSTCKWrapperBuilder : IndicatorBuilder
    {
        public MTSTCKWrapperBuilder() : base()
        {
            PREV = new PreviousVariable(Lines);
        }
        
        public PreviousVariable PREV;
        public ICustomData<double, bool> Cross(ICustomData<double, double> a, ReferenceSeries b) 
        {
            var result = DataSeriesManager.CreateOrUpdateCross(HistoryDataSeries, a, b, CrossDirection.Down);

            return result;
        }
        public ICustomData<double, bool> Cross(ReferenceSeries b, ICustomData<double, double> a)
        {
            var result = DataSeriesManager.CreateOrUpdateCross(HistoryDataSeries, a, b, CrossDirection.Up);

            return result;
        }
        public int BarsSince<T1, T2>(ICustomData<T1, T2> series)
        {
            var index = -1;
            if (series.Name.Contains(DataSeriesManager.CrossPartialName)) {
                index = DataSeriesManager.LastCross(series.Name);
            }            

            return index;
        }
        public int BarsSince(ICustomData<double, double> a, double b, Func<double, double, bool> callback)
        {
            var allNumericSeries = DataSeriesManager.getAllNumericSerries();

            var index = allNumericSeries.FindIndex(x => x.Name == a.Name);

            var result = -1;

            if (index != -1) {
                for (int i = 0; i < HistoryDataSeries.Count - 1; i++)
                {
                    if (callback(allNumericSeries[index].GetValueByIndex(i), b))
                    {
                        result = i;
                        break;
                    }
                }
            }

            return result;
        }
        public ReferenceSeries Ref(ICustomData<double, double> newRefSeries, int index)
        {
            var result = new ReferenceSeries(newRefSeries.Name);
            result.Value = DataSeriesManager.GetCustom(newRefSeries) != null && DataSeriesManager.GetCustom(newRefSeries).Count > Math.Abs(index) ? DataSeriesManager.GetCustom(newRefSeries).GetValueByIndex(Math.Abs(index)) : -1;

            return result;
        }
        public ICustomData<double, double> AddCustom(string name, Func<double, double> algo)        
        {    
            var newRefSeries = new CUSTOM(name, algo);
            var result = DataSeriesManager.CreateCustom(HistoryDataSeries, newRefSeries);

            return result;
        }
        public ICustomData<double, double> Mov(PriceType type, int period, MAMode mode) 
        {
            var result =  DataSeriesManager.CreateOrUpdateMA(type, period, mode, HistoryDataSeries);

            return result;
        }

        public ICustomData<double, double> Mov(double value, int period, MAMode mode)
        {
            var result = DataSeriesManager.CreateOrUpdateMA(value, period, mode, HistoryDataSeries);

            return result;
        }

        public ICustomData<double, double> Mom(int period)
        {
            var result = DataSeriesManager.CreateOrUpdateMomentum(period, HistoryDataSeries);

            return result;
        }
        public override void Update(TickStatus args)
        {
            if (args != TickStatus.IsQuote) {
                DataSeriesManager.AutoUpdate();
            }

            this.OnUpdate(args);
        }
        public virtual void OnUpdate(TickStatus args)
        {

        }

        private DataSeriesManager DataSeriesManager = new DataSeriesManager();
    }
}
