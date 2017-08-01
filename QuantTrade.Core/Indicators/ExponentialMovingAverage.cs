using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;
using QuantTrade.Core.Securities;
using System.ComponentModel;

namespace QuantTrade.Core.Indicators
{
    /// <summary>
    /// ExponentialMovingAverage
    /// </summary>
    public class ExponentialMovingAverage : BaseIndicator, IIndicator 
    {
        #region Properties

        private decimal _smoothingFactor;
        private decimal _smaTotal;
        private decimal _previousEMAValue;

        public override bool IsReady
        {
            get
            {
                return Samples >= Period;
            }
        }

        #endregion

        
        /// <summary>
        /// Constructor - can define the smoothing factor
        /// </summary>
        public ExponentialMovingAverage(int period, decimal smoothingFactor)
        {
            Period = period;
            _smoothingFactor = smoothingFactor;
        }

        /// <summary>
        /// Constructor - can define the moving average typr factor
        /// </summary>
        public ExponentialMovingAverage(int period,  MovingAverageType movingAverageType)
        {
            Period = period;

            //Wilders EMA
            if (movingAverageType ==   MovingAverageType.Wilders)
            {
                _smoothingFactor = 1m / Period;

            }
            //Creating a Standard EMA
            else
            {
                _smoothingFactor= 2m /((decimal)period + 1.0m);
            }
            
        }

        // <summary>
        /// Gets called from other indicators
        /// </summary>
        /// <param name="data"></param>
        public void UpdateIndicator(decimal price)
        {
            calculateValue(price);
        }

        /// <summary>
        /// Gets called from the CSVReader
        /// </summary>
        public void UpdateIndicator(TradeBar data)
        {
            calculateValue(data.Close);
        }

        /// <summary>
        /// 
        /// </summary>
        private void calculateValue(decimal price)
        {
            Samples++;

            // Save values for SMA calculation
            if (Samples < Period)
            {
                _smaTotal += price;
            }
            //Calc the SMA 
            else if (Samples == Period)
            {
                _smaTotal += price;
                Value = _smaTotal / Period;
                _previousEMAValue = Value;
            }
            //Calc EMA
            else
            {
                Value = price * _smoothingFactor + _previousEMAValue * (1 - _smoothingFactor);
                _previousEMAValue = Value;
            }

       
        }
    }
}
