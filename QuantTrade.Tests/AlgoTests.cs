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
    public class AlgoTests
    {

        [TestMethod()]
        public void EMATest()
        {
            int period =6;

            //Read a string
            double[] vars = { 145.44, 139.8, 140.8, 137.8, 145.8, 146.3, 144.1, 143.1, 145, 147, 145.8, 147, 144.8, 144.4, 140.3, 141.9, 140.8, 140.3, 135.9, 139.6 };
            ExponentialMovingAverage ema = new Core.Indicators.ExponentialMovingAverage(period);
            
            for (int i = 0; i < vars.Length; i++)
            {
                ema.UpdateIndicator(Convert.ToDecimal(vars[i]));

                if (i+1 == period) Assert.IsTrue(ema.IsReady);

                if (i == 5) Assert.IsTrue(Math.Round(ema.Value,2) == 142.66m, "Assert 5");
                if (i == 6) Assert.IsTrue(Math.Round(ema.Value, 2) == 143.07m, "Assert 6");
                if (i == 7) Assert.IsTrue(Math.Round(ema.Value, 2) == 143.08m, "Assert 7");
                if (i == 8) Assert.IsTrue(Math.Round(ema.Value, 2) == 143.63m, "Assert 8");
                if (i == 9) Assert.IsTrue(Math.Round(ema.Value, 2) == 144.59m, "Assert 9");

            }

        }


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
            Assert.IsTrue(broker.OrderHistory.Count == 0);
            broker.OnOrder += processMOCOrder;

            //Buy stock  
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Buy, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            Assert.IsTrue(broker.OrderHistory.Count == 1);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar.Close);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 2521m);
            Assert.IsTrue(broker.StockPortfolio[0].CurrentPrice == 25.21m);
            Assert.IsTrue(broker.StockPortfolio[0].TotalCurrentValue == 2521m);
            Assert.IsTrue(broker.TotalTrades ==1);
            Assert.IsTrue(broker.TotalTransactionFees == 7);
            Assert.IsTrue(broker.AvailableCash == 7472);
            Assert.IsTrue(broker.CurrentPortfolioValue == 2521m);

            //Buy more stock  
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Buy, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            Assert.IsTrue(broker.OrderHistory.Count == 2);
            Assert.IsTrue(broker.TotalTrades == 2);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 5042);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar.Close);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.TotalTransactionFees == 14);
            Assert.IsTrue(broker.TotalTradesCancelled == 0);
            Assert.IsTrue(broker.StockPortfolio[0].TotalCurrentValue == 5042);
            Assert.IsTrue(broker.AvailableCash == 4944);
            Assert.IsTrue(broker.CurrentPortfolioValue == 5042);

            //Exceed buy qty
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Buy, 1000); // execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            Assert.IsTrue(broker.OrderHistory.Count == 3);
            Assert.IsTrue(broker.TotalTrades == 2);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar.Close);
            Assert.IsTrue(broker.TotalTransactionFees == 14);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 5042);
            Assert.IsTrue(broker.TotalTradesCancelled == 1);
            Assert.IsTrue(broker.AvailableCash == 4944);
            Assert.IsTrue(broker.CurrentPortfolioValue == 5042);

            //Sell stock  
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Sell, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            Assert.IsTrue(broker.OrderHistory.Count == 4);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 2521);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar.Close);
            Assert.IsTrue(broker.TotalTrades == 3);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.TotalTransactionFees == 21);
            Assert.IsTrue(broker.AvailableCash == 7458);
            Assert.IsTrue(broker.CurrentPortfolioValue == 2521);

            //Sell remaining stock  
            broker.ExecuteOrder(tradeBar, OrderType.MOC, Core.Action.Sell, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            Assert.IsTrue(broker.TotalTrades == 4);
            Assert.IsTrue(broker.OrderHistory.Count == 5);
            Assert.IsTrue(broker.StockPortfolio.Count == 0);
            Assert.IsTrue(broker.TotalTransactionFees == 28);
            Assert.IsTrue(broker.AvailableCash == 9972m);
            Assert.IsTrue(broker.CurrentPortfolioValue == 0);
            Assert.IsTrue(broker.StartingCash == 10000m);

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
            Assert.IsTrue(broker.OrderHistory.Count == 0);
            broker.OnOrder += processMOOOrder;

            //Buy stock - order gets queued becuase it is MOO 
            broker.ExecuteOrder(tradeBar1, OrderType.MOO, Core.Action.Buy, 100); //execute
            Assert.IsTrue(broker.PendingOrderQueue.Count == 1);
            Assert.IsTrue(broker.OrderHistory.Count == 0);
            Assert.IsTrue(broker.StockPortfolio.Count == 0);
            Assert.IsTrue(broker.TotalTrades == 0);
            Assert.IsTrue(broker.TotalTransactionFees == 0);
            Assert.IsTrue(broker.AvailableCash == 10000);
            Assert.IsTrue(broker.CurrentPortfolioValue == 0);


            //Process orders in the QUEUE by sending in a new order
            broker.ProcessNewTradebar(tradeBar2);
            Assert.IsTrue(broker.PendingOrderQueue.Count == 0);
            Assert.IsTrue(broker.OrderHistory.Count == 1);
            Assert.IsTrue(broker.TotalTrades == 1);
            Assert.IsTrue(broker.StockPortfolio[0].AverageFillPrice == tradeBar2.Open);
            Assert.IsTrue(broker.StockPortfolio[0].TotalInvested == 2549);
            Assert.IsTrue(broker.StockPortfolio.Count == 1);
            Assert.IsTrue(broker.TotalTransactionFees == 7);
            Assert.IsTrue(broker.TotalTradesCancelled == 0);
            Assert.IsTrue(broker.AvailableCash == 7444);
            Assert.IsTrue(broker.StockPortfolio[0].CurrentPrice == 26.21m);
            Assert.IsTrue(broker.StockPortfolio[0].TotalCurrentValue == 2621m);
            Assert.IsTrue(broker.CurrentPortfolioValue == 2621);

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
        

        [TestMethod()]
        public void RSITest()
        {
            int period = 2;

            //Read a string
            double[] vars = { 242.64, 244.66, 243.01, 242.95, 242.84, 243.13, 243.29, 241.33, 243.49, 241.35, 241.8, 242.21, 242.77, 240.55, 242.11, 242.37, 242.19, 244.01, 244.42, 245.56, 245.53 };

            RelativeStrengthIndex rsi = new RelativeStrengthIndex(period, MovingAverageType.Wilders);

            for (int i = 0; i < vars.Length; i++)
            {
                rsi.UpdateIndicator(Convert.ToDecimal(vars[i]));

                if (i == period * 5)
                    Assert.IsTrue(rsi.IsReady);

                if (i == 0) Assert.IsTrue(Math.Round(rsi.Value, 0) == 100m, "Assert 0");
                if (i == 1) Assert.IsTrue(Math.Round(rsi.Value, 0) == 100m, "Assert 1");
                if (i == 2) Assert.IsTrue(Math.Round(rsi.Value, 0) == 55m, "Assert 2");
                if (i == 3) Assert.IsTrue(Math.Round(rsi.Value, 0) == 53m, "Assert 3");
                if (i == 4) Assert.IsTrue(Math.Round(rsi.Value, 0) == 48m, "Assert 4");

                if (i == 5) Assert.IsTrue(Math.Round(rsi.Value, 0) == 66m, "Assert 5");
                if (i == 6) Assert.IsTrue(Math.Round(rsi.Value, 0) == 76m, "Assert 6");
                if (i == 7) Assert.IsTrue(Math.Round(rsi.Value, 0) == 10m, "Assert 7");
                if (i == 8) Assert.IsTrue(Math.Round(rsi.Value, 0) == 69m, "Assert 8");
                if (i == 9) Assert.IsTrue(Math.Round(rsi.Value, 0) == 30m, "Assert 9");
                
            }
            
        }

        [TestMethod()]
        public void SMATest()
        {
            int period = 2;

            //Read a string
            double[] vars = { 242.64, 244.66, 243.01, 242.95, 242.84, 243.13, 243.29, 241.33, 243.49, 241.35, 241.8, 242.21, 242.77, 240.55, 242.11, 242.37, 242.19, 244.01, 244.42, 245.56, 245.53 };

            SimpleMovingAverage sma = new SimpleMovingAverage(period);

            for (int i = 0; i < vars.Length; i++)
            {
                sma.UpdateIndicator(Convert.ToDecimal(vars[i]));

                if (i == 1) Assert.IsTrue(sma.IsReady);
                
                if (i == 0) Assert.IsTrue(Math.Round(sma.Value, 2) == 242.64m, "Assert 0");
                if (i == 1) Assert.IsTrue(Math.Round(sma.Value, 2) == 243.65m, "Assert 1");
                if (i == 2) Assert.IsTrue(Math.Round(sma.Value, 2) == 243.84m, "Assert 2");
                if (i == 3) Assert.IsTrue(Math.Round(sma.Value, 2) == 242.98m, "Assert 3");
                if (i == 4) Assert.IsTrue(Math.Round(sma.Value, 2) == 242.90m, "Assert 4");

                //Debug.WriteLine(Math.Round(sma.Value,2));

            }

        }

        [TestMethod()]
        public void ReadDataTest()
        { 
            string symbol = "DONOTDELETE";

            IDataReader reader = new CSVReader();
            reader.OnTradeBar += Reader_OnData;
            reader.ReadData(symbol, Resolution.Daily);
            
        }

        [TestMethod()]
        private void Reader_OnData(TradeBar data, EventArgs e)
        {
            Assert.IsTrue(data.High > 0, "high");
            Assert.IsTrue(data.Low > 0, "low");
            Assert.IsTrue(data.Symbol == "DONOTDELETE", "symbol");
            Assert.IsTrue(data.TradeResolution == Resolution.Daily,  "traderes");
            Assert.IsTrue(data.Volume > 0,  "volume");
        }


        [TestMethod()]
        public void RunSimpleAlgo()
        {
            //IAlogorithm algo = new SimpleAlogrithm();
            //algo.Initialize();
        }
    }

}