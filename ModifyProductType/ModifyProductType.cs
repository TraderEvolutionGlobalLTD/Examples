using Runtime.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeApi.Trading;

namespace ModifyProductType
{
	public class ModifyProductTypeExample : StrategyBuilder
	{
		//Result with SuperPositionId
		TradeResult result;

		public ModifyProductTypeExample() : base()
		{
			#region Initialization
			Credentials.ProjectName = "ModifyProductTypeExample";
			#endregion
		}

		public override void Init()
		{
			var instrument = InstrumentsManager.Current;
			var account = AccountManager.Current;

			//Create 3 trades for SuperPosition with Product Type Intraday
			ProductType productType = ProductType.Intraday;
			OrderRequest trade1 = new OrderRequest(OrderType.Market, instrument, account, OrderSide.Buy, 1);
			OrderRequest trade2 = new OrderRequest(OrderType.Market, instrument, account, OrderSide.Buy, 2);
			OrderRequest trade3 = new OrderRequest(OrderType.Market, instrument, account, OrderSide.Buy, 3);
			trade1.ProductType = trade2.ProductType = trade3.ProductType = productType;

			//Place orders
			result = OrdersManager.Send(trade1);
			OrdersManager.Send(trade2);
			OrdersManager.Send(trade3);
		}

		public override void Update(TickStatus args)
		{
			if (args == TickStatus.IsQuote && result != null)
			{
				var positions = PositionsManager.GetPositions();

				//Change producType for trade2
				foreach (Position position in positions)
				{
					if (position.SuperPositionId == result.Id && position.Quantity == 2)
					{
						position.ChangeProductType(ProductType.Delivery);
						break;
					}
				}
				result = null;
			}
		}
	}
}
