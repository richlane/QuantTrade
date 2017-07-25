using QuantTrade.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Securities
{
    public class Broker
    {
        #region Events

        public event OnOrderHandler OnOrder;
        public EventArgs e = null;

        #endregion

        #region Order Queues

        public List<Order> OrderHistory { get; private set; }

        public List<Order> PendingOrderQueue { get; private set; }

        #endregion

        #region Portfolio
        private decimal _highestPrice;

        private decimal _lowestPrice;

        private bool _allowMargin;

        private decimal _totalWins;

        private decimal _totalLosses;

        private int _totalSellTrades;

        public int WinRate
        {
            get
            {
                if (_totalWins == 0 && _totalSellTrades == 0)
                    return 0;

                decimal rate = _totalWins / _totalSellTrades;
                return Convert.ToInt32(Math.Round(rate * 100));
            }
        }
       
        public int LossRate
        {
            get
            {
                if (_totalLosses == 0 && _totalSellTrades == 0)
                    return 0;

                decimal rate =  _totalLosses / _totalSellTrades;
                return Convert.ToInt32(Math.Round(rate * 100));
            }
        }

        public decimal StartingCash { get; private set; }

        public Decimal TransactionFee { get; private set; }

        public Decimal TotalTradesCancelled { get; private set; }

        public Decimal AvailableCash { get; private set; }

        public Decimal TotalTrades { get; private set; }

        public decimal TotalTransactionFees { get; private set; }

        public List<Stock> StockPortfolio { get; private set; }

        public Decimal CurrentPortfolioValue
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

        //public Decimal MaxDrawdown
        //{
        //    get
        //    {
        //     return   _lowestPrice - _highestPrice / _highestPrice;
        //    }
        //}

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public Broker(decimal startingCash, decimal transactionFee, bool allowMargin)
            {
                StartingCash = startingCash;
                AvailableCash = startingCash;
                TransactionFee = transactionFee;

                StockPortfolio = new List<Stock>();

                OrderHistory = new List<Order>();
                PendingOrderQueue = new List<Order>();

                _allowMargin = allowMargin;
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
        /// Updated holdings when an order gets filled.
        /// </summary>
        private void processOrder(Order order)
        {
            //Only process filled orders
            if (order.Status != OrderStatus.Filled) return;

            OrderHistory.Add(order);
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
                if (AvailableCash < (order.Quantity * order.FillPrice))
                {
                    if( !_allowMargin )
                        isValid = false;
                }
            }
            //Sell side validation
            else if (order.Action == Action.Sell)
            {
                //Make sure we have the stock
                if (!StockPortfolio.Exists(p => p.Symbol == order.Symbol && p.Quantity >= order.Quantity))
                {

                    // Logger.Log($"{order.FillDate} - Unable to process Sell order: {order.Symbol} not in Porfolio or qty exceeded.");
                    isValid = false;
                }
            }

            //Cancel order
            if (isValid == false)
            {
                order.Status = OrderStatus.Cancelled;
                order.FillPrice = 0;
                order.Quantity = 0;
                OrderHistory.Add(order);
                TotalTradesCancelled++;
            }

            return isValid;
        }

        /// <summary>
        /// Fills the order.
        /// </summary>
        private void buildOrder(TradeBar tradeBar, Order order)
        {
            //Makes sure we are supposed to be here
            if (order.Status != OrderStatus.New && order.Status != OrderStatus.Pending)
                return; 
           
            //put MOO orders in the QUEUE
            if (order.OrderType == OrderType.MOO && order.Status == OrderStatus.New) 
            {
                order.Status = OrderStatus.Pending;
                PendingOrderQueue.Add(order);
                return;
            }

            //Process ready orders 
            //Which price to use for fill?
            order.FillPrice = tradeBar.Close;

            if(order.OrderType == OrderType.MOO)
            {
                order.FillPrice = tradeBar.Open;
            }
          
           //VALIDATION REQUIRED - make sure we can afford to buy the stock
           if (validateOrder(order) == false) return;
      
            //Update porfolio
            order.OrderType = OrderType.Market;
            order.FillDate = tradeBar.Day;
            order.Status = OrderStatus.Filled;

            processOrder(order);
    
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

            buildOrder(tradeBar, order);
        }

        /// <summary>
        /// Processing order sitting in the queue when a new trade bar comes in (i.e. MOO orders)
        /// and updates the current proces of stocks on our portfolio.
        /// Called from base algo.
        /// </summary>
        public void ProcessNewTradebar(TradeBar tradeBar)
        {
            for (int i = 0; i < PendingOrderQueue.Count; i++)
            {
                Order order = PendingOrderQueue[i];

                //I removed the date check becuase I was not taking into consideration weekends
                // tradeBar.Day.AddDays(-1).ToShortDateString() == order.DateSubmitted.ToShortDateString())

                //Pick up MOO orders placed yesterday
                if (order.OrderType == OrderType.MOO &&
                    order.Status == OrderStatus.Pending)
                {
                    buildOrder(tradeBar, order);

                    PendingOrderQueue.RemoveAt(i);
                }
            }

            //Update current value of portfolio
            if (IsHoldingStock(tradeBar.Symbol))
            {
                StockPortfolio.Find(p => p.Symbol == tradeBar.Symbol).CurrentPrice = tradeBar.Close;
            }

            //Calculate drawdown
            //calcDrawdown(tradeBar);
        }

        /// <summary>
        /// Calculates max drawdown
        /// </summary>
        /// <param name="tradebar"></param>
        //private void calcDrawdown(TradeBar tradebar)
        //{
        //    if (tradebar.High >= _highestPrice)
        //    {
        //        _highestPrice = tradebar.High;
        //    }


        //    if (tradebar.Low <= _lowestPrice)
        //    {
        //        _lowestPrice = tradebar.Low ;
        //    }
        //}

    }
}
