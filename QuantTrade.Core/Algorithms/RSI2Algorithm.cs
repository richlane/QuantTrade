/*
* BSD 2-Clause License 
* Copyright (c) 2017, Rich Lane 
* All rights reserved. 
* 
* Redistribution and use in source and binary forms, with or without 
* modification, are permitted provided that the following conditions are met: 
* 
* Redistributions of source code must retain the above copyright notice, this 
* list of conditions and the following disclaimer. 
* 
* Redistributions in binary form must reproduce the above copyright notice, 
* this list of conditions and the following disclaimer in the documentation 
* and/or other materials provided with the distribution. 
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE 
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/

using QuantTrade.Core;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Utilities;
using System;
using System.Text;

namespace QuantTrade.Core.Algorithm
{
    /// <summary>
    /// Jack's RSI2 Strategy
    /// </summary>
    public class RSI2Algorithm : BaseAlgorithm, IAlogorithm
    {
        //Indicators & related settings
        private Resolution _resolution = Resolution.Daily;
        private RelativeStrengthIndex _rsi;
        private SimpleMovingAverage _sma;

        public int _smaLookBackPeriod = 30;
        public int _rsiLookBackPeriod = 2;
        public int _rsiBuyLevel = 30;
        public int _rsiSellLevel = 70;
     
        //Date Ranges
        private int _startYear = 2010;
        private int _endYear = 2017;

        //Sell Stop
        bool _useSellStop = true;
        public decimal _sellStopPercentage = .05m;
        decimal _sellStopPrice;
        decimal _pctToInvest;   //Using the 2%, 3%, 5% investment strategy
      
        //Custom Transaction Logging
        bool _enableTransactionLogging =true;
        StringBuilder _transactionLogBuilder;
        string _logComments = "";
        

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
        public void Initialize(string symbol = "SPY", bool buyAndHold = false, string comments = "")
        {
            Symbol = symbol;
            BuyAndHold = buyAndHold;
            Comments = comments;
            
            //Update base class proprties 
            SetStartDate(_startYear-1, 11, 15); //Set Start Date --> Need 45 days for the warmup period so start in November
            SetEndDate(_endYear, 12, 31);

            Resolution = _resolution;
            subscribeToEvents();

            //Setup Indictors
            _rsi = CreateRelativeStrengthIndexIndicator(_rsiLookBackPeriod, MovingAverageType.Wilders);
            _sma = CreateSimpleMovingAverageIndicator(_smaLookBackPeriod);
         
            //Execute Tests
            RunTest();

            //Custom logging
            if(_enableTransactionLogging)
                Logger.LogTransactionsToFile(_transactionLogBuilder.ToString(), Symbol);
        }


        /// <summary>
        /// Event handler for a newly executed order
        /// </summary>
        public void OnOrderEvent(Order order, EventArgs e)
        {
            //set sell stop price
            if (order.Status == OrderStatus.Filled && _pctToInvest == 1M && _useSellStop)
            {
                _sellStopPrice = 
                    Broker.StockPortfolio.Find(p => p.Symbol == Symbol).AverageFillPrice * (1 - _sellStopPercentage);
            }
        }

        /// <summary>
        /// Event Handing new trade bar
        /// </summary>
        public void OnTradeBarEvent(TradeBar tradebar, EventArgs e)
        {
            //Make sure indicators are ready
            if (!_rsi.IsReady || !_sma.IsReady)
            {
                return;
            }
                
            //Buy, Sell, or Hold?
            Action action = getBuySellHoldDecision(tradebar);

            switch (action)
            {
                case Action.Buy:
                    decimal dollarAmt = (Broker.AvailableCash * _pctToInvest);
                    int buyQty = Convert.ToInt32(Math.Round(dollarAmt / tradebar.Close));

                    //Buying MOC -> Prices are cheaper at MOC. The market tends to gap the next day; buy low/sell high
                    base.ExecuteOrder(Action.Buy,  OrderType.MOO, buyQty);
                    break;

                case Action.Sell:
                    int sellQty = Broker.StockPortfolio.Find(p => p.Symbol == Symbol).Quantity;

                    //Selling MOO -> Lets take advantage of the morning gap up!
                    base.ExecuteOrder(Action.Sell, OrderType.MOO, sellQty);
                    break;
            }

            //Custom logging
            if(_enableTransactionLogging)
                logTransacton(tradebar, action);
        }

        /// <summary>
        /// Should we buy, sell or hold?
        /// </summary>
        private Action getBuySellHoldDecision(TradeBar tradebar)
        {
            Action action = Action.na;
            bool buying = false;
            _logComments = "";

            /////////////////////////////////////////
            //Here is the buy and hold logic
            /////////////////////////////////////////
            if (BuyAndHold)
            {
                if (Broker.IsHoldingStock(Symbol) == false)
                {
                    action = Action.Buy;
                    _pctToInvest = 1M;
                }
                return action;
            }


            /////////////////////////////////////////
            //Swing trade buy Logic
            /////////////////////////////////////////
            if ( _rsi.Value  < _rsiBuyLevel && _pctToInvest < 1M)
            {
                action = Action.Buy;
                buying = true;

                //Calculate how much we want to invest using the 2%, 3%, 5% strategy
                if (_pctToInvest == 0)
                {
                    _pctToInvest = .2M;
                }
                else if (_pctToInvest == .2M)
                {
                    _pctToInvest = .38M;
                }
                else if (_pctToInvest == .38M)
                {
                    _pctToInvest = 1M;
                }

                // _pctToInvest = 1M;


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
            //Swing trade sell and hold logic
            /////////////////////////////////////////
            if (Broker.IsHoldingStock(Symbol) && buying == false)
            {
                //Sell - we hit our oversold RSI and SMA levels
                if ( _rsi.Value > _rsiSellLevel && tradebar.Close > _sma.Value)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    _sellStopPrice = 0;
                }
                //Sell - stopped out
                else if (_sellStopPrice > 0 && tradebar.Close < _sellStopPrice)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    _sellStopPrice = 0;
                    _logComments = "Stopped out";
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
        private void logTransacton(TradeBar data, Action action)
        {
            if (_transactionLogBuilder == null)
            {
                _transactionLogBuilder = new StringBuilder();
                _transactionLogBuilder.AppendLine("Date,Symbol,Action,Open,Close,RSI,SMA,Comments");
            }

            _transactionLogBuilder.AppendLine(string.Format
                ("{0},{1},{2},{3},{4},{5},{6},{7}",
                 data.Day,
                Symbol,
                action.ToString().Replace("na", ""),
                Math.Round(data.Open, 2),
                Math.Round(data.Close, 2),
                Math.Round(_rsi.Value),
                Math.Round(_sma.Value), 
                _logComments
               ));
        }

        #endregion
    }
}
