using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

using EnumerateFile;

namespace WinDiskSize
{
    public partial class FormMain : Form
    {

        protected const string csAPP_TITLE = "Win Disk Size v3.03";

        protected const String csMDB_TEMPLATE = "WinDiskSize_Template.mdb";

        protected List<MyDirInfo> aDi;

        protected System.Windows.Forms.Timer tmrWalk;

        protected bool m_bStop;
        protected int m_iFilterValue;
        protected int m_iFreez;

        StatusBar mainStatusBar;
        StatusBarPanel sbPanelLb;
        StatusBarPanel sbPanelLevel;

        protected MyDb m_db = new MyDb();

        protected string m_sStorageLabel;

        public FormMain()
        {
            InitializeComponent();

            this.Text = csAPP_TITLE;

            mainStatusBar = new StatusBar();
            //
            sbPanelLb = new StatusBarPanel();
            sbPanelLb.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            sbPanelLb.Text = "Item Count: 0";
            sbPanelLb.ToolTipText = "ListBox's item count";
            sbPanelLb.AutoSize = StatusBarPanelAutoSize.Spring;
            mainStatusBar.Panels.Add(sbPanelLb);
            //
            sbPanelLevel = new StatusBarPanel();
            sbPanelLevel.BorderStyle = StatusBarPanelBorderStyle.Sunken; // Raised;
            sbPanelLevel.Text = "Max Level: N/A";
            sbPanelLevel.ToolTipText = "Maximum level (depth) parsed";
            sbPanelLevel.AutoSize = StatusBarPanelAutoSize.Contents;
            mainStatusBar.Panels.Add(sbPanelLevel);
            //
            mainStatusBar.ShowPanels = true;
            Controls.Add(mainStatusBar);

          //cbDrive.SelectedIndex = -1;
            foreach (System.IO.DriveInfo drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    cbDrive.Items.Add(drive.Name.Substring(0, 2));
                }
            }

            cbUnit.SelectedIndex = 2;

            cbFreez.SelectedIndex = 0;

            lblPercent.Text = "";
            sbPanelLb.Text = "Item Count: 0";
            sbPanelLevel.Text = "Max Level: N/A";

            m_iFilterValue = 100;
            tbFilterValue.Text = m_iFilterValue.ToString();

            m_iFreez = 500;

            aDi = new List<MyDirInfo>();

            tmrWalk = new System.Windows.Forms.Timer();
            tmrWalk.Interval = 250;
            tmrWalk.Tick += new EventHandler(tmrWalk_Tick);

            this.FormClosing += new FormClosingEventHandler(FormMain_FormClosing);
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.Left = Math.Max(0, RegistryRead("Main_Left", this.Left));
            this.Top = Math.Max(0, RegistryRead("Main_Top", this.Top));
            this.Width = Math.Max(800, RegistryRead("Main_Width", this.Width));
            this.Height = Math.Max(500, RegistryRead("Main_Height", this.Height));

            tbExcludedAlways.Text    = RegistryRead("Exclude Always",      tbExcludedAlways.Text);
            tbExcludeSysFolders.Text = RegistryRead("Exclude Sys Folders", tbExcludeSysFolders.Text);

            tbStartFolder.Text = RegistryRead("Last Start Folder", "");

            if (tbStartFolder.Text.Length > 0 && cbDrive.SelectedIndex < 0)
            {
                string sTmp = tbStartFolder.Text;
                sTmp = sTmp.Replace("\\", "_");
                sTmp = sTmp.Replace(":", "_");
                try
                {
                    int iChecked = 0;
                    iChecked = RegistryRead("Exclude Sys Folders(" + sTmp + ")", iChecked);
                    if (iChecked > 0)
                    {
                        chbExcludeSysFolders.Checked = true;
                    }
                    else
                    {
                        chbExcludeSysFolders.Checked = false;
                    }
                }
                catch (Exception)
                {
                }
            }

            tbServer.Text   = RegistryRead("Last SQL Server",   tbServer.Text);
            tbDb.Text       = RegistryRead("Last SQL Db",       tbDb.Text);
            tbUser.Text     = RegistryRead("Last SQL User",     tbUser.Text);
            tbPw.Text       = RegistryRead("Last SQL Pw",       tbPw.Text);
            chbStorePw.Checked = tbPw.Text.Length > 0;

            string sLastMdbCsvFolder = RegistryRead("Last MDB and CSV Folder", "");
            if (sLastMdbCsvFolder.Length == 0)
            {
                //Backward compatibility...
                sLastMdbCsvFolder = RegistryRead("Last MDB Folder", "");
            }
            if (sLastMdbCsvFolder.Length > 0)
            {
                tbMdbCsvFolderPath.Text = sLastMdbCsvFolder;
            }

            int iChk;

            iChk = RegistryRead("MDB Instead", 0 /*false*/);
            if (iChk > 0) chbMdb.Checked = true;

