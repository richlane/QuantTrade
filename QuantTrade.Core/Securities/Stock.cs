using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Securities
{
    public class Stock
    {
        public int Quantity { get; set; } 

        public string Symbol { get; set; }

        public decimal AverageFillPrice
        {
            get
            {
                return TotalInvested/Quantity;
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
