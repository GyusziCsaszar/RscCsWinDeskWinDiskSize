﻿using System;

//     SRC: https://www.codeproject.com/Articles/19688/In-C-Use-Win-API-to-Enumerate-File-and-Directory 
// LICENSE: https://www.codeproject.com/info/cpol10.aspx

namespace EnumerateFile
{
    /// <summary>
    /// ????????,???? FileDirectoryEnumerator ?????
    /// </summary>
    /// <remarks>
    /// 
    /// ?? ??? ( http://www.xdesigner.cn )2006-12-8
    /// 
    /// ?????????????????
    /// 
    /// FileDirectoryEnumerable e = new FileDirectoryEnumerable();
    /// e.SearchPath = @"c:\";
    /// e.ReturnStringType = true ;
    /// e.SearchPattern = "*.exe";
    /// e.SearchDirectory = false ;
    /// e.SearchFile = true;
    /// foreach (object name in e)
    /// {
    /// System.Console.WriteLine(name);
    /// }
    /// System.Console.ReadLine();
    /// 
    ///</remarks>

    public class LongFileInfo : System.IO.FileSystemInfo
    {

        //BUG: Unable to open pagefile.sys!!!
        /*
        [System.Runtime.InteropServices.DllImport
         ("kernel32.dll",
         CharSet = System.Runtime.InteropServices.CharSet.Auto,
         SetLastError = true)]
        static extern bool GetFileSizeEx(IntPtr hFile, out long lpFileSize);

        [System.Runtime.InteropServices.DllImport
         ("kernel32.dll",
         CharSet = System.Runtime.InteropServices.CharSet.Auto,
         SetLastError = true)]
        //NOT WORKS!!!
        /*
        private static extern IntPtr CreateFile(string pFileName, long dwDesiredAccess, long dwShareMode, IntPtr lpSecurityAttributes,
            long dwCreationDisposition, long dwFlagsAndAttributes, IntPtr hTemplateFile);
        *
        //SRC: https://www.pinvoke.net/default.aspx/kernel32.CreateFile
        public static extern IntPtr CreateFile(
             [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)] string filename,
             [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] uint uiAccess, //FileAccess access,
             [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] uint uiShare, //FileShare share,
             IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
             [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] uint uiFileMode, //FileMode creationDisposition,
             [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] uint uiFlags, //FileAttributes flagsAndAttributes,
             IntPtr templateFile);


        [System.Runtime.InteropServices.DllImport
         ("kernel32.dll",
         CharSet = System.Runtime.InteropServices.CharSet.Auto,
         SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [System.Runtime.InteropServices.DllImport
         ("kernel32.dll",
         CharSet = System.Runtime.InteropServices.CharSet.Auto,
         SetLastError = true)]
        private static extern bool GetFileTime(IntPtr hFile, ref FILETIME lpCreationTime,
           ref FILETIME lpLastAccessTime, ref FILETIME lpLastWriteTime);
        */

        [Serializable,
        System.Runtime.InteropServices.StructLayout
        (System.Runtime.InteropServices.LayoutKind.Sequential,
        CharSet = System.Runtime.InteropServices.CharSet.Auto
        ),
        System.Runtime.InteropServices.BestFitMapping(false)]
        private struct WIN32_FIND_DATA
        {
            public int dwFileAttributes;
            public int ftCreationTime_dwLowDateTime;
            public int ftCreationTime_dwHighDateTime;
            public int ftLastAccessTime_dwLowDateTime;
            public int ftLastAccessTime_dwHighDateTime;
            public int ftLastWriteTime_dwLowDateTime;
            public int ftLastWriteTime_dwHighDateTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [System.Runtime.InteropServices.MarshalAs
            (System.Runtime.InteropServices.UnmanagedType.ByValTStr,
            SizeConst = 260)]
            public string cFileName;
            [System.Runtime.InteropServices.MarshalAs
            (System.Runtime.InteropServices.UnmanagedType.ByValTStr,
            SizeConst = 14)]
            public string cAlternateFileName;
        }
        [System.Runtime.InteropServices.DllImport
        ("kernel32.dll",
        CharSet = System.Runtime.InteropServices.CharSet.Auto,
        SetLastError = true)]
        private static extern IntPtr FindFirstFile(string pFileName,
            ref WIN32_FIND_DATA pFindFileData);
        [System.Runtime.InteropServices.DllImport
        ("kernel32.dll",
        CharSet = System.Runtime.InteropServices.CharSet.Auto,
        SetLastError = true)]
        private static extern bool FindNextFile(IntPtr hndFindFile,
            ref WIN32_FIND_DATA lpFindFileData);
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FindClose(IntPtr hndFindFile);
        private static long ToLong(int height, int low)
        {
            long v = (uint)height;
            v = v << 0x20;
            v = v | ((uint)low);
            return v;
        }

