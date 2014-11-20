using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace Task3
{
    public static class Toolhelp32
    {
        public const uint TH32CS_SNAPHEAPLIST = 0x00000001;
        public const uint TH32CS_SNAPPROCESS = 0x00000002;
        public const uint TH32CS_SNAPTHREAD = 0x00000004;
        public const uint TH32CS_SNAPMODULE = 0x00000008;
        public const uint TH32CS_SNAPMODULE32 = 0x00000010;
        public const uint TH32CS_SNAPALL = (TH32CS_SNAPHEAPLIST |
                                                 TH32CS_SNAPPROCESS |
                                                 TH32CS_SNAPTHREAD |
                                                 TH32CS_SNAPMODULE);
        public const uint TH32CS_INHERIT = 0x80000000;

        public const uint HF32_DEFAULT = 1;
        public const uint HF32_SHARED = 2;

        public const uint LF32_FIXED = 0x00000001;
        public const uint LF32_FREE = 0x00000002;
        public const uint LF32_MOVEABLE = 0x00000004;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool HeapValidate(IntPtr hHeap, UInt32 dwFlags, IntPtr lpMem);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern UInt32 HeapSize(IntPtr hHeap, UInt32 dwFlags, IntPtr lpMem);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("kernel32.dll")]
        public static extern bool Heap32ListFirst(IntPtr hSnapshot, ref HEAPLIST32 lphl);

        [DllImport("kernel32.dll")]
        public static extern bool Heap32ListNext(IntPtr hSnapshot, ref HEAPLIST32 lphl);

        [DllImport("kernel32.dll")]
        public static extern bool Heap32First(ref HEAPENTRY32 lphe,
            uint th32ProcessID, uint th32HeapID);

        [DllImport("kernel32.dll")]
        public static extern bool Heap32Next(ref HEAPENTRY32 lphe);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool Process32First([In]IntPtr hSnapshot, ref ProcessEntry32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool Process32Next([In]IntPtr hSnapshot, ref ProcessEntry32 lppe);

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public static List<ProcessEntry32> GetAllProcess()
        {
            List<ProcessEntry32> res = new List<ProcessEntry32>();
            IntPtr handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            if (handle == INVALID_HANDLE_VALUE)
                return null;
            ProcessEntry32 Process = new ProcessEntry32();
            Process.dwSize = (uint)Marshal.SizeOf(typeof(ProcessEntry32));
            if (Process32First(handle,ref Process))
                do
                {
                        res.Add(Process);
                }
                while (Process32Next(handle,ref Process));
            return res;

        }

        public static List<HEAPENTRY32> GetAllHeapsByProcess(uint processId)
        {
            IntPtr handle = CreateToolhelp32Snapshot(TH32CS_SNAPHEAPLIST, processId);
            if (handle == INVALID_HANDLE_VALUE)
                return null;
            List<HEAPENTRY32> result = new List<HEAPENTRY32>();

            HEAPLIST32 hl = new HEAPLIST32();

            hl.dwSize = (uint)Marshal.SizeOf(hl);
            try
            {
                if (Heap32ListFirst(handle, ref hl))
                    do
                    {
                        HEAPENTRY32 he = new HEAPENTRY32();
                        he.dwSize = (uint)Marshal.SizeOf(he);
                        if (Heap32First(ref he, hl.th32ProcessID, hl.th32HeapID))
                        {
                            do
                            {
                                if (he.dwFlags == LF32_FIXED)
                                {
                                    HEAPENTRY32 pe = new HEAPENTRY32();
                                    pe = he;
                                    result.Add(pe);
                                }
                            }
                            while (Heap32Next(ref he));
                        }
                    }
                    while (Heap32ListNext(handle, ref hl));
            }
            finally
            {
                CloseHandle(handle);
            }
            return result;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ProcessEntry32
    {
        const int MAX_PATH = 260;
        public uint dwSize; 
        public uint cntUsage; 
        public uint th32ProcessID; 
        public IntPtr th32DefaultHeapID; 
        public uint th32ModuleID;  
        public uint cntThreads; 
        public uint th32ParentProcessID; 
        public int pcPriClassBase; 
        public uint dwFlags; 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string szExeFile;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct HEAPLIST32
    {
        public uint dwSize;
        public uint th32ProcessID;
        public uint th32HeapID;
        public uint dwFlags;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct HEAPENTRY32
    {
        public uint dwSize;
        public IntPtr hHandle;
        public uint dwAddress;
        public uint dwBlockSize;
        public uint dwFlags;
        public uint dwLockCount;
        public uint dwResvd;
        public uint th32ProcessID;
        public uint th32HeapID;
    }
}
