namespace TestAgent.FormsManager
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnSelectFiles = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lstSelectedTests = new System.Windows.Forms.CheckedListBox();
            this.cbTestfile = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtHostname = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnSelectFiles);
            this.pnlMain.Controls.Add(this.txtOutput);
            this.pnlMain.Controls.Add(this.label5);
            this.pnlMain.Controls.Add(this.btnRun);
            this.pnlMain.Controls.Add(this.label4);
            this.pnlMain.Controls.Add(this.lstSelectedTests);
            this.pnlMain.Controls.Add(this.cbTestfile);
            this.pnlMain.Controls.Add(this.label3);
            this.pnlMain.Controls.Add(this.lstFiles);
            this.pnlMain.Controls.Add(this.label2);
            this.pnlMain.Enabled = false;
            this.pnlMain.Location = new System.Drawing.Point(18, 65);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(522, 479);
            this.pnlMain.TabIndex = 7;
            // 
            // btnSelectFiles
            // 
            this.btnSelectFiles.Location = new System.Drawing.Point(4, 20);
            this.btnSelectFiles.Name = "btnSelectFiles";
            this.btnSelectFiles.Size = new System.Drawing.Size(188, 23);
            this.btnSelectFiles.TabIndex = 21;
            this.btnSelectFiles.Text = "Select files";
            this.btnSelectFiles.UseVisualStyleBackColor = true;
            this.btnSelectFiles.Click += new System.EventHandler(this.btnSelectFiles_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(0, 292);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(519, 183);
            this.txtOutput.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-2, 276);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Output:";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(1, 250);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(518, 23);
            this.btnRun.TabIndex = 18;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(212, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Tests to execute:";
            // 
            // lstSelectedTests
            // 
            this.lstSelectedTests.FormattingEnabled = true;
            this.lstSelectedTests.Location = new System.Drawing.Point(215, 59);
            this.lstSelectedTests.Name = "lstSelectedTests";
            this.lstSelectedTests.Size = new System.Drawing.Size(304, 184);
            this.lstSelectedTests.TabIndex = 16;
            // 
            // cbTestfile
            // 
            this.cbTestfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTestfile.FormattingEnabled = true;
            this.cbTestfile.Location = new System.Drawing.Point(215, 19);
            this.cbTestfile.Name = "cbTestfile";
            this.cbTestfile.Size = new System.Drawing.Size(304, 21);
            this.cbTestfile.TabIndex = 15;
            this.cbTestfile.SelectedIndexChanged += new System.EventHandler(this.cbTestfile_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(212, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Test file:";
            // 
            // lstFiles
            // 
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.Location = new System.Drawing.Point(3, 45);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(190, 199);
            this.lstFiles.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-2, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Files to sent:";
            // 
            // btnConnect
            // 
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(18, 33);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(523, 23);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtHostname
            // 
            this.txtHostname.Location = new System.Drawing.Point(83, 6);
            this.txtHostname.Name = "txtHostname";
            this.txtHostname.Size = new System.Drawing.Size(458, 20);
            this.txtHostname.TabIndex = 5;
            this.txtHostname.TextChanged += new System.EventHandler(this.txtHostname_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Agent Host:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 550);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtHostname);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Test Agent Manager";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnSelectFiles;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox lstSelectedTests;
        private System.Windows.Forms.ComboBox cbTestfile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtHostname;
        private System.Windows.Forms.Label label1;
    }
}

