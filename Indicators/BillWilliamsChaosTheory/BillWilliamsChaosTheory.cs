using Runtime.Script;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TradeApi.History;
using TradeApi.Indicators;

namespace BillWilliamsChaosTheory
{
    public class BillWilliamsChaosTheory : IndicatorBuilder
    {
        public BillWilliamsChaosTheory()
        : base()
        {
            #region Initialization
            Credentials.ProjectName = "BillWilliamsChaosTheory";
            #endregion

            Lines.Set("JAW");
            Lines["JAW"].Color = Color.Blue;

            Lines.Set("TEETH");
            Lines["TEETH"].Color = Color.Red;

            Lines.Set("LIPS");
            Lines["LIPS"].Color = Color.Green;

            Lines.Set("Fractal up");
            Lines["Fractal up"].Color = Color.White;
            Lines["Fractal up"].Style = LineStyle.Symbol;
            Lines["Fractal up"].ArrowCode = 217;

            Lines.Set("Fractal down");
            Lines["Fractal down"].Color = Color.White;
            Lines["Fractal down"].Style = LineStyle.Symbol;
            Lines["Fractal down"].ArrowCode = 218;

            Lines.Set("Ideal Fractal up");
            Lines["Ideal Fractal up"].Color = Color.Gold;
            Lines["Ideal Fractal up"].Style = LineStyle.Symbol;
            Lines["Ideal Fractal up"].ArrowCode = 217;

            Lines.Set("Ideal Fractal down");
            Lines["Ideal Fractal down"].Color = Color.Gold;
            Lines["Ideal Fractal down"].Style = LineStyle.Symbol;
            Lines["Ideal Fractal down"].ArrowCode = 218;

            Lines.Set("Resistance");
            Lines["Resistance"].Color = Color.Purple;
            Lines["Resistance"].Style = LineStyle.Ladder;
            Lines["Resistance"].Width = 2;

            Lines.Set("Support");
            Lines["Support"].Color = Color.Orange;
            Lines["Support"].Style = LineStyle.Ladder;
            Lines["Support"].Width = 2;

            SeparateWindow = false;
        }

        // Alligator
        [InputParameter(InputType.Checkbox, "Show Alligator", 0)]
        public bool showAlligator = false;

        [InputParameter(InputType.Combobox, "Type of Jaw Moving Average", 1)]
        [ComboboxItem("Simple", MAMode.SMA)]
        [ComboboxItem("Exponential", MAMode.EMA)]
        [ComboboxItem("Modified", MAMode.SMMA)]
        [ComboboxItem("Linear Weighted", MAMode.LWMA)]
        public MAMode JawMAType = MAMode.SMMA;

        [InputParameter(InputType.Combobox, "Sources prices for Jaw MA", 2)]
        [ComboboxItem("Close", PriceType.Close)]
        [ComboboxItem("Open", PriceType.Open)]
        [ComboboxItem("High", PriceType.High)]
        [ComboboxItem("Low", PriceType.Low)]
        [ComboboxItem("Typical", PriceType.Typical)]
        [ComboboxItem("Medium", PriceType.Medium)]
        [ComboboxItem("Weighted", PriceType.Weighted)]
        public PriceType JawSourcePrice = PriceType.Medium;

        [InputParameter(InputType.Numeric, "Period of Jaw MA", 3)]
        [SimpleNumeric(1D, 99999D)]
        public int JawMAPeriod = 13;

        [InputParameter(InputType.Numeric, "Shift of Jaw MA", 4)]
        [SimpleNumeric(1D, 99999D)]
        public int JawMAShift = 8;

        [InputParameter(InputType.Combobox, "Type of Jaw MA", 5)]
        [ComboboxItem("Simple", MAMode.SMA)]
        [ComboboxItem("Exponential", MAMode.EMA)]
        [ComboboxItem("Modified", MAMode.SMMA)]
        [ComboboxItem("Linear Weighted", MAMode.LWMA)]
        public MAMode TeethMAType = MAMode.LWMA;

