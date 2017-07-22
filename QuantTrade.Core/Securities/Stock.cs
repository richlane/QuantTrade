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

       // public decimal SharePrice { get; set; }


        //public Decimal StockValue
        //{
        //    get
        //    {
        //        return SharePrice * Quantity;
        //    }
        //}
    }
}
