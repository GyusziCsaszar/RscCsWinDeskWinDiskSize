using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Reflection;
using Microsoft.Win32;

namespace WinDiskSizeEx
{
    public partial class FormMain : Form
    {

        protected const string csAPP_TITLE = "Win Disk Size Ex v1.15";

        protected const string csVIEW_TITLE_COMPARE = "Disk Map";
        protected const string csVIEW_TITLE_REPORT  = "Disk Map Report";

        protected const string csMAX_LEVEL_TITLE = "Max Level: ";

        protected const int ciCOMPARE_COL_SIZE  = 2;
        protected const int ciCOMPARE_COL_COUNT = 6;

        protected const int ciTASK_STATUS_PLANNED       = 1;
        protected const int ciTASK_STATUS_STARTED       = 2;
        protected const int ciTASK_STATUS_COMPLETED     = 3;
        protected const int ciTASK_STATUS_REPORT        = 4;

        protected const String csMDB_TEMPLATE   = "WinDiskSize_Template.mdb";

        protected const String csMDB_MAP_PREFIX = "WinDiskSizeMap";
        protected const String csMDB_RPT_PREFIX = "WinDiskSizeRpt";

        protected const String csARG_TAG_DO             = "DO";
        protected const String csARG_TAG_OPEN           = "OPEN";
        protected const String csARG_TAG_SAVE           = "SAVE";
        protected const String csARG_TAG_MAP            = "MAP";
        protected const String csARG_TAG_REPORT         = "RPT";
        protected const String csARG_TAG_DB             = "DB";
        protected const String csARG_TAG_DB_MDB         = "MDB";
        protected const String csARG_TAG_DB_SQLSERVER   = "SQLSERVER";
        protected const String csARG_TAG_TASK           = "Task";

        protected List<TabPage> m_ViewStack     = new List<TabPage>();

        protected List<String>  m_InterTabArgs  = new List<String>();

        protected List<MyTask>  m_Tasks         = new List<MyTask>();

        StatusBar mainStatusBar;
        StatusBarPanel sbPanelLb;
        StatusBarPanel sbPanelLevel;

        protected int m_iTickCountLast;

        protected int m_iSourceOffset = 0;

        // Settings
        protected bool m_bSetting_CompareFileDateTime           = false;
        protected bool m_bSetting_CompareFileDateOnly           = false;
        protected bool m_bSetting_HideChildrenOfMissingFolder   = true;
        protected bool m_bSetting_HideChildrenOfEqualFolder     = true;
        protected bool m_bSetting_DoubleCheck                   = true;
        protected int  m_iFreez                                 = 500;  // Milliseconds

        public FormMain()
        {
            InitializeComponent();

            this.Text = csAPP_TITLE;
            lblCaption.Text = csAPP_TITLE;

            // SRC: https://stackoverflow.com/questions/552579/how-to-hide-tabpage-from-tabcontrol
            Views.Appearance = TabAppearance.FlatButtons;
            Views.ItemSize = new Size(0, 1);
            Views.SizeMode = TabSizeMode.Fixed;

            mainStatusBar = new StatusBar();
            //
            sbPanelLb = new StatusBarPanel();
            sbPanelLb.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            sbPanelLb.Text = "Folder Count: N/A";
            sbPanelLb.ToolTipText = "ListBox's item count";
            sbPanelLb.AutoSize = StatusBarPanelAutoSize.Spring;
            mainStatusBar.Panels.Add(sbPanelLb);
            //
            sbPanelLevel = new StatusBarPanel();
            sbPanelLevel.BorderStyle = StatusBarPanelBorderStyle.Sunken; // StatusBarPanelBorderStyle.Raised;
            sbPanelLevel.Text = csMAX_LEVEL_TITLE + "N/A";
            sbPanelLevel.ToolTipText = "Maximum level (depth) parsed";
            sbPanelLevel.AutoSize = StatusBarPanelAutoSize.Contents;
            mainStatusBar.Panels.Add(sbPanelLevel);
            //
            mainStatusBar.ShowPanels = true;
            Controls.Add(mainStatusBar);

            // SRC: https://stackoverflow.com/questions/13625103/how-can-i-stop-form-flickering-when-using-tabcontrol-tabpages0-controls-add
            /*
            Views.SetStyle(ControlStyles.DoubleBuffer |
              ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint,
              true);
            */
            Views.GetType().InvokeMember(
                 "SetStyle",
                 BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.InvokeMethod,
                 null,
                 Views,
                 new object[] { ControlStyles.DoubleBuffer /* | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint */, true });
            /*
            lvCompare.GetType().InvokeMember(
                 "SetStyle",
                 BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                 null,
                 lvCompare,
                 new object[] { ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true });
            */
        }

        // SRC: https://stackoverflow.com/questions/13625103/how-can-i-stop-form-flickering-when-using-tabcontrol-tabpages0-controls-add
        /*
        // BUG: ListControl on TabPage has ScrollBar painting issues!!!
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        */

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.Left = Math.Max(0, RegistryRead("Main_Left", this.Left));
            this.Top = Math.Max(0, RegistryRead("Main_Top", this.Top));
            this.Width = Math.Max(800, RegistryRead("Main_Width", this.Width));
            this.Height = Math.Max(500, RegistryRead("Main_Height", this.Height));

            tbSourceSqlServerInstance.Text = RegistryRead("Last SQL Server", tbSourceSqlServerInstance.Text);
            tbSourceSqlServerDb.Text = RegistryRead("Last SQL Db", tbSourceSqlServerDb.Text);
            tbSourceSqlServerUser.Text = RegistryRead("Last SQL User", tbSourceSqlServerUser.Text);
            tbSourceSqlServerPw.Text = RegistryRead("Last SQL Pw", tbSourceSqlServerPw.Text);
            chbSourceSqlServerSavePw.Checked = (tbSourceSqlServerPw.Text.Length > 0);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (lblPrsMain.Visible)
            {
                e.Cancel = true;
                return; //In Progress...
            }

            if (this.Left >= 0) RegistryWrite("Main_Left", this.Left);
            if (this.Top >= 0) RegistryWrite("Main_Top", this.Top);
            if (this.Width >= 800) RegistryWrite("Main_Width", this.Width);
            if (this.Height >= 500) RegistryWrite("Main_Height", this.Height);

            int iCol;
            
            iCol = -1;
            foreach (ColumnHeader ch in lvTaskList.Columns)
            {
                iCol++;
                RegistryWrite("TaskCol" + iCol.ToString(), ch.Width);
            }

            iCol = -1;
            foreach (ColumnHeader ch in lvCompareTask.Columns)
            {
                iCol++;
                RegistryWrite("CompareTaskCol" + iCol.ToString(), ch.Width);
            }

            iCol = -1;
            foreach (ColumnHeader ch in lvCompare.Columns)
            {
                iCol++;
                RegistryWrite("CompareCol" + iCol.ToString(), ch.Width);
            }
        }

        private void RegistryWrite(string sName, string sValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ressive.Hu\\WinDiskSizeEx");
            key.SetValue(sName, sValue);
            key.Dispose();
        }

        private void RegistryWrite(string sName, int iValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ressive.Hu\\WinDiskSizeEx");
            key.SetValue(sName, iValue);
            key.Dispose();
        }

        private string RegistryRead(string sName, string sDefaultValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ressive.Hu\\WinDiskSizeEx");
            string sValue = (string) key.GetValue(sName, sDefaultValue);
            key.Dispose();

            return sValue;
        }

        private int RegistryRead(string sName, int iDefaultValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ressive.Hu\\WinDiskSizeEx");
            int iValue = (int) key.GetValue(sName, iDefaultValue);
            key.Dispose();

            return iValue;
        }

        private string InterTabArgumentsAsString(string sTagFrom)
        {
            string sRes = "";

            for (int i = m_InterTabArgs.Count - 1; i >= 0; i--)
            {
                if (sRes.Length > 0) sRes = ";" + sRes;
                sRes = m_InterTabArgs[i] + sRes;

                if (sTagFrom.Length > 0 && m_InterTabArgs[i] == sTagFrom)
                {
                    break;
                }
            }

            return sRes;
        }

        private void InterTabArgumentsReverseRemoveUntil(string sTagUntil, string sTagStop)
        {
            if (sTagUntil.Length == 0) return;

            bool bFound = false;
            for (int i = m_InterTabArgs.Count - 1; i >= 0; i--)
            {
                if (m_InterTabArgs[i] == sTagUntil)
                {
                    bFound = true;
                    break;
                }

                if (m_InterTabArgs[i] == sTagStop)
                {
                    return; //Not found after sTagStop
                }
            }

            if (!bFound)
            {
                return; //sTagUntil not found!!!
            }

            bool bBreak = false;
            for (int i = m_InterTabArgs.Count - 1; i >= 0; i--)
            {
                if (m_InterTabArgs[i] == sTagUntil)
                {
                    bBreak = true; ;
                }

                m_InterTabArgs.RemoveAt(i);

                if (bBreak)
                {
                    break;
                }
            }
        }

