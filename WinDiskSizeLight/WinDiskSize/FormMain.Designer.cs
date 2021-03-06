﻿namespace WinDiskSize
{
    partial class FormMain
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
            this.lbDirList = new System.Windows.Forms.ListBox();
            this.btnList = new System.Windows.Forms.Button();
            this.btnParse = new System.Windows.Forms.Button();
            this.btnDecLevel = new System.Windows.Forms.Button();
            this.txLevel = new System.Windows.Forms.TextBox();
            this.btnIncLevel = new System.Windows.Forms.Button();
            this.lblPercent = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFilterValue = new System.Windows.Forms.TextBox();
            this.cbUnit = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbFreez = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClearFilters = new System.Windows.Forms.Button();
            this.cbDrive = new System.Windows.Forms.ComboBox();
            this.tbCaption = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnStartFolder = new System.Windows.Forms.Button();
            this.tbStartFolder = new System.Windows.Forms.TextBox();
            this.chbShort83 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(295, 429);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Level Filter:";
            // 
            // lbDirList
            // 
            this.lbDirList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDirList.BackColor = System.Drawing.Color.LightYellow;
            this.lbDirList.FormattingEnabled = true;
            this.lbDirList.Location = new System.Drawing.Point(15, 103);
            this.lbDirList.Name = "lbDirList";
            this.lbDirList.Size = new System.Drawing.Size(647, 316);
            this.lbDirList.TabIndex = 1;
            // 
            // btnList
            // 
            this.btnList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnList.Location = new System.Drawing.Point(173, 3);
            this.btnList.Name = "btnList";
            this.btnList.Size = new System.Drawing.Size(60, 23);
            this.btnList.TabIndex = 2;
            this.btnList.Text = "Start";
            this.btnList.UseVisualStyleBackColor = true;
            this.btnList.Click += new System.EventHandler(this.btnList_Click);
            // 
            // btnParse
            // 
            this.btnParse.Enabled = false;
            this.btnParse.Location = new System.Drawing.Point(173, 26);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(61, 23);
            this.btnParse.TabIndex = 4;
            this.btnParse.Text = "Continue";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // btnDecLevel
            // 
            this.btnDecLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDecLevel.Location = new System.Drawing.Point(359, 425);
            this.btnDecLevel.Name = "btnDecLevel";
            this.btnDecLevel.Size = new System.Drawing.Size(24, 23);
            this.btnDecLevel.TabIndex = 5;
            this.btnDecLevel.Text = "-";
            this.btnDecLevel.UseVisualStyleBackColor = true;
            this.btnDecLevel.Click += new System.EventHandler(this.btnDecLevel_Click);
            // 
            // txLevel
            // 
            this.txLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txLevel.ForeColor = System.Drawing.Color.Red;
            this.txLevel.Location = new System.Drawing.Point(389, 427);
            this.txLevel.Name = "txLevel";
            this.txLevel.ReadOnly = true;
            this.txLevel.Size = new System.Drawing.Size(18, 20);
            this.txLevel.TabIndex = 6;
            this.txLevel.Text = "1";
            this.txLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnIncLevel
            // 
            this.btnIncLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIncLevel.Location = new System.Drawing.Point(413, 425);
            this.btnIncLevel.Name = "btnIncLevel";
            this.btnIncLevel.Size = new System.Drawing.Size(23, 23);
            this.btnIncLevel.TabIndex = 7;
            this.btnIncLevel.Text = "+";
            this.btnIncLevel.UseVisualStyleBackColor = true;
            this.btnIncLevel.Click += new System.EventHandler(this.btnIncLevel_Click);
            // 
            // lblPercent
            // 
            this.lblPercent.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblPercent.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblPercent.Location = new System.Drawing.Point(240, 29);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(347, 16);
            this.lblPercent.TabIndex = 8;
            this.lblPercent.Text = "label2";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnStop.Location = new System.Drawing.Point(593, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(69, 44);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCopy.Location = new System.Drawing.Point(15, 4);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(87, 45);
            this.btnCopy.TabIndex = 10;
            this.btnCopy.Text = "Copy Result as Text";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 430);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Size Filter:";
            // 
            // tbFilterValue
            // 
            this.tbFilterValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbFilterValue.ForeColor = System.Drawing.Color.Red;
            this.tbFilterValue.Location = new System.Drawing.Point(159, 427);
            this.tbFilterValue.MaxLength = 4;
            this.tbFilterValue.Name = "tbFilterValue";
            this.tbFilterValue.Size = new System.Drawing.Size(59, 20);
            this.tbFilterValue.TabIndex = 12;
            this.tbFilterValue.Text = "0";
            this.tbFilterValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbFilterValue.Enter += new System.EventHandler(this.tbFilterValue_Enter);
            this.tbFilterValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFilterValue_KeyPress);
            this.tbFilterValue.Leave += new System.EventHandler(this.tbFilterValue_Leave);
            // 
            // cbUnit
            // 
            this.cbUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbUnit.FormattingEnabled = true;
            this.cbUnit.Items.AddRange(new object[] {
            "B",
            "KB",
            "MB",
            "GB",
            "TB"});
            this.cbUnit.Location = new System.Drawing.Point(224, 427);
            this.cbUnit.Name = "cbUnit";
            this.cbUnit.Size = new System.Drawing.Size(54, 21);
            this.cbUnit.TabIndex = 13;
            this.cbUnit.SelectedIndexChanged += new System.EventHandler(this.cbUnit_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(497, 429);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Max UI Freez:";
            // 
            // cbFreez
            // 
            this.cbFreez.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFreez.FormattingEnabled = true;
            this.cbFreez.Items.AddRange(new object[] {
            "500",
            "1000",
            "2000",
            "2500",
            "5000",
            "9999"});
            this.cbFreez.Location = new System.Drawing.Point(576, 426);
            this.cbFreez.Name = "cbFreez";
            this.cbFreez.Size = new System.Drawing.Size(60, 21);
            this.cbFreez.TabIndex = 15;
            this.cbFreez.SelectedIndexChanged += new System.EventHandler(this.cbFreez_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(642, 429);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "ms";
            // 
            // btnClearFilters
            // 
            this.btnClearFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearFilters.Location = new System.Drawing.Point(14, 424);
            this.btnClearFilters.Name = "btnClearFilters";
            this.btnClearFilters.Size = new System.Drawing.Size(75, 23);
            this.btnClearFilters.TabIndex = 17;
            this.btnClearFilters.Text = "Clear Filters";
            this.btnClearFilters.UseVisualStyleBackColor = true;
            this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
            // 
            // cbDrive
            // 
            this.cbDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cbDrive.FormattingEnabled = true;
            this.cbDrive.Items.AddRange(new object[] {
            "A:",
            "B:",
            "C:",
            "D:",
            "E:",
            "F:",
            "G:",
            "H:",
            "I:",
            "J:",
            "K:",
            "L:",
            "M:",
            "N:",
            "O:",
            "P:",
            "Q:",
            "R:",
            "S:",
            "T:",
            "U:",
            "V:",
            "W:",
            "X:",
            "Y:",
            "Z:"});
            this.cbDrive.Location = new System.Drawing.Point(108, 12);
            this.cbDrive.Name = "cbDrive";
            this.cbDrive.Size = new System.Drawing.Size(55, 28);
            this.cbDrive.TabIndex = 18;
            this.cbDrive.SelectedIndexChanged += new System.EventHandler(this.cbDrive_SelectedIndexChanged);
            // 
            // tbCaption
            // 
            this.tbCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCaption.BackColor = System.Drawing.Color.Silver;
            this.tbCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbCaption.Location = new System.Drawing.Point(15, 77);
            this.tbCaption.Name = "tbCaption";
            this.tbCaption.ReadOnly = true;
            this.tbCaption.Size = new System.Drawing.Size(647, 20);
            this.tbCaption.TabIndex = 19;
            this.tbCaption.TabStop = false;
            this.tbCaption.Text = "{Hierarchy Level} (SUM of FileSizes) [Most Recent File Creation/Modify (aka Chang" +
                "e) FileTime] Path";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Start Folder:";
            // 
            // btnStartFolder
            // 
            this.btnStartFolder.Location = new System.Drawing.Point(108, 51);
            this.btnStartFolder.Name = "btnStartFolder";
            this.btnStartFolder.Size = new System.Drawing.Size(55, 23);
            this.btnStartFolder.TabIndex = 21;
            this.btnStartFolder.Text = "...";
            this.btnStartFolder.UseVisualStyleBackColor = true;
            this.btnStartFolder.Click += new System.EventHandler(this.btnStartFolder_Click);
            // 
            // tbStartFolder
            // 
            this.tbStartFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStartFolder.Location = new System.Drawing.Point(173, 53);
            this.tbStartFolder.Name = "tbStartFolder";
            this.tbStartFolder.ReadOnly = true;
            this.tbStartFolder.Size = new System.Drawing.Size(488, 20);
            this.tbStartFolder.TabIndex = 22;
            this.tbStartFolder.TabStop = false;
            // 
            // chbShort83
            // 
            this.chbShort83.AutoSize = true;
            this.chbShort83.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chbShort83.ForeColor = System.Drawing.Color.Goldenrod;
            this.chbShort83.Location = new System.Drawing.Point(239, 7);
            this.chbShort83.Name = "chbShort83";
            this.chbShort83.Size = new System.Drawing.Size(647, 17);
            this.chbShort83.TabIndex = 23;
            this.chbShort83.Text = "Parse and/or Show Short Names (8 + 3 format) //NOTE: On WinX the core system crea" +
                "tes Long Pathes (32K)";
            this.chbShort83.UseVisualStyleBackColor = true;
            this.chbShort83.Click += new System.EventHandler(this.chbShort83_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 477);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.chbShort83);
            this.Controls.Add(this.tbStartFolder);
            this.Controls.Add(this.btnStartFolder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbCaption);
            this.Controls.Add(this.cbDrive);
            this.Controls.Add(this.btnClearFilters);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbFreez);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbUnit);
            this.Controls.Add(this.tbFilterValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.btnIncLevel);
            this.Controls.Add(this.txLevel);
            this.Controls.Add(this.btnDecLevel);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.btnList);
            this.Controls.Add(this.lbDirList);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "WinDiskSize";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbDirList;
        private System.Windows.Forms.Button btnList;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Button btnDecLevel;
        private System.Windows.Forms.TextBox txLevel;
        private System.Windows.Forms.Button btnIncLevel;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFilterValue;
        private System.Windows.Forms.ComboBox cbUnit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbFreez;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnClearFilters;
        private System.Windows.Forms.ComboBox cbDrive;
        private System.Windows.Forms.TextBox tbCaption;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnStartFolder;
        private System.Windows.Forms.TextBox tbStartFolder;
        private System.Windows.Forms.CheckBox chbShort83;
    }
}

