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

        public decimal StartingCash { get; private set; }

        public Decimal TransactionFee { get; private set; }

        public Decimal TransactionErrors { get; private set; }

        public Decimal AvailableCash { get; private set; }

        public Decimal TotalTransactions { get; private set; }

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
        
        public List<Stock> StockPortfolio { get; private set; }
        
        #endregion


        /// <summary>
        /// 
        /// </summary>
        public Broker(decimal startingCash, decimal transactionFee)
        {
            StartingCash = startingCash;
            AvailableCash = startingCash;
            TransactionFee = transactionFee;

            StockPortfolio  = new List<Stock>();

            OrderHistory = new List<Order>();
            PendingOrderQueue = new List<Order>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradeBar"></param>
        public void UpdatePortfolioValue(TradeBar tradeBar)
        {


        }
        
        /// <summary>
        /// 
        /// </summary>
        private void updatePortfolioTransaction(Order order)
        {
            if (order.Status != OrderStatus.Complete) return;
                            
            OrderHistory.Add(order);
            TotalTransactions++;

            //Transaction fees
            TotalTransactionFees += TransactionFee;
            AvailableCash -= TransactionFee;

            Stock stock = new Stock()
            {
                Symbol = order.Symbol,
                Quantity = order.Quantity
            };


            ///////////////////////////////
            //Update buy transaction
            //////////////////////////////
            if (order.Action == Action.Buy)
            {
                AvailableCash -= (order.Quantity * order.ExecutionPrice);

                if(StockPortfolio.Exists(p => p.Symbol == stock.Symbol))
                {
                    StockPortfolio.Find(p => p.Symbol == stock.Symbol).Quantity += stock.Quantity;
                }
                else
                {
                    StockPortfolio.Add(stock);
                }
            }
            ///////////////////////////////
            //Update sell transaction
            //////////////////////////////
            else if (order.Action == Action.Sell)
            {
                AvailableCash += (order.Quantity * order.ExecutionPrice);

                if (StockPortfolio.Exists(p => p.Symbol == stock.Symbol))
                {
                    Stock existingStock  = StockPortfolio.Find(p => p.Symbol == stock.Symbol);
                    existingStock.Quantity -= order.Quantity;

                    if (existingStock.Quantity <=0 )
                    {
                        StockPortfolio.Remove(existingStock);
                    }
                }
           
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool validateOrder(Order order)
        {
            bool isValid = true;

            //Buy side validation
            if (order.Action == Action.Buy)
            {
                if (AvailableCash < (order.Quantity * order.ExecutionPrice))
                {
                    Logger.Log($"{order.ExecutionDate} - Unable to process Buy order: Insufficient funds.");
                    isValid = false;
                }
            }
            //Sell side validation
            else if (order.Action == Action.Sell)
            {
                //Make sure we have the stock
               if(!StockPortfolio.Exists(p => p.Symbol == order.Symbol && p.Quantity >= order.Quantity))
                {
                   
                    Logger.Log($"{order.ExecutionDate} - Unable to process Sell order: {order.Symbol} not in Porfolio or qty exceeded.");
                    isValid = false;
                }
            }

            //Cancel order
            if(isValid == false)
            {
                order.Status = OrderStatus.Cancelled;
                order.ExecutionPrice = 0;
                order.Quantity = 0;
                OrderHistory.Add(order);
                TransactionErrors++;
            }

            return isValid;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessWaitingOrders(TradeBar tradeBar)
        {
            for (int i = 0; i < PendingOrderQueue.Count; i++)
            {
                Order order = PendingOrderQueue[i];

                //Pick up MOO orders placed yesterday
                if (order.OrderType == OrderType.MOO &&
                    order.Status == OrderStatus.Pending &&
                    tradeBar.Day.AddDays(-1) == order.DateSubmitted)
                {
                    executeOrder(tradeBar, order);

                    PendingOrderQueue.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// Called by
        /// </summary>
        /// <param name="tradeBar"></param>
        /// <param name="order"></param>
        private void executeOrder(TradeBar tradeBar, Order order)
        {
            //Process MOC & Markets orders
            if (order.OrderType == OrderType.MOC || order.OrderType == OrderType.Market)
            {
                order.ExecutionPrice = tradeBar.Close;
            }
            //Process MOO Orders
            else if (order.OrderType == OrderType.MOO)
            {
                order.ExecutionPrice = tradeBar.Open;
            }

            //VALIDATION REQUIRED!!!!
            if (validateOrder(order) == false) return;

            //Update porfolio
            order.OrderType = OrderType.Market;
            order.ExecutionDate = DateTime.Today;
            order.Status = OrderStatus.Complete;

            updatePortfolioTransaction(order);
            Logger.Log($"{order.ExecutionDate} - {order.Action} Order Complete");

            //Throw event
            if (OnOrder != null)
            {
                OnOrder(order, e);
            }
        }


        /// <summary>
        /// Called by the base alog
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

            executeOrder(tradeBar, order);
            
        }

    }
}
