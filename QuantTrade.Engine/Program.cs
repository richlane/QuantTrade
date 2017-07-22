
using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuantTrade
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //TransactionManager tranasctionManager = new TransactionManager();
            //tranasctionManager.RunAlogrithm(new SimpleAlogrithm());

            IAlogorithm algo = new SimpleAlogrithm();
            algo.Initialize();

            Console.WriteLine("Hit any Enter to exit");
            Console.ReadLine();
        }
    }
}
