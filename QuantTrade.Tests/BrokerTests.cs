using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantTrade.Core;
using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core.Indicators;
using System.Diagnostics;
using QuantTrade.Core.Securities;

namespace QuantTrade.Tests
{
    [TestClass()]
    public class BrokerTests
    {
        [TestMethod()]
        public void BrokerMOCTest()
        {
            //
            TradeBar tradeBar = new TradeBar()
            {
              Close=25.21m,
              Day=DateTime.Today,
              High=26m,
              Low=24m,
              Open=24.49m,
              Symbol="SPY",
              TradeResolution= Resolution.Daily,
              Volume=10000
            };

            //
            Broker broker = new Broker(10000m, 7m, false);
         
            Assert.IsTrue(broker.TransactionFee == 7m);
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            //Assert.IsTrue(broker.OrderHistory.Count == 0);
            broker.OnOrder += processMOCOrder;

            //Buy stock  
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Buy, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            //Assert.IsTrue(broker.OrderHistory.Count == 1);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar.Close);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 2521m);
            Assert.IsTrue(broker.StockPortfolio[0].CurrentPrice == 25.21m);
            Assert.IsTrue(broker.StockPortfolio[0].TotalCurrentValue == 2521m);
            Assert.IsTrue(broker.TotalTrades ==1);
            Assert.IsTrue(broker.TotalTransactionFees == 7);
            Assert.IsTrue(broker.AvailableCash == 7472);
            Assert.IsTrue(broker.PortfolioValue == 2521m);
            Assert.IsTrue(broker.TotalEquity == broker.PortfolioValue + broker.AvailableCash);

            //Buy more stock  
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Buy, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
           // Assert.IsTrue(broker.OrderHistory.Count == 2);
            Assert.IsTrue(broker.TotalTrades == 2);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 5042);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar.Close);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.TotalTransactionFees == 14);
            Assert.IsTrue(broker.TotalTradesCancelled == 0);
            Assert.IsTrue(broker.StockPortfolio[0].TotalCurrentValue == 5042);
            Assert.IsTrue(broker.AvailableCash == 4944);
            Assert.IsTrue(broker.PortfolioValue == 5042);
            Assert.IsTrue(broker.TotalEquity == broker.PortfolioValue + broker.AvailableCash);


            //Exceed buy qty
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Buy, 1000); // execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            //Assert.IsTrue(broker.OrderHistory.Count == 3);
            Assert.IsTrue(broker.TotalTrades == 2);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar.Close);
            Assert.IsTrue(broker.TotalTransactionFees == 14);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 5042);
            Assert.IsTrue(broker.TotalTradesCancelled == 1);
            Assert.IsTrue(broker.AvailableCash == 4944);
            Assert.IsTrue(broker.PortfolioValue == 5042);

            //Sell stock  
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Sell, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            //Assert.IsTrue(broker.OrderHistory.Count == 4);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 2521);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar.Close);
            Assert.IsTrue(broker.TotalTrades == 3);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.TotalTransactionFees == 21);
            Assert.IsTrue(broker.AvailableCash == 7458);
            Assert.IsTrue(broker.PortfolioValue == 2521);
            Assert.IsTrue(broker.TotalEquity == broker.PortfolioValue + broker.AvailableCash);

            //Sell remaining stock  
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Sell, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            Assert.IsTrue(broker.TotalTrades == 4);
            //Assert.IsTrue(broker.OrderHistory.Count == 5);
            Assert.IsTrue(broker.StockPortfolio.Count == 0);
            Assert.IsTrue(broker.TotalTransactionFees == 28);
            Assert.IsTrue(broker.AvailableCash == 9972m);
            Assert.IsTrue(broker.PortfolioValue == 0);
            Assert.IsTrue(broker.StartingCash == 10000m);
            Assert.IsTrue(broker.TotalEquity == broker.PortfolioValue + broker.AvailableCash);

            string x = "";
        }

        [TestMethod()]
        public void BrokerMOOTest()
        {
            //
            TradeBar tradeBar1 = new TradeBar()
            {
                Close = 25.21m,
                Day = DateTime.Today.AddDays(-2),
                High = 26m,
                Low = 24m,
                Open = 24.49m,
                Symbol = "SPY",
                TradeResolution = Resolution.Daily,
                Volume = 10000
            };

            TradeBar tradeBar2 = new TradeBar()
            {
                Close = 26.21m,
                Day = DateTime.Today.AddDays(-1),
                High = 27m,
                Low = 25m,
                Open = 25.49m,
                Symbol = "SPY",
                TradeResolution = Resolution.Daily,
                Volume = 10000
            };

            TradeBar tradeBar3 = new TradeBar()
            {
                Close = 27.21m,
                Day = DateTime.Today,
                High = 28m,
                Low = 26m,
                Open = 26.49m,
                Symbol = "SPY",
                TradeResolution = Resolution.Daily,
                Volume = 10000
            };
            //
            Broker broker = new Broker(10000m, 7m, false);

            Assert.IsTrue(broker.TransactionFee == 7m);
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            //Assert.IsTrue(broker.OrderHistory.Count == 0);
            broker.OnOrder += processMOOOrder;

            //Buy stock - order gets queued becuase it is MOO 
            broker.ExecuteOrder(tradeBar1, OrderType.MOO, Core.Action.Buy, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 1);
            //Assert.IsTrue(broker.OrderHistory.Count == 0);
            Assert.IsTrue(broker.StockPortfolio.Count == 0);
            Assert.IsTrue(broker.TotalTrades == 0);
            Assert.IsTrue(broker.TotalTransactionFees == 0);
            Assert.IsTrue(broker.AvailableCash == 10000);
            Assert.IsTrue(broker.PortfolioValue == 0);


            //Process orders in the QUEUE by sending in a new order
            broker.ProcessNewTradebar(tradeBar2);
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            //Assert.IsTrue(broker.OrderHistory.Count == 1);
            Assert.IsTrue(broker.TotalTrades == 1);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar2.Open);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 2549);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.TotalTransactionFees == 7);
            Assert.IsTrue(broker.TotalTradesCancelled == 0);
            Assert.IsTrue(broker.AvailableCash == 7444);
            Assert.IsTrue(broker.StockPortfolio[0].CurrentPrice == 26.21m);
            Assert.IsTrue(broker.StockPortfolio[0].TotalCurrentValue == 2621m);
            Assert.IsTrue(broker.PortfolioValue == 2621);
            Assert.IsTrue(broker.TotalEquity == broker.PortfolioValue + broker.AvailableCash);

            //Sell stock - order gets queued becuase it is MOO 

            string x = "";
        }

        private void processMOOOrder(Order data, EventArgs e)
        {
            Assert.IsTrue(data.FillPrice == 25.49m);
            Assert.IsTrue(data.Quantity == 100);
            Assert.IsTrue(data.Status == OrderStatus.Filled);
            Assert.IsTrue(data.OrderType == OrderType.Market);
            Assert.IsTrue(data.Symbol == "SPY");
        }

        private void processMOCOrder(Order data, EventArgs e)
        {
            Assert.IsTrue(data.FillDate == DateTime.Today);
            Assert.IsTrue(data.FillPrice == 25.21m);
            Assert.IsTrue(data.DateSubmitted == DateTime.Today);
            Assert.IsTrue(data.Quantity == 100);
            Assert.IsTrue(data.Status ==  OrderStatus.Filled);
            Assert.IsTrue(data.OrderType == OrderType.Market);
            Assert.IsTrue(data.Symbol == "SPY");
        }
        

     
    }

}