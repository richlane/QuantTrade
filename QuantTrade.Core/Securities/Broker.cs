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



        #region Properties

        // public List<Order> OrderHistory { get; private set; }

        private bool _allowMargin;

        private decimal _totalWins;

        private decimal _totalLosses;

        private int _totalSellTrades;

        public double CompoundingAnnualPerformance
        {
            get
            {
                //Number of years in this dataset:
                double years = (EquityOverTime.Keys.LastOrDefault() - EquityOverTime.Keys.FirstOrDefault()).TotalDays / 365;

                return Math.Round((Math.Pow((double)TotalEquity / (double)StartingCash, (1 / (double)years)) - 1) * 100,2);
            }

        }

        public List<Order> PendingOrderQueue { get; private set; }

        public SortedDictionary<DateTime, decimal> EquityOverTime { get; private set; }
        
        
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

                decimal rate = _totalLosses / _totalSellTrades;
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

        public Decimal TotalEquity
        {
            get
            {
               return  PortfolioValue + AvailableCash;
          
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

                return Math.Round(Math.Abs(drawdowns.Min() * 100), 0);
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
            _allowMargin = Convert.ToBoolean(Configuration.Config.GetToken("allow-margin"));
            TransactionFee = Convert.ToDecimal(Configuration.Config.GetToken("transaction-fee"));
            StartingCash = Convert.ToDecimal(Configuration.Config.GetToken("starting-cash"));
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
            //OrderHistory = new List<Order>();
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
                if (AvailableCash < (order.Quantity * order.FillPrice))
                {
                    if (!_allowMargin)
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
                //OrderHistory.Add(order);
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

                //I removed the date check becuase I was not taking into consideration weekends
                // tradeBar.Day.AddDays(-1).ToShortDateString() == order.DateSubmitted.ToShortDateString())

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
