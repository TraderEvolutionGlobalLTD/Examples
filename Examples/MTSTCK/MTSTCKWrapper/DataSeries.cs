using System;
using System.Collections.Generic;
using TradeApi.History;

namespace MTSTCKWrapper
{
    public abstract class DataSeries<T1, T2> : ICustomData<T1, T2>
    {
        public bool IsNewBar
        {
            get;
            private set;
        }
        public DataSeries(string name)
        {
            this.Name = name;
        }
        public string Name { get; }
        public int Count { get { return _dataList.Count; } }
        public void GetHistoricalData(HistoricalData hd)
        {
            _currentData = hd;
        }
        public T2 GetValue(T1 value)
        {
            ChangeIsNewBarFlag();
            T2 dataValue = GetCustomValue(value, 0);
            SetValueToList(dataValue);
            return dataValue;
        }
        public void GetHistoryValue(T1 value, int index)
        {
            T2 dataValue = GetCustomValue(value, index);
            _dataList.Insert(index, dataValue);
        }
        public T2 GetValueByIndex(int index)
        {
            if (index < 0 || _dataList.Count <= index)
            {
                throw new ArgumentOutOfRangeException();
            }
            return _dataList[index];
        }
        internal HistoricalData CurrentData { get { return _currentData; } }
        internal abstract T2 GetCustomValue(T1 value, int index);

        protected List<T2> _dataList = new List<T2>();

        private HistoricalData _currentData;

        private int _preCalculateBarCount;
        private void SetValueToList(T2 value)
        {
            if (IsNewBar)
            {
                _dataList.Insert(0, value);
            }
            else
            {
                _dataList[0] = value;
            }
        }
        private void ChangeIsNewBarFlag()
        {
            IsNewBar = (_preCalculateBarCount < _currentData.Count ? true : false);
            _preCalculateBarCount = _currentData.Count;
        }
    }
}
