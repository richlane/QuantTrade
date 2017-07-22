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

        public Decimal ExecutionPrice { get; set; }

        public DateTime ExecutionDate { get; set; }

        /// <summary>
        /// The date the actual order was placed but
        /// </summary>
        public DateTime DateSubmitted { get; private set; }

        public int Quantity { get; set; }
        
        public Action Action { get; set; }

        public string Symbol { get; set; }

        public Guid OrderID { get; private set; }

        public Order()
        {
            OrderID = Guid.NewGuid();
            DateSubmitted = DateTime.Today;
        }

        
    }
}
