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

                SqlCommand cmdIdentity = conn.CreateCommand();
                cmdIdentity.Transaction = trans;
                cmdIdentity.CommandText = "SELECT IDENT_CURRENT ('dbo.Task')";
                int iTmp = Convert.ToInt32(cmdIdentity.ExecuteScalar());

                trans.Commit();

                conn.Close();

                m_bIsReady = true;

                m_sServer = sServer;
                m_sDb = sDb;
                m_sUser = sUser;
                m_sPw = sPw;

                return true;
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
                String sConnetionString = "Data Source=" + m_sServer + ";Initial Catalog=" + m_sDb + ";User ID=" + m_sUser + ";Password=" + m_sPw;
                SqlConnection conn = new SqlConnection(sConnetionString);
                conn.Open();

                SqlTransaction trans1 = conn.BeginTransaction();

                SqlCommand cmdInsert = conn.CreateCommand();
                cmdInsert.Transaction = trans1;
                cmdInsert.CommandText = "INSERT INTO dbo.Task (Version, Status, Program, VersionString, Machine, StartDate) VALUES (100, 1, 'WinDiskSize', 'CS2010EXPRESS.100', '" +
                                            Environment.MachineName + "', SYSDATETIME())";
                cmdInsert.ExecuteNonQuery();

                SqlCommand cmdIdentity = conn.CreateCommand();
                cmdIdentity.Transaction = trans1;
                cmdIdentity.CommandText = "SELECT IDENT_CURRENT ('dbo.Task')";
                m_iTaskID = Convert.ToInt32(cmdIdentity.ExecuteScalar());

                trans1.Commit();

                //

                SqlTransaction trans2 = conn.BeginTransaction();

                SqlCommand cmdUpdateStart = conn.CreateCommand();
                cmdUpdateStart.Transaction = trans2;
                cmdUpdateStart.CommandText = "UPDATE dbo.Task SET FolderType = '" + sFolderType + "', FolderPath = '" +
                                                sFolderPath + "', Status = 2 WHERE ID=" + m_iTaskID.ToString();
                cmdUpdateStart.ExecuteNonQuery();

                trans2.Commit();

                conn.Close();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

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

                SqlTransaction trans1 = m_conn.BeginTransaction();

                String sSQL = "INSERT INTO dbo.FolderRAW (TaskID, [Level], SizeSUM, MinFileDate, MaxFileDate, NameShort83, PathShort83, NameLong, PathLong) VALUES ("
                                                         + m_iTaskID.ToString() + ", " + iLevel.ToString() + ", '" + sSizeSUM + "', "
                        /* '...' or NULL --> */          + sMinFileDate + ", " + sMaxFileDate
                                                         + ", @sNameShort83, @sPathShort83, @sNameLong, @sPathLong)";

                SqlCommand cmdInsert = m_conn.CreateCommand();
                cmdInsert.Transaction = trans1;
                cmdInsert.CommandText = sSQL;

                cmdInsert.Parameters.Add("@sNameShort83",   SqlDbType.NVarChar).Value = sNameShort83;
                cmdInsert.Parameters.Add("@sPathShort83",   SqlDbType.NVarChar).Value = sPathShort83;
                cmdInsert.Parameters.Add("@sNameLong",      SqlDbType.NVarChar).Value = sNameLong;
                cmdInsert.Parameters.Add("@sPathLong",      SqlDbType.NVarChar).Value = sPathLong;

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

                SqlTransaction trans1 = m_conn.BeginTransaction();

                SqlCommand cmdUpdateEnd = m_conn.CreateCommand();
                cmdUpdateEnd.Transaction = trans1;
                cmdUpdateEnd.CommandText = "UPDATE dbo.Task SET EndDate = SYSDATETIME(), Status = 3 WHERE ID=" + m_iTaskID.ToString();
                cmdUpdateEnd.ExecuteNonQuery();

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
