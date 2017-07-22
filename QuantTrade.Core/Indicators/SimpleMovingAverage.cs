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
    public class SimpleMovingAverage : BaseIndicator, IIndicator
    {
        private List<decimal> _rollingSum;

        public override bool IsReady
        {
            get
            {
                return _rollingSum.Count >= Period;
            }
        }


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

            //Make sure the last closing price is at the top
            _rollingSum.Insert(0, input);

            //Trim the buffer by remove the oldest price from the running numbers
            if (_rollingSum.Count > Period)
            {
                _rollingSum.RemoveAt(_rollingSum.Count - 1);
            }

            Value = _rollingSum.Average();
            

           
        }
    }
}
