/*
* BSD 2-Clause License 
* Copyright (c) 2017, Rich Lane 
* All rights reserved. 
* 
* Redistribution and use in source and binary forms, with or without 
* modification, are permitted provided that the following conditions are met: 
* 
* Redistributions of source code must retain the above copyright notice, this 
* list of conditions and the following disclaimer. 
* 
* Redistributions in binary form must reproduce the above copyright notice, 
* this list of conditions and the following disclaimer in the documentation 
* and/or other materials provided with the distribution. 
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE 
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/

using QuantTrade.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Securities
{
    /// <summary>
    /// This class is the manages our portfolio and handles everything related to buying and selling stocks. 
    /// </summary>
    public class Broker
    {
        #region Events

        public event OnOrderHandler OnOrder;
        public EventArgs e = null;

        #endregion

        #region Statistics

        private decimal _totalWins;

        private decimal _totalLosses;

        private int _totalSellTrades;

        public double CompoundingAnnualPerformance
        {
            get
            {
                //Number of years in this dataset:
                double years = (EquityOverTime.Keys.LastOrDefault() - EquityOverTime.Keys.FirstOrDefault()).TotalDays / 365;
                return Math.Round((Math.Pow((double)TotalEquity / (double)StartingCash, (1 / (double)years)) - 1) , 4);
            }

        }

        public SortedDictionary<DateTime, decimal> EquityOverTime { get; private set; }

        public decimal WinRate
        {
            get
            {
                //Buy and hold
                if (_totalWins == 0 && _totalLosses == 0 && TotalTrades == 1) return 1;

                //Avoid divide error
                if (_totalWins == 0 && _totalSellTrades == 0) return 0;

         
                decimal rate = _totalWins / _totalSellTrades;
                return Math.Round(rate,2);
            }
        }

        public decimal LossRate
        {
            get
            {
                //Avoid divide error
                if (_totalLosses == 0 && _totalSellTrades == 0) return 0;

                decimal rate = _totalLosses / _totalSellTrades;
                return Math.Round(rate,2);
            }
        }

        public Decimal MaxDrawdownPercent
        {
            get
            {
                var prices = EquityOverTime.Values.ToList();
                if (prices.Count == 0) return 0;

                var drawdowns = new List<decimal>();
                var high = prices[0];
                foreach (var price in prices)
                {
                    if (price > high) high = price;
                    if (high > 0) drawdowns.Add(price / high - 1);
                }

                return Math.Round(Math.Abs(drawdowns.Min()), 2);
            }
        }

        public decimal NetProfit
        {
            get
            {
                return Math.Round(((TotalEquity - StartingCash) / StartingCash), 2);

            }
        }

        /// <summary>
        /// Total value of cash on hand and stocks in the portfolio
        /// </summary>
        public Decimal TotalEquity
        {
            get
            {
                return Math.Round(PortfolioValue + AvailableCash, 2);

            }
        }

        #endregion


        #region Properties

        private bool _allowMargin;
     
        public List<Order> PendingOrderQueue { get; private set; }
        
        public decimal StartingCash { get; private set; }
        
        public Decimal TransactionFee { get; private set; }

        public int TotalTradesCancelled { get; private set; }

        public Decimal AvailableCash { get; private set; }

        public int TotalTrades { get; private set; }
        
        public decimal TotalTransactionFees { get; private set; }

        public List<Stock> StockPortfolio { get; private set; }

        /// <summary>
        /// Total value of stock held in the portfolio
        /// </summary>
        public Decimal PortfolioValue
        {
            get
            {
                decimal totalValue = 0;

                foreach (Stock stock in StockPortfolio)
                {
                    totalValue += stock.CurrentPrice * stock.Quantity;
                }

                return totalValue;
            }
        }

     
        #endregion

        /// <summary>
        /// Constructor - called by the unti test. Values are passed in as opposed to read from the json config file.
        /// </summary>
        public Broker(decimal startingCash, decimal transactionFee, bool allowMargin)
        {
            _allowMargin = allowMargin;
            StartingCash = startingCash;
            AvailableCash = StartingCash;
            TransactionFee = transactionFee;

            initCollections();
        }


        /// <summary>
        /// Constructor - called by the BaseAlgorithm class. Values are read from the json config file.
        /// </summary>
        public Broker()
        {
            _allowMargin = Convert.ToBoolean(Utilities.Config.GetToken("allow-margin"));
            TransactionFee = Convert.ToDecimal(Utilities.Config.GetToken("transaction-fee"));
            StartingCash = Convert.ToDecimal(Utilities.Config.GetToken("starting-cash"));
            AvailableCash = StartingCash;

            initCollections();

        }

        /// <summary>
        /// Initializes the collections.
        /// </summary>
        private void initCollections()
        {
            StockPortfolio = new List<Stock>();
            EquityOverTime = new SortedDictionary<DateTime, decimal>();
            PendingOrderQueue = new List<Order>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool IsHoldingStock(string symbol)
        {
            return StockPortfolio.Exists(p => p.Symbol == symbol);
        }

        /// <summary>
        /// Updates holdings for filled orders.
        /// </summary>
        private void updateHoldings(Order order)
        {
            //Only process filled orders
            if (order.Status != OrderStatus.Filled) return;

            //OrderHistory.Add(order);
            TotalTrades++;

            //Transaction fees
            TotalTransactionFees += TransactionFee;
            AvailableCash -= TransactionFee;

            decimal orderTotal = order.Quantity * order.FillPrice;

            ///////////////////////////////
            //Buying
            //////////////////////////////
            switch (order.Action)
            {
                //Buying
                case Action.Buy:
                    AvailableCash -= orderTotal;

                    //Find existing stock in holdings
                    if (IsHoldingStock(order.Symbol))
                    {
                        Stock stock = StockPortfolio.Find(p => p.Symbol == order.Symbol);
                        stock.Quantity += order.Quantity;
                        stock.TotalInvested += orderTotal;
                        stock.CurrentPrice = order.FillPrice;
                    }
                    //Create a new stock object and add to holdings
                    else
                    {
                        Stock newStock = new Stock()
                        {
                            Symbol = order.Symbol,
                            Quantity = order.Quantity,
                            TotalInvested = orderTotal,
                            CurrentPrice = order.FillPrice
                        };

                        StockPortfolio.Add(newStock);
                    }
                    break;

                ///////////////////////
                //Selling
                ///////////////////////
                case Action.Sell:
                    AvailableCash += orderTotal;
                    _totalSellTrades++;

                    if (IsHoldingStock(order.Symbol))
                    {
                        Stock stock = StockPortfolio.Find(p => p.Symbol == order.Symbol);

                        //Is this a win or loss?
                        if (orderTotal > (stock.AverageFillPrice * order.Quantity))
                        {
                            _totalWins++;
                        }
                        else
                        {
                            _totalLosses++;
                        }
                        stock.TotalInvested -= stock.AverageFillPrice * order.Quantity;
                        stock.Quantity -= order.Quantity;

                        //remove the stock from the portfolio if no longer holding any.
                        if (stock.Quantity <= 0)
                        {
                            StockPortfolio.Remove(stock);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Order validation.
        /// </summary>
        private bool validateOrder(Order order)
        {
            bool isValid = true;

            //Buy side validation
            if (order.Action == Action.Buy)
            {
                //See if we can afford the buy order
                if (AvailableCash < (order.Quantity * order.FillPrice))
                {
                    if (!_allowMargin)
                        isValid = false;
                }
            }
            //Sell side validation
            else if (order.Action == Action.Sell)
            {
                //Make sure we have the stock befoe we sell it
                if (!StockPortfolio.Exists(p => p.Symbol == order.Symbol && p.Quantity >= order.Quantity))
                {
                    isValid = false;
                }
            }

            //Cancel order
            if (isValid == false)
            {
                order.Status = OrderStatus.Cancelled;
                order.FillPrice = 0;
                order.Quantity = 0;
                TotalTradesCancelled++;
            }

            return isValid;
        }

        /// <summary>
        /// Determintes how to process the order (i.e. MOC, MOO, etc).
        /// </summary>
        private void fillOrder(TradeBar tradeBar, Order order)
        {
            //Makes sure we are supposed to be here
            if (order.Status != OrderStatus.New && order.Status != OrderStatus.Pending)
                return;

            //Put MOO orders in the QUEUE and exit the method. We will process them tommorow.
            if (order.OrderType == OrderType.MOO && order.Status == OrderStatus.New)
            {
                order.Status = OrderStatus.Pending;
                PendingOrderQueue.Add(order);
                return;
            }

            //Which price to use for fill?
            if (order.OrderType == OrderType.MOO)
            {
                //Use the current opening price of the tradebar
                order.FillPrice = tradeBar.Open;
            }
            else
            {
                //Use the current closing price of the tradebar
                order.FillPrice = tradeBar.Close;
            }

            //VALIDATION REQUIRED - make sure we can afford to buy the stock
            if (validateOrder(order) == false) return;

            //Update porfolio
            order.OrderType = OrderType.Market; // Convert all order to market orders
            order.FillDate = tradeBar.Day;
            order.Status = OrderStatus.Filled;

            updateHoldings(order);

            //Throw event
            if (OnOrder != null)
            {
                OnOrder(order, e);
            }
        }

        /// <summary>
        /// Called by the base alogrithm.
        /// </summary>
        public void ExecuteOrder(TradeBar tradeBar, OrderType orderType, Action action, int quantity)
        {
            //Create order object then build, validate, and process.
            Order order = order = new Order()
            {
                Symbol = tradeBar.Symbol,
                Action = action,
                OrderType = orderType,
                Quantity = quantity,
                DateSubmitted = tradeBar.Day
            };

            fillOrder(tradeBar, order);
        }

        /// <summary>
        /// Processing order sitting in the queue when a new trade bar comes in (i.e. MOO orders)
        /// and updates the current value of stocks on our portfolio.
        /// Called from base algo.
        /// </summary>
        public void ProcessNewTradebar(TradeBar tradeBar)
        {
            //Pick up pending MOO orders placed yesterday
            for (int i = 0; i < PendingOrderQueue.Count; i++)
            {
                Order order = PendingOrderQueue[i];

                if (order.OrderType == OrderType.MOO &&  order.Status == OrderStatus.Pending)
                {
                    fillOrder(tradeBar, order);
                    PendingOrderQueue.RemoveAt(i);
                }
            }

            //Update current stock price of stocks in the portfolio
            if (IsHoldingStock(tradeBar.Symbol))
            {
                StockPortfolio.Find(p => p.Symbol == tradeBar.Symbol).CurrentPrice = tradeBar.Close;
            }

            //Update equity over time
            EquityOverTime.Add(tradeBar.Day, TotalEquity);
        }

      

    }
}
