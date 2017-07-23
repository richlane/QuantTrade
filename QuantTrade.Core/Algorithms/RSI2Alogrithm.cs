using QuantTrade.Core;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Algorithm
{
    public class RSI2Alogrithm : BaseAlgorithm, IAlogorithm
    {
        private string _symbol = "SPXL";
        bool _buyAndHold = false;

        int _smaLookBackPeriod = 30;
        int _rsiLookBackPeriod = 2;
        int _rsiBuyLevel = 30;
        int _rsiSellLevel = 70;
        OrderType _orderType = OrderType.MOO;

        //Date Ranges
        private int _startYear = 2009;
        private int _endYear = 2009;

        //Sell Stop
        bool _useSellStop = false;
        decimal _sellStopMultiplier = .05M;

        //Account Settings
        private decimal _transactionFee = 7M;
        private decimal _availableCash = 10000M;

        //Indicators
        private Resolution _resolution = Resolution.Daily;
        private RelativeStrengthIndex _rsi;
        private SimpleMovingAverage _sma;

        #region Misc

        bool _isLastTradingDay;
        bool _holdingStock;
        decimal _sellStopPrice;
        decimal _pctToInvest;
        bool _firstRun;
        string _comment;
        #endregion

        /// <summary>
        /// Subscribe to base class events.
        /// </summary>
        private void subscribeToEvents()
        {
            //recieving updated trade data
            base.OnTradeBarEvent += this.OnTradeBarEvent;
            base.OnOrderEvent += this.OnOrderEvent;
        }
        
        
        /// <summary>
        /// Launch Algo.
        /// </summary>
        public void Initialize()
        {
            _firstRun = true;

            //Update base class proprties 
            SetStartDate(_startYear-1, 12, 2); //Set Start Date --> Need 45 days for the warmup period so start in November
            SetEndDate(_endYear, 3, 10);

            StartingCash = _availableCash;
            TransactionFee = _transactionFee;
            Resolution = _resolution;
            Symbol = _symbol;
            subscribeToEvents();

            //Setup Indictors
            _rsi = GenerateRelativeStrengthIndexIndicator(_rsiLookBackPeriod, MovingAverageType.Wilders);
            _sma = GenerateSimpleMovingAverageIndicator(_smaLookBackPeriod);
        
            //Execute Tests
            RunTest();
        }


        /// <summary>
        /// Event handler for a newly executed order
        /// </summary>
        public void OnOrderEvent(Order data, EventArgs e)
        {
            //if(data.Status== OrderStatus.Filled)
            //{
            //    _holdingStock = true;
            //}
            //else
            //{
            //    _holdingStock = false;
            //}

            //set sell stop price
            if (data.Status == OrderStatus.Filled && _pctToInvest == 1M && _useSellStop)
            {
                _sellStopPrice = Broker.Holdings.Find(p => p.Symbol == _symbol).AverageFillPrice * (1 - _sellStopMultiplier);
            }
        }

        /// <summary>
        /// Event Handing new trade bar
        /// </summary>
        public void OnTradeBarEvent(TradeBar data, EventArgs e)
        {
            //Make sure indicators are ready
            if (!_rsi.IsReady || !_sma.IsReady)
            {
                return;
            }
                
            //Watch for last trading day so we can liquidate portfolio
            if (data.Day >= EndDate.AddDays(-5).Date)
            {
                _isLastTradingDay = true;
            }
              

            //Buy, Sell, or Hold?
            Action action = getBuySellHoldDecision(data);

            switch (action)
            {
                case Action.Buy:
                    decimal dollarAmt = (Broker.AvailableCash * _pctToInvest);
                    int buyQty = Convert.ToInt32(Math.Round(dollarAmt / data.Close));
                    base.ExecuteOrder(Action.Buy, _orderType, buyQty);
                    break;

                case Action.Sell:
                    if(Broker.IsHoldingStock(_symbol))
                    {
                        int sellQty = Broker.Holdings.Find(p => p.Symbol == _symbol).Quantity;
                        base.ExecuteOrder(Action.Sell, _orderType, sellQty);
                    }
                    break;
            }

            logTransacton(data, action);
            _firstRun = false;
        }

       

        /// <summary>
        /// Should we buy, sell or hold?
        /// </summary>
        private Action getBuySellHoldDecision(TradeBar data)
        {
            if (_buyAndHold)
                return getLongTermBuyAndHold(data);

            Action action = Action.Hold;
            bool buying = false;
            _comment = "";


            /////////////////////////////////////////
            //Buy Logic
            /////////////////////////////////////////
            if ( _rsi.Value  < _rsiBuyLevel &&
                _isLastTradingDay == false &&
                _pctToInvest < 1M)
            {
                action = Action.Buy;
                //_holdingStock = true;
                buying = true;
      
                //Calculate how much we want to invest using the 2%, 3%, 5% strategy
               
                //if (_pctToInvest == 0)
                //{
                //    _pctToInvest = .2M;
                //}
                //else if (_pctToInvest == .2M)
                //{
                //    _pctToInvest = .38M;
                //}
                //else if (_pctToInvest == .38M)
                //{
                //    _pctToInvest = 1M;
                //}

                _pctToInvest = 1M;


                //if (_pctToInvest == 0)
                //{
                //    _pctToInvest = .5M;
                //}
                //else if (_pctToInvest == .5M)
                //{
                //    _pctToInvest = 1M;
                //}
            }

            /////////////////////////////////////////
            //Sell and Hold Logic
            /////////////////////////////////////////
            if (Broker.IsHoldingStock(_symbol) && buying == false)
            {
                //Sell - we hit our oversold RSI and SMA levels
                if ( _rsi.Value > _rsiSellLevel
                   && data.Close > _sma.Value)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    //_holdingStock = false;
                    _sellStopPrice = 0;
                }
                //Sell - last trading day and we want to close out positons to calc returns
                else if (_isLastTradingDay)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    //_holdingStock = false;
                    _sellStopPrice = 0;
                }
                //Sell - stopped out
                else if (_sellStopPrice > 0 && data.Close < _sellStopPrice)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    //_holdingStock = false;
                    _sellStopPrice = 0;
                    _comment = "stopped out";
                }
                //Hold
                else
                {
                    action = Action.Hold;
                }
            }

            return action;
        }


        /// <summary>
        /// Used for buy and hold only!
        /// </summary>
        private Action getLongTermBuyAndHold(TradeBar data)
        {
            Action action = Action.Hold;
       
            if (_holdingStock == false &&
                _isLastTradingDay == false)
            {
                action = Action.Buy;
                _holdingStock = true;
                _pctToInvest = 1M;
            }
            else if (_holdingStock == true &&
                 _isLastTradingDay)
            {
                action = Action.Sell;
                _holdingStock = false;
                _pctToInvest = 0M;
            }
            return action;
        }

        /// <summary>
        /// Logs the transaction into a csv for review
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        private void logTransacton(TradeBar data, Action action)
        {
            string logData = "";
            string status = action.ToString();


            if (_holdingStock == false && action== Action.Hold)
            {
                status = "";
            }

                if (_firstRun)
                {
                    logData = "Date,Symbol,Action,Open,Close,RSI,SMA,Comment";
                    Logger.LogTransaction(logData);
                }

            logData = string.Format
                ("{0},{1},{2},{3},{4},{5},{6},{7}",
                data.Day,
                _symbol,
                status,
                Math.Round(data.Open, 2),
                Math.Round(data.Close, 2),
                Math.Round(_rsi.Value),
                Math.Round(_sma.Value),
                _comment);

            Logger.LogTransaction(logData);
        }
    }
}
