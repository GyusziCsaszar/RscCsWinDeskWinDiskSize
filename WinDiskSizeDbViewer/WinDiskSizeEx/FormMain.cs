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

        protected MyDb m_db = new MyDb();

        public FormMain()
        {
            InitializeComponent();

            // SRC: https://stackoverflow.com/questions/552579/how-to-hide-tabpage-from-tabcontrol
            Views.Appearance = TabAppearance.FlatButtons;
            Views.ItemSize = new Size(0, 1);
            Views.SizeMode = TabSizeMode.Fixed;


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

        private bool ConnectToDb()
        {
            string[] asArgs = m_sInterTabArguments.Split(';');
            if (asArgs.Length < 3)
            {
                MessageBox.Show("Unexpected inter-Tab arguments \"" + m_sInterTabArguments + "\"!");
                return false;
            }

            if (asArgs[0] != "DB")
            {
                MessageBox.Show("Unexpected inter-Tab arguments \"" + m_sInterTabArguments + "\"!");
                return false;
            }

            if (asArgs[1] == "MDB")
            {
                MyMdb mdb = new MyMdb(csMDB_TEMPLATE);
                mdb.Folder = System.IO.Path.GetDirectoryName(asArgs[2]);
                mdb.MdbFile = System.IO.Path.GetFileName(asArgs[2]);

                m_db.Close();
                m_db = mdb;
            }
            else
            {
                MessageBox.Show("Unexpected source type \"" + asArgs[1] + "\"!");
                return false;
            }

            return true;
        }

        private void ListTasks()
        {
            lvTaskList.Items.Clear();
            lvTaskList.Columns.Clear();

            if (!ConnectToDb())
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

            if (!m_db.QueryTasks())
            {
                MessageBox.Show(m_db.LastError);
                return;
            }

            int iRowCount = m_db.RowCount();
            if (iRowCount < 0)
            {
                MessageBox.Show(m_db.LastError);
                return;
            }

            lvTaskList.BeginUpdate();

            for (int iRow = 0; iRow < iRowCount; iRow++)
            {
                int iTaskID = m_db.FieldAsInt(iRow, "ID");

                int iStatus = m_db.FieldAsInt(iRow, "Status");
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

                lvTaskList.Items.Add(m_db.FieldAsString(iRow, "Machine"));

                lvTaskList.Items[iRow].SubItems.Add(sStatus);
                lvTaskList.Items[iRow].SubItems.Add(m_db.FieldAsString(iRow, "StartDate"));
                lvTaskList.Items[iRow].SubItems.Add(m_db.FieldAsString(iRow, "EndDate"));
                lvTaskList.Items[iRow].SubItems.Add(m_db.FieldAsString(iRow, "FolderType"));
                lvTaskList.Items[iRow].SubItems.Add(m_db.FieldAsString(iRow, "FolderPath"));

                lvTaskList.Items[iRow].Tag = iTaskID;
            }

            lvTaskList.EndUpdate();
            lvTaskList.Refresh();
        }

        private void ListFolders()
        {
            lvCompare.Items.Clear();
            lvCompare.Columns.Clear();

            lvCompareTask.Items.Clear();
            lvCompareTask.Columns.Clear();

            if (!ConnectToDb())
            {
                return;
            }

            int iTaskID = -1;
            string[] asArgs = m_sInterTabArguments.Split(';');
            for (int i = 0; i < asArgs.Length; i++)
            {
                if (asArgs[i] == "TaskID")
                {
                    if (Int32.TryParse(asArgs[i + 1], out iTaskID))
                    {
                        break;
                    }
                }
            }
            if (iTaskID < 0)
            {
                MessageBox.Show("Unexpected inter-Tab arguments \"" + m_sInterTabArguments + "\"!");
                return;
            }

            lvCompare.Columns.Add("Compare");
            lvCompare.Columns.Add("Size"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
            lvCompare.Columns.Add("FOLDERS");
            lvCompare.Columns.Add("File Count"); lvCompare.Columns[lvCompare.Columns.Count - 1].TextAlign = HorizontalAlignment.Right;
            lvCompare.Columns.Add("File Date (MIN)");
            lvCompare.Columns.Add("File Date (MAX)");
            lvCompare.Columns.Add("Path");
            lvCompare.Columns.Add("FOLDERS (8.3)");
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

            lvCompareTask.Columns.Add("Machine");
            lvCompareTask.Columns.Add("Status");
            lvCompareTask.Columns.Add("Started");
            lvCompareTask.Columns.Add("Completed");
            lvCompareTask.Columns.Add("Type");
            lvCompareTask.Columns.Add("Root Folder");

          /*int*/ iCol = -1;
            foreach (ColumnHeader ch in lvCompareTask.Columns)
            {
                iCol++;
                int iWidth = RegistryRead("TaskCol" + iCol.ToString(), -1);
                if (iWidth > -1)
                {
                    ch.Width = iWidth;
                }
            }

            if (!m_db.QueryFolders(iTaskID))
            {
                MessageBox.Show(m_db.LastError);
                return;
            }

            int iRowCount = m_db.RowCount();
            if (iRowCount < 0)
            {
                MessageBox.Show(m_db.LastError);
                return;
            }
            if (iRowCount == 0)
            {
                MessageBox.Show("No folders with TaskID = " + iTaskID.ToString() + "!");
                return;
            }

            lvCompare.BeginUpdate();

            prsMain.Visible = true;
            prsMain.Minimum = 0;
            prsMain.Maximum = iRowCount;
            prsMain.Value = 0;
            prsMain.Update();

            List<MyDirItem> aDir = new List<MyDirItem>();
            MyDirItem diNew;

            int     iLevelNext;
            String  sIndent;
            String  sTmp;

            for (int iRow = 0; iRow < iRowCount; iRow++)
            {
                prsMain.Value = prsMain.Value + 1;
                prsMain.Update();

                diNew = new MyDirItem();
                diNew.m_iLevel          = m_db.FieldAsInt(   iRow, "TreeLevel");
                diNew.SizeAsString      = m_db.FieldAsString(iRow, "SizeSUM");
                diNew.CountAsString     = m_db.FieldAsString(iRow, "CountSUM");
                diNew.m_sName = m_db.FieldAsString(iRow, "NameLong");

                sTmp = m_db.FieldAsString(iRow, "MinFileDate");
                if (sTmp.Length > 5)
                {
                    sTmp = sTmp.Insert(4, ".");
                    sTmp = sTmp.Insert(7, ".");
                    sTmp = sTmp.Insert(10, ".");
                }
                diNew.m_sFileDateMin    = sTmp;

                sTmp = m_db.FieldAsString(iRow, "MaxFileDate");
                if (sTmp.Length > 5)
                {
                    sTmp = sTmp.Insert(4, ".");
                    sTmp = sTmp.Insert(7, ".");
                    sTmp = sTmp.Insert(10, ".");
                }
                diNew.m_sFileDateMax = sTmp;

                diNew.m_sPath           = m_db.FieldAsString(iRow, "PathLong");
                diNew.m_sName83         = m_db.FieldAsString(iRow, "NameShort83");
                diNew.m_sPath83         = m_db.FieldAsString(iRow, "PathShort83");

                iLevelNext = -1;
                if (iRow + 1 < iRowCount)
                {
                    iLevelNext = m_db.FieldAsInt(iRow + 1, "TreeLevel");
                }

                lvCompare.Items.Add("");

                sIndent = "";
                for (int i = 0; i < diNew.m_iLevel; i++)
                {
                    //String sTmp = " ɭ ʟ ʈ | ͢   - _ ¯ ʘ ̶  " + m_db.FieldAsString(iRow, "NameLong");

                    if ((i == diNew.m_iLevel - 1) && (((iLevelNext > -1) && (iLevelNext < diNew.m_iLevel)) || iLevelNext == -1))
                    {
                        sIndent += " ʟ  ";
                    }
                    else
                    {
                        sIndent += " |  ";
                    }
                }
                lvCompare.Items[iRow].SubItems.Add(diNew.SizeAsString);
                lvCompare.Items[iRow].SubItems.Add(sIndent + diNew.m_sName);

                lvCompare.Items[iRow].SubItems.Add(diNew.CountAsString);
                lvCompare.Items[iRow].SubItems.Add(diNew.m_sFileDateMin);
                lvCompare.Items[iRow].SubItems.Add(diNew.m_sFileDateMax);

                lvCompare.Items[iRow].SubItems.Add(diNew.m_sPath);

                lvCompare.Items[iRow].SubItems.Add(sIndent + diNew.m_sName83);
                lvCompare.Items[iRow].SubItems.Add(diNew.m_sPath83);
            }

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

        private void btnBack_Click(object sender, EventArgs e)
        {
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
                    int iPos = m_sInterTabArguments.IndexOf("TaskID;");
                    if (iPos >= 0)
                    {
                        m_sInterTabArguments = m_sInterTabArguments.Substring(0, iPos);
                    }
                    if (m_sInterTabArguments.Length > 0) m_sInterTabArguments += ";";
                    m_sInterTabArguments += "TaskID;" + ((int)lvTaskList.SelectedItems[0].Tag).ToString();

                    GoBackAndRefresh();
                }
            }
            else
            {
                MessageBox.Show("Next is not implemented for View \"" + Views.SelectedTab.Text + "\"!");
            }
        }
    }
}
