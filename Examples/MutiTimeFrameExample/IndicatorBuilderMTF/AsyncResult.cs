using TradeApi.History;

namespace IndicatorBuilderMTF
{
    public class AsyncResult
    {
        public AsyncResult(HistoricalData _data, bool loaded = false)
        {
            Data = _data;
            IsReady = (loaded || _data.Count > 0) ? true : false;
        }

        public HistoricalData Data
        {
            get;
        }

        public bool IsReady
        {
            get;
        }


        public string Status
        {
            get; set;
        }
    }
}
