namespace WebPrinter
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
            this.printerListBox = new System.Windows.Forms.ListBox();
            this.printBtn = new System.Windows.Forms.Button();
            this.printerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // printerGroupBox
            // 
            this.printerGroupBox.Controls.Add(this.printerListBox);
            this.printerGroupBox.Location = new System.Drawing.Point(24, 27);
            this.printerGroupBox.Name = "printerGroupBox";
            this.printerGroupBox.Size = new System.Drawing.Size(300, 200);
            this.printerGroupBox.TabIndex = 0;
            this.printerGroupBox.TabStop = false;
            this.printerGroupBox.Text = "Printer";
            // 
            // printerListBox
            // 
            this.printerListBox.FormattingEnabled = true;
            this.printerListBox.ItemHeight = 12;
            this.printerListBox.Location = new System.Drawing.Point(16, 29);
            this.printerListBox.Name = "printerListBox";
            this.printerListBox.Size = new System.Drawing.Size(250, 148);
            this.printerListBox.TabIndex = 1;
            // 
            // printBtn
            // 
            this.printBtn.Location = new System.Drawing.Point(355, 56);
            this.printBtn.Name = "printBtn";
            this.printBtn.Size = new System.Drawing.Size(75, 23);
            this.printBtn.TabIndex = 2;
            this.printBtn.Text = "Print Now";
            this.printBtn.UseVisualStyleBackColor = true;
            this.printBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.printBtn);
            this.Controls.Add(this.printerGroupBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.printerGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox printerGroupBox;
        private System.Windows.Forms.ListBox printerListBox;
        private System.Windows.Forms.Button printBtn;
    }
}