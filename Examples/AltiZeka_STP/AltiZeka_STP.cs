using System.Collections.Generic;
using System.Drawing;
using Runtime.Script;
using TradeApi.History;
using TradeApi.Indicators;
using System.Linq;

namespace AltiZeka_STP
{
    /// <summary>
    /// AltiZeka_STP
    /// </summary>
    public class AltiZeka_STP : IndicatorBuilder
    {
        public AltiZeka_STP()
            : base()
        {
            #region Initialization
            Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
            Credentials.ProjectName = "AltiZeka_STP";
            #endregion

            Lines.Set("cdlKeyReversalDown");
            Lines["cdlKeyReversalDown"].Color = Color.Blue;
            //Lines["cdlKeyReversalDown"].Visible = false;

            Lines.Set("cdl3DayCompression");
            Lines["cdl3DayCompression"].Color = Color.Aqua;
            //Lines["cdl3DayCompression"].Visible = false;

            Lines.Set("cdlIslandReversal");
            Lines["cdlIslandReversal"].Color = Color.Red;
            //Lines["cdlIslandReversal"].Visible = false;

            Lines.Set("cdlOutsideDay");
            Lines["cdlOutsideDay"].Color = Color.Orange;
            //Lines["cdlOutsideDay"].Visible = false;

            Lines.Set("cdlWideRangeDays");
            Lines["cdlWideRangeDays"].Color = Color.Pink;
            //Lines["cdlWideRangeDays"].Visible = false;

            Lines.Set("cdlGapOpening");
            Lines["cdlGapOpening"].Color = Color.Purple;
            //Lines["cdlGapOpening"].Visible = false;

            SeparateWindow = false;
        }

        private int _atrPeriod = 20;

        private BuiltInIndicator _atr;
        private BuiltInIndicator _trend;

        public List<PatternFormation> patternFormation = new List<PatternFormation>();

        /// <summary>
        /// This function will be called after creating
        /// </summary>
        public override void Init()
        {
            _atr = IndicatorsManager.BuildIn.ATR(HistoryDataSeries, _atrPeriod);
            _trend = IndicatorsManager.BuildIn.MA(HistoryDataSeries, 80, MAMode.SMA);             
        }

        /// <summary>
        /// Entry point. This function is called when new quote comes or new bar created
        /// </summary>
        public override void Update(TickStatus args)
        {
            if (HistoryDataSeries.Count < 80)
                return;

            if (args != TickStatus.IsQuote) {

                patternFormation = new List<PatternFormation>();

                patternFormation.Add(new PatternFormation(cdlKeyReversal(), Lines["cdlKeyReversalDown"].Color));
                patternFormation.Add(new PatternFormation(cdl3DayCompression(), Lines["cdl3DayCompression"].Color));
                patternFormation.Add(new PatternFormation(cdlIslandReversal(), Lines["cdlIslandReversal"].Color));
                patternFormation.Add(new PatternFormation(cdlOutsideDay(), Lines["cdlOutsideDay"].Color));
                patternFormation.Add(new PatternFormation(cdlWideRangeDays(), Lines["cdlWideRangeDays"].Color));
                patternFormation.Add(new PatternFormation(cdlGapOpening(), Lines["cdlGapOpening"].Color));

                var lastFormation = patternFormation.Where(x => x.Direction != TradeDirection.None).LastOrDefault();

                var value = lastFormation.Direction == TradeDirection.Buy ? GetLow(1) - HistoryDataSeries.HistoricalRequest.Instrument.MinimalTickSize * 100D : GetHigh(1) + HistoryDataSeries.HistoricalRequest.Instrument.MinimalTickSize * 100D;

                var figure = lastFormation.Direction == TradeDirection.Buy ? PrimitiveFigure.TriangleUp : PrimitiveFigure.TriangleDown;

                BarStamps.Set(new BarStampPrimitiveFigure(lastFormation.Color, 1, value, figure));
            }
        }

        private double GetHigh(int index) {
            return HistoryDataSeries.GetValue(PriceType.High, index);
        }

        private double GetOpen(int index)
        {
            return HistoryDataSeries.GetValue(PriceType.Open, index);
        }

        private double GetClose(int index)
        {
            return HistoryDataSeries.GetValue(PriceType.Close, index);
        }

        private double GetLow(int index)
        {
            return HistoryDataSeries.GetValue(PriceType.Low, index);
        }

        private double GetATR(int index)
        {
            return _atr.GetValue(index);
        }

        private double GetTrend(int index)
        {
            return _trend.GetValue(index);
        }

        public TradeDirection cdlKeyReversal()
        {
            if (GetHigh(0) > GetHigh(1) && GetLow(0) < GetLow(1))
            {
                if (GetClose(0) < GetLow(1) && GetTrend(0) < GetTrend(1))
                    return TradeDirection.Sell;
                else if (GetClose(0) > GetHigh(1) && GetTrend(0) > GetTrend(1))
                    return TradeDirection.Buy;
            }
            return TradeDirection.None;
        }

        public TradeDirection cdl3DayCompression()
        {
            if (GetATR(0) < GetATR(3) && GetATR(1) < GetATR(3) && GetATR(2) < GetATR(3) && GetTrend(0) > GetTrend(1))
                return TradeDirection.Buy;
            return TradeDirection.None;
        }

        public TradeDirection cdlIslandReversal()
        {
            if (GetLow(0) > GetHigh(1) && GetClose(0) < GetOpen(0) && GetTrend(0) < GetTrend(1))
                return TradeDirection.Sell;
            if (GetLow(1) < GetHigh(0) && GetClose(0) > GetOpen(0) && GetTrend(0) > GetTrend(1))
                return TradeDirection.Buy;
            return TradeDirection.None;
        }

        public TradeDirection cdlOutsideDay()
        {
            if (GetHigh(0) > GetHigh(1) && GetLow(0) < GetLow(1))
            {
                if (GetClose(0) < 0.75 * GetLow(0) + 0.25 * GetHigh(0) && GetTrend(0) < GetTrend(1))
                    return TradeDirection.Sell;
                if (GetClose(0) > 0.25 * GetLow(0) + 0.75 * GetHigh(0) && GetTrend(0) > GetTrend(1))
                    return TradeDirection.Buy;
            }
            return TradeDirection.None;
        }

        public TradeDirection cdlWideRangeDays()
        {
            if (GetATR(0) > 1.5 * GetATR(_atrPeriod))
                return cdlOutsideDay();
            return TradeDirection.None;
        }

        public TradeDirection cdlGapOpening()
        {
            var ratio = (GetClose(0) - GetClose(1)) / GetATR(_atrPeriod);
            if (ratio >= 0.5 && GetTrend(0) < GetTrend(1))
                return TradeDirection.Sell;
            if (ratio <= -0.5 && GetTrend(0) > GetTrend(1))
                return TradeDirection.Buy;
            return TradeDirection.None;
        }
    }

    public enum TradeDirection
    {
        Buy,
        Sell,
        None
    }

    public class PatternFormation
    {
        public PatternFormation(TradeDirection direction, Color color) {
            Direction = direction;
            Color = color;
        }
        public TradeDirection Direction;
        public Color Color;
    }
}
