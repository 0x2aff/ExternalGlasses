using System;
using System.Runtime.InteropServices;

namespace ExternalGlasses.Imports
{
    public static class Kernel32
    {
        public const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const uint PROCESS_CREATE_PROCESS = 0x0080;
        public const uint PROCESS_CREATE_THREAD = 0x0002;
        public const uint PROCESS_DUP_HANDLE = 0x0040;
        public const uint PROCESS_QUERY_INFORMATION = 0x0400;
        public const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x0080;
        public const uint PROCESS_SET_INFORMATION = 0x0200;
        public const uint PROCESS_SET_QUOTA = 0x0100;
        public const uint PROCESS_SUSPEND_RESUME = 0x0800;
        public const uint PROCESS_TERMINATE = 0x0001;
        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, Int32 lpBaseAddress, [Out] byte[] lpBuffer, UInt32 dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
