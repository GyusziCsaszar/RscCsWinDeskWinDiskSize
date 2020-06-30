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

        protected const String csMDB_TEMPLATE = "WinDiskSize_Template.mdb";

        protected List<TabPage> m_ViewStack = new List<TabPage>();

        protected String m_sInterTabArguments;

        protected List<MyTask> m_Tasks = new List<MyTask>();

        StatusBar mainStatusBar;
        StatusBarPanel sbPanelLb;
        StatusBarPanel sbPanelLevel;

        // Settings
        protected bool m_bSetting_CompareFileDateMinAndMax      = false;
        protected bool m_bSetting_HideChildrenOfMissingFolder   = true;
        protected bool m_bSetting_HideChildrenOfEqualFolder     = true;

        public FormMain()
        {
            InitializeComponent();

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

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
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
                MessageBox.Show("Unexpected inter-Tab arguments \"" + sInterTabArguments + "\"!");
                return null;
            }

            if (asArgs[0] != "DB")
            {
                MessageBox.Show("Unexpected inter-Tab arguments \"" + sInterTabArguments + "\"!");
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
                MessageBox.Show("Unexpected source type \"" + asArgs[1] + "\"!");
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
                MessageBox.Show(db.LastError);
                return;
            }

            int iRowCount = db.RowCount();
            if (iRowCount < 0)
            {
                MessageBox.Show(db.LastError);
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

                lvTaskList.Items.Add(db.FieldAsString(iRow, "Machine"));

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
                lvCompareTask.Columns.Add("#"); lvCompareTask.Columns[lvCompareTask.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
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
                lvCompare.Columns.Add("Folder");
                lvCompare.Columns.Add("File Count"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
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
                            lvCompareTask.Items.Add((lvCompareTask.Items.Count + 1).ToString());
                            for (int j = i + 2; j < asArgs.Length; j++)
                            {
                                lvCompareTask.Items[lvCompareTask.Items.Count - 1].SubItems.Add(asArgs[j]);

                                switch (j - (i + 2))
                                {
                                    case 0:
                                        myTask.m_sMachine = asArgs[j];
                                        break;
                                    case 1:
                                        myTask.m_sStatus = asArgs[j];
                                        break;
                                    case 2:
                                        myTask.m_sStarted = asArgs[j];
                                        break;
                                    case 3:
                                        myTask.m_sCompleted = asArgs[j];
                                        break;
                                    case 4:
                                        myTask.m_sType = asArgs[j];
                                        break;
                                    case 5:
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
                    MessageBox.Show("Unexpected inter-Tab arguments \"" + m_sInterTabArguments + "\"!");
                    return;
                }
            }

            if (!db.QueryFolders(myTask.m_iTaskID))
            {
                MessageBox.Show(db.LastError);
                return;
            }

            int iRowCount = db.RowCount();
            if (iRowCount < 0)
            {
                MessageBox.Show(db.LastError);
                return;
            }
            if (iRowCount == 0)
            {
                MessageBox.Show("No folders with TaskID = " + myTask.m_iTaskID.ToString() + "!");
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

            myTask.Folders.Clear();
            myTask.m_iMaxLevel = 0;

            int     iLevelNext;
            String  sIndent;
            String  sTmp;

            for (int iRow = 0; iRow < iRowCount; iRow++)
            {
                prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1); ;
                prsMain.Update();

                MyFolder fldrNew = new MyFolder();
                myTask.Folders.Add(fldrNew);

                fldrNew.m_iLevel        = db.FieldAsInt(   iRow, "TreeLevel");
                fldrNew.SizeAsString    = db.FieldAsString(iRow, "SizeSUM");
                fldrNew.CountAsString   = db.FieldAsString(iRow, "CountSUM");
                fldrNew.m_sName         = db.FieldAsString(iRow, "NameLong");

                myTask.m_iMaxLevel = Math.Max(myTask.m_iMaxLevel, fldrNew.m_iLevel);

                sTmp = db.FieldAsString(iRow, "MinFileDate");
                if (sTmp.Length > 5)
                {
                    sTmp = sTmp.Insert(4, ".");
                    sTmp = sTmp.Insert(7, ".");
                    sTmp = sTmp.Insert(10, ".");
                }
                fldrNew.m_sFileDateMin  = sTmp;

                sTmp = db.FieldAsString(iRow, "MaxFileDate");
                if (sTmp.Length > 5)
                {
                    sTmp = sTmp.Insert(4, ".");
                    sTmp = sTmp.Insert(7, ".");
                    sTmp = sTmp.Insert(10, ".");
                }
                fldrNew.m_sFileDateMax  = sTmp;

                fldrNew.m_sPath         = db.FieldAsString(iRow, "PathLong");
                fldrNew.m_sName83       = db.FieldAsString(iRow, "NameShort83");
                fldrNew.m_sPath83       = db.FieldAsString(iRow, "PathShort83");

                iLevelNext = -1;
                if (iRow + 1 < iRowCount)
                {
                    iLevelNext = db.FieldAsInt(iRow + 1, "TreeLevel");
                }

                // ROLLED BACK!!!
                /*
                fldrNew.m_bHasChildren = false;
                if (iLevelNext >= 0)
                {
                    if (iLevelNext >= fldrNew.m_iLevel)
                    {
                        fldrNew.m_bHasChildren = true;
                    }
                }
                */

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
                    lvCompare.Items.Add("1");

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.SizeAsString);
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sIndent + fldrNew.m_sName);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.CountAsString);
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sFileDateMin);
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sFileDateMax);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sPath);

                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sIndent + fldrNew.m_sName83);
                    lvCompare.Items[iRow].SubItems.Add(fldrNew.m_sPath83);
                }
            }

            string sCnt = "";
            string sLvl = "";
            foreach (MyTask tsk in m_Tasks)
            {
                if (sCnt.Length > 0) sCnt += " vs. ";
                sCnt += tsk.Folders.Count.ToString();

                if (sLvl.Length > 0) sLvl += " vs. ";
                sLvl += tsk.m_iMaxLevel.ToString();
            }
            sbPanelLb.Text = "Folder Count: " + sCnt;
            sbPanelLevel.Text = "Max Level: " + sLvl;

            if (bAddingToCompare)
            {
                MyTask myTaskOrig = m_Tasks[0];

                lblPrsMain.Text = "Comparing tasks...";
                lblPrsMain.Update();
                prsMain.Minimum = 0;
                prsMain.Maximum = Math.Max(myTaskOrig.Folders.Count, myTask.Folders.Count);
                prsMain.Value = 0;
                prsMain.Update();

                int iFldrOrig = 0;
                int iFldr = 0;
                for (; ; )
                {
                    prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1);
                    prsMain.Update();

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
                            if ( (m_bSetting_HideChildrenOfMissingFolder == false) || (myTask.Folders[j].m_iLevel == 1) )
                            {
                                myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[j]);

                                //

                                lvCompare.Items.Insert(iFldrOrig, m_Tasks.Count.ToString());

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].SizeAsString);
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sIndent + myTask.Folders[j].m_sName);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].CountAsString);
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sFileDateMin);
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sFileDateMax);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sPath);

                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sIndent + myTask.Folders[j].m_sName83);
                                lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sPath83);

                                lvCompare.Items[iFldrOrig].BackColor = Color.LightPink;

                                //

                                iFldrOrig++;
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
                        if ( (myTaskOrig.Folders[iFldrOrig].m_sCount != myTask.Folders[iFldr].m_sCount) ||
                             (myTaskOrig.Folders[iFldrOrig].m_sSize != myTask.Folders[iFldr].m_sSize) ||
                             (m_bSetting_CompareFileDateMinAndMax && (
                             (myTaskOrig.Folders[iFldrOrig].m_sFileDateMin != myTask.Folders[iFldr].m_sFileDateMin) ||
                             (myTaskOrig.Folders[iFldrOrig].m_sFileDateMax != myTask.Folders[iFldr].m_sFileDateMax) ) ) )
                        {
                            iFldrOrig++;

                            //

                            myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[iFldr]);

                            //

                            lvCompare.Items.Insert(iFldrOrig, m_Tasks.Count.ToString());

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].SizeAsString);
                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName);

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].CountAsString);
                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateMin);
                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateMax);

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath);

                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName83);
                            lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath83);

                            lvCompare.Items[iFldrOrig].BackColor = Color.LightBlue;
                        }
                        else
                        {
                            lvCompare.Items[iFldrOrig].BackColor = Color.LightGreen;
                        }

                        iFldrOrig++;
                        iFldr++;
                    }

                    // IN QUESTION!!!
                    /*
                    else if ((iCmp > 0) && (!myTask.Folders[iFldr].m_bHasChildren))
                    {
                        myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[iFldr]);

                        //

                        lvCompare.Items.Insert(iFldrOrig, m_Tasks.Count.ToString());

                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].SizeAsString);
                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName);

                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].CountAsString);
                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateMin);
                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateMax);

                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath);

                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName83);
                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath83);

                        lvCompare.Items[iFldrOrig].BackColor = Color.LightPink;

                        //

                        iFldrOrig++;

                        iFldr++;
                    }
                    */

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
                            else if (myTaskOrig.Folders[iFldrOrig].m_iLevel > myTask.Folders[i].m_iLevel) // FINE-TUNE!!!
                            {
                                iCmp2 = 0; // Let it go!
                            }

                            // IN QUESTION!!!
                            /*
                            else if (myTask.Folders[i].m_i64Size == 0)
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
                                for (int j = iFldr; j < i; j++)
                                {
                                    myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[j]);

                                    //

                                    lvCompare.Items.Insert(iFldrOrig, m_Tasks.Count.ToString());

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].SizeAsString);
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sIndent + myTask.Folders[j].m_sName);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].CountAsString);
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sFileDateMin);
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sFileDateMax);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sPath);

                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sIndent + myTask.Folders[j].m_sName83);
                                    lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[j].m_sPath83);

                                    lvCompare.Items[iFldrOrig].BackColor = Color.LightPink;

                                    //

                                    iFldrOrig++;

                                    if (m_bSetting_HideChildrenOfMissingFolder)
                                    {
                                        break; // ADD THE MISSING PARENT FOLDER ONLY!!! // FINE TUNE!
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

                            /////////////////////////////////////
                            // BUG!!! INSERTS EXISTING FOLDERS!!!
                            {
                                /*
                                if (iFldr < myTask.Folders.Count - 1) /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! *
                                {
                                    if ( (m_bSetting_HideChildrenOfMissingFolder) &&
                                         (lvCompare.Items[iFldrOrig - 1].BackColor == Color.LightPink) &&
                                         (myTaskOrig.Folders[iFldrOrig - 1].m_iLevel < myTask.Folders[iFldr].m_iLevel) )
                                    {
                                        //NOP!!!
                                    }
                                    else
                                    {
                                        myTaskOrig.Folders.Insert(iFldrOrig, myTask.Folders[iFldr]);

                                        //

                                        lvCompare.Items.Insert(iFldrOrig, m_Tasks.Count.ToString());

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].SizeAsString);
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].CountAsString);
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateMin);
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sFileDateMax);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath);

                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sIndent + myTask.Folders[iFldr].m_sName83);
                                        lvCompare.Items[iFldrOrig].SubItems.Add(myTask.Folders[iFldr].m_sPath83);

                                        lvCompare.Items[iFldrOrig].BackColor = Color.LightPink;

                                        //

                                        iFldrOrig++;
                                    }
                                }

                                iFldr++;
                                */
                            }
                            // BUG!!! INSERTS EXISTING FOLDERS!!!
                            /////////////////////////////////////

                            if (iFldrOrig < myTaskOrig.Folders.Count - 1 /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! */)
                            {
                                if (m_bSetting_HideChildrenOfMissingFolder)
                                {
                                    if ((lvCompare.Items[iFldrOrig - 1].BackColor == Color.Orange) &&
                                         (myTaskOrig.Folders[iFldrOrig - 1].m_iLevel < myTaskOrig.Folders[iFldrOrig].m_iLevel))
                                    {
                                        lvCompare.Items.RemoveAt(iFldrOrig);
                                        myTaskOrig.Folders.RemoveAt(iFldrOrig);

                                        iFldrOrig--; // Will be incremented!!!
                                    }
                                    else
                                    {
                                        lvCompare.Items[iFldrOrig].BackColor = Color.Orange;
                                    }
                                }
                                else
                                {
                                    lvCompare.Items[iFldrOrig].BackColor = Color.Orange;
                                }
                            }

                            iFldrOrig++;

                        }
                    }

                    // IN QUESTION!!!
                    /*
                    else if ((iCmp < 0) && (!myTaskOrig.Folders[iFldrOrig].m_bHasChildren))
                    {
                        lvCompare.Items[iFldrOrig].BackColor = Color.Orange;

                        iFldrOrig++;
                    }
                    */

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
                            else if (myTaskOrig.Folders[i].m_iLevel < myTask.Folders[iFldr].m_iLevel) // FINE-TUNE!!!
                            {
                                iCmp2 = 0; // Let it go!

                                iFldr++;
                            }

                            // IN QUESTION!!!
                            /*
                            else if (!myTaskOrig.Folders[i].m_bHasChildren) // FINE-TUNE!!!
                            {
                                iCmp2 = 0; // Let it go!

                                iFldr++;
                            }
                            */

                            else
                            {
                                iCmp2 = myTaskOrig.Folders[i].m_sPath.CompareTo(myTask.Folders[iFldr].m_sPath);
                            }

                            if (iCmp2 == 0)
                            {

                                if (m_bSetting_HideChildrenOfMissingFolder)
                                {
                                    lvCompare.Items[iFldrOrig].BackColor = Color.Orange;

                                    iFldrOrig++;

                                    for (int j = iFldrOrig; j < i; j++)
                                    {
                                        lvCompare.Items.RemoveAt(iFldrOrig);
                                        myTaskOrig.Folders.RemoveAt(iFldrOrig);
                                    }
                                }
                                else
                                {
                                    for (int j = iFldrOrig; j < i; j++)
                                    {
                                        lvCompare.Items[j].BackColor = Color.Orange;
                                    }

                                    iFldrOrig = i;
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
                            if (iFldrOrig < myTaskOrig.Folders.Count - 1 /* ATTN!!! - FINAL ITEM IS ABOUT ROOT!!! */)
                            {
                                if (m_bSetting_HideChildrenOfMissingFolder)
                                {
                                    if ((lvCompare.Items[iFldrOrig - 1].BackColor == Color.Orange) &&
                                         (myTaskOrig.Folders[iFldrOrig - 1].m_iLevel < myTaskOrig.Folders[iFldrOrig].m_iLevel))
                                    {
                                        lvCompare.Items.RemoveAt(iFldrOrig);
                                        myTaskOrig.Folders.RemoveAt(iFldrOrig);

                                        iFldrOrig--; // Will be incremented!!!
                                    }
                                    else
                                    {
                                        lvCompare.Items[iFldrOrig].BackColor = Color.Orange;
                                    }
                                }
                                else
                                {
                                    lvCompare.Items[iFldrOrig].BackColor = Color.Orange;
                                }
                            }

                            iFldrOrig++;
                        }
                    }
                }

                if (m_bSetting_HideChildrenOfEqualFolder)
                {

                    lblPrsMain.Text = "Hiding equal subfolders...";
                    lblPrsMain.Update();
                    prsMain.Minimum = 0;
                    prsMain.Maximum = myTaskOrig.Folders.Count;
                    prsMain.Value = 0;
                    prsMain.Update();

                    iFldrOrig = 0;
                    for (; ; )
                    {
                        if (iFldrOrig >= myTaskOrig.Folders.Count)
                        {
                            break;
                        }

                        prsMain.Value = Math.Min(prsMain.Maximum, prsMain.Value + 1);
                        prsMain.Update();

                        if ( (myTaskOrig.Folders[iFldrOrig].m_iLevel > 1) &&
                             (lvCompare.Items[iFldrOrig].BackColor == Color.LightGreen) &&
                             (lvCompare.Items[iFldrOrig - 1].BackColor == Color.LightGreen) &&  // Prior Folder is Equal
                             (myTaskOrig.Folders[iFldrOrig - 1].m_iLevel == 1 ))                // Prior Folder is in Root
                        {
                            lvCompare.Items.RemoveAt(iFldrOrig);
                            myTaskOrig.Folders.RemoveAt(iFldrOrig);
                        }
                        else
                        {
                            iFldrOrig++;
                        }
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

            db.Close();
            db = null;
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

        private void btnBack_Click(object sender, EventArgs e)
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
            }
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
            if (Views.SelectedTab == viewSourceMdb)
            {
                String sFolder = tbSourceMdbFolder.Text;
                lbSourceMdbFolder.Items.Clear();
                if (sFolder.Length > 0)
                {
                    if (sFolder[sFolder.Length - 1] != '\\') sFolder += "\\";

                    if (!System.IO.Directory.Exists(sFolder))
                    {
                        MessageBox.Show("Folder \"" + sFolder + "\" does not exist!");
                    }
                    else
                    {
                        RegistryWrite("Last MDB Folder", sFolder);

                        string[] asFiles = System.IO.Directory.GetFiles(sFolder, "*.mdb");
                        foreach (string sFile in asFiles)
                        {
                            lbSourceMdbFolder.Items.Add(System.IO.Path.GetFileName(sFile));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select a folder first!");
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
                MessageBox.Show("Refresh is not implemented for View \"" + Views.SelectedTab.Text + "\"!");
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
            if (Views.SelectedTab == viewSourceMdb)
            {
                if (lbSourceMdbFolder.SelectedIndex < 0)
                {
                    MessageBox.Show("Select a file first!");
                }
                else
                {
                    String sPath = tbSourceMdbFolder.Text;
                    if (sPath[sPath.Length - 1] != '\\') sPath += "\\";
                    sPath += (string) lbSourceMdbFolder.Items[lbSourceMdbFolder.SelectedIndex];

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
                    MessageBox.Show("Select a task first!");
                }
                else
                {
                    foreach (MyTask myTask in m_Tasks)
                    {
                        if (myTask.m_iTaskID    != (int)lvTaskList.SelectedItems[0].Tag) continue;
                        if (myTask.m_sMachine   != lvTaskList.SelectedItems[0].Text) continue;
                        if (myTask.m_sStatus    != lvTaskList.SelectedItems[0].SubItems[1].Text) continue;
                        if (myTask.m_sStarted   != lvTaskList.SelectedItems[0].SubItems[2].Text) continue;
                        if (myTask.m_sCompleted != lvTaskList.SelectedItems[0].SubItems[3].Text) continue;
                        if (myTask.m_sType      != lvTaskList.SelectedItems[0].SubItems[4].Text) continue;
                        if (myTask.m_sPath      != lvTaskList.SelectedItems[0].SubItems[5].Text) continue;

                        MessageBox.Show("Task has been already added!");
                        return;
                    }

                    int iPos = m_sInterTabArguments.IndexOf("TaskID;");
                    if (iPos >= 0)
                    {
                        m_sInterTabArguments = m_sInterTabArguments.Substring(0, iPos);
                    }
                    if (m_sInterTabArguments.Length > 0) m_sInterTabArguments += ";";
                    m_sInterTabArguments += "TaskID;" + ((int)lvTaskList.SelectedItems[0].Tag).ToString();
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[1].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[2].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[3].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[4].Text;
                    m_sInterTabArguments += ";" + lvTaskList.SelectedItems[0].SubItems[5].Text;

                    GoBackAndRefresh();
                }
            }
            else
            {
                MessageBox.Show("Next is not implemented for View \"" + Views.SelectedTab.Text + "\"!");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (Views.SelectedTab == viewCompare)
            {
                if (m_Tasks.Count > 1)
                {
                    MessageBox.Show("The maximum allowed one more Task has been already added!");
                    return;
                }

                m_ViewStack.Add(Views.SelectedTab);
                Views.SelectedTab = viewSource;

                m_sInterTabArguments = "";
            }
            else
            {
                MessageBox.Show("Add is not implemented for View \"" + Views.SelectedTab.Text + "\"!");
            }

        }
    }
}
