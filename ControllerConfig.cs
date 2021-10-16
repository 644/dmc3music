using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace dmc3music
{
    public partial class ControllerConfig : Form
    {
        private DMC3MusicConfig Config { get; set; }
        public DirectInput directInput = new DirectInput();
        private Timer ControllerTimer { get; set; }
        public Joystick joystick { get; set; }
        public Guid joystickGuid { get; set; } = Guid.Empty;
        public string joystickName { get; set; }
        public Dictionary<string, Guid> controllerDict { get; set; } = new Dictionary<string, Guid>();
        public Dictionary<string, int> ControllerKeysMap = new Dictionary<string, int>()
        {
            { "Start", 7 },
            { "Select", 6 },
            { "Circle", 1 },
            { "Triangle", 3 },
            { "Square", 2 },
            { "Cross", 0 },
            { "L1", 4 },
            { "R1", 5 },
            { "L2", 255 },
            { "R2", 255 },
            { "L3", 8 },
            { "R3", 9 },
            { "L<->R", 0 }
        };
        public Dictionary<string, int> ButtonsAsNums = new Dictionary<string, int>()
        {
            { "Buttons0", 0 },
            { "Buttons1", 1 },
            { "Buttons2", 2 },
            { "Buttons3", 3 },
            { "Buttons4", 4 },
            { "Buttons5", 5 },
            { "Buttons6", 6 },
            { "Buttons7", 7 },
            { "Buttons8", 8 },
            { "Buttons9", 9 },
            { "Buttons10", 10 },
            { "Buttons11", 11 },
            { "Buttons12", 12 },
            { "Buttons13", 13 },
            { "Buttons14", 14 },
            { "Buttons15", 15 },
            { "Buttons16", 16 },
        };
        public PictureBox picClear { get; set; }
        public PictureBox picClicked { get; set; }
        public string currentKey { get; set; } = "Start";
        public Form InputForm { get; set; }

        public ControllerConfig()
        {
            InitializeComponent();
            Config = DMC3MusicConfigWriter.ReadConfig();
            if (Config.DMC3Path == string.Empty || Config.DMC3Path == null || !Directory.Exists(Config.DMC3Path))
            {
                MessageBox.Show("Please make sure the path to DMC3 is correct in the Options tab", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
            }
            IList<DeviceInstance> controllers = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices);
            foreach (DeviceInstance ctrl in controllers)
            {
                comboBox1.Items.Add(ctrl.InstanceName);
                controllerDict.Add(ctrl.InstanceName, ctrl.InstanceGuid);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string contents = $"[{joystickName}]\r\n";
                foreach (KeyValuePair<string, int> kp in ControllerKeysMap)
                {
                    contents += $"{kp.Key}={kp.Value}\r\n";
                }
                string dest = Path.Combine(Config.DMC3Path, "DMC3SE.ini");
                File.WriteAllText(dest, contents);
                MessageBox.Show($"Successfully written the controller config to {dest}", "Controller Config", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            joystickName = comboBox1.SelectedItem.ToString();

            if (!controllerDict.TryGetValue(joystickName, out Guid tmpGuid))
            {
                return;
            }

            joystickGuid = tmpGuid;
            joystick = new Joystick(directInput, joystickGuid);

            IList<EffectInfo> allEffects = joystick.GetEffects();
            foreach (EffectInfo effectInfo in allEffects)
            {
                Console.WriteLine("Effect available {0}", effectInfo.Name);
            }

            joystick.Properties.BufferSize = 128;
            joystick.Acquire();
        }

        private void PollController(object sender, EventArgs e)
        {
            joystick.Poll();
            JoystickUpdate[] datas = joystick.GetBufferedData();
            foreach (JoystickUpdate state in datas)
            {
                string button = state.Offset.ToString();
                if (button.Contains("Buttons") && !InputForm.IsDisposed)
                {
                    Console.WriteLine(state);
                    if (ButtonsAsNums.TryGetValue(button, out int b))
                    {
                        ControllerKeysMap[currentKey] = b;
                    }
                    InputForm.Dispose();
                    picClear.Visible = true;
                    picClicked.Visible = false;
                }
            }
        }

        private void BeginPoll(string displayKey)
        {
            if (joystickGuid == Guid.Empty)
            {
                MessageBox.Show("Please select a controller from the dropdown menu", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            refreshBoxes();
            picClear.Visible = false;
            picClicked.Visible = true;
            if (Application.OpenForms.OfType<MessageForm>().Count() > 0)
            {
                InputForm.Dispose();
            }
            InputForm = new MessageForm(displayKey);
            InputForm.Show(this);
            ControllerTimer = new Timer
            {
                Interval = 10
            };
            ControllerTimer.Tick -= new EventHandler(PollController);
            ControllerTimer.Tick += new EventHandler(PollController);
            ControllerTimer.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            picClear = pictureBox1;
            picClicked = pictureBox24;
            currentKey = "Triangle";
            BeginPoll("Triangle/Y");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            picClear = pictureBox2;
            picClicked = pictureBox22;
            currentKey = "Square";
            BeginPoll("Square/X");
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            picClear = pictureBox3;
            picClicked = pictureBox14;
            currentKey = "Cross";
            BeginPoll("Cross/A");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            picClear = pictureBox4;
            picClicked = pictureBox13;
            currentKey = "Circle";
            BeginPoll("Circle/B");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            picClear = pictureBox5;
            picClicked = pictureBox23;
            currentKey = "Start";
            BeginPoll("Options/Start");
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            picClear = pictureBox6;
            picClicked = pictureBox21;
            currentKey = "Select";
            BeginPoll("Select/Back");
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            picClear = pictureBox12;
            picClicked = pictureBox17;
            currentKey = "L3";
            BeginPoll("L3/Left Stick");
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            picClear = pictureBox11;
            picClicked = pictureBox20;
            currentKey = "R3";
            BeginPoll("R3/Right Stick");
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            picClear = pictureBox9;
            picClicked = pictureBox18;
            currentKey = "R1";
            BeginPoll("R1/RB");
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            picClear = pictureBox10;
            picClicked = pictureBox19;
            currentKey = "R2";
            BeginPoll("R2/RT");
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            picClear = pictureBox7;
            picClicked = pictureBox15;
            currentKey = "L1";
            BeginPoll("L1/LB");
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            picClear = pictureBox8;
            picClicked = pictureBox16;
            currentKey = "L2";
            BeginPoll("L2/LT");
        }

        private void refreshBoxes()
        {
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox5.Visible = true;
            pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            pictureBox8.Visible = true;
            pictureBox9.Visible = true;
            pictureBox10.Visible = true;
            pictureBox11.Visible = true;
            pictureBox12.Visible = true;
            pictureBox13.Visible = false;
            pictureBox14.Visible = false;
            pictureBox15.Visible = false;
            pictureBox16.Visible = false;
            pictureBox17.Visible = false;
            pictureBox18.Visible = false;
            pictureBox19.Visible = false;
            pictureBox20.Visible = false;
            pictureBox21.Visible = false;
            pictureBox22.Visible = false;
            pictureBox23.Visible = false;
            pictureBox24.Visible = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                ControllerKeysMap["L2"] = 255;
                ControllerKeysMap["R2"] = 255;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                ControllerKeysMap["L<->R"] = 1;
            }
            else
            {
                ControllerKeysMap["L<->R"] = 0;
            }
        }
    }
}
