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

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace QuantTrade.Core.Utilities
{
    /// <summary>
    /// Reads Json.Config settings
    /// </summary>
    public static class Config
    {
        //Location of the configuration file.
        private const string _configurationFileName = "config.json";
        private static JObject _settings;

        /// <summary>
        /// 
        /// </summary>
        static Config()
        {
                
        }


        /// <summary>
        /// Get value from JSON config file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetToken(string key)
        {
            //refresh settings
            _settings = JObject.Parse(File.ReadAllText(_configurationFileName));

            return _settings.SelectToken(key).ToString();

        }


        /// <summary>
        /// Update JSON config file.
        /// </summary>
        /// <param name="tokens"></param>

        public static void SaveTokens(Dictionary<string, string> tokens)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject(File.ReadAllText(_configurationFileName));

            foreach (var item in tokens)
            {
                jsonObj[item.Key] = item.Value;
            }
            
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(_configurationFileName, output);
        }
    }
}
