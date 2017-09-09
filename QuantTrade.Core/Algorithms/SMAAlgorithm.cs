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
    ///  Buy when close is below SMA and sell when close is above SMA
    /// </summary>
    public class SMAAlgorithm : BaseAlgorithm, IAlogorithm
    {      
        //Indicators & related settings
        private Resolution _resolution = Resolution.Daily;
        private SimpleMovingAverage _sma;

        private int _smaLookBackPeriod = 10;
     
        //Sell Stop
        bool _useSellStop = true;
        decimal _sellStopPrice;
        decimal _sellStopPercentage = .05m;
        decimal _pctToInvest;   //Using the 2%, 3%, 5% investment strategy


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
        
            //Execute Test
            RunTest();
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
            if (!_sma.IsReady)
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
                    _pctToInvest = 1M;
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
            //Sell and Hold Logic
            /////////////////////////////////////////
            if (Broker.IsHoldingStock(Symbol) && buying == false)
            {
                //Sell - we hit our SMA level
                if ( tradebar.Close > _sma.Value)
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
