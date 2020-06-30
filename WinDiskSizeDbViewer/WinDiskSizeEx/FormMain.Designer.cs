namespace WinDiskSizeEx
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
            this.Views = new System.Windows.Forms.TabControl();
            this.viewMain = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMainOpenReport = new System.Windows.Forms.Button();
            this.lblTabCaptionMain = new System.Windows.Forms.Label();
            this.lblMainOpen = new System.Windows.Forms.Label();
            this.btnMainOpen = new System.Windows.Forms.Button();
            this.viewSource = new System.Windows.Forms.TabPage();
            this.lblTabCaptionSource = new System.Windows.Forms.Label();
            this.lblSourceMdb = new System.Windows.Forms.Label();
            this.btnSourceMdb = new System.Windows.Forms.Button();
            this.viewSourceMdb = new System.Windows.Forms.TabPage();
            this.btnSourceMdbNew = new System.Windows.Forms.Button();
            this.tbSourceMdbNewTitle = new System.Windows.Forms.TextBox();
            this.lblSourceMdbNewTitle = new System.Windows.Forms.Label();
            this.lblTabCaptionSourceMdb = new System.Windows.Forms.Label();
            this.lbSourceMdbFolder = new System.Windows.Forms.ListBox();
            this.tbSourceMdbFolder = new System.Windows.Forms.TextBox();
            this.lblSourceMdbFolderList = new System.Windows.Forms.Label();
            this.lblSourceMdbFolder = new System.Windows.Forms.Label();
            this.btnSourceMdbFolder = new System.Windows.Forms.Button();
            this.viewTasks = new System.Windows.Forms.TabPage();
            this.lblTabCaptionTasks = new System.Windows.Forms.Label();
            this.lvTaskList = new System.Windows.Forms.ListView();
            this.lblTaskList = new System.Windows.Forms.Label();
            this.viewCompare = new System.Windows.Forms.TabPage();
            this.lblDiffClrEquHnd = new System.Windows.Forms.Label();
            this.lblDiffClrMissOtherHdn = new System.Windows.Forms.Label();
            this.lblDiffClrMissOther = new System.Windows.Forms.Label();
            this.lblDiffClrMissOneHdn = new System.Windows.Forms.Label();
            this.lblDiffClrMissOne = new System.Windows.Forms.Label();
            this.lblDiffClrDiffOther = new System.Windows.Forms.Label();
            this.lblDiffClrDiffOne = new System.Windows.Forms.Label();
            this.lblDiffClrEqu = new System.Windows.Forms.Label();
            this.lblDiffClrs = new System.Windows.Forms.Label();
            this.lblDiffClrUnk = new System.Windows.Forms.Label();
            this.lvCompareTask = new System.Windows.Forms.ListView();
            this.lvCompare = new System.Windows.Forms.ListView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.lblViewStack = new System.Windows.Forms.Label();
            this.prsMain = new System.Windows.Forms.ProgressBar();
            this.btnCompareWith = new System.Windows.Forms.Button();
            this.lblPrsMain = new System.Windows.Forms.Label();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnAddToReport = new System.Windows.Forms.Button();
            this.btnOpenUpdatedReport = new System.Windows.Forms.Button();
            this.btnPrintReport = new System.Windows.Forms.Button();
            this.Views.SuspendLayout();
            this.viewMain.SuspendLayout();
            this.viewSource.SuspendLayout();
            this.viewSourceMdb.SuspendLayout();
            this.viewTasks.SuspendLayout();
            this.viewCompare.SuspendLayout();
            this.SuspendLayout();
            // 
            // Views
            // 
            this.Views.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Views.Controls.Add(this.viewMain);
            this.Views.Controls.Add(this.viewSource);
            this.Views.Controls.Add(this.viewSourceMdb);
            this.Views.Controls.Add(this.viewTasks);
            this.Views.Controls.Add(this.viewCompare);
            this.Views.Location = new System.Drawing.Point(12, 56);
            this.Views.Name = "Views";
            this.Views.SelectedIndex = 0;
            this.Views.Size = new System.Drawing.Size(675, 384);
            this.Views.TabIndex = 0;
            // 
            // viewMain
            // 
            this.viewMain.BackColor = System.Drawing.SystemColors.Window;
            this.viewMain.Controls.Add(this.label1);
            this.viewMain.Controls.Add(this.btnMainOpenReport);
            this.viewMain.Controls.Add(this.lblTabCaptionMain);
            this.viewMain.Controls.Add(this.lblMainOpen);
            this.viewMain.Controls.Add(this.btnMainOpen);
            this.viewMain.Location = new System.Drawing.Point(4, 22);
            this.viewMain.Name = "viewMain";
            this.viewMain.Padding = new System.Windows.Forms.Padding(3);
            this.viewMain.Size = new System.Drawing.Size(667, 358);
            this.viewMain.TabIndex = 1;
            this.viewMain.Text = "Home";
            this.viewMain.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(235, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(326, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Open an existing Disk Map Compare Result Report recorded earlier.";
            // 
            // btnMainOpenReport
            // 
            this.btnMainOpenReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMainOpenReport.Location = new System.Drawing.Point(73, 152);
            this.btnMainOpenReport.Name = "btnMainOpenReport";
            this.btnMainOpenReport.Size = new System.Drawing.Size(132, 23);
            this.btnMainOpenReport.TabIndex = 4;
            this.btnMainOpenReport.Text = "Open Disk Map Report";
            this.btnMainOpenReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMainOpenReport.UseVisualStyleBackColor = true;
            this.btnMainOpenReport.Click += new System.EventHandler(this.btnMainOpenReport_Click);
            // 
            // lblTabCaptionMain
            // 
            this.lblTabCaptionMain.AutoSize = true;
            this.lblTabCaptionMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTabCaptionMain.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblTabCaptionMain.Location = new System.Drawing.Point(68, 36);
            this.lblTabCaptionMain.Name = "lblTabCaptionMain";
            this.lblTabCaptionMain.Size = new System.Drawing.Size(272, 25);
            this.lblTabCaptionMain.TabIndex = 3;
            this.lblTabCaptionMain.Text = "Select what you want to Do";
            // 
            // lblMainOpen
            // 
            this.lblMainOpen.AutoSize = true;
            this.lblMainOpen.Location = new System.Drawing.Point(235, 116);
            this.lblMainOpen.Name = "lblMainOpen";
            this.lblMainOpen.Size = new System.Drawing.Size(213, 13);
            this.lblMainOpen.TabIndex = 1;
            this.lblMainOpen.Text = "Open an existing Disk Map recorded earlier.";
            // 
            // btnMainOpen
            // 
            this.btnMainOpen.BackColor = System.Drawing.SystemColors.Window;
            this.btnMainOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMainOpen.Location = new System.Drawing.Point(73, 111);
            this.btnMainOpen.Name = "btnMainOpen";
            this.btnMainOpen.Size = new System.Drawing.Size(132, 23);
            this.btnMainOpen.TabIndex = 0;
            this.btnMainOpen.Text = "Open Disk Map";
            this.btnMainOpen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMainOpen.UseVisualStyleBackColor = false;
            this.btnMainOpen.Click += new System.EventHandler(this.btnMainOpen_Click);
            // 
            // viewSource
            // 
            this.viewSource.BackColor = System.Drawing.SystemColors.Window;
            this.viewSource.Controls.Add(this.lblTabCaptionSource);
            this.viewSource.Controls.Add(this.lblSourceMdb);
            this.viewSource.Controls.Add(this.btnSourceMdb);
            this.viewSource.Location = new System.Drawing.Point(4, 22);
            this.viewSource.Name = "viewSource";
            this.viewSource.Padding = new System.Windows.Forms.Padding(3);
            this.viewSource.Size = new System.Drawing.Size(667, 358);
            this.viewSource.TabIndex = 2;
            this.viewSource.Text = "Source";
            this.viewSource.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // lblTabCaptionSource
            // 
            this.lblTabCaptionSource.AutoSize = true;
            this.lblTabCaptionSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTabCaptionSource.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblTabCaptionSource.Location = new System.Drawing.Point(68, 36);
            this.lblTabCaptionSource.Name = "lblTabCaptionSource";
            this.lblTabCaptionSource.Size = new System.Drawing.Size(313, 25);
            this.lblTabCaptionSource.TabIndex = 4;
            this.lblTabCaptionSource.Text = "Select the type of the Database";
            // 
            // lblSourceMdb
            // 
            this.lblSourceMdb.AutoSize = true;
            this.lblSourceMdb.Location = new System.Drawing.Point(235, 116);
            this.lblSourceMdb.Name = "lblSourceMdb";
            this.lblSourceMdb.Size = new System.Drawing.Size(259, 13);
            this.lblSourceMdb.TabIndex = 1;
            this.lblSourceMdb.Text = "Open existing Microsoft Access Database (.MDB) file.";
            // 
            // btnSourceMdb
            // 
            this.btnSourceMdb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSourceMdb.Location = new System.Drawing.Point(73, 111);
            this.btnSourceMdb.Name = "btnSourceMdb";
            this.btnSourceMdb.Size = new System.Drawing.Size(132, 23);
            this.btnSourceMdb.TabIndex = 0;
            this.btnSourceMdb.Text = "MDB";
            this.btnSourceMdb.UseVisualStyleBackColor = true;
            this.btnSourceMdb.Click += new System.EventHandler(this.btnSourceMdb_Click);
            // 
            // viewSourceMdb
            // 
            this.viewSourceMdb.BackColor = System.Drawing.SystemColors.Window;
            this.viewSourceMdb.Controls.Add(this.btnSourceMdbNew);
            this.viewSourceMdb.Controls.Add(this.tbSourceMdbNewTitle);
            this.viewSourceMdb.Controls.Add(this.lblSourceMdbNewTitle);
            this.viewSourceMdb.Controls.Add(this.lblTabCaptionSourceMdb);
            this.viewSourceMdb.Controls.Add(this.lbSourceMdbFolder);
            this.viewSourceMdb.Controls.Add(this.tbSourceMdbFolder);
            this.viewSourceMdb.Controls.Add(this.lblSourceMdbFolderList);
            this.viewSourceMdb.Controls.Add(this.lblSourceMdbFolder);
            this.viewSourceMdb.Controls.Add(this.btnSourceMdbFolder);
            this.viewSourceMdb.Location = new System.Drawing.Point(4, 22);
            this.viewSourceMdb.Name = "viewSourceMdb";
            this.viewSourceMdb.Padding = new System.Windows.Forms.Padding(3);
            this.viewSourceMdb.Size = new System.Drawing.Size(667, 358);
            this.viewSourceMdb.TabIndex = 3;
            this.viewSourceMdb.Text = "Source (.MDB)";
            this.viewSourceMdb.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // btnSourceMdbNew
            // 
            this.btnSourceMdbNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSourceMdbNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSourceMdbNew.Location = new System.Drawing.Point(586, 184);
            this.btnSourceMdbNew.Name = "btnSourceMdbNew";
            this.btnSourceMdbNew.Size = new System.Drawing.Size(60, 23);
            this.btnSourceMdbNew.TabIndex = 8;
            this.btnSourceMdbNew.Text = "Create";
            this.btnSourceMdbNew.UseVisualStyleBackColor = true;
            this.btnSourceMdbNew.Click += new System.EventHandler(this.btnSourceMdbNew_Click);
            // 
            // tbSourceMdbNewTitle
            // 
            this.tbSourceMdbNewTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSourceMdbNewTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSourceMdbNewTitle.Location = new System.Drawing.Point(209, 187);
            this.tbSourceMdbNewTitle.Name = "tbSourceMdbNewTitle";
            this.tbSourceMdbNewTitle.Size = new System.Drawing.Size(365, 20);
            this.tbSourceMdbNewTitle.TabIndex = 7;
            // 
            // lblSourceMdbNewTitle
            // 
            this.lblSourceMdbNewTitle.AutoSize = true;
            this.lblSourceMdbNewTitle.Location = new System.Drawing.Point(70, 189);
            this.lblSourceMdbNewTitle.Name = "lblSourceMdbNewTitle";
            this.lblSourceMdbNewTitle.Size = new System.Drawing.Size(133, 13);
            this.lblSourceMdbNewTitle.TabIndex = 6;
            this.lblSourceMdbNewTitle.Text = "Title of new Database File:";
            // 
            // lblTabCaptionSourceMdb
            // 
            this.lblTabCaptionSourceMdb.AutoSize = true;
            this.lblTabCaptionSourceMdb.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTabCaptionSourceMdb.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblTabCaptionSourceMdb.Location = new System.Drawing.Point(68, 36);
            this.lblTabCaptionSourceMdb.Name = "lblTabCaptionSourceMdb";
            this.lblTabCaptionSourceMdb.Size = new System.Drawing.Size(320, 25);
            this.lblTabCaptionSourceMdb.TabIndex = 5;
            this.lblTabCaptionSourceMdb.Text = "Select Folder and Database File";
            // 
            // lbSourceMdbFolder
            // 
            this.lbSourceMdbFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSourceMdbFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSourceMdbFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbSourceMdbFolder.FormattingEnabled = true;
            this.lbSourceMdbFolder.ItemHeight = 16;
            this.lbSourceMdbFolder.Location = new System.Drawing.Point(73, 245);
            this.lbSourceMdbFolder.Name = "lbSourceMdbFolder";
            this.lbSourceMdbFolder.Size = new System.Drawing.Size(573, 98);
            this.lbSourceMdbFolder.TabIndex = 4;
            this.lbSourceMdbFolder.DoubleClick += new System.EventHandler(this.lbSourceMdbFolder_DoubleClick);
            // 
            // tbSourceMdbFolder
            // 
            this.tbSourceMdbFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSourceMdbFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSourceMdbFolder.Location = new System.Drawing.Point(73, 140);
            this.tbSourceMdbFolder.Name = "tbSourceMdbFolder";
            this.tbSourceMdbFolder.Size = new System.Drawing.Size(573, 20);
            this.tbSourceMdbFolder.TabIndex = 3;
            this.tbSourceMdbFolder.TextChanged += new System.EventHandler(this.tbSourceMdbFolder_TextChanged);
            // 
            // lblSourceMdbFolderList
            // 
            this.lblSourceMdbFolderList.AutoSize = true;
            this.lblSourceMdbFolderList.Location = new System.Drawing.Point(70, 229);
            this.lblSourceMdbFolderList.Name = "lblSourceMdbFolderList";
            this.lblSourceMdbFolderList.Size = new System.Drawing.Size(356, 13);
            this.lblSourceMdbFolderList.TabIndex = 2;
            this.lblSourceMdbFolderList.Text = "List of available Microsoft Access Database (.MDB) files in selected folder.";
            // 
            // lblSourceMdbFolder
            // 
            this.lblSourceMdbFolder.AutoSize = true;
            this.lblSourceMdbFolder.Location = new System.Drawing.Point(235, 116);
            this.lblSourceMdbFolder.Name = "lblSourceMdbFolder";
            this.lblSourceMdbFolder.Size = new System.Drawing.Size(365, 13);
            this.lblSourceMdbFolder.TabIndex = 1;
            this.lblSourceMdbFolder.Text = "Select folder containing Disk Map files (Microsoft Access Database (.MDB)).";
            // 
            // btnSourceMdbFolder
            // 
            this.btnSourceMdbFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSourceMdbFolder.Location = new System.Drawing.Point(73, 111);
            this.btnSourceMdbFolder.Name = "btnSourceMdbFolder";
            this.btnSourceMdbFolder.Size = new System.Drawing.Size(132, 23);
            this.btnSourceMdbFolder.TabIndex = 0;
            this.btnSourceMdbFolder.Text = "Select Folder";
            this.btnSourceMdbFolder.UseVisualStyleBackColor = true;
            this.btnSourceMdbFolder.Click += new System.EventHandler(this.btnSourceMdbFolder_Click);
            // 
            // viewTasks
            // 
            this.viewTasks.BackColor = System.Drawing.SystemColors.Window;
            this.viewTasks.Controls.Add(this.lblTabCaptionTasks);
            this.viewTasks.Controls.Add(this.lvTaskList);
            this.viewTasks.Controls.Add(this.lblTaskList);
            this.viewTasks.Location = new System.Drawing.Point(4, 22);
            this.viewTasks.Name = "viewTasks";
            this.viewTasks.Padding = new System.Windows.Forms.Padding(3);
            this.viewTasks.Size = new System.Drawing.Size(667, 358);
            this.viewTasks.TabIndex = 4;
            this.viewTasks.Text = "Tasks";
            this.viewTasks.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // lblTabCaptionTasks
            // 
            this.lblTabCaptionTasks.AutoSize = true;
            this.lblTabCaptionTasks.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTabCaptionTasks.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblTabCaptionTasks.Location = new System.Drawing.Point(68, 36);
            this.lblTabCaptionTasks.Name = "lblTabCaptionTasks";
            this.lblTabCaptionTasks.Size = new System.Drawing.Size(396, 25);
            this.lblTabCaptionTasks.TabIndex = 6;
            this.lblTabCaptionTasks.Text = "Select a Disk Map Task recorded earlier";
            // 
            // lvTaskList
            // 
            this.lvTaskList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTaskList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvTaskList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lvTaskList.FullRowSelect = true;
            this.lvTaskList.Location = new System.Drawing.Point(76, 132);
            this.lvTaskList.Name = "lvTaskList";
            this.lvTaskList.Size = new System.Drawing.Size(564, 210);
            this.lvTaskList.TabIndex = 1;
            this.lvTaskList.UseCompatibleStateImageBehavior = false;
            this.lvTaskList.View = System.Windows.Forms.View.Details;
            this.lvTaskList.DoubleClick += new System.EventHandler(this.lvTaskList_DoubleClick);
            // 
            // lblTaskList
            // 
            this.lblTaskList.AutoSize = true;
            this.lblTaskList.Location = new System.Drawing.Point(73, 116);
            this.lblTaskList.Name = "lblTaskList";
            this.lblTaskList.Size = new System.Drawing.Size(200, 13);
            this.lblTaskList.TabIndex = 0;
            this.lblTaskList.Text = "Select a Disk Map Task in the database.";
            // 
            // viewCompare
            // 
            this.viewCompare.BackColor = System.Drawing.SystemColors.Window;
            this.viewCompare.Controls.Add(this.lblDiffClrEquHnd);
            this.viewCompare.Controls.Add(this.lblDiffClrMissOtherHdn);
            this.viewCompare.Controls.Add(this.lblDiffClrMissOther);
            this.viewCompare.Controls.Add(this.lblDiffClrMissOneHdn);
            this.viewCompare.Controls.Add(this.lblDiffClrMissOne);
            this.viewCompare.Controls.Add(this.lblDiffClrDiffOther);
            this.viewCompare.Controls.Add(this.lblDiffClrDiffOne);
            this.viewCompare.Controls.Add(this.lblDiffClrEqu);
            this.viewCompare.Controls.Add(this.lblDiffClrs);
            this.viewCompare.Controls.Add(this.lblDiffClrUnk);
            this.viewCompare.Controls.Add(this.lvCompareTask);
            this.viewCompare.Controls.Add(this.lvCompare);
            this.viewCompare.Location = new System.Drawing.Point(4, 22);
            this.viewCompare.Name = "viewCompare";
            this.viewCompare.Padding = new System.Windows.Forms.Padding(3);
            this.viewCompare.Size = new System.Drawing.Size(667, 358);
            this.viewCompare.TabIndex = 0;
            this.viewCompare.Text = "Compare";
            this.viewCompare.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // lblDiffClrEquHnd
            // 
            this.lblDiffClrEquHnd.AutoSize = true;
            this.lblDiffClrEquHnd.BackColor = System.Drawing.Color.Turquoise;
            this.lblDiffClrEquHnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrEquHnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrEquHnd.Location = new System.Drawing.Point(92, 141);
            this.lblDiffClrEquHnd.Name = "lblDiffClrEquHnd";
            this.lblDiffClrEquHnd.Size = new System.Drawing.Size(66, 15);
            this.lblDiffClrEquHnd.TabIndex = 12;
            this.lblDiffClrEquHnd.Text = "Equ. HDN";
            // 
            // lblDiffClrMissOtherHdn
            // 
            this.lblDiffClrMissOtherHdn.AutoSize = true;
            this.lblDiffClrMissOtherHdn.BackColor = System.Drawing.Color.Red;
            this.lblDiffClrMissOtherHdn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrMissOtherHdn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrMissOtherHdn.Location = new System.Drawing.Point(485, 141);
            this.lblDiffClrMissOtherHdn.Name = "lblDiffClrMissOtherHdn";
            this.lblDiffClrMissOtherHdn.Size = new System.Drawing.Size(80, 15);
            this.lblDiffClrMissOtherHdn.TabIndex = 11;
            this.lblDiffClrMissOtherHdn.Text = "Miss. 2 HDN";
            // 
            // lblDiffClrMissOther
            // 
            this.lblDiffClrMissOther.AutoSize = true;
            this.lblDiffClrMissOther.BackColor = System.Drawing.Color.LightPink;
            this.lblDiffClrMissOther.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrMissOther.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrMissOther.Location = new System.Drawing.Point(430, 141);
            this.lblDiffClrMissOther.Name = "lblDiffClrMissOther";
            this.lblDiffClrMissOther.Size = new System.Drawing.Size(49, 15);
            this.lblDiffClrMissOther.TabIndex = 10;
            this.lblDiffClrMissOther.Text = "Miss. 2";
            // 
            // lblDiffClrMissOneHdn
            // 
            this.lblDiffClrMissOneHdn.AutoSize = true;
            this.lblDiffClrMissOneHdn.BackColor = System.Drawing.Color.Goldenrod;
            this.lblDiffClrMissOneHdn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrMissOneHdn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrMissOneHdn.Location = new System.Drawing.Point(335, 141);
            this.lblDiffClrMissOneHdn.Name = "lblDiffClrMissOneHdn";
            this.lblDiffClrMissOneHdn.Size = new System.Drawing.Size(80, 15);
            this.lblDiffClrMissOneHdn.TabIndex = 9;
            this.lblDiffClrMissOneHdn.Text = "Miss. 1 HDN";
            // 
            // lblDiffClrMissOne
            // 
            this.lblDiffClrMissOne.AutoSize = true;
            this.lblDiffClrMissOne.BackColor = System.Drawing.Color.Orange;
            this.lblDiffClrMissOne.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrMissOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrMissOne.Location = new System.Drawing.Point(280, 141);
            this.lblDiffClrMissOne.Name = "lblDiffClrMissOne";
            this.lblDiffClrMissOne.Size = new System.Drawing.Size(49, 15);
            this.lblDiffClrMissOne.TabIndex = 8;
            this.lblDiffClrMissOne.Text = "Miss. 1";
            // 
            // lblDiffClrDiffOther
            // 
            this.lblDiffClrDiffOther.AutoSize = true;
            this.lblDiffClrDiffOther.BackColor = System.Drawing.Color.LightBlue;
            this.lblDiffClrDiffOther.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrDiffOther.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrDiffOther.Location = new System.Drawing.Point(221, 141);
            this.lblDiffClrDiffOther.Name = "lblDiffClrDiffOther";
            this.lblDiffClrDiffOther.Size = new System.Drawing.Size(44, 15);
            this.lblDiffClrDiffOther.TabIndex = 7;
            this.lblDiffClrDiffOther.Text = "Diff. 2";
            // 
            // lblDiffClrDiffOne
            // 
            this.lblDiffClrDiffOne.AutoSize = true;
            this.lblDiffClrDiffOne.BackColor = System.Drawing.Color.PaleTurquoise;
            this.lblDiffClrDiffOne.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrDiffOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrDiffOne.Location = new System.Drawing.Point(171, 141);
            this.lblDiffClrDiffOne.Name = "lblDiffClrDiffOne";
            this.lblDiffClrDiffOne.Size = new System.Drawing.Size(44, 15);
            this.lblDiffClrDiffOne.TabIndex = 6;
            this.lblDiffClrDiffOne.Text = "Diff. 1";
            // 
            // lblDiffClrEqu
            // 
            this.lblDiffClrEqu.AutoSize = true;
            this.lblDiffClrEqu.BackColor = System.Drawing.Color.LightGreen;
            this.lblDiffClrEqu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrEqu.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrEqu.Location = new System.Drawing.Point(51, 141);
            this.lblDiffClrEqu.Name = "lblDiffClrEqu";
            this.lblDiffClrEqu.Size = new System.Drawing.Size(35, 15);
            this.lblDiffClrEqu.TabIndex = 5;
            this.lblDiffClrEqu.Text = "Equ.";
            // 
            // lblDiffClrs
            // 
            this.lblDiffClrs.AutoSize = true;
            this.lblDiffClrs.Location = new System.Drawing.Point(6, 141);
            this.lblDiffClrs.Name = "lblDiffClrs";
            this.lblDiffClrs.Size = new System.Drawing.Size(39, 13);
            this.lblDiffClrs.TabIndex = 4;
            this.lblDiffClrs.Text = "Colors:";
            // 
            // lblDiffClrUnk
            // 
            this.lblDiffClrUnk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDiffClrUnk.AutoSize = true;
            this.lblDiffClrUnk.BackColor = System.Drawing.Color.Yellow;
            this.lblDiffClrUnk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiffClrUnk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDiffClrUnk.Location = new System.Drawing.Point(580, 141);
            this.lblDiffClrUnk.Name = "lblDiffClrUnk";
            this.lblDiffClrUnk.Size = new System.Drawing.Size(78, 15);
            this.lblDiffClrUnk.TabIndex = 3;
            this.lblDiffClrUnk.Text = "Unk. (ERR.)";
            // 
            // lvCompareTask
            // 
            this.lvCompareTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvCompareTask.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvCompareTask.FullRowSelect = true;
            this.lvCompareTask.Location = new System.Drawing.Point(6, 6);
            this.lvCompareTask.Name = "lvCompareTask";
            this.lvCompareTask.Size = new System.Drawing.Size(652, 132);
            this.lvCompareTask.TabIndex = 1;
            this.lvCompareTask.UseCompatibleStateImageBehavior = false;
            this.lvCompareTask.View = System.Windows.Forms.View.Details;
            this.lvCompareTask.DoubleClick += new System.EventHandler(this.lvCompareTask_DoubleClick);
            // 
            // lvCompare
            // 
            this.lvCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvCompare.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvCompare.FullRowSelect = true;
            this.lvCompare.Location = new System.Drawing.Point(6, 159);
            this.lvCompare.Name = "lvCompare";
            this.lvCompare.Size = new System.Drawing.Size(652, 197);
            this.lvCompare.TabIndex = 0;
            this.lvCompare.UseCompatibleStateImageBehavior = false;
            this.lvCompare.View = System.Windows.Forms.View.Details;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Location = new System.Drawing.Point(705, 114);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCaption.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblCaption.Location = new System.Drawing.Point(19, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(176, 25);
            this.lblCaption.TabIndex = 2;
            this.lblCaption.Text = "Win Disk Size Ex";
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Location = new System.Drawing.Point(705, 56);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Location = new System.Drawing.Point(705, 85);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblViewStack
            // 
            this.lblViewStack.AutoSize = true;
            this.lblViewStack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblViewStack.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblViewStack.Location = new System.Drawing.Point(21, 36);
            this.lblViewStack.Name = "lblViewStack";
            this.lblViewStack.Size = new System.Drawing.Size(30, 13);
            this.lblViewStack.TabIndex = 5;
            this.lblViewStack.Text = "N/A";
            // 
            // prsMain
            // 
            this.prsMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prsMain.BackColor = System.Drawing.SystemColors.Info;
            this.prsMain.Location = new System.Drawing.Point(314, 26);
            this.prsMain.Name = "prsMain";
            this.prsMain.Size = new System.Drawing.Size(373, 23);
            this.prsMain.TabIndex = 6;
            this.prsMain.Visible = false;
            // 
            // btnCompareWith
            // 
            this.btnCompareWith.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompareWith.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCompareWith.Location = new System.Drawing.Point(705, 157);
            this.btnCompareWith.Name = "btnCompareWith";
            this.btnCompareWith.Size = new System.Drawing.Size(75, 45);
            this.btnCompareWith.TabIndex = 7;
            this.btnCompareWith.Text = "Compare with...";
            this.btnCompareWith.UseVisualStyleBackColor = true;
            this.btnCompareWith.Click += new System.EventHandler(this.btnCompareWith_Click);
            // 
            // lblPrsMain
            // 
            this.lblPrsMain.AutoSize = true;
            this.lblPrsMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblPrsMain.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblPrsMain.Location = new System.Drawing.Point(311, 9);
            this.lblPrsMain.Name = "lblPrsMain";
            this.lblPrsMain.Size = new System.Drawing.Size(30, 13);
            this.lblPrsMain.TabIndex = 8;
            this.lblPrsMain.Text = "N/A";
            this.lblPrsMain.Visible = false;
            // 
            // btnHome
            // 
            this.btnHome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Location = new System.Drawing.Point(705, 9);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(75, 40);
            this.btnHome.TabIndex = 9;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnAddToReport
            // 
            this.btnAddToReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddToReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddToReport.Location = new System.Drawing.Point(705, 223);
            this.btnAddToReport.Name = "btnAddToReport";
            this.btnAddToReport.Size = new System.Drawing.Size(75, 45);
            this.btnAddToReport.TabIndex = 10;
            this.btnAddToReport.Text = "Add to Report...";
            this.btnAddToReport.UseVisualStyleBackColor = true;
            this.btnAddToReport.Click += new System.EventHandler(this.btnAddToReport_Click);
            // 
            // btnOpenUpdatedReport
            // 
            this.btnOpenUpdatedReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenUpdatedReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenUpdatedReport.Location = new System.Drawing.Point(705, 274);
            this.btnOpenUpdatedReport.Name = "btnOpenUpdatedReport";
            this.btnOpenUpdatedReport.Size = new System.Drawing.Size(75, 57);
            this.btnOpenUpdatedReport.TabIndex = 11;
            this.btnOpenUpdatedReport.Text = "Open Updated Report...";
            this.btnOpenUpdatedReport.UseVisualStyleBackColor = true;
            this.btnOpenUpdatedReport.Click += new System.EventHandler(this.btnOpenUpdatedReport_Click);
            // 
            // btnPrintReport
            // 
            this.btnPrintReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrintReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrintReport.Location = new System.Drawing.Point(705, 352);
            this.btnPrintReport.Name = "btnPrintReport";
            this.btnPrintReport.Size = new System.Drawing.Size(75, 69);
            this.btnPrintReport.TabIndex = 12;
            this.btnPrintReport.Text = "Print Report (generated .RTF file)...";
            this.btnPrintReport.UseVisualStyleBackColor = true;
            this.btnPrintReport.Click += new System.EventHandler(this.btnPrintReport_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(792, 473);
            this.Controls.Add(this.btnPrintReport);
            this.Controls.Add(this.btnOpenUpdatedReport);
            this.Controls.Add(this.btnAddToReport);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.lblPrsMain);
            this.Controls.Add(this.btnCompareWith);
            this.Controls.Add(this.prsMain);
            this.Controls.Add(this.lblViewStack);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.Views);
            this.Name = "FormMain";
            this.Text = "Win Disk Size Ex";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.Views.ResumeLayout(false);
            this.viewMain.ResumeLayout(false);
            this.viewMain.PerformLayout();
            this.viewSource.ResumeLayout(false);
            this.viewSource.PerformLayout();
            this.viewSourceMdb.ResumeLayout(false);
            this.viewSourceMdb.PerformLayout();
            this.viewTasks.ResumeLayout(false);
            this.viewTasks.PerformLayout();
            this.viewCompare.ResumeLayout(false);
            this.viewCompare.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl Views;
        private System.Windows.Forms.TabPage viewCompare;
        private System.Windows.Forms.TabPage viewMain;
        private System.Windows.Forms.ListView lvCompare;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label lblMainOpen;
        private System.Windows.Forms.Button btnMainOpen;
        private System.Windows.Forms.TabPage viewSource;
        private System.Windows.Forms.Label lblSourceMdb;
        private System.Windows.Forms.Button btnSourceMdb;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.TabPage viewSourceMdb;
        private System.Windows.Forms.Label lblSourceMdbFolder;
        private System.Windows.Forms.Button btnSourceMdbFolder;
        private System.Windows.Forms.TextBox tbSourceMdbFolder;
        private System.Windows.Forms.Label lblSourceMdbFolderList;
        private System.Windows.Forms.ListBox lbSourceMdbFolder;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label lblViewStack;
        private System.Windows.Forms.ProgressBar prsMain;
        private System.Windows.Forms.TabPage viewTasks;
        private System.Windows.Forms.ListView lvTaskList;
        private System.Windows.Forms.Label lblTaskList;
        private System.Windows.Forms.ListView lvCompareTask;
        private System.Windows.Forms.Button btnCompareWith;
        private System.Windows.Forms.Label lblPrsMain;
        private System.Windows.Forms.Label lblDiffClrUnk;
        private System.Windows.Forms.Label lblDiffClrEqu;
        private System.Windows.Forms.Label lblDiffClrs;
        private System.Windows.Forms.Label lblDiffClrDiffOne;
        private System.Windows.Forms.Label lblDiffClrDiffOther;
        private System.Windows.Forms.Label lblDiffClrMissOtherHdn;
        private System.Windows.Forms.Label lblDiffClrMissOther;
        private System.Windows.Forms.Label lblDiffClrMissOneHdn;
        private System.Windows.Forms.Label lblDiffClrMissOne;
        private System.Windows.Forms.Label lblDiffClrEquHnd;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Label lblTabCaptionMain;
        private System.Windows.Forms.Label lblTabCaptionSource;
        private System.Windows.Forms.Label lblTabCaptionSourceMdb;
        private System.Windows.Forms.Label lblTabCaptionTasks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMainOpenReport;
        private System.Windows.Forms.Button btnAddToReport;
        private System.Windows.Forms.Button btnSourceMdbNew;
        private System.Windows.Forms.TextBox tbSourceMdbNewTitle;
        private System.Windows.Forms.Label lblSourceMdbNewTitle;
        private System.Windows.Forms.Button btnOpenUpdatedReport;
        private System.Windows.Forms.Button btnPrintReport;
    }
}

