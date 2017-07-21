
using QuantTrade.Core.Algorithm;
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
            IAlogorithm algo = new SimpleAlogrithm();
            algo.Initialize();

            Console.WriteLine("Hit any Endter to exit");
            Console.ReadLine();
        }
    }
}
