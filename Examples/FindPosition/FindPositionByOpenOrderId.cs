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

namespace FindPosition
{
    public class FindPositionByOpenOrderId : StrategyBuilder
    {
        public FindPositionByOpenOrderId()
            : base()
        {
            #region Initialization
            Credentials.Author = "";
            Credentials.Description = "";
            Credentials.Company = "";
            Credentials.Copyrights = "";
            Credentials.DateOfCreation = new DateTime(2020, 1, 24);
            Credentials.ExpirationDate = DateTime.MinValue;
            Credentials.Version = "";
            Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
            Credentials.ProjectName = "FindPositionByOrderId";
            #endregion 


        }

        /// <summary>
        /// This function will be called after creating
        /// </summary>
        public override void Init()
        {
            PositionsManager.OnOpen += (p) =>
            {
                Notification.Print($"Open position Id: {p.ID}  OpenOrderId: {p.OpenOrderID}");

                if (orderId == p.OpenOrderID)
                {
                    Notification.Print($"Close position id {p.ID}");
                    PositionsManager.Close(p);
                }
                else
                    Notification.Print("Position not found");
            };
        }

        string orderId;

        /// <summary>
        /// Entry point. This function is called when new quote comes or new bar created
        /// </summary>
        public override void Update(TickStatus args)
        {

            if (counter > 0)
                return;

            counter++;

            orderId = PlaceMarketOrder(OrderSide.Buy, 1, ProductType.Delivery);

            Notification.Print($"Open Market order Id: {orderId}");
        }

        int counter = 0;

        string PlaceMarketOrder(OrderSide side, double amount, ProductType productType)
        {
            OrderRequest request = new OrderRequest(OrderType.Market, InstrumentsManager.Current, AccountManager.Current, side, amount)
            {
                ProductType = productType,
            };

            return OrdersManager.Send(request).Id;
        }

        /// <summary>
        /// This function will be called before removing
        /// </summary>
        public override void Complete()
        {

        }
    }
}