            iChk = RegistryRead("CSV Files Instead", 0 /*false*/);
            if (iChk > 0) chbCsvFiles.Checked = true;

            tbCsvEol.Text = RegistryRead("CSV File EOL", "\\r\\n");

            tbCsvDelim.Text = RegistryRead("CSV File Delimiter", ";");

            tbCsvFldrLstDelim.Text = RegistryRead("CSV File Folder List Delimiter", "\\");

            iChk = RegistryRead("CSV Files CRC32", 0 /*false*/);
            if (iChk > 0) chbCsvCrc32.Checked = true;
        }

        void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled)
                {
                    m_bStop = true;
                    e.Cancel = true;
                }
            }

            if (!e.Cancel)
            {
                m_db.Close();

                RegistryWrite("Exclude Always",      tbExcludedAlways.Text);
                RegistryWrite("Exclude Sys Folders", tbExcludeSysFolders.Text);

                if (tbStartFolder.Text.Length > 0 && cbDrive.SelectedIndex < 0)
                {
                    string sTmp = tbStartFolder.Text;
                    sTmp = sTmp.Replace("\\", "_");
                    sTmp = sTmp.Replace(":", "_");
                    try
                    {
                        int iChecked = 0;
                        if (chbExcludeSysFolders.Checked) iChecked = 1;
                        RegistryWrite("Exclude Sys Folders(" + sTmp + ")", iChecked);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (this.Left >= 0) RegistryWrite("Main_Left", this.Left);
                if (this.Top >= 0) RegistryWrite("Main_Top", this.Top);
                if (this.Width >= 800) RegistryWrite("Main_Width", this.Width);
                if (this.Height >= 500) RegistryWrite("Main_Height", this.Height);
            }
        }

        private void RegistryWrite(string sName, string sValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ressive.Hu\\WinDiskSize");
            key.SetValue(sName, sValue);
            key.Dispose();
        }

        private void RegistryWrite(string sName, int iValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ressive.Hu\\WinDiskSize");
            key.SetValue(sName, iValue);
            key.Dispose();
        }

        private string RegistryRead(string sName, string sDefaultValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ressive.Hu\\WinDiskSize");
            string sValue = (string)key.GetValue(sName, sDefaultValue);
            key.Dispose();

            return sValue;
        }

        private int RegistryRead(string sName, int iDefaultValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ressive.Hu\\WinDiskSize");
            int iValue = (int)key.GetValue(sName, iDefaultValue);
            key.Dispose();

            return iValue;
        }

        void tmrWalk_Tick(object sender, EventArgs e)
        {
            tmrWalk.Stop();

            int iTickCountStart = Environment.TickCount;

            int iEnumCnt = 0;
            for (; ; )
            {
                bool bEnumCalled = false;

                int iIdx = -1;
                foreach (MyDirInfo di in aDi)
                {
                    iIdx++;
                    if (di.bParsed) continue;

                    bEnumCalled = true;
                    EnumFolder(di, iIdx);
                    iEnumCnt++;

                    break;
                }

                //Adding item to show SumOfFileSizes in root folder
                if (iIdx == 0)
                {
                    MyDirInfo diDrive = aDi[0];

                    MyDirInfo di = new MyDirInfo();
                    di.sStartFolder = "";

                    di.sNameLong = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);
                    di.sNameShort83 = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);
                    di.sPathLong = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);
                    di.sPathShort83 = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);

                    di.AddFileSize(diDrive.GetSizeSUM());
                    di.AddFileCount(diDrive.GetCountSUM());

                    di.AddFileSizeSUM(diDrive.GetSizeSUM());
                    di.AddFileCountSUM(diDrive.GetCountSUM());

                    diDrive.CopyChangeDateTo(di);
                    di.bParsed = true;  //ATTN: Without this duplicated parsing occures!!!
                    di.bHidden = true;    //ATTN: Exclude this from counting files!!!
                    aDi.Add(di);

                }

                if (!bEnumCalled) break;

                if ((Environment.TickCount - iTickCountStart) > m_iFreez) break;
            }


            int iCnt = 0;
            int iCntParsed = 0;
            foreach (MyDirInfo di in aDi)
            {
                if (!di.bHidden) iCnt++;
                if (di.bParsed)
                {
                    if (!di.bHidden) iCntParsed++;
                    continue;
                }
            }
            String sPercent = iCntParsed.ToString() + " of " + iCnt.ToString() + " folder(s)";
            Double d = 0.0;
            if (iCnt > 0)
            {
                d = iCntParsed;
                d = d / ((Double)iCnt);
            }
            int i = ((int) (d * 100.0));
            sPercent += " ( " + i.ToString() + " % )";
            sPercent += " [" + iEnumCnt.ToString() + "]";
            lblPercent.Text = sPercent;

            ListBoxRefresh();

            if (m_bStop)
            {
                lbDirList.BackColor = Color.LightPink;

                lblPercent.ForeColor = Color.Red;

                lblPercent.Text = sPercent + " (stopped)";

                btnParse.Enabled = true;
                btnStop.Enabled = false;
            }
            else
            {
                btnStop.Enabled = true;

                if (iCntParsed == iCnt)
                {
                    lbDirList.BackColor = Color.LightGreen;

                    lblPercent.Text = sPercent + " (DONE)";

                    btnStop.Enabled = false;

                    CopyToDatabase();
                }
                else
                {
                    lbDirList.BackColor = Color.LightYellow;

                    tmrWalk.Start();
                }
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            if (tmrWalk.Enabled) return;

            if (!m_db.IsReady)
            {
                if (tbMdbCsvFolderPath.Text.Length > 0)
                {
                    if (DialogResult.Yes != MessageBox.Show("INFO: There are Database connection information specified\n(MDB Path: \"" + tbMdbCsvFolderPath.Text + "\")\nBUT not connected to Database!\n\nDo you want to continue?", csAPP_TITLE, MessageBoxButtons.YesNo))
                    {
                        return;
                    }
                }
                else if (tbServer.Text.Length > 0 && tbDb.Text.Length > 0 && tbUser.Text.Length > 0)
                {
                    if (DialogResult.Yes != MessageBox.Show("INFO: There are Database connection information specified\n(SQL Server: \"" + tbServer.Text + "\")\nBUT not connected to Database!\n\nDo you want to continue?", csAPP_TITLE, MessageBoxButtons.YesNo))
                    {
                        return;
                    }
                }
            }

            string sFolderType = "";
            string sFolderPath = "";
            if (this.cbDrive.SelectedIndex >= 0)
            {
                sFolderType = "Drive";
                sFolderPath = cbDrive.Text;

                if (DialogResult.Yes != MessageBox.Show("INFO: Folder Path is also specified!\n" + sFolderType + " " + sFolderPath + " will be parsed!\n\nDo you want to continue?", csAPP_TITLE, MessageBoxButtons.YesNo))
                {
                    return;
                }
            }
            else
            {
                sFolderType = "Folder";
                sFolderPath = tbStartFolder.Text;
            }
            if (sFolderPath.Length == 0)
            {
                MessageBox.Show("No Drive nor Folder specified!", csAPP_TITLE);
                return;
            }

            m_sStorageLabel = "";
            string sLabelPath = sFolderPath.Substring(0, 2) + "\\";
            sLabelPath += "WinDiskSize_LABEL.txt";
            if (System.IO.File.Exists(sLabelPath))
            {
                m_sStorageLabel = System.IO.File.ReadAllText(sLabelPath);
            }
            else
            {
                if (DialogResult.Yes != MessageBox.Show("INFO: The optional LABEL file \"" + sLabelPath + "\" does not exist!\n\nDo you want to continue?", csAPP_TITLE, MessageBoxButtons.YesNo))
                {
                    return;
                }
            }

            string sStorageSize = "";
            string sStorageFree = "";
            foreach (System.IO.DriveInfo drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == sFolderPath.Substring(0, 2) + "\\")
                {
                    sStorageSize = drive.TotalSize.ToString();
                    sStorageFree     = drive.TotalFreeSpace.ToString();

                    break;
                }
            }

            if (m_db.IsReady)
            {
                if (!m_db.BeginTask(sFolderType, sFolderPath, m_sStorageLabel, sStorageSize, sStorageFree))
                {
                    MessageBox.Show(m_db.LastError, csAPP_TITLE);
                    return;
                }
                else
                {
                    tbTaskID.Text = m_db.TaskID.ToString();
                }
            }

            btnStop.Enabled = false;
            btnParse.Enabled = false;
            m_bStop = false;

            lblPercent.Text = "";
            sbPanelLb.Text = "Item Count: 0";
            sbPanelLevel.Text = "Max Level: N/A";

            aDi.Clear();

            string sLabel = m_sStorageLabel;
            if (sLabel.Length == 0)
            {
                sLabel = "[none]";
            }
            this.Text = csAPP_TITLE + " - " + sFolderType + ": " + sFolderPath + " - " + sLabel;

            String sStartFolder = sFolderPath + "\\";

            MyDirInfo di = new MyDirInfo();
            di.sStartFolder = sStartFolder; //ATTN: Cosmetic only!!!
            di.sNameLong    = sStartFolder;
            di.sNameShort83 = sStartFolder;
            di.sPathLong    = sStartFolder;
            di.sPathShort83 = sStartFolder;
            aDi.Add(di);

            lbDirList.BackColor = Color.LightYellow;

            ListBoxRefresh();

            btnParse_Click(null, EventArgs.Empty);
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            if (tmrWalk.Enabled) return;

            btnStop.Enabled = false;
            btnParse.Enabled = false;
            m_bStop = false;

            lblPercent.ForeColor = Color.Green;

            tmrWalk.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            m_bStop = true;
        }

        private void EnumFolder(MyDirInfo di, int iDirInfoIdx)
        {
            string sExclAlways = "";
            if (tbExcludedAlways.Text.Length > 0)
            {
                sExclAlways = ";" + tbExcludedAlways.Text.ToLower() + ";";

                sExclAlways = sExclAlways.Replace("; ", ";");
                sExclAlways = sExclAlways.Replace(" ;", ";");
            }

            string sExclSysFolders = "";
            if (chbExcludeSysFolders.Checked && tbExcludeSysFolders.Text.Length > 0)
            {
                sExclSysFolders = ";" + tbExcludeSysFolders.Text.ToLower() + ";";

                sExclSysFolders = sExclSysFolders.Replace("; ", ";");
                sExclSysFolders = sExclSysFolders.Replace(" ;", ";");
            }

            FileDirectoryEnumerable myEnum = new FileDirectoryEnumerable();

            if (chbShort83.Checked)
            {
                myEnum.SearchPath    = di.sPathShort83; //ATTN!!! HAS TO BE ENDED WITH \!!! Otherwise default directory of the drive will be the start direcotry!!!
            }
            else
            {
                myEnum.SearchPath    = di.sPathLong;
            }

            myEnum.SearchPattern = "*.*";

            myEnum.ThrowIOException = false; //ATTN!!!

            myEnum.SearchForFile = false;
            myEnum.SearchForDirectory = true;
            myEnum.ReturnStringType = false;
            myEnum.SearchPattern = "*.*";
            int iSubIdx = iDirInfoIdx;
            foreach (System.IO.DirectoryInfo dir in myEnum)
            {
              //if (chbExcludeSysFolders.Checked)
                {
                    /*
                    if (
                            ((!di.bShow83) && (di.sPathLong.Substring(   di.sStartFolder.Length - 1) == "\\")) ||
                            (  di.bShow83  && (di.sPathShort83.Substring(di.sStartFolder.Length - 1) == "\\"))
                       )
                    */
                    if (di.iLevel == 0)
                    {
                        string sNameSm = ";" + dir.Name.ToLower() + ";";

                        /*
                        if ( (sNameSm == "windows") ||
                             (sNameSm == "program files") ||
                             (sNameSm == "program files (x86)") ||
                             (sNameSm == "programdata") ||
                             (sNameSm == "users") ||
                             (sNameSm == "system volume information") ||
                             (sNameSm == "$recycle.bin")
                           )
                        */
                        if (
                             (sExclAlways.IndexOf(sNameSm) >= 0) ||
                             (chbExcludeSysFolders.Checked && sExclSysFolders.IndexOf(sNameSm) >= 0)
                           )
                        {
                            continue;
                        }
                    }
                }

                iSubIdx++;

                MyDirInfo diSub = new MyDirInfo();
                diSub.diParent = di;
                diSub.iLevel = di.iLevel + 1;
                diSub.sStartFolder = di.sStartFolder;

                diSub.sNameLong = dir.Name;

                diSub.sNameShort83 = "";
                String s = dir.ToString();
                int i = s.LastIndexOf('\\');
                if (i >= 0)
                {
                    diSub.sNameShort83 = s.Substring(i + 1);
                }
                else
                {
                    diSub.sNameShort83 = dir.Name; //Not to fail!!!
                }

                //FIX
                //diSub.sPathLong = di.sPathLong + "\\" + diSub.sNameLong;
                //diSub.sPathShort83 = di.sPathShort83 + "\\" + diSub.sNameShort83;
                diSub.sPathLong = di.sPathLong + diSub.sNameLong + "\\";
                diSub.sPathShort83 = di.sPathShort83 + diSub.sNameShort83 + "\\";

                aDi.Insert(iSubIdx, diSub);
            }

            myEnum.SearchForFile = true;
            myEnum.SearchForDirectory = false;
            myEnum.ReturnStringType = false;
            myEnum.SearchPattern = "*.*";
            //WinXHome FIX
            //foreach (System.IO.FileInfo file in myEnum)
            foreach (LongFileInfo file in myEnum)
            {
                di.AddFileSize(file.Length);
                di.AddFileCount(1);

                di.AddFileSizeSUM(file.Length);
                di.AddFileCountSUM(1);

                di.AddFileChangeDate(file.CreationTime);
                di.AddFileChangeDate(file.LastWriteTime);

                if (m_db != null)
                {
                    if (m_db.IsReady)
                    {
                        string sPathLong;
                        if (di.sStartFolder.Length > 0)
                        {
                            sPathLong = di.sPathLong.Substring(di.sStartFolder.Length - 1);
                        }
                        else
                        {
                            sPathLong = di.sPathLong;
                        }

                        if (sPathLong.Length > 0)
                        {
                            if (sPathLong[0] == '\\')
                            {
                                sPathLong = sPathLong.Substring(1);
                            }
                        }

                        if (sPathLong.Length > 0)
                        {
                            if (sPathLong[sPathLong.Length - 1] == '\\')
                            {
                                sPathLong = sPathLong.Substring(0, sPathLong.Length - 1);
                            }
                        }

                        m_db.AddFileRAW(sPathLong, file.Name, file.Length, file.CreationTime, file.LastWriteTime);
                    }
                }
            }

            di.bParsed = true;

            myEnum.Close();
        }

        private void ListBoxRefresh()
        {
            int iMaxLevelFilter = Int32.Parse(txLevel.Text);

            Int64 i64SizeFilter = 0;
            if (m_iFilterValue > 0)
            {
                i64SizeFilter = m_iFilterValue;

                for(int iMul = 0; iMul < cbUnit.SelectedIndex; iMul++)
                {
                    i64SizeFilter = i64SizeFilter * ((Int64) 1024);
                }
            }

            int iMaxLevel = -1;

            lbDirList.BeginUpdate();

            lbDirList.Items.Clear();
            if (aDi != null)
            {
                bool bFirst = true;
                foreach (MyDirInfo di in aDi)
                {
                    if (bFirst || /*(!di.bParsed) ||*/ (di.iLevel <= iMaxLevelFilter || iMaxLevelFilter == 0))
                    {
                        if (bFirst || (!di.bParsed) || di.GetSizeSUM() >= i64SizeFilter)
                        {
                            di.bShow83 = chbShort83.Checked;
                            lbDirList.Items.Add(di);
                        }
                    }

                    iMaxLevel = Math.Max(iMaxLevel, di.iLevel);

                    bFirst = false;
                }
            }

            lbDirList.EndUpdate();

            sbPanelLb.Text = "Item Count: " + lbDirList.Items.Count.ToString();
            sbPanelLevel.Text = "Max Level: " + iMaxLevel.ToString();
        }

        private void btnDecLevel_Click(object sender, EventArgs e)
        {
            int iMaxLevel = Int32.Parse(txLevel.Text);
            if (iMaxLevel > 0)
            {
                iMaxLevel--;
                txLevel.Text = iMaxLevel.ToString();
            }

            ListBoxRefresh();
        }

        private void btnIncLevel_Click(object sender, EventArgs e)
        {
            int iMaxLevel = Int32.Parse(txLevel.Text);
            //if (iMaxLevel > 0)
            {
                iMaxLevel++;
                txLevel.Text = iMaxLevel.ToString();
            }

            ListBoxRefresh();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled)
                {
                    m_bStop = true;
                    return;
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("WinDiskSize Results (");
            sb.Append(DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString() + ")\r\n\r\n");

            sb.Append("Level Filter: " + txLevel.Text + "\r\n");

            sb.Append("Size Filter: ");
            if (m_iFilterValue > 0)
                sb.Append(m_iFilterValue.ToString() + " " + cbUnit.Text);
            else
                sb.Append("<none>");
            sb.Append("\r\n\r\n");

            String sStartFolder;
            if (cbDrive.SelectedIndex >= 0)
            {
                sStartFolder = cbDrive.Text + "\\";
            }
            else
            {
                sStartFolder = tbStartFolder.Text + "\\";
            }
            sb.Append("Start Folder: " + sStartFolder + "\r\n\r\n");

            sb.Append(tbCaption.Text + "\r\n\r\n");

            for (int i = 0; i < lbDirList.Items.Count; i++)
            {
                if (i > 0) sb.Append("\r\n");
                sb.Append(lbDirList.Items[i].ToString());
            }

            sb.Append("\r\n\r\n<END OF FILE>");

            System.Windows.Forms.Clipboard.SetText(sb.ToString());
        }

        private void tbFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Number only
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void tbFilterValue_Enter(object sender, EventArgs e)
        {
            tbFilterValue.Text = "";
        }

        private void tbFilterValue_Leave(object sender, EventArgs e)
        {
            if (tbFilterValue.Text == "")
            {
                tbFilterValue.Text = m_iFilterValue.ToString();
            }
            else
            {
                int iValue = -1;
                if (!int.TryParse(tbFilterValue.Text, out iValue))
                {
                    tbFilterValue.Text = m_iFilterValue.ToString();
                }
                else
                {
                    m_iFilterValue = iValue;
                    ListBoxRefresh();
                }
            }
        }

        private void cbUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBoxRefresh();
        }

        private void cbFreez_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_iFreez = Int32.Parse(cbFreez.Text);
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {

            m_iFilterValue = 0;
            tbFilterValue.Text = m_iFilterValue.ToString();

            txLevel.Text = "1";

            ListBoxRefresh();

        }

        private void cbDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled) return;
            }

            if (cbDrive.SelectedIndex >= 0)
            {
                string sTmp = cbDrive.Text;
                sTmp = sTmp.Replace("\\", "_");
                sTmp = sTmp.Replace(":", "_");
                try
                {
                    int iChecked = 0;
                    iChecked = RegistryRead("Exclude Sys Folders(" + sTmp + ")", iChecked);
                    if (iChecked > 0)
                    {
                        if (DialogResult.Yes == MessageBox.Show("Also exclude has been checked for this drive last time!\n\nDo you want to check it now?", csAPP_TITLE, MessageBoxButtons.YesNo))
                        {
                            chbExcludeSysFolders.Checked = true;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            if (cbDrive.SelectedIndex >= 0)
            {
                string sTmp = cbDrive.Text;
                sTmp = sTmp.Replace("\\", "_");
                sTmp = sTmp.Replace(":", "_");
                try
                {
                    int iChecked = 0;
                    if (chbExcludeSysFolders.Checked) iChecked = 1;
                    RegistryWrite("Exclude Sys Folders(" + sTmp + ")", iChecked);
                }
                catch (Exception)
                {
                }
            }

            this.Text = csAPP_TITLE;

            btnStop.Enabled = false;
            btnParse.Enabled = false;
            m_bStop = false;

            lblPercent.Text = "";
            sbPanelLb.Text = "Item Count: 0";
            sbPanelLevel.Text = "Max Level: N/A";

            if (aDi != null)
            {
                aDi.Clear();
            }

            lbDirList.BackColor = Color.LightYellow;

            if (aDi != null) ListBoxRefresh();

            if (cbDrive.SelectedIndex >= 0)
            {
                btnList.PerformClick();
            }
        }

        private void btnStartFolder_Click(object sender, EventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled) return;
            }

            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                cbDrive.SelectedIndex = -1;

                tbStartFolder.Text = dlg.SelectedPath;

                RegistryWrite("Last Start Folder", tbStartFolder.Text);

                chbExcludeSysFolders.Checked = false;
                if (tbStartFolder.Text.Length > 0 && cbDrive.SelectedIndex < 0)
                {
                    string sTmp = tbStartFolder.Text;
                    sTmp = sTmp.Replace("\\", "_");
                    sTmp = sTmp.Replace(":", "_");
                    try
                    {
                        int iChecked = 0;
                        iChecked = RegistryRead("Exclude Sys Folders(" + sTmp + ")", iChecked);
                        if (iChecked > 0)
                        {
                            if (DialogResult.Yes == MessageBox.Show("Also exclude has been checked for this folder last time!\n\nDo you want to check it now?", csAPP_TITLE, MessageBoxButtons.YesNo))
                            {
                                chbExcludeSysFolders.Checked = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                btnList_Click(null, EventArgs.Empty);
            }
        }

        private void chbShort83_Click(object sender, EventArgs e)
        {
            ListBoxRefresh();
        }

        private void btnDbConnect_Click(object sender, EventArgs e)
        {

            tbTaskID.Text = "";

            if (chbMdb.Checked) //tbMdbPath.Text.Length > 0)
            {
                if (!System.IO.Directory.Exists(tbMdbCsvFolderPath.Text))
                {
                    MessageBox.Show("MDB Folder Path \"" + tbMdbCsvFolderPath.Text + "\" does not exist!", csAPP_TITLE);
                }
                else
                {
                    String sMdbTemplatePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                    if (sMdbTemplatePath[sMdbTemplatePath.Length - 1] != '\\') sMdbTemplatePath += "\\";
                    sMdbTemplatePath += csMDB_TEMPLATE;

                    if (!System.IO.File.Exists(sMdbTemplatePath))
                    {
                        MessageBox.Show("Template MDB Database \"" + sMdbTemplatePath + "\" does not exist!", csAPP_TITLE);
                    }
                    else
                    {
                        MyMdb mdb = new MyMdb();
                        mdb.Folder = tbMdbCsvFolderPath.Text;
                        mdb.MdbTemplatePath = sMdbTemplatePath;

                        m_db.Close();
                        m_db = mdb;

                        btnDbConnect.Enabled = false;
                        btnDbConnect.Visible = false;

                        chbMdb.Enabled = false;
                        chbCsvFiles.Enabled = false;

                        RegistryWrite("Last MDB and CSV Folder", tbMdbCsvFolderPath.Text);
                    }
                }
            }
            else if (chbCsvFiles.Checked)
            {
                if (!System.IO.Directory.Exists(tbMdbCsvFolderPath.Text))
                {
                    MessageBox.Show("CSV Folder Path \"" + tbMdbCsvFolderPath.Text + "\" does not exist!", csAPP_TITLE);
                }
                else
                {
                    MyCsv csv = new MyCsv();
                    csv.Folder = tbMdbCsvFolderPath.Text;

                    string sTmp;

                    sTmp = tbCsvEol.Text;
                    sTmp = sTmp.Replace("\\r", "\r");
                    sTmp = sTmp.Replace("\\n", "\n");
                    sTmp = sTmp.Replace("\\t", "\t");
                    if (sTmp.Length > 0)
                    {
                        csv.EOL = sTmp;
                    }

                    sTmp = tbCsvDelim.Text;
                    sTmp = sTmp.Replace("\\r", "\r");
                    sTmp = sTmp.Replace("\\n", "\n");
                    sTmp = sTmp.Replace("\\t", "\t");
                    if (sTmp.Length > 0)
                    {
                        csv.Delimiter = sTmp;
                    }

                    if (tbCsvFldrLstDelim.Text.Length > 0)
                    {
                        csv.FolderListDelimiter = tbCsvFldrLstDelim.Text;
                    }

                    csv.CRC32 = chbCsvCrc32.Checked;

                    m_db.Close();
                    m_db = csv;

                    btnDbConnect.Enabled = false;
                    btnDbConnect.Visible = false;

                    chbMdb.Enabled = false;
                    chbCsvFiles.Enabled = false;

                    RegistryWrite("Last MDB and CSV Folder", tbMdbCsvFolderPath.Text);

                    RegistryWrite("CSV File EOL", tbCsvEol.Text);
                    RegistryWrite("CSV File Delimiter", tbCsvDelim.Text);
                    RegistryWrite("CSV File Folder List Delimiter", tbCsvFldrLstDelim.Text);
                }
            }
            else
            {
                RegistryWrite("Last SQL Server", tbServer.Text);
                RegistryWrite("Last SQL Db", tbDb.Text);
                RegistryWrite("Last SQL User", tbUser.Text);

                if (chbStorePw.Checked)
                {
                    RegistryWrite("Last SQL Pw", tbPw.Text);
                }

                MySqlServer sqlsvr = new MySqlServer();
                if (!sqlsvr.TestConnect(tbServer.Text, tbDb.Text, tbUser.Text, tbPw.Text))
                {
                    MessageBox.Show(sqlsvr.LastError, csAPP_TITLE);
                }
                else
                {
                    m_db.Close();
                    m_db = sqlsvr;

                    btnDbConnect.Enabled = false;
                    btnDbConnect.Visible = false;
                }
            }

        }

        private void chbExcludeSysFolders_CheckedChanged(object sender, EventArgs e)
        {
            if (chbExcludeSysFolders.Checked)
            {
                chbExcludeSysFolders.ForeColor = Color.Red;
            }
            else
            {
                chbExcludeSysFolders.ForeColor = Color.Black;
            }
        }

        private void CopyToDatabase()
        {
            if (aDi == null) return;
            if (aDi.Count == 0) return;
            if (!m_db.IsReady) return;

            prsSqlSvr.Visible = true;
            prsSqlSvr.Minimum = 0;
            prsSqlSvr.Maximum = aDi.Count;
            prsSqlSvr.Value = 0;
            prsSqlSvr.Update();

            long lCnt = -1;
            foreach (MyDirInfo di in aDi)
            {
                lCnt++;

                prsSqlSvr.Value = prsSqlSvr.Value + 1;
                prsSqlSvr.Update();

                String sNameShort83 = "";
                String sPathShort83 = "";
                String sNameLong    = "";
                String sPathLong    = "";
                if (di.sPathLong != "files_in_StartFolder")
                {
                    if (di.sStartFolder.Length > 0)
                    {
                        sPathShort83 = di.sPathShort83.Substring(di.sStartFolder.Length - 1);
                    }
                    else
                    {
                        sPathShort83 = di.sPathShort83;
                    }

                    if (sPathShort83 == "\\")
                        sNameShort83 = ""; //"\\";
                    else
                        sNameShort83 = di.sNameShort83;

                    if (di.sStartFolder.Length > 0)
                    {
                        sPathLong = di.sPathLong.Substring(di.sStartFolder.Length - 1);
                    }
                    else
                    {
                        sPathLong = di.sPathLong;
                    }

                    if (sPathLong == "\\")
                        sNameLong = ""; //"\\";
                    else
                        sNameLong = di.sNameLong;
                }

                string sMaxFileDate = "";
                if (di.dtYoungestFile_Valid)
                {
                    sMaxFileDate  = "'";
                    sMaxFileDate += di.dtYoungestFile.Year.ToString();

                    if (di.dtYoungestFile.Month < 10)
                        sMaxFileDate += "0" + di.dtYoungestFile.Month.ToString();
                    else
                        sMaxFileDate += di.dtYoungestFile.Month.ToString();

                    if (di.dtYoungestFile.Day < 10)
                        sMaxFileDate += "0" + di.dtYoungestFile.Day.ToString();
                    else
                        sMaxFileDate += di.dtYoungestFile.Day.ToString();

                    sMaxFileDate += " ";
                    sMaxFileDate += di.dtYoungestFile.ToLongTimeString();

                    sMaxFileDate += "'";
                }
                else
                {
                    sMaxFileDate = "NULL";
                }

                string sMinFileDate = "";
                if (di.dtOldestFile_Valid)
                {
                    sMinFileDate = "'";
                    sMinFileDate += di.dtOldestFile.Year.ToString();

                    if (di.dtOldestFile.Month < 10)
                        sMinFileDate += "0" + di.dtOldestFile.Month.ToString();
                    else
                        sMinFileDate += di.dtOldestFile.Month.ToString();

                    if (di.dtOldestFile.Day < 10)
                        sMinFileDate += "0" + di.dtOldestFile.Day.ToString();
                    else
                        sMinFileDate += di.dtOldestFile.Day.ToString();

                    sMinFileDate += " ";
                    sMinFileDate += di.dtOldestFile.ToLongTimeString();

                    sMinFileDate += "'";
                }
                else
                {
                    sMinFileDate = "NULL";
                }

                if (!m_db.AddFolderRAW(di.iLevel, di.GetCount().ToString(), di.GetCountSUM().ToString(), di.GetSize().ToString(), di.GetSizeSUM().ToString(), sMinFileDate, sMaxFileDate, sNameShort83, sPathShort83, sNameLong, sPathLong))
                {
                    break;
                }
            }

            prsSqlSvr.Visible = false;
            prsSqlSvr.Update();

            if (m_db.HasLastError)
            {
                MessageBox.Show(m_db.LastError, csAPP_TITLE);
                return;
            }

            if (m_db.EndTask())
            {
                MessageBox.Show("Folder Listing has been successfully transferred to specified Database!\n(Item Count: " + lCnt.ToString() + ")\n\nThe App will now exit!", csAPP_TITLE);
                Close();
            }
            else
            {
                MessageBox.Show(m_db.LastError, csAPP_TITLE);
            }


        }

        private void btnMdbPath_Click(object sender, EventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled) return;
            }

            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbMdbCsvFolderPath.Text = dlg.SelectedPath;

                tbServer.Enabled = false;
                tbDb.Enabled = false;
                tbUser.Enabled = false;
                tbPw.Enabled = false;

                chbMdb.Checked = true;

                btnDbConnect.PerformClick();
            }
        }

        private void tbSqlAny_TextChanged(object sender, EventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled) return;
            }

            //ROLLBACK!
            /*
            tbMdbPath.Text = "";
            */
        }

        private void btnClearStartFolder_Click(object sender, EventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled) return;
            }

            tbStartFolder.Text = "";
        }

        private void btnClearDrive_Click(object sender, EventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled) return;
            }

            cbDrive.SelectedIndex = -1;
        }

        private void btnClearMdbPath_Click(object sender, EventArgs e)
        {
            if (tmrWalk != null)
            {
                if (tmrWalk.Enabled) return;
            }

            tbMdbCsvFolderPath.Text = "";

            RegistryWrite("Last MDB and CSV Folder", tbMdbCsvFolderPath.Text);

            chbMdb.Checked = false;
        }

        private void chbMdbPath_CheckedChanged(object sender, EventArgs e)
        {
            if (chbMdb.Checked)
            {
                chbCsvFiles.Checked = false;

                chbMdb.ForeColor = Color.Red;
            }
            else
            {
                chbMdb.ForeColor = Color.RoyalBlue;
            }

            int iChk = 0;
            if (chbMdb.Checked) iChk = 1;
            RegistryWrite("MDB Instead", iChk);
        }

        private void chbStorePw_CheckedChanged(object sender, EventArgs e)
        {
            if (chbStorePw.Checked)
            {
                chbStorePw.ForeColor = Color.Red;
            }
            else
            {
                chbStorePw.ForeColor = Color.Black;

                RegistryWrite("Last SQL Pw", "");
            }
        }

        private void chbCsvFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (chbCsvFiles.Checked)
            {
                chbMdb.Checked = false;

                chbCsvFiles.ForeColor = Color.Red;
            }
            else
            {
                chbCsvFiles.ForeColor = Color.RoyalBlue;
            }

            int iChk = 0;
            if (chbCsvFiles.Checked) iChk = 1;
            RegistryWrite("CSV Files Instead", iChk);
        }

        private void chbCsvCrc32_CheckedChanged(object sender, EventArgs e)
        {
            if (chbCsvCrc32.Checked)
            {
                chbCsvCrc32.ForeColor = Color.Red;
            }
            else
            {
                chbCsvCrc32.ForeColor = Color.Black;
            }

            int iChk = 0;
            if (chbCsvCrc32.Checked) iChk = 1;
            RegistryWrite("CSV Files CRC32", iChk);
        }

    }
}
