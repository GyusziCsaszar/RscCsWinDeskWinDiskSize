using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace WinDiskSizeEx
{
    public class MyDb
    {

        protected string m_sLastError;

        protected bool m_bIsReady;

        protected int m_iTaskID = -1;

        protected DataSet m_dataSet;

        public MyDb()
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

        public virtual void Close()
        {
            m_sLastError = "";

            m_bIsReady = false;

            m_iTaskID = -1;

            if (m_dataSet != null)
            {
                m_dataSet.Clear();
                m_dataSet = null;
            }
        }

        public virtual void OpenIfNotOpen()
        {
        }

        public virtual int AddTask(int iStatus, string sLabel)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return -1;
            }

            m_sLastError = "AddTask is not implemented!";

            return -1;
        }

        public virtual int AddReportSubTaskIfNotExists(int iStatus, string sFolderType, string sFolderPath, string sLabel, string sStorageSize, string sStorageFree, string sMachine, string sStartDate, string sEndDate)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return -1;
            }

            m_sLastError = "AddReportSubTaskIfNotExists is not implemented!";

            return -1;
        }

        public virtual bool BeginTask(string sFolderType, string sFolderPath, string sLabel, string sStorageSize, string sStorageFree)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            m_iTaskID = -1;

            m_sLastError = "BeginTask is not implemented!";

            return false;
        }

        public virtual bool AddReportFolderRAWIfNotExists(int iTaskID, int iTreeLevel, string sCount, string sCountSUM, string sSize, string sSizeSUM, string sMinFileDate, string sMaxFileDate, string sNameShort83, string sPathShort83, string sNameLong, string sPathLong, int iReportSubTaskID)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            m_sLastError = "AddReportFolderRAWIfNotExists is not implemented!";

            return false;
        }

        public virtual bool AddFolderRAW(int iTreeLevel, string sCount, string sCountSUM, string sSize, string sSizeSUM, string sMinFileDate, string sMaxFileDate, string sNameShort83, string sPathShort83, string sNameLong, string sPathLong, int iReportSubTaskID)
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

            m_sLastError = "AddFolderRAW is not implemented!";

            return false;
        }

        public virtual bool EndTask(int iTaskID, int iStatus)
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

            m_sLastError = "EndTask is not implemented!";

            return false;
        }

        public virtual bool QueryTasks(int iTaskStatusFilter, int iReportTaskID)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            m_sLastError = "QueryTasks is not implemented!";

            return false;
        }

        public virtual bool QueryFolders(int iTaskID, string sOrderBy)
        {
            if (!IsReady)
            {
                m_sLastError = "Is not Ready!";
                return false;
            }

            m_sLastError = "QueryFolders is not implemented!";

            return false;
        }

        public virtual int RowCount()
        {
            if (m_dataSet == null)
            {
                return -1;
            }

            return m_dataSet.Tables[0].Rows.Count;
        }

        public virtual String FieldAsString(int iRow, String sFieldName)
        {
            if (m_dataSet == null)
            {
                return null;
            }

            int iCol = m_dataSet.Tables[0].Columns[sFieldName].Ordinal;

            return m_dataSet.Tables[0].Rows[iRow][iCol].ToString();
        }

        public virtual int FieldAsInt(int iRow, String sFieldName)
        {
            if (m_dataSet == null)
            {
                return -1;
            }

            int iCol = m_dataSet.Tables[0].Columns[sFieldName].Ordinal;

            return (int)m_dataSet.Tables[0].Rows[iRow][iCol];
        }

    }
}
