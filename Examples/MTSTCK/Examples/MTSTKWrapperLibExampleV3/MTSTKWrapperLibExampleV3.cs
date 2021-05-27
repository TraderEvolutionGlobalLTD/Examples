using MTSTCKWrapper;
using MTSTCKWrapper.Core;
using Runtime.Script;
using System;
using System.Drawing;
using TradeApi.History;
using TradeApi.Indicators;

namespace MTSTKWrapperLibExampleV3
{
    public class MTSTKWrapperLibExampleV3 : MTSTCKWrapperBuilder
    {
        public MTSTKWrapperLibExampleV3()
        : base()
        {
            #region Initialization
            Credentials.ProjectName = "MTSTKWrapperLibExample V3";
            #endregion

            Lines.Set("RB2");
            Lines["RB2"].Color = Color.CornflowerBlue;

            Lines.Set("RB1");
            Lines["RB1"].Color = Color.Red;

            Lines.Set("D1");
            Lines["D1"].Color = Color.Orange;

            Lines.Set("D");
            Lines["D"].Color = Color.Green;

            Lines.Set("BP");
            Lines["BP"].Color = Color.Gray;

            Lines.Set("B");
            Lines["B"].Color = Color.Yellow;

            Lines.Set("B1");
            Lines["B1"].Color = Color.LightBlue;

            Lines.Set("SB1");
            Lines["SB1"].Color = Color.DimGray;

            Lines.Set("SB2");
            Lines["SB2"].Color = Color.White;

            SeparateWindow = false;

            NW = (double value, int index) =>
            {
                if (TF == Period.Minute)
                    return ROC(Minute(), index + 1, ROC_RESULT.Absolute) < 0D;
                else if (TF == Period.Hour)
                    return ROC(Hour(), index + 1, ROC_RESULT.Absolute) < 0D;
                else if (TF == Period.Day)
                    return ROC(DayOfWeek(), index + 1, ROC_RESULT.Absolute) < 0;
                else if (TF == Period.Week)
                    return ROC(DayOfMonth(), index + 1, ROC_RESULT.Absolute) < 0;
                else
                    return ROC(Month(), index + 1, ROC_RESULT.Absolute) < 0;
            };
        }

        [InputParameter(InputType.Combobox, "Period", 4)]
        [ComboboxItem("Minute", Period.Minute)]
        [ComboboxItem("Hour", Period.Hour)]
        [ComboboxItem("Day", Period.Day)]
        [ComboboxItem("Week", Period.Week)]
        [ComboboxItem("Month", Period.Month)]
        public Period TF = Period.Minute;

        public CUSTOM C, L, H;
        public override void Init()
        {
            C = CreateSimpleCustomSeries("C");

            L = CreateSimpleCustomSeries("L");

            H = CreateSimpleCustomSeries("H");
        }

        public Func<double, int, bool> NW;

        public override void OnUpdate(TickStatus args)
        {
            if (args != TickStatus.IsQuote)
            {
                C.GetValue(HistoryDataSeries.GetValue(PriceType.Close));
                H.GetValue(HistoryDataSeries.GetValue(PriceType.High));
                L.GetValue(HistoryDataSeries.GetValue(PriceType.Low)); ;

                var WH = ValueWhen(1, NW, Ref(HighestSince(1, NW, H), -1));

                var WL = ValueWhen(1, NW, Ref(LowestSince(1, NW, L), -1));
                var WCL = ValueWhen(1, NW, Ref(C, -1));
                var BP = (WH + WL + WCL) / 3;
                var D = ((WH - WL) / 2) + BP;
                var B = BP - ((WH - WL) / 2);
                var D1 = (WH - WL) + BP;
                var B1 = BP - (WH - WL);
                var SB1 = BP - ((WH - WL) * 1.0618);
                var SB2 = BP - ((WH - WL) * 0.98382);
                var RB1 = ((WH - WL) * 1.0618) + BP;
                var RB2 = ((WH - WL) * 0.98382) + BP;

                Lines["RB2"].SetValue(RB2);
                Lines["RB1"].SetValue(RB1);
                Lines["D1"].SetValue(D1);
                Lines["D"].SetValue(D);
                Lines["BP"].SetValue(BP);
                Lines["B"].SetValue(B);
                Lines["B1"].SetValue(B1);
                Lines["SB1"].SetValue(SB1);
                Lines["SB2"].SetValue(SB2);
            }
        }
    }
}
