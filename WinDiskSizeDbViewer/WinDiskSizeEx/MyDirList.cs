using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinDiskSizeEx
{

    class MyDirItem
    {

        public int      m_iLevel;
        public String   m_sCount;
        public Int64    m_i64Count;
        public String   m_sSize;
        public Int64    m_i64Size;
        public String   m_sName;
        public String   m_sPath;
        public String   m_sFileDateMin;
        public String   m_sFileDateMax;
        public String   m_sName83;
        public String   m_sPath83;

        public MyDirItem()
        {
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

        public string SizeAsString
        {
            get
            {
                return ToShortSizeString();
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

        public String ToShortSizeString()
        {
            Int64 i64Div = 1;
            Int64 i64DivNext = 1024;

            int iCnt = -1;
            for (; ; )
            {
                iCnt++;
                if (m_i64Size < i64DivNext)
                {
                    Int64 i64Res = (m_i64Size * 10) / i64Div;
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