        private string InterTabArgumentsReverseByName(string sTagFrom, int iOffset)
        {
            for (int i = m_InterTabArgs.Count - 1; i >= 0; i--)
            {
                if (m_InterTabArgs[i] == sTagFrom)
                {
                    if (i + iOffset < m_InterTabArgs.Count)
                    {
                        return m_InterTabArgs[i + iOffset];
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            return "";
        }

        private MyDb ConnectToDb(string sInterTabArguments)
        {
            string[] asArgs = sInterTabArguments.Split(';');

            if (asArgs.Length < 3)
            {
                MessageBox.Show("Unexpected inter-Tab arguments \"" + sInterTabArguments + "\"!", csAPP_TITLE);
                return null;
            }

            if (asArgs[0] != csARG_TAG_DB)
            {
                MessageBox.Show("Unexpected inter-Tab arguments \"" + sInterTabArguments + "\"!", csAPP_TITLE);
                return null;
            }

            if (asArgs[1] == csARG_TAG_DB_MDB)
            {
                String sMdbTemplatePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                if (sMdbTemplatePath[sMdbTemplatePath.Length - 1] != '\\') sMdbTemplatePath += "\\";
                sMdbTemplatePath += csMDB_TEMPLATE;

                if (!System.IO.File.Exists(sMdbTemplatePath))
                {
                    MessageBox.Show("Template MDB Database \"" + sMdbTemplatePath + "\" does not exist!", csAPP_TITLE);
                    return null;
                }
                else
                {
                    MyMdb mdb = new MyMdb(sMdbTemplatePath);
                    mdb.Folder = System.IO.Path.GetDirectoryName(asArgs[2]);
                    mdb.MdbFile = System.IO.Path.GetFileName(asArgs[2]);

                    return mdb;
                }
            }
            else if (asArgs[1] == csARG_TAG_DB_SQLSERVER)
            {
                MySqlServer sqlsvr = new MySqlServer();
                if (!sqlsvr.TestConnect(tbSourceSqlServerInstance.Text, tbSourceSqlServerDb.Text, tbSourceSqlServerUser.Text, tbSourceSqlServerPw.Text))
                {
                    MessageBox.Show("SQL Server Error: " + sqlsvr.LastError, csAPP_TITLE);

                    sqlsvr.Close();

                    return null;
                }
                else
                {
                    return sqlsvr;
                }
            }
            else
            {
                MessageBox.Show("Unexpected source type \"" + asArgs[1] + "\"!", csAPP_TITLE);
                return null;
            }
        }

        private string GetSqlFileDate(string sFileDateTime, string sFileDateOnly)
        {
            string sSqlFileDate = "NULL";

            string sFileDate = null;
            if (m_bSetting_CompareFileDateTime)
            {
                sFileDate = sFileDateTime;
            }
            else if (m_bSetting_CompareFileDateOnly)
            {
                sFileDate = sFileDateOnly;
            }
            else
            {
                return "NULL";
            }

            if (sFileDate != null && sFileDate.Length > 0)
            {
                sSqlFileDate = "'" + sFileDate + "'";
            }

            return sSqlFileDate;
        }

        private void ListTasks()
        {
            lvTaskList.Items.Clear();
            lvTaskList.Columns.Clear();

            MyDb db = ConnectToDb(InterTabArgumentsAsString(csARG_TAG_DB));
            if (db == null)
            {
                return;
            }

            lvTaskList.Columns.Add("Task ID"); lvTaskList.Columns[lvTaskList.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
            lvTaskList.Columns.Add("Label");
            lvTaskList.Columns.Add("Total Size"); lvTaskList.Columns[lvTaskList.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
            lvTaskList.Columns.Add("Free Space"); lvTaskList.Columns[lvTaskList.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
            lvTaskList.Columns.Add("Machine");
            lvTaskList.Columns.Add("Status Number");
            lvTaskList.Columns.Add("Status");
            lvTaskList.Columns.Add("Started");
            lvTaskList.Columns.Add("Completed");
            lvTaskList.Columns.Add("Type");
            lvTaskList.Columns.Add("Root Folder");

            int iCol = -1;
            foreach (ColumnHeader ch in lvTaskList.Columns)
            {
                iCol++;
                int iWidth = RegistryRead("TaskCol" + iCol.ToString(), -1);
                if (iWidth > -1)
                {
                    ch.Width = iWidth;
                }
            }

            int iTaskStatusFilter = -1;
            string sType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);
            if (sType == csARG_TAG_REPORT)
            {
                iTaskStatusFilter = ciTASK_STATUS_REPORT;
            }

            if (!db.QueryTasks(iTaskStatusFilter, -1))
            {
                MessageBox.Show(db.LastError, csAPP_TITLE);
                return;
            }

            int iRowCount = db.RowCount();
            if (iRowCount < 0)
            {
                MessageBox.Show(db.LastError, csAPP_TITLE);
                return;
            }

            lvTaskList.BeginUpdate();

            for (int iRow = 0; iRow < iRowCount; iRow++)
            {
                int iTaskID = db.FieldAsInt(iRow, "ID");

                int iStatus = db.FieldAsInt(iRow, "Status");
                String sStatus = "<unknown>";
                switch (iStatus)
                {
                    case ciTASK_STATUS_PLANNED:
                        sStatus = "Planned";
                        break;
                    case ciTASK_STATUS_STARTED:
                        sStatus = "Started";
                        break;
                    case ciTASK_STATUS_COMPLETED:
                        sStatus = "Completed";
                        break;
                    case ciTASK_STATUS_REPORT:
                        sStatus = "REPORT";
                        break;
                }

                lvTaskList.Items.Add(iTaskID.ToString());

                lvTaskList.Items[iRow].SubItems.Add(db.FieldAsString(iRow, "Label"));

                string sTmp;
                sTmp = db.FieldAsString(iRow, "StorageSize");
                if (sTmp.Length > 0)
                {
                    Int64 i64 = 0;
                    if (Int64.TryParse(sTmp, out i64))
                    {
                        sTmp = MyFolder.ToShortSizeString(i64);
                    }
                }
                lvTaskList.Items[iRow].SubItems.Add(sTmp);

                sTmp = db.FieldAsString(iRow, "StorageFree");
                if (sTmp.Length > 0)
                {
                    Int64 i64 = 0;
                    if (Int64.TryParse(sTmp, out i64))
                    {
                        sTmp = MyFolder.ToShortSizeString(i64);
                    }
                }
                lvTaskList.Items[iRow].SubItems.Add(sTmp);

                lvTaskList.Items[iRow].SubItems.Add(db.FieldAsString(iRow, "Machine"));
                lvTaskList.Items[iRow].SubItems.Add(iStatus.ToString());
                lvTaskList.Items[iRow].SubItems.Add(sStatus);
                lvTaskList.Items[iRow].SubItems.Add(db.FieldAsString(iRow, "StartDate"));
                lvTaskList.Items[iRow].SubItems.Add(db.FieldAsString(iRow, "EndDate"));
                lvTaskList.Items[iRow].SubItems.Add(db.FieldAsString(iRow, "FolderType"));
                lvTaskList.Items[iRow].SubItems.Add(db.FieldAsString(iRow, "FolderPath"));

                lvTaskList.Items[iRow].Tag = iTaskID;
            }

            lvTaskList.EndUpdate();
            lvTaskList.Refresh();

            db.Close();
            db = null;

            string sDbType = InterTabArgumentsReverseByName(csARG_TAG_DB, 1);
            if (sDbType != csARG_TAG_DB_SQLSERVER)
            {
                if (lvTaskList.Items.Count == 1)
                {
                    lvTaskList.Items[0].Selected = true;
                    btnNext.PerformClick();
                }
            }
        }

        private void ListFolders()
        {
            //FIX...
            if (m_InterTabArgs.Count == 0)
            {
                int iTaskIdx = -1;
                for (int i = 0; i < lvCompareTask.Items.Count; i++)
                {
                    if (lvCompareTask.Items[i].Text == ">>")
                    {
                        iTaskIdx = i;
                        break;
                    }
                }
                if (iTaskIdx >= 0)
                {
                    ListFoldersOfTask(iTaskIdx);
                }

                return;
            }

            if (lvCompareTask.Columns.Count == 0)
            {
                lvCompareTask.Columns.Add("Active");
                lvCompareTask.Columns.Add("#"); lvCompareTask.Columns[lvCompareTask.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompareTask.Columns.Add("Label");
                lvCompareTask.Columns.Add("Total Size"); lvCompareTask.Columns[lvCompareTask.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompareTask.Columns.Add("Free Space"); lvCompareTask.Columns[lvCompareTask.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompareTask.Columns.Add("Machine");
                lvCompareTask.Columns.Add("Status Number");
                lvCompareTask.Columns.Add("Status");
                lvCompareTask.Columns.Add("Started");
                lvCompareTask.Columns.Add("Completed");
                lvCompareTask.Columns.Add("Type");
                lvCompareTask.Columns.Add("Root Folder");

                int iCol = -1;
                foreach (ColumnHeader ch in lvCompareTask.Columns)
                {
                    iCol++;
                    int iWidth = RegistryRead("CompareTaskCol" + iCol.ToString(), -1);
                    if (iWidth > -1)
                    {
                        ch.Width = iWidth;
                    }
                }
            }

            if (lvCompare.Columns.Count == 0)
            {
                lvCompare.Columns.Add("#"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompare.Columns.Add("Task");
                lvCompare.Columns.Add("Size"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompare.Columns.Add("Size SUM"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompare.Columns.Add("Level"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompare.Columns.Add("Folder");
                lvCompare.Columns.Add("File Count"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompare.Columns.Add("File Count SUM"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompare.Columns.Add("File Date (MIN)");
                lvCompare.Columns.Add("File Date (MAX)");
                lvCompare.Columns.Add("Path");
                lvCompare.Columns.Add("Folder (8.3)");
                lvCompare.Columns.Add("Path (8.3)");

                int iCol = -1;
                foreach (ColumnHeader ch in lvCompare.Columns)
                {
                    iCol++;
                    int iWidth = RegistryRead("CompareCol" + iCol.ToString(), -1);
                    if (iWidth > -1)
                    {
                        ch.Width = iWidth;
                    }
                }
            }

            string sAddedTaskType = "";
            bool bCompareWithTask = false;
            MyTask myTask = null;
            if (m_Tasks.Count == 0)
            {
                myTask = new MyTask();
                m_Tasks.Add(myTask);

                myTask.m_sInterTabArguments = InterTabArgumentsAsString(csARG_TAG_DB);
            }
            else
            {
                myTask = m_Tasks[0];

                string sArgs = InterTabArgumentsAsString(csARG_TAG_DB);
                if (myTask.m_sInterTabArguments != sArgs)
                {
                    if (sArgs.IndexOf(";" + csARG_TAG_TASK + ";") < 0)
                    {
                        // RESTORE: Interruped Add with Tasks loaded...
                      //sArgs = myTask.m_sInterTabArguments;

                        InterTabArgumentsReverseRemoveUntil(csARG_TAG_DO, "");
                    }
                    else
                    {
                        sAddedTaskType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);

                        if (sAddedTaskType == csARG_TAG_MAP)
                        {
                            bCompareWithTask = true;
                        }

                        myTask = new MyTask();
                        m_Tasks.Add(myTask);

                        myTask.m_sInterTabArguments = sArgs;
                    }
                }
            }

            if (!bCompareWithTask)
            {
                //lvCompareTask.Items.Clear();
                lvCompare.Items.Clear();
            }

            if (myTask.m_iTaskID < 0)
            {
                for (int i = 0; i < m_InterTabArgs.Count; i++)
                {
                    if (m_InterTabArgs[i] == csARG_TAG_TASK)
                    {
                        if (Int32.TryParse(m_InterTabArgs[i + 1], out myTask.m_iTaskID))
                        {
                            if (lvCompareTask.Items.Count == 0)
                            {
                                lvCompareTask.Items.Add(">>");
                            }
                            else
                            {
                                lvCompareTask.Items.Add("  ");
                            }
                            lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(lvCompareTask.Items.Count.ToString());

                            for (int j = i + 2; j < m_InterTabArgs.Count; j++)
                            {
                                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(m_InterTabArgs[j]);

                                switch (j - (i + 2))
                                {
                                    case 0:
                                        myTask.m_sLabel = m_InterTabArgs[j];
                                        break;
                                    case 1:
                                        myTask.m_sTotalSize = m_InterTabArgs[j];
                                        break;
                                    case 2:
                                        myTask.m_sFreeSpace = m_InterTabArgs[j];
                                        break;
                                    case 3:
                                        myTask.m_sMachine = m_InterTabArgs[j];
                                        break;
                                    case 4:
                                        myTask.m_iStatus = Int32.Parse(m_InterTabArgs[j]);
                                        break;
                                    case 5:
                                        myTask.m_sStatus = m_InterTabArgs[j];
                                        break;
                                    case 6:
                                        myTask.m_sStarted = m_InterTabArgs[j];
                                        break;
                                    case 7:
                                        myTask.m_sCompleted = m_InterTabArgs[j];
                                        break;
                                    case 8:
                                        myTask.m_sType = m_InterTabArgs[j];
                                        break;
                                    case 9:
                                        myTask.m_sPath = m_InterTabArgs[j];
                                        break;
                                }
                            }

                            break;
                        }
                    }
                }
                if (myTask.m_iTaskID < 0)
                {
                    MessageBox.Show("Unexpected inter-Tab arguments \"" + InterTabArgumentsAsString("") + "\"!", csAPP_TITLE);
                    return;
                }
            }

            //BUGFIX: Just after add (Compare or Report) pressing Refresh re-added last task!!!
            m_InterTabArgs.Clear();

            MyDb db = ConnectToDb(myTask.m_sInterTabArguments);
            if (db == null)
            {
                return;
            }

            int iRowCount = 0;
            String sTmp;

            if (viewCompare.Text == csVIEW_TITLE_REPORT)
            {
                if (!db.QueryTasks(-1 /*Not Filtered*/, myTask.m_iTaskID /*iReportTaskID*/))
                {
                    MessageBox.Show(db.LastError, csAPP_TITLE);
                    return;
                }

                iRowCount = db.RowCount();
                if (iRowCount < 0)
                {
                    MessageBox.Show(db.LastError, csAPP_TITLE);
                    return;
                }

                if (iRowCount == 0)
                {
                    MessageBox.Show("The report is empty!", csAPP_TITLE);
                    return;
                }

                lvCompareTask.BeginUpdate();

                for (int iRow = 0; iRow < iRowCount; iRow++)
                {
                    lvCompareTask.Items.Add("  ");

                    int iTaskID = db.FieldAsInt(iRow, "ID");

                    MyTask mySubTask = new MyTask();
                    m_Tasks.Add(mySubTask);

                    mySubTask.m_iTaskID = iTaskID;

                    mySubTask.m_sLabel = db.FieldAsString(iRow, "Label");

                    sTmp = db.FieldAsString(iRow, "StorageSize");
                    if (sTmp.Length > 0)
                    {
                        Int64 i64 = 0;
                        if (Int64.TryParse(sTmp, out i64))
                        {
                            sTmp = MyFolder.ToShortSizeString(i64);
                        }
                    }
                    mySubTask.m_sTotalSize = sTmp;

                    sTmp = db.FieldAsString(iRow, "StorageFree");
                    if (sTmp.Length > 0)
                    {
                        Int64 i64 = 0;
                        if (Int64.TryParse(sTmp, out i64))
                        {
                            sTmp = MyFolder.ToShortSizeString(i64);
                        }
                    }
                    mySubTask.m_sFreeSpace = sTmp;

                    mySubTask.m_sMachine = db.FieldAsString(iRow, "Machine");

                    mySubTask.m_iStatus = db.FieldAsInt(iRow, "Status");
                    mySubTask.m_sStatus = "<unknown>";
                    switch (mySubTask.m_iStatus)
                    {
                        case 1:
                            mySubTask.m_sStatus = "Planned";
                            break;
                        case 2:
                            mySubTask.m_sStatus = "Started";
                            break;
                        case 3:
                            mySubTask.m_sStatus = "Completed";
                            break;
                        case 4:
                            mySubTask.m_sStatus = "REPORT";
                            break;
                    }

                    mySubTask.m_sStarted = db.FieldAsString(iRow, "StartDate");
                    mySubTask.m_sCompleted = db.FieldAsString(iRow, "EndDate");
                    mySubTask.m_sType = db.FieldAsString(iRow, "FolderType");
                    mySubTask.m_sPath = db.FieldAsString(iRow, "FolderPath");

                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(lvCompareTask.Items.Count.ToString());
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sLabel);
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sTotalSize);
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sFreeSpace);
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sMachine);
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_iStatus.ToString());
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sStatus);
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sStarted);
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sCompleted);
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sType);
                    lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(mySubTask.m_sPath);
                }

                lvCompareTask.EndUpdate();
                lvCompareTask.Refresh();
            }
            else if (sAddedTaskType == csARG_TAG_REPORT)
            {
                for (int i = 0; i < m_Tasks.Count; i++)
                {
                    if (i == m_Tasks.Count - 1)
                    {
                        lvCompareTask.Items[i].Text = ">>";
                    }
                    else
                    {
                        lvCompareTask.Items[i].Text = "  ";
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    MyTask tsk = m_Tasks[i];

                    int iTaskID_inReport = db.AddReportSubTaskIfNotExists(tsk.m_iStatus, tsk.m_sType, tsk.m_sPath, tsk.m_sLabel, tsk.m_sTotalSize, tsk.m_sFreeSpace, tsk.m_sMachine, tsk.m_sStarted, tsk.m_sCompleted);
                    if (iTaskID_inReport < 0)
                    {
                        MessageBox.Show(db.LastError, csAPP_TITLE);
                        return;
                    }

                    m_Tasks[i].m_iTaskID_inReport = iTaskID_inReport;
                }

                lblPrsMain.Visible = true;
                lblPrsMain.Text = "Adding to Report...";
                lblPrsMain.Update();
                prsMain.Visible = true;
                prsMain.Minimum = 0;
                prsMain.Maximum = m_Tasks[2].Folders.Count;
                prsMain.Value = 0;
                prsMain.Update();
                m_iTickCountLast = Environment.TickCount;

                foreach (MyFolder fldr in m_Tasks[2].Folders)
                {
                    prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1); ;
                    prsMain.Update();
                    if (Environment.TickCount - m_iTickCountLast > m_iFreez)
                    {
                        m_iTickCountLast = Environment.TickCount;
                        Application.DoEvents();
                    }

                    if (fldr.m_iLevel == 1)
                    {
                        switch (fldr.m_State)
                        {

                            case MyFolderState.Equals:
                            {
                                if (!db.AddReportFolderRAWIfNotExists(myTask.m_iTaskID,
                                                            fldr.m_iLevel,
                                                            fldr.m_sCount,
                                                            fldr.m_sCountSUM,
                                                            fldr.m_sSize,
                                                            fldr.m_sSizeSUM,
                                                            GetSqlFileDate(fldr.m_sFileDateTimeMin, fldr.m_sFileDateOnlyMin),
                                                            GetSqlFileDate(fldr.m_sFileDateTimeMax, fldr.m_sFileDateOnlyMax),
                                                            fldr.m_sName83,
                                                            fldr.m_sPath83,
                                                            fldr.m_sName,
                                                            fldr.m_sPath,
                                                            m_Tasks[fldr.m_iTaskIndex].m_iTaskID_inReport))
                                {
                                    MessageBox.Show(db.LastError, csAPP_TITLE);
                                    return;
                                }

                                if (!db.AddReportFolderRAWIfNotExists(myTask.m_iTaskID,
                                                            fldr.m_Twin.m_iLevel,
                                                            fldr.m_Twin.m_sCount,
                                                            fldr.m_Twin.m_sCountSUM,
                                                            fldr.m_Twin.m_sSize,
                                                            fldr.m_Twin.m_sSizeSUM,
                                                            GetSqlFileDate(fldr.m_Twin.m_sFileDateTimeMin, fldr.m_Twin.m_sFileDateOnlyMin),
                                                            GetSqlFileDate(fldr.m_Twin.m_sFileDateTimeMax, fldr.m_Twin.m_sFileDateOnlyMax),
                                                            fldr.m_Twin.m_sName83,
                                                            fldr.m_Twin.m_sPath83,
                                                            fldr.m_Twin.m_sName,
                                                            fldr.m_Twin.m_sPath,
                                                            m_Tasks[fldr.m_Twin.m_iTaskIndex].m_iTaskID_inReport))
                                {
                                    MessageBox.Show(db.LastError, csAPP_TITLE);
                                    return;
                                }

                                myTask.Folders.Add(fldr);
                                myTask.Folders.Add(fldr.m_Twin);

                                break;
                            }

                            case MyFolderState.MissingOne:
                            case MyFolderState.MissingOther:
                            case MyFolderState.DiffersOne:
                            case MyFolderState.DiffersOther:
                            case MyFolderState.Unknown:
                            {
                                if (!db.AddReportFolderRAWIfNotExists(myTask.m_iTaskID,
                                                            fldr.m_iLevel,
                                                            fldr.m_sCount,
                                                            fldr.m_sCountSUM,
                                                            fldr.m_sSize,
                                                            fldr.m_sSizeSUM,
                                                            GetSqlFileDate(fldr.m_sFileDateTimeMin, fldr.m_sFileDateOnlyMin),
                                                            GetSqlFileDate(fldr.m_sFileDateTimeMax, fldr.m_sFileDateOnlyMax),
                                                            fldr.m_sName83,
                                                            fldr.m_sPath83,
                                                            fldr.m_sName,
                                                            fldr.m_sPath,
                                                            m_Tasks[fldr.m_iTaskIndex].m_iTaskID_inReport))
                                {
                                    MessageBox.Show(db.LastError, csAPP_TITLE);
                                    return;
                                }

                                myTask.Folders.Add(fldr);

                                break;
                            }

                        }
                    }
                }

                db.EndTask(myTask.m_iTaskID, ciTASK_STATUS_REPORT);

                lblPrsMain.Visible = false;
                lblPrsMain.Text = "N/A";
                lblPrsMain.Update();
                prsMain.Visible = false;
                prsMain.Update();

                db.Close();
                db = null;

                ListFoldersOfTask(m_Tasks.Count - 1);

                return;
            }

            bool bReportView = (viewCompare.Text == csVIEW_TITLE_REPORT);

            string sOrderBy = "";
            if (bReportView)
            {
                sOrderBy = "ORDER BY NameLong, ReportSubTaskID";
            }
            if (!db.QueryFolders(myTask.m_iTaskID, sOrderBy))
            {
                MessageBox.Show(db.LastError, csAPP_TITLE);
                return;
            }

            iRowCount = db.RowCount();
            if (iRowCount < 0)
            {
                MessageBox.Show(db.LastError, csAPP_TITLE);
                return;
            }
            if (iRowCount == 0)
            {
                MessageBox.Show("No folders with TaskID = " + myTask.m_iTaskID.ToString() + "!", csAPP_TITLE);
                return;
            }

            lvCompare.BeginUpdate();

            lblPrsMain.Visible = true;
            lblPrsMain.Text = "Loading task...";
            lblPrsMain.Update();
            prsMain.Visible = true;
            prsMain.Minimum = 0;
            prsMain.Maximum = iRowCount;
            prsMain.Value = 0;
            prsMain.Update();
            m_iTickCountLast = Environment.TickCount;

            myTask.Folders.Clear();
            myTask.m_iMaxLevel = 0;

            int         iLevelNext;
            String      sIndent;

            MyFolder    fldrPrev = null;

            for (int iRow = 0; iRow < iRowCount; iRow++)
            {
                prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1); ;
                prsMain.Update();
                if (Environment.TickCount - m_iTickCountLast > m_iFreez)
                {
                    m_iTickCountLast = Environment.TickCount;
                    Application.DoEvents();
                }

                MyFolder fldrNew = new MyFolder();
                myTask.Folders.Add(fldrNew);

                if (bCompareWithTask)
                {
                    fldrNew.m_iTaskIndex = 1;
                }
                else
                {
                    fldrNew.m_iTaskIndex = 0;
                }

                fldrNew.m_iLevel           = db.FieldAsInt(   iRow, "TreeLevel");
                fldrNew.CountAsString      = db.FieldAsString(iRow, "FileCountSelf");
                fldrNew.CountSUMAsString   = db.FieldAsString(iRow, "FileCountSUM");
                fldrNew.SizeAsString       = db.FieldAsString(iRow, "FileSizeSelf");
                fldrNew.SizeSUMAsString    = db.FieldAsString(iRow, "FileSizeSUM");
                fldrNew.m_sName            = db.FieldAsString(iRow, "NameLong");

                myTask.m_iMaxLevel = Math.Max(myTask.m_iMaxLevel, fldrNew.m_iLevel);

                sTmp = db.FieldAsString(iRow, "MinFileDate");
                if (sTmp.Length > 5)
                {
                    sTmp = sTmp.Insert(4, ".");
                    sTmp = sTmp.Insert(7, ".");
                    sTmp = sTmp.Insert(10, ".");

                    fldrNew.m_sFileDateTimeMin  = sTmp;
                    fldrNew.m_sFileDateOnlyMin  = sTmp.Substring(0, 10);
                }

                sTmp = db.FieldAsString(iRow, "MaxFileDate");
                if (sTmp.Length > 5)
                {
                    sTmp = sTmp.Insert(4, ".");
                    sTmp = sTmp.Insert(7, ".");
                    sTmp = sTmp.Insert(10, ".");

                    fldrNew.m_sFileDateTimeMax  = sTmp;
                    fldrNew.m_sFileDateOnlyMax  = sTmp.Substring(0, 10);
                }

                fldrNew.m_sPath         = db.FieldAsString(iRow, "PathLong");
                fldrNew.m_sName83       = db.FieldAsString(iRow, "NameShort83");
                fldrNew.m_sPath83       = db.FieldAsString(iRow, "PathShort83");

                if (bReportView)
                {
                    fldrNew.m_iReportSubTaskID = db.FieldAsInt(iRow, "ReportSubTaskID");

                    for (int i = 1; i < m_Tasks.Count; i++)
                    {
                        if (m_Tasks[i].m_iTaskID == fldrNew.m_iReportSubTaskID)
                        {
                            fldrNew.m_iTaskIndex = i;
                            break;
                        }
                    }
                }

                iLevelNext = -1;
                if (iRow + 1 < iRowCount)
                {
                    iLevelNext = db.FieldAsInt(iRow + 1, "TreeLevel");
                }

                if (!bReportView)
                {
                    sIndent = "";
                    for (int i = 0; i < fldrNew.m_iLevel; i++)
                    {
                        //String sTmp = " ɭ ʟ ʈ | ͢   - _ ¯ ʘ ̶  " + m_db.FieldAsString(iRow, "NameLong");

                        if ((i == fldrNew.m_iLevel - 1) && (((iLevelNext > -1) && (iLevelNext < fldrNew.m_iLevel)) || iLevelNext == -1))
                        {
                            sIndent += " ʟ  ";
                        }
                        else
                        {
                            sIndent += " |  ";
                        }
                    }
                    fldrNew.m_sIndent = sIndent;
                }
                else
                {
                    if (fldrPrev != null)
                    {
                        if (
                                (fldrNew.m_sName == fldrPrev.m_sName) &&
                                (fldrNew.m_sPath == fldrPrev.m_sPath) /* &&
                                (fldrNew.m_sName83 == fldrPrev.m_sName83) &&
                                (fldrNew.m_sPath83 == fldrPrev.m_sPath83) */ &&
                                (fldrNew.m_sCount == fldrPrev.m_sCount) &&
                                (fldrNew.m_sCountSUM == fldrPrev.m_sCountSUM) &&
                                (fldrNew.m_sSize == fldrPrev.m_sSize) &&
                                (fldrNew.m_sSizeSUM == fldrPrev.m_sSizeSUM) &&
                                (fldrNew.m_sFileDateTimeMin == fldrPrev.m_sFileDateTimeMin) &&
                                (fldrNew.m_sFileDateTimeMax == fldrPrev.m_sFileDateTimeMax) &&
                                (fldrNew.m_sFileDateOnlyMin == fldrPrev.m_sFileDateOnlyMin) &&
                                (fldrNew.m_sFileDateOnlyMax == fldrPrev.m_sFileDateOnlyMax)
                           )
                        {

                            if (
                                    (fldrNew.m_sName83 == fldrPrev.m_sName83) &&
                                    (fldrNew.m_sPath83 == fldrPrev.m_sPath83) &&
                                    (fldrNew.m_iReportSubTaskID == fldrPrev.m_iReportSubTaskID)
                               )
                            {
                                //REQUIRED FIX!!!
                                //  BUG: MyMdb.AddReportFolderRAWIfNotExists SOMEHOW inserts identical rows!!!
                                myTask.Folders.RemoveAt(myTask.Folders.Count - 1);
                                continue;
                            }
                            else
                            {
                                if (fldrNew.m_State == MyFolderState.DiffersOne || fldrNew.m_State == MyFolderState.DiffersOther ||
                                    fldrPrev.m_State == MyFolderState.DiffersOne || fldrPrev.m_State == MyFolderState.DiffersOther)
                                {
                                    if (fldrNew.m_iTaskIndex == 1)
                                    {
                                        fldrNew.m_State = MyFolderState.DiffersOne; //My be changing later!!!
                                    }
                                    else
                                    {
                                        fldrNew.m_State = MyFolderState.DiffersOther; //My be changing later!!!
                                    }
                                }
                                else
                                {
                                    fldrNew.m_State = MyFolderState.Equals;
                                    fldrPrev.m_State = MyFolderState.Equals;
                                }

                                if (fldrPrev.m_Twin != null)
                                {
                                    fldrNew.m_Twin = fldrPrev.m_Twin;
                                }
                                else
                                {
                                    fldrNew.m_Twin = fldrPrev;
                                }
                            }
                        }
                        else
                        {
                            if (
                                    (fldrNew.m_sName == fldrPrev.m_sName) &&
                                    (fldrNew.m_sPath == fldrPrev.m_sPath) /* &&
                                    (fldrNew.m_sName83 == fldrPrev.m_sName83) &&
                                    (fldrNew.m_sPath83 == fldrPrev.m_sPath83)
                                     */
                               )
                            {
                                if (fldrNew.m_iTaskIndex == 1)
                                {
                                    fldrNew.m_State = MyFolderState.DiffersOne; //My be changing later!!!
                                }
                                else
                                {
                                    fldrNew.m_State = MyFolderState.DiffersOther; //My be changing later!!!
                                }

                                if (fldrPrev.m_iTaskIndex == 1)
                                {
                                    fldrPrev.m_State = MyFolderState.DiffersOne; //My be changing later!!!
                                }
                                else
                                {
                                    fldrPrev.m_State = MyFolderState.DiffersOther; //My be changing later!!!
                                }
                            }
                            else
                            {
                                if (fldrNew.m_iTaskIndex == 1)
                                {
                                    fldrNew.m_State = MyFolderState.MissingOne; //My be changing later!!!
                                }
                                else
                                {
                                    fldrNew.m_State = MyFolderState.MissingOther; //My be changing later!!!
                                }
                            }
                        }
                    }
                    else
                    {
                        if (fldrNew.m_iTaskIndex == 1)
                        {
                            fldrNew.m_State = MyFolderState.MissingOne; //My be changing later!!!
                        }
                        else
                        {
                            fldrNew.m_State = MyFolderState.MissingOther; //My be changing later!!!
                        }
                    }
                }

                if (!bCompareWithTask && !bReportView)
                {
                    lvCompare.Items.Add((fldrNew.m_iTaskIndex + 1).ToString());

                    lvCompare.Items[iRow].SubItems.Add(m_Tasks[fldrNew.m_iTaskIndex].Title);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.SizeAsString);
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.SizeSUMAsString);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_iLevel.ToString());
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sIndent + fldrNew.m_sName);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.CountAsString);
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.CountSUMAsString);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sFileDateTimeMin);
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sFileDateTimeMax);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sPath);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sIndent + fldrNew.m_sName83);
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sPath83);
                }

                fldrPrev = fldrNew;
            }

            int iMaxLevel = 0;

            string sCnt = "";
            string sLvl = "";
            foreach (MyTask tsk in m_Tasks)
            {
                if (sCnt.Length > 0) sCnt += " vs. ";
                sCnt += tsk.Folders.Count.ToString();

                if (sLvl.Length > 0) sLvl += " vs. ";
                sLvl += tsk.m_iMaxLevel.ToString();

                iMaxLevel = Math.Max(iMaxLevel, tsk.m_iMaxLevel);

                if (bReportView)
                {
                    break;
                }
            }
            sbPanelLb.Text = "Folder Count: " + sCnt;
            sbPanelLevel.Text = csMAX_LEVEL_TITLE + sLvl;

            if (bCompareWithTask)
            {
                for (int i = 0; i < m_Tasks.Count; i++)
                {
                    lvCompareTask.Items[i].Text = "  ";
                }

                MyTask myTaskRes = new MyTask();
                m_Tasks.Add(myTaskRes);
                myTaskRes.m_sStatus = "RESULT";
                myTaskRes.m_sType = "RESULT";
                myTaskRes.m_sPath = "Result of comparing the two Tasks above.";
                myTaskRes.m_sStarted = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                lvCompareTask.Items.Add(">>");
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(lvCompareTask.Items.Count.ToString());
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sLabel);
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sTotalSize);
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sFreeSpace);
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sMachine);
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add("");
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sStatus);
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sStarted);
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sCompleted);
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sType);
                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(myTaskRes.m_sPath);

                MyTask myTaskOrig = m_Tasks[0];

                lblPrsMain.Text = "Comparing tasks...";
                lblPrsMain.Update();
                prsMain.Minimum = 0;
                prsMain.Maximum = Math.Max(myTaskOrig.Folders.Count, myTask.Folders.Count);
                prsMain.Value = 0;
                prsMain.Update();
                m_iTickCountLast = Environment.TickCount;

                int iFldrOrig = 0;
                int iFldr = 0;
                for (; ; )
                {
                    prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1);
                    prsMain.Update();
                    if (Environment.TickCount - m_iTickCountLast > m_iFreez)
                    {
                        m_iTickCountLast = Environment.TickCount;
                        Application.DoEvents();
                    }

                    if (iFldrOrig >= myTaskOrig.Folders.Count && iFldr >= myTask.Folders.Count)
                    {
                        //Both list ended!!!
                        break;
                    }
                    else if (iFldrOrig >= myTaskOrig.Folders.Count)
                    {
                        iFldrOrig--; /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! */

                        for (int j = iFldr; j < myTask.Folders.Count - 1 /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! */; j++)
                        {
                            if ((m_bSetting_HideChildrenOfMissingFolder == false) || (myTask.Folders[j].m_iLevel == 1))
                            {
                                myTask.Folders[j].m_State = MyFolderState.MissingOther;

                                //myTaskRes.Folders.Add(myTask.Folders[j]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                                myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[j]);

                                //

                                lvCompare.Items.Insert(iFldrOrig, (myTask.Folders[j].m_iTaskIndex + 1).ToString());

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Title);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].SizeAsString);
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].SizeSUMAsString);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_iLevel.ToString());
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sIndent + myTask.Folders[j].m_sName);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].CountAsString);
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].CountSUMAsString);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sFileDateTimeMin);
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sFileDateTimeMax);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sPath);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sIndent + myTask.Folders[j].m_sName83);
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sPath83);

                                lvCompare.Items[iFldrOrig].BackColor = Color.LightPink;

                                //

                                iFldrOrig++;
                            }
                            else
                            {
                                myTask.Folders[j].m_State = MyFolderState.MissingOther_HIDDEN;
                            }
                        }

                        //break;
                        iFldrOrig = myTaskOrig.Folders.Count - 1;
                        iFldr = myTask.Folders.Count - 1;
                    }
                    else if (iFldr >= myTask.Folders.Count)
                    {
                        for (; ; )
                        {
                            if (iFldrOrig >= myTaskOrig.Folders.Count - 1 /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! */)
                            {
                                break;
                            }

                            if (m_bSetting_HideChildrenOfMissingFolder)
                            {
                                if (myTaskOrig.Folders[iFldrOrig].m_iLevel > 1)
                                {
                                    lvCompare.Items.RemoveAt(iFldrOrig);
                                    myTaskOrig.Folders.RemoveAt(iFldrOrig);
                                }
                                else
                                {
                                    lvCompare.Items[iFldrOrig].BackColor = Color.Orange;
                                    iFldrOrig++;
                                }
                            }
                            else
                            {
                                lvCompare.Items[iFldrOrig].BackColor = Color.Orange;
                                iFldrOrig++;
                            }
                        }

                        //break;
                        iFldrOrig = myTaskOrig.Folders.Count - 1;
                        iFldr     = myTask.Folders.Count - 1;
                    }

                    int iCmp = myTaskOrig.Folders[iFldrOrig].m_sPath.CompareTo(myTask.Folders[iFldr].m_sPath);

                    if (iCmp == 0)
                    {
                        if ( (myTaskOrig.Folders[iFldrOrig].m_sCountSUM != myTask.Folders[iFldr].m_sCountSUM)
                            
                             ||
                             
                             (myTaskOrig.Folders[iFldrOrig].m_sSizeSUM != myTask.Folders[iFldr].m_sSizeSUM)
                             
                             ||

                             (m_bSetting_CompareFileDateOnly && (
                             (myTaskOrig.Folders[iFldrOrig].m_sFileDateOnlyMin != myTask.Folders[iFldr].m_sFileDateOnlyMin) ||
                             (myTaskOrig.Folders[iFldrOrig].m_sFileDateOnlyMax != myTask.Folders[iFldr].m_sFileDateOnlyMax)))

                             ||

                             (m_bSetting_CompareFileDateTime && (
                             (myTaskOrig.Folders[iFldrOrig].m_sFileDateTimeMin != myTask.Folders[iFldr].m_sFileDateTimeMin) ||
                             (myTaskOrig.Folders[iFldrOrig].m_sFileDateTimeMax != myTask.Folders[iFldr].m_sFileDateTimeMax)))
                           )
                        {
                            myTaskOrig.Folders[iFldrOrig].m_State = MyFolderState.DiffersOne;
                            myTaskRes.Folders.Add(myTaskOrig.Folders[iFldrOrig]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                            myTaskOrig.Folders[iFldrOrig].m_bSizeMissMatch = (myTaskOrig.Folders[iFldrOrig].m_sSizeSUM != myTask.Folders[iFldr].m_sSizeSUM);
                            myTask.Folders[iFldr].m_bSizeMissMatch = myTaskOrig.Folders[iFldrOrig].m_bSizeMissMatch;

                            if (myTaskOrig.Folders[iFldrOrig].m_bSizeMissMatch)
                            {
                                lvCompare.Items[iFldrOrig].SubItems[ciCOMPARE_COL_SIZE].Text = myTaskOrig.Folders[iFldrOrig].m_i64SizeSUM.ToString() + " B";
                            }

                            lvCompare.Items[iFldrOrig].BackColor = Color.PaleTurquoise;
                            iFldrOrig++;

                            //

                            myTask.Folders[iFldr].m_State = MyFolderState.DiffersOther;
                            myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[iFldr]);
                            //myTaskRes.Folders.Add(myTask.Folders[iFldr]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                            //

                            lvCompare.Items.Insert(iFldrOrig, (myTask.Folders[iFldr].m_iTaskIndex + 1).ToString());

                            lvCompare.Items[iFldrOrig].SubItems.Add(m_Tasks[myTask.Folders[iFldr].m_iTaskIndex].Title);

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].SizeAsString);
                            if (myTask.Folders[iFldr].m_bSizeMissMatch)
                            {
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_i64SizeSUM.ToString() + " B");
                            }
                            else
                            {
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].SizeSUMAsString);
                            }

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_iLevel.ToString());
                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName);

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].CountAsString);
                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].CountSUMAsString);

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateTimeMin);
                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateTimeMax);

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath);

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName83);
                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath83);

                            lvCompare.Items[iFldrOrig].BackColor = Color.LightBlue;

                        }
                        else
                        {
                            myTaskOrig.Folders[iFldrOrig].m_State = MyFolderState.Equals;

                            //Assing Twins...
                            myTaskOrig.Folders[iFldrOrig].m_Twin = myTask.Folders[iFldr];
                            myTask.Folders[iFldr].m_Twin = myTaskOrig.Folders[iFldrOrig];

                            myTaskRes.Folders.Add(myTaskOrig.Folders[iFldrOrig]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                            myTask.Folders[iFldr].m_State = MyFolderState.Equals;

                            lvCompare.Items[iFldrOrig].BackColor = Color.LightGreen;

                            if (m_bSetting_HideChildrenOfEqualFolder && (iFldrOrig > 0))
                            {
                                if ( (myTaskOrig.Folders[iFldrOrig - 1].m_State == MyFolderState.Equals) &&
                                     (myTaskOrig.Folders[iFldrOrig - 1].m_iLevel < myTaskOrig.Folders[iFldrOrig].m_iLevel) &&
                                     (myTaskOrig.Folders[iFldrOrig].m_iLevel > 1) )
                                {
                                    myTask.Folders[iFldr].m_State = MyFolderState.Equals_HIDDEN;
                                    myTaskOrig.Folders[iFldrOrig].m_State = MyFolderState.Equals_HIDDEN;

                                    //Assing Twins...
                                    myTaskOrig.Folders[iFldrOrig].m_Twin = myTask.Folders[iFldr];
                                    myTask.Folders[iFldr].m_Twin = myTaskOrig.Folders[iFldrOrig];

                                    myTaskOrig.Folders.RemoveAt(iFldrOrig);
                                    lvCompare.Items.RemoveAt(iFldrOrig);

                                    iFldrOrig--;
                                }
                            }
                        }

                        iFldrOrig++;
                        iFldr++;
                    }
                    else if (iCmp > 0)
                    {
                        bool bSolved = false;

                        for (int i = iFldr + 1; i < myTask.Folders.Count - 1 /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! */; i++)
                        {
                            int iCmp2;

                            //if (myTaskOrig.Folders[iFldrOrig].m_iLevel != myTask.Folders[i].m_iLevel) // FINE-TUNE!!!
                            if (myTaskOrig.Folders[iFldrOrig].m_iLevel < myTask.Folders[i].m_iLevel)
                            {
                                //Compare only on the same level!!!
                                continue;
                            }

                            // WRONG-WRONG-WRONG-WRONG!!!
                            /*
                            else if (myTaskOrig.Folders[iFldrOrig].m_iLevel > myTask.Folders[i].m_iLevel) // FINE-TUNE!!!
                            {
                                iCmp2 = 0; // Let it go!
                            }
                            */

                            else
                            {
                                iCmp2 = myTaskOrig.Folders[iFldrOrig].m_sPath.CompareTo(myTask.Folders[i].m_sPath);
                            }

                            if (iCmp2 == 0)
                            {
                                int iStartLevel = myTask.Folders[iFldr].m_iLevel;

                                for (int j = iFldr; j < i; j++)
                                {
                                    /////////////////////////////////////////
                                    //BUGFIX: Too many folders become hidden!
                                    if (j > iFldr && myTask.Folders[j].m_iLevel <= iStartLevel)
                                    {
                                        i = j;
                                        break;
                                    }
                                    //BUGFIX: Too many folders become hidden!
                                    /////////////////////////////////////////

                                    if ((m_bSetting_HideChildrenOfMissingFolder == false) || (myTask.Folders[j].m_iLevel == iStartLevel))
                                    {
                                        myTask.Folders[j].m_State = MyFolderState.MissingOther;

                                        //myTaskRes.Folders.Add(myTask.Folders[j]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                                        myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[j]);

                                        //

                                        lvCompare.Items.Insert(iFldrOrig, (myTask.Folders[j].m_iTaskIndex + 1).ToString());

                                        lvCompare.Items[iFldrOrig].SubItems.Add(m_Tasks[myTask.Folders[j].m_iTaskIndex].Title);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].SizeAsString);
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].SizeSUMAsString);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_iLevel.ToString());
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sIndent + myTask.Folders[j].m_sName);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].CountAsString);
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].CountSUMAsString);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sFileDateTimeMin);
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sFileDateTimeMax);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sPath);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sIndent + myTask.Folders[j].m_sName83);
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sPath83);

                                        lvCompare.Items[iFldrOrig].BackColor = Color.LightPink;

                                        //

                                        iFldrOrig++;
                                    }
                                    else
                                    {
                                        myTask.Folders[j].m_State = MyFolderState.MissingOther_HIDDEN;
                                    }
                                }

                                iFldr = i;

                                bSolved = true;
                                break;
                            }

                            if (bSolved)
                            {
                                break;
                            }
                        }

                        if (!bSolved)
                        {
                            if (iFldrOrig < myTaskOrig.Folders.Count - 1 /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! */)
                            {
                                if (m_bSetting_HideChildrenOfMissingFolder)
                                {
                                    if ((lvCompare.Items[iFldrOrig - 1].BackColor == Color.Orange) &&
                                         (myTaskOrig.Folders[iFldrOrig - 1].m_iLevel < myTaskOrig.Folders[iFldrOrig].m_iLevel))
                                    {
                                        myTaskOrig.Folders[iFldrOrig].m_State = MyFolderState.MissingOne_HIDDEN;
                                        myTaskRes.Folders.Add(myTaskOrig.Folders[iFldrOrig]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                                        lvCompare.Items.RemoveAt(iFldrOrig);
                                        myTaskOrig.Folders.RemoveAt(iFldrOrig);

                                        iFldrOrig--; // Will be incremented!!!
                                    }
                                    else
                                    {
                                        myTaskOrig.Folders[iFldrOrig].m_State = MyFolderState.MissingOne;
                                        myTaskRes.Folders.Add(myTaskOrig.Folders[iFldrOrig]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                                        lvCompare.Items[iFldrOrig].BackColor = Color.Orange;
                                    }
                                }
                                else
                                {
                                    myTaskOrig.Folders[iFldrOrig].m_State = MyFolderState.MissingOne;
                                    myTaskRes.Folders.Add(myTaskOrig.Folders[iFldrOrig]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                                    lvCompare.Items[iFldrOrig].BackColor = Color.Orange;
                                }
                            }

                            iFldrOrig++;

                        }
                    }

                    else if (iCmp < 0)
                    {
                        bool bSolved = false;

                        for (int i = iFldrOrig + 1; i < myTaskOrig.Folders.Count - 1 /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! */; i++)
                        {
                            int iCmp2;

                            //if (myTaskOrig.Folders[i].m_iLevel != myTask.Folders[iFldr].m_iLevel) // FINE-TUNE!!!
                            if (myTaskOrig.Folders[i].m_iLevel > myTask.Folders[iFldr].m_iLevel)
                            {
                                //Compare only on the same level!!!
                                continue;
                            }

                            // WRONG-WRONG-WRONG-WRONG!!!
                            /*
                            else if (myTaskOrig.Folders[i].m_iLevel < myTask.Folders[iFldr].m_iLevel) // FINE-TUNE!!!
                            {
                                iCmp2 = 0; // Let it go!

                                //BUGFIX (commented next line): Folder skip were caused!!!
                                //iFldr++;

                            }
                            */

                            else
                            {
                                iCmp2 = myTaskOrig.Folders[i].m_sPath.CompareTo(myTask.Folders[iFldr].m_sPath);
                            }

                            if (iCmp2 == 0)
                            {
                                int iStartLevel = myTaskOrig.Folders[iFldrOrig].m_iLevel;

                                for (int j = iFldrOrig; j < i; j++)
                                {
                                    /////////////////////////////////////////
                                    //BUGFIX: Too many folders become hidden!
                                    if (j > iFldrOrig && myTaskOrig.Folders[iFldrOrig].m_iLevel <= iStartLevel)
                                    {
                                        break;
                                    }
                                    //BUGFIX: Too many folders become hidden!
                                    /////////////////////////////////////////

                                    if ((m_bSetting_HideChildrenOfMissingFolder == false) || (myTaskOrig.Folders[iFldrOrig].m_iLevel == iStartLevel))
                                    {
                                        myTaskOrig.Folders[iFldrOrig].m_State = MyFolderState.MissingOne;
                                        lvCompare.Items[iFldrOrig].BackColor = Color.Orange;

                                        myTaskRes.Folders.Add(myTaskOrig.Folders[iFldrOrig]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                                        iFldrOrig++;
                                    }
                                    else
                                    {
                                        myTaskOrig.Folders[iFldrOrig].m_State = MyFolderState.MissingOne_HIDDEN;
                                        myTaskRes.Folders.Add(myTaskOrig.Folders[iFldrOrig]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                                        lvCompare.Items.RemoveAt(iFldrOrig);
                                        myTaskOrig.Folders.RemoveAt(iFldrOrig);
                                    }
                                }

                                bSolved = true;
                                break;
                            }

                            if (bSolved)
                            {
                                break;
                            }
                        }

                        if (!bSolved)
                        {

                            // WORKS BAD (the commented out if)!!!
                            /*
                            if (m_bSetting_HideChildrenOfMissingFolder)
                            {
                                if ( (myTask.Folders[iFldr].m_iLevel > 1) && 
                                     ( (myTask.Folders[iFldr - 1].m_State == MyFolderState.MissingOther) ||
                                       (myTask.Folders[iFldr - 1].m_State == MyFolderState.MissingOther_HIDDEN) ) &&
                                     (myTask.Folders[iFldr - 1].m_iLevel == (myTask.Folders[iFldr].m_iLevel - 1)) &&
                                     myTask.Folders[iFldr].m_sPath.IndexOf(myTask.Folders[iFldr - 1].m_sPath) == 0) //Parent of this...
                                {
                                */
                                    myTask.Folders[iFldr].m_State = MyFolderState.MissingOther;

                                    //myTaskRes.Folders.Add(myTask.Folders[iFldr]); //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!

                                    myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[iFldr]);

                                    //

                                    lvCompare.Items.Insert(iFldrOrig, (myTask.Folders[iFldr].m_iTaskIndex + 1).ToString());

                                    lvCompare.Items[iFldrOrig].SubItems.Add(m_Tasks[myTask.Folders[iFldr].m_iTaskIndex].Title);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].SizeAsString);
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].SizeSUMAsString);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_iLevel.ToString());
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].CountAsString);
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].CountSUMAsString);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateTimeMin);
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateTimeMax);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName83);
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath83);

                                    lvCompare.Items[iFldrOrig].BackColor = Color.LightPink;

                                    //

                                    iFldrOrig++;

                                /*
                                }
                                else
                                {
                                    myTask.Folders[iFldr].m_State = MyFolderState.MissingOther_HIDDEN;
                                }
                            }
                            else
                            {
                                myTask.Folders[iFldr].m_State = MyFolderState.MissingOther_HIDDEN;
                            }
                            */

                            iFldr++;
                        }
                    }
                }

                if (m_bSetting_HideChildrenOfMissingFolder)
                {
                    MyFolder[] fldrStack = new MyFolder[iMaxLevel + 1];
                    int fldrStackIndex = -1;

                    lblPrsMain.Text = "Hideing missing whole branches...";
                    lblPrsMain.Update();
                    prsMain.Minimum = 0;
                    prsMain.Maximum = myTaskOrig.Folders.Count;
                    prsMain.Value = 0;
                    prsMain.Update();
                    m_iTickCountLast = Environment.TickCount;

                    fldrPrev = null;
                    MyFolder fldr;
                    iFldrOrig = -1;
                    for (; ; )
                    {
                        iFldrOrig++;

                        if (iFldrOrig >= myTaskOrig.Folders.Count)
                        {
                            break;
                        }

                        prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1);
                        prsMain.Update();
                        if (Environment.TickCount - m_iTickCountLast > m_iFreez)
                        {
                            m_iTickCountLast = Environment.TickCount;
                            Application.DoEvents();
                        }

                        fldr = myTaskOrig.Folders[iFldrOrig];

                        if (fldrPrev != null)
                        {
                            if (fldr.m_iLevel > fldrPrev.m_iLevel)
                            {
                                //Push
                                fldrStackIndex++;
                                fldrStack[fldrStackIndex] = fldrPrev;

                                fldrPrev.m_iIndex_TEMP      = iFldrOrig;
                                fldrPrev.m_StateBranch_TEMP = fldrPrev.m_State;
                            }
                            else if (fldr.m_iLevel < fldrPrev.m_iLevel)
                            {
                                for (; ; )
                                {
                                    if (fldrStack[fldrStackIndex].m_iLevel < fldr.m_iLevel)
                                    {
                                        break;
                                    }

                                    if (fldrStack[fldrStackIndex].m_iLevel > 0 && fldrStack[fldrStackIndex].m_StateBranch_TEMP != MyFolderState.Unknown)
                                    {
                                        switch (fldrStack[fldrStackIndex].m_StateBranch_TEMP)
                                        {
                                            case MyFolderState.MissingOther:
                                            {
                                                while ((iFldrOrig - 1) >= fldrStack[fldrStackIndex].m_iIndex_TEMP)
                                                {
                                                    myTaskOrig.Folders[iFldrOrig - 1].m_State = MyFolderState.MissingOther_HIDDEN;
                                                    myTaskOrig.Folders.RemoveAt(iFldrOrig - 1);

                                                    lvCompare.Items.RemoveAt(iFldrOrig - 1);

                                                    iFldrOrig--;
                                                }
                                                break;
                                            }
                                        }
                                    }

                                    //Pop
                                    fldrStackIndex--;

                                    if (fldrStackIndex <= 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            else if (fldrStackIndex >= 0)
                            {
                                if (fldr.m_State != fldrStack[fldrStackIndex].m_StateBranch_TEMP)
                                {
                                    for (int i = 0; i <= fldrStackIndex; i++)
                                    {
                                        fldrStack[i].m_StateBranch_TEMP = MyFolderState.Unknown;
                                    }
                                }
                            }
                        }

                        fldrPrev = fldr;
                        fldrPrev.m_iIndex_TEMP = iFldrOrig;
                    }
                }

                if (m_bSetting_DoubleCheck)
                {
                    bool bMissMatch = false;

                    for (int iTaskIndex = 0; iTaskIndex <= 1; iTaskIndex++)
                    {
                        MyFolder[] fldrStack = new MyFolder[iMaxLevel + 1];
                        int fldrStackIndex = -1;

                        lblPrsMain.Text = "Checking consistency of Task #" + (iTaskIndex + 1).ToString() + "...";
                        lblPrsMain.Update();
                        prsMain.Minimum = 0;
                        prsMain.Maximum = myTaskOrig.Folders.Count;
                        prsMain.Value = 0;
                        prsMain.Update();
                        m_iTickCountLast = Environment.TickCount;

                        fldrPrev = null;
                        MyFolder fldr;
                        iFldrOrig = -1;
                        for (; ; )
                        {
                            iFldrOrig++;

                            if (iFldrOrig >= myTaskOrig.Folders.Count)
                            {
                                break;
                            }

                            prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1);
                            prsMain.Update();
                            if (Environment.TickCount - m_iTickCountLast > m_iFreez)
                            {
                                m_iTickCountLast = Environment.TickCount;
                                Application.DoEvents();
                            }

                            fldr = myTaskOrig.Folders[iFldrOrig];
                            if ((fldr.m_iTaskIndex != iTaskIndex) && (fldr.m_State != MyFolderState.Equals) && (fldr.m_State != MyFolderState.Equals_HIDDEN))
                            {
                                continue;
                            }

                            if (fldrPrev != null)
                            {
                                if (fldr.m_iLevel > fldrPrev.m_iLevel)
                                {
                                    //Push
                                    fldrStackIndex++;
                                    fldrStack[fldrStackIndex] = fldrPrev;

                                    //fldrPrev.m_ai64CountSUM_DBLCHK[iTaskIndex] += fldrPrev.m_i64Count;
                                    //fldrPrev.m_ai64SizeSUM_DBLCHK[iTaskIndex] += fldrPrev.m_i64Size;

                                    for (int i = fldrStackIndex; i >= 0; i--)
                                    {
                                        fldrStack[i].m_ai64CountSUM_DBLCHK[iTaskIndex] += fldrPrev.m_i64Count;
                                        fldrStack[i].m_ai64SizeSUM_DBLCHK[iTaskIndex] += fldrPrev.m_i64Size;
                                    }
                                }
                                else
                                {
                                    // List item representing files in the root
                                    if ((fldr.m_iLevel == 0) && (fldrStackIndex >= 0))
                                    {
                                        fldrStack[0].m_ai64CountSUM_DBLCHK[iTaskIndex] += fldr.m_i64CountSUM;
                                        fldrStack[0].m_ai64SizeSUM_DBLCHK[iTaskIndex] += fldr.m_i64SizeSUM;
                                    }

                                    for (int i = fldrStackIndex; i >= 0; i--)
                                    {
                                        fldrStack[i].m_ai64CountSUM_DBLCHK[iTaskIndex] += fldrPrev.m_i64CountSUM;
                                        fldrStack[i].m_ai64SizeSUM_DBLCHK[iTaskIndex] += fldrPrev.m_i64SizeSUM;
                                    }
                                }

                                if (fldr.m_iLevel < fldrPrev.m_iLevel)
                                {
                                    for (; ; )
                                    {
                                        if (fldrStack[fldrStackIndex].m_iLevel < fldr.m_iLevel)
                                        {
                                            break;
                                        }

                                        if (fldrStack[fldrStackIndex].m_i64CountSUM != fldrStack[fldrStackIndex].m_ai64CountSUM_DBLCHK[iTaskIndex])
                                        {
                                            lvCompare.Items[fldrStack[fldrStackIndex].m_iIndex_TEMP].SubItems[ciCOMPARE_COL_COUNT].Text += " vs. (" + (iTaskIndex + 1).ToString() + ") " + fldrStack[fldrStackIndex].m_ai64CountSUM_DBLCHK[iTaskIndex].ToString();
                                            lvCompare.Items[fldrStack[fldrStackIndex].m_iIndex_TEMP].BackColor = Color.Yellow;

                                            fldrStack[fldrStackIndex].m_bMissMatch[iTaskIndex] = true;

                                            bMissMatch = true;
                                        }

                                        if (fldrStack[fldrStackIndex].m_i64SizeSUM != fldrStack[fldrStackIndex].m_ai64SizeSUM_DBLCHK[iTaskIndex])
                                        {
                                            lvCompare.Items[fldrStack[fldrStackIndex].m_iIndex_TEMP].SubItems[ciCOMPARE_COL_SIZE].Text = fldrStack[fldrStackIndex].m_i64SizeSUM.ToString() + " B";

                                            lvCompare.Items[fldrStack[fldrStackIndex].m_iIndex_TEMP].SubItems[ciCOMPARE_COL_SIZE].Text += " vs. (" + (iTaskIndex + 1).ToString() + ") " + fldrStack[fldrStackIndex].m_ai64SizeSUM_DBLCHK[iTaskIndex].ToString() + " B";
                                            lvCompare.Items[fldrStack[fldrStackIndex].m_iIndex_TEMP].BackColor = Color.Yellow;

                                            fldrStack[fldrStackIndex].m_bMissMatch[iTaskIndex] = true;

                                            bMissMatch = true;
                                        }

                                        //Pop
                                        fldrStackIndex--;

                                        if (fldrStackIndex <= 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            fldrPrev = fldr;
                            fldrPrev.m_iIndex_TEMP = iFldrOrig;
                        }
                    }

                    if (bMissMatch)
                    {
                        //ROLLBACK
                        /*
                        MessageBox.Show("Double check found missmatches!\n\nSee yellow (state: Unknown (ERROR)) items for more information!", csAPP_TITLE);
                        */
                    }
                }

                //ATTN: myTaskRes.Folders WILL BE THE NEW myTaskOrig.Folders at the end!!!
                List<MyFolder> FoldersTemp = myTaskOrig.Folders;
                myTaskOrig.Folders = myTaskRes.Folders;
                myTaskRes.Folders = FoldersTemp;
            }

            lblPrsMain.Visible = false;
            lblPrsMain.Text = "N/A";
            lblPrsMain.Update();
            prsMain.Visible = false;
            prsMain.Update();

            lvCompare.EndUpdate();
            lvCompare.Refresh();

            db.Close();
            db = null;

            if (bReportView)
            {
                ListFoldersOfTask(0);
            }
        }

        private void ListFoldersOfTask(int iTaskIndex)
        {
            MyTask myTask = m_Tasks[iTaskIndex];

            for (int i = 0; i < m_Tasks.Count; i++)
            {
                if (i == iTaskIndex)
                {
                    lvCompareTask.Items[i].Text = ">>";
                }
                else
                {
                    lvCompareTask.Items[i].Text = "  ";
                }
            }

            lvCompare.BeginUpdate();

            lblPrsMain.Visible = true;
            lblPrsMain.Text = "Refreshing task...";
            lblPrsMain.Update();
            prsMain.Visible = true;
            prsMain.Minimum = 0;
            prsMain.Maximum = myTask.Folders.Count;
            prsMain.Value = 0;
            prsMain.Update();
            m_iTickCountLast = Environment.TickCount;

            bool bReportView = viewCompare.Text == csVIEW_TITLE_REPORT;

            lvCompare.Items.Clear();

            for (int iFldr = 0; iFldr < myTask.Folders.Count; iFldr++)
            {
                prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1); ;
                prsMain.Update();
                if (Environment.TickCount - m_iTickCountLast > m_iFreez)
                {
                    m_iTickCountLast = Environment.TickCount;
                    Application.DoEvents();
                }

                MyFolder fldr = myTask.Folders[iFldr];

                lvCompare.Items.Add((fldr.m_iTaskIndex + 1).ToString());

                lvCompare.Items[iFldr].SubItems.Add(m_Tasks[fldr.m_iTaskIndex].Title);

                if (!bReportView || (fldr.m_Twin == null))
                {
                    lvCompare.Items[iFldr].SubItems.Add(fldr.SizeAsString);
                    if (fldr.m_bSizeMissMatch)
                    {
                        lvCompare.Items[iFldr].SubItems.Add(fldr.m_i64SizeSUM.ToString() + " B");
                    }
                    else
                    {
                        lvCompare.Items[iFldr].SubItems.Add(fldr.SizeSUMAsString);
                    }

                    lvCompare.Items[iFldr].SubItems.Add(fldr.m_iLevel.ToString());
                    lvCompare.Items[iFldr].SubItems.Add(fldr.m_sIndent + fldr.m_sName);

                    lvCompare.Items[iFldr].SubItems.Add(fldr.CountAsString);
                    lvCompare.Items[iFldr].SubItems.Add(fldr.CountSUMAsString);

                    lvCompare.Items[iFldr].SubItems.Add(fldr.m_sFileDateTimeMin);
                    lvCompare.Items[iFldr].SubItems.Add(fldr.m_sFileDateTimeMax);

                    lvCompare.Items[iFldr].SubItems.Add(fldr.m_sPath);

                    lvCompare.Items[iFldr].SubItems.Add(fldr.m_sIndent + fldr.m_sName83);
                    lvCompare.Items[iFldr].SubItems.Add(fldr.m_sPath83);
                }

                if ((fldr.m_bMissMatch[0] || fldr.m_bMissMatch[1]) && (iTaskIndex == 2))
                {
                    lvCompare.Items[iFldr].BackColor = Color.Yellow;

                    bool bSizeMissMatch = false;

                    if (iTaskIndex != 1 && fldr.m_bMissMatch[0])
                    {
                        if (fldr.m_i64CountSUM != fldr.m_ai64CountSUM_DBLCHK[0])
                        {
                            lvCompare.Items[iFldr].SubItems[ciCOMPARE_COL_COUNT].Text += " vs. (1) " + fldr.m_ai64CountSUM_DBLCHK[0].ToString();
                        }

                        if (fldr.m_i64SizeSUM != fldr.m_ai64SizeSUM_DBLCHK[0])
                        {
                            lvCompare.Items[iFldr].SubItems[ciCOMPARE_COL_SIZE].Text = fldr.m_i64SizeSUM.ToString() + " B";
                            lvCompare.Items[iFldr].SubItems[ciCOMPARE_COL_SIZE].Text += " vs. (1) " + fldr.m_ai64SizeSUM_DBLCHK[0].ToString() + " B";

                            bSizeMissMatch = true;
                        }
                    }

                    if (iTaskIndex != 0 && fldr.m_bMissMatch[1])
                    {
                        if (fldr.m_i64CountSUM != fldr.m_ai64CountSUM_DBLCHK[1])
                        {
                            lvCompare.Items[iFldr].SubItems[ciCOMPARE_COL_COUNT].Text += " vs. (2) " + fldr.m_ai64CountSUM_DBLCHK[1].ToString();
                        }

                        if (fldr.m_i64SizeSUM != fldr.m_ai64SizeSUM_DBLCHK[1])
                        {
                            if (!bSizeMissMatch)
                            {
                                lvCompare.Items[iFldr].SubItems[ciCOMPARE_COL_SIZE].Text = fldr.m_i64SizeSUM.ToString() + " B";
                            }

                            lvCompare.Items[iFldr].SubItems[ciCOMPARE_COL_SIZE].Text += " vs. (2) " + fldr.m_ai64SizeSUM_DBLCHK[1].ToString() + " B";
                        }
                    }
                }
                else
                {
                    switch (fldr.m_State)
                    {

                        case MyFolderState.Unknown:
                            if (m_Tasks.Count > 1) // || viewCompare.Text == csVIEW_TITLE_COMPARE)
                            {
                                lvCompare.Items[iFldr].BackColor = Color.Yellow;
                            }
                            break;

                        case MyFolderState.Equals:
                            lvCompare.Items[iFldr].BackColor = Color.LightGreen;
                            break;

                        case MyFolderState.Equals_HIDDEN:
                            lvCompare.Items[iFldr].BackColor = Color.Turquoise;
                            break;

                        case MyFolderState.DiffersOne:
                            lvCompare.Items[iFldr].BackColor = Color.PaleTurquoise;
                            break;

                        case MyFolderState.DiffersOther:
                            lvCompare.Items[iFldr].BackColor = Color.LightBlue;
                            break;

                        case MyFolderState.MissingOne:
                            lvCompare.Items[iFldr].BackColor = Color.Orange;
                            break;

                        case MyFolderState.MissingOne_HIDDEN:
                            lvCompare.Items[iFldr].BackColor = Color.Goldenrod;
                            break;

                        case MyFolderState.MissingOther:
                            lvCompare.Items[iFldr].BackColor = Color.LightPink;
                            break;

                        case MyFolderState.MissingOther_HIDDEN:
                            lvCompare.Items[iFldr].BackColor = Color.Red;
                            break;
                    }
                }
            }

            lblPrsMain.Visible = false;
            lblPrsMain.Text = "N/A";
            lblPrsMain.Update();
            prsMain.Visible = false;
            prsMain.Update();

            lvCompare.EndUpdate();
            lvCompare.Refresh();
        }

        private void viewAny_Enter(object sender, EventArgs e)
        {
            lblCaption.Text = ((TabPage)sender).Text;

            String sTmp = "";
            foreach (TabPage tp in m_ViewStack)
            {
                if (sTmp.Length > 0) sTmp += " -> ";
                sTmp += tp.Text;
            }
            if (sTmp.Length > 0) sTmp += " -> ";
            sTmp += lblCaption.Text;

            lblViewStack.Text = sTmp;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            while (DoBack()) ;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            DoBack();
        }

        private bool DoBack()
        {
            if (Views.SelectedTab == viewCompare)
            {
                lvCompare.Items.Clear();
                lvCompareTask.Items.Clear();
                m_Tasks.Clear();

                sbPanelLb.Text = "Folder Count: N/A";
                sbPanelLevel.Text = csMAX_LEVEL_TITLE + "N/A";
            }

            if (m_ViewStack.Count > 0)
            {
                Views.SelectedTab = m_ViewStack[m_ViewStack.Count - 1];
                m_ViewStack.RemoveAt(m_ViewStack.Count - 1);

                viewAny_Enter(Views.SelectedTab, EventArgs.Empty);

                return true;
            }

            return false;
        }

        private void GoBackAndRefresh(TabPage skipThisPage = null)
        {
            if (m_InterTabArgs.Count == 0) return;

            for (; ; )
            {
                if (m_ViewStack.Count == 0) break;

                if (   (m_ViewStack[m_ViewStack.Count - 1] != skipThisPage) &&
                       (m_ViewStack[m_ViewStack.Count - 1] == viewMain) )
                {

                    string sType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);
                    if (sType == csARG_TAG_MAP)
                    {
                        viewCompare.Text = csVIEW_TITLE_COMPARE;
                    }
                    else
                    {
                        viewCompare.Text = csVIEW_TITLE_REPORT;
                    }

                    Views.SelectedTab = viewCompare;
                    btnRefresh.PerformClick();

                    break;
                }
                else if (   (m_ViewStack[m_ViewStack.Count - 1] != skipThisPage) &&
                            (m_ViewStack[m_ViewStack.Count - 1] == viewCompare) )
                {
                    m_ViewStack.RemoveAt(m_ViewStack.Count - 1);

                    Views.SelectedTab = viewCompare;
                    btnRefresh.PerformClick();
                    break;
                }
                else
                {
                    m_ViewStack.RemoveAt(m_ViewStack.Count - 1);
                }
            }
        }

        private void btnMainOpen_Click(object sender, EventArgs e)
        {
            m_ViewStack.Add(Views.SelectedTab);
            Views.SelectedTab = viewSource;

            m_InterTabArgs.Clear();
            m_InterTabArgs.Add(csARG_TAG_DO);
            m_InterTabArgs.Add(csARG_TAG_OPEN);
            m_InterTabArgs.Add(csARG_TAG_MAP);
        }

        private void btnMainOpenReport_Click(object sender, EventArgs e)
        {
            m_ViewStack.Add(Views.SelectedTab);
            Views.SelectedTab = viewSource;

            m_InterTabArgs.Clear();
            m_InterTabArgs.Add(csARG_TAG_DO);
            m_InterTabArgs.Add(csARG_TAG_OPEN);
            m_InterTabArgs.Add(csARG_TAG_REPORT);
        }

        private void btnSourceMdb_Click(object sender, EventArgs e)
        {
            m_ViewStack.Add(Views.SelectedTab);
            Views.SelectedTab = viewSourceMdb;

            lbSourceMdbFolder.Items.Clear();

            string sType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);

            string sLastMdbFolder = RegistryRead(sType + " MDB Last Folder", "");

            tbSourceMdbFolder.Text = sLastMdbFolder;

            string sOp = InterTabArgumentsReverseByName(csARG_TAG_DO, 1);
            if (sOp == csARG_TAG_OPEN)
            {
                if (lblSourceMdbNewTitle.Visible)
                {
                    m_iSourceOffset = lblSourceMdbFolderList.Top - lblSourceMdbNewTitle.Top;

                    lblSourceMdbNewTitle.Visible = false;

                    tbSourceMdbNewTitle.Visible = false;

                    btnSourceMdbNew.Visible = false;

                    lblSourceMdbFolderList.Top  -= m_iSourceOffset;
                    lbSourceMdbFolder.Top       -= m_iSourceOffset;
                    lbSourceMdbFolder.Height    += m_iSourceOffset;
                }

                tbSourceMdbNewTitle.Text = "";
            }
            else
            {
                if (!lblSourceMdbNewTitle.Visible)
                {
                    lbSourceMdbFolder.Height    -= m_iSourceOffset;
                    lbSourceMdbFolder.Top       += m_iSourceOffset;
                    lblSourceMdbFolderList.Top  += m_iSourceOffset;

                    btnSourceMdbNew.Visible = true;

                    tbSourceMdbNewTitle.Visible = true;

                    lblSourceMdbNewTitle.Visible = true;
                }

                tbSourceMdbNewTitle.Text = ""; // "NEW (" + DateTime.Now.ToShortDateString() + ")";
            }

            if (sLastMdbFolder.Length > 0)
            {
                btnRefresh.PerformClick();
            }
        }

        private void btnSourceSqlServer_Click(object sender, EventArgs e)
        {
            m_ViewStack.Add(Views.SelectedTab);
            Views.SelectedTab = viewSourceSqlServer;
        }

        private void btnSourceSqlServerTestConnect_Click(object sender, EventArgs e)
        {
            RegistryWrite("Last SQL Server", tbSourceSqlServerInstance.Text);
            RegistryWrite("Last SQL Db", tbSourceSqlServerDb.Text);
            RegistryWrite("Last SQL User", tbSourceSqlServerUser.Text);

            if (chbSourceSqlServerSavePw.Checked)
            {
                RegistryWrite("Last SQL Pw", tbSourceSqlServerPw.Text);
            }

            MySqlServer sqlsvr = new MySqlServer();
            if (!sqlsvr.TestConnect(tbSourceSqlServerInstance.Text, tbSourceSqlServerDb.Text, tbSourceSqlServerUser.Text, tbSourceSqlServerPw.Text))
            {
                MessageBox.Show("SQL Server Error: " + sqlsvr.LastError, csAPP_TITLE);

                sqlsvr.Close();
            }
            else
            {
                sqlsvr.Close();

                InterTabArgumentsReverseRemoveUntil(csARG_TAG_DB, csARG_TAG_DO);

                m_InterTabArgs.Add(csARG_TAG_DB);
                m_InterTabArgs.Add(csARG_TAG_DB_SQLSERVER);
                m_InterTabArgs.Add(tbSourceSqlServerInstance.Text);
                m_InterTabArgs.Add(tbSourceSqlServerDb.Text);
                m_InterTabArgs.Add(tbSourceSqlServerUser.Text);
                m_InterTabArgs.Add(tbSourceSqlServerPw.Text);

                m_ViewStack.Add(Views.SelectedTab);
                Views.SelectedTab = viewTasks;

                btnRefresh.PerformClick();
            }
        }

        private void btnSourceMdbFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbSourceMdbFolder.Text = dlg.SelectedPath;

                btnRefresh.PerformClick();
            }
        }

        private void btnSourceMdbNew_Click(object sender, EventArgs e)
        {
            if (tbSourceMdbFolder.Text.Length == 0)
            {
                MessageBox.Show("Select a folder first!", csAPP_TITLE);
                return;
            }

            if (tbSourceMdbNewTitle.Text.Length == 0)
            {
                MessageBox.Show("Type a title first!", csAPP_TITLE);
                return;
            }

            string sType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);

            string sPreFix = "";
            if (sType == csARG_TAG_MAP)
            {
                sPreFix = csMDB_MAP_PREFIX;
            }
            else
            {
                sPreFix = csMDB_RPT_PREFIX;
            }

            String sMdbPath = tbSourceMdbFolder.Text;
            if (sMdbPath[sMdbPath.Length - 1] != '\\') sMdbPath += "\\";
            sMdbPath += sPreFix;
            sMdbPath += " " + tbSourceMdbNewTitle.Text;
            DateTime dt = DateTime.Now;
            string sTmp = dt.ToShortDateString(); // + " " + dt.ToShortTimeString().Replace(":", "-");
            sMdbPath += " (" + sTmp + ")";
            sMdbPath += ".mdb";

            if (System.IO.File.Exists(sMdbPath))
            {
                MessageBox.Show("File \"" + sMdbPath + "\" already exists!", csAPP_TITLE);
                return;
            }

            String sMdbTemplatePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            if (sMdbTemplatePath[sMdbTemplatePath.Length - 1] != '\\') sMdbTemplatePath += "\\";
            sMdbTemplatePath += csMDB_TEMPLATE;

            if (!System.IO.File.Exists(sMdbTemplatePath))
            {
                MessageBox.Show("Template MDB Database \"" + sMdbTemplatePath + "\" does not exist!", csAPP_TITLE);
                return;
            }

            System.IO.File.Copy(sMdbTemplatePath, sMdbPath);

            MyDb db = ConnectToDb("DB;MDB;" + sMdbPath);
            if (db == null)
            {
                MessageBox.Show("Unable to prepare file \"" + sMdbPath + "\" already exists!", csAPP_TITLE);
                return;
            }

            if (db.AddTask(ciTASK_STATUS_REPORT, tbSourceMdbNewTitle.Text) == -1)
            {
                MessageBox.Show(db.LastError, csAPP_TITLE);

                db.Close();
                return;
            }

            db.Close();

            InterTabArgumentsReverseRemoveUntil(csARG_TAG_DB, csARG_TAG_DO);

            m_InterTabArgs.Add(csARG_TAG_DB);
            m_InterTabArgs.Add(csARG_TAG_DB_MDB);
            m_InterTabArgs.Add(sMdbPath);

            m_ViewStack.Add(Views.SelectedTab);
            Views.SelectedTab = viewTasks;

            btnRefresh.PerformClick();
        }

        private void btnTaskNew_Click(object sender, EventArgs e)
        {
            if (tbTaskNew.Text.Length == 0)
            {
                MessageBox.Show("Type a task label first!", csAPP_TITLE);
                return;
            }

            string sType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);

            string sDbType = InterTabArgumentsReverseByName(csARG_TAG_DB, 1);
            if (sDbType == csARG_TAG_DB_SQLSERVER)
            {
                MyDb db = ConnectToDb(InterTabArgumentsAsString(csARG_TAG_DB));
                if (db == null)
                {
                    return;
                }

                int iTaskStatus;
                if (sType == csARG_TAG_MAP)
                {
                    iTaskStatus = ciTASK_STATUS_PLANNED;

                    MessageBox.Show("Create is not supported!", csAPP_TITLE);

                    db.Close();
                    return;
                }
                else
                {
                    iTaskStatus = ciTASK_STATUS_REPORT;
                }

                DateTime dt = DateTime.Now;
                string sTmp = tbTaskNew.Text + " (" + dt.ToShortDateString() + ")";

                if (db.AddTask(iTaskStatus, sTmp) == -1)
                {
                    MessageBox.Show(db.LastError, csAPP_TITLE);

                    db.Close();
                    return;
                }

                tbTaskNew.Text = "";

                db.Close();
            }
            else
            {
                MessageBox.Show("Unexpected Database Type: \"" + sDbType + "\"!");
                return;
            }

            btnRefresh.PerformClick();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            if (Views.SelectedTab == viewSourceMdb)
            {
                String sFolder = tbSourceMdbFolder.Text;
                lbSourceMdbFolder.Items.Clear();
                if (sFolder.Length > 0)
                {
                    if (sFolder[sFolder.Length - 1] != '\\') sFolder += "\\";

                    if (!System.IO.Directory.Exists(sFolder))
                    {
                        MessageBox.Show("Folder \"" + sFolder + "\" does not exist!", csAPP_TITLE);
                    }
                    else
                    {

                        string sType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);

                        RegistryWrite(sType + " MDB Last Folder", sFolder);

                        string sPreFix = "";
                        if (sType == csARG_TAG_MAP)
                        {
                            sPreFix = csMDB_MAP_PREFIX;
                        }
                        else
                        {
                            sPreFix = csMDB_RPT_PREFIX;
                        }

                        string[] asFiles = System.IO.Directory.GetFiles(sFolder, sPreFix + "*.mdb");
                        foreach (string sFile in asFiles)
                        {
                            string sTmp = System.IO.Path.GetFileName(sFile);
                            sTmp = sTmp.Substring(csMDB_MAP_PREFIX.Length);
                            sTmp = sTmp.Substring(0, sTmp.Length - 4);

                            int iPos = sTmp.IndexOf(") (");
                            if (iPos >= 0)
                            {
                                string sTmp2 = sTmp.Substring(iPos + 3);
                                sTmp = sTmp.Substring(0, iPos);
                                sTmp = sTmp + ")\t(";
                                sTmp = sTmp + sTmp2;
                            }

                            lbSourceMdbFolder.Items.Add(sTmp);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select a folder first!", csAPP_TITLE);
                }
            }
            else if (Views.SelectedTab == viewTasks)
            {
                ListTasks();
            }
            else if (Views.SelectedTab == viewCompare)
            {
                ListFolders();
            }
            else
            {
                MessageBox.Show("Refresh is not implemented for View \"" + Views.SelectedTab.Text + "\"!", csAPP_TITLE);
            }
        }

        private void tbSourceMdbFolder_TextChanged(object sender, EventArgs e)
        {
            lbSourceMdbFolder.Items.Clear();
        }

        private void lbSourceMdbFolder_DoubleClick(object sender, EventArgs e)
        {
            btnNext.PerformClick();
        }

        private void lvTaskList_DoubleClick(object sender, EventArgs e)
        {
            btnNext.PerformClick();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            if (Views.SelectedTab == viewSourceMdb)
            {
                if (lbSourceMdbFolder.SelectedIndex < 0)
                {
                    MessageBox.Show("Select a file first!", csAPP_TITLE);
                }
                else
                {
                    string sType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);

                    string sPreFix = "";
                    if (sType == csARG_TAG_MAP)
                    {
                        sPreFix = csMDB_MAP_PREFIX;
                    }
                    else
                    {
                        sPreFix = csMDB_RPT_PREFIX;
                    }

                    String sPathMdb = tbSourceMdbFolder.Text;
                    if (sPathMdb[sPathMdb.Length - 1] != '\\') sPathMdb += "\\";
                    sPathMdb += sPreFix;
                    string sTmp = (string)lbSourceMdbFolder.Items[lbSourceMdbFolder.SelectedIndex];
                    sTmp = sTmp.Replace(")\t(", ") (");
                    sPathMdb += sTmp;
                    sPathMdb += ".mdb";

                    InterTabArgumentsReverseRemoveUntil(csARG_TAG_DB, csARG_TAG_DO);

                    m_InterTabArgs.Add(csARG_TAG_DB);
                    m_InterTabArgs.Add(csARG_TAG_DB_MDB);
                    m_InterTabArgs.Add(sPathMdb);

                    m_ViewStack.Add(Views.SelectedTab);
                    Views.SelectedTab = viewTasks;

                    btnRefresh.PerformClick();
                }
            }
            else if (Views.SelectedTab == viewTasks)
            {
                if (lvTaskList.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Select a Task first!", csAPP_TITLE);
                }
                else
                {
                    if (   lvTaskList.SelectedItems[0].SubItems[6].Text != "Completed"
                        && lvTaskList.SelectedItems[0].SubItems[6].Text != "REPORT")
                    {
                        if (DialogResult.Yes != MessageBox.Show("WARNING!!! The selected Task is not Completed!!!\n\nFolder list is likely INCOMPLETE!!!"
                            + "\n\nDo you want to open Task?", csAPP_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        {
                            return;
                        }
                    }

                    string sType = InterTabArgumentsReverseByName(csARG_TAG_DO, 2);
                    if (sType == csARG_TAG_MAP)
                    {
                        foreach (MyTask myTask in m_Tasks)
                        {
                            if (myTask.m_iTaskID != (int)lvTaskList.SelectedItems[0].Tag) continue;
                            if (myTask.m_sLabel != lvTaskList.SelectedItems[0].SubItems[1].Text) continue;
                            if (myTask.m_sTotalSize != lvTaskList.SelectedItems[0].SubItems[2].Text) continue;
                            if (myTask.m_sFreeSpace != lvTaskList.SelectedItems[0].SubItems[3].Text) continue;
                            if (myTask.m_sMachine != lvTaskList.SelectedItems[0].SubItems[4].Text) continue;
                            if (myTask.m_iStatus.ToString() != lvTaskList.SelectedItems[0].SubItems[5].Text) continue;
                            if (myTask.m_sStatus != lvTaskList.SelectedItems[0].SubItems[6].Text) continue;
                            if (myTask.m_sStarted != lvTaskList.SelectedItems[0].SubItems[7].Text) continue;
                            if (myTask.m_sCompleted != lvTaskList.SelectedItems[0].SubItems[8].Text) continue;
                            if (myTask.m_sType != lvTaskList.SelectedItems[0].SubItems[9].Text) continue;
                            if (myTask.m_sPath != lvTaskList.SelectedItems[0].SubItems[10].Text) continue;

                            MessageBox.Show("Task has been already added!", csAPP_TITLE);
                            return;
                        }
                    }

                    InterTabArgumentsReverseRemoveUntil(csARG_TAG_TASK, csARG_TAG_DO);

                    m_InterTabArgs.Add(csARG_TAG_TASK);
                    m_InterTabArgs.Add(((int)lvTaskList.SelectedItems[0].Tag).ToString());
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[1].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[2].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[3].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[4].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[5].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[6].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[7].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[8].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[9].Text);
                    m_InterTabArgs.Add(lvTaskList.SelectedItems[0].SubItems[10].Text);

                    GoBackAndRefresh();
                }
            }
            else
            {
                MessageBox.Show("Next is not implemented for View \"" + Views.SelectedTab.Text + "\"!", csAPP_TITLE);
            }
        }

        private void btnCompareWith_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            if (Views.SelectedTab == viewCompare)
            {
                if (viewCompare.Text != csVIEW_TITLE_COMPARE)
                {
                    MessageBox.Show("Compare with... is not supported in view \"" + viewCompare.Text + "\"!", csAPP_TITLE);
                    return;
                }

                if (m_Tasks.Count > 1)
                {
                    MessageBox.Show("The maximum allowed one more Task has been already added!", csAPP_TITLE);
                    return;
                }

                m_ViewStack.Add(Views.SelectedTab);
                Views.SelectedTab = viewSource;

                m_InterTabArgs.Clear();
                m_InterTabArgs.Add(csARG_TAG_DO);
                m_InterTabArgs.Add(csARG_TAG_OPEN);
                m_InterTabArgs.Add(csARG_TAG_MAP);
            }
            else
            {
                MessageBox.Show("Compare with... is not implemented for View \"" + Views.SelectedTab.Text + "\"!", csAPP_TITLE);
            }
        }

        private void btnAddToReport_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            if (Views.SelectedTab == viewCompare)
            {
                if (viewCompare.Text != csVIEW_TITLE_COMPARE)
                {
                    MessageBox.Show("Add to Report... is not supported in view \"" + viewCompare.Text + "\"!", csAPP_TITLE);
                    return;
                }

                if (m_Tasks.Count < 3)
                {
                    MessageBox.Show("Compare before adding to Report!", csAPP_TITLE);
                    return;
                }

                if (m_Tasks.Count > 3)
                {
                    MessageBox.Show("Adding to multiple reports is not allowed!", csAPP_TITLE);
                    return;
                }

                int iMaxLevel = 0;
              //bool bHasMissMatch = false;
                foreach (MyFolder fldr in m_Tasks[2].Folders)
                {
                    iMaxLevel = Math.Max(iMaxLevel, fldr.m_iLevel);

                    if (iMaxLevel > 1)
                    {
                        break;
                    }

                    /*
                    if (fldr.m_State == MyFolderState.DiffersOne || (fldr.m_State == MyFolderState.Unknown && fldr.m_iLevel > 0) )
                    {
                        bHasMissMatch = true;
                    }
                    */
                }
                if (iMaxLevel > 1)
                {
                    MessageBox.Show("Compare Result with maximum Level 1 Folders are alloved to be added to Report!"
                                    + "\n\nTip: Split folders into two to make sure that the one is Missing and the other is Equal! This will result Level 1 Folders in Compare Result!", csAPP_TITLE);
                    return;
                }

                /*
                if (bHasMissMatch)
                {
                    if (DialogResult.Yes != MessageBox.Show("There are Folders exist in both location but with Different content!"
                                                            + "\n\nSuch Folders WILL BE NOT COPIED into the REPORT!!!"
                                                            + "\n\nDo you want to continue?", csAPP_TITLE))
                    {
                        return;
                    }
                }
                */

                m_ViewStack.Add(Views.SelectedTab);
                Views.SelectedTab = viewSource;

                m_InterTabArgs.Clear();
                m_InterTabArgs.Add(csARG_TAG_DO);
                m_InterTabArgs.Add(csARG_TAG_SAVE);
                m_InterTabArgs.Add(csARG_TAG_REPORT);
            }
            else
            {
                MessageBox.Show("Add to Report... is not implemented for View \"" + Views.SelectedTab.Text + "\"!", csAPP_TITLE);
            }
        }

        private void btnOpenUpdatedReport_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            if (Views.SelectedTab == viewCompare)
            {
                if (viewCompare.Text != csVIEW_TITLE_COMPARE)
                {
                    MessageBox.Show("Open Updated Report... is not supported in view \"" + viewCompare.Text + "\"!", csAPP_TITLE);
                    return;
                }

                if (m_Tasks.Count < 4)
                {
                    MessageBox.Show("Compare and Add to Report before opening updated Report!", csAPP_TITLE);
                    return;
                }

                m_InterTabArgs.Clear();
                m_InterTabArgs.Add(csARG_TAG_DO);
                m_InterTabArgs.Add(csARG_TAG_OPEN);
                m_InterTabArgs.Add(csARG_TAG_REPORT);

                string[] asArgs = m_Tasks[3].m_sInterTabArguments.Split(';');
                foreach(string sArg in asArgs)
                {
                    m_InterTabArgs.Add(sArg);
                }

                lvCompare.Items.Clear();
                lvCompareTask.Items.Clear();
                m_Tasks.Clear();

                sbPanelLb.Text = "Folder Count: N/A";
                sbPanelLevel.Text = csMAX_LEVEL_TITLE + "N/A";

                GoBackAndRefresh(viewCompare);

                viewAny_Enter(Views.SelectedTab, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Add to Report... is not implemented for View \"" + Views.SelectedTab.Text + "\"!", csAPP_TITLE);
            }
        }

        private void btnPrintReport_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            if (Views.SelectedTab == viewCompare)
            {
                if (viewCompare.Text != csVIEW_TITLE_REPORT)
                {
                    MessageBox.Show("Print Report... is not supported in view \"" + viewCompare.Text + "\"!", csAPP_TITLE);
                    return;
                }

                if (m_Tasks.Count < 2)
                {
                    MessageBox.Show("No Disk Map Task added to this report yet, unable to generate report!", csAPP_TITLE);
                    return;
                }


                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".rtf";
                dlg.Filter = "Rich Text Document Format (*.RTF)|*.rtf|All Files (*.*)|*.*";
                if (DialogResult.OK != dlg.ShowDialog())
                {
                    return;
                }
                string sReportPath = dlg.FileName;

                try
                {
                    string sPart, sVal;

                  //string sReportFolder = "D:\\PROJECTS (TESTS)\\WinDiskSize DISK MAPs (TEST)\\(OLD 105)\\";
                    string sReportFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                    if (sReportFolder[sReportFolder.Length - 1] != '\\') sReportFolder += "\\";

                    string sRep001 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.001");
                    string sRep002 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.002");
                    string sRep003 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.003");
                    string sRep004 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.004");   //Red
                    string sRep005 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.005");   //Orange
                    string sRep006 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.006");   //Green
                    string sRep007 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.007");   //Blue
                    string sRep008 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.008");   //Light Blue
                    string sRep009 = System.IO.File.ReadAllText(sReportFolder + "WinDiskSize_Report_01.rtf.009");

                    StringBuilder sb = new StringBuilder();

                    sPart = sRep001;
                    sPart = sPart.Replace("DATE_OF_REPORT", RtfEncoding(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()));
                    sb.Append(sPart);

                    for (int i = 1; i < m_Tasks.Count; i++)
                    {
                        int iIdx;

                        if (i < m_Tasks.Count - 1)
                        {
                            iIdx = 1;
                            sPart = sRep002;
                        }
                        else
                        {
                            iIdx = 2;
                            sPart = sRep003;
                        }

                        sPart = sPart.Replace("LABEL_" + iIdx.ToString(), RtfEncoding(m_Tasks[i].Title));

                        sVal = lvCompareTask.Items[i].SubItems[4].Text + " free of " + lvCompareTask.Items[i].SubItems[3].Text;
                        sPart = sPart.Replace("SIZE_" + iIdx.ToString(), RtfEncoding(sVal));

                        sPart = sPart.Replace("MACHINE_" + iIdx.ToString(), RtfEncoding(m_Tasks[i].m_sMachine));

                        sPart = sPart.Replace("STATUS_" + iIdx.ToString(), RtfEncoding(m_Tasks[i].m_sStatus));

                        sVal = m_Tasks[i].m_sStarted;
                        if (m_Tasks[i].m_sCompleted.Length > 0)
                        {
                            sVal += " and " + m_Tasks[i].m_sCompleted;
                        }
                        sPart = sPart.Replace("GENERATED_" + iIdx.ToString(), RtfEncoding(sVal));

                        sPart = sPart.Replace("TYPEFOLDER_" + iIdx.ToString(), RtfEncoding(m_Tasks[i].m_sType + " - " + m_Tasks[i].m_sPath));

                        sb.Append(sPart);
                    }

                    MyTask tsk = m_Tasks[0];
                    for (int i = 0; i < tsk.Folders.Count; i++)
                    {
                        MyFolder fldr = tsk.Folders[i];

                        int iIdx = 1;
                        sPart = sRep004; //Red

                        switch (fldr.m_State)
                        {

                            case MyFolderState.MissingOther:
                            {
                                iIdx = 1;
                                sPart = sRep004; //Red
                                break;
                            }

                            case MyFolderState.MissingOne:
                            {
                                iIdx = 2;
                                sPart = sRep005; //Orange
                                break;
                            }

                            case MyFolderState.Equals:
                            {
                                iIdx = 3;
                                sPart = sRep006; //Green
                                break;
                            }

                            case MyFolderState.DiffersOther:
                            {
                                iIdx = 4;
                                sPart = sRep007; //Blue
                                break;
                            }

                            case MyFolderState.DiffersOne:
                            {
                                iIdx = 5;
                                sPart = sRep008; //Light Blue
                                break;
                            }

                        }

                        string sFolder = "";
                        if (lvCompare.Items[i].SubItems.Count > 5)
                        {
                            sFolder = lvCompare.Items[i].SubItems[5].Text;
                        }

                        sVal = "";
                        if (sFolder.Length > 0)
                        {
                            int iCC = 1;
                            for (int j = i + 1; j < tsk.Folders.Count; j++)
                            {
                                if (tsk.Folders[j].m_Twin != fldr)
                                {
                                    break;
                                }

                                iCC++;
                            }
                            sVal = iCC.ToString();
                        }
                        sPart = sPart.Replace("CC_" + iIdx.ToString(), RtfEncoding(sVal));

                        sPart = sPart.Replace("TASK_" + iIdx.ToString(), RtfEncoding(m_Tasks[fldr.m_iTaskIndex].Title));

                        sPart = sPart.Replace("FOLDER_" + iIdx.ToString(), RtfEncoding(sFolder)); //fldr.m_sName));

                        sVal = "";
                        if (lvCompare.Items[i].SubItems.Count > 3)
                        {
                            sVal = lvCompare.Items[i].SubItems[3].Text;
                        }
                        sPart = sPart.Replace("FLDRSIZE_" + iIdx.ToString(), RtfEncoding(sVal));

                        sVal = "";
                        if (lvCompare.Items[i].SubItems.Count > 7)
                        {
                            sVal = lvCompare.Items[i].SubItems[7].Text;
                        }
                        sPart = sPart.Replace("FILECNT_" + iIdx.ToString(), RtfEncoding(sVal));

                        sVal = "";
                        if (lvCompare.Items[i].SubItems.Count > 8)
                        {
                            sVal = lvCompare.Items[i].SubItems[8].Text;
                        }
                        sPart = sPart.Replace("DTMIN_" + iIdx.ToString(), RtfEncoding(sVal));

                        sVal = "";
                        if (lvCompare.Items[i].SubItems.Count > 9)
                        {
                            sVal = lvCompare.Items[i].SubItems[9].Text;
                        }
                        sPart = sPart.Replace("DTMAX_" + iIdx.ToString(), RtfEncoding(sVal));

                        sb.Append(sPart);
                    }

                    sb.Append(sRep009); //EOF

                    System.IO.File.WriteAllText(sReportPath, sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating report: " + ex.Message, csAPP_TITLE);
                    return;
                }

                try
                {
                    System.Diagnostics.Process.Start(sReportPath);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error generating report: " + ex.Message, csAPP_TITLE);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Add to Report... is not implemented for View \"" + Views.SelectedTab.Text + "\"!", csAPP_TITLE);
            }
        }

        private string RtfEncoding(string s)
        {

            // ATTN: MUST BE THE FIRST!!!
            s = s.Replace("\\", "\\'5c");

            s = s.Replace("á", "\\'e1");
            s = s.Replace("é", "\\'e9");
            s = s.Replace("í", "\\'ed");
            s = s.Replace("ó", "\\'f3");
            s = s.Replace("ö", "\\'f6");
            s = s.Replace("ő", "\\'f5");
            s = s.Replace("ú", "\\'fa");
            s = s.Replace("ü", "\\'fc");
            s = s.Replace("ű", "\\'fb");

            s = s.Replace("Á", "\\'c1");
            s = s.Replace("É", "\\'c9");
            s = s.Replace("Í", "\\'cd");
            s = s.Replace("Ó", "\\'d3");
            s = s.Replace("Ö", "\\'d6");
            s = s.Replace("Ő", "\\'d5");
            s = s.Replace("Ú", "\\'da");
            s = s.Replace("Ü", "\\'dc");
            s = s.Replace("Ű", "\\'db");

            return s;
        }

        private void lvCompareTask_DoubleClick(object sender, EventArgs e)
        {
            if (viewCompare.Text != csVIEW_TITLE_COMPARE)
            {
                MessageBox.Show("Feature is not supported in view \"" + viewCompare.Text + "\"!", csAPP_TITLE);
                return;
            }

            if (lvCompareTask.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Select a Task first!", csAPP_TITLE);
                return;
            }

            int iTaskIdx = lvCompareTask.SelectedIndices[0];

            /*
            if (lvCompareTask.Items[iTaskIdx].Text == ">>")
            {
                MessageBox.Show("Selected Task is already listed!", csAPP_TITLE);
                return;
            }
            */

            ListFoldersOfTask(iTaskIdx);
        }

        private void chbSourceSqlServerSavePw_CheckedChanged(object sender, EventArgs e)
        {
            if (chbSourceSqlServerSavePw.Checked)
            {
                chbSourceSqlServerSavePw.ForeColor = Color.Red;
            }
            else
            {
                chbSourceSqlServerSavePw.ForeColor = Color.Black;

                RegistryWrite("Last SQL Pw", "");
            }
        }
    }
}
