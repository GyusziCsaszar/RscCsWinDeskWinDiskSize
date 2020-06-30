using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;

namespace WinDiskSizeEx
{
    public class MySqlServer : MyDb
    {

        protected string        m_sServer;
        protected string        m_sDb;
        protected string        m_sUser;
        protected string        m_sPw;

        protected SqlConnection m_conn;

        public MySqlServer()
        {
            Close();
        }

        public bool TestConnect(string sServer, string sDb, string sUser, string sPw)
        {
            Close();

            try
            {
                String sConnetionString = "Data Source=" + sServer + ";Initial Catalog=" + sDb + ";User ID=" + sUser + ";Password=" + sPw;
                SqlConnection conn = new SqlConnection(sConnetionString);
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                /*
                SqlCommand cmdIdentity = conn.CreateCommand();
                cmdIdentity.Transaction = trans;
                cmdIdentity.CommandText = "SELECT IDENT_CURRENT ('dbo.Task')";
                object oResult = cmdIdentity.ExecuteScalar();
                */

                SqlCommand cmdChk1 = conn.CreateCommand();
                cmdChk1.Transaction = trans;
                cmdChk1.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Task'";
                int iTableCount_Task = (int)cmdChk1.ExecuteScalar();

                SqlCommand cmdChk2 = conn.CreateCommand();
                cmdChk2.Transaction = trans;
                cmdChk2.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'FolderRAW'";
                int iTableCount_FolderRAW = (int)cmdChk2.ExecuteScalar();

                trans.Commit();

                conn.Close();

                if (iTableCount_Task == 0 || iTableCount_FolderRAW == 0)
                {
                    string sTbls = "";
                    if (iTableCount_Task == 0)
                    {
                        if (sTbls.Length > 0) sTbls += ", ";
                        sTbls += "Task";
                    }
                    if (iTableCount_FolderRAW == 0)
                    {
                        if (sTbls.Length > 0) sTbls += ", ";
                        sTbls += "FoldrRAW";
                    }
                    m_sLastError = "Table \"" + sTbls + "\" does not exist in SQL Server Database (" + sServer + ", " + sDb + ")!";

                    return false;
                }
                else
                {

                    m_bIsReady = true;

                    m_sServer = sServer;
                    m_sDb = sDb;
                    m_sUser = sUser;
                    m_sPw = sPw;

                    return true;
                }

            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

                return false;
            }
        }

