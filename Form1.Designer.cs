namespace dmc3music
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.changeShuffle = new System.Windows.Forms.Button();
            this.shuffleCheckBox = new System.Windows.Forms.CheckBox();
            this.volumeSlider1 = new NAudio.Gui.VolumeSlider();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip4 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip5 = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // changeShuffle
            // 
            this.changeShuffle.Location = new System.Drawing.Point(77, 8);
            this.changeShuffle.Name = "changeShuffle";
            this.changeShuffle.Size = new System.Drawing.Size(148, 23);
            this.changeShuffle.TabIndex = 2;
            this.changeShuffle.Text = "Change Shuffle Rotation";
            this.toolTip4.SetToolTip(this.changeShuffle, "edit the tracks that play in the randomizer");
            this.changeShuffle.UseVisualStyleBackColor = true;
            this.changeShuffle.Click += new System.EventHandler(this.changeShuffle_Click);
            // 
            // shuffleCheckBox
            // 
            this.shuffleCheckBox.AutoSize = true;
            this.shuffleCheckBox.Location = new System.Drawing.Point(12, 12);
            this.shuffleCheckBox.Name = "shuffleCheckBox";
            this.shuffleCheckBox.Size = new System.Drawing.Size(59, 17);
            this.shuffleCheckBox.TabIndex = 3;
            this.shuffleCheckBox.Text = "Shuffle";
            this.toolTip5.SetToolTip(this.shuffleCheckBox, "play random songs instead of the correct ones");
            this.shuffleCheckBox.UseVisualStyleBackColor = true;
            this.shuffleCheckBox.CheckedChanged += new System.EventHandler(this.shuffleCheckBox_CheckedChanged);
            // 
            // volumeSlider1
            // 
            this.volumeSlider1.Location = new System.Drawing.Point(12, 116);
            this.volumeSlider1.Name = "volumeSlider1";
            this.volumeSlider1.Size = new System.Drawing.Size(213, 13);
            this.volumeSlider1.TabIndex = 6;
            this.toolTip1.SetToolTip(this.volumeSlider1, "adjust the volume");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(95, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Volume";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Not Playing";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::dmc3music.Properties.Resources.icons8_play_30;
            this.pictureBox2.Location = new System.Drawing.Point(127, 53);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(39, 31);
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::dmc3music.Properties.Resources.icons8_pause_30;
            this.pictureBox1.Location = new System.Drawing.Point(77, 53);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(31, 31);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 185);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.volumeSlider1);
            this.Controls.Add(this.shuffleCheckBox);
            this.Controls.Add(this.changeShuffle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "DMC3 Music";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button changeShuffle;
        private System.Windows.Forms.CheckBox shuffleCheckBox;
        private NAudio.Gui.VolumeSlider volumeSlider1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip3;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip4;
        private System.Windows.Forms.ToolTip toolTip5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

