using Runtime.Script;
using System.Collections.Generic;
using TradeApi.History;
using TradeApi.Indicators;
using TradeApi.Trading;

namespace DoubleBuy
{
    public class DoubleBuy : StrategyBuilder
    {
        public DoubleBuy()
           : base()
        {
            #region Initialization
            Credentials.ProjectName = "Double Buy Example";
            #endregion
        }

        [InputParameter(InputType.Numeric, "MA Period", 0)]
        [SimpleNumeric(1D, 99999)]
        public int period = 50;

        [InputParameter(InputType.Numeric, "Lot size", 3)]
        [SimpleNumeric(1D, 100D)]
        public double lotSize = 1;

        [InputParameter(InputType.Numeric, "Take Profit", 6)]
        [SimpleNumeric(1D, 99999)]
        public int takeProfit = 50; //Take Profit  

        [InputParameter(InputType.Numeric, "Stop Loss", 7)]
        [SimpleNumeric(1D, 99999)]
        public int stopLoss = 0; //Stop Loss

        [InputParameter(InputType.Combobox, "Type of Moving Average", 8)]
        [ComboboxItem("Simple", MAMode.SMA)]
        [ComboboxItem("Exponential", MAMode.EMA)]
        [ComboboxItem("Modified", MAMode.SMMA)]
        [ComboboxItem("Linear Weighted", MAMode.LWMA)]
        public MAMode maType = MAMode.SMA;

        private BuiltInIndicator ma; 

        private TradeStatus tradeStatus = TradeStatus.Allow;

        TradeResult tradeResult;
        private void OnFill(Order o) {

           if (o.IsTakeProfitOrder /*&& o.BoundTo.ID!=null && o.BoundTo.ID == tradeResult.Id*/) {
               tradeStatus = TradeStatus.Allow;
               tradeResult = OperationHandler(PositionSide.Long);
           }
        }

        public override void Init()
        {
            ma = IndicatorsManager.BuildIn.MA(HistoryDataSeries, period, maType);

            OrdersManager.OnFill += OnFill;
        }

        public override void Update(TickStatus args)
        {
            List<Order> requiredOrders = GetOrders();

            if (args != TickStatus.IsBar)
            {
                if(requiredOrders.Count == 0)
                {
                    tradeStatus = TradeStatus.Allow;
                }
                Entry();
            }
        }

        public override void Complete()
        {
            OrdersManager.OnFill -= OnFill;
        }

        private List<Order> GetOrders()
        {
            return OrdersManager.GetOrders(true, orders => orders.Instrument.Symbol == HistoryDataSeries.HistoricalRequest.Instrument.Symbol);
        }

        private TradeResult SendOrder(OrderType type, OrderSide side, double price, double lots)
        {
            var instrument = HistoryDataSeries.HistoricalRequest.Instrument;

            OrderRequest request = new OrderRequest(type, instrument, AccountManager.Current, side, lots)
            {
                StopLossPrice = stopLoss,
                TakeProfitPrice = takeProfit,
                Price = price,
                SLTPPriceType = SLTPPriceType.Ticks
            };
            tradeStatus = TradeStatus.Disallow;
            return OrdersManager.Send(request);
        }

        private void Entry()
        {
            if (HistoryDataSeries.Count - 1 <= period) {
                return;
            }
            double maValue1 = ma.GetValue(offset: 0, lineNumber: 0);
            double maValue2 = ma.GetValue(offset: 1, lineNumber: 0);
            double maValue3 = ma.GetValue(offset: 2, lineNumber: 0);

            double closePrice1 = HistoryDataSeries.GetValue(PriceType.Close, 0);
            double closePrice2 = HistoryDataSeries.GetValue(PriceType.Close, 1);
            double closePrice3 = HistoryDataSeries.GetValue(PriceType.Close, 2);

            var buySignal = maValue1 < closePrice1 && maValue2 < closePrice2 && maValue3 < closePrice3;

            tradeResult = new TradeResult();

            if (buySignal)
            {
                tradeResult = OperationHandler(PositionSide.Long);
            }

            if (tradeResult != null && tradeResult.HasError)
                Notification.Print("An error occured during order opening: " + tradeResult.Message);
        }

        private TradeResult OperationHandler(PositionSide operationType)
        {
            var instrument = HistoryDataSeries.HistoricalRequest.Instrument;
            TradeResult tradeResult = null;
            if (tradeStatus == TradeStatus.Allow)
            {
                var newOperation = operationType == PositionSide.Short ? OrderSide.Sell : OrderSide.Buy;
                var price = operationType == PositionSide.Short ? instrument.DayInfo.Bid : instrument.DayInfo.Ask;
                tradeResult = SendOrder(OrderType.Market, newOperation, price, lotSize);
            }
            return tradeResult;
        }

        private enum TradeStatus
        {
            Allow,
            Disallow
        }
    }
}
