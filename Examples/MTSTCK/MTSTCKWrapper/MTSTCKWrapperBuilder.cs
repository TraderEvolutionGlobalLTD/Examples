using Runtime.Script;
using System;
using TradeApi.History;
using TradeApi.Indicators;
using MTSTCKWrapper.Helpers;
using System.Collections.Generic;

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
            if (series.Name.Contains(DataSeriesManager.CrossPartialName))
            {
                index = DataSeriesManager.LastCross(series.Name);
            }

            return index;
        }
        public int BarsSince(ICustomData<double, double> a, double b, Func<double, double, bool> callback)
        {
            var allNumericSeries = DataSeriesManager.getAllNumericSerries();

            var index = allNumericSeries.FindIndex(x => x.Name == a.Name);

            var result = -1;

            if (index != -1)
            {
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
        public double ValueWhen(int offset, Func<double, int, bool> callback, ReferenceSeries searchPattern)
        {
            var allNumericSeries = DataSeriesManager.getAllNumericSerries();

            var index = allNumericSeries.FindIndex(x => x.Name == searchPattern.Name);

            var result = -1D;
            var counter = 0;

            if (index != -1)
            {
                for (int i = 0; i < HistoryDataSeries.Count - 1; i++)
                {
                    if (Convert.ToBoolean(callback(allNumericSeries[index].GetValueByIndex(i), i)))
                    {
                        result = searchPattern.Value;
                        counter += 1;
                        if (counter == offset)
                            break;
                    }
                }
            }

            return result;
        }
        public CUSTOM HighestSince(int b, Func<double, int, bool> callback, ICustomData<double, double> a)
        {
            return ExtremumSince(b, callback, a, EXTREMUM.High) as CUSTOM;
        }
        public CUSTOM LowestSince(int b, Func<double, int, bool> callback, ICustomData<double, double> a)
        {
            return ExtremumSince(b, callback, a, EXTREMUM.Low) as CUSTOM;
        }
        public CUSTOM CreateSimpleCustomSeries(string name)
        {
            return AddCustom(name, (value) => {
                return value;
            }) as CUSTOM;
        }
        public enum ROC_RESULT
        {
            Absolute,
            Percent
        }
        public double ROC(double pastValue, int offset, ROC_RESULT type)
        {
            var result = Double.NaN;

            var lastClose = HistoryDataSeries.GetValue(PriceType.Close, offset);

            result = type == ROC_RESULT.Absolute ? lastClose - pastValue : (lastClose - pastValue) / pastValue * 100;

            return result;
        }

        public T TimeFrameRequest<T>(IntervalPeriod periodInterval, DataType dataType = DataType.Bid) where T : HistoricalData
        {
            if (bardata != null)
            {
                var searchName = bardata.HistoricalRequest.Instrument.Symbol + ((bardata.HistoricalRequest as TimeHistoricalRequest).Period).ToString() + (bardata.HistoricalRequest as TimeHistoricalRequest).Value.ToString();

                if (asyncResults.FindIndex(x => x.Name == searchName) == -1)
                {
                    return SendRequest<T>(periodInterval, dataType);
                }

                return bardata as T;
            }
            else
            {
                return SendRequest<T>(periodInterval, dataType);
            }
        }

        private T SendRequest<T>(IntervalPeriod periodInterval, DataType dataType = DataType.Bid) where T : HistoricalData
        {
            T data = null;

            var request = new TimeHistoricalRequest(InstrumentsManager.Current, dataType, periodInterval.Period, periodInterval.Value);
            data = HistoricalDataManager.Get(request, new Interval(GetFromPeriod(periodInterval), DateTime.UtcNow)) as T;

            var result = new AsyncResult(data);
            HistoricalDataManager.OnLoaded = (historianData) =>
            {
                result = new AsyncResult(historianData, true);
                result.Status = "Done";
            };

            do
            {
                result.Status = "Loading";

            } while (!result.IsReady);

            asyncResults.Add(result);

            data = result.Data as T;

            return data;
        }

        private BarData bardata;

        private List<AsyncResult> asyncResults = new List<AsyncResult>();

        public double TimeFrameGetPrice(IntervalPeriod periodInterval, PriceType priceType, int index = 0)
        {
            bardata = TimeFrameRequest<BarData>(periodInterval);

            var price = 0D;

            if (bardata != null && bardata.Count > 0)
            {
                price = bardata.GetValue(priceType, index);
            }

            return price;
        }

        private DateTime GetFromPeriod(IntervalPeriod periodInterval)
        {
            var currentFromInterval = HistoryDataSeries.GetTimeUtc(HistoryDataSeries.Count - 1);

            switch (periodInterval.Period)
            {
                case Period.Second: return currentFromInterval > DateTime.UtcNow.AddSeconds(-periodInterval.Value) ? DateTime.UtcNow.AddSeconds(-periodInterval.Value) : currentFromInterval;
                case Period.Minute: return currentFromInterval > DateTime.UtcNow.AddMinutes(-periodInterval.Value) ? DateTime.UtcNow.AddMinutes(-periodInterval.Value) : currentFromInterval;
                case Period.Hour: return currentFromInterval > DateTime.UtcNow.AddHours(-periodInterval.Value) ? DateTime.UtcNow.AddHours(-periodInterval.Value) : currentFromInterval;
                case Period.Day: return currentFromInterval > DateTime.UtcNow.AddDays(-periodInterval.Value) ? DateTime.UtcNow.AddDays(-periodInterval.Value) : currentFromInterval;
                case Period.Week: return currentFromInterval > DateTime.UtcNow.AddDays(-periodInterval.Value * 7) ? DateTime.UtcNow.AddDays(-periodInterval.Value * 7) : currentFromInterval;
                case Period.Month: return currentFromInterval > DateTime.UtcNow.AddMonths(-periodInterval.Value) ? DateTime.UtcNow.AddMonths(-periodInterval.Value) : currentFromInterval;
                case Period.Year: return currentFromInterval > DateTime.UtcNow.AddYears(-periodInterval.Value) ? DateTime.UtcNow.AddYears(-periodInterval.Value) : currentFromInterval;
                default: return currentFromInterval;
            }
        }
        public double Minute()
        {

            return TimeFrameGetPrice(new IntervalPeriod(Period.Minute, 1), PriceType.Close, 1);
        }
        public double Hour()
        {
            return TimeFrameGetPrice(new IntervalPeriod(Period.Hour, 1), PriceType.Close, 1);
        }
        public double DayOfWeek()
        {
            return TimeFrameGetPrice(new IntervalPeriod(Period.Day, 1), PriceType.Close, 1);
        }
        public double DayOfMonth()
        {
            return TimeFrameGetPrice(new IntervalPeriod(Period.Week, 1), PriceType.Close, 1);
        }
        public double Month()
        {
            return TimeFrameGetPrice(new IntervalPeriod(Period.Month, 1), PriceType.Close, 1);
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
            var result = DataSeriesManager.CreateOrUpdateMA(type, period, mode, HistoryDataSeries);

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
            if (args != TickStatus.IsQuote)
            {
                DataSeriesManager.AutoUpdate();
            }

            this.OnUpdate(args);
        }
        public virtual void OnUpdate(TickStatus args)
        {

        }
        private DataSeriesManager DataSeriesManager = new DataSeriesManager();
        private ICustomData<double, double> ExtremumSince(int offset, Func<double, int, bool> callback, ICustomData<double, double> searchData, EXTREMUM type)
        {

            var allNumericSeries = DataSeriesManager.getAllNumericSerries();

            var name = type == EXTREMUM.High ? $"highestSince{searchData.Name}{offset}" : $"lowestSince{searchData.Name}{offset}";

            var indexOfRequiredHighest = allNumericSeries.FindIndex(x => x.Name == name);

            if (indexOfRequiredHighest != -1)
            {
                var index = allNumericSeries.FindIndex(x => x.Name == searchData.Name);

                if (index != -1)
                {
                    for (int i = 0; i < HistoryDataSeries.Count - 1; i++)
                    {
                        if (Convert.ToBoolean(callback(allNumericSeries[index].GetValueByIndex(i), i)))
                        {
                            allNumericSeries[indexOfRequiredHighest].GetValue(i);
                            break;
                        }
                    }
                }
                return allNumericSeries[indexOfRequiredHighest] as CUSTOM;
            }
            else
            {
                return AddCustom(name, (value) =>
                {
                    var index = allNumericSeries.FindIndex(x => x.Name == searchData.Name);
                    if (index != -1)
                    {
                        var result = allNumericSeries[index].GetValueByIndex(0);
                        for (int j = 0; j < value; j++)
                        {
                            result = type == EXTREMUM.High ? allNumericSeries[index].GetValueByIndex(j) > result ? allNumericSeries[index].GetValueByIndex(j) : result
                            : allNumericSeries[index].GetValueByIndex(j) < result ? allNumericSeries[index].GetValueByIndex(j) : result;
                        }
                        return result;
                    }
                    return 0D;
                });
            }
        }
        private enum EXTREMUM
        {
            Low,
            High
        }
    }
}
