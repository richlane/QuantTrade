/*
* BSD 2-Clause License 
* Copyright (c) 2017, Rich Lane 
* All rights reserved. 
* 
* Redistribution and use in source and binary forms, with or without 
* modification, are permitted provided that the following conditions are met: 
* 
* Redistributions of source code must retain the above copyright notice, this 
* list of conditions and the following disclaimer. 
* 
* Redistributions in binary form must reproduce the above copyright notice, 
* this list of conditions and the following disclaimer in the documentation 
* and/or other materials provided with the distribution. 
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE 
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/


using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace QuantTrade
{
    class Program
    {
        private static string _defaultAlgo;
        private static Type _defaultAlgoType;  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Console.WindowHeight = Console.LargestWindowHeight;
                Logger.Log("Starting run..." + Environment.NewLine, ConsoleColor.Green);

                //Start Timer
                DateTime startRun = DateTime.Now;
                
                //TESTING TESTING TESTING
                //runCustomRSI2Alog("TQQQ");

                //Get the default algo to run from the config.json file
                _defaultAlgo = $"QuantTrade.Core.Algorithm.{Config.GetToken("default-alogrithm")}";
                _defaultAlgoType = Type.GetType($"{_defaultAlgo}, QuantTrade.Core");

                //Run buy and hold stocks - used to benchmark 
                runAlgorithm(Config.GetToken("buyandhold-stocks"), true);

                //Run swing trade stocks
                runAlgorithm(Config.GetToken("swingtrade-stocks"), false);

                //End Timer
                DateTime endRun = DateTime.Now;
                decimal totalRunTime = (endRun - startRun).Milliseconds;
                Logger.Log($"Total Run Time: {totalRunTime.ToString()} ms", ConsoleColor.Yellow);

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                Logger.Log(ex.StackTrace.ToString());
            }
            finally
            {
                Logger.Log("Run complete. Hit any Enter to exit... ", ConsoleColor.Green);
                Console.ReadLine();
            }
           
        }

        /// <summary>
        /// Loops the stocks and run the default alogorithm
        /// </summary>
        private static void runAlgorithm(string symbols, bool buyAndHold)
        {
            if (string.IsNullOrEmpty(symbols)) return;
            
            //Loops the stocks and run the alogo
            string[] symbolsCollection = symbols.Split(' ');

            //Parallel method for processing
            Parallel.ForEach(symbolsCollection, symbol =>
            {
                IAlogorithm algo = Activator.CreateInstance(_defaultAlgoType) as IAlogorithm;
                algo.Initialize(symbol, buyAndHold);

                // Console.WriteLine("{0}, Thread Id= {1}", symbol, Thread.CurrentThread.ManagedThreadId);
            });

        }


        /// <summary>
        /// Here I am trying to have to software set a slew of parameters and tell me what generates the best results.
        /// </summary>
        private static void runCustomRSI2Alogo(string symbol)
        {
            for (int i = 1; i < 100; i++)
            {
                RSI2Algorithm algo = new RSI2Algorithm();
                algo._smaLookBackPeriod = i;
            
                algo.Initialize(symbol, false, "i = " + i);
            }
        }
    }
}
