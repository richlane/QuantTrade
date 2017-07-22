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

        private decimal _totalWins;

        private decimal _totalLosses;

        public decimal WinRate { get; private set; }

        public decimal LossRate { get; private set; }

        public decimal StartingCash { get; private set; }

        public Decimal TransactionFee { get; private set; }

        public Decimal TotalTradesCancelled { get; private set; }

        public Decimal AvailableCash { get; private set; }

        public Decimal TotalTrades { get; private set; }

        //public Decimal PortfolioValue
        //{
        //    get
        //    {
        //        decimal total = 0;
        //        foreach (Stock item in StockPortfolio)
        //        {
        //            total += item.StockValue;
        //        }

        //        return total;
        //    }
        //}

        //public decimal TotalProfits { get; set; }

        public decimal TotalTransactionFees { get; private set; }
        
        public List<Stock> Holdings { get; private set; }
        
        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public Broker(decimal startingCash, decimal transactionFee)
        {
            StartingCash = startingCash;
            AvailableCash = startingCash;
            TransactionFee = transactionFee;

            Holdings  = new List<Stock>();

            OrderHistory = new List<Order>();
            PendingOrderQueue = new List<Order>();
        }

        /// <summary>
        /// Updates holdings w/new trade bar data. Called from base alogo.
        /// </summary>
        /// <param name="tradeBar"></param>
        public void UpdateHoldingsValue(TradeBar tradeBar)
        {
            throw new ApplicationException("Not implemented");
        }
        
        /// <summary>
        /// Updated holdings when an order gets filled.
        /// </summary>
        private void updateHoldingsForFilledOrder(Order order)
        {
            if (order.Status != OrderStatus.Filled) return;
                            
            OrderHistory.Add(order);
            TotalTrades++;

            //Transaction fees
            TotalTransactionFees += TransactionFee;
            AvailableCash -= TransactionFee;

            Stock newStock = new Stock()
            {
                Symbol = order.Symbol,
                Quantity = order.Quantity,
                FillPrice=order.FillPrice
            };

            ///////////////////////////////
            //Update buy transaction
            //////////////////////////////
            if (order.Action == Action.Buy)
            {
                AvailableCash -= (order.Quantity * order.FillPrice);

                if(Holdings.Exists(p => p.Symbol == newStock.Symbol))
                {
                    Stock existingStock = Holdings.Find(p => p.Symbol == newStock.Symbol);
                    existingStock.Quantity += newStock.Quantity;
                    existingStock.FillPrice = newStock.FillPrice;
                }
                else
                {
                    Holdings.Add(newStock);
                }
            }
            ///////////////////////////////
            //Update sell transaction
            //////////////////////////////
            else if (order.Action == Action.Sell)
            {
                AvailableCash += (order.Quantity * order.FillPrice);

                if (Holdings.Exists(p => p.Symbol == newStock.Symbol))
                {
                    Stock existingStock  = Holdings.Find(p => p.Symbol == newStock.Symbol);
                    existingStock.Quantity -= order.Quantity;

                    if (existingStock.Quantity <=0 )
                    {
                        Holdings.Remove(existingStock);
                    }
                }
           
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
                    Logger.Log($"{order.FillDate} - Unable to process Buy order: Insufficient funds.");
                    isValid = false;
                }
            }
            //Sell side validation
            else if (order.Action == Action.Sell)
            {
                //Make sure we have the stock
               if(!Holdings.Exists(p => p.Symbol == order.Symbol && p.Quantity >= order.Quantity))
                {
                   
                    Logger.Log($"{order.FillDate} - Unable to process Sell order: {order.Symbol} not in Porfolio or qty exceeded.");
                    isValid = false;
                }
            }

            //Cancel order
            if(isValid == false)
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

                //Pick up MOO orders placed yesterday
                if (order.OrderType == OrderType.MOO &&
                    order.Status == OrderStatus.Pending &&
                    tradeBar.Day.AddDays(-1) == order.DateSubmitted)
                {
                    fillOrder(tradeBar, order);

                    PendingOrderQueue.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// Fills the order.
        /// </summary>
        private void fillOrder(TradeBar tradeBar, Order order)
        {
            //Process MOC & Markets orders
            if (order.OrderType == OrderType.MOC || order.OrderType == OrderType.Market)
            {
                order.FillPrice = tradeBar.Close;
            }
            //Process MOO Orders
            else if (order.OrderType == OrderType.MOO)
            {
                order.FillPrice = tradeBar.Open;
            }

            //VALIDATION REQUIRED!!!!
            if (validateOrder(order) == false) return;

            //Update porfolio
            order.OrderType = OrderType.Market;
            order.FillDate = DateTime.Today;
            order.Status = OrderStatus.Filled;

            updateHoldingsForFilledOrder(order);
            Logger.Log($"{order.FillDate} - {order.Action} Order Complete");

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
                Quantity = quantity
            };

            fillOrder(tradeBar, order);
            
        }

    }
}
