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

using QuantTrade.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuantTrade.UI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConfigureForm : Form
    {

        /// <summary>
        /// 
        /// </summary>
        public ConfigureForm()
        {
            InitializeComponent();
            readSettings();
        }
        

        /// <summary>
        /// Close Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        /// <summary>
        /// Save Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, EventArgs e)
        {
            saveSettings();
            this.Close();
        }

        /// <summary>
        /// Write settings to JSON config file
        /// </summary>
        private void saveSettings()
        {
            Dictionary<string, string> tokens = new Dictionary<string, string>();

            tokens.Add("starting-cash", StartingCash.Text);

            tokens.Add("allow-margin", AllowMargin.Checked.ToString());
            tokens.Add("transaction-fee", TransactionFee.Text);
            tokens.Add("buyandhold-stocks", BuyAndHoldStocks.Text.ToUpper());
            tokens.Add("swingtrade-stocks", SwingTradeStocks.Text.ToUpper());
            Config.SaveTokens(tokens);
        }


        // <summary>
        /// Read settings from JSON config file
        /// </summary>
        private void readSettings()
        {
            StartingCash.Text = Config.GetToken("starting-cash");
            TransactionFee.Text = Config.GetToken("transaction-fee");
            BuyAndHoldStocks.Text = Config.GetToken("buyandhold-stocks");
            SwingTradeStocks.Text = Config.GetToken("swingtrade-stocks");
            AllowMargin.Checked= Convert.ToBoolean( Config.GetToken("allow-margin"));

        }
    }
}
