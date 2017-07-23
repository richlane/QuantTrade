using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Securities
{
    public class Order
    {
        public OrderStatus Status { get; set; }
        
        public OrderType OrderType { get; set; }

        public Decimal FillPrice { get; set; }

        public DateTime FillDate { get; set; }

        /// <summary>
        /// The date the actual order was placed but
        /// </summary>
        public DateTime DateSubmitted { get;  set; }

        public int Quantity { get; set; }
        
        public Action Action { get; set; }

        public string Symbol { get; set; }

        public Guid OrderID { get; private set; }

        public Order()
        {
            OrderID = Guid.NewGuid();
            Status = OrderStatus.New;
        }

        
    }
}
