using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Data;
using QuantTrade.Core.Utilities;

namespace QuantTrade.Core.Algorithm
{
    public class BaseAlgorithm
    {
        public event OnDataHandler OnTradeBarEvent;  //Used by inheriting algorithm
        public event OnOrderHandler OnOrderEvent;  //Used by inheriting algorithm
        public EventArgs e = null;

        #region Properties 

        private TradeBar _currentTradebar;
       
        public Broker Broker { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
            
        public Resolution Resolution { get; set; }

        public String Symbol { get; set; }

        public List<IIndicator> Indicators { get; set; }

        public decimal StartingCash  { get; set; }

        public decimal TransactionFee { get; set; }

        private IDataReader _dataReader;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public BaseAlgorithm()
        {
            Indicators = new List<IIndicator>() ;
       
            _dataReader = new CSVReader();
            _dataReader.OnTradeBar += this.OnTradeBar;
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetStartDate(int year, int month, int day)
        {
            var start = new DateTime(year, month, day);
            start = start.Date;
            StartDate = start;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetEndDate(int year, int month, int day)
        {
            var end = new DateTime(year, month, day);
            end = end.Date;
            EndDate = end;
        }

        /// <summary>
        /// Read Data from data sources
        /// </summary>
        public void RunTest()
        {
            Broker = new Broker(StartingCash, TransactionFee);
            Broker.OnOrder += this.OnOrder;

            Logger.Log("Staring Run...............");
            _dataReader.ReadData(Symbol, Resolution, StartDate, EndDate);

            generateReport();
        }

        /// <summary>
        /// 
        /// </summary>
        private void generateReport()
        {
            decimal profitability = Math.Round( ((Broker.AvailableCash - Broker.StartingCash)/ Broker.StartingCash) *100, 2);
            string endingCash = string.Format("{0:c}", Broker.AvailableCash);
            string startingCash = string.Format("{0:c}", Broker.StartingCash);

            Logger.Log(" ");
            Logger.Log("---------------------------------------------------");
            Logger.Log($"Symbol: {Symbol}");
            Logger.Log($"Dates: {StartDate} - {EndDate}");
            Logger.Log($"Starting Cash: {startingCash}");
            Logger.Log($"Ending Cash: {endingCash}");
            Logger.Log($"Total Profitability: {profitability}%");
            Logger.Log($"Total Fees: ${Broker.TotalTransactionFees}");
            Logger.Log($"Total Trades: {Broker.TotalTrades}");
            Logger.Log($"Trades Cancelled: {Broker.TotalTradesCancelled} (Insufficient funds)");
            Logger.Log("---------------------------------------------------");
            Logger.Log(" ");
        }

        /// <summary>
        /// Picks up order events from the transaction manager
        /// </summary>
        public virtual void OnOrder(Order data, EventArgs e)
        {
            //update portfolio
            if(data.Status== OrderStatus.Filled)
            {
                if (OnOrderEvent != null)
                {
                    OnOrderEvent(data, e);
                }
            }
        }
        
        /// <summary>
         /// Event Handing for new trade bar - this is used by the inheriting algo's and is called from the IDataReader.
         /// </summary> 
        public virtual void OnTradeBar(TradeBar data, EventArgs e)
        {
            _currentTradebar = data;

            //Update indicators
            foreach (IIndicator item in Indicators)
            {
                item.UpdateIndicator(data.Close);
            }
       
            //Update the inheriting class OnData events
            if (OnTradeBarEvent != null)
            {
                OnTradeBarEvent(data, e);
            }

        }

        public void ExecuteOrder(Core.Action action, OrderType orderType, int quantity)
        {
            
            //Place Order
            Broker.ExecuteOrder(_currentTradebar, orderType, action, quantity);
        }

        #region Indicator Factories

        public RelativeStrengthIndex GenerateRelativeStrengthIndexIndicator(int period, MovingAverageType movingAverageType)
        {
            RelativeStrengthIndex indicator = new RelativeStrengthIndex(period, movingAverageType);
            Indicators.Add(indicator);
            return indicator;
        }
        
        public SimpleMovingAverage GenerateSimpleMovingAverageIndicator(int period)
        {
            SimpleMovingAverage indicator = new SimpleMovingAverage(period);
            Indicators.Add(indicator);
            return indicator;
        }
        #endregion

    }
}
