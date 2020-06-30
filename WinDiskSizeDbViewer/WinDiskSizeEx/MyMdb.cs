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
       protected string             m_sFolder;
       protected string             m_sMdbTemplatePath;


        protected string            m_sMdbPath;

        protected OleDbConnection   m_conn;

        public MyMdb(String sMdbTemplatePath)
        : base()
        {
            m_sMdbTemplatePath = sMdbTemplatePath;

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

        public override int AddTask(int iStatus, string sLabel)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return -1;
            }

            try
            {
                string sConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_sMdbPath + ";";

                var conn = new OleDbConnection(sConnectString);
                conn.Open();

                OleDbCommand cmd1 = new OleDbCommand("SELECT ID FROM Task WHERE Version = 100 AND Status = " + iStatus.ToString()
                                            + " AND Program = 'WinDiskSize' AND VersionString = 'CS2010EXPRESS.100'"
                                            + " AND Label = '" + sLabel + "'", conn);

                object oTaskID = cmd1.ExecuteScalar();

                if (oTaskID != null)
                {
                    m_sLastError = "Task with Label \"" + sLabel + "\" already exists!";
                    return -1;
                }

                OleDbCommand cmd2 = new OleDbCommand("INSERT INTO Task (Version, Status, Program, VersionString, Label, StorageSize, StorageFree, Machine, StartDate)" 
                                            + " VALUES (100, " + iStatus.ToString() + ", 'WinDiskSize', 'CS2010EXPRESS.100', '"
                                            + sLabel + "', NULL, NULL, '" + Environment.MachineName + "', Now())", conn);

                cmd2.ExecuteNonQuery();

                OleDbCommand cmd3 = new OleDbCommand("SELECT @@IDENTITY", conn);

                int iTaskID = (int) cmd3.ExecuteScalar();

                conn.Close();

                return iTaskID;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return -1;
            }
        }

        public override int AddReportSubTaskIfNotExists(int iStatus, string sFolderType, string sFolderPath, string sLabel, string sStorageSize, string sStorageFree, string sMachine, string sStartDate, string sEndDate)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return -1;
            }

            try
            {
                string sConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_sMdbPath + ";";

                var conn = new OleDbConnection(sConnectString);
                conn.Open();

                OleDbCommand cmd1 = new OleDbCommand("SELECT ID FROM Task WHERE Version = 100 AND Status = " + iStatus.ToString()
                                            + " AND Program = 'WinDiskSize' AND VersionString = 'CS2010EXPRESS.100'"
                                            + " AND FolderType = '" + sFolderType + "' AND FolderPath = '" + sFolderPath + "' AND Label = '" + sLabel + "'"
                                            + " AND StorageSize = '" + sStorageSize + "' AND StorageFree = '" + sStorageFree + "'"
                                            + " AND Machine = '" + sMachine + "' AND StartDate = @StartDate AND EndDate = @EndDate", conn);

                cmd1.Parameters.Add("@StartDate", OleDbType.DBTimeStamp).Value = DateTime.Parse(sStartDate);
                cmd1.Parameters.Add("@EndDate", OleDbType.DBTimeStamp).Value = DateTime.Parse(sEndDate);

                object oTaskID = cmd1.ExecuteScalar();

                int iTaskID = -1;

                if (oTaskID != null)
                {
                    iTaskID = (int) oTaskID;
                }
                else
                {
                    OleDbCommand cmd2 = new OleDbCommand("INSERT INTO Task (Version, Status, Program, VersionString, FolderType, FolderPath, Label"
                                                + ", StorageSize, StorageFree, Machine, StartDate, EndDate)"
                                                + " VALUES (100, " + iStatus.ToString() + ", 'WinDiskSize', 'CS2010EXPRESS.100', '"
                                                + sFolderType + "', '" + sFolderPath + "', '" + sLabel + "', '" + sStorageSize + "', '" + sStorageFree + "', '"
                                                + sMachine + "', @StartDate, @EndDate)", conn);

                    cmd2.Parameters.Add("@StartDate", OleDbType.DBTimeStamp).Value = DateTime.Parse(sStartDate);
                    cmd2.Parameters.Add("@EndDate", OleDbType.DBTimeStamp).Value = DateTime.Parse(sEndDate);

                    cmd2.ExecuteNonQuery();

                    OleDbCommand cmd3 = new OleDbCommand("SELECT @@IDENTITY", conn);

                    iTaskID = (int)cmd3.ExecuteScalar();
                }

                //

                conn.Close();

                return iTaskID;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return -1;
            }
        }

        public override bool BeginTask(string sFolderType, string sFolderPath, string sLabel, string sStorageSize, string sStorageFree)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            m_iTaskID = -1;

            try
            {
                if (m_sFolder.Length == 0)
                {
                    m_sLastError = "MDB Path is not specified!";
                    return false;
                }

                String sTmp = m_sFolder;
                if (sTmp[sTmp.Length - 1] != '\\') sTmp += "\\";

                String sMdbPath = sTmp + "WinDiskSizeMap (" + Environment.MachineName + ")";

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

                OleDbCommand cmd1 = new OleDbCommand("INSERT INTO Task (Version, Status, Program, VersionString, Label, StorageSize, StorageFree, Machine, StartDate)" 
                                            + " VALUES (100, 1, 'WinDiskSize', 'CS2010EXPRESS.100', '" +
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

        public override bool AddReportFolderRAWIfNotExists(int iTaskID, int iTreeLevel, string sCount, string sCountSUM, string sSize, string sSizeSUM, string sMinFileDate, string sMaxFileDate, string sNameShort83, string sPathShort83, string sNameLong, string sPathLong, int iReportSubTaskID)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            try
            {
                OpenIfNotOpen();

                OleDbCommand cmd1 = new OleDbCommand("SELECT ID FROM FolderRAW WHERE TaskID = " + iTaskID.ToString() + " AND ReportSubTaskID = " + iReportSubTaskID.ToString()
                                            + " AND TreeLevel = " + iTreeLevel.ToString()
                                            + " AND FileCountSelf = '" + sCount + "' AND FileCountSUM = '" + sCountSUM + "'"
                                            + " AND FileSizeSelf = '" + sSize + "' AND FileSizeSUM = '" + sSizeSUM + "'"
                    /* '...' or NULL --> */ + " AND MinFileDate = " + sMinFileDate + " AND MaxFileDate = " + sMaxFileDate
                                         // + " AND NameShort83 = @sNameShort83 AND PathShort83 = @sPathShort83"
                                            + " AND NameLong = @sNameLong AND PathLong = @sPathLong", m_conn);

                /*
                cmd1.Parameters.Add("@sNameShort83", OleDbType.VarWChar).Value = sNameShort83;
                cmd1.Parameters.Add("@sPathShort83", OleDbType.VarWChar).Value = sPathShort83;
                */
                cmd1.Parameters.Add("@sNameLong", OleDbType.VarWChar).Value = sNameLong;
                cmd1.Parameters.Add("@sPathLong", OleDbType.VarWChar).Value = sPathLong;

                object oFolderRAWID = cmd1.ExecuteScalar();
                if (oFolderRAWID != null)
                {
                    return true;
                }

                //

                String sSQL = "INSERT INTO FolderRAW (TaskID, ReportSubTaskID, TreeLevel, FileCountSelf, FileCountSUM, FileSizeSelf, FileSizeSUM, MinFileDate, MaxFileDate, NameShort83, PathShort83, NameLong, PathLong)"
                                                    + " VALUES (" + iTaskID.ToString() + ", " + iReportSubTaskID.ToString() + ", "
                                                    + iTreeLevel.ToString() + ", '" + sCount + "', '" + sCountSUM + "', '" + sSize + "', '" + sSizeSUM + "', "
                    /* '...' or NULL --> */         + sMinFileDate + ", " + sMaxFileDate
                                                    + ", @sNameShort83, @sPathShort83, @sNameLong, @sPathLong)";

                OleDbCommand cmd2 = new OleDbCommand(sSQL, m_conn);

                cmd2.Parameters.Add("@sNameShort83", OleDbType.VarWChar).Value = sNameShort83;
                cmd2.Parameters.Add("@sPathShort83", OleDbType.VarWChar).Value = sPathShort83;
                cmd2.Parameters.Add("@sNameLong", OleDbType.VarWChar).Value = sNameLong;
                cmd2.Parameters.Add("@sPathLong", OleDbType.VarWChar).Value = sPathLong;

                cmd2.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return false;
            }
        }

        public override bool AddFolderRAW(int iTreeLevel, string sCount, string sCountSUM, string sSize, string sSizeSUM, string sMinFileDate, string sMaxFileDate, string sNameShort83, string sPathShort83, string sNameLong, string sPathLong, int iReportSubTaskID)
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

                String sSQL = "INSERT INTO FolderRAW (TaskID, ReportSubTaskID, TreeLevel, FileCountSelf, FileCountSUM, FileSizeSelf, FileSizeSUM, MinFileDate, MaxFileDate, NameShort83, PathShort83, NameLong, PathLong)"
                                                    + " VALUES (" + m_iTaskID.ToString() + ", " + iReportSubTaskID.ToString() + ", "
                                                    + iTreeLevel.ToString() + ", '" + sCount + "', '" + sCountSUM + "', '" + sSize + "', '" + sSizeSUM + "', "
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

        public override bool EndTask(int iTaskID, int iStatus)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            //Changed to support Report Task!
            /*
            if (m_iTaskID <= 0)
            {
                m_sLastError = "BeginTask() not called!";
                return false;
            }
            */

            try
            {
                OpenIfNotOpen();

                OleDbCommand cmd1 = new OleDbCommand("UPDATE Task SET EndDate = Now(), Status = " + iStatus.ToString() + " WHERE ID=" + /*m_iTaskID*/ iTaskID.ToString(), m_conn);

                cmd1.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "OleDB (MDB) Error: " + ex.Message;

                return false;
            }
        }

        public override bool QueryTasks(int iTaskStatusFilter, int iReportTaskID)
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

                string sSQL = "SELECT DISTINCT Task.* FROM Task";
                if (iTaskStatusFilter > -1)
                {
                    sSQL += " WHERE Status = " + iTaskStatusFilter.ToString();
                    sSQL += " ORDER BY ID DESC";
                }
                else if (iReportTaskID > -1)
                {
                    sSQL += " INNER JOIN FolderRAW ON FolderRAW.ReportSubTaskID = Task.ID";
                    sSQL += " WHERE FolderRAW.TaskID = " + iReportTaskID.ToString();
                    sSQL += " ORDER BY Task.ID";
                }
                else
                {
                    sSQL += " ORDER BY ID DESC";
                }

                OleDbCommand cmd = new OleDbCommand(sSQL, m_conn);

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

        public override bool QueryFolders(int iTaskID, string sOrderBy)
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

                string sSQL = "SELECT * FROM FolderRAW WHERE TaskID = " + iTaskID.ToString() + " ";
                if (sOrderBy.Length > 0)
                {
                    sSQL += sOrderBy;
                }
                else
                {
                    sSQL += "ORDER BY ID";
                }

                OleDbCommand cmd = new OleDbCommand(sSQL, m_conn);

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

    }
}
