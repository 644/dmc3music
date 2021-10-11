using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

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
            try
            {
                InitializeComponent();
            }
            catch { }
            try
            {
                Config = DMC3MusicConfigWriter.ReadConfig();
                Player = new SongPlayer(Config);
                SongChangeTimer = new Timer();
            }
            catch
            {
                MessageBox.Show("Error", "There was a problem opening the config", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                ConfigChanged = false;
                volumeSlider1.VolumeChanged += OnVolumeSliderChanged;
                GameStartTimer = new Timer();
                GameStartTimer.Interval = 50;
                GameStartTimer.Tick += new EventHandler(GameStart);
                GameStartTimer.Start();
            }
            catch
            {
                System.Windows.Forms.Application.Exit();
            }
        }

        #region Form Control Methods

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                shuffleCheckBox.Checked = Config.Shuffle;
                changeShuffle.Enabled = shuffleCheckBox.Checked;
                numericUpDown1.Value = Config.BattleTimer;
                numericUpDown2.Value = Config.AmbientTimer;
                textBox1.Text = Config.DMC3Path;
            }
            catch
            {
                MessageBox.Show("Error", "There was a problem opening the config", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                foreach (string filename in Directory.EnumerateFiles("./saves", "*.sav"))
                {
                    string saveName = filename.Split('\\').Last();
                    comboBox1.Items.Add(saveName);
                }
            }
            catch { }
        }
        void OnVolumeSliderChanged(object sender, EventArgs e)
        {
            try
            {
                Player.Volume(volumeSlider1.Volume);
            }
            catch { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (ConfigChanged && !SaveConfigPrompt()) e.Cancel = true;
            }
            catch { }
        }

        private void changeShuffle_Click(object sender, EventArgs e)
        {
            try
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
            catch { }
        }

        private void shuffleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Config.Shuffle = shuffleCheckBox.Checked;
                changeShuffle.Enabled = shuffleCheckBox.Checked;
                ConfigChanged = true;
            }
            catch { }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (ConfigChanged && !SaveConfigPrompt()) return;

            DisableConfigControls();

            try
            {
                Player = new SongPlayer(Config);
            }
            catch
            {
                MessageBox.Show("Error", "There was a problem loading the Song Player", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                DMC3Process = Process.GetProcessesByName("dmc3se")[0];
            }
            catch
            {
                MessageBox.Show("pls start game first", "dmc3se.exe not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                ProcessHandle = OpenProcess(PROCESS_WM_READ, false, DMC3Process.Id);
                BaseAddress = DMC3Process.MainModule.BaseAddress.ToInt32();
                SongChangeTimer = new Timer();
                SongChangeTimer.Interval = 50;
                SongChangeTimer.Tick += new EventHandler(CheckSong);
                SongChangeTimer.Start();
            }
            catch { }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                SongChangeTimer.Stop();
                Player.Stop();
                EnableConfigControls();
            }
            catch
            {
                MessageBox.Show("Error", "There was a problem stopping the Song Player", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                var ControllerForm = new ControllerConfig();
                DialogResult result = ControllerForm.ShowDialog();
                ControllerForm.Dispose();
            }
            catch { }
        }

        #endregion

        #region Functionality Methods

        private void GameStart(object sender, EventArgs e)
        {
            try
            {
                DMC3Process = Process.GetProcessesByName("dmc3se")[0];
            }
            catch
            {
                return;
            }
            Player = new SongPlayer(Config);
            try
            {
                GameStartTimer.Stop();
                DisableConfigControls();
                ProcessHandle = OpenProcess(PROCESS_WM_READ, false, DMC3Process.Id);
                BaseAddress = DMC3Process.MainModule.BaseAddress.ToInt32();
                SongChangeTimer = new Timer();
                SongChangeTimer.Interval = 50;
                SongChangeTimer.Tick += new EventHandler(CheckSong);
                SongChangeTimer.Start();
            }
            catch
            {
                MessageBox.Show("Error", "There was a problem opening the DMC3 Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckSong(object sender, EventArgs e)
        {
            try
            {
                if (Player.isPlaying)
                    label2.Text = "Playing: " + Player.OldTrack;
                else
                    label2.Text = "Not Playing";

                if (Process.GetProcessesByName("dmc3se").Length == 0)
                {
                    Player.Stop();
                    SongChangeTimer.Stop();
                    GameStartTimer.Start();
                    return;
                }
            }
            catch { }

            try
            {
                int checkRoom = 0;
                ReadProcessMemory(ProcessHandle, BaseAddress + 0x20C39EC, ref checkRoom, sizeof(int), 0);

                if (checkRoom == 0)
                {
                    if (Player.isPlaying)
                    {
                        Player.FadeOut();
                    }
                    return;
                }

                int roomId = -1;
                int enemyCount = -1;
                int enemyCountPtr1 = -1;
                int enemyCountPtr2 = -1;
                int missionNumber = -1;
                int isLoading = -1;
                ReadProcessMemory(ProcessHandle, BaseAddress + 0x76B150, ref roomId, sizeof(int), 0);
                ReadProcessMemory(ProcessHandle, BaseAddress + 0x76B860 + 0xC40 + 0x8, ref enemyCountPtr1, sizeof(int), 0);
                ReadProcessMemory(ProcessHandle, enemyCountPtr1 + 0x18, ref enemyCountPtr2, sizeof(int), 0);
                ReadProcessMemory(ProcessHandle, enemyCountPtr2 + 0xA78, ref enemyCount, sizeof(int), 0);
                ReadProcessMemory(ProcessHandle, BaseAddress + 0x76B148, ref missionNumber, sizeof(int), 0);
                ReadProcessMemory(ProcessHandle, BaseAddress + 0x205BCB8, ref isLoading, sizeof(int), 0);

                Player.PlayRoomSong(roomId, enemyCount, missionNumber);
            }
            catch { }
        }

        private bool SaveConfigPrompt()
        {
            try
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
            catch
            {
                return false;
            }
        }

        private void DisableConfigControls()
        {
            try
            {
                shuffleCheckBox.Enabled = false;
                changeShuffle.Enabled = false;
                pictureBox2.Enabled = false;
            }
            catch { }
        }

        private void EnableConfigControls()
        {
            try
            {
                shuffleCheckBox.Enabled = true;
                changeShuffle.Enabled = true;
                pictureBox2.Enabled = true;
            }
            catch { }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (Config.BattleTimer != (int)numericUpDown1.Value)
                {
                    Config.BattleTimer = (int)numericUpDown1.Value;
                    ConfigChanged = true;
                }
            }
            catch { }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (Config.AmbientTimer != (int)numericUpDown2.Value)
                {
                    Config.AmbientTimer = (int)numericUpDown2.Value;
                    ConfigChanged = true;
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = folderBrowserDialog1.SelectedPath;
                    Config.DMC3Path = textBox1.Text;
                    ConfigChanged = true;
                }
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var fileName = comboBox1.SelectedItem.ToString();
                var source = Path.Combine("./saves", fileName);
                var destination = Path.Combine(Config.DMC3Path, "save0.sav");

                File.Copy(source, destination, true);
                label7.Text = $"Copied '{fileName}' Successfully!";
                label7.MaximumSize = new Size((sender as Control).ClientSize.Width - label7.Left, 10000);
            }
            catch
            {
                label7.Text = "Failed To Copy!";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, checkBox1.Checked);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Livesplit Split File (*.lss)|*.lss";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                CloseOutput = true,
                OmitXmlDeclaration = false
            };

            using (XmlWriter writer = XmlWriter.Create(dlg.FileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Run");
                writer.WriteAttributeString("version", null, "1.7.0");
                writer.WriteStartElement("GameIcon");
                writer.WriteEndElement();
                writer.WriteStartElement("GameName");
                writer.WriteString("Devil May Cry 3: Special Edition");
                writer.WriteEndElement();
                writer.WriteStartElement("CategoryName");
                writer.WriteString("Dante New Game");
                writer.WriteEndElement();
                writer.WriteStartElement("Metadata");
                writer.WriteStartElement("Run");
                writer.WriteAttributeString("id", null, "");
                writer.WriteEndElement();
                writer.WriteStartElement("Platform");
                writer.WriteAttributeString("usesEmulator", null, "False");
                writer.WriteEndElement();
                writer.WriteStartElement("Region");
                writer.WriteEndElement();
                writer.WriteStartElement("Variables");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("Offset");
                writer.WriteString("00:00:00");
                writer.WriteEndElement();
                writer.WriteStartElement("AttemptCount");
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteStartElement("AttemptHistory");
                writer.WriteEndElement();
                writer.WriteStartElement("Segments");

                foreach (int item in listBox1.SelectedIndices)
                {
                    foreach (string room in SplitsGenerator.roomsByMission[item])
                    {
                        writer.WriteStartElement("Segment");
                        writer.WriteStartElement("Name");
                        writer.WriteString(room);
                        writer.WriteEndElement();
                        writer.WriteStartElement("Icon");
                        writer.WriteEndElement();
                        writer.WriteStartElement("SplitTimes");
                        writer.WriteStartElement("SplitTime");
                        writer.WriteAttributeString("name", null, "Personal Best");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteStartElement("BestSegmentTime");
                        writer.WriteEndElement();
                        writer.WriteStartElement("SegmentHistory");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
                writer.WriteStartElement("AutoSplitterSettings");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Livesplit Split File (*.lss)|*.lss";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                CloseOutput = true,
                OmitXmlDeclaration = false
            };

            using (XmlWriter writer = XmlWriter.Create(dlg.FileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Run");
                writer.WriteAttributeString("version", null, "1.7.0");
                writer.WriteStartElement("GameIcon");
                writer.WriteEndElement();
                writer.WriteStartElement("GameName");
                writer.WriteString("Devil May Cry 3: Special Edition");
                writer.WriteEndElement();
                writer.WriteStartElement("CategoryName");
                writer.WriteString("Dante New Game");
                writer.WriteEndElement();
                writer.WriteStartElement("Metadata");
                writer.WriteStartElement("Run");
                writer.WriteAttributeString("id", null, "");
                writer.WriteEndElement();
                writer.WriteStartElement("Platform");
                writer.WriteAttributeString("usesEmulator", null, "False");
                writer.WriteEndElement();
                writer.WriteStartElement("Region");
                writer.WriteEndElement();
                writer.WriteStartElement("Variables");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("Offset");
                writer.WriteString("00:00:00");
                writer.WriteEndElement();
                writer.WriteStartElement("AttemptCount");
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteStartElement("AttemptHistory");
                writer.WriteEndElement();
                writer.WriteStartElement("Segments");

                foreach (int item in listBox1.SelectedIndices)
                {
                    writer.WriteStartElement("Segment");
                    writer.WriteStartElement("Name");
                    writer.WriteString(SplitsGenerator.missionNames[item]);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Icon");
                    writer.WriteEndElement();
                    writer.WriteStartElement("SplitTimes");
                    writer.WriteStartElement("SplitTime");
                    writer.WriteAttributeString("name", null, "Personal Best");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteStartElement("BestSegmentTime");
                    writer.WriteEndElement();
                    writer.WriteStartElement("SegmentHistory");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteStartElement("AutoSplitterSettings");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }

        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                var StyleForm = new StyleSwitcher();
                DialogResult result = StyleForm.ShowDialog();
                StyleForm.Dispose();
            }
            catch { }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if ((string)Config.DMC3Path == string.Empty || Config.DMC3Path == null || !Directory.Exists(Config.DMC3Path))
                {
                    MessageBox.Show("Please make sure the path to DMC3 is correct in the Options tab", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (checkBox2.Checked)
                {
                    foreach (string filename in Directory.EnumerateFiles("./inputsthing"))
                    {
                        string modName = filename.Split('\\').Last();
                        string dest = Path.Combine(Config.DMC3Path, modName);
                        File.Copy(filename, dest, true);
                        label9.Text = "Successfully Installed!";
                    }
                } else
                {
                    string dinputSrc = "./styleswitcher/dinput8.dll";
                    string dest = Path.Combine(Config.DMC3Path, "dinput8.dll");
                    File.Copy(dinputSrc, dest, true);
                    label9.Text = "Successfully Uninstalled!";
                }
            }
            catch {
                label9.Text = "Failed To Install!";
            }
        }
    }
}
