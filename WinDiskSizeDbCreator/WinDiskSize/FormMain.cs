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

        public FormMain()
        {
            InitializeComponent();

            cbDrive.SelectedIndex = 2;

            cbUnit.SelectedIndex = 2;

            cbFreez.SelectedIndex = 0;

            lblPercent.Text = "";

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
                    di.sPath = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);
                    di.sName = "files_in_StartFolder"; // + diDrive.sName.Substring(0, 1);
                    di.AddFileLength(diDrive.GetSizeSum());
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
            String sPercent = iCntParsed.ToString() + " of " + iCnt.ToString() + " files";
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

            btnStop.Enabled = false;
            btnParse.Enabled = false;
            m_bStop = false;

            lblPercent.Text = "";

            aDi.Clear();

            String sStartFolder;
            if (cbDrive.SelectedIndex >= 0)
            {
                sStartFolder = cbDrive.Text + "\\"; // "C:\";
            }
            else
            {
                sStartFolder = tbStartFolder.Text + "\\";
            }
            if (sStartFolder.Length == 0) return;

            MyDirInfo di = new MyDirInfo();
            di.sStartFolder = sStartFolder; //ATTN: Cosmetic only!!!
            di.sPath        = sStartFolder;
            di.sName        = sStartFolder;
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

            /*
            for (int i = 1; i <= 1000; i++)
            {
                int iIdx = -1;
                foreach (MyDirInfo di in aDi)
                {
                    iIdx++;
                    if (di.bParsed) continue;

                    EnumFolder(di, iIdx);

                    break;
                }

                ListBoxRefresh();
            }
            */
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            m_bStop = true;
        }

        private void EnumFolder(MyDirInfo di, int iDirInfoIdx)
        {

            FileDirectoryEnumerable myEnum = new FileDirectoryEnumerable();
            myEnum.SearchPath    = di.sPath; //ATTN!!! HAS TO BE ENDED WITH \!!! Otherwise default directory of the drive will be the start direcotry!!!
            myEnum.SearchPattern = "*.*";

            myEnum.ThrowIOException = false; //ATTN!!!

            /*
            myEnum.ReturnStringType = true;
            int iCount = 0;
            foreach (string strFileName in myEnum)
            {
                lbDirList.Items.Add(strFileName);
                // LOOK ! , you can interrupt freely
                if ((iCount++) > 10)
                    break;
            }
            */

            myEnum.SearchForFile = false;
            myEnum.SearchForDirectory = true;
            myEnum.ReturnStringType = false;
            myEnum.SearchPattern = "*.*";
            int iSubIdx = iDirInfoIdx;
            foreach (System.IO.DirectoryInfo dir in myEnum)
            {
                iSubIdx++;

                MyDirInfo diSub = new MyDirInfo();
                diSub.diParent = di;
                diSub.iLevel = di.iLevel + 1;
                diSub.sStartFolder = di.sStartFolder;
                diSub.sPath = di.sPath + "\\" + dir.Name;
                diSub.sName = dir.Name;
                aDi.Insert(iSubIdx, diSub);
            }

            myEnum.SearchForFile = true;
            myEnum.SearchForDirectory = false;
            myEnum.ReturnStringType = false;
            myEnum.SearchPattern = "*.*";
            foreach (System.IO.FileInfo file in myEnum)
            {
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
            int iMaxLevel = Int32.Parse(txLevel.Text);

            Int64 i64SizeFilter = 0;
            if (m_iFilterValue > 0)
            {
                i64SizeFilter = m_iFilterValue;

                for(int iMul = 0; iMul < cbUnit.SelectedIndex; iMul++)
                {
                    i64SizeFilter = i64SizeFilter * ((Int64) 1024);
                }
            }

            lbDirList.Items.Clear();
            if (aDi != null)
            {
                bool bFirst = true;
                foreach (MyDirInfo di in aDi)
                {
                    if (bFirst || /*(!di.bParsed) ||*/ (di.iLevel <= iMaxLevel || iMaxLevel == 0))
                    {
                        if (bFirst || (!di.bParsed) || di.GetSizeSum() >= i64SizeFilter)
                        {
                            lbDirList.Items.Add(di);
                        }
                    }

                    bFirst = false;
                }
            }
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

    }
}
