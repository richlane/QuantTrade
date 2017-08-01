using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Securities
{
    /// <summary>
    /// Model for stock information as it related to our portfolio.
    /// </summary>
    public class Stock
    {
        public int Quantity { get; set; } 

        public string Symbol { get; set; }

        public decimal AverageFillPrice
        {
            get
            {
                if (TotalInvested == 0 || Quantity == 0)
                {
                    return 0;
                }
                else
                {
                    return TotalInvested / Quantity;
                }
            }
        }

        public decimal TotalInvested { get; set; }

        public decimal CurrentPrice { get; set; }

        public decimal TotalCurrentValue
        {
            get
            {
                return CurrentPrice * Quantity;
            }
        }


    }
}
