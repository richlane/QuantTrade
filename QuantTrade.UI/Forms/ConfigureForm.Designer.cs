namespace QuantTrade.UI
{
    partial class ConfigureForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BuyAndHoldStocks = new System.Windows.Forms.TextBox();
            this.SwingTradeStocks = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DefaultAlgorithm = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.AlphaAPIKey = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TransactionFee = new System.Windows.Forms.TextBox();
            this.StartingCash = new System.Windows.Forms.TextBox();
            this.AllowMargin = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Save = new System.Windows.Forms.Button();
            this.Close = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.StartDate = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.EndDate = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(213, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Buy && Hold Symbols:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(226, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Swing Trade Symbols:";
            // 
            // BuyAndHoldStocks
            // 
            this.BuyAndHoldStocks.Location = new System.Drawing.Point(237, 52);
            this.BuyAndHoldStocks.Name = "BuyAndHoldStocks";
            this.BuyAndHoldStocks.Size = new System.Drawing.Size(437, 31);
            this.BuyAndHoldStocks.TabIndex = 2;
            // 
            // SwingTradeStocks
            // 
            this.SwingTradeStocks.Location = new System.Drawing.Point(237, 97);
            this.SwingTradeStocks.Name = "SwingTradeStocks";
            this.SwingTradeStocks.Size = new System.Drawing.Size(437, 31);
            this.SwingTradeStocks.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.DefaultAlgorithm);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.BuyAndHoldStocks);
            this.groupBox1.Controls.Add(this.SwingTradeStocks);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(25, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(870, 200);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stocks";
            // 
            // DefaultAlgorithm
            // 
            this.DefaultAlgorithm.Location = new System.Drawing.Point(237, 145);
            this.DefaultAlgorithm.Name = "DefaultAlgorithm";
            this.DefaultAlgorithm.Size = new System.Drawing.Size(437, 31);
            this.DefaultAlgorithm.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 148);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(182, 25);
            this.label9.TabIndex = 7;
            this.label9.Text = "Default Algorithm:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(680, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 25);
            this.label7.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(680, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 25);
            this.label6.TabIndex = 5;
            this.label6.Text = "(space delimited)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(700, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 25);
            this.label3.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AlphaAPIKey);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.TransactionFee);
            this.groupBox2.Controls.Add(this.StartingCash);
            this.groupBox2.Controls.Add(this.AllowMargin);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(25, 416);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(870, 215);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Account Settings";
            // 
            // AlphaAPIKey
            // 
            this.AlphaAPIKey.Location = new System.Drawing.Point(237, 155);
            this.AlphaAPIKey.Name = "AlphaAPIKey";
            this.AlphaAPIKey.Size = new System.Drawing.Size(250, 31);
            this.AlphaAPIKey.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(155, 25);
            this.label8.TabIndex = 11;
            this.label8.Text = "Alpha API Key:";
            // 
            // TransactionFee
            // 
            this.TransactionFee.Location = new System.Drawing.Point(237, 105);
            this.TransactionFee.Name = "TransactionFee";
            this.TransactionFee.Size = new System.Drawing.Size(250, 31);
            this.TransactionFee.TabIndex = 10;
            // 
            // StartingCash
            // 
            this.StartingCash.Location = new System.Drawing.Point(237, 56);
            this.StartingCash.Name = "StartingCash";
            this.StartingCash.Size = new System.Drawing.Size(250, 31);
            this.StartingCash.TabIndex = 5;
            // 
            // AllowMargin
            // 
            this.AllowMargin.AutoSize = true;
            this.AllowMargin.Location = new System.Drawing.Point(505, 58);
            this.AllowMargin.Name = "AllowMargin";
            this.AllowMargin.Size = new System.Drawing.Size(167, 29);
            this.AllowMargin.TabIndex = 9;
            this.AllowMargin.Text = "Allow Margin";
            this.AllowMargin.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 25);
            this.label5.TabIndex = 2;
            this.label5.Text = "Transaction Fee:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 25);
            this.label4.TabIndex = 1;
            this.label4.Text = "Starting Cash:";
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(295, 653);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(145, 63);
            this.Save.TabIndex = 7;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Close
            // 
            this.Close.Location = new System.Drawing.Point(460, 653);
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(145, 63);
            this.Close.TabIndex = 8;
            this.Close.Text = "Close";
            this.Close.UseVisualStyleBackColor = true;
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(682, 103);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(176, 25);
            this.label10.TabIndex = 9;
            this.label10.Text = "(space delimited)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.EndDate);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.StartDate);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(25, 234);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(870, 159);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Date Ranges";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 55);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(118, 25);
            this.label11.TabIndex = 2;
            this.label11.Text = "From Date:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 106);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(94, 25);
            this.label12.TabIndex = 3;
            this.label12.Text = "To Date:";
            // 
            // StartDate
            // 
            this.StartDate.Location = new System.Drawing.Point(237, 49);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(250, 31);
            this.StartDate.TabIndex = 9;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(504, 55);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(140, 25);
            this.label13.TabIndex = 10;
            this.label13.Text = "(mm/dd/yyyy)";
            // 
            // EndDate
            // 
            this.EndDate.Location = new System.Drawing.Point(237, 100);
            this.EndDate.Name = "EndDate";
            this.EndDate.Size = new System.Drawing.Size(250, 31);
            this.EndDate.TabIndex = 11;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(504, 100);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(140, 25);
            this.label14.TabIndex = 12;
            this.label14.Text = "(mm/dd/yyyy)";
            // 
            // ConfigureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 768);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureForm";
            this.Text = "Configure Backtest";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox BuyAndHoldStocks;
        private System.Windows.Forms.TextBox SwingTradeStocks;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Close;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TransactionFee;
        private System.Windows.Forms.TextBox StartingCash;
        private System.Windows.Forms.CheckBox AllowMargin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox AlphaAPIKey;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox DefaultAlgorithm;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox EndDate;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox StartDate;
    }
}