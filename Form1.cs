﻿using System;
using NAudio.Wave;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace dmc3music
{
    public partial class Form1 : Form
    {
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        public static extern int ReadProcessMemory(IntPtr hProcess, int lpBase, ref int lpBuffer, int nSize, int lpNumberOfBytesRead);

        private Timer SongChangeTimer { get; set; }

        private SongPlayer Player { get; set; }

        private Process DMC3Process { get; set; }
        private IntPtr ProcessHandle { get; set; }
        private int BaseAddress { get; set; }

        public Form1()
        {
            InitializeComponent();
            Player = new SongPlayer();
        }

        public static class Globals
        {
            public static WaveOut waveOut = new WaveOut();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DMC3Process = Process.GetProcessesByName("dmc3se")[0];
            } catch
            {
                MessageBox.Show("pls start game first", "dmc3se.exe not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ProcessHandle = OpenProcess(PROCESS_WM_READ, false, DMC3Process.Id);
            BaseAddress = DMC3Process.MainModule.BaseAddress.ToInt32();
            SongChangeTimer = new Timer();
            SongChangeTimer.Interval = 250;
            SongChangeTimer.Tick += new EventHandler(CheckSong);
            SongChangeTimer.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SongChangeTimer.Stop();
            Player.Stop();
        }

        private void CheckSong(object sender, EventArgs e)
        {
            int checkRoom = 0;
            ReadProcessMemory(ProcessHandle, BaseAddress + 0x20C39EC, ref checkRoom, sizeof(int), 0);
            if (checkRoom == 0)
            {
                Player.Stop();
                return;
            }

            int roomId = -1;
            int enemyCount = -1;
            int enemyCountPtr1 = -1;
            int enemyCountPtr2 = -1;
            ReadProcessMemory(ProcessHandle, BaseAddress + 0x76B150, ref roomId, sizeof(int), 0);
            ReadProcessMemory(ProcessHandle, BaseAddress + 0x76B860 + 0xC40 + 0x8, ref enemyCountPtr1, sizeof(int), 0);
            ReadProcessMemory(ProcessHandle, enemyCountPtr1 + 0x18, ref enemyCountPtr2, sizeof(int), 0);
            ReadProcessMemory(ProcessHandle, enemyCountPtr2 + 0xA78, ref enemyCount, sizeof(int), 0);
            Player.PlayRoomSong(roomId, enemyCount);
        }
    }
}
