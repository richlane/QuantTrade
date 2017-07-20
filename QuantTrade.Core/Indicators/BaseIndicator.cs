using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Indicators
{
   public class BaseIndicator
    {
        public bool IsReady { get; set; }

        public int Samples { get; set; }
       
        public string Name
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        protected List<Decimal> Buffer { get; set; }
        
        public int Period { get; set; }

        public void Reset()
        {
            Buffer.Clear();
        }
     
        public decimal Value { get; set; }


    }
}