        [InputParameter(InputType.Combobox, "Sources prices for Teeth MA", 6)]
        [ComboboxItem("Close", PriceType.Close)]
        [ComboboxItem("Open", PriceType.Open)]
        [ComboboxItem("High", PriceType.High)]
        [ComboboxItem("Low", PriceType.Low)]
        [ComboboxItem("Typical", PriceType.Typical)]
        [ComboboxItem("Medium", PriceType.Medium)]
        [ComboboxItem("Weighted", PriceType.Weighted)]
        public PriceType TeethSourcePrice = PriceType.Close;

        [InputParameter(InputType.Numeric, "Period of Teeth MA", 7)]
        [SimpleNumeric(1D, 99999D)]
        public int TeethMAPeiod = 8;

        [InputParameter(InputType.Numeric, "Shift of Teeth MA", 8)]
        [SimpleNumeric(1D, 99999D)]
        public int TeethMAShift = 5;

        [InputParameter(InputType.Combobox, "Type of Lips MA", 9)]
        [ComboboxItem("Simple", MAMode.SMA)]
        [ComboboxItem("Exponential", MAMode.EMA)]
        [ComboboxItem("Modified", MAMode.SMMA)]
        [ComboboxItem("Linear Weighted", MAMode.LWMA)]
        public MAMode LipsMAType = MAMode.SMA;

        [InputParameter(InputType.Combobox, "Sources prices for Teeth MA", 10)]
        [ComboboxItem("Close", PriceType.Close)]
        [ComboboxItem("Open", PriceType.Open)]
        [ComboboxItem("High", PriceType.High)]
        [ComboboxItem("Low", PriceType.Low)]
        [ComboboxItem("Typical", PriceType.Typical)]
        [ComboboxItem("Medium", PriceType.Medium)]
        [ComboboxItem("Weighted", PriceType.Weighted)]
        public PriceType LipsSourcePrice = PriceType.Close;

        [InputParameter(InputType.Numeric, "Period of Lips Moving Average", 11)]
        [SimpleNumeric(1D, 99999D)]
        public int LipsMAPeiod = 5;

        [InputParameter(InputType.Numeric, "Shift of Lips Moving Average", 12)]
        [SimpleNumeric(1D, 99999D)]
        public int LipsMAShift = 3;

        // Fractals
        [InputParameter(InputType.Checkbox, "Show Fractal", 13)]
        public bool showFractal = false;

        [InputParameter(InputType.Numeric, "Fractal Dimension", 14)]
        [SimpleNumeric(1D, 100D)]
        public int fractalDimension = 3;

        [InputParameter(InputType.Combobox, "Aggregation", 15)]
        [ComboboxItem("Current", AggregationType.Current)]
        [ComboboxItem("Custom", AggregationType.Custom)]
        public AggregationType agrType = AggregationType.Current;

        [InputParameter(InputType.Combobox, "Period Type", 16)]
        [ComboboxItem("Second", Period.Second)]
        [ComboboxItem("Minute", Period.Minute)]
        [ComboboxItem("Hour", Period.Hour)]
        [ComboboxItem("Day", Period.Day)]
        [ComboboxItem("Week", Period.Week)]
        [ComboboxItem("Month", Period.Month)]
        [ComboboxItem("Year", Period.Year)]
        public Period periodType = Period.Hour;

        [InputParameter(InputType.Numeric, "Period Value", 17)]
        [SimpleNumeric(1D, 100D)]
        public int periodValue = 1;

        // Acceleration / Deceleration Oscillator (AC)

        [InputParameter(InputType.Numeric, "Fast Length", 18)]
        [SimpleNumeric(1D, 99999D)]
        public int fastLength = 5;

        [InputParameter(InputType.Numeric, "Slow Length", 19)]
        [SimpleNumeric(1D, 99999D)]
        public int slowLength = 34;

        [InputParameter(InputType.Numeric, "Smoothing Length", 20)]
        [SimpleNumeric(1D, 99999D)]
        public int smoothLength = 5;

        // Market Facilitation Index (MFI)

        [InputParameter(InputType.Checkbox, "Show MFI", 21)]
        public bool showMFI = false;

        // Resistance / Support
        [InputParameter(InputType.Checkbox, "Show Ideal Fractal", 22)]
        public bool showIdealFractal = false;

        // Valid Fractal
        [InputParameter(InputType.Checkbox, "Show Resistance/Support", 23)]
        public bool showResSup = true;

