using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

namespace WinDiskSize
{
    public class MyDb
    {

        protected string m_sLastError;

        protected bool m_bIsReady;

        protected int m_iTaskID = -1;

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
        }

        public virtual bool BeginTask(string sFolderType, string sFolderPath, string sLabel, string sStorageSize, string sStorageFree)
        {
            if (!IsReady)
            {
                m_sLastError = "TestConnect() not called!";
                return false;
            }

            m_iTaskID = -1;

            m_sLastError = "BeginTask is not implemented!";

            return false;
        }

        public virtual bool AddFolderRAW(int iTreeLevel, string sCount, string sCountSUM, string sSize, string sSizeSUM, string sMinFileDate, string sMaxFileDate, string sNameShort83, string sPathShort83, string sNameLong, string sPathLong)
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

            m_sLastError = "AddFolderRAW is not implemented!";

            return false;
        }

        public virtual bool EndTask()
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

            m_sLastError = "EndTask is not implemented!";

            return false;
        }

    }
}
