
using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Utilities;
using QuantTrade.Core.Configuration;
using System;


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
            try
            {
                //Get the default alo to run from the config.json ile
                string algoToRun = $"QuantTrade.Core.Algorithm.{Config.GetToken("default-alogrithm")}";
                Type type = Type.GetType($"{algoToRun}, QuantTrade.Core");
                IAlogorithm algo = Activator.CreateInstance(type) as IAlogorithm;

                algo.Initialize();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                Logger.Log(ex.StackTrace.ToString());
            }
            finally
            {
                Console.WriteLine("Hit any Enter to exit");
                Console.ReadLine();
            }
                
           
        }
    }
}
