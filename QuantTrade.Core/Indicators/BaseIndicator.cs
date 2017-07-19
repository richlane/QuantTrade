using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Indicators
{
   public class BaseIndicator
    {

        public string Name
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        protected List<Decimal> Buffer { get; set; }
        
        public int Length { get; set; }

        public void Reset()
        {
            Buffer.Clear();
        }

        public bool IsReady
        {
            get
            {
                return (Buffer.Count >= Length);
            }
        }

        public decimal Value { get; set; }


    }
}
