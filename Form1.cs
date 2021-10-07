using System;
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

        private Timer GameStartTimer { get; set; }

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
            volumeSlider1.VolumeChanged += OnVolumeSliderChanged;
            GameStartTimer = new Timer();
            GameStartTimer.Interval = 250;
            GameStartTimer.Tick += new EventHandler(GameStart);
            GameStartTimer.Start();
        }

        #region Form Control Methods

        private void Form1_Load(object sender, EventArgs e)
        {
            shuffleCheckBox.Checked = Config.Shuffle;
            changeShuffle.Enabled = shuffleCheckBox.Checked;
            numericUpDown1.Value = Config.BattleTimer;
            numericUpDown2.Value = Config.AmbientTimer;
        }
        void OnVolumeSliderChanged(object sender, EventArgs e)
        {
            Player.Volume(volumeSlider1.Volume);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ConfigChanged && !SaveConfigPrompt()) e.Cancel = true;
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (ConfigChanged && !SaveConfigPrompt()) return;

            DisableConfigControls();

            Player = new SongPlayer(Config);
            try
            {
                DMC3Process = Process.GetProcessesByName("dmc3se")[0];
            }
            catch
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SongChangeTimer.Stop();
            Player.Stop();
            EnableConfigControls();
        }

        #endregion

        #region Functionality Methods

        private void GameStart(object sender, EventArgs e)
        {
            Player = new SongPlayer(Config);
            try
            {
                DMC3Process = Process.GetProcessesByName("dmc3se")[0];
            }
            catch
            {
                return;
            }
            GameStartTimer.Stop();
            DisableConfigControls();
            ProcessHandle = OpenProcess(PROCESS_WM_READ, false, DMC3Process.Id);
            BaseAddress = DMC3Process.MainModule.BaseAddress.ToInt32();
            SongChangeTimer = new Timer();
            SongChangeTimer.Interval = 250;
            SongChangeTimer.Tick += new EventHandler(CheckSong);
            SongChangeTimer.Start();
        }

        private void CheckSong(object sender, EventArgs e)
        {
            if (Player.isPlaying)
                label2.Text = "Playing: " + Player.OldTrack;
            else
                label2.Text = "Not Playing";

            int checkRoom = 0;
            ReadProcessMemory(ProcessHandle, BaseAddress + 0x20C39EC, ref checkRoom, sizeof(int), 0);

            if (checkRoom == 0)
            {
                Player.Stop();
                if (Process.GetProcessesByName("dmc3se").Length == 0)
                {
                    SongChangeTimer.Stop();
                    GameStartTimer.Start();
                }
                return;
            }

            int roomId = -1;
            int enemyCount = -1;
            int enemyCountPtr1 = -1;
            int enemyCountPtr2 = -1;
            int missionNumber = -1;
            ReadProcessMemory(ProcessHandle, BaseAddress + 0x76B150, ref roomId, sizeof(int), 0);
            ReadProcessMemory(ProcessHandle, BaseAddress + 0x76B860 + 0xC40 + 0x8, ref enemyCountPtr1, sizeof(int), 0);
            ReadProcessMemory(ProcessHandle, enemyCountPtr1 + 0x18, ref enemyCountPtr2, sizeof(int), 0);
            ReadProcessMemory(ProcessHandle, enemyCountPtr2 + 0xA78, ref enemyCount, sizeof(int), 0);
            ReadProcessMemory(ProcessHandle, BaseAddress + 0x76B148, ref missionNumber, sizeof(int), 0);

            Player.PlayRoomSong(roomId, enemyCount, missionNumber);
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
            pictureBox2.Enabled = false;
        }

        private void EnableConfigControls()
        {
            shuffleCheckBox.Enabled = true;
            changeShuffle.Enabled = true;
            pictureBox2.Enabled = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (Config.BattleTimer != (int)numericUpDown1.Value)
            {
                Config.BattleTimer = (int)numericUpDown1.Value;
                ConfigChanged = true;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (Config.AmbientTimer != (int)numericUpDown2.Value)
            {
                Config.AmbientTimer = (int)numericUpDown2.Value;
                ConfigChanged = true;
            }
        }

        #endregion
    }
}
