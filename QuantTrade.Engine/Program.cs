
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
               
                //Get the default algo to run from the config.json file
                string algoToRun = $"QuantTrade.Core.Algorithm.{Config.GetToken("default-alogrithm")}";
                Type type = Type.GetType($"{algoToRun}, QuantTrade.Core");

                //Run buy and hold on the spy!
                IAlogorithm algo = Activator.CreateInstance(type) as IAlogorithm;
                algo.Initialize("SPY", true, "Buy and hold");

                //Now Read stock list from config.json & run throught the algo
                string[] symbols = Config.GetToken("symbols").Split(' ');

                foreach (var symbol in symbols)
                {
                    algo = Activator.CreateInstance(type) as IAlogorithm;
                    algo.Initialize(symbol, false);
                }
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
