using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

namespace WinDiskSize
{
    public class MySqlServer
    {

        protected string m_sLastError;

        protected bool m_bIsReady;

        protected string m_sServer;
        protected string m_sDb;
        protected string m_sUser;
        protected string m_sPw;

        protected int m_iTaskID = -1;

        public MySqlServer()
        {
            Close();
        }

        public bool HasLastError
        {
            get
            {
                return (m_sLastError.Length > 0);
            }
        }

        public string LastError
        {
            get
            {
                return m_sLastError;
            }
        }

        public bool IsReady
        {
            get
            {
                return m_bIsReady;
            }
        }

        public int TaskID
        {
            get
            {
                return m_iTaskID;
            }
        }

        public void Close()
        {
            m_sLastError = "";

            m_bIsReady = false;

            m_sServer = "";
            m_sDb = "";
            m_sUser = "";
            m_sPw = "";

            m_iTaskID = -1;
        }

        public bool TestConnect(string sServer, string sDb, string sUser, string sPw)
        {
            Close();

            String sConnetionString = "Data Source=" + sServer + ";Initial Catalog=" + sDb + ";User ID=" + sUser + ";Password=" + sPw;

            SqlConnection conn = new SqlConnection(sConnetionString);
            try
            {
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

        public bool BeginTask(string sFolderType, string sFolderPath)
        {
            if (!IsReady)
            {
                m_sLastError = "TestConnect() not called!";
                return false;
            }

            m_iTaskID = -1;

            String sConnetionString = "Data Source=" + m_sServer + ";Initial Catalog=" + m_sDb + ";User ID=" + m_sUser + ";Password=" + m_sPw;

            SqlConnection conn = new SqlConnection(sConnetionString);
            try
            {
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

        public bool AddFolderRAW(string sSizeSUM, string sYoungestFileDate, string sNameShort83, string sPathShort83, string sNameLong, string sPathLong)
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

            String sConnetionString = "Data Source=" + m_sServer + ";Initial Catalog=" + m_sDb + ";User ID=" + m_sUser + ";Password=" + m_sPw;

            SqlConnection conn = new SqlConnection(sConnetionString);
            try
            {
                conn.Open();

                SqlTransaction trans1 = conn.BeginTransaction();

                sNameShort83 = sNameShort83.Replace("'", "''");
                sPathShort83 = sPathShort83.Replace("'", "''");
                sNameLong = sNameLong.Replace("'", "''");
                sPathLong = sPathLong.Replace("'", "''");

                SqlCommand cmdInsert = conn.CreateCommand();
                cmdInsert.Transaction = trans1;
                cmdInsert.CommandText = "INSERT INTO dbo.FolderRAW (TaskID, SizeSUM, YoungestFileDate, NameShort83, PathShort83, NameLong, PathLong) VALUES (" +
                                                    m_iTaskID.ToString() + ", " + sSizeSUM + ", " + sYoungestFileDate + ", '" + sNameShort83 + "', '" + sPathShort83 
                                                    + "', '" + sNameLong + "', '" + sPathLong + "')";
                cmdInsert.ExecuteNonQuery();

                trans1.Commit();

                conn.Close();

                return true;
            }
            catch (Exception ex)
            {
                m_sLastError = "SQL Server Error: " + ex.Message;

                return false;
            }
        }

        public bool EndTask()
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

            String sConnetionString = "Data Source=" + m_sServer + ";Initial Catalog=" + m_sDb + ";User ID=" + m_sUser + ";Password=" + m_sPw;

            SqlConnection conn = new SqlConnection(sConnetionString);
            try
            {
                conn.Open();

                SqlTransaction trans1 = conn.BeginTransaction();

                SqlCommand cmdUpdateEnd = conn.CreateCommand();
                cmdUpdateEnd.Transaction = trans1;
                cmdUpdateEnd.CommandText = "UPDATE dbo.Task SET EndDate = SYSDATETIME(), Status = 3 WHERE ID=" + m_iTaskID.ToString();
                cmdUpdateEnd.ExecuteNonQuery();

                trans1.Commit();

                conn.Close();

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
