using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace dmc3music
{
    public partial class ShuffleRotation : Form
    {
        public DMC3MusicConfig Config { get; set; }

        public ShuffleRotation(DMC3MusicConfig config)
        {
            InitializeComponent();
            Config = config;
        }

        private void ShuffleRotation_Load(object sender, EventArgs e)
        {
            List<string> tracks = TrackReader.ReadTracks(Config.MusicPath, new string[] { "*.ogg" });
            HashSet<string> shuffleRotationSet = Config.ShuffleRotation.ToHashSet();
            foreach (string track in tracks)
            {
                if (shuffleRotationSet.Contains(track))
                {
                    tracksShuffling.Items.Add(track);
                }
                else
                {
                    tracksRemaining.Items.Add(track);
                }
            }
        }

        private void addTrack_Click(object sender, EventArgs e)
        {
            tracksShuffling.Items.Add(tracksRemaining.SelectedItem);
            tracksRemaining.Items.Remove(tracksRemaining.SelectedItem);
        }

        private void removeTrack_Click(object sender, EventArgs e)
        {
            if (tracksShuffling.Items.Count == 1)
            {
                MessageBox.Show(
                    "At least one track has to be in the rotation!",
                    "Leave One Track",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            tracksRemaining.Items.Add(tracksShuffling.SelectedItem);
            tracksShuffling.Items.Remove(tracksShuffling.SelectedItem);
        }

        private void confirm_Click(object sender, EventArgs e)
        {
            List<string> newShuffleRotation = tracksShuffling.Items.Cast<string>().ToList();
            Config.ShuffleRotation = newShuffleRotation;
        }
    }
}
