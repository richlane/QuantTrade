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
    /// <summary>
    /// Base algo
    /// </summary>
    public class BaseAlgorithm
    {
        #region Events

        public event OnDataHandler OnTradeBarEvent;  //Used by inheriting algorithm
        public event OnOrderHandler OnOrderEvent;  //Used by inheriting algorithm
        public EventArgs e = null;

        #endregion

        #region Properties 

        private TradeBar _currentTradebar;
        private DateTime _startRun;
        private DateTime _endRun;

        public string Comments { get; set; }
        public Broker Broker { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Resolution Resolution { get; set; }
        public String Symbol { get; set; }
        public bool BuyAndHold { get; set; }
        public List<IIndicator> Indicators { get; set; }

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

        private IDataReader _dataReader;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseAlgorithm()
        {
            Indicators = new List<IIndicator>() ;
       
            //Setup our data source
            _dataReader = new CSVReader();
            _dataReader.OnTradeBar += this.OnTradeBar;
        }

       
        /// <summary>
        /// Kicks off the test and reads data from data source
        /// </summary>
        public void RunTest()
        {
            _startRun = DateTime.Now;

            Symbol = Symbol.ToUpper();
            
            //Get our broker and wire up the events
            Broker = new Broker();
            Broker.OnOrder += this.OnOrder;

            //Kick of the reader!
            _dataReader.ReadData(Symbol, Resolution, StartDate, EndDate);

            _endRun = DateTime.Now;

            //Create the results report
            generateSummaryReport();
        }

        /// <summary>
        /// Create summary report
        /// </summary>
        private void generateSummaryReport()
        {
            double totalRunTime = (_endRun - _startRun).Milliseconds;

            decimal profitability = Math.Round( ((Broker.AvailableCash + Broker.PortfolioValue - Broker.StartingCash)/ Broker.StartingCash) *100);
            string endingCash = string.Format("{0:c}", Math.Round(Broker.TotalEquity));
            string startingCash = string.Format("{0:c}", Math.Round(Broker.StartingCash));

            StringBuilder report = new StringBuilder();

            report.AppendLine($"Test: {this.GetType().Name}  Buy & Hold: {BuyAndHold}");
            report.AppendLine($"Test Dates: {StartDate} - {EndDate}");
            report.AppendLine($"Symbol: {Symbol}");
            report.AppendLine($"Starting Account: {startingCash}");
            report.AppendLine($"Ending Account: {endingCash}");
            report.AppendLine($"Net Profit: {profitability}%");
            report.AppendLine($"Annual Return: {Broker.CompoundingAnnualPerformance}%");
            report.AppendLine($"Win Rate: {Broker.WinRate}%");
            report.AppendLine($"Loss Rate: {Broker.LossRate}%");
            report.AppendLine($"Max Drawdown: {Math.Round(Broker.MaxDrawdownPercent)}%");
            report.AppendLine($"Total Fees: ${Broker.TotalTransactionFees}");
            report.AppendLine($"Total Trades: {Broker.TotalTrades}");

            if(!string.IsNullOrEmpty(Comments))
            {
                report.AppendLine($"Comments: {Comments}");
            }

            report.AppendLine("---------------------------------------------------");

            Logger.Log(report.ToString());
            Logger.LogReportResultsToFile(report.ToString());
        }

        /// <summary>
        /// Picks up order events from the Broker
        /// </summary>
        public virtual void OnOrder(Order data, EventArgs e)
        {
            //Propogate the order event to the inheriting algo
            if (OnOrderEvent != null)
            {
                OnOrderEvent(data, e);
            }
           
        }
        
        /// <summary>
         /// Event Handing for new trade bar - this is used by the inheriting algo's and is called from the IDataReader.
         /// </summary> 
        public virtual void OnTradeBar(TradeBar data, EventArgs e)
        {
            //Note do not change the order of events
            _currentTradebar = data;

            //Step 1: Update indicators
            foreach (IIndicator item in Indicators)
            {
                item.UpdateIndicator(data.Close);
            }

            //Step 2: Update queued orders in the broker queue
            Broker.ProcessNewTradebar(data);

            //Step 3: Update the inheriting class OnData events
            if (OnTradeBarEvent != null)
            {
                OnTradeBarEvent(data, e);
            }

        }

        /// <summary>
        /// Place a buy or sell order
        /// </summary>
        public void ExecuteOrder(Core.Action action, OrderType orderType, int quantity)
        {
            //Place Order
            Broker.ExecuteOrder(_currentTradebar, orderType, action, quantity);
        }

        #region Indicator Factories

        public RelativeStrengthIndex CreateRelativeStrengthIndexIndicator(int period, MovingAverageType movingAverageType)
        {
            RelativeStrengthIndex indicator = new RelativeStrengthIndex(period, movingAverageType);
            Indicators.Add(indicator);
            return indicator;
        }

        public ExponentialMovingAverage CreateExponentialIndicator(int period, MovingAverageType movingAverageType)
        {
            ExponentialMovingAverage indicator = new ExponentialMovingAverage(period, movingAverageType);
            Indicators.Add(indicator);
            return indicator;
        }

        public SimpleMovingAverage CreateSimpleMovingAverageIndicator(int period)
        {
            SimpleMovingAverage indicator = new SimpleMovingAverage(period);
            Indicators.Add(indicator);
            return indicator;
        }
        #endregion

    }
}
