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
            MAMode modeResult;
            ICustomData<double, double> result;
            int findResult = MaDataSeries.FindIndex(x => Enum.TryParse(x.Name, out modeResult));

            if (findResult != -1)
            {
                MaDataSeries[findResult].GetValue(historicalData.GetValue(type, 0));
                result = MaDataSeries[findResult];
            }
            else
            {
                result = CreateAndFillMA(historicalData, mode, period, type);
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
            var refIndex = CustomDataSeries.FindIndex(x => x.Name == newRefSeries.Name);

            if (refIndex != -1)
            {
                return CustomDataSeries[refIndex];
            }

            return null;
        }
        internal void AutoUpdate()
        {
            this.CrossDataSeries.ForEach(x => x.GetValue(Double.NaN));
            this.MaDataSeries.ForEach(x => x.GetValue(Double.NaN));
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
            return MaDataSeries.Concat(CustomDataSeries).ToList();
        }

        private readonly List<ICustomData<double, double>> MaDataSeries = new List<ICustomData<double, double>>();

        private readonly List<ICustomData<double, bool>> CrossDataSeries = new List<ICustomData<double, bool>>();

        private readonly List<ICustomData<double, double>> CustomDataSeries = new List<ICustomData<double, double>>();
        private ICustomData<double, double> CreateAndFillMA(HistoricalData currentData, MAMode MAMode, int period, PriceType type)
        {
            switch (MAMode)
            {
                case MAMode.SMA:
                    MaDataSeries.Insert(0, new SMA(period));
                    break;
                case MAMode.EMA:
                    MaDataSeries.Insert(0, new EMA(period));
                    break;
                case MAMode.SMMA:
                    MaDataSeries.Insert(0, new SMMA(period));
                    break;
                case MAMode.LWMA:
                    MaDataSeries.Insert(0, new LWMA(period));
                    break;
            }
            (MaDataSeries[0] as DataSeries<double, double>).GetHistoricalData(currentData);

            FillMA(currentData, type);

            MaDataSeries[0].GetValue(currentData.GetValue(type, 0));

            return MaDataSeries[0];
        }
        private void FillMA(HistoricalData historicalData, PriceType type)
        {
            for (int i = 0; i < historicalData.Count; i++)
            {
                (MaDataSeries[0] as DataSeries<double, double>).GetHistoryValue(historicalData.GetValue(type, i), i);
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
