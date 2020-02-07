using Runtime.Script;
using System;
using System.Windows.Forms;
using TradeApi.Indicators;
using TradeApi.ToolBelt;

namespace GetCursorPositionOnChart
{
    public class GetCursorPosition : IndicatorBuilder
    {
        Control chartControl;

        public GetCursorPosition()
            : base()
        {
            #region Initialization
            Credentials.Author = "";
            Credentials.Description = "";
            Credentials.Company = "";
            Credentials.Copyrights = "";
            Credentials.DateOfCreation = new DateTime(2020, 1, 31);
            Credentials.ExpirationDate = DateTime.MinValue;
            Credentials.Version = "";
            Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
            Credentials.ProjectName = "GetCursorPosition";
            #endregion
        }

        public override void Init()
        {
            chartControl = ChartSource.GetChartControl() as Control;
            if(chartControl != null)
                chartControl.MouseMove += ChartControl_MouseMove;
        }

        private void ChartControl_MouseMove(object sender, MouseEventArgs e)
        {
            var point = ChartSource.PointToChart(Cursor.Position); //getting the cursor position in the client coordinates
            int curWinNumber = ChartSource.FindWindow(this);
            var timeValue = ChartSource.GetTimeValue(point, curWinNumber);
            var curTime = Tools.TimeConvertor.UtcToCurrentTimeZone(timeValue.TimeUtc);
            var offset = HistoryDataSeries.FindInterval(timeValue.TimeUtc);
            var strOffset = offset != -1 ? offset.ToString() : "Not Selected";

            Notification.Comment($"Time: {curTime}, Price: {timeValue.Value}, BarIndex: {strOffset}");
        }

        public override void Update(TickStatus args)
        {
        }

        public override void Complete()
        {
            if (chartControl != null)
                chartControl.MouseMove -= ChartControl_MouseMove;
        }
    }
}
