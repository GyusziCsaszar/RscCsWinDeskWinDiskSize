﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinDiskSize
{
    public class MyDirInfo
    {

        public MyDirInfo diParent;

        public int iLevel;

        public String sStartFolder; //Optional, Cosmetic: To make reported pathes relative. (To make comparation easier in cases.)

        public String sNameLong;
        public String sNameShort83;

        public String sPathLong;
        public String sPathShort83;

        public bool bShow83;

        public bool bParsed;

        public bool bHidden;

        protected Int64 i64Count;

        protected Int64 i64CountSUM;

        protected Int64 i64Size;

        protected Int64 i64SizeSUM;

        public DateTime dtYoungestFile;
        public bool dtYoungestFile_Valid;

        public DateTime dtOldestFile;
        public bool dtOldestFile_Valid;

        public MyDirInfo()
        {
            diParent = null;

            iLevel = 0;

            sNameLong = "";
            sNameShort83 = "";

            sPathLong = "";
            sPathShort83 = "";

            bShow83 = false;

            bParsed = false;

            bHidden = false;

            i64Count = 0;

            i64CountSUM = 0;

            i64Size = 0;

            i64SizeSUM = 0;

            dtYoungestFile = new DateTime();
            dtYoungestFile_Valid = false;

            dtOldestFile = new DateTime();
            dtOldestFile_Valid = false;
        }

        public void AddFileCount(Int64 i64FileCount)
        {
            i64Count += i64FileCount;
        }

        public Int64 GetCount()
        {
            return i64Count;
        }

        public void AddFileCountSUM(Int64 i64FileCount)
        {
            if (diParent != null) diParent.AddFileCountSUM(i64FileCount);

            i64CountSUM += i64FileCount;
        }

        public Int64 GetCountSUM()
        {
            return i64CountSUM;
        }

        public void AddFileSize(Int64 i64FileSize)
        {
            i64Size += i64FileSize;
        }

        public Int64 GetSize()
        {
            return i64Size;
        }

        public void AddFileSizeSUM(Int64 i64FileSize)
        {
            if (diParent != null) diParent.AddFileSizeSUM(i64FileSize);

            i64SizeSUM += i64FileSize;
        }

        public Int64 GetSizeSUM()
        {
            return i64SizeSUM;
        }

        public void AddFileChangeDate(DateTime dt)
        {
            if (diParent != null) diParent.AddFileChangeDate(dt);

            if ((!dtYoungestFile_Valid) || dtYoungestFile.CompareTo(dt) < 0)
            {
                dtYoungestFile = dt;
                dtYoungestFile_Valid = true;
            }

            if ((!dtOldestFile_Valid) || dtOldestFile.CompareTo(dt) > 0)
            {
                dtOldestFile = dt;
                dtOldestFile_Valid = true;
            }
        }

        public void CopyChangeDateTo(MyDirInfo di)
        {
            di.dtYoungestFile = dtYoungestFile;
            di.dtYoungestFile_Valid = dtYoungestFile_Valid;

            di.dtOldestFile = dtOldestFile;
            di.dtOldestFile_Valid = dtOldestFile_Valid;
        }

        public String ToShortSizeString(Int64 i64)
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

        override public String ToString()
        {
            String s = "";

            //if (!bParsed) s += "(!) ";    //NULL for size and ChangeDate is enough!!!

            /*
            for (int i = 0; i < iLevel; i++)
            {
                s += "    ";
            }
            */
            s += "{" + iLevel.ToString() + "} ";

            String sSum = ToShortSizeString(i64SizeSUM);
            if (sSum == "0 B")
            {
              //s += "(" + sSum + ")      "; //Cosmetic only...
                s += "(NULL)      "; //Cosmetic only...
            }
            else
            {
                s += "(" + sSum + ")";
            }

            s += "\t";

            if (dtYoungestFile_Valid)
            {
                s += "[" + dtYoungestFile.ToShortDateString() + " " + dtYoungestFile.ToLongTimeString() + "]";
            }
            else
            {
                s += "[NULL]                       "; //Cosmetic only...
            }

            /*
            s += "\t";

            if (dtOldestFile_Valid)
            {
                s += "[" + dtOldestFile.ToShortDateString() + " " + dtOldestFile.ToLongTimeString() + "]";
            }
            else
            {
                s += "[NULL]                       "; //Cosmetic only...
            }
            */

            s += "\t";

            if (bShow83)
            {
                if (sStartFolder.Length > 0)
                {
                    s += "." + sPathShort83.Substring(sStartFolder.Length - 1);
                }
                else
                {
                    s += sPathShort83;
                }
            }
            else
            {
                if (sStartFolder.Length > 0)
                {
                    s += "." + sPathLong.Substring(sStartFolder.Length - 1);
                }
                else
                {
                    s += sPathLong;
                }
            }

            return s;
        }

    }
}
