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
    ///  Buy when close is below SMA(20) and sell when close is above SMA(20)
    /// </summary>
    public class SMA20Algorithm : BaseAlgorithm, IAlogorithm
    {      
        //Indicators & related settings
        private Resolution _resolution = Resolution.Daily;
        private SimpleMovingAverage _sma;

        private int _smaLookBackPeriod = 20;
     
        //Date Ranges
        private int _startYear = 2011;
        private int _endYear = 2017;

        //Sell Stop
        bool _useSellStop = false;
        decimal _sellStopPrice;
        decimal _sellStopPercentage = .05m; 

       
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

            /// Set Start Date --> Need # days > SMA for the warmup period
            DateTime theActualStartDate= calcStartDate();
            SetStartDate(theActualStartDate.Year, theActualStartDate.Month, theActualStartDate.Day);
            SetEndDate(_endYear, 12, 31);
            SetStartDate(_startYear-1, 11, 15);  
            SetEndDate(_endYear, 12, 31);

            Resolution = _resolution;
            subscribeToEvents();

            //Setup Indictors
            _sma = CreateSimpleMovingAverageIndicator(_smaLookBackPeriod);
        
            //Execute Test
            RunTest();
        }


        /// <summary>
        /// Set Start Date --> Need # days > SMA for the warmup period
        /// </summary>
        private DateTime calcStartDate()
        {
            int backDays = ((_smaLookBackPeriod / 5) * 2) + _smaLookBackPeriod;
            DateTime tmpStartDate = new DateTime(_startYear, 1, 1);
            tmpStartDate = tmpStartDate.AddDays(backDays * -1);

            return tmpStartDate;
        }

        /// <summary>
        /// Event handler for a newly executed order
        /// </summary>
        public void OnOrderEvent(Order order, EventArgs e)
        {
            //set sell stop price
            if (order.Status == OrderStatus.Filled && _useSellStop)
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
            if (!_sma.IsReady)
            {
                return;
            }
      
            //Buy, Sell, or Hold?
            Action action = getBuySellHoldDecision(tradebar);

            switch (action)
            {
                case Action.Buy:
                    decimal dollarAmt = (Broker.AvailableCash);
                    int buyQty = Convert.ToInt32(Math.Round(dollarAmt / tradebar.Close));

                    base.ExecuteOrder(Action.Buy, OrderType.MOC, buyQty);
                    break;

                case Action.Sell:
                    int sellQty = Broker.StockPortfolio.Find(p => p.Symbol == Symbol).Quantity;

                    base.ExecuteOrder(Action.Sell, OrderType.MOC, sellQty);
                    break;
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
            //Buy Logic
            /////////////////////////////////////////
            if (tradebar.Close < _sma.Value)
            {
                action = Action.Buy;
                buying = true;
            }

            /////////////////////////////////////////
            //Sell and Hold Logic
            /////////////////////////////////////////
            if (Broker.IsHoldingStock(Symbol) && buying == false)
            {
                //Sell - we hit our SMA level
                if ( tradebar.Close > _sma.Value)
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
