using System;
using System.Collections.Generic;
using System.Drawing;

namespace TradingSessions
{
    class Settings
    {
        #region Properties
        /// <summary>
        /// List to keep states of selected Exchanges
        /// </summary>
        public List<Exchange> defaultExchanges = new List<Exchange>();

        //text
        public static StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap };
        public static StringFormat leftSideFormat = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near, FormatFlags = StringFormatFlags.NoWrap };
        public static StringFormat rightSideFormat = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far, FormatFlags = StringFormatFlags.NoWrap };

        #endregion

        public Settings()
        {
            #region  Default Exchanges List
            defaultExchanges.Add(new Exchange("Europe", "Frankfurt (FSX)", "09:00:00", "17:30:00", "W. Europe Standard Time"));
            defaultExchanges.Add(new Exchange("Europe", "London (LSE)", "8:00:00", "16:30:00", "GMT Standard Time"));
            defaultExchanges.Add(new Exchange("America", "New York (NYSE)", "9:30:00", "16:00:00", "Eastern Standard Time"));
            defaultExchanges.Add(new Exchange("Oceania", "Sydney (ASX)", "10:00:00", "16:00:00", "AUS Eastern Standard Time"));
            defaultExchanges.Add(new Exchange("Asia", "Tokyo (JPX)", "09:00:00", "15:00:00", "Tokyo Standard Time"));
            defaultExchanges.Add(new Exchange("Asia", "Hong Kong (HKEX)", "09:30:00", "16:00:00", "China Standard Time"));
            #endregion
        }
    }

}
