﻿namespace WebPrinter
{
    partial class Form1
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
            this.printerGroupBox = new System.Windows.Forms.GroupBox();
            this.landscapeBox = new System.Windows.Forms.CheckBox();
            this.printerListBox = new System.Windows.Forms.ListBox();
            this.printBWarenTestBtn = new System.Windows.Forms.Button();
            this.printVCDSTestBtn = new System.Windows.Forms.Button();
            this.printerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // printerGroupBox
            // 
            this.printerGroupBox.Controls.Add(this.landscapeBox);
            this.printerGroupBox.Controls.Add(this.printerListBox);
            this.printerGroupBox.Location = new System.Drawing.Point(24, 27);
            this.printerGroupBox.Name = "printerGroupBox";
            this.printerGroupBox.Size = new System.Drawing.Size(300, 393);
            this.printerGroupBox.TabIndex = 0;
            this.printerGroupBox.TabStop = false;
            this.printerGroupBox.Text = "Printer";
            // 
            // landscapeBox
            // 
            this.landscapeBox.AutoSize = true;
            this.landscapeBox.Checked = global::WebPrinter.Properties.Settings.Default.Landscape;
            this.landscapeBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WebPrinter.Properties.Settings.Default, "landscape", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.landscapeBox.Location = new System.Drawing.Point(16, 201);
            this.landscapeBox.Name = "landscapeBox";
            this.landscapeBox.Size = new System.Drawing.Size(78, 16);
            this.landscapeBox.TabIndex = 2;
            this.landscapeBox.Text = "Landscape";
            this.landscapeBox.UseVisualStyleBackColor = true;
            this.landscapeBox.CheckedChanged += new System.EventHandler(this.LandscapeBox_CheckedChanged);
            // 
            // printerListBox
            // 
            this.printerListBox.FormattingEnabled = true;
            this.printerListBox.ItemHeight = 12;
            this.printerListBox.Location = new System.Drawing.Point(16, 29);
            this.printerListBox.Name = "printerListBox";
            this.printerListBox.Size = new System.Drawing.Size(250, 148);
            this.printerListBox.TabIndex = 1;
            this.printerListBox.SelectedIndexChanged += new System.EventHandler(this.PrinterListBox_SelectedIndexChanged);
            // 
            // printBWarenTestBtn
            // 
            this.printBWarenTestBtn.Location = new System.Drawing.Point(355, 56);
            this.printBWarenTestBtn.Name = "printBWarenTestBtn";
            this.printBWarenTestBtn.Size = new System.Drawing.Size(123, 23);
            this.printBWarenTestBtn.TabIndex = 2;
            this.printBWarenTestBtn.Text = "Print BWaren Test";
            this.printBWarenTestBtn.UseVisualStyleBackColor = true;
            this.printBWarenTestBtn.Click += new System.EventHandler(this.PrintPdfTestBtn_Click);
            // 
            // printVCDSTestBtn
            // 
            this.printVCDSTestBtn.Location = new System.Drawing.Point(355, 85);
            this.printVCDSTestBtn.Name = "printVCDSTestBtn";
            this.printVCDSTestBtn.Size = new System.Drawing.Size(123, 23);
            this.printVCDSTestBtn.TabIndex = 3;
            this.printVCDSTestBtn.Text = "Print VCDS Test";
            this.printVCDSTestBtn.UseVisualStyleBackColor = true;
            this.printVCDSTestBtn.Click += new System.EventHandler(this.PrintVCDSTestBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.printVCDSTestBtn);
            this.Controls.Add(this.printBWarenTestBtn);
            this.Controls.Add(this.printerGroupBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.printerGroupBox.ResumeLayout(false);
            this.printerGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox printerGroupBox;
        private System.Windows.Forms.ListBox printerListBox;
        private System.Windows.Forms.Button printBWarenTestBtn;
        private System.Windows.Forms.CheckBox landscapeBox;
        private System.Windows.Forms.Button printVCDSTestBtn;
    }
}