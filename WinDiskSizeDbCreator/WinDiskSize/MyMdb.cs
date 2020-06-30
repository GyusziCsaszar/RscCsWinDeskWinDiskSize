using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.OleDb;

namespace WinDiskSize
{
    public class MyMdb : MyDb
    {

        protected string            m_sFolder;
        protected string            m_sMdbTemplatePath;

        protected string            m_sMdbPath;

        protected OleDbConnection   m_conn;

        public MyMdb()
        {
            Close();
        }

        public string Folder
        {
            get
            {
                return m_sFolder;
            }
            set
            {
                m_sFolder = value;
            }
        }

        public string MdbTemplatePath
        {
            get
            {
                return m_sMdbTemplatePath;
            }
            set
            {
                m_bIsReady = true;

                m_sMdbTemplatePath = value;
            }
        }

        public override void Close()
        {
            base.Close();

            m_sFolder           = "";
            m_sMdbTemplatePath  = "";

            m_sMdbPath          = "";

            if (m_conn != null)
            {
                m_conn.Close();
                m_conn = null;
            }
        }

        public override bool BeginTask(string sFolderType, string sFolderPath, string sLabel, string sStorageSize, string sStorageFree)
        {
            if (!IsReady)
            {
                m_sLastError = "TestConnect() not called!";
                return false;
            }

            m_iTaskID = -1;

            try
            {
                String sTmp = m_sFolder;
                if (sTmp.Length == 0) sTmp = "C:\\";
                if (sTmp[sTmp.Length - 1] != '\\') sTmp += "\\";
                String sMdbPath = sTmp + "WinDiskSizeMap (";
                if (sLabel.Length > 0)
                {
                    sMdbPath += sLabel + ") (";
                }
                sMdbPath += Environment.MachineName + ")";

                DateTime dt = DateTime.Now;
                sTmp = dt.ToShortDateString(); // + " " + dt.ToShortTimeString().Replace(":", "-");
                sMdbPath += " (" + sTmp + ")";

                sTmp = sFolderPath.Replace("\\", "_");
                sTmp = sTmp.Replace(":", "_");
                sMdbPath += " (" + sTmp + ")";

                sMdbPath += ".mdb";

                if (!System.IO.File.Exists(sMdbPath))
                {
                    System.IO.File.Copy(m_sMdbTemplatePath, sMdbPath, true);
                }

                m_sMdbPath = sMdbPath;

                string sConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_sMdbPath + ";";

                var conn = new OleDbConnection(sConnectString);
                conn.Open();

                OleDbCommand cmd1 = new OleDbCommand("INSERT INTO Task (Version, Status, Program, VersionString, Label, StorageSize, StorageFree, Machine, StartDate) VALUES (100, 1, 'WinDiskSize', 'CS2010EXPRESS.100', '" +
                                            sLabel + "', '" + sStorageSize + "', '" + sStorageFree + "', '" + Environment.MachineName + "', Now())", conn);

                cmd1.ExecuteNonQuery();

                OleDbCommand cmd2 = new OleDbCommand("SELECT @@IDENTITY", conn);

                m_iTaskID = (int) cmd2.ExecuteScalar();

                //

                OleDbCommand cmd3 = new OleDbCommand("UPDATE Task SET FolderType = '" + sFolderType + "', FolderPath = '" +
                                                sFolderPath + "', Status = 2 WHERE ID=" + m_iTaskID.ToString(), conn);

                cmd3.ExecuteNonQuery();

                conn.Close();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return false;
            }
        }

        public override bool AddFolderRAW(int iTreeLevel, string sCount, string sCountSUM, string sSize, string sSizeSUM, string sMinFileDate, string sMaxFileDate, string sNameShort83, string sPathShort83, string sNameLong, string sPathLong)
        {
            if (!IsReady)
            {
                m_sLastError = "TestConnect() not called!";
                return false;
            }
            if (m_iTaskID <= 0)
            {
                m_sLastError = "BeginTask() not called!";
                return false;
            }

            try
            {
                if (m_conn == null)
                {
                    string sConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_sMdbPath + ";";

                    var conn = new OleDbConnection(sConnectString);
                    conn.Open();

                    m_conn = conn;
                }

                String sSQL = "INSERT INTO FolderRAW (TaskID, TreeLevel, FileCountSelf, FileCountSUM, FileSizeSelf, FileSizeSUM, MinFileDate, MaxFileDate, NameShort83, PathShort83, NameLong, PathLong) VALUES ("
                                                    + m_iTaskID.ToString() + ", " + iTreeLevel.ToString() + ", '" + sCount + "', '" + sCountSUM + "', '" + sSize + "', '" + sSizeSUM + "', "
                    /* '...' or NULL --> */         + sMinFileDate + ", " + sMaxFileDate
                                                    + ", @sNameShort83, @sPathShort83, @sNameLong, @sPathLong)";

                OleDbCommand cmd1 = new OleDbCommand(sSQL, m_conn);

                cmd1.Parameters.Add("@sNameShort83",    OleDbType.VarWChar).Value = sNameShort83;
                cmd1.Parameters.Add("@sPathShort83",    OleDbType.VarWChar).Value = sPathShort83;
                cmd1.Parameters.Add("@sNameLong",       OleDbType.VarWChar).Value = sNameLong;
                cmd1.Parameters.Add("@sPathLong",       OleDbType.VarWChar).Value = sPathLong;

                cmd1.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return false;
            }
        }

        public override bool EndTask()
        {
            if (!IsReady)
            {
                m_sLastError = "TestConnect() not called!";
                return false;
            }
            if (m_iTaskID <= 0)
            {
                m_sLastError = "BeginTask() not called!";
                return false;
            }

            try
            {
                if (m_conn == null)
                {
                    string sConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_sMdbPath + ";";

                    var conn = new OleDbConnection(sConnectString);
                    conn.Open();

                    m_conn = conn;
                }

                OleDbCommand cmd1 = new OleDbCommand("UPDATE Task SET EndDate = Now(), Status = 3 WHERE ID=" + m_iTaskID.ToString(), m_conn);

                cmd1.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return false;
            }
        }

    }
}
