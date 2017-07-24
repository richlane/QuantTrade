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

        private bool _allowMargin;

        private decimal _totalWins;

        private decimal _totalLosses;

        public decimal WinRate { get; private set; }

        public decimal LossRate { get; private set; }

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
                foreach (Stock stock in StockPortfolio)
                {
                   
                }
                return 0m;
            }
        }


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
        private void finalizeOrder(Order order)
        {
            //Only process filled orders
            if (order.Status != OrderStatus.Filled) return;

            OrderHistory.Add(order);
            TotalTrades++;

            //Transaction fees
            TotalTransactionFees += TransactionFee;
            AvailableCash -= TransactionFee;

            decimal totalInvested = order.Quantity * order.FillPrice;

            ///////////////////////////////
            //Buying
            //////////////////////////////
            switch (order.Action)
            {
                //Buying
                case Action.Buy:
                    AvailableCash -= totalInvested;

                    //Find existing stock in holdings
                    if (IsHoldingStock(order.Symbol))
                    {
                        Stock stock = StockPortfolio.Find(p => p.Symbol == order.Symbol);
                        stock.Quantity += order.Quantity;
                        stock.TotalInvested += totalInvested;
                        stock.CurrentPrice = order.FillPrice;
                    }
                    //Create a new stock object and add to holdings
                    else
                    {
                        Stock newStock = new Stock()
                        {
                            Symbol = order.Symbol,
                            Quantity = order.Quantity,
                            TotalInvested = totalInvested,
                            CurrentPrice = order.FillPrice
                        };

                        StockPortfolio.Add(newStock);
                    }
                    break;

                ///////////////////////
                //Selling
                ///////////////////////
                case Action.Sell:
                    AvailableCash += (order.Quantity * order.FillPrice);

                    if (StockPortfolio.Exists(p => p.Symbol == order.Symbol))
                    {
                        Stock stock = StockPortfolio.Find(p => p.Symbol == order.Symbol);
                        stock.Quantity -= order.Quantity;
                        stock.TotalInvested -= totalInvested;

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
        /// Processing order sitting in the queue when a new trade bar comes in (i.e. MOO orders).
        /// Called from base algo.
        /// </summary>
        public void ProcessPendingOrderQueue(TradeBar tradeBar)
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
                    fillOrder(tradeBar, order);

                    PendingOrderQueue.RemoveAt(i);
                }
            }

            //Update current value of portfolio
            if (IsHoldingStock(tradeBar.Symbol))
            {
                StockPortfolio.Find(p => p.Symbol == tradeBar.Symbol).CurrentPrice = tradeBar.Close;
            }
        }

        /// <summary>
        /// Fills the order.
        /// </summary>
        private void fillOrder(TradeBar tradeBar, Order order)
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

            finalizeOrder(order);
    
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
            //Create order object
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

    }
}
