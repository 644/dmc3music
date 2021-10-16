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
