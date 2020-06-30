namespace WinDiskSize
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
            this.btnDbConnect = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.chbExcludeSysFolders = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbDb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPw = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbTaskID = new System.Windows.Forms.TextBox();
            this.prsSqlSvr = new System.Windows.Forms.ProgressBar();
            this.btnMdbPath = new System.Windows.Forms.Button();
            this.tbMdbCsvFolderPath = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnClearStartFolder = new System.Windows.Forms.Button();
            this.btnClearDrive = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.tbExcludedAlways = new System.Windows.Forms.TextBox();
            this.tbExcludeSysFolders = new System.Windows.Forms.TextBox();
            this.btnClearMdbPath = new System.Windows.Forms.Button();
            this.chbMdb = new System.Windows.Forms.CheckBox();
            this.chbStorePw = new System.Windows.Forms.CheckBox();
            this.chbCsvFiles = new System.Windows.Forms.CheckBox();
            this.lblMdbCsvFolder = new System.Windows.Forms.Label();
            this.lblCsvEol = new System.Windows.Forms.Label();
            this.tbCsvEol = new System.Windows.Forms.TextBox();
            this.lblCsvDelim = new System.Windows.Forms.Label();
            this.tbCsvDelim = new System.Windows.Forms.TextBox();
            this.lblCsvFldrLstDelim = new System.Windows.Forms.Label();
            this.tbCsvFldrLstDelim = new System.Windows.Forms.TextBox();
            this.chbCsvCrc32 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(295, 420);
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
            this.lbDirList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDirList.FormattingEnabled = true;
            this.lbDirList.Location = new System.Drawing.Point(17, 230);
            this.lbDirList.Name = "lbDirList";
            this.lbDirList.Size = new System.Drawing.Size(765, 171);
            this.lbDirList.TabIndex = 1;
            // 
            // btnList
            // 
            this.btnList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnList.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnList.Location = new System.Drawing.Point(15, 23);
            this.btnList.Name = "btnList";
            this.btnList.Size = new System.Drawing.Size(60, 28);
            this.btnList.TabIndex = 2;
            this.btnList.Text = "Start";
            this.btnList.UseVisualStyleBackColor = true;
            this.btnList.Click += new System.EventHandler(this.btnList_Click);
            // 
            // btnParse
            // 
            this.btnParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnParse.Enabled = false;
            this.btnParse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnParse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnParse.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnParse.Location = new System.Drawing.Point(651, 26);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(70, 23);
            this.btnParse.TabIndex = 4;
            this.btnParse.Text = "Continue";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // btnDecLevel
            // 
            this.btnDecLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDecLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDecLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnDecLevel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnDecLevel.Location = new System.Drawing.Point(359, 416);
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
            this.txLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLevel.ForeColor = System.Drawing.Color.Red;
            this.txLevel.Location = new System.Drawing.Point(389, 418);
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
            this.btnIncLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIncLevel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnIncLevel.Location = new System.Drawing.Point(413, 416);
            this.btnIncLevel.Name = "btnIncLevel";
            this.btnIncLevel.Size = new System.Drawing.Size(23, 23);
            this.btnIncLevel.TabIndex = 7;
            this.btnIncLevel.Text = "+";
            this.btnIncLevel.UseVisualStyleBackColor = true;
            this.btnIncLevel.Click += new System.EventHandler(this.btnIncLevel_Click);
            // 
            // lblPercent
            // 
            this.lblPercent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPercent.BackColor = System.Drawing.SystemColors.Info;
            this.lblPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblPercent.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblPercent.Location = new System.Drawing.Point(14, 4);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(766, 16);
            this.lblPercent.TabIndex = 8;
            this.lblPercent.Text = "label2";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnStop.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnStop.Location = new System.Drawing.Point(727, 23);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(54, 28);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCopy.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnCopy.Location = new System.Drawing.Point(461, 409);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(134, 36);
            this.btnCopy.TabIndex = 10;
            this.btnCopy.Text = "Copy Result as Text";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 421);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Size Filter:";
            // 
            // tbFilterValue
            // 
            this.tbFilterValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbFilterValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbFilterValue.ForeColor = System.Drawing.Color.Red;
            this.tbFilterValue.Location = new System.Drawing.Point(159, 418);
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
            this.cbUnit.BackColor = System.Drawing.SystemColors.Info;
            this.cbUnit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbUnit.FormattingEnabled = true;
            this.cbUnit.Items.AddRange(new object[] {
            "B",
            "KB",
            "MB",
            "GB",
            "TB"});
            this.cbUnit.Location = new System.Drawing.Point(224, 418);
            this.cbUnit.Name = "cbUnit";
            this.cbUnit.Size = new System.Drawing.Size(54, 21);
            this.cbUnit.TabIndex = 13;
            this.cbUnit.SelectedIndexChanged += new System.EventHandler(this.cbUnit_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(615, 420);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Max UI Freez:";
            // 
            // cbFreez
            // 
            this.cbFreez.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFreez.BackColor = System.Drawing.SystemColors.Info;
            this.cbFreez.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbFreez.FormattingEnabled = true;
            this.cbFreez.Items.AddRange(new object[] {
            "500",
            "1000",
            "2000",
            "2500",
            "5000",
            "9999"});
            this.cbFreez.Location = new System.Drawing.Point(694, 417);
            this.cbFreez.Name = "cbFreez";
            this.cbFreez.Size = new System.Drawing.Size(60, 21);
            this.cbFreez.TabIndex = 15;
            this.cbFreez.SelectedIndexChanged += new System.EventHandler(this.cbFreez_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(760, 420);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "ms";
            // 
            // btnClearFilters
            // 
            this.btnClearFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearFilters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnClearFilters.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnClearFilters.Location = new System.Drawing.Point(14, 415);
            this.btnClearFilters.Name = "btnClearFilters";
            this.btnClearFilters.Size = new System.Drawing.Size(75, 23);
            this.btnClearFilters.TabIndex = 17;
            this.btnClearFilters.Text = "Clear Filters";
            this.btnClearFilters.UseVisualStyleBackColor = true;
            this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
            // 
            // cbDrive
            // 
            this.cbDrive.BackColor = System.Drawing.SystemColors.Info;
            this.cbDrive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cbDrive.FormattingEnabled = true;
            this.cbDrive.Location = new System.Drawing.Point(163, 23);
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
            this.tbCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbCaption.Location = new System.Drawing.Point(15, 210);
            this.tbCaption.Name = "tbCaption";
            this.tbCaption.ReadOnly = true;
            this.tbCaption.Size = new System.Drawing.Size(767, 20);
            this.tbCaption.TabIndex = 19;
            this.tbCaption.TabStop = false;
            this.tbCaption.Text = "{Hierarchy Level} (SUM of FileSizes) [Most Recent File Creation/Modify (aka Chang" +
    "e) FileTime] Path";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(79, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "select Drive";
            // 
            // btnStartFolder
            // 
            this.btnStartFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnStartFolder.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnStartFolder.Location = new System.Drawing.Point(345, 26);
            this.btnStartFolder.Name = "btnStartFolder";
            this.btnStartFolder.Size = new System.Drawing.Size(28, 23);
            this.btnStartFolder.TabIndex = 21;
            this.btnStartFolder.Text = "...";
            this.btnStartFolder.UseVisualStyleBackColor = true;
            this.btnStartFolder.Click += new System.EventHandler(this.btnStartFolder_Click);
            // 
            // tbStartFolder
            // 
            this.tbStartFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStartFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbStartFolder.Location = new System.Drawing.Point(431, 29);
            this.tbStartFolder.Name = "tbStartFolder";
            this.tbStartFolder.ReadOnly = true;
            this.tbStartFolder.Size = new System.Drawing.Size(202, 20);
            this.tbStartFolder.TabIndex = 22;
            this.tbStartFolder.TabStop = false;
            // 
            // chbShort83
            // 
            this.chbShort83.AutoSize = true;
            this.chbShort83.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chbShort83.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chbShort83.ForeColor = System.Drawing.Color.Goldenrod;
            this.chbShort83.Location = new System.Drawing.Point(17, 105);
            this.chbShort83.Name = "chbShort83";
            this.chbShort83.Size = new System.Drawing.Size(644, 17);
            this.chbShort83.TabIndex = 23;
            this.chbShort83.Text = "Parse and/or Show Short Names (8 + 3 format) //NOTE: On WinX the core system crea" +
    "tes Long Pathes (32K)";
            this.chbShort83.UseVisualStyleBackColor = true;
            this.chbShort83.Click += new System.EventHandler(this.chbShort83_Click);
            // 
            // btnDbConnect
            // 
            this.btnDbConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDbConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnDbConnect.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnDbConnect.Location = new System.Drawing.Point(17, 128);
            this.btnDbConnect.Name = "btnDbConnect";
            this.btnDbConnect.Size = new System.Drawing.Size(75, 48);
            this.btnDbConnect.TabIndex = 24;
            this.btnDbConnect.Text = "Connect to DB";
            this.btnDbConnect.UseVisualStyleBackColor = true;
            this.btnDbConnect.Click += new System.EventHandler(this.btnDbConnect_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(98, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "SQL Server\\Instance:";
            // 
            // tbServer
            // 
            this.tbServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbServer.Location = new System.Drawing.Point(212, 128);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(138, 20);
            this.tbServer.TabIndex = 26;
            this.tbServer.TextChanged += new System.EventHandler(this.tbSqlAny_TextChanged);
            // 
            // chbExcludeSysFolders
            // 
            this.chbExcludeSysFolders.AutoSize = true;
            this.chbExcludeSysFolders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chbExcludeSysFolders.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chbExcludeSysFolders.Location = new System.Drawing.Point(78, 83);
            this.chbExcludeSysFolders.Name = "chbExcludeSysFolders";
            this.chbExcludeSysFolders.Size = new System.Drawing.Size(99, 17);
            this.chbExcludeSysFolders.TabIndex = 27;
            this.chbExcludeSysFolders.Text = "Also exclude:";
            this.chbExcludeSysFolders.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chbExcludeSysFolders.UseVisualStyleBackColor = true;
            this.chbExcludeSysFolders.CheckedChanged += new System.EventHandler(this.chbExcludeSysFolders_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(362, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "DB:";
            // 
            // tbDb
            // 
            this.tbDb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDb.Location = new System.Drawing.Point(390, 128);
            this.tbDb.Name = "tbDb";
            this.tbDb.Size = new System.Drawing.Size(124, 20);
            this.tbDb.TabIndex = 29;
            this.tbDb.Text = "";
            this.tbDb.TextChanged += new System.EventHandler(this.tbSqlAny_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(523, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 30;
            this.label8.Text = "User:";
            // 
            // tbUser
            // 
            this.tbUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbUser.Location = new System.Drawing.Point(555, 128);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(93, 20);
            this.tbUser.TabIndex = 31;
            this.tbUser.Text = "";
            this.tbUser.TextChanged += new System.EventHandler(this.tbSqlAny_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(654, 119);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 32;
            this.label9.Text = "Password:";
            // 
            // tbPw
            // 
            this.tbPw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPw.Location = new System.Drawing.Point(711, 129);
            this.tbPw.Name = "tbPw";
            this.tbPw.Size = new System.Drawing.Size(68, 20);
            this.tbPw.TabIndex = 33;
            this.tbPw.UseSystemPasswordChar = true;
            this.tbPw.TextChanged += new System.EventHandler(this.tbSqlAny_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 130);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "DB Task ID:";
            // 
            // tbTaskID
            // 
            this.tbTaskID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbTaskID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbTaskID.ForeColor = System.Drawing.Color.Red;
            this.tbTaskID.Location = new System.Drawing.Point(17, 153);
            this.tbTaskID.Name = "tbTaskID";
            this.tbTaskID.ReadOnly = true;
            this.tbTaskID.Size = new System.Drawing.Size(58, 22);
            this.tbTaskID.TabIndex = 36;
            // 
            // prsSqlSvr
            // 
            this.prsSqlSvr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prsSqlSvr.BackColor = System.Drawing.SystemColors.Info;
            this.prsSqlSvr.Location = new System.Drawing.Point(14, 2);
            this.prsSqlSvr.Name = "prsSqlSvr";
            this.prsSqlSvr.Size = new System.Drawing.Size(766, 17);
            this.prsSqlSvr.TabIndex = 37;
            this.prsSqlSvr.Visible = false;
            // 
            // btnMdbPath
            // 
            this.btnMdbPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMdbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnMdbPath.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnMdbPath.Location = new System.Drawing.Point(144, 180);
            this.btnMdbPath.Name = "btnMdbPath";
            this.btnMdbPath.Size = new System.Drawing.Size(26, 23);
            this.btnMdbPath.TabIndex = 39;
            this.btnMdbPath.Text = "...";
            this.btnMdbPath.UseVisualStyleBackColor = true;
            this.btnMdbPath.Click += new System.EventHandler(this.btnMdbPath_Click);
            // 
            // tbMdbCsvFolderPath
            // 
            this.tbMdbCsvFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMdbCsvFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbMdbCsvFolderPath.Location = new System.Drawing.Point(233, 182);
            this.tbMdbCsvFolderPath.Name = "tbMdbCsvFolderPath";
            this.tbMdbCsvFolderPath.ReadOnly = true;
            this.tbMdbCsvFolderPath.Size = new System.Drawing.Size(546, 20);
            this.tbMdbCsvFolderPath.TabIndex = 40;
            this.tbMdbCsvFolderPath.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label12.Location = new System.Drawing.Point(282, 32);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 13);
            this.label12.TabIndex = 41;
            this.label12.Text = "or Folder";
            // 
            // btnClearStartFolder
            // 
            this.btnClearStartFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearStartFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnClearStartFolder.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnClearStartFolder.Location = new System.Drawing.Point(378, 26);
            this.btnClearStartFolder.Name = "btnClearStartFolder";
            this.btnClearStartFolder.Size = new System.Drawing.Size(47, 23);
            this.btnClearStartFolder.TabIndex = 42;
            this.btnClearStartFolder.Text = "Clear";
            this.btnClearStartFolder.UseVisualStyleBackColor = true;
            this.btnClearStartFolder.Click += new System.EventHandler(this.btnClearStartFolder_Click);
            // 
            // btnClearDrive
            // 
            this.btnClearDrive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnClearDrive.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnClearDrive.Location = new System.Drawing.Point(224, 26);
            this.btnClearDrive.Name = "btnClearDrive";
            this.btnClearDrive.Size = new System.Drawing.Size(47, 23);
            this.btnClearDrive.TabIndex = 43;
            this.btnClearDrive.Text = "Clear";
            this.btnClearDrive.UseVisualStyleBackColor = true;
            this.btnClearDrive.Click += new System.EventHandler(this.btnClearDrive_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label13.Location = new System.Drawing.Point(14, 61);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(161, 13);
            this.label13.TabIndex = 44;
            this.label13.Text = "Exlucluded Level 1 folders:";
            // 
            // tbExcludedAlways
            // 
            this.tbExcludedAlways.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExcludedAlways.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbExcludedAlways.Location = new System.Drawing.Point(178, 58);
            this.tbExcludedAlways.Name = "tbExcludedAlways";
            this.tbExcludedAlways.Size = new System.Drawing.Size(603, 20);
            this.tbExcludedAlways.TabIndex = 45;
            this.tbExcludedAlways.Text = "$RECYCLE.BIN; System Volume Information";
            // 
            // tbExcludeSysFolders
            // 
            this.tbExcludeSysFolders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbExcludeSysFolders.Location = new System.Drawing.Point(178, 82);
            this.tbExcludeSysFolders.Name = "tbExcludeSysFolders";
            this.tbExcludeSysFolders.Size = new System.Drawing.Size(601, 20);
            this.tbExcludeSysFolders.TabIndex = 46;
            this.tbExcludeSysFolders.Text = "Users; Windows; Program Files; Program Files (x86); ProgramData";
            // 
            // btnClearMdbPath
            // 
            this.btnClearMdbPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearMdbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnClearMdbPath.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnClearMdbPath.Location = new System.Drawing.Point(178, 180);
            this.btnClearMdbPath.Name = "btnClearMdbPath";
            this.btnClearMdbPath.Size = new System.Drawing.Size(47, 23);
            this.btnClearMdbPath.TabIndex = 47;
            this.btnClearMdbPath.Text = "Clear";
            this.btnClearMdbPath.UseVisualStyleBackColor = true;
            this.btnClearMdbPath.Click += new System.EventHandler(this.btnClearMdbPath_Click);
            // 
            // chbMdb
            // 
            this.chbMdb.AutoSize = true;
            this.chbMdb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chbMdb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chbMdb.ForeColor = System.Drawing.Color.RoyalBlue;
            this.chbMdb.Location = new System.Drawing.Point(101, 156);
            this.chbMdb.Name = "chbMdb";
            this.chbMdb.Size = new System.Drawing.Size(84, 17);
            this.chbMdb.TabIndex = 48;
            this.chbMdb.Text = "MDB instead";
            this.chbMdb.UseVisualStyleBackColor = true;
            this.chbMdb.CheckedChanged += new System.EventHandler(this.chbMdbPath_CheckedChanged);
            // 
            // chbStorePw
            // 
            this.chbStorePw.AutoSize = true;
            this.chbStorePw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chbStorePw.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chbStorePw.ForeColor = System.Drawing.Color.RoyalBlue;
            this.chbStorePw.Location = new System.Drawing.Point(657, 135);
            this.chbStorePw.Name = "chbStorePw";
            this.chbStorePw.Size = new System.Drawing.Size(48, 17);
            this.chbStorePw.TabIndex = 49;
            this.chbStorePw.Text = "Save";
            this.chbStorePw.UseVisualStyleBackColor = true;
            this.chbStorePw.CheckedChanged += new System.EventHandler(this.chbStorePw_CheckedChanged);
            // 
            // chbCsvFiles
            // 
            this.chbCsvFiles.AutoSize = true;
            this.chbCsvFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chbCsvFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chbCsvFiles.ForeColor = System.Drawing.Color.RoyalBlue;
            this.chbCsvFiles.Location = new System.Drawing.Point(191, 156);
            this.chbCsvFiles.Name = "chbCsvFiles";
            this.chbCsvFiles.Size = new System.Drawing.Size(105, 17);
            this.chbCsvFiles.TabIndex = 50;
            this.chbCsvFiles.Text = "CSV files instead:";
            this.chbCsvFiles.UseVisualStyleBackColor = true;
            this.chbCsvFiles.CheckedChanged += new System.EventHandler(this.chbCsvFiles_CheckedChanged);
            // 
            // lblMdbCsvFolder
            // 
            this.lblMdbCsvFolder.AutoSize = true;
            this.lblMdbCsvFolder.Location = new System.Drawing.Point(14, 185);
            this.lblMdbCsvFolder.Name = "lblMdbCsvFolder";
            this.lblMdbCsvFolder.Size = new System.Drawing.Size(123, 13);
            this.lblMdbCsvFolder.TabIndex = 51;
            this.lblMdbCsvFolder.Text = "MDB or CSV folder path:";
            // 
            // lblCsvEol
            // 
            this.lblCsvEol.AutoSize = true;
            this.lblCsvEol.Location = new System.Drawing.Point(297, 158);
            this.lblCsvEol.Name = "lblCsvEol";
            this.lblCsvEol.Size = new System.Drawing.Size(31, 13);
            this.lblCsvEol.TabIndex = 52;
            this.lblCsvEol.Text = "EOL:";
            // 
            // tbCsvEol
            // 
            this.tbCsvEol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCsvEol.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbCsvEol.Location = new System.Drawing.Point(330, 156);
            this.tbCsvEol.Name = "tbCsvEol";
            this.tbCsvEol.Size = new System.Drawing.Size(48, 18);
            this.tbCsvEol.TabIndex = 53;
            this.tbCsvEol.Text = "\\r\\n";
            this.tbCsvEol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblCsvDelim
            // 
            this.lblCsvDelim.AutoSize = true;
            this.lblCsvDelim.Location = new System.Drawing.Point(386, 158);
            this.lblCsvDelim.Name = "lblCsvDelim";
            this.lblCsvDelim.Size = new System.Drawing.Size(50, 13);
            this.lblCsvDelim.TabIndex = 54;
            this.lblCsvDelim.Text = "Delimiter:";
            // 
            // tbCsvDelim
            // 
            this.tbCsvDelim.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCsvDelim.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbCsvDelim.Location = new System.Drawing.Point(437, 156);
            this.tbCsvDelim.Name = "tbCsvDelim";
            this.tbCsvDelim.Size = new System.Drawing.Size(48, 18);
            this.tbCsvDelim.TabIndex = 55;
            this.tbCsvDelim.Text = ";";
            this.tbCsvDelim.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblCsvFldrLstDelim
            // 
            this.lblCsvFldrLstDelim.AutoSize = true;
            this.lblCsvFldrLstDelim.Location = new System.Drawing.Point(494, 158);
            this.lblCsvFldrLstDelim.Name = "lblCsvFldrLstDelim";
            this.lblCsvFldrLstDelim.Size = new System.Drawing.Size(101, 13);
            this.lblCsvFldrLstDelim.TabIndex = 56;
            this.lblCsvFldrLstDelim.Text = "Folder List Delimiter:";
            // 
            // tbCsvFldrLstDelim
            // 
            this.tbCsvFldrLstDelim.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCsvFldrLstDelim.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbCsvFldrLstDelim.Location = new System.Drawing.Point(602, 156);
            this.tbCsvFldrLstDelim.Name = "tbCsvFldrLstDelim";
            this.tbCsvFldrLstDelim.Size = new System.Drawing.Size(48, 18);
            this.tbCsvFldrLstDelim.TabIndex = 57;
            this.tbCsvFldrLstDelim.Text = "\\";
            this.tbCsvFldrLstDelim.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chbCsvCrc32
            // 
            this.chbCsvCrc32.AutoSize = true;
            this.chbCsvCrc32.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chbCsvCrc32.Location = new System.Drawing.Point(657, 157);
            this.chbCsvCrc32.Name = "chbCsvCrc32";
            this.chbCsvCrc32.Size = new System.Drawing.Size(102, 17);
            this.chbCsvCrc32.TabIndex = 58;
            this.chbCsvCrc32.Text = "Compute CRC32";
            this.chbCsvCrc32.UseVisualStyleBackColor = true;
            this.chbCsvCrc32.CheckedChanged += new System.EventHandler(this.chbCsvCrc32_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(792, 473);
            this.Controls.Add(this.chbCsvCrc32);
            this.Controls.Add(this.tbCsvFldrLstDelim);
            this.Controls.Add(this.lblCsvFldrLstDelim);
            this.Controls.Add(this.tbCsvDelim);
            this.Controls.Add(this.lblCsvDelim);
            this.Controls.Add(this.tbCsvEol);
            this.Controls.Add(this.lblCsvEol);
            this.Controls.Add(this.lblMdbCsvFolder);
            this.Controls.Add(this.chbCsvFiles);
            this.Controls.Add(this.chbStorePw);
            this.Controls.Add(this.chbMdb);
            this.Controls.Add(this.btnClearMdbPath);
            this.Controls.Add(this.tbExcludeSysFolders);
            this.Controls.Add(this.tbExcludedAlways);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.btnClearDrive);
            this.Controls.Add(this.btnClearStartFolder);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnDbConnect);
            this.Controls.Add(this.tbMdbCsvFolderPath);
            this.Controls.Add(this.prsSqlSvr);
            this.Controls.Add(this.tbTaskID);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.lbDirList);
            this.Controls.Add(this.tbPw);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbUser);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbDb);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chbExcludeSysFolders);
            this.Controls.Add(this.tbServer);
            this.Controls.Add(this.label6);
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
            this.Controls.Add(this.btnList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMdbPath);
            this.Name = "FormMain";
            this.Text = "WinDiskSize";
            this.Shown += new System.EventHandler(this.FormMain_Shown);
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
        private System.Windows.Forms.Button btnDbConnect;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.CheckBox chbExcludeSysFolders;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbDb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbUser;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbPw;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbTaskID;
        private System.Windows.Forms.ProgressBar prsSqlSvr;
        private System.Windows.Forms.Button btnMdbPath;
        private System.Windows.Forms.TextBox tbMdbCsvFolderPath;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnClearStartFolder;
        private System.Windows.Forms.Button btnClearDrive;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbExcludedAlways;
        private System.Windows.Forms.TextBox tbExcludeSysFolders;
        private System.Windows.Forms.Button btnClearMdbPath;
        private System.Windows.Forms.CheckBox chbMdb;
        private System.Windows.Forms.CheckBox chbStorePw;
        private System.Windows.Forms.CheckBox chbCsvFiles;
        private System.Windows.Forms.Label lblMdbCsvFolder;
        private System.Windows.Forms.Label lblCsvEol;
        private System.Windows.Forms.TextBox tbCsvEol;
        private System.Windows.Forms.Label lblCsvDelim;
        private System.Windows.Forms.TextBox tbCsvDelim;
        private System.Windows.Forms.Label lblCsvFldrLstDelim;
        private System.Windows.Forms.TextBox tbCsvFldrLstDelim;
        private System.Windows.Forms.CheckBox chbCsvCrc32;
    }
}