        protected string m_sPath;

        protected bool m_bExists;
        protected DateTime m_dtCreateTime;
        protected DateTime m_dtLastWriteTime;
        protected long m_lFileSize;

        public LongFileInfo(string sPath)

            : base()
        {

            m_sPath = sPath;

            m_bExists = false;
            m_lFileSize = 0;
            m_dtCreateTime = DateTime.MinValue;
            m_dtLastWriteTime = DateTime.MinValue;

            IntPtr handle = IntPtr.Zero;
            try
            {
                WIN32_FIND_DATA myData = new WIN32_FIND_DATA();

                String sSdkPath = "\\\\?\\" + m_sPath;
                handle = FindFirstFile(sSdkPath, ref myData);
                if (handle.ToInt32() == -1 /*INVALID_HANDLE_VALUE*/)
                {
                    //System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error());
                    m_bExists = false;
                }
                else
                {
                    m_bExists = true;

                    m_lFileSize = ToLong(myData.nFileSizeHigh,
                        myData.nFileSizeLow);

                    long timeCreation = ToLong(myData.ftCreationTime_dwHighDateTime,
                        myData.ftCreationTime_dwLowDateTime);
                    //ATTN: Not GMT, Local Time instead!!!
                  //m_dtCreateTime = System.DateTime.FromFileTimeUtc(timeCreation);
                    m_dtCreateTime = System.DateTime.FromFileTime(timeCreation);

                    long timeLastWrite = ToLong(myData.ftLastWriteTime_dwHighDateTime,
                        myData.ftLastWriteTime_dwLowDateTime);
                    //ATTN: Not GMT, Local Time instead!!!
                  //m_dtLastWriteTime = System.DateTime.FromFileTimeUtc(timeLastWrite);
                    m_dtLastWriteTime = System.DateTime.FromFileTime(timeLastWrite);

                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (handle != IntPtr.Zero && handle.ToInt32() != -1 /*INVALID_HANDLE_VALUE*/)
                {
                    FindClose(handle);
                }
            }

        }

        public override string Name
        {

            get { return m_sPath; }

        }

        public override bool Exists
        {

            get
            {

                return m_bExists;

            }

        }

        public long Length
        {

            get
            {

                //BUG: Unable to open pagefile.sys!!!
                /*
                String sPath = "\\\\?\\" + m_sPath;
                IntPtr handle = CreateFile(sPath, (uint) 0x80000000L /*GENERIC_READ*, 1 /*FILE_SHARE_READ*, IntPtr.Zero, 3 /*OPEN_EXISTING*, 1 /*FILE_ATTRIBUTE_READONLY*, IntPtr.Zero);
                if (handle == IntPtr.Zero || handle.ToInt32() == -1 /*INVALID_HANDLE_VALUE*)
                {
                    return 0;
                }

                long fileSize;
                bool result = GetFileSizeEx(handle, out fileSize);
                if (!result)
                {
                    return 0;
                }

                if (handle != IntPtr.Zero && handle.ToInt32() != -1 /*INVALID_HANDLE_VALUE*)
                {
                    CloseHandle(handle);
                }
                */

                return m_lFileSize;
            }

        }

        public DateTime CreationTime
        {

            get
            {

                //BUG: Unable to open pagefile.sys!!!
                /*
                DateTime dtCreationTime = DateTime.MinValue;
                DateTime dtLastAccessTime = DateTime.MinValue;
                DateTime dtLastWriteTime = DateTime.MinValue;
                IntPtr handle = IntPtr.Zero;
                FILETIME ftCreationTime = new FILETIME();
                FILETIME ftLastAccessTime = new FILETIME();
                FILETIME ftLastWriteTime = new FILETIME();
                try
                {
                    String sPath = "\\\\?\\" + m_sPath;
                    handle = CreateFile(sPath, (uint)0x80000000L /*GENERIC_READ*, 1 /*FILE_SHARE_READ*, IntPtr.Zero, 3 /*OPEN_EXISTING*, 1 /*FILE_ATTRIBUTE_READONLY*, IntPtr.Zero);
                    if (handle.ToInt32() == -1 /*INVALID_HANDLE_VALUE*)
                    {
                        System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error());
                    }

                    if (GetFileTime(handle, ref ftCreationTime, ref ftLastAccessTime, ref ftLastWriteTime) != true)
                    {
                        System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error());
                    }

                    dtCreationTime = DateTime.FromFileTimeUtc((((long)ftCreationTime.dwHighDateTime) << 32) | ((uint)ftCreationTime.dwLowDateTime));
                    dtLastAccessTime = DateTime.FromFileTimeUtc((((long)ftLastAccessTime.dwHighDateTime) << 32) | ((uint)ftLastAccessTime.dwLowDateTime));
                    dtLastWriteTime = DateTime.FromFileTimeUtc((((long)ftLastWriteTime.dwHighDateTime) << 32) | ((uint)ftLastWriteTime.dwLowDateTime));
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (handle != IntPtr.Zero && handle.ToInt32() != -1 /*INVALID_HANDLE_VALUE*)
                    {
                        CloseHandle(handle);
                    }
                }
                return dtCreationTime;
                */

                return m_dtCreateTime;
            }

        }

        public DateTime LastWriteTime
        {

            get
            {

                //BUG: Unable to open pagefile.sys!!!
                /*
                DateTime dtCreationTime = DateTime.MinValue;
                DateTime dtLastAccessTime = DateTime.MinValue;
                DateTime dtLastWriteTime = DateTime.MinValue;
                IntPtr handle = IntPtr.Zero;
                FILETIME ftCreationTime = new FILETIME();
                FILETIME ftLastAccessTime = new FILETIME();
                FILETIME ftLastWriteTime = new FILETIME();
                try
                {
                    String sPath = "\\\\?\\" + m_sPath;
                    handle = CreateFile(sPath, (uint)0x80000000L /*GENERIC_READ*, 1 /*FILE_SHARE_READ*, IntPtr.Zero, 3 /*OPEN_EXISTING*, 1 /*FILE_ATTRIBUTE_READONLY*, IntPtr.Zero);
                    if (handle.ToInt32() == -1 /*INVALID_HANDLE_VALUE*)
                    {
                        System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error());
                    }

                    if (GetFileTime(handle, ref ftCreationTime, ref ftLastAccessTime, ref ftLastWriteTime) != true)
                    {
                        System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error());
                    }

                    dtCreationTime = DateTime.FromFileTimeUtc((((long)ftCreationTime.dwHighDateTime) << 32) | ((uint)ftCreationTime.dwLowDateTime));
                    dtLastAccessTime = DateTime.FromFileTimeUtc((((long)ftLastAccessTime.dwHighDateTime) << 32) | ((uint)ftLastAccessTime.dwLowDateTime));
                    dtLastWriteTime = DateTime.FromFileTimeUtc((((long)ftLastWriteTime.dwHighDateTime) << 32) | ((uint)ftLastWriteTime.dwLowDateTime));
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (handle != IntPtr.Zero && handle.ToInt32() != -1 /*INVALID_HANDLE_VALUE*)
                    {
                        CloseHandle(handle);
                    }
                }
                return ftLastWriteTime;
                */

                return m_dtLastWriteTime;
            }

        }

        public override void Delete()
        {

            throw new NotImplementedException();

        }

    }