        // William %R
        [InputParameter(InputType.Numeric, "William %R Length", 24)]
        [SimpleNumeric(1D, 99999D)]
        public int length = 14;

        // Generate Signal
        [InputParameter(InputType.Checkbox, "Show Signal", 25)]
        public bool showSignal = true;

        private BuiltInIndicator jawMa;
        private BuiltInIndicator teethMa;
        private BuiltInIndicator lipsMa;

        private BuiltInIndicator ac;
        private BuiltInIndicator fast_ao;
        private BuiltInIndicator slow_ao;

        private bool oneAlert;

        private double mfiOffsetPrimitive = 0D;
        private double mfiOffsetText = 0D;

        private bool[] greenMFI;
        private bool[] fadeMFI;
        private bool[] fakeMFI;
        private bool[] squatMFI;

        private List<double> RPercentage = new List<double>();
        public override void Init()
        {
            if (showAlligator)
            {
                initAlligator();
            }
            fast_ao = IndicatorsManager.BuildIn.MA((int x) => { return (High(x) - Low(x)) / 2; }, fastLength, MAMode.SMA);
            slow_ao = IndicatorsManager.BuildIn.MA((int x) => { return (High(x) - Low(x)) / 2; }, slowLength, MAMode.SMA);
            ac = IndicatorsManager.BuildIn.MA((int x) => { return fast_ao.GetValue(x) - slow_ao.GetValue(x); }, smoothLength, MAMode.SMA);

            oneAlert = false;


            if (showMFI)
            {
                mfiOffsetPrimitive = 70 * HistoryDataSeries.HistoricalRequest.Instrument.MinimalTickSize;
                mfiOffsetText = mfiOffsetPrimitive + 140 * HistoryDataSeries.HistoricalRequest.Instrument.MinimalTickSize;

                fillMFI();
            }

            if (showIdealFractal || showResSup || showSignal)
            {
                initAlligator();
                fillMFI();
            }
        }

        private void fillMFI()
        {
            greenMFI = new bool[fractalDimension + 1].Select(i => false).ToArray();
            fadeMFI = new bool[fractalDimension + 1].Select(i => false).ToArray();
            fakeMFI = new bool[fractalDimension + 1].Select(i => false).ToArray();
            squatMFI = new bool?[fractalDimension + 1].Select(i => false).ToArray();
        }

        private void initAlligator()
        {
            Lines[0].TimeShift = JawMAShift;
            Lines[1].TimeShift = TeethMAShift;
            Lines[2].TimeShift = LipsMAShift;

            jawMa = IndicatorsManager.BuildIn.MA(HistoryDataSeries, JawMAPeriod, JawMAType, 0, JawSourcePrice);
            teethMa = IndicatorsManager.BuildIn.MA(HistoryDataSeries, TeethMAPeiod, TeethMAType, 0, TeethSourcePrice);
            lipsMa = IndicatorsManager.BuildIn.MA(HistoryDataSeries, LipsMAPeiod, LipsMAType, 0, LipsSourcePrice);
        }

