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
    public class ExponentialMovingAverage : BaseIndicator, IIndicator 
    {
        private decimal _smoothingFactor;
        private decimal _smaTotal;
        private decimal _previousValue;

        public override bool IsReady
        {
            get
            {
                return Samples >= Period;
            }
        }


        /// <summary>
        /// Constructor - Creating standard EMA
        /// </summary>
        public ExponentialMovingAverage(int period)
        {
            Period = period;
            _smoothingFactor = 2.0m / ((decimal)period + 1.0m); //default smooting
        }

        /// <summary>
        /// Constructor - Creating a Wilders EMA
        /// </summary>
        public ExponentialMovingAverage(int period, decimal smoothingFactor)
        {
            Period = period;
            _smoothingFactor = smoothingFactor;
        }

        // <summary>
        /// Gets called from other indicators
        /// </summary>
        /// <param name="data"></param>
        public void UpdateIndicator(decimal data)
        {
            calculate(data);
        }

        /// <summary>
        /// Gets called from the CSVReader
        /// </summary>
        public void UpdateIndicator(TradeBar data)
        {
            calculate(data.Close);
        }

        private void calculate(decimal input)
        {
            Samples++;

            // Save values for SMA calculation
            if (Samples < Period)
            {
                _smaTotal += input;
            }
            //Calc the SMA 
            else if (Samples == Period)
            {
                _smaTotal += input;
                Value = _smaTotal / Period;
                _previousValue = Value;
            }
            //Calc EMA
            else
            {
                Value = input * _smoothingFactor + _previousValue * (1 - _smoothingFactor);
                _previousValue = Value;
            }

       
        }
    }
}
