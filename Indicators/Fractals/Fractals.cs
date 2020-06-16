using Runtime.Script;
using System;
using System.Drawing;
using TradeApi.History;
using TradeApi.Indicators;

namespace Fractals
{
    public class Fractals : IndicatorBuilder
    {
        public Fractals()
        : base()
        {
            Credentials.ProjectName = "Fractals";

            Lines.Set("Fractal up");
            Lines["Fractal up"].Color = Color.Red;
            Lines["Fractal up"].Style = LineStyle.Symbol;
            Lines["Fractal up"].ArrowCode = 234;

            Lines.Set("Fractal down");
            Lines["Fractal down"].Color = Color.Red;
            Lines["Fractal down"].Style = LineStyle.Symbol;
            Lines["Fractal down"].ArrowCode = 233;

            SeparateWindow = false;
        }

        [InputParameter(InputType.Numeric, "Fractal Dimension", 0)]
        [SimpleNumeric(1D, 100D)]
        public int fractalDimension = 3;

        [InputParameter(InputType.Combobox, "Aggregation", 1)]
        [ComboboxItem("Current", AggregationType.Current)]
        [ComboboxItem("Custom", AggregationType.Custom)]
        public AggregationType agrType = AggregationType.Current;

        [InputParameter(InputType.Combobox, "Period Type", 2)]
        [ComboboxItem("Second", Period.Second)]
        [ComboboxItem("Minute", Period.Minute)]
        [ComboboxItem("Hour", Period.Hour)]
        [ComboboxItem("Day", Period.Day)]
        [ComboboxItem("Week", Period.Week)]
        [ComboboxItem("Month", Period.Month)]
        [ComboboxItem("Year", Period.Year)]
        public Period periodType = Period.Hour;

        [InputParameter(InputType.Numeric, "Period Value", 3)]
        [SimpleNumeric(1D, 100D)]
        public int periodValue = 1;

        private bool oneAlert;

        public override void Init()
        {
            this.ScriptShortName = string.Format("Fractals({0})", fractalDimension);
            oneAlert = false;
        }

        public override void Update(TickStatus args)
        {
            if (args != TickStatus.IsQuote)
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
                    Lines[0].SetValue( High(position + 1) + 5 * InstrumentsManager.Current.MinimalTickSize, position + 1);
                }

                if (isLowest)
                {
                    Lines[1].SetValue(Low(position + 1) - 5 * InstrumentsManager.Current.MinimalTickSize, position + 1);
                }
            }
        }

        private double High(int i)
        {
            return HistoryDataSeries.GetValue(PriceType.High, i);
        }

        private double Low(int i)
        {
            return HistoryDataSeries.GetValue(PriceType.Low, i);
        }
    }

    public enum AggregationType
    {
        Current, Custom
    };
}
