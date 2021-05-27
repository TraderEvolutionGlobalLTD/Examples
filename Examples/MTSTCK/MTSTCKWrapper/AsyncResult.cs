using TradeApi.History;

namespace MTSTCKWrapper
{
    public class AsyncResult
    {
        public AsyncResult(HistoricalData _data, bool loaded = false)
        {
            Data = _data;
            IsReady = (loaded || _data.Count > 0) ? true : false;
            Name = _data.HistoricalRequest.Instrument.Symbol + ((_data.HistoricalRequest as TimeHistoricalRequest).Period).ToString() + (_data.HistoricalRequest as TimeHistoricalRequest).Value.ToString();
        }

        public HistoricalData Data
        {
            get;
        }

        public bool IsReady
        {
            get;
        }

        public string Name
        {
            get;
        }


        public string Status
        {
            get; set;
        }
    }
}
