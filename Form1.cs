using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NVorbis;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.PowerBI.Api.Models;
using System.Collections;

namespace dmc3music
{
    public partial class Form1 : Form
    {
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        public static extern int ReadProcessMemory(IntPtr hProcess, int lpBase, ref int lpBuffer, int nSize, int lpNumberOfBytesRead);

        public Form1()
        {
            InitializeComponent();
        }

        public static class Globals
        {
            public static WaveOut waveOut = new NAudio.Wave.WaveOut();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hashtable songTable = new Hashtable(){
                {0, "Battle_00.ogg"},
                {1, "Battle_01.ogg"},
                {2, "Battle_01.ogg"},
                {6, "Boss_01.ogg"}
            };
            Process process = Process.GetProcessesByName("dmc3se")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            int baseAddress = process.MainModule.BaseAddress.ToInt32();
            bool isPlaying = false;
            int playSong = 0;
            int roomId = 0;

            while (true)
            {
                ReadProcessMemory(processHandle, baseAddress + 0x20C39EC, ref playSong, sizeof(int), 0);
                if(playSong == 1 && !isPlaying)
                {
                    isPlaying = true;
                    ReadProcessMemory(processHandle, baseAddress + 0x76B150, ref roomId, sizeof(int), 0);
                    var track = songTable[roomId];
                    Console.WriteLine(track);
                    var vorbis = new NAudio.Vorbis.VorbisWaveReader(@"D:\SteamLibrary\steamapps\common\Devil May Cry 3\native\sound\" + track);
                    Globals.waveOut.Init(vorbis);
                    Globals.waveOut.Play();
                }
                else if(playSong == 0 && isPlaying)
                {
                    isPlaying = false;
                    Globals.waveOut?.Stop();
                }
                System.Threading.Thread.Sleep(250);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Globals.waveOut?.Stop();
        }
    }
}
