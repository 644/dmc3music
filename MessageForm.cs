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
    public partial class MessageForm : Form
    {
        public MessageForm(string inputKey)
        {
            InitializeComponent();
            label1.Text = $"Press the {inputKey} button on your controller";
        }
    }
}
