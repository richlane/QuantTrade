using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Indicators
{
   public class BaseIndicator
    {
        public virtual bool IsReady { get; set; }

        public int Samples { get; set; }
       
        public string Name
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        public int Period { get; set; }

        //protected List<Decimal> Buffer { get; set; }
        

        //public void Reset()
        //{
        //    Buffer.Clear();
        //}

        public decimal Value { get; set; }


    }
}
