using QuantTrade.Core;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Utilities;
using System;
using System.Collections.Generic;

namespace QuantTrade.Core.Algorithm
{
    /// <summary>
    ///  Buy when close is below SMA(50) and sell when close is above SMA(50)
    /// </summary>
    public class SMA20Algorithm : BaseAlgorithm, IAlogorithm
    {      
        //Indicators & related settings
        private Resolution _resolution = Resolution.Daily;
        private SimpleMovingAverage _sma;

        private int _smaLookBackPeriod = 10;
        private OrderType _orderType = OrderType.MOC;

        //Date Ranges
        private int _startYear = 2014;
        private int _endYear = 2016;

        //Sell Stop
        bool _useSellStop = false;
        decimal _sellStopPrice;
        decimal _sellStopPercentage = .3m; 

        //Account Settings
        private decimal _transactionFee = 7M;
        private decimal _startingCash = 10000M;

      
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
            SetStartDate(_startYear-1, 11, 15); //Need time to warm up indicators --> allowing 45 days 
            SetEndDate(_endYear, 12, 31);
            Resolution = _resolution;
            subscribeToEvents();

            //Setup Indictors
            _sma = CreateSimpleMovingAverageIndicator(_smaLookBackPeriod);
        
            //Execute Tests
            RunTest();
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

                    base.ExecuteOrder(Action.Buy, _orderType, buyQty);
                    break;

                case Action.Sell:
                    int sellQty = Broker.StockPortfolio.Find(p => p.Symbol == Symbol).Quantity;

                    base.ExecuteOrder(Action.Sell, _orderType, sellQty);
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
            if (BuyAndHold && Broker.IsHoldingStock(Symbol) == false)
            {
               action = Action.Buy;
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
