
using System;
using TradeApi.History;
using TradeApi.Indicators;

namespace IndicatorBuilderMTF
{
    public class IndicatorBuilder_MTF : IndicatorBuilder
    {

        public IndicatorBuilder_MTF() : base()
        {

        }

        public T TimeFrameRequest<T>(IntervalPeriod periodInterval, DataType dataType = DataType.Bid) where T : HistoricalData
        {
            var request = new TimeHistoricalRequest(InstrumentsManager.Current, dataType, periodInterval.Period, periodInterval.Value);
            var data = HistoricalDataManager.Get(request, new Interval(GetFromPeriod(periodInterval), DateTime.UtcNow)) as T;


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


            data = result.Data as T;

            return data;
        }

        public double TimeFrameGetPrice(IntervalPeriod periodInterval, PriceType priceType, int index = 0)
        {
            var date = TimeFrameRequest<BarData>(periodInterval);
            var price = 0D;


            if (date != null && date.Count > 0)
            {
                price = date.GetValue(priceType, index);
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

        /*public void TimeFrameSet(int ticks)
        {
            HistoricalDataManager.ChangeInterval(HistoryDataSeries, new Interval(GetFromTick(ticks), DateTime.UtcNow));
        }

        public void TimeFrameSet(IntervalPeriod periodInterval)
        {
            if (DefaultFrom == default(DateTime))
            {
                DefaultFrom = HistoryDataSeries.GetTimeUtc(HistoryDataSeries.Count - 1);
            }

            HistoricalDataManager.ChangeInterval(HistoryDataSeries, new Interval(GetFromPeriod(periodInterval), DateTime.UtcNow));
        }

        public void TimeFrameRestore()
        {
            HistoricalDataManager.ChangeInterval(HistoryDataSeries, new Interval(DefaultFrom, DateTime.UtcNow));
        }*/

        /*private DateTime GetFromTick(int ticks)
        {
            var bboData = HistoryDataSeries as BBOData;
            return bboData.GetTimeUtc(ticks);
        }


        private DateTime DefaultFrom;*/

    }
}
