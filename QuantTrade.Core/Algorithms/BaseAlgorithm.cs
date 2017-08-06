using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Data;
using QuantTrade.Core.Utilities;
using QuantTrade.Core.Reports;

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
     
        public string Comments { get; set; }
        public Broker Broker { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public Resolution Resolution { get; set; }
        public String Symbol { get; set; }
        public bool BuyAndHold { get; set; }
        public List<IIndicator> Indicators { get; private set; }
        public SummaryReport SummaryReport { get; private set; }
        public decimal TotalRunTime { get; private set; }



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
            string dataSourceString = $"QuantTrade.Core.Data.{Config.GetToken("data-source")}";
            Type dataSourceType = Type.GetType($"{dataSourceString}, QuantTrade.Core");
            IDataSource dataSource = Activator.CreateInstance(dataSourceType) as IDataSource;
            
            _dataReader = new CSVReader(dataSource);
            _dataReader.OnTradeBar += this.OnTradeBar;
        }

       
        /// <summary>
        /// Kicks off the test and reads data from data source
        /// </summary>
        public void RunTest()
        {
            Symbol = Symbol.ToUpper();
            
            //Get our broker and wire up the events
            Broker = new Broker();
            Broker.OnOrder += this.OnOrder;

            //Start Timer
            DateTime startRun = DateTime.Now;

            //Kick of the reader!
            _dataReader.ReadData(Symbol, Resolution, StartDate, EndDate);
         
            //Create the results report
            generateSummaryReport();

            //End Timer
            DateTime endRun = DateTime.Now;
            TotalRunTime = (endRun - startRun).Milliseconds;
        }

        /// <summary>
        /// Create summary report
        /// </summary>
        private void generateSummaryReport()
        {
            SummaryReport = new SummaryReport()
            {
                AlgorithmName = this.GetType().Name,
                RunDates =  this.StartDate.ToShortDateString() + " - " + this.EndDate.ToShortDateString(),
                Symbol = this.Symbol,
                BuyAndHold = BuyAndHold,
                EndingAccount =  Broker.TotalEquity,
                StartingAccount = Broker.StartingCash,
                NetProfit = Broker.NetProfit,
                AnnualReturn = Broker.CompoundingAnnualPerformance,
                WinRate = Broker.WinRate,
                LossRate = Broker.LossRate,
                MaxDrawDown = Broker.MaxDrawdownPercent,
                TotalFees = Broker.TotalTransactionFees,
                TotalTrades = Broker.TotalTrades
            };

            //Log summary report
            Logger.LogSummaryReport(SummaryReport);
            
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
