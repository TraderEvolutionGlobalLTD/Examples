using Runtime.Script;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeApi.Indicators;

namespace UseExternalIndicator
{
    public class UseExternalIndicatorExample : IndicatorBuilder
    {

        [InputParameter(InputType.Numeric, "MFI Period", 0)]
        [SimpleNumeric(1.0, 99999.0)]
        public int mfiPeriod = 14;


        BuiltInIndicator custom;

        public UseExternalIndicatorExample() : base()
        {
            #region Initialization
            Credentials.Author = "";
            Credentials.Description = "";
            Credentials.Company = "";
            Credentials.Copyrights = "";
            Credentials.DateOfCreation = new DateTime(2020, 1, 16);
            Credentials.ExpirationDate = DateTime.MinValue;
            Credentials.Version = "1.0";
            Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
            Credentials.ProjectName = "UseExternalIndicatorExample";
            #endregion 

            Lines.Set("Test");
            Lines["Test"].Color = Color.Blue;
            Lines["Test"].Style = LineStyle.Dot;
            Lines["Test"].Width = 3;
        }

        public override void Init() 
        {
            custom = IndicatorsManager.CreateInstance("MFI", new object[] { "mfiPeriod", mfiPeriod }, HistoryDataSeries);
        }

        public override void Update(TickStatus args) 
        {
            Lines["Test"].SetValue(custom.GetValue());
        }

        public override void Complete() { }
    }
}