    public class FileDirectoryEnumerable : System.Collections.IEnumerable
    {
        private bool bolReturnStringType = true;
        /// <summary>
        /// ??????????????,???true???????????,
        /// ???? System.IO.FileInfo?System.IO.DirectoryInfo??
        /// </summary>
        public bool ReturnStringType
        {
            get { return bolReturnStringType; }
            set { bolReturnStringType = value; }
        }
        private string strSearchPattern = "*";
        /// <summary>
        /// ??????????
        /// </summary>
        public string SearchPattern
        {
            get { return strSearchPattern; }
            set { strSearchPattern = value; }
        }
        private string strSearchPath = null;
        /// <summary>
        /// ????,???????
        /// </summary>
        public string SearchPath
        {
            get { return strSearchPath; }
            set { strSearchPath = value; }
        }
        private bool bolSearchForFile = true;
        /// <summary>
        /// ??????
        /// </summary>
        public bool SearchForFile
        {
            get { return bolSearchForFile; }
            set { bolSearchForFile = value; }
        }
        private bool bolSearchForDirectory = true;
        /// <summary>
        /// ???????
        /// </summary>
        public bool SearchForDirectory
        {
            get { return bolSearchForDirectory; }
            set { bolSearchForDirectory = value; }
        }
        private bool bolThrowIOException = true;
        /// <summary>
        /// ??IO?????????
        /// </summary>
        public bool ThrowIOException
        {
            get { return this.bolThrowIOException; }
            set { this.bolThrowIOException = value; }
        }
        /// <summary>
        /// ?????????????
        /// </summary>
        /// <returns>?????</returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            FileDirectoryEnumerator e = new FileDirectoryEnumerator();
            e.ReturnStringType = this.bolReturnStringType;
            e.SearchForDirectory = this.bolSearchForDirectory;
            e.SearchForFile = this.bolSearchForFile;
            e.SearchPath = this.strSearchPath;
            e.SearchPattern = this.strSearchPattern;
            e.ThrowIOException = this.bolThrowIOException;
            myList.Add(e);
            return e;
        }
        /// <summary>
        /// ????
        /// </summary>
        public void Close()
        {
            foreach (FileDirectoryEnumerator e in myList)
            {
                e.Close();
            }
            myList.Clear();
        }
        private System.Collections.ArrayList myList = new System.Collections.ArrayList();
    }//public class FileDirectoryEnumerable : System.Collections.IEnumerable
    /// <summary>
    /// ?????????
    /// </summary>
    /// <remarks>????Win32API?? FindFirstFile , FindNextFile 
    /// ? FindClose ?????
    /// 
    /// ????????? FileDirectoryEnumerator 
    /// 
    /// FileDirectoryEnumerator e = new FileDirectoryEnumerator();
    /// e.SearchPath = @"c:\";
    /// e.Reset();
    /// e.ReturnStringType = true ;
    /// while (e.MoveNext())
    /// {
    /// System.Console.WriteLine
    /// ( e.LastAccessTime.ToString("yyyy-MM-dd HH:mm:ss")
    /// + " " + e.FileLength + " \t" + e.Name );
    /// }
    /// e.Close();
    /// System.Console.ReadLine();
    /// 
    /// ?? ??? ( http://www.xdesigner.cn )2006-12-8</remarks>
    public class FileDirectoryEnumerator : System.Collections.IEnumerator
    {
        #region ?????????????? **********************************
        /// <summary>
        /// ????
        /// </summary>
        private string sCurrentShortFileName = "";
        private object objCurrentObject = null;
        private bool bolIsEmpty = false;
        /// <summary>
        /// ?????
        /// </summary>
        public bool IsEmpty
        {
            get { return bolIsEmpty; }
        }
        private int intSearchedCount = 0;
        /// <summary>
        /// ?????????
        /// </summary>
        public int SearchedCount
        {
            get { return intSearchedCount; }
        }
        private bool bolIsFile = true;
        /// <summary>
        /// ?????????,??true????????,?????
        /// </summary>
        public bool IsFile
        {
            get { return bolIsFile; }
        }
        private int intLastErrorCode = 0;
        /// <summary>
        /// ???????Win32????
        /// </summary>
        public int LastErrorCode
        {
            get { return intLastErrorCode; }
        }
        /// <summary>
        /// ???????
        /// </summary>
        public string NameLong
        {
            get
            {
                if (this.objCurrentObject != null)
                {
                    if (objCurrentObject is string)
                        return (string)this.objCurrentObject;
                    else
                        return ((System.IO.FileSystemInfo)this.objCurrentObject).Name;
                }
                return null;
            }
        }
        /// <summary>
        /// ???????
        /// </summary>
        public string NameShort83
        {
            get
            {
                if (this.objCurrentObject != null)
                {
                    return this.sCurrentShortFileName;
                }
                return null;
            }
        }
        /// <summary>
        /// ??????
        /// </summary>
        public System.IO.FileAttributes Attributes
        {
            get { return (System.IO.FileAttributes)myData.dwFileAttributes; }
        }
        /// <summary>
        /// ????????
        /// </summary>
        public System.DateTime CreationTime
        {
            get
            {
                long time = ToLong(myData.ftCreationTime_dwHighDateTime,
                    myData.ftCreationTime_dwLowDateTime);
                System.DateTime dtm = System.DateTime.FromFileTimeUtc(time);
                return dtm.ToLocalTime();
            }
        }
        /// <summary>
        /// ??????????
        /// </summary>
        public System.DateTime LastAccessTime
        {
            get
            {
                long time = ToLong(myData.ftLastAccessTime_dwHighDateTime,
                    myData.ftLastAccessTime_dwLowDateTime);
                System.DateTime dtm = System.DateTime.FromFileTimeUtc(time);
                return dtm.ToLocalTime();
            }
        }
        /// <summary>
        /// ??????????
        /// </summary>
        public System.DateTime LastWriteTime
        {
            get
            {
                long time = ToLong(myData.ftLastWriteTime_dwHighDateTime,
                    myData.ftLastWriteTime_dwLowDateTime);
                System.DateTime dtm = System.DateTime.FromFileTimeUtc(time);
                return dtm.ToLocalTime();
            }
        }
        /// <summary>
        /// ??????,????????????????,???????????0
        /// </summary>
        public long FileLength
        {
            get
            {
                if (this.bolIsFile)
                    return ToLong(myData.nFileSizeHigh, myData.nFileSizeLow);
                else
                    return 0;
            }
        }
        #endregion
        #region ??????????? ****************************************
        private bool bolThrowIOException = true;
        /// <summary>
        /// ??IO?????????
        /// </summary>
        public bool ThrowIOException
        {
            get { return this.bolThrowIOException; }
            set { this.bolThrowIOException = value; }
        }
        private bool bolReturnStringType = true;
        /// <summary>
        /// ??????????????,???true???????????,
        /// ???? System.IO.FileInfo?System.IO.DirectoryInfo??
        /// </summary>
        public bool ReturnStringType
        {
            get { return bolReturnStringType; }
            set { bolReturnStringType = value; }
        }
        private string strSearchPattern = "*";
        /// <summary>
        /// ??????????,?????
        /// </summary>
        public string SearchPattern
        {
            get { return strSearchPattern; }
            set { strSearchPattern = value; }
        }
        private string strSearchPath = null;
        /// <summary>
        /// ??????,???????,??????,???????
        /// </summary>
        public string SearchPath
        {
            get { return strSearchPath; }
            set { strSearchPath = value; }
        }
        private bool bolSearchForFile = true;
        /// <summary>
        /// ??????
        /// </summary>
        public bool SearchForFile
        {
            get { return bolSearchForFile; }
            set { bolSearchForFile = value; }
        }
        private bool bolSearchForDirectory = true;
        /// <summary>
        /// ???????
        /// </summary>
        public bool SearchForDirectory
        {
            get { return bolSearchForDirectory; }
            set { bolSearchForDirectory = value; }
        }
        #endregion
        /// <summary>
        /// ????,????
        /// </summary>
        public void Close()
        {
            this.CloseHandler();
        }
        #region IEnumerator ?? **********************************************
        /// <summary>
        /// ??????
        /// </summary>
        public object Current
        {
            get { return objCurrentObject; }
        }
        /// <summary>
        /// ??????????
        /// </summary>
        /// <returns>??????</returns>
        public bool MoveNext()
        {
            bool success = false;
            while (true)
            {
                if (this.bolStartSearchFlag)
                    success = this.SearchNext();
                else
                    success = this.StartSearch();
                if (success)
                {
                    if (this.UpdateCurrentObject())
                        return true;
                }
                else
                {
                    this.objCurrentObject = null;
                    return false;
                }
            }
        }
        /// <summary>
        /// ??????
        /// </summary>
        public void Reset()
        {
            if (this.strSearchPath == null)
                throw new System.ArgumentNullException("SearchPath can not null");
            if (this.strSearchPattern == null || this.strSearchPattern.Length == 0)
                this.strSearchPattern = "*";
            this.intSearchedCount = 0;
            this.objCurrentObject = null;
            this.CloseHandler();
            this.bolStartSearchFlag = false;
            this.bolIsEmpty = false;
            this.intLastErrorCode = 0;
        }
        #endregion
        #region ??WIN32API?????? **************************************
        [Serializable,
        System.Runtime.InteropServices.StructLayout
        (System.Runtime.InteropServices.LayoutKind.Sequential,
        CharSet = System.Runtime.InteropServices.CharSet.Auto
        ),
        System.Runtime.InteropServices.BestFitMapping(false)]
        private struct WIN32_FIND_DATA
        {
            public int dwFileAttributes;
            public int ftCreationTime_dwLowDateTime;
            public int ftCreationTime_dwHighDateTime;
            public int ftLastAccessTime_dwLowDateTime;
            public int ftLastAccessTime_dwHighDateTime;
            public int ftLastWriteTime_dwLowDateTime;
            public int ftLastWriteTime_dwHighDateTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [System.Runtime.InteropServices.MarshalAs
            (System.Runtime.InteropServices.UnmanagedType.ByValTStr,
            SizeConst = 260)]
            public string cFileName;
            [System.Runtime.InteropServices.MarshalAs
            (System.Runtime.InteropServices.UnmanagedType.ByValTStr,
            SizeConst = 14)]
            public string cAlternateFileName;
        }
        [System.Runtime.InteropServices.DllImport
        ("kernel32.dll",
        CharSet = System.Runtime.InteropServices.CharSet.Auto,
        SetLastError = true)]
        private static extern IntPtr FindFirstFile(string pFileName,
            ref WIN32_FIND_DATA pFindFileData);
        [System.Runtime.InteropServices.DllImport
        ("kernel32.dll",
        CharSet = System.Runtime.InteropServices.CharSet.Auto,
        SetLastError = true)]
        private static extern bool FindNextFile(IntPtr hndFindFile,
            ref WIN32_FIND_DATA lpFindFileData);
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FindClose(IntPtr hndFindFile);
        private static long ToLong(int height, int low)
        {
            long v = (uint)height;
            v = v << 0x20;
            v = v | ((uint)low);
            return v;
        }
        private static void WinIOError(int errorCode, string str)
        {
            switch (errorCode)
            {
                case 80:
                    throw new System.IO.IOException("IO_FileExists :" + str);
                case 0x57:
                    throw new System.IO.IOException("IOError:" + MakeHRFromErrorCode(errorCode));
                case 0xce:
                    throw new System.IO.PathTooLongException("PathTooLong:" + str);
                case 2:
                    throw new System.IO.FileNotFoundException("FileNotFound:" + str);
                case 3:
                    throw new System.IO.DirectoryNotFoundException("PathNotFound:" + str);
                case 5:
                    throw new UnauthorizedAccessException("UnauthorizedAccess:" + str);
                case 0x20:
                    throw new System.IO.IOException("IO_SharingViolation:" + str);
            }
            throw new System.IO.IOException("IOError:" + MakeHRFromErrorCode(errorCode));
        }
        private static int MakeHRFromErrorCode(int errorCode)
        {
            return (-2147024896 | errorCode);
        }
        #endregion
        #region ????? ****************************************************
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        /// <summary>
        /// ?????????
        /// </summary>
        private System.IntPtr intSearchHandler = INVALID_HANDLE_VALUE;
        private WIN32_FIND_DATA myData = new WIN32_FIND_DATA();
        /// <summary>
        /// ??????
        /// </summary>
        private bool bolStartSearchFlag = false;
        /// <summary>
        /// ??????
        /// </summary>
        private void CloseHandler()
        {
            if (this.intSearchHandler != INVALID_HANDLE_VALUE)
            {
                FindClose(this.intSearchHandler);
                this.intSearchHandler = INVALID_HANDLE_VALUE;
            }
        }
        /// <summary>
        /// ????
        /// </summary>
        /// <returns>??????</returns>
        private bool StartSearch()
        {
            bolStartSearchFlag = true;
            bolIsEmpty = false;
            objCurrentObject = null;
            intLastErrorCode = 0;

            //NOT NEEDED!!!
            //string strPath = System.IO.Path.Combine(strSearchPath, this.strSearchPattern);
            string strPath = strSearchPath + this.strSearchPattern;

            this.CloseHandler();
            intSearchHandler = FindFirstFile( "\\\\?\\" + strPath, ref myData);
            if (intSearchHandler == INVALID_HANDLE_VALUE)
            {
                intLastErrorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                if (intLastErrorCode == 2)
                {
                    bolIsEmpty = true;
                    return false;
                }
                if (this.bolThrowIOException)
                    WinIOError(intLastErrorCode, strSearchPath);
                else
                    return false;
            }
            return true;
        }
        /// <summary>
        /// ?????
        /// </summary>
        /// <returns>??????</returns>
        private bool SearchNext()
        {
            if (bolStartSearchFlag == false)
                return false;
            if (bolIsEmpty)
                return false;
            if (intSearchHandler == INVALID_HANDLE_VALUE)
                return false;
            intLastErrorCode = 0;
            if (FindNextFile(intSearchHandler, ref myData) == false)
            {
                intLastErrorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                this.CloseHandler();
                if (intLastErrorCode != 0 && intLastErrorCode != 0x12)
                {
                    if (this.bolThrowIOException)
                        WinIOError(intLastErrorCode, strSearchPath);
                    else
                        return false;
                }
                return false;
            }
            return true;
        }//private bool SearchNext()
        /// <summary>
        /// ??????
        /// </summary>
        /// <returns>??????</returns>
        private bool UpdateCurrentObject()
        {
            if (intSearchHandler == INVALID_HANDLE_VALUE)
                return false;
            bool Result = false;
            this.objCurrentObject = null;
            if ((myData.dwFileAttributes & 0x10) == 0)
            {
                // ???????
                this.bolIsFile = true;
                if (this.bolSearchForFile)
                    Result = true;
            }
            else
            {
                // ???????
                this.bolIsFile = false;
                if (this.bolSearchForDirectory)
                {
                    if (myData.cFileName == "." || myData.cFileName == "..")
                        Result = false;
                    else
                        Result = true;
                }
            }
            if (Result)
            {
                string sFileName;

                //sFileName = myData.cFileName; //Long Name

                sFileName = myData.cAlternateFileName; //Short Name (8+3 format)
                if (sFileName.Length == 0) sFileName = myData.cFileName;

                if (this.bolReturnStringType)
                {
                    this.sCurrentShortFileName = sFileName;
                    this.objCurrentObject = sFileName; //myData.cFileName;
                }
                else
                {
                    this.sCurrentShortFileName = sFileName;

                    //NOT NEEDED
                    //string p = System.IO.Path.Combine(this.strSearchPath, sFileName); //myData.cFileName);

                    //WinXHome FIX: Do not use Short File Name for files!!!
                    //string sPath = this.strSearchPath + sFileName;

                    string sPath;
                    if (this.bolIsFile)
                    {
                        //WinXHome FIX: Do not use Short File Name for files!!!
                        //sPath = this.strSearchPath + sFileName;
                        sPath = this.strSearchPath + myData.cFileName;

                        //WinXHome FIX
                        //this.objCurrentObject = new System.IO.FileInfo(sPath);
                        this.objCurrentObject = new LongFileInfo(sPath);
                    }
                    else
                    {
                        sPath = this.strSearchPath + sFileName; //ATTN: 8+3 Short File Name!!!
                        this.objCurrentObject = new System.IO.DirectoryInfo(sPath);
                    }
                }
                this.intSearchedCount++;
            }
            return Result;
        }//private bool UpdateCurrentObject()
        #endregion
    }//public class FileDirectoryEnumerator : System.Collections.IEnumerator
}