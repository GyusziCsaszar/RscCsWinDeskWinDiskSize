using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EnumerateFile;

namespace WinDiskSize
{
    public partial class FormMain : Form
    {

        protected List<MyDirInfo> aDi;

        protected System.Windows.Forms.Timer tmrWalk;

        protected bool m_bStop;
        protected int m_iFilterValue;
        protected int m_iFreez;

        StatusBar mainStatusBar;
        StatusBarPanel sbPanelLb;
        StatusBarPanel sbPanelLevel;

        protected MySqlServer m_sqlsvr = new MySqlServer();

        public FormMain()
        {
            InitializeComponent();

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
            sbPanelLevel.BorderStyle = StatusBarPanelBorderStyle.Raised;
            sbPanelLevel.Text = "Max Level: N/A";
            sbPanelLevel.ToolTipText = "Maximum level (depth) parsed";
            sbPanelLevel.AutoSize = StatusBarPanelAutoSize.Contents;
            mainStatusBar.Panels.Add(sbPanelLevel);
            //
            mainStatusBar.ShowPanels = true;
            Controls.Add(mainStatusBar);

            cbDrive.SelectedIndex = 2;

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

                //Adding item to show SumOfFileSizes in drive's root folder
                if (iIdx == 0)
                {
                    MyDirInfo diDrive = aDi[0];

                    MyDirInfo di = new MyDirInfo();
                    di.sStartFolder = "";

                    di.sNameLong = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);
                    di.sNameShort83 = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);
                    di.sPathLong = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);
                    di.sPathShort83 = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);

                    di.AddFileLength(diDrive.GetSizeSum());
                    diDrive.CopyChangeDateTo(di);
                    di.iDirCountNoRecurse = diDrive.iDirCountNoRecurse;
                    di.iFileCountNoRecurse = diDrive.iFileCountNoRecurse;

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

                    CopyToSqlServer();
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

            string sFolderType = "";
            string sFolderPath = "";
            if (this.cbDrive.SelectedIndex >= 0)
            {
                sFolderType = "Drive";
                sFolderPath = cbDrive.Text;
            }
            else
            {
                sFolderType = "Folder";
                sFolderPath = tbStartFolder.Text;
            }
            if (sFolderPath.Length == 0) return;

            if (m_sqlsvr.IsReady)
            {
                if (!m_sqlsvr.BeginTask(sFolderType, sFolderPath))
                {
                    MessageBox.Show(m_sqlsvr.LastError);
                    return;
                }
                else
                {
                    tbTaskID.Text = m_sqlsvr.TaskID.ToString();
                }
            }

            btnStop.Enabled = false;
            btnParse.Enabled = false;
            m_bStop = false;

            lblPercent.Text = "";
            sbPanelLb.Text = "Item Count: 0";
            sbPanelLevel.Text = "Max Level: N/A";

            aDi.Clear();

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
            di.iDirCountNoRecurse = 0;
            foreach (System.IO.DirectoryInfo dir in myEnum)
            {
                di.iDirCountNoRecurse += 1;

                if (chbExcludeSysFolders.Checked)
                {
                    if (
                            ((!di.bShow83) && (di.sPathLong.Substring(   di.sStartFolder.Length - 1) == "\\")) ||
                            (  di.bShow83  && (di.sPathShort83.Substring(di.sStartFolder.Length - 1) == "\\"))
                       )
                    {
                        string sNameSm = dir.Name.ToLower();

                        if ( (sNameSm == "windows") ||
                             (sNameSm == "program files") ||
                             (sNameSm == "program files (x86)") ||
                             (sNameSm == "programdata") ||
                             (sNameSm == "users") ||
                             (sNameSm == "system volume information") ||
                             (sNameSm == "$recycle.bin")
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
            di.iFileCountNoRecurse = 0;
            foreach (LongFileInfo file in myEnum)
            {
                di.iFileCountNoRecurse += 1;

                di.AddFileLength(file.Length);
                //lbDirList.Items.Add(file.Name + "\tLength:" + file.Length + " bytes ");

                di.AddFileChangeDate(file.CreationTime);
                di.AddFileChangeDate(file.LastWriteTime);
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
                        if (bFirst || (!di.bParsed) || di.GetSizeSum() >= i64SizeFilter)
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
        }

        private void btnStartFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                cbDrive.SelectedIndex = -1;

                tbStartFolder.Text = dlg.SelectedPath;

                btnList_Click(null, EventArgs.Empty);
            }
        }

        private void chbShort83_Click(object sender, EventArgs e)
        {
            ListBoxRefresh();
        }

        private void btnSqlSvr_Click(object sender, EventArgs e)
        {

            tbTaskID.Text = "";

            if (!m_sqlsvr.TestConnect(tbServer.Text, tbDb.Text, tbUser.Text, tbPw.Text))
            {
                prsSqlSvr.Visible = false;
                MessageBox.Show(m_sqlsvr.LastError);
            }
            else
            {
                prsSqlSvr.Visible = true;
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

        private void btnCopyToSqlServer_Click(object sender, EventArgs e)
        {
        }

        private void CopyToSqlServer()
        {
            if (aDi == null) return;
            if (aDi.Count == 0) return;
            if (!m_sqlsvr.IsReady) return;

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

                string sYoungestFileDate = "";
                if (di.dtYoungestFile_Valid)
                {
                    sYoungestFileDate = "'";
                    sYoungestFileDate += di.dtYoungestFile.Year.ToString();

                    if (di.dtYoungestFile.Month < 10)
                        sYoungestFileDate += "0" + di.dtYoungestFile.Month.ToString();
                    else
                        sYoungestFileDate += di.dtYoungestFile.Month.ToString();

                    if (di.dtYoungestFile.Day < 10)
                        sYoungestFileDate += "0" + di.dtYoungestFile.Day.ToString();
                    else
                        sYoungestFileDate += di.dtYoungestFile.Day.ToString();

                    sYoungestFileDate += " ";
                    sYoungestFileDate += di.dtYoungestFile.ToLongTimeString();

                    sYoungestFileDate += "'";
                }
                else
                {
                    sYoungestFileDate = "NULL";
                }

                if (!m_sqlsvr.AddFolderRAW(di.iLevel, di.GetSizeSum().ToString(), sYoungestFileDate, sNameShort83, sPathShort83, sNameLong, sPathLong,
                        di.iDirCountNoRecurse, di.iFileCountNoRecurse))
                {
                    break;
                }
            }

            prsSqlSvr.Visible = false;
            prsSqlSvr.Update();

            if (m_sqlsvr.HasLastError)
            {
                MessageBox.Show(m_sqlsvr.LastError);
                return;
            }

            if (m_sqlsvr.EndTask())
            {
                MessageBox.Show("Folder List has been successfully transferred to SQL Server Database!\n\nItem Count: " + lCnt.ToString());
            }
            else
            {
                MessageBox.Show(m_sqlsvr.LastError);
            }


        }

    }
}
