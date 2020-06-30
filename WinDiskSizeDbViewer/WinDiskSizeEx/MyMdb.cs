using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.OleDb;

namespace WinDiskSizeEx
{
    public class MyMdb : MyDb
    {
        protected string            m_sTemplateMdbName;

        protected string            m_sFolder;

        protected string            m_sMdbPath;

        protected OleDbConnection   m_conn;
        protected DataSet           m_dataSet;

        public MyMdb(String sTemplateMdbName)
        : base()
        {
            m_sTemplateMdbName = sTemplateMdbName;

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
                m_bIsReady = true;

                m_sFolder = value;
            }
        }

        public string MdbFile
        {
            get
            {
                return System.IO.Path.GetFileName(m_sMdbPath);
            }
            set
            {
                String sTmp = m_sFolder;
                if (sTmp.Length == 0) sTmp = "C:\\";
                if (sTmp[sTmp.Length - 1] != '\\') sTmp += "\\";

                m_sMdbPath = sTmp + value;
            }
        }

        public override void Close()
        {
            base.Close();

            //DO NOT!!!
            //m_sTemplateMdb = "";

            m_sFolder = "";

            m_sMdbPath = "";

            if (m_dataSet != null)
            {
                m_dataSet.Clear();
                m_dataSet = null;
            }

            if (m_conn != null)
            {
                m_conn.Close();
                m_conn = null;
            }
        }

        public override void OpenIfNotOpen()
        {
            if (IsReady && (m_conn == null))
            {
                string sConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_sMdbPath + ";";

                var conn = new OleDbConnection(sConnectString);
                conn.Open();

                m_conn = conn;
            }
        }

        public override bool BeginTask(string sFolderType, string sFolderPath)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            m_iTaskID = -1;

            try
            {
                String sTmp = m_sFolder;
                if (sTmp.Length == 0) sTmp = "C:\\";
                if (sTmp[sTmp.Length - 1] != '\\') sTmp += "\\";

                String sMdbTemplatePath = sTmp + m_sTemplateMdbName;

                String sMdbPath = sTmp + "WinDiskSize (" + Environment.MachineName + ")";

                DateTime dt = DateTime.Now;
                sTmp = dt.ToShortDateString(); // + " " + dt.ToShortTimeString().Replace(":", "-");
                sMdbPath += " (" + sTmp + ")";

                sTmp = sFolderPath.Replace("\\", "_");
                sTmp = sTmp.Replace(":", "_");
                sMdbPath += " (" + sTmp + ")";

                sMdbPath += ".mdb";

                if (!System.IO.File.Exists(sMdbPath))
                {
                    System.IO.File.Copy(sMdbTemplatePath, sMdbPath, true);
                }

                m_sMdbPath = sMdbPath;

                string sConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_sMdbPath + ";";

                var conn = new OleDbConnection(sConnectString);
                conn.Open();

                OleDbCommand cmd1 = new OleDbCommand("INSERT INTO Task (Version, Status, Program, VersionString, Machine, StartDate) VALUES (100, 1, 'WinDiskSize', 'CS2010EXPRESS.100', '" +
                                            Environment.MachineName + "', Now())", conn);

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

        public override bool AddFolderRAW(int iLevel, string sSizeSUM, string sMinFileDate, string sMaxFileDate, string sNameShort83, string sPathShort83, string sNameLong, string sPathLong)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }
            if (m_iTaskID <= 0)
            {
                m_sLastError = "BeginTask() not called!";
                return false;
            }

            try
            {
                OpenIfNotOpen();

                String sSQL = "INSERT INTO FolderRAW (TaskID, [Level], SizeSUM, MinFileDate, MaxFileDate, NameShort83, PathShort83, NameLong, PathLong) VALUES ("
                                                    + m_iTaskID.ToString() + ", " + iLevel.ToString() + ", '" + sSizeSUM + "', "
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
                m_sLastError = "Is not Ready!";
                return false;
            }
            if (m_iTaskID <= 0)
            {
                m_sLastError = "BeginTask() not called!";
                return false;
            }

            try
            {
                OpenIfNotOpen();

                OleDbCommand cmd1 = new OleDbCommand("UPDATE Task SET EndDate = Now() WHERE ID=" + m_iTaskID.ToString(), m_conn);

                cmd1.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return false;
            }
        }

        public override bool QueryTasks()
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            if (m_sMdbPath.Length == 0)
            {
                m_sLastError = "No MDB file specified!";
                return false;
            }

            try
            {
                OpenIfNotOpen();

                OleDbCommand cmd = new OleDbCommand("SELECT * FROM Task ORDER BY ID DESC", m_conn);

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet);

                if (m_dataSet != null)
                {
                    m_dataSet.Clear();
                }
                m_dataSet = dataSet;

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return false;
            }
        }

        public override bool QueryFolders(int iTaskID)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            if (m_sMdbPath.Length == 0)
            {
                m_sLastError = "No MDB file specified!";
                return false;
            }

            try
            {
                OpenIfNotOpen();

                OleDbCommand cmd = new OleDbCommand("SELECT * FROM FolderRAW WHERE TaskID = " + iTaskID.ToString() + " ORDER BY ID", m_conn);

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet);

                if (m_dataSet != null)
                {
                    m_dataSet.Clear();
                }
                m_dataSet = dataSet;

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return false;
            }
        }

        public override int RowCount()
        {
            if (m_dataSet == null)
            {
                return -1;
            }

            return m_dataSet.Tables[0].Rows.Count;
        }

        public override String FieldAsString(int iRow, String sFieldName)
        {
            if (m_dataSet == null)
            {
                return null;
            }

            int iCol = m_dataSet.Tables[0].Columns[sFieldName].Ordinal;

            return m_dataSet.Tables[0].Rows[iRow][iCol].ToString();
        }

        public override int FieldAsInt(int iRow, String sFieldName)
        {
            if (m_dataSet == null)
            {
                return -1;
            }

            int iCol = m_dataSet.Tables[0].Columns[sFieldName].Ordinal;

            return (int) m_dataSet.Tables[0].Rows[iRow][iCol];
        }

    }
}
