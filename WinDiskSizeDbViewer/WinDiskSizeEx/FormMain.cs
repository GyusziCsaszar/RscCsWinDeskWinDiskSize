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

        protected const string csAPP_TITLE = "Win Disk Size Ex v 1.00";

        protected const int ciCOMPARE_COL_SIZE  = 2;
        protected const int ciCOMPARE_COL_COUNT = 6;

        protected const String csMDB_TEMPLATE = "WinDiskSize_Template.mdb";
        protected const String csMDB_MAP_PREFIX = "WinDiskSizeMap";

        protected List<TabPage> m_ViewStack = new List<TabPage>();

        protected String m_sInterTabArguments;

        protected List<MyTask> m_Tasks = new List<MyTask>();

        StatusBar mainStatusBar;
        StatusBarPanel sbPanelLb;
        StatusBarPanel sbPanelLevel;

        protected int m_iTickCountLast;

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
            sbPanelLevel.Text = "Max Level: N/A";
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

        private MyDb ConnectToDb(string sInterTabArguments)
        {
            string[] asArgs = sInterTabArguments.Split(';');

            if (asArgs.Length < 3)
            {
                MessageBox.Show("Unexpected inter-Tab arguments \"" + sInterTabArguments + "\"!", csAPP_TITLE);
                return null;
            }

            if (asArgs[0] != "DB")
            {
                MessageBox.Show("Unexpected inter-Tab arguments \"" + sInterTabArguments + "\"!", csAPP_TITLE);
                return null;
            }

            if (asArgs[1] == "MDB")
            {
                MyMdb mdb = new MyMdb(csMDB_TEMPLATE);
                mdb.Folder = System.IO.Path.GetDirectoryName(asArgs[2]);
                mdb.MdbFile = System.IO.Path.GetFileName(asArgs[2]);

                return mdb;
            }
            else
            {
                MessageBox.Show("Unexpected source type \"" + asArgs[1] + "\"!", csAPP_TITLE);
                return null;
            }
        }

        private void ListTasks()
        {
            lvTaskList.Items.Clear();
            lvTaskList.Columns.Clear();

            MyDb db = ConnectToDb(m_sInterTabArguments);
            if (db == null)
            {
                return;
            }

            lvTaskList.Columns.Add("Task ID"); lvTaskList.Columns[lvTaskList.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
            lvTaskList.Columns.Add("Label");
            lvTaskList.Columns.Add("Total Size"); lvTaskList.Columns[lvTaskList.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
            lvTaskList.Columns.Add("Free Space"); lvTaskList.Columns[lvTaskList.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
            lvTaskList.Columns.Add("Machine");
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

            if (!db.QueryTasks())
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
                    case 1 :
                        sStatus = "Planned";
                        break;
                    case 2:
                        sStatus = "Started";
                        break;
                    case 3:
                        sStatus = "Completed";
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
        }

        private void ListFolders()
        {
            if (lvCompareTask.Columns.Count == 0)
            {
                lvCompareTask.Columns.Add("Active");
                lvCompareTask.Columns.Add("#"); lvCompareTask.Columns[lvCompareTask.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompareTask.Columns.Add("Label");
                lvCompareTask.Columns.Add("Total Size"); lvCompareTask.Columns[lvCompareTask.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompareTask.Columns.Add("Free Space"); lvCompareTask.Columns[lvCompareTask.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
                lvCompareTask.Columns.Add("Machine");
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

            bool bAddingToCompare = false;
            MyTask myTask = null;
            if (m_Tasks.Count == 0)
            {
                myTask = new MyTask();
                m_Tasks.Add(myTask);

                myTask.m_sInterTabArguments = m_sInterTabArguments;
            }
            else
            {
                myTask = m_Tasks[0];

                if (myTask.m_sInterTabArguments != m_sInterTabArguments)
                {
                    if (m_sInterTabArguments.IndexOf(";TaskID;") < 0)
                    {
                        // RESTORE: Interruped Add with Tasks loaded...
                        m_sInterTabArguments = myTask.m_sInterTabArguments;
                    }
                    else
                    {
                        bAddingToCompare = true;

                        myTask = new MyTask();
                        m_Tasks.Add(myTask);

                        myTask.m_sInterTabArguments = m_sInterTabArguments;
                    }
                }
            }

            if (!bAddingToCompare)
            {
                //lvCompareTask.Items.Clear();
                lvCompare.Items.Clear();
            }

            MyDb db = ConnectToDb(myTask.m_sInterTabArguments);
            if (db == null)
            {
                return;
            }

            if (myTask.m_iTaskID < 0)
            {
                string[] asArgs = m_sInterTabArguments.Split(';');
                for (int i = 0; i < asArgs.Length; i++)
                {
                    if (asArgs[i] == "TaskID")
                    {
                        if (Int32.TryParse(asArgs[i + 1], out myTask.m_iTaskID))
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

                            for (int j = i + 2; j < asArgs.Length; j++)
                            {
                                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(asArgs[j]);

                                switch (j - (i + 2))
                                {
                                    case 0:
                                        myTask.m_sLabel = asArgs[j];
                                        break;
                                    case 1:
                                        myTask.m_sTotalSize = asArgs[j];
                                        break;
                                    case 2:
                                        myTask.m_sFreeSpace = asArgs[j];
                                        break;
                                    case 3:
                                        myTask.m_sMachine = asArgs[j];
                                        break;
                                    case 4:
                                        myTask.m_sStatus = asArgs[j];
                                        break;
                                    case 5:
                                        myTask.m_sStarted = asArgs[j];
                                        break;
                                    case 6:
                                        myTask.m_sCompleted = asArgs[j];
                                        break;
                                    case 7:
                                        myTask.m_sType = asArgs[j];
                                        break;
                                    case 8:
                                        myTask.m_sPath = asArgs[j];
                                        break;
                                }
                            }

                            break;
                        }
                    }
                }
                if (myTask.m_iTaskID < 0)
                {
                    MessageBox.Show("Unexpected inter-Tab arguments \"" + m_sInterTabArguments + "\"!", csAPP_TITLE);
                    return;
                }
            }

            if (!db.QueryFolders(myTask.m_iTaskID))
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

            int     iLevelNext;
            String  sIndent;
            String  sTmp;

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

                if (bAddingToCompare)
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

                iLevelNext = -1;
                if (iRow + 1 < iRowCount)
                {
                    iLevelNext = db.FieldAsInt(iRow + 1, "TreeLevel");
                }

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

                if (!bAddingToCompare)
                {
                    lvCompare.Items.Add((fldrNew.m_iTaskIndex + 1).ToString());

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
            }
            sbPanelLb.Text = "Folder Count: " + sCnt;
            sbPanelLevel.Text = "Max Level: " + sLvl;

            if (bAddingToCompare)
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

                    MyFolder fldrPrev = null;
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

                        MyFolder fldrPrev = null;
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
                        MessageBox.Show("Double check found missmatches!\n\nSee yellow (state: Unknown (ERROR)) items for more information!", csAPP_TITLE);
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
                            lvCompare.Items[iFldr].BackColor = Color.Yellow;
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
                sbPanelLevel.Text = "Max Level: N/A";
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

        private void GoBackAndRefresh()
        {
            if (m_sInterTabArguments.Length == 0) return;

            for (; ; )
            {
                if (m_ViewStack.Count == 0) break;

                if (m_ViewStack[m_ViewStack.Count - 1] == viewMain)
                {
                    Views.SelectedTab = viewCompare;
                    btnRefresh.PerformClick();
                    break;
                }
                else if (m_ViewStack[m_ViewStack.Count - 1] == viewCompare)
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

            m_sInterTabArguments = "";
        }

        private void btnSourceMdb_Click(object sender, EventArgs e)
        {
            m_ViewStack.Add(Views.SelectedTab);
            Views.SelectedTab = viewSourceMdb;

            m_sInterTabArguments = "";

            string sLastMdbFolder = RegistryRead("Last MDB Folder", "");
            if (sLastMdbFolder.Length > 0)
            {
                tbSourceMdbFolder.Text = sLastMdbFolder;
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
                        RegistryWrite("Last MDB Folder", sFolder);

                        string[] asFiles = System.IO.Directory.GetFiles(sFolder, csMDB_MAP_PREFIX + "*.mdb");
                        foreach (string sFile in asFiles)
                        {
                            string sTmp = System.IO.Path.GetFileName(sFile);
                            sTmp = sTmp.Substring(csMDB_MAP_PREFIX.Length);
                            sTmp = sTmp.Substring(0, sTmp.Length - 4);
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
                    String sPath = tbSourceMdbFolder.Text;
                    if (sPath[sPath.Length - 1] != '\\') sPath += "\\";
                    sPath += csMDB_MAP_PREFIX;
                    sPath += (string) lbSourceMdbFolder.Items[lbSourceMdbFolder.SelectedIndex];
                    sPath += ".mdb";

                    int iPos = m_sInterTabArguments.IndexOf("DB;");
                    if (iPos >= 0)
                    {
                        m_sInterTabArguments = m_sInterTabArguments.Substring(0, iPos);
                    }
                    if (m_sInterTabArguments.Length > 0) m_sInterTabArguments += ";";
                    m_sInterTabArguments = "DB;" + "MDB;" + sPath;

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
                    foreach (MyTask myTask in m_Tasks)
                    {
                        if (myTask.m_iTaskID    != (int)lvTaskList.SelectedItems[0].Tag) continue;
                        if (myTask.m_sLabel     != lvTaskList.SelectedItems[0].SubItems[1].Text) continue;
                        if (myTask.m_sTotalSize != lvTaskList.SelectedItems[0].SubItems[2].Text) continue;
                        if (myTask.m_sFreeSpace != lvTaskList.SelectedItems[0].SubItems[3].Text) continue;
                        if (myTask.m_sMachine   != lvTaskList.SelectedItems[0].SubItems[4].Text) continue;
                        if (myTask.m_sStatus    != lvTaskList.SelectedItems[0].SubItems[5].Text) continue;
                        if (myTask.m_sStarted   != lvTaskList.SelectedItems[0].SubItems[6].Text) continue;
                        if (myTask.m_sCompleted != lvTaskList.SelectedItems[0].SubItems[7].Text) continue;
                        if (myTask.m_sType      != lvTaskList.SelectedItems[0].SubItems[8].Text) continue;
                        if (myTask.m_sPath      != lvTaskList.SelectedItems[0].SubItems[9].Text) continue;

                        MessageBox.Show("Task has been already added!", csAPP_TITLE);
                        return;
                    }

                    int iPos = m_sInterTabArguments.IndexOf("TaskID;");
                    if (iPos >= 0)
                    {
                        m_sInterTabArguments = m_sInterTabArguments.Substring(0, iPos);
                    }
                    if (m_sInterTabArguments.Length > 0) m_sInterTabArguments += ";";
                    m_sInterTabArguments += "TaskID;" + ((int)lvTaskList.SelectedItems[0].Tag).ToString();
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[1].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[2].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[3].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[4].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[5].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[6].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[7].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[8].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[9].Text;

                    GoBackAndRefresh();
                }
            }
            else
            {
                MessageBox.Show("Next is not implemented for View \"" + Views.SelectedTab.Text + "\"!", csAPP_TITLE);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lblPrsMain.Visible) return; //In Progress...

            if (Views.SelectedTab == viewCompare)
            {
                if (m_Tasks.Count > 1)
                {
                    MessageBox.Show("The maximum allowed one more Task has been already added!", csAPP_TITLE);
                    return;
                }

                m_ViewStack.Add(Views.SelectedTab);
                Views.SelectedTab = viewSource;

                m_sInterTabArguments = "";
            }
            else
            {
                MessageBox.Show("Add is not implemented for View \"" + Views.SelectedTab.Text + "\"!", csAPP_TITLE);
            }

        }

        private void lvCompareTask_DoubleClick(object sender, EventArgs e)
        {
            if (lvCompareTask.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Select a Task first!", csAPP_TITLE);
                return;
            }

            int iTaskIdx = lvCompareTask.SelectedIndices[0];
            if (lvCompareTask.Items[iTaskIdx].Text == ">>")
            {
                MessageBox.Show("Selected Task is already listed!", csAPP_TITLE);
                return;
            }

            ListFoldersOfTask(iTaskIdx);
        }
    }
}
