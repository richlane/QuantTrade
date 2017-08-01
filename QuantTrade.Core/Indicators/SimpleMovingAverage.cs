using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;
using System.ComponentModel;
using System.Diagnostics;
using QuantTrade.Core.Securities;

namespace QuantTrade.Core.Indicators
{
    /// <summary>
    /// SimpleMovingAverage
    /// </summary>
    public class SimpleMovingAverage : BaseIndicator, IIndicator
    {
        #region Properties

        private List<decimal> _rollingSum;

        public override bool IsReady
        {
            get
            {
                return _rollingSum.Count >= Period;
            }
        }

        #endregion


        /// <summary>
        /// Constructor - Creating standard SMA
        /// </summary>
        public SimpleMovingAverage(int period)
        {
            Period = period;
            _rollingSum = new List<decimal>();
        }


        // <summary>
        /// Gets called from other indicators
        /// </summary>
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

            //Make sure the last closing price is at the top
            _rollingSum.Insert(0, price);

            //Trim the buffer by remove the oldest price from the running numbers
            if (_rollingSum.Count > Period)
            {
                _rollingSum.RemoveAt(_rollingSum.Count - 1);
            }

            Value = _rollingSum.Average();
        }
    }
}
