using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Algorithm
{
    public interface IAlogorithm
    {
        void Initialize(string symbol, bool buyAndHold);
      
    }
}
