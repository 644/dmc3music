using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace dmc3music
{
    public partial class Form1 : Form
    {
        private DMC3MusicConfig Config { get; set; }

        private DMC3Process DMC3 { get; set; }

        private Timer SongChangeTimer { get; set; }

        private Timer SongProgressTimer { get; set; }

        private Timer GameStartTimer { get; set; }

        private SongPlayer Player { get; set; }

        private bool ConfigChanged { get; set; }

        private bool newTrack { get; set; } = true;

        private string outMaxPos { get; set; }

        public Form1()
        {

            InitializeComponent();
            try
            {
                Config = DMC3MusicConfigWriter.ReadConfig();
            }
            catch
            {
                MessageBox.Show("Error", "There was a problem opening the config", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            try
            {

                if (Config.DMC3Path == string.Empty || Config.DMC3Path == null || !Directory.Exists(Config.DMC3Path))
                {
                    getGamePath();
                }
            }
            catch { }

            try
            {
                Player = new SongPlayer(Config);
                SongChangeTimer = new Timer();
                SongProgressTimer = new Timer();
            }
            catch
            {
                Application.Exit();
            }

            try
            {
                ConfigChanged = false;
                volumeSlider1.VolumeChanged += OnVolumeSliderChanged;
                GameStartTimer = new Timer
                {
                    Interval = 50
                };
                GameStartTimer.Tick += new EventHandler(GameStart);
                GameStartTimer.Start();
            }
            catch
            {
                Application.Exit();
            }

            try
            {
                if (!Directory.Exists("tracks"))
                {
                    NotifyUser(5000, "Missing Tracks", "You will need to download the tracks from the otions tab to play music");
                }
            }
            catch { }
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
                NotifyUser(5000, "Error", "There was a problem opening the config");
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

        private void OnVolumeSliderChanged(object sender, EventArgs e)
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
                if (ConfigChanged && !SaveConfigPrompt())
                {
                    e.Cancel = true;
                }
            }
            catch { }
        }

        private void changeShuffle_Click(object sender, EventArgs e)
        {
            try
            {
                ShuffleRotation shuffleForm = new ShuffleRotation(Config);
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
            if (ConfigChanged && !SaveConfigPrompt())
            {
                return;
            }

            DisableConfigControls();

            try
            {
                Player = new SongPlayer(Config);
            }
            catch
            {
                NotifyUser(5000, "Error", "There was a problem loading the Song Player");
            }

            DMC3 = new DMC3Process();
            if (!DMC3.OpenReadOnly())
            {
                DMC3.Dispose();
                NotifyUser(5000, "pls start game first", "dmc3se.exe not found");
                return;
            }

            try
            {
                SongChangeTimer = new Timer
                {
                    Interval = 50
                };
                SongChangeTimer.Tick += new EventHandler(CheckSong);
                SongChangeTimer.Start();

                SongProgressTimer = new Timer
                {
                    Interval = 100
                };
                SongProgressTimer.Tick += new EventHandler(GetSongProgress);
                SongProgressTimer.Start();
            }
            catch { }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                SongChangeTimer.Stop();
                SongProgressTimer.Stop();
                Player.Stop();
                DMC3.Dispose();
                EnableConfigControls();
            }
            catch
            {
                NotifyUser(5000, "Error", "There was a problem stopping the Song Player");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                ControllerConfig ControllerForm = new ControllerConfig();
                DialogResult result = ControllerForm.ShowDialog();
                ControllerForm.Dispose();
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                StyleSwitcher StyleForm = new StyleSwitcher();
                DialogResult result = StyleForm.ShowDialog();
                ConfigChanged = StyleForm.ConfigChanged;
                StyleForm.Dispose();
            }
            catch { }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Config.DMC3Path == string.Empty || Config.DMC3Path == null || !Directory.Exists(Config.DMC3Path))
                {
                    NotifyUser(5000, "Error", "Please make sure the path to DMC3 is correct in the Options tab");
                    return;
                }
                if (checkBox2.Checked)
                {
                    foreach (string filename in Directory.EnumerateFiles("./inputsthing"))
                    {
                        string modName = filename.Split('\\').Last();
                        string dest = Path.Combine(Config.DMC3Path, modName);
                        File.Copy(filename, dest, true);
                        NotifyUser(5000, "DMC3 Inputs Thing", "Successfully Installed!");
                    }
                }
                else
                {
                    string dinputSrc = "./styleswitcher/dinput8.dll";
                    string dest = Path.Combine(Config.DMC3Path, "dinput8.dll");
                    File.Copy(dinputSrc, dest, true);
                    NotifyUser(5000, "DMC3 Inputs Thing", "Successfully Uninstalled!");
                }
            }
            catch
            {
                NotifyUser(5000, "DMC3 Inputs Thing", "Failed To Install!");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                CheckUpdates UpdateForm = new CheckUpdates();
                DialogResult result = UpdateForm.ShowDialog();
                UpdateForm.Dispose();
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                GetMusic MusicForm = new GetMusic();
                DialogResult result = MusicForm.ShowDialog();
                MusicForm.Dispose();
            }
            catch { }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Config.DMC3Path))
            {
                Process.Start("explorer.exe", Config.DMC3Path);
            }
            else
            {
                NotifyUser(5000, "Error", "Couldn't find the path to DMC3. Make sure it's set above.");
            }
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
                    DMC3MusicConfigWriter.WriteConfig(Config);
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string fileName = comboBox1.SelectedItem.ToString();
                string source = Path.Combine("./saves", fileName);
                string destination = Path.Combine(Config.DMC3Path, "save0.sav");

                File.Copy(source, destination, true);
                NotifyUser(5000, "Save Loader", $"Copied '{fileName}' Successfully!");
            }
            catch
            {
                NotifyUser(5000, "Save Loader", "Failed To Copy!");
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
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Livesplit Split File (*.lss)|*.lss"
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

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
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Livesplit Split File (*.lss)|*.lss"
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

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

        #region Functionality Methods

        private void getGamePath()
        {
            string steamPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null);
            string libraryPath = Path.Combine(steamPath, "steamapps/libraryfolders.vdf");
            string[] steamLibraries = File.ReadAllLines(libraryPath);
            string gamePath = "";
            string tmpGamePath = "";

            foreach (string line in steamLibraries)
            {
                Match matchPath = Regex.Match(line, @"""(?<path>\w:\\\\.*)""");
                if (matchPath.Success)
                {
                    tmpGamePath = matchPath.Groups["path"].Value.Replace(@"\\", @"\");
                }
                Match matchGame = Regex.Match(line, @"""(?<6550>\w:\\\\.*)""");
                if (matchGame.Success)
                {
                    gamePath = tmpGamePath;
                }
            }


            gamePath = Path.GetFullPath(Path.Combine(gamePath, "steamapps/common/Devil May Cry 3"));
            if (Directory.Exists(gamePath))
            {
                Config.DMC3Path = gamePath;
                DMC3MusicConfigWriter.WriteConfig(Config);
                MessageBox.Show($"Automatically set the game path to '{gamePath}'. To change this, set the path in the options tab.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void GameStart(object sender, EventArgs e)
        {
            using (DMC3Process DMC3RW = new DMC3Process())
            {
                if (ConfigChanged)
                {
                    ConfigChanged = false;
                    Config = DMC3MusicConfigWriter.ReadConfig();
                }

                if (!DMC3RW.OpenReadWrite())
                {
                    return;
                }

                if (Config.CutsceneMovement)
                {
                    IntPtr cutsceneMovementPtrSS = IntPtr.Add(DMC3RW.BaseAddress, 0x1DFF20);
                    int cutsceneMovementPtrSS2 = DMC3RW.GetIntPtr(cutsceneMovementPtrSS);

                    if (DMC3RW.ReadExactMem(cutsceneMovementPtrSS2 + 0x26) != 2)
                    {
                        return;
                    }

                    DMC3RW.WriteExactMem(new byte[] { 0x00 }, cutsceneMovementPtrSS2 + 0x26);
                    DMC3RW.WriteMem(new byte[] { 0xC7, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0x2C1A6F);
                }
            }

            DMC3 = new DMC3Process();

            if (!DMC3.OpenReadOnly())
            {
                DMC3.Dispose();
                return;
            }

            Player = new SongPlayer(Config);

            try
            {
                GameStartTimer.Stop();
                DisableConfigControls();

                SongChangeTimer = new Timer
                {
                    Interval = 50
                };
                SongChangeTimer.Tick += new EventHandler(CheckSong);
                SongChangeTimer.Start();

                SongProgressTimer = new Timer
                {
                    Interval = 100
                };
                SongProgressTimer.Tick += new EventHandler(GetSongProgress);
                SongProgressTimer.Start();
            }
            catch
            {
                DMC3.Dispose();
                NotifyUser(5000, "Error", "There was a problem opening the DMC3 Process");
            }
        }

        private void GetSongProgress(object sender, EventArgs e)
        {
            if (Player.isPlaying)
            {
                label2.Text = $"Playing: {Player.OldTrack}";
                TimeSpan currentPos = TimeSpan.FromMilliseconds(Player.TrackPos);

                if (newTrack)
                {
                    newTrack = false;
                    TimeSpan maxPos = TimeSpan.FromSeconds(Player.TrackLength);
                    outMaxPos = maxPos.ToString(@"m\:ss\.ff");
                }

                string outCurrentPos;

                if (currentPos.Minutes > 0)
                {
                    outCurrentPos = currentPos.ToString(@"m\:ss\.ff");
                }
                else
                {
                    outCurrentPos = currentPos.ToString(@"ss\.ff");
                }

                label2.Text = $"Playing : {Player.OldTrack} ({outCurrentPos}/{outMaxPos})";
            }
            else
            {
                label2.Text = "Not Playing";
            }
        }

        private void CheckSong(object sender, EventArgs e)
        {
            if (DMC3.ProcHasExited)
            {
                DMC3.Dispose();
                Player.Stop();
                SongChangeTimer.Stop();
                SongProgressTimer.Stop();
                GameStartTimer.Start();
                return;
            }

            try
            {
                int checkRoom = 0;
                checkRoom = DMC3.ReadMem(0x20C39EC);

                if (checkRoom <= 0)
                {
                    if (Player.isPlaying)
                    {
                        Player.FadeOut();
                    }
                    return;
                }

                int roomId = -1;
                int enemyCount = -1;
                int missionNumber = -1;
                int isLoading = -1;
                int vanguardSpawned = -1;

                roomId = DMC3.ReadMem(0x76B150);

                IntPtr tmpPtr = IntPtr.Add(DMC3.BaseAddress, 0x76B860 + 0xC40 + 0x8);
                int enemyCountPtr1 = DMC3.GetIntPtr(tmpPtr);
                tmpPtr = IntPtr.Add(new IntPtr(enemyCountPtr1), 0x18);
                int enemyCountPtr2 = DMC3.GetIntPtr(tmpPtr);
                tmpPtr = IntPtr.Add(new IntPtr(enemyCountPtr2), 0xA78);
                enemyCount = DMC3.GetIntPtr(tmpPtr);

                missionNumber = DMC3.ReadMem(0x76B148);
                isLoading = DMC3.ReadMem(0x205BCB8);

                if (missionNumber == 2)
                {
                    vanguardSpawned = DMC3.ReadMem(0x5585AC);
                    if (vanguardSpawned == 770)
                    {
                        roomId = 66;
                    }
                }

                if (roomId != -1)
                {
                    Player.PlayRoomSong(roomId, enemyCount, missionNumber);
                    newTrack = true;
                }
            }
            catch
            {
                DMC3.Dispose();
                Player.Stop();
                SongChangeTimer.Stop();
                SongProgressTimer.Stop();
                GameStartTimer.Start();
                return;
            }
        }

        private bool SaveConfigPrompt()
        {
            try
            {
                DialogResult result = MessageBox.Show("Would you like to save your current configuration settings?", "Save Configuration", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

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

        private void NotifyUser(int timeout, string tipTitle, string tipText)
        {
            NotifyIcon notifyIcon1 = new NotifyIcon(this.components);
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(timeout, tipTitle, tipText, ToolTipIcon.None);
            notifyIcon1.BalloonTipClosed += (sender, e) => {
                var thisIcon = (NotifyIcon)sender;
                thisIcon.Visible = false;
                thisIcon.Dispose();
            };
        }

        #endregion
    }
}
