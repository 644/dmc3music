using PeanutButter.INI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace dmc3music
{
    public partial class StyleSwitcher : Form
    {
        private DMC3MusicConfig Config { get; set; }
        public bool ConfigChanged { get; set; } = false;
        public string styleWindowed { get; set; } = "0";
        public INIFile styleIni { get; set; }
        public Dictionary<string, string> stylesDict { get; set; } = new Dictionary<string, string>()
        {
            { "Swordmaster", "0" },
            { "Gunslinger", "1" },
            { "Trickster", "2" },
            { "Royalguard", "3" },
            { "Quicksilver", "4" },
            { "Doppelganger", "5" }
        };

        public string styleLoc, styleBGM, styleSEVol, bossRush, hotKeys, blurShader,
            fogShader, shadowEngine, gammaCorrection,
            arcadeMode, arcadeRoom, arcadeMission, arcadeStyle, arcadeWeapons, styleResolution;

        public StyleSwitcher()
        {
            InitializeComponent();
            Config = DMC3MusicConfigWriter.ReadConfig();
            if (Config.DMC3Path == string.Empty || Config.DMC3Path == null || !Directory.Exists(Config.DMC3Path))
            {
                MessageBox.Show("Please make sure the path to DMC3 is correct in the Options tab", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
            }

            styleLoc = Path.Combine(Config.DMC3Path, "StyleSwitcher.ini");
            if (File.Exists(styleLoc))
            {
                styleIni = new INIFile(styleLoc);
            }
            else if (File.Exists("./styleswitcher/StyleSwitcher.ini"))
            {
                styleIni = new INIFile("./styleswitcher/StyleSwitcher.ini");
            }
            else
            {
                MessageBox.Show("Could not load any style switcher ini files", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
            }

            getIniConfig();
        }

        private void getIniConfig()
        {
            checkBox9.Checked = Config.CutsceneMovement;

            styleWindowed = styleIni.GetValue("DISPLAY", "Mode");
            checkBox1.Checked = (styleWindowed == "0") ? true : false;

            styleBGM = styleIni.GetValue("SOUND", "DisableSoundDriver");
            checkBox2.Checked = (styleBGM == "1") ? true : false;

            styleSEVol = styleIni.GetValue("SOUND", "Volume.SE");

            float x = Convert.ToSingle(styleSEVol) / 100f;
            float y;
            if (x <= 0)
            {
                y = 0;
            }
            else
            {
                float dbVolume = (1 - x) * -48f;
                y = (float)Math.Pow(10, dbVolume / 20);
            }

            volumeSlider1.Volume = y;

            bossRush = styleIni.GetValue("GAME", "BossRush");
            checkBox3.Checked = (bossRush == "1") ? true : false;

            arcadeMode = styleIni.GetValue("GAME", "Arcade");
            checkBox4.Checked = (arcadeMode == "1") ? true : false;

            hotKeys = styleIni.GetValue("INPUT", "Hotkeys");
            checkBox5.Checked = (hotKeys == "1") ? true : false;

            blurShader = styleIni.GetValue("DISPLAY", "DisableBlurShader");
            checkBox6.Checked = (blurShader == "1") ? true : false;

            fogShader = styleIni.GetValue("DISPLAY", "DisableFogShader");
            checkBox7.Checked = (fogShader == "1") ? true : false;

            shadowEngine = styleIni.GetValue("DISPLAY", "DisableShadowEngine");
            checkBox8.Checked = (shadowEngine == "1") ? true : false;

            gammaCorrection = styleIni.GetValue("DISPLAY", "GammaCorrection");
            checkBox10.Checked = (gammaCorrection == "1") ? true : false;

            styleResolution = styleIni.GetValue("DISPLAY", "Resolution");
            comboBox1.SelectedIndex = comboBox1.FindString(styleResolution.Replace("@60", ""));

            arcadeMission = styleIni.GetValue("GAME", "Arcade.Mission");
            comboBox3.SelectedIndex = comboBox3.FindString(arcadeMission);

            arcadeStyle = styleIni.GetValue("GAME", "Arcade.Style");
            int styleIndex = 0;
            styleIndex = int.Parse(arcadeStyle);
            comboBox4.SelectedIndex = styleIndex;

            arcadeRoom = styleIni.GetValue("GAME", "Arcade.Room");
            textBox1.Text = arcadeRoom;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DMC3MusicConfigWriter.WriteConfig(Config);

                string styleDll = Path.Combine(Config.DMC3Path, "StyleSwitcher.dll");
                if (!File.Exists(styleDll))
                {
                    foreach (string filePath in Directory.EnumerateFiles("./styleswitcher"))
                    {
                        string fileName = filePath.Split('\\').Last();
                        string dest = Path.Combine(Config.DMC3Path, fileName);
                        File.Copy(filePath, dest, true);
                    }
                    string installLoc = Path.Combine(Config.DMC3Path, "install.bat");
                    System.Diagnostics.Process.Start(installLoc);
                }

                styleIni.WrapValueInQuotes = false;

                styleIni["DISPLAY"]["Mode"] = styleWindowed;
                styleIni["SOUND"]["DisableSoundDriver"] = styleBGM;

                float db = 20 * (float)Math.Log10(volumeSlider1.Volume);
                int percent = (int)Math.Round((1 - (db / -48f)) * 100f);

                if (percent <= 0)
                {
                    styleSEVol = "0";
                }
                else if (percent >= 100)
                {
                    styleSEVol = "100";
                }
                else
                {
                    styleSEVol = percent.ToString();
                }

                styleIni["SOUND"]["Volume.SE"] = styleSEVol;

                styleIni["GAME"]["BossRush"] = bossRush;
                styleIni["GAME"]["Arcade"] = arcadeMode;
                styleIni["INPUT"]["Hotkeys"] = hotKeys;
                styleIni["DISPLAY"]["DisableBlurShader"] = blurShader;
                styleIni["DISPLAY"]["DisableFogShader"] = fogShader;
                styleIni["DISPLAY"]["DisableShadowEngine"] = shadowEngine;
                styleIni["DISPLAY"]["GammaCorrection"] = gammaCorrection;
                styleIni["DISPLAY"]["Resolution"] = styleResolution;
                styleIni["GAME"]["Arcade.Mission"] = arcadeMission;
                styleIni["GAME"]["Arcade.Style"] = arcadeStyle;
                styleIni["GAME"]["Arcade.Room"] = arcadeRoom;

                File.WriteAllText(styleLoc, styleIni.ToString().Replace("BGM[]={", "BGM[] = {"));
                label6.Text = "Installed Successfully!";
                label6.Visible = true;
                ConfigChanged = true;
            }
            catch
            {
                label6.Text = "Failed To Install!";
                label6.Visible = true;
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            Config.CutsceneMovement = checkBox9.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            styleWindowed = (checkBox1.Checked) ? "0" : "1";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            styleBGM = (checkBox2.Checked) ? "1" : "0";
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Arcade Mission
            arcadeMission = comboBox3.SelectedItem.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Arcade Weapons
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Arcade Style
            if (stylesDict.TryGetValue(comboBox4.SelectedItem.ToString(), out string tmpStyle))
            {
                arcadeStyle = tmpStyle;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Resolution
            styleResolution = comboBox1.SelectedItem.ToString() + "@60";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Arcade Room
            arcadeRoom = textBox1.Text;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            // Arcade mode
            arcadeMode = (checkBox4.Checked) ? "1" : "0";
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            // Gamma
            gammaCorrection = (checkBox10.Checked) ? "1" : "0";
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            // Shadow Engine
            shadowEngine = (checkBox8.Checked) ? "1" : "0";
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            // Fog Shader
            fogShader = (checkBox7.Checked) ? "1" : "0";
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            // Blur Shader
            blurShader = (checkBox6.Checked) ? "1" : "0";
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            // Hotkeys
            hotKeys = (checkBox5.Checked) ? "1" : "0";
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            // Boss Rush
            bossRush = (checkBox3.Checked) ? "1" : "0";
        }
    }
}
