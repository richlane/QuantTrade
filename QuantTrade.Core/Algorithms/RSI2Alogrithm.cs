using QuantTrade.Core;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Utilities;
using System;


namespace QuantTrade.Core.Algorithm
{
    public class RSI2Alogrithm : BaseAlgorithm, IAlogorithm
    {
        //Indicators & related settings
        private Resolution _resolution = Resolution.Daily;
        private RelativeStrengthIndex _rsi;
        private SimpleMovingAverage _sma;

        int _smaLookBackPeriod = 25;
        int _rsiLookBackPeriod = 2;
        int _rsiBuyLevel = 30;
        int _rsiSellLevel = 70;
        OrderType _orderType = OrderType.MOO;

        //Date Ranges
        private int _startYear = 2010;
        private int _endYear = 2016;

        //Sell Stop
        bool _useSellStop = true;
        decimal _sellStopPercentage = .3m;

        //Account Settings
        private decimal _transactionFee = 7M;
        private decimal _startingCash = 10000M;

      

        #region Misc

        decimal _sellStopPrice;
        decimal _pctToInvest;
        bool _buyAndHold = false;

        //bool _firstRun=true;

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
        public void Initialize(string symbol = "SPY", bool buyAndHold = false)
        {
            Symbol = symbol;
            _buyAndHold = buyAndHold;

            //Update base class proprties 
            SetStartDate(_startYear-1, 11, 15); //Set Start Date --> Need 45 days for the warmup period so start in November
            SetEndDate(_endYear, 12, 31);

            StartingCash = _startingCash;
            TransactionFee = _transactionFee;
            Resolution = _resolution;
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
            //set sell stop price
            if (data.Status == OrderStatus.Filled && _pctToInvest == 1M && _useSellStop)
            {
                _sellStopPrice = 
                    Broker.StockPortfolio.Find(p => p.Symbol == Symbol).AverageFillPrice * (1 - _sellStopPercentage);
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
                    if(Broker.IsHoldingStock(Symbol)) //make sure we are hoding the stock before selling
                    {
                        int sellQty = Broker.StockPortfolio.Find(p => p.Symbol == Symbol).Quantity;
                        base.ExecuteOrder(Action.Sell, _orderType, sellQty);
                    }
                    break;
            }

            //logTransacton(data, action);
            //_firstRun = false;
        }

        /// <summary>
        /// Should we buy, sell or hold?
        /// </summary>
        private Action getBuySellHoldDecision(TradeBar data)
        {
            Action action = Action.Hold;
            bool buying = false;
     
            //Here is the buy and hold logic
            if (_buyAndHold)
            {
                if (Broker.IsHoldingStock(Symbol) == false)
                {
                    action = Action.Buy;
                    _pctToInvest = 1M;
                }
                return action;
            }
           

            /////////////////////////////////////////
            //Buy Logic
            /////////////////////////////////////////
            if ( _rsi.Value  < _rsiBuyLevel && _pctToInvest < 1M)
            {
                action = Action.Buy;
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
            if (Broker.IsHoldingStock(Symbol) && buying == false)
            {
                //Sell - we hit our oversold RSI and SMA levels
                if ( _rsi.Value > _rsiSellLevel
                   && data.Close > _sma.Value)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    _sellStopPrice = 0;
                }
                //Sell - stopped out
                else if (_sellStopPrice > 0 && data.Close < _sellStopPrice)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    _sellStopPrice = 0;
                }
                //Hold
                else
                {
                    action = Action.Hold;
                }
            }

            return action;
        }

        #region used for verbose logging

        /// <summary>
        /// Logs the transaction into a csv for review
        /// </summary>
        //private void logTransacton(TradeBar data, Action action)
        //{
        //    return;

        //    string logData = "";
        //    string status = action.ToString();


        //    if (Broker.IsHoldingStock(Symbol) == false && action== Action.Hold)
        //    {
        //        status = "";
        //    }

        //    if (_firstRun)
        //    {
        //        logData = "Date,Symbol,Action,Open,Close,RSI,SMA,Comment";
        //        Logger.LogTransaction(logData);
        //    }

        //    logData = string.Format
        //        ("{0},{1},{2},{3},{4},{5},{6},{7}",
        //        data.Day,
        //        Symbol,
        //        status,
        //        Math.Round(data.Open, 2),
        //        Math.Round(data.Close, 2),
        //        Math.Round(_rsi.Value),
        //        Math.Round(_sma.Value),
        //        _comment);

        //    Logger.LogTransaction(logData);
        //}

        #endregion
    }
}