        public override void Update(TickStatus args)
        {
            if (oneAlert == true)
                return;

            if (showAlligator)
            {
                Alligator();
            }

            if (args != TickStatus.IsQuote)
            {
                if (showFractal)
                {
                    Fractal();
                }

                if (showMFI)
                {
                    MFI();
                }

                if (showIdealFractal || showResSup)
                {

                    Alligator(draw: showAlligator);

                    Fractal(draw: showFractal);

                    MFI(draw: showMFI);

                    IdealFractal(draw: showIdealFractal);

                    SupporResistance(draw: showResSup);
                }

                if (showSignal)
                {

                    Alligator(draw: showAlligator);

                    RPercentage.Insert(0, 0);

                    if (HistoryDataSeries.Count - 1 <= length)
                        return;

                    WilliamR();

                    var alligatorUpTrend = (Lines["LIPS"].GetValue(1) > Lines["TEETH"].GetValue(1)) && (Lines["TEETH"].GetValue(1) > (Lines["JAW"].GetValue(1)));
                    var alligatorDownTrend = (Lines["JAW"].GetValue(1) > Lines["TEETH"].GetValue(1)) && (Lines["TEETH"].GetValue(1) > Lines["LIPS"].GetValue(1));
                    var alligatorAwake = (alligatorUpTrend || alligatorDownTrend);

                    // Long
                    var acAboveZeroLong = (ac.GetValue(2) > 0) && (ac.GetValue(1) > 0) && (ac.GetValue(2) < ac.GetValue(1));
                    var acBelowZeroLong = (ac.GetValue(3) < 0) && (ac.GetValue(2) < 0) && (ac.GetValue(1) < 0) && (ac.GetValue(3) < ac.GetValue(2)) && (ac.GetValue(2) < ac.GetValue(1));
                    var acBreakoutLong = (ac.GetValue(2) < 0) && (ac.GetValue(1) > 0) && (ac.GetValue(2) < ac.GetValue(1));
                    var longWilliamR = RPercentage[1] > -50;
                    var breakoutUpFractal = (Lines["Resistance"].GetValue() <= HLC3(1)) && (Lines["Resistance"].GetValue() >= Low(1)) && (Lines["Resistance"].GetValue() < Close(1)) && (Lines["Resistance"].GetValue() > Open(1)) && !fakeMFI[1];

                    var longCondition = (acAboveZeroLong || acBelowZeroLong || acBreakoutLong) && breakoutUpFractal && longWilliamR && alligatorAwake;

                    // Short
                    var acBelowZeroShort = (ac.GetValue(2) < 0) && (ac.GetValue(1) < 0) && (ac.GetValue(2) > ac.GetValue(1));
                    var acAboveZeroShort = (ac.GetValue(3) > 0) && (ac.GetValue(2) > 0) && (ac.GetValue(1) > 0) && (ac.GetValue(3) > ac.GetValue(2)) && (ac.GetValue(2) > ac.GetValue(1));
                    var acBreakoutShort = (ac.GetValue(2) > 0) && (ac.GetValue(1) < 0) && (ac.GetValue(2) > ac.GetValue(1));
                    var shortWilliamR = RPercentage[1] < -50;
                    var breakoutDownFractal = (Lines["Support"].GetValue() >= HLC3(1)) && (Lines["Support"].GetValue() <= High(1)) && (Lines["Support"].GetValue() > Close(1)) && (Lines["Support"].GetValue() < Open(1)) && !fakeMFI[1];

                    var shortCondition = (acBelowZeroShort || acAboveZeroShort || acBreakoutShort) && breakoutDownFractal && shortWilliamR && alligatorAwake;


                    if (longCondition)
                    {
                        drawBarStamp("SIGNAL LONG", Color.LimeGreen, PrimitiveFigure.TriangleUp, false, 20);
                    }

                    if (shortCondition)
                    {
                        drawBarStamp("SIGNAL SHORT", Color.DeepPink, PrimitiveFigure.TriangleDown, true, 20);
                    }
                }
            }
        }

        private double GetVolume(int x = 0)
        {
            var vol = (HistoryDataSeries as BarData).GetVolume(x);

            return !double.IsNaN(vol) ? vol : (HistoryDataSeries as BarData).GetTicks(x);
        }

        private void drawMFI(string text, Color color)
        {
            drawBarStamp(text, color, PrimitiveFigure.Circle);
        }


        private void drawBarStamp(string text, Color color, PrimitiveFigure primitiveFigure, bool up = false, double offset = 1)
        {
            var tickSize = HistoryDataSeries.HistoricalRequest.Instrument.MinimalTickSize;
            offset = offset * tickSize;
            BarStamps.Set(new BarStampPrimitiveFigure(color, 0, up? High() + mfiOffsetPrimitive + offset : Low() - mfiOffsetPrimitive - offset, primitiveFigure));
            BarStamps.Set(new BarStampText(text, new Font("Arial", 10), color, Color.Transparent, 0, up? High() + mfiOffsetText + offset + 3 * tickSize : Low() - mfiOffsetText - offset - 10 * tickSize));
        }

