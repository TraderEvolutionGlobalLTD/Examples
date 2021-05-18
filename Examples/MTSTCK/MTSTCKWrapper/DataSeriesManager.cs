using MTSTCKWrapper.Containers;
using MTSTCKWrapper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeApi.History;
using TradeApi.Indicators;

namespace MTSTCKWrapper
{
    public class DataSeriesManager
    {
        public readonly string CrossPartialName = "CROSS";
        public ICustomData<double, double> CreateOrUpdateMA(PriceType type, int period, MAMode mode, HistoricalData historicalData)
        {
            ICustomData<double, double> result;

            string name = mode.ToString() + period.ToString();

            int findResult = BuiltInIndicatorDataSeries.FindIndex(x => x.Name == name);

            if (findResult != -1)
            {
                BuiltInIndicatorDataSeries[findResult].GetValue(historicalData.GetValue(type, 0));
                result = BuiltInIndicatorDataSeries[findResult];
            }
            else
            {
                result = CreateAndFillMA(historicalData, mode, period, type, name);
            }

            return result;
        }

        public ICustomData<double, double> CreateOrUpdateMA(double value, int period, MAMode mode, HistoricalData historicalData)
        {
            ICustomData<double, double> result;

            string name = mode.ToString() + period.ToString();

            int findResult = BuiltInIndicatorDataSeries.FindIndex(x => x.Name == name);

            if (findResult != -1)
            {
                BuiltInIndicatorDataSeries[findResult].GetValue(value);
                result = BuiltInIndicatorDataSeries[findResult];
            }
            else
            {
                result = CreateMA(historicalData, mode, period, name, value);
            }

            return result;
        }


        public ICustomData<double, double> CreateOrUpdateMomentum(int period, HistoricalData historicalData)
        {
            ICustomData<double, double> result;

            string name = "Momentum" + period.ToString();

            int findResult = BuiltInIndicatorDataSeries.FindIndex(x => x.Name == name);

            if (findResult != -1)
            {
                BuiltInIndicatorDataSeries[findResult].GetValue(0);
                result = BuiltInIndicatorDataSeries[findResult];
            }
            else
            {
                result = CreateMomentum(historicalData, period, name, 0);
            }

            return result;
        }


        public ICustomData<double, bool> CreateOrUpdateCross(HistoricalData currentData, ICustomData<double, double> a, ReferenceSeries b, CrossDirection direction)
        {
            ICustomData<double, bool> result;
            string name = direction == CrossDirection.Down ? a.Name + CrossPartialName + b.Name : b.Name + CrossPartialName + a.Name;
            int findResult = CrossDataSeries.FindIndex(x => x.Name == name);

            if (findResult != -1)
            {
                CrossDataSeries[findResult].GetValue(b.Value);
                result = CrossDataSeries[findResult];
            }
            else
            {
                result = CreateAndFillCross(currentData, a, b, direction);
            }

            return result;
        }        
        public ICustomData<double, double> CreateCustom(HistoricalData currentData, ICustomData<double, double> newRefSeries)
        {
            CustomDataSeries.Insert(0, newRefSeries);
            (CustomDataSeries[0] as DataSeries<double, double>).GetHistoricalData(currentData);
            FillCustom(currentData);

            return CustomDataSeries[0];
        }
        private void FillCustom(HistoricalData historicalData)
        {
            for (int i = 0; i < historicalData.Count; i++)
            {
                (CustomDataSeries[0] as DataSeries<double, double>).GetHistoryValue(Double.NaN, i);
            }
        }
        public ICustomData<double, double> GetCustom(ICustomData<double, double> newRefSeries)
        {
            var refIndex = this.getAllNumericSerries().FindIndex(x => x.Name == newRefSeries.Name);

            if (refIndex != -1)
            {
                return getAllNumericSerries()[refIndex];
            }

            return null;
        }
        internal void AutoUpdate()
        {
            this.CrossDataSeries.ForEach(x => x.GetValue(Double.NaN));
            this.BuiltInIndicatorDataSeries.ForEach(x => x.GetValue(Double.NaN));
            this.CustomDataSeries.ForEach(x => x.GetValue(Double.NaN));
        }
        public int LastCross(string seriesName)
        {
            var crossIndex = CrossDataSeries.FindIndex(x => x.Name == seriesName);
            var result = -1;
            for (int i = 0; i < CrossDataSeries.Count - 1; i++)
            {
                if (CrossDataSeries[crossIndex].GetValueByIndex(i))
                {
                    result = i;
                    break;
                }
            }

            return result;
        }
        public List<ICustomData<double, double>> getAllNumericSerries()
        {
            return BuiltInIndicatorDataSeries.Concat(CustomDataSeries).ToList();
        }

