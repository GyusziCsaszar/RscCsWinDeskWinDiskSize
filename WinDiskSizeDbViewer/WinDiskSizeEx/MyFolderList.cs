using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinDiskSizeEx
{

    public class MyTask
    {

        public string           m_sInterTabArguments;

        public int              m_iTaskID;
        public string           m_sLabel;
        public string           m_sTotalSize;
        public string           m_sFreeSpace;
        public string           m_sMachine;
        public string           m_sStatus;
        public string           m_sStarted;
        public string           m_sCompleted;
        public string           m_sType;
        public string           m_sPath;

        public List<MyFolder>   Folders = new List<MyFolder>();

        public int              m_iMaxLevel;

        public MyTask()
        {
            m_iTaskID   = -1;
            m_iMaxLevel = 0;
        }

    }

    public enum MyFolderState
    {
        Unknown,

        Equals,
        Equals_HIDDEN,

        DiffersOne,
        DiffersOther,

        MissingOne,
        MissingOne_HIDDEN,

        MissingOther,
        MissingOther_HIDDEN
    }

    public class MyFolder
    {

        public int              m_iLevel;
        public String           m_sCount;
        public Int64            m_i64Count;
        public String           m_sCountSUM;
        public Int64            m_i64CountSUM;
        public String           m_sSize;
        public Int64            m_i64Size;
        public String           m_sSizeSUM;
        public Int64            m_i64SizeSUM;
        public String           m_sName;
        public String           m_sPath;
        public String           m_sFileDateTimeMin;
        public String           m_sFileDateTimeMax;
        public String           m_sFileDateOnlyMin;
        public String           m_sFileDateOnlyMax;
        public String           m_sName83;
        public String           m_sPath83;

        public String           m_sIndent;  // UI Decoration

        public int              m_iTaskIndex;
        public MyFolderState    m_State;
        public bool             m_bSizeMissMatch;

        public int              m_iIndex_TEMP;
        public Int64[]          m_ai64CountSUM_DBLCHK;
        public Int64[]          m_ai64SizeSUM_DBLCHK;
        public bool[]           m_bMissMatch;

        public MyFolderState    m_StateBranch_TEMP;

        public MyFolder()
        {
            m_iTaskIndex          = -1;
            m_State               = MyFolderState.Unknown;
            m_bSizeMissMatch      = false;

            m_iIndex_TEMP         = -1;
            m_ai64CountSUM_DBLCHK = new Int64[2];
            m_ai64SizeSUM_DBLCHK  = new Int64[2];
            m_bMissMatch          = new bool[2];

            m_StateBranch_TEMP    = MyFolderState.Unknown;
        }

        public string CountAsString
        {
            get
            {
                return m_i64Count.ToString();
            }
            set
            {
                m_sCount = value;

                m_i64Count = 0;
                if (m_sCount.Length > 0)
                {
                    Int64.TryParse(m_sCount, out m_i64Count);
                }
            }
        }

        public string CountSUMAsString
        {
            get
            {
                return m_i64CountSUM.ToString();
            }
            set
            {
                m_sCountSUM = value;

                m_i64CountSUM = 0;
                if (m_sCountSUM.Length > 0)
                {
                    Int64.TryParse(m_sCountSUM, out m_i64CountSUM);
                }
            }
        }

        public string SizeAsString
        {
            get
            {
                return ToShortSizeString(m_i64Size);
            }
            set
            {
                m_sSize = value;

                m_i64Size = 0;
                if (m_sSize.Length > 0)
                {
                    Int64.TryParse(m_sSize, out m_i64Size);
                }
            }
        }

        public string SizeSUMAsString
        {
            get
            {
                return ToShortSizeString(m_i64SizeSUM);
            }
            set
            {
                m_sSizeSUM = value;

                m_i64SizeSUM = 0;
                if (m_sSizeSUM.Length > 0)
                {
                    Int64.TryParse(m_sSizeSUM, out m_i64SizeSUM);
                }
            }
        }

        public static String ToShortSizeString(Int64 i64)
        {
            Int64 i64Div = 1;
            Int64 i64DivNext = 1024;

            int iCnt = -1;
            for (; ; )
            {
                iCnt++;
                if (i64 < i64DivNext)
                {
                    Int64 i64Res = (i64 * 10) / i64Div;
                    String sRes = i64Res.ToString();
                    if (i64Res > 0)
                    {
                        sRes = sRes.Substring(0, sRes.Length - 1) + "." + sRes.Substring(sRes.Length - 1);
                    }
                    switch (iCnt)
                    {
                        case 0: sRes += " B"; break;
                        case 1: sRes += " KB"; break;
                        case 2: sRes += " MB"; break;
                        case 3: sRes += " GB"; break;
                        case 4: sRes += " TB"; break;
                        default: sRes += " ??"; break;
                    }
                    return sRes;
                }

                i64Div = i64DivNext;
                i64DivNext *= 1024;
            }
        }

    }
}
