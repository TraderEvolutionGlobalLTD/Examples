using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Runtime.Script;
using TradeApi;
using TradeApi.History;
using TradeApi.Indicators;
using TradeApi.Instruments;
using TradeApi.ToolBelt;
using TradeApi.Trading;



namespace LoadCustomIndicatorStrategy
{
    /// <summary>
    /// LoadCustomIndicatorStrategy
    /// 
    /// </summary>
    public class LoadCustomIndicatorStrategy : StrategyBuilder
    {
        public LoadCustomIndicatorStrategy()
            : base()
        {
            #region Initialization
            Credentials.Author = "";
            Credentials.Company = "";
            Credentials.Copyrights = "";
            Credentials.DateOfCreation = new DateTime(2021, 5, 31);
            Credentials.ExpirationDate = DateTime.MinValue;
            Credentials.Version = "";
            Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
            Credentials.ProjectName = "LoadCustomIndicatorStrategy";
            #endregion 


        }

        [InputParameter(InputType.Color, "Color for minimum:", 0)]
        public Color minColor = Color.Green;

        [InputParameter(InputType.Color, "Color for maximum:", 1)]
        public Color maxColor = Color.Yellow;

        BuiltInIndicator customIndicator;

        /// <summary>
        /// This function will be called after creating
        /// </summary>
        public override void Init()
        {
            //minColor, maxColor is additional parameters for custom indicator
            customIndicator = IndicatorsManager.BuildIn.Custom("CustomIndicator", HistoryDataSeries, minColor.ToArgb(), maxColor.ToArgb());
        }

        /// <summary>
        /// Entry point. This function is called when new quote comes or new bar created
        /// </summary>
        public override void Update(TickStatus args)
        {
            var val = customIndicator.GetValue();

            if (val == -1)
                Notification.Print("New minimum reached: " + HistoryDataSeries.GetValue(PriceType.Low));

            else if (val == 1)
                Notification.Print("New maximum reached: " + +HistoryDataSeries.GetValue(PriceType.High));

        }

        /// <summary>
        /// This function will be called before removing
        /// </summary>
        public override void Complete()
        {

        }
    }
}