        public override void Close()
        {
            base.Close();

            m_sServer = "";
            m_sDb = "";
            m_sUser = "";
            m_sPw = "";

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
                String sConnetionString = "Data Source=" + m_sServer + ";Initial Catalog=" + m_sDb + ";User ID=" + m_sUser + ";Password=" + m_sPw;

                SqlConnection conn = new SqlConnection(sConnetionString);
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
                OpenIfNotOpen();

                SqlTransaction trans1 = m_conn.BeginTransaction();

                //FOR REPORT?
                int iForReport = 0;
                if (iStatus == 4)
                {
                    iForReport = 1;
                }

                SqlCommand cmd1 = new SqlCommand("SELECT ID FROM Task WHERE ForReport = " + iForReport + " AND Version = 100 AND Status = " + iStatus.ToString()
                                            + " AND Program = N'WinDiskSize' AND VersionString = N'CS2010EXPRESS.100'"
                                            + " AND Label = N'" + sLabel + "'", m_conn);
                cmd1.Transaction = trans1;

                object oTaskID = cmd1.ExecuteScalar();

                if (oTaskID != null)
                {
                    m_sLastError = "Task with Label \"" + sLabel + "\" already exists!";
                    return -1;
                }

                SqlCommand cmd2 = new SqlCommand("INSERT INTO Task (ForReport, Version, Status, Program, VersionString, Label, StorageSize, StorageFree, Machine, StartDate)"
                                            + " VALUES (" + iForReport + ", 100, " + iStatus.ToString() + ", N'WinDiskSize', N'CS2010EXPRESS.100', '"
                                            // FIX: To Support SQL Server 2000 SP3
                                          //+ sLabel + "', NULL, NULL, N'" + Environment.MachineName + "', SYSDATETIME())", m_conn);
                                            + sLabel + "', NULL, NULL, N'" + Environment.MachineName + "', GETDATE())", m_conn);
                cmd2.Transaction = trans1;

                cmd2.ExecuteNonQuery();

                SqlCommand cmdIdentity = m_conn.CreateCommand();
                cmdIdentity.Transaction = trans1;
                cmdIdentity.CommandText = "SELECT IDENT_CURRENT ('dbo.Task')";
                int iTaskID = Convert.ToInt32(cmdIdentity.ExecuteScalar());

                trans1.Commit();

                m_conn.Close();
                m_conn = null;

                return iTaskID;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

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
                OpenIfNotOpen();

                SqlTransaction trans1 = m_conn.BeginTransaction();

                SqlCommand cmd1 = new SqlCommand("SELECT ID FROM Task WHERE ForReport = 2 AND Version = 100 AND Status = " + iStatus.ToString()
                                            + " AND Program = N'WinDiskSize' AND VersionString = N'CS2010EXPRESS.100'"
                                            + " AND FolderType = N'" + sFolderType + "' AND FolderPath = N'" + sFolderPath + "' AND Label = N'" + sLabel + "'"
                                            + " AND StorageSize = N'" + sStorageSize + "' AND StorageFree = N'" + sStorageFree + "'"
                                            + " AND Machine = N'" + sMachine + "' AND StartDate = @StartDate AND EndDate = @EndDate", m_conn);
                cmd1.Transaction = trans1;

                cmd1.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = DateTime.Parse(sStartDate);
                cmd1.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.Parse(sEndDate);

                object oTaskID = cmd1.ExecuteScalar();

                int iTaskID = -1;

                if (oTaskID != null)
                {
                    iTaskID = (int)oTaskID;
                }
                else
                {
                    SqlCommand cmd2 = new SqlCommand("INSERT INTO Task (ForReport, Version, Status, Program, VersionString, FolderType, FolderPath, Label"
                                                + ", StorageSize, StorageFree, Machine, StartDate, EndDate)"
                                                + " VALUES (2, 100, " + iStatus.ToString() + ", N'WinDiskSize', N'CS2010EXPRESS.100', N'"
                                                + sFolderType + "', N'" + sFolderPath + "', N'" + sLabel + "', N'" + sStorageSize + "', N'" + sStorageFree + "', N'"
                                                + sMachine + "', @StartDate, @EndDate)", m_conn);
                    cmd2.Transaction = trans1;

                    cmd2.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = DateTime.Parse(sStartDate);
                    cmd2.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.Parse(sEndDate);

                    cmd2.ExecuteNonQuery();

                    SqlCommand cmdIdentity = m_conn.CreateCommand();
                    cmdIdentity.Transaction = trans1;
                    cmdIdentity.CommandText = "SELECT IDENT_CURRENT ('dbo.Task')";
                    iTaskID = Convert.ToInt32(cmdIdentity.ExecuteScalar());
                }

                trans1.Commit();

                //

                return iTaskID;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

                return -1;
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
                OpenIfNotOpen();

                SqlTransaction trans1 = m_conn.BeginTransaction();

                SqlCommand cmdInsert = m_conn.CreateCommand();
                cmdInsert.Transaction = trans1;
                cmdInsert.CommandText = "INSERT INTO dbo.Task (ForReport, Version, Status, Program, VersionString, Label, StorageSize, StorageFree, Machine, StartDate)"
                                            + " VALUES (0, 100, 1, N'WinDiskSize', N'CS2010EXPRESS.100', N'" +
                    // FIX: To Support SQL Server 2000 SP3
                    //sLabel + "', N'" + sStorageSize + "', N'" + sStorageFree + "', N'" + Environment.MachineName + "', SYSDATETIME())";
                                            sLabel + "', N'" + sStorageSize + "', N'" + sStorageFree + "', N'" + Environment.MachineName + "', GETDATE())";
                cmdInsert.ExecuteNonQuery();

                SqlCommand cmdIdentity = m_conn.CreateCommand();
                cmdIdentity.Transaction = trans1;
                cmdIdentity.CommandText = "SELECT IDENT_CURRENT ('dbo.Task')";
                m_iTaskID = Convert.ToInt32(cmdIdentity.ExecuteScalar());

                trans1.Commit();

                //

                SqlTransaction trans2 = m_conn.BeginTransaction();

                SqlCommand cmdUpdateStart = m_conn.CreateCommand();
                cmdUpdateStart.Transaction = trans2;
                cmdUpdateStart.CommandText = "UPDATE dbo.Task SET FolderType = N'" + sFolderType + "', FolderPath = N'" +
                                                sFolderPath + "', Status = 2 WHERE ID=" + m_iTaskID.ToString();
                cmdUpdateStart.ExecuteNonQuery();

                trans2.Commit();

                m_conn.Close();
                m_conn = null;

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

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

                SqlTransaction trans1 = m_conn.BeginTransaction();

                SqlCommand cmd1 = new SqlCommand("SELECT ID FROM FolderRAW WHERE TaskID = " + iTaskID.ToString() + " AND ReportSubTaskID = " + iReportSubTaskID.ToString()
                                            + " AND TreeLevel = " + iTreeLevel.ToString()
                                            + " AND FileCountSelf = N'" + sCount + "' AND FileCountSUM = N'" + sCountSUM + "'"
                                            + " AND FileSizeSelf = N'" + sSize + "' AND FileSizeSUM = N'" + sSizeSUM + "'"
                    /* '...' or NULL --> */ + " AND MinFileDate = " + sMinFileDate + " AND MaxFileDate = " + sMaxFileDate
                    // + " AND NameShort83 = @sNameShort83 AND PathShort83 = @sPathShort83"
                                            + " AND NameLong = @sNameLong AND PathLong = @sPathLong", m_conn);
                cmd1.Transaction = trans1;

                /*
                cmd1.Parameters.Add("@sNameShort83", SqlDbType.NVarChar).Value = sNameShort83;
                cmd1.Parameters.Add("@sPathShort83", SqlDbType.NVarChar).Value = sPathShort83;
                */
                cmd1.Parameters.Add("@sNameLong", SqlDbType.NVarChar).Value = sNameLong;
                cmd1.Parameters.Add("@sPathLong", SqlDbType.NVarChar).Value = sPathLong;

                object oFolderRAWID = cmd1.ExecuteScalar();
                if (oFolderRAWID != null)
                {
                    trans1.Commit();
                    return true;
                }

                //

                String sSQL = "INSERT INTO FolderRAW (TaskID, ReportSubTaskID, TreeLevel, FileCountSelf, FileCountSUM, FileSizeSelf, FileSizeSUM, MinFileDate, MaxFileDate, NameShort83, PathShort83, NameLong, PathLong)"
                                                    + " VALUES (" + iTaskID.ToString() + ", " + iReportSubTaskID.ToString() + ", "
                                                    + iTreeLevel.ToString() + ", N'" + sCount + "', N'" + sCountSUM + "', N'" + sSize + "', N'" + sSizeSUM + "', "
                    /* '...' or NULL --> */         + sMinFileDate + ", " + sMaxFileDate
                                                    + ", @sNameShort83, @sPathShort83, @sNameLong, @sPathLong)";

                SqlCommand cmd2 = new SqlCommand(sSQL, m_conn);
                cmd2.Transaction = trans1;

                cmd2.Parameters.Add("@sNameShort83", SqlDbType.NVarChar).Value = sNameShort83;
                cmd2.Parameters.Add("@sPathShort83", SqlDbType.NVarChar).Value = sPathShort83;
                cmd2.Parameters.Add("@sNameLong", SqlDbType.NVarChar).Value = sNameLong;
                cmd2.Parameters.Add("@sPathLong", SqlDbType.NVarChar).Value = sPathLong;

                cmd2.ExecuteNonQuery();

                trans1.Commit();

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
                OpenIfNotOpen();

                SqlTransaction trans1 = m_conn.BeginTransaction();

                String sSQL = "INSERT INTO dbo.FolderRAW (TaskID, ReportSubTaskID, TreeLevel, FileCountSelf, FileCountSUM, FileSizeSelf, FileSizeSUM, MinFileDate, MaxFileDate, NameShort83, PathShort83, NameLong, PathLong) VALUES ("
                                                         + m_iTaskID.ToString() + ", " + iReportSubTaskID.ToString() + ", " + iTreeLevel.ToString() + ", N'" + sCount + "', N'" + sCountSUM + "', N'" + sSize + "', N'" + sSizeSUM + "', "
                    /* '...' or NULL --> */          + sMinFileDate + ", " + sMaxFileDate
                                                         + ", @sNameShort83, @sPathShort83, @sNameLong, @sPathLong)";

                SqlCommand cmdInsert = m_conn.CreateCommand();
                cmdInsert.Transaction = trans1;
                cmdInsert.CommandText = sSQL;

                cmdInsert.Parameters.Add("@sNameShort83", SqlDbType.NVarChar).Value = sNameShort83;
                cmdInsert.Parameters.Add("@sPathShort83", SqlDbType.NVarChar).Value = sPathShort83;
                cmdInsert.Parameters.Add("@sNameLong", SqlDbType.NVarChar).Value = sNameLong;
                cmdInsert.Parameters.Add("@sPathLong", SqlDbType.NVarChar).Value = sPathLong;

                cmdInsert.ExecuteNonQuery();

                trans1.Commit();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

                return false;
            }
        }

