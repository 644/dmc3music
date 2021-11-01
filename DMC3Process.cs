using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dmc3music
{
    class DMC3Process : IDisposable
    {
        [DllImport("kernel32.dll")]
        private static extern UIntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        private static extern int ReadProcessMemory(UIntPtr hProcess, IntPtr lpBase, ref int lpBuffer, int nSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(UIntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(UIntPtr hProcess);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        public Process Process { get; set; }
        public UIntPtr Handle { get; set; }
        public IntPtr BaseAddress { get; set; }
        public Version VersionInfo { get; set; }
        public bool ProcHasExited { get; set; } = false;

        private void HasExited(object sender, EventArgs e)
        {
            ProcHasExited = true;
        }

        public bool OpenReadOnly()
        {
            Process[] localProcess = Process.GetProcessesByName("dmc3se");

            if (localProcess.Length == 0)
            {
                return false;
            }

            Process = localProcess[0];
            Handle = OpenProcess(ProcessAccessFlags.VMRead, false, Process.Id);

            if(Handle == UIntPtr.Zero)
            {
                return false;
            }

            BaseAddress = Process.MainModule.BaseAddress;
            try
            {
                VersionInfo = new Version(Process.MainModule.FileVersionInfo.ProductVersion.Replace(",", "."));
            }
            catch
            {
                return false;
            }

            Process.EnableRaisingEvents = true;
            Process.Exited += new EventHandler(HasExited);

            return true;
        }

        public bool OpenReadWrite()
        {
            Process[] localProcess = Process.GetProcessesByName("dmc3se");

            if (localProcess.Length == 0)
            {
                return false;
            }

            Process = localProcess[0];
            Handle = OpenProcess(ProcessAccessFlags.All, false, Process.Id);

            if (Handle == UIntPtr.Zero)
            {
                return false;
            }

            BaseAddress = Process.MainModule.BaseAddress;
            try
            {
                VersionInfo = new Version(Process.MainModule.FileVersionInfo.ProductVersion.Replace(",", "."));
            }
            catch
            {
                return false;
            }

            Process.EnableRaisingEvents = true;
            Process.Exited += new EventHandler(HasExited);

            return true;
        }

        public void WriteMem(byte[] bytes, int address)
        {
            IntPtr offset = IntPtr.Add(BaseAddress, address);
            WriteProcessMemory(Handle, offset, bytes, (uint)bytes.LongLength, out _);
        }

        public void WriteInt(int intVal, int address)
        {
            byte[] bytes = new byte[4];

            bytes[3] = (byte)(intVal >> 24);
            bytes[2] = (byte)(intVal >> 16);
            bytes[1] = (byte)(intVal >> 8);
            bytes[0] = (byte)intVal;

            WriteProcessMemory(Handle, new IntPtr(address), bytes, (uint)bytes.LongLength, out _);
        }

        public void WriteExactMem(byte[] bytes, int address)
        {
            WriteProcessMemory(Handle, new IntPtr(address), bytes, (uint)bytes.LongLength, out _);
        }

        public int ReadExactMem(int address)
        {
            int res = -1;
            ReadProcessMemory(Handle, new IntPtr(address), ref res, sizeof(int), 0);
            return res;
        }

        public int ReadMem(int address)
        {
            IntPtr offset = IntPtr.Add(BaseAddress, address);
            int res = -1;
            ReadProcessMemory(Handle, offset, ref res, sizeof(int), 0);
            return res;
        }

        public int GetIntPtr(IntPtr address)
        {
            int res = 0;
            ReadProcessMemory(Handle, address, ref res, sizeof(int), 0);
            return res;
        }

        private bool _disposedValue;

        private readonly SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    CloseHandle(Handle);
                    _safeHandle.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
