using System;
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

        private DMC3MusicConfig Config { get; set; }

        private Timer SongChangeTimer { get; set; }

        private SongPlayer Player { get; set; }

        private Process DMC3Process { get; set; }
        private IntPtr ProcessHandle { get; set; }
        private int BaseAddress { get; set; }

        private bool ConfigChanged { get; set; }

        public Form1()
        {
            InitializeComponent();
            Config = DMC3MusicConfigWriter.ReadConfig();
            Player = new SongPlayer(Config);
            SongChangeTimer = new Timer();
            ConfigChanged = false;
        }

        #region Form Control Methods

        private void Form1_Load(object sender, EventArgs e)
        {
            shuffleCheckBox.Checked = Config.Shuffle;
            changeShuffle.Enabled = shuffleCheckBox.Checked;
        }

        private void startMusic_Click(object sender, EventArgs e)
        {
            if (ConfigChanged && !SaveConfigPrompt()) return;

            DisableConfigControls();

            Player = new SongPlayer(Config);
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

        private void stopMusic_Click(object sender, EventArgs e)
        {
            SongChangeTimer.Stop();
            Player.Stop();
            EnableConfigControls();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SaveConfigPrompt()) e.Cancel = true;
        }

        private void changeShuffle_Click(object sender, EventArgs e)
        {
            var shuffleForm = new ShuffleRotation(Config);
            DialogResult result = shuffleForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                Config = shuffleForm.Config;
                ConfigChanged = true;
            }
            shuffleForm.Dispose();
        }

        private void shuffleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.Shuffle = shuffleCheckBox.Checked;
            changeShuffle.Enabled = shuffleCheckBox.Checked;
            ConfigChanged = true;
        }

        #endregion

        #region Functionality Methods

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

        private bool SaveConfigPrompt()
        {
            DialogResult result = MessageBox.Show(
                "Would you like to save your current configuration settings?",
                "Save Configuration",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Exclamation
            );

            if (result == DialogResult.Yes)
            {
                DMC3MusicConfigWriter.WriteConfig(Config);
                ConfigChanged = false;
            }
            return result != DialogResult.Cancel;
        }

        private void DisableConfigControls()
        {
            shuffleCheckBox.Enabled = false;
            changeShuffle.Enabled = false;
            startMusic.Enabled = false;
        }

        private void EnableConfigControls()
        {
            shuffleCheckBox.Enabled = true;
            changeShuffle.Enabled = true;
            startMusic.Enabled = true;
        }

        #endregion
    }
}
