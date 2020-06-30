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
            this.lblMainOpen = new System.Windows.Forms.Label();
            this.btnMainOpen = new System.Windows.Forms.Button();
            this.viewSource = new System.Windows.Forms.TabPage();
            this.lblSourceMdb = new System.Windows.Forms.Label();
            this.btnSourceMdb = new System.Windows.Forms.Button();
            this.viewSourceMdb = new System.Windows.Forms.TabPage();
            this.lbSourceMdbFolder = new System.Windows.Forms.ListBox();
            this.tbSourceMdbFolder = new System.Windows.Forms.TextBox();
            this.lblSourceMdbFolderList = new System.Windows.Forms.Label();
            this.lblSourceMdbFolder = new System.Windows.Forms.Label();
            this.btnSourceMdbFolder = new System.Windows.Forms.Button();
            this.viewTasks = new System.Windows.Forms.TabPage();
            this.lvTaskList = new System.Windows.Forms.ListView();
            this.lblTaskList = new System.Windows.Forms.Label();
            this.viewCompare = new System.Windows.Forms.TabPage();
            this.lvCompareTask = new System.Windows.Forms.ListView();
            this.lvCompare = new System.Windows.Forms.ListView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.lblViewStack = new System.Windows.Forms.Label();
            this.prsMain = new System.Windows.Forms.ProgressBar();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblPrsMain = new System.Windows.Forms.Label();
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
            this.Views.Size = new System.Drawing.Size(672, 388);
            this.Views.TabIndex = 0;
            // 
            // viewMain
            // 
            this.viewMain.BackColor = System.Drawing.SystemColors.Window;
            this.viewMain.Controls.Add(this.lblMainOpen);
            this.viewMain.Controls.Add(this.btnMainOpen);
            this.viewMain.Location = new System.Drawing.Point(4, 22);
            this.viewMain.Name = "viewMain";
            this.viewMain.Padding = new System.Windows.Forms.Padding(3);
            this.viewMain.Size = new System.Drawing.Size(664, 362);
            this.viewMain.TabIndex = 1;
            this.viewMain.Text = "Main";
            this.viewMain.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // lblMainOpen
            // 
            this.lblMainOpen.AutoSize = true;
            this.lblMainOpen.Location = new System.Drawing.Point(172, 116);
            this.lblMainOpen.Name = "lblMainOpen";
            this.lblMainOpen.Size = new System.Drawing.Size(147, 13);
            this.lblMainOpen.TabIndex = 1;
            this.lblMainOpen.Text = "Open an existing folder listing.";
            // 
            // btnMainOpen
            // 
            this.btnMainOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMainOpen.Location = new System.Drawing.Point(73, 111);
            this.btnMainOpen.Name = "btnMainOpen";
            this.btnMainOpen.Size = new System.Drawing.Size(75, 23);
            this.btnMainOpen.TabIndex = 0;
            this.btnMainOpen.Text = "Open";
            this.btnMainOpen.UseVisualStyleBackColor = true;
            this.btnMainOpen.Click += new System.EventHandler(this.btnMainOpen_Click);
            // 
            // viewSource
            // 
            this.viewSource.BackColor = System.Drawing.SystemColors.Window;
            this.viewSource.Controls.Add(this.lblSourceMdb);
            this.viewSource.Controls.Add(this.btnSourceMdb);
            this.viewSource.Location = new System.Drawing.Point(4, 22);
            this.viewSource.Name = "viewSource";
            this.viewSource.Padding = new System.Windows.Forms.Padding(3);
            this.viewSource.Size = new System.Drawing.Size(664, 362);
            this.viewSource.TabIndex = 2;
            this.viewSource.Text = "Source";
            this.viewSource.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // lblSourceMdb
            // 
            this.lblSourceMdb.AutoSize = true;
            this.lblSourceMdb.Location = new System.Drawing.Point(172, 116);
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
            this.btnSourceMdb.Size = new System.Drawing.Size(75, 23);
            this.btnSourceMdb.TabIndex = 0;
            this.btnSourceMdb.Text = "MDB";
            this.btnSourceMdb.UseVisualStyleBackColor = true;
            this.btnSourceMdb.Click += new System.EventHandler(this.btnSourceMdb_Click);
            // 
            // viewSourceMdb
            // 
            this.viewSourceMdb.BackColor = System.Drawing.SystemColors.Window;
            this.viewSourceMdb.Controls.Add(this.lbSourceMdbFolder);
            this.viewSourceMdb.Controls.Add(this.tbSourceMdbFolder);
            this.viewSourceMdb.Controls.Add(this.lblSourceMdbFolderList);
            this.viewSourceMdb.Controls.Add(this.lblSourceMdbFolder);
            this.viewSourceMdb.Controls.Add(this.btnSourceMdbFolder);
            this.viewSourceMdb.Location = new System.Drawing.Point(4, 22);
            this.viewSourceMdb.Name = "viewSourceMdb";
            this.viewSourceMdb.Padding = new System.Windows.Forms.Padding(3);
            this.viewSourceMdb.Size = new System.Drawing.Size(664, 362);
            this.viewSourceMdb.TabIndex = 3;
            this.viewSourceMdb.Text = "Source (.MDB)";
            this.viewSourceMdb.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // lbSourceMdbFolder
            // 
            this.lbSourceMdbFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSourceMdbFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSourceMdbFolder.FormattingEnabled = true;
            this.lbSourceMdbFolder.Location = new System.Drawing.Point(73, 194);
            this.lbSourceMdbFolder.Name = "lbSourceMdbFolder";
            this.lbSourceMdbFolder.Size = new System.Drawing.Size(573, 145);
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
            this.lblSourceMdbFolderList.Location = new System.Drawing.Point(70, 178);
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
            this.lblSourceMdbFolder.Size = new System.Drawing.Size(386, 13);
            this.lblSourceMdbFolder.TabIndex = 1;
            this.lblSourceMdbFolder.Text = "Select folder containing Win Disk Size files (Microsoft Access Database (.MDB)).";
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
            this.viewTasks.Controls.Add(this.lvTaskList);
            this.viewTasks.Controls.Add(this.lblTaskList);
            this.viewTasks.Location = new System.Drawing.Point(4, 22);
            this.viewTasks.Name = "viewTasks";
            this.viewTasks.Padding = new System.Windows.Forms.Padding(3);
            this.viewTasks.Size = new System.Drawing.Size(664, 362);
            this.viewTasks.TabIndex = 4;
            this.viewTasks.Text = "Tasks";
            this.viewTasks.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // lvTaskList
            // 
            this.lvTaskList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTaskList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            this.lblTaskList.Size = new System.Drawing.Size(152, 13);
            this.lblTaskList.TabIndex = 0;
            this.lblTaskList.Text = "Select a Task in the database.";
            // 
            // viewCompare
            // 
            this.viewCompare.BackColor = System.Drawing.SystemColors.Window;
            this.viewCompare.Controls.Add(this.lvCompareTask);
            this.viewCompare.Controls.Add(this.lvCompare);
            this.viewCompare.Location = new System.Drawing.Point(4, 22);
            this.viewCompare.Name = "viewCompare";
            this.viewCompare.Padding = new System.Windows.Forms.Padding(3);
            this.viewCompare.Size = new System.Drawing.Size(664, 362);
            this.viewCompare.TabIndex = 0;
            this.viewCompare.Text = "Compare";
            this.viewCompare.Enter += new System.EventHandler(this.viewAny_Enter);
            // 
            // lvCompareTask
            // 
            this.lvCompareTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvCompareTask.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvCompareTask.FullRowSelect = true;
            this.lvCompareTask.Location = new System.Drawing.Point(6, 6);
            this.lvCompareTask.Name = "lvCompareTask";
            this.lvCompareTask.Size = new System.Drawing.Size(652, 84);
            this.lvCompareTask.TabIndex = 1;
            this.lvCompareTask.UseCompatibleStateImageBehavior = false;
            this.lvCompareTask.View = System.Windows.Forms.View.Details;
            // 
            // lvCompare
            // 
            this.lvCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvCompare.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvCompare.FullRowSelect = true;
            this.lvCompare.Location = new System.Drawing.Point(6, 96);
            this.lvCompare.Name = "lvCompare";
            this.lvCompare.Size = new System.Drawing.Size(652, 260);
            this.lvCompare.TabIndex = 0;
            this.lvCompare.UseCompatibleStateImageBehavior = false;
            this.lvCompare.View = System.Windows.Forms.View.Details;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Location = new System.Drawing.Point(702, 99);
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
            this.btnBack.Location = new System.Drawing.Point(702, 13);
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
            this.btnNext.Location = new System.Drawing.Point(702, 56);
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
            this.prsMain.Size = new System.Drawing.Size(370, 23);
            this.prsMain.TabIndex = 6;
            this.prsMain.Visible = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(702, 142);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(789, 477);
            this.Controls.Add(this.lblPrsMain);
            this.Controls.Add(this.btnAdd);
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
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblPrsMain;
    }
}

