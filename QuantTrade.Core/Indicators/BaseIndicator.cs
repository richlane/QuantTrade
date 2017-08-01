using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Indicators
{
    /// <summary>
    /// Base indicator class.
    /// </summary>
    public class BaseIndicator
    {
        #region Properties

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

        public decimal Value { get; set; }

        #endregion
    }
}