        public override bool EndTask(int iTaskID, int iStatus)
        {
            if (!IsReady)
            {
                m_sLastError = "TestConnect() not called!";
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

                SqlTransaction trans1 = m_conn.BeginTransaction();

                SqlCommand cmdUpdateEnd = m_conn.CreateCommand();
                cmdUpdateEnd.Transaction = trans1;
                // FIX: To Support SQL Server 2000 SP3
                //cmdUpdateEnd.CommandText = "UPDATE dbo.Task SET EndDate = SYSDATETIME(), Status = 3 WHERE ID=" + m_iTaskID.ToString();
                cmdUpdateEnd.CommandText = "UPDATE dbo.Task SET EndDate = GETDATE(), Status = " + iStatus.ToString() + " WHERE ID=" + iTaskID.ToString(); // m_iTaskID.ToString();
                cmdUpdateEnd.ExecuteNonQuery();

                trans1.Commit();

                m_conn.Close();
                m_conn = null;

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

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

            try
            {
                OpenIfNotOpen();

                SqlTransaction trans1 = m_conn.BeginTransaction();

                string sSQL = "SELECT DISTINCT Task.* FROM Task";
                if (iTaskStatusFilter > -1)
                {
                    sSQL += " WHERE ForReport > 0 AND Status = " + iTaskStatusFilter.ToString();
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
                    sSQL += " WHERE ForReport = 0";
                    sSQL += " ORDER BY ID DESC";
                }

                SqlCommand cmd = new SqlCommand(sSQL, m_conn);
                cmd.Transaction = trans1;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet);

                if (m_dataSet != null)
                {
                    m_dataSet.Clear();
                }
                m_dataSet = dataSet;

                trans1.Commit();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

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

            try
            {
                OpenIfNotOpen();

                SqlTransaction trans1 = m_conn.BeginTransaction();

                string sSQL = "SELECT * FROM FolderRAW WHERE TaskID = " + iTaskID.ToString() + " ";
                if (sOrderBy.Length > 0)
                {
                    sSQL += sOrderBy;
                }
                else
                {
                    sSQL += "ORDER BY ID";
                }

                SqlCommand cmd = new SqlCommand(sSQL, m_conn);
                cmd.Transaction = trans1;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet);

                if (m_dataSet != null)
                {
                    m_dataSet.Clear();
                }
                m_dataSet = dataSet;

                trans1.Commit();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

                return false;
            }
        }

    }
}