        public static bool[] ShiftLeft(bool[] array)
        {
            return array.Skip(array.Length - 1).Concat(array.Take(array.Length - 1)).ToArray();
        }
        private void MFI(bool draw = true)
        {
            if (HistoryDataSeries.Count - 1 <= 0)
                return;

            var MFI0 = High() - Low() / GetVolume();
            var MFI1 = High(1) - Low(1) / GetVolume(1);
            var MFIplus = MFI0 > MFI1;
            var MFIminus = MFI0 < MFI1;
            var volplus = GetVolume() > GetVolume(1);
            var volminus = GetVolume() < GetVolume(1);
            var _greenMFI = volplus && MFIplus;
            var _fadeMFI = volminus && MFIminus;
            var _fakeMFI = volminus && MFIplus;
            var _squatMFI = volplus && MFIminus;

            greenMFI = ShiftLeft(greenMFI);
            greenMFI[0] = _greenMFI;

            fadeMFI = ShiftLeft(fadeMFI);
            fadeMFI[0] = _fadeMFI;

            fakeMFI = ShiftLeft(fakeMFI);
            fakeMFI[0] = _fakeMFI;

            squatMFI = ShiftLeft(squatMFI);
            squatMFI[0] = _squatMFI;

            if (!draw)
            {
                return;
            }

            if (_greenMFI)
            {
                drawMFI("GR", Color.FromArgb(150, Color.Green));
            }

            if (_fadeMFI)
            {
                drawMFI("FD", Color.FromArgb(150, Color.Blue));
            }

            if (_fakeMFI)
            {
                drawMFI("FK", Color.FromArgb(150, Color.Gray));
            }

            if (_squatMFI)
            {
                drawMFI("SQ", Color.FromArgb(150, Color.Red));
            }
        }
        private void Alligator(bool draw = true)
        {
            Lines["JAW"].Visible = draw;
            Lines["JAW"].SetValue(jawMa.GetValue());

            Lines["TEETH"].Visible = draw;
            Lines["TEETH"].SetValue(teethMa.GetValue());

            Lines["LIPS"].Visible = draw;
            Lines["LIPS"].SetValue(lipsMa.GetValue());
        }

        private double OHLC4(int index)
        {
            return (High() + Low() + Close() + Open()) / 4;
        }

        private double HLC3(int index)
        {
            return (High() + Low() + Close()) / 3;
        }
        private void IdealFractal(bool draw = true)
        {
            var validUpFractal = !double.IsNaN(Lines["Fractal up"].GetValue()) && OHLC4(fractalDimension) > Lines["TEETH"].GetValue() && greenMFI[fractalDimension];
            var validDownFractal = !double.IsNaN(Lines["Fractal down"].GetValue()) && OHLC4(fractalDimension) < Lines["TEETH"].GetValue() && greenMFI[fractalDimension];

            Lines["Ideal Fractal up"].Visible = draw;
            Lines["Ideal Fractal down"].Visible = draw;

            if (validUpFractal)
                Lines["Ideal Fractal up"].SetValue(High(0) + 5 * InstrumentsManager.Current.MinimalTickSize);
            if (validDownFractal)
                Lines["Ideal Fractal down"].SetValue(Low(0) - 5 * InstrumentsManager.Current.MinimalTickSize);
        }


        // William %R
        private void WilliamR()
        {
            var upper = High(HistoryDataSeries.GetHighest(PriceType.High, new Interval(HistoryDataSeries.GetTimeUtc(length), DateTime.UtcNow)).FirstOrDefault());
            var lower = Low(HistoryDataSeries.GetLowest(PriceType.Low, new Interval(HistoryDataSeries.GetTimeUtc(length), DateTime.UtcNow)).FirstOrDefault());
            RPercentage[0] = 100 * (Close() - upper) / (upper - lower);
        }

        private void SupporResistance(bool draw = true)
        {

            if (showResSup && !double.IsNaN(Lines["Fractal up"].GetValue()))
            {
                var validUpFractal = !double.IsNaN(Lines["Fractal up"].GetValue()) && OHLC4(fractalDimension) > Lines["TEETH"].GetValue() && greenMFI[fractalDimension];

                Lines["Resistance"].Visible = draw;
                Lines["Resistance"].SetValue(validUpFractal ? High(fractalDimension) : HistoryDataSeries.Count - 1 > 0 ? Lines["Resistance"].GetValue(1) : double.NaN);

            }

            if (showResSup && !double.IsNaN(Lines["Fractal down"].GetValue()))
            {
                var validDownFractal = !double.IsNaN(Lines["Fractal down"].GetValue()) && OHLC4(fractalDimension) < Lines["TEETH"].GetValue() && greenMFI[fractalDimension];

                Lines["Support"].Visible = draw;
                Lines["Support"].SetValue(validDownFractal ? Low(fractalDimension) : HistoryDataSeries.Count - 1 > 0 ? Lines["Support"].GetValue(1) : double.NaN);
            }
        }

