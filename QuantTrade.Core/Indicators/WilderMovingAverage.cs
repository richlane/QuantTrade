using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;

using System.ComponentModel;

namespace QuantTrade.Core.Indicators
{
    public class WilderMovingAverage : BaseIndicator, IIndicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WilderMovingAverage(int length)
        {
            this.Buffer = new List<decimal>();
            Length = length;
        }

        /// <summary>
        /// Gets called from the CSVReader
        /// </summary>
        public void UpdateIndicator(TradeBar data)
        {
            decimal newValue = Math.Round(data.Close,2);
            Buffer.Add(newValue);

            if (Buffer.Count > Length)
                Buffer.RemoveAt(0);

            Value = (Value * (Buffer.Count - 1) + newValue) / Buffer.Count;
            Value = Math.Round(Value, 2);
        }
    }
}
