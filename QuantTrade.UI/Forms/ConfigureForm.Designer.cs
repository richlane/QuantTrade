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
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TransactionFee = new System.Windows.Forms.TextBox();
            this.StartingCash = new System.Windows.Forms.TextBox();
            this.AllowMargin = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Save = new System.Windows.Forms.Button();
            this.Close = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Buy && Hold Stocks:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Swing Trade Stocks:";
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
            this.SwingTradeStocks.Location = new System.Drawing.Point(237, 100);
            this.SwingTradeStocks.Name = "SwingTradeStocks";
            this.SwingTradeStocks.Size = new System.Drawing.Size(437, 31);
            this.SwingTradeStocks.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.BuyAndHoldStocks);
            this.groupBox1.Controls.Add(this.SwingTradeStocks);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(25, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(870, 187);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stocks";
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
            this.groupBox2.Controls.Add(this.TransactionFee);
            this.groupBox2.Controls.Add(this.StartingCash);
            this.groupBox2.Controls.Add(this.AllowMargin);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(25, 239);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(870, 197);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Account Settings";
            // 
            // TransactionFee
            // 
            this.TransactionFee.Location = new System.Drawing.Point(237, 107);
            this.TransactionFee.Name = "TransactionFee";
            this.TransactionFee.Size = new System.Drawing.Size(223, 31);
            this.TransactionFee.TabIndex = 10;
            // 
            // StartingCash
            // 
            this.StartingCash.Location = new System.Drawing.Point(237, 58);
            this.StartingCash.Name = "StartingCash";
            this.StartingCash.Size = new System.Drawing.Size(223, 31);
            this.StartingCash.TabIndex = 5;
            // 
            // AllowMargin
            // 
            this.AllowMargin.AutoSize = true;
            this.AllowMargin.Location = new System.Drawing.Point(481, 60);
            this.AllowMargin.Name = "AllowMargin";
            this.AllowMargin.Size = new System.Drawing.Size(167, 29);
            this.AllowMargin.TabIndex = 9;
            this.AllowMargin.Text = "Allow Margin";
            this.AllowMargin.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 25);
            this.label5.TabIndex = 2;
            this.label5.Text = "Transaction Fee:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 25);
            this.label4.TabIndex = 1;
            this.label4.Text = "Starting Cash:";
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(267, 545);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(145, 63);
            this.Save.TabIndex = 7;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Close
            // 
            this.Close.Location = new System.Drawing.Point(432, 545);
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(145, 63);
            this.Close.TabIndex = 8;
            this.Close.Text = "Close";
            this.Close.UseVisualStyleBackColor = true;
            this.Close.Click += new System.EventHandler(this.Close_Click);
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
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(680, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 25);
            this.label7.TabIndex = 6;
            // 
            // ConfigureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 700);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureForm";
            this.Text = "Configure";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
    }
}