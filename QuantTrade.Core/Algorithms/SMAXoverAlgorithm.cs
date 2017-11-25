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
using System.Collections.Generic;

namespace QuantTrade.Core.Algorithm
{
    /// <summary>
    ///  This alog uses an EMA and SMA crossover strategy.
    /// </summary>
    public class SMAXoverAlgorithm : BaseAlgorithm, IAlogorithm
    {      
        //Indicators & related settings
        private Resolution _resolution = Resolution.Daily;
        private SimpleMovingAverage _sma;
        private ExponentialMovingAverage _ema;

        private int _smaLookBackPeriod = 10;
        private int _emaLookBackPeriod = 5;

        private string _emaState = "";  

        //I am going to need to store the moving averages in a collection to be able
        //to detect a crossover. 
        private List<decimal> _emaCollection = new List<decimal>();
        private List<decimal> _smaCollection = new List<decimal>();

        //Sell Stop
        bool _useSellStop = false;
        decimal _sellStopPrice;
        decimal _sellStopPercentage = .05m;  //sell stop at a 5% loss
        

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

            /////////////////////////////////////
            //Update base class proprties 
            /////////////////////////////////////

            //Get dates from config file
            string[] startDate = Config.GetToken("start-date").Split('/');
            string[] endDate = Config.GetToken("end-date").Split('/');

            //Now set date in base class
            SetStartDate(Convert.ToInt32(startDate[2]), Convert.ToInt32(startDate[0]), Convert.ToInt32(startDate[1]));
            SetEndDate(Convert.ToInt32(endDate[2]), Convert.ToInt32(endDate[0]), Convert.ToInt32(endDate[1]));

            Resolution = _resolution;
            subscribeToEvents();

            //Setup Indictors
            _sma = CreateSimpleMovingAverageIndicator(_smaLookBackPeriod);
            _ema = CreateExponentialMovingAverageIndicator(_emaLookBackPeriod, MovingAverageType.Wilders);
        
            //Execute Test
            RunTest();
        }

        /// <summary>
        /// Event handler for a newly executed order
        /// </summary>
        public void OnOrderEvent(Order order, EventArgs e)
        {
            //set sell stop price
            if (order.Status == OrderStatus.Filled  && _useSellStop && order.Action == Action.Buy)
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
            if (!_sma.IsReady || !_ema.IsReady)
            {
                return;
            }

            //Build up the moving average collections
            _emaCollection.Add(_ema.Value);
            _smaCollection.Add(_sma.Value);

            checkEMAState();

            //Buy, Sell, or Hold?
            Action action = getBuySellHoldDecision(tradebar);   

            switch (action)
            {
                case Action.Buy:
                    decimal dollarAmt = Broker.AvailableCash;
                    int buyQty = Convert.ToInt32(Math.Round(dollarAmt / tradebar.Close));

                    //Buying MOO  
                    base.ExecuteOrder(Action.Buy, OrderType.MOO, buyQty);
                    break;

                case Action.Sell:
                    int sellQty = Broker.StockPortfolio.Find(p => p.Symbol == Symbol).Quantity;

                    //Selling MOO  
                    base.ExecuteOrder(Action.Sell, OrderType.MOO, sellQty);
                    break;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void checkEMAState()
        {
            //Get current collection index on the moving averages
            int currentIdx = _smaCollection.Count - 1;

            if (currentIdx == 0) return;

            //watch for the x-over
            if (_emaCollection[currentIdx - 1] < _smaCollection[currentIdx - 1] &&
                _emaCollection[currentIdx] > _smaCollection[currentIdx])
            {
                _emaState = "EMA_CROSSED_ABOVE"; //Buy
            }
            else if (_emaCollection[currentIdx - 1] > _smaCollection[currentIdx - 1] &&
                _emaCollection[currentIdx] < _smaCollection[currentIdx])

            {
                _emaState = "EMA_CROSSED_BELOW"; //Sell
            }
            else if (_emaCollection[currentIdx] < _smaCollection[currentIdx])

            {
                _emaState = "EMA_IS_BELOW"; //Do Nothing
            }
            else
            {
                _emaState = "EMA_IS_ABOVE"; //Do Nothing
            }
        }

        /// <summary>
        /// Should we buy, sell or hold?
        /// </summary>
        private Action getBuySellHoldDecision(TradeBar tradebar)
        {
            Action action = Action.na;
            bool buying = false;
            
            //Here is the buy and hold logic
            if(BuyAndHold)
            {
                if (Broker.IsHoldingStock(Symbol) == false)
                {
                    action = Action.Buy;
                }
                return action;
            }
            

            /////////////////////////////////////////
            //Buy Logic: by when the EMA crosses above the SMA 
            //EMA_CROSSED_ABOVE, EMA_CROSSED_BELOW, EMA_IS_ABOVE, EMA_IS_BELOW
            /////////////////////////////////////////
            if (_emaState == "EMA_CROSSED_ABOVE" && Broker.IsHoldingStock(Symbol) == false)
            {
                action = Action.Buy;
                buying = true;
            }

            /////////////////////////////////////////
            //Sell and Hold Logic: Sell when the stock closes below the EMA
            //EMA_CROSSED_ABOVE, EMA_CROSSED_BELOW, EMA_IS_ABOVE, EMA_IS_BELOW
            /////////////////////////////////////////
            if (Broker.IsHoldingStock(Symbol) && buying == false)
            {
                //Sell - Stock is below the EMA
                if (tradebar.Close < _emaCollection[_emaCollection.Count - 1])
                {
                    action = Action.Sell;
                    _sellStopPrice = 0;
                }
                //Sell - stopped out
                else if (_sellStopPrice > 0 && tradebar.Close < _sellStopPrice)
                {
                    action = Action.Sell;
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

   
    }
}
