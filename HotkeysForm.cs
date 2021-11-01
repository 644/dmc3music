using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dmc3music
{
    public partial class HotkeysForm : Form
    {
        public HotkeysForm()
        {
            InitializeComponent();
        }

        private bool modSet { get; set; } = false;

        public Dictionary<Keys, bool> modKeysStartGame { get; set; } = new Dictionary<Keys, bool>()
        {
            { Keys.Control, false },
            { Keys.Shift, false },
            { Keys.Alt, false }
        };

        private void SetText(string text)
        {
            if(textBox1.Text.Length == 0)
            {
                textBox1.Text = text;
            } else
            {
                textBox1.Text += $"+ {text}";
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                return;
            }
            if (textBox1.Text.Contains(e.KeyCode.ToString())){
                return;
            }

            else if((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (textBox1.Text.Contains(Keys.Control.ToString()))
                {
                    return;
                }
                modSet = true;
                modKeysStartGame[Keys.Control] = true;
                SetText(Keys.Control.ToString());
            }

            else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                if (textBox1.Text.Contains(Keys.Alt.ToString()))
                {
                    return;
                }
                modSet = true;
                SetText(Keys.Alt.ToString());
            }

            else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (textBox1.Text.Contains(Keys.Shift.ToString()))
                {
                    return;
                }
                modSet = true;
                SetText(Keys.Shift.ToString());
            }
            else
            {
                SetText(e.KeyCode.ToString());
            }
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            modSet = false;
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