        private void Fractal(bool draw = true)
        {
            if (oneAlert == true)
                return;

            if (fractalDimension % 2 == 0 || fractalDimension < 3)
            {
                oneAlert = true;
                Notification.Alert("Fractal Dimension must be an odd number, greater than or equal to three");
            }

            if (!(HistoryDataSeries.HistoricalRequest is TimeHistoricalRequest) && !(HistoryDataSeries.HistoricalRequest is TickHistoricalRequest))
            {
                oneAlert = true;
                Notification.Alert("Fractals can be built only on the type of aggregation Tick and Time");
            }

            bool isHighest = true;
            bool isLowest = true;

            int minutesInCurrentPeriod = (HistoryDataSeries.HistoricalRequest as TimeHistoricalRequest).Value;

            int dim = fractalDimension;

            int minutesInPeriod = 0;

            if (agrType == AggregationType.Custom)
            {
                switch (periodType)
                {
                    case Period.Second:
                        minutesInPeriod = 1;
                        break;

                    case Period.Minute:
                        minutesInPeriod = (int)TimeSpan.FromMinutes(1).TotalMinutes;
                        break;

                    case Period.Hour:
                        minutesInPeriod = (int)TimeSpan.FromHours(1).TotalMinutes;
                        break;

                    case Period.Day:
                        minutesInPeriod = (int)TimeSpan.FromDays(1).TotalMinutes;
                        break;

                    case Period.Week:
                        minutesInPeriod = (int)TimeSpan.FromDays(7).TotalMinutes;
                        break;

                    case Period.Month:
                        minutesInPeriod = (int)TimeSpan.FromDays(30).TotalMinutes;
                        break;

                    case Period.Year:
                        minutesInPeriod = (int)TimeSpan.FromDays(365).TotalMinutes;
                        break;

                    default:
                        minutesInPeriod = (int)TimeSpan.FromHours(1).TotalMinutes;
                        break;
                }

                int minutesInMajorPeriod = minutesInPeriod * periodValue;

                if (!oneAlert && minutesInCurrentPeriod > minutesInMajorPeriod)
                {
                    oneAlert = true;

                    Notification.Alert("Please check custom period");
                }

                dim = fractalDimension * (minutesInMajorPeriod / minutesInCurrentPeriod);
            }

            if (HistoryDataSeries.HistoricalRequest is TickHistoricalRequest) // Tick chart
            {
                dim = periodValue * (-(HistoryDataSeries.HistoricalRequest as TimeHistoricalRequest).Value);
            }

            if (HistoryDataSeries.Count < dim + 2)
                return;

            int position = (dim - 1) / 2;

            for (int i = dim; i >= 0; i--)
            {
                if (position != i)
                {
                    if (High(position + 1) <= High(i + 1))
                    {
                        isHighest = false;
                    }

                    if (Low(position + 1) >= Low(i + 1))
                    {
                        isLowest = false;
                    }
                }
            }

            if (isHighest)
            {
                Lines["Fractal up"].Visible = draw;
                Lines["Fractal up"].SetValue(High(position + 1) + 5 * InstrumentsManager.Current.MinimalTickSize, position + 1);
            }

            if (isLowest)
            {
                Lines["Fractal down"].Visible = draw;
                Lines["Fractal down"].SetValue(Low(position + 1) - 5 * InstrumentsManager.Current.MinimalTickSize, position + 1);
            }
        }

        private double High(int i = 0)
        {
            return HistoryDataSeries.GetValue(PriceType.High, i);
        }

        private double Low(int i = 0)
        {
            return HistoryDataSeries.GetValue(PriceType.Low, i);
        }

        private double Close(int i = 0)
        {
            return HistoryDataSeries.GetValue(PriceType.Close, i);
        }

        private double Open(int i = 0)
        {
            return HistoryDataSeries.GetValue(PriceType.Open, i);
        }

        public enum AggregationType
        {
            Current, Custom
        };
    }
}
