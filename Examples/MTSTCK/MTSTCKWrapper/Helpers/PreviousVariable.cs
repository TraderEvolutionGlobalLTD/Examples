using TradeApi.Indicators;
using TradeApi.History;
using System;

namespace MTSTCKWrapper.Helpers
{
    public class PreviousVariable
    {
        public Lines lines;
        public PreviousVariable(Lines _lines)
        {
            lines = _lines;
        }
        public double this[string variableName]
        { 
            get {  
                try { return lines[variableName].GetValue(1);} 
                catch { return double.NaN;}
              } 
        }
    }
}