        private readonly List<ICustomData<double, double>> BuiltInIndicatorDataSeries = new List<ICustomData<double, double>>();

        private readonly List<ICustomData<double, bool>> CrossDataSeries = new List<ICustomData<double, bool>>();

        private readonly List<ICustomData<double, double>> CustomDataSeries = new List<ICustomData<double, double>>();

        private ICustomData<double, double> CreateAndFillMA(HistoricalData currentData, MAMode MAMode, int period, PriceType type, string name)
        {
            this.AppendMa(currentData, MAMode, period, name);

            FillMA(currentData, type);

            BuiltInIndicatorDataSeries[0].GetValue(currentData.GetValue(type, 0));

            return BuiltInIndicatorDataSeries[0];
        }
        private void AppendMa(HistoricalData currentData, MAMode MAMode, int period, string name) 
        {
            switch (MAMode)
            {
                case MAMode.SMA:
                    BuiltInIndicatorDataSeries.Insert(0, new SMA(period, name));
                    break;
                case MAMode.EMA:
                    BuiltInIndicatorDataSeries.Insert(0, new EMA(period, name));
                    break;
                case MAMode.SMMA:
                    BuiltInIndicatorDataSeries.Insert(0, new SMMA(period, name));
                    break;
                case MAMode.LWMA:
                    BuiltInIndicatorDataSeries.Insert(0, new LWMA(period, name));
                    break;
            }
            (BuiltInIndicatorDataSeries[0] as DataSeries<double, double>).GetHistoricalData(currentData);
        }
        private ICustomData<double, double> CreateMA(HistoricalData currentData, MAMode MAMode, int period, string name, double value)
        {
            this.AppendMa(currentData, MAMode, period, name);

            BuiltInIndicatorDataSeries[0].GetValue(value);

            return BuiltInIndicatorDataSeries[0];
        }

        private ICustomData<double, double> CreateMomentum(HistoricalData currentData, int period, string name, double value)
        {
            BuiltInIndicatorDataSeries.Insert(0, new MOM(period, name));

            (BuiltInIndicatorDataSeries[0] as DataSeries<double, double>).GetHistoricalData(currentData);

            BuiltInIndicatorDataSeries[0].GetValue(value);

            return BuiltInIndicatorDataSeries[0];
        }
        private void FillMA(HistoricalData historicalData, PriceType type)
        {
            for (int i = 0; i < historicalData.Count; i++)
            {
                (BuiltInIndicatorDataSeries[0] as DataSeries<double, double>).GetHistoryValue(historicalData.GetValue(type, i), i);
            }
        }
        private ICustomData<double, bool> CreateAndFillCross(HistoricalData currentData, ICustomData<double, double> a, ReferenceSeries b, CrossDirection direction)
        {
            CrossDataSeries.Insert(0, new CROSS(a, b.Name, direction));
            (CrossDataSeries[0] as DataSeries<double, bool>).GetHistoricalData(currentData);
            FillCross(currentData, a);
            CrossDataSeries[0].GetValue(b.Value);

            return CrossDataSeries[0];
        }
        private void FillCross(HistoricalData historicalData, ICustomData<double, double> a)
        {
            for (int i = 0; i < historicalData.Count; i++)
            {
                (CrossDataSeries[0] as DataSeries<double, bool>).GetHistoryValue(a.GetValueByIndex(i), i);
            }
        }
    }

    public enum CrossDirection
    {
        Up,
        Down
    }
}
