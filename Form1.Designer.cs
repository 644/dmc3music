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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.startMusic = new System.Windows.Forms.Button();
            this.stopMusic = new System.Windows.Forms.Button();
            this.changeShuffle = new System.Windows.Forms.Button();
            this.shuffleCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // startMusic
            // 
            this.startMusic.Location = new System.Drawing.Point(85, 98);
            this.startMusic.Name = "startMusic";
            this.startMusic.Size = new System.Drawing.Size(75, 23);
            this.startMusic.TabIndex = 0;
            this.startMusic.Text = "Start Music";
            this.startMusic.UseVisualStyleBackColor = true;
            this.startMusic.Click += new System.EventHandler(this.startMusic_Click);
            // 
            // stopMusic
            // 
            this.stopMusic.Location = new System.Drawing.Point(85, 142);
            this.stopMusic.Name = "stopMusic";
            this.stopMusic.Size = new System.Drawing.Size(75, 23);
            this.stopMusic.TabIndex = 1;
            this.stopMusic.Text = "Stop Music";
            this.stopMusic.UseVisualStyleBackColor = true;
            this.stopMusic.Click += new System.EventHandler(this.stopMusic_Click);
            // 
            // changeShuffle
            // 
            this.changeShuffle.Location = new System.Drawing.Point(77, 8);
            this.changeShuffle.Name = "changeShuffle";
            this.changeShuffle.Size = new System.Drawing.Size(148, 23);
            this.changeShuffle.TabIndex = 2;
            this.changeShuffle.Text = "Change Shuffle Rotation";
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
            this.shuffleCheckBox.UseVisualStyleBackColor = true;
            this.shuffleCheckBox.CheckedChanged += new System.EventHandler(this.shuffleCheckBox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 177);
            this.Controls.Add(this.shuffleCheckBox);
            this.Controls.Add(this.changeShuffle);
            this.Controls.Add(this.stopMusic);
            this.Controls.Add(this.startMusic);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "DMC3 Music";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startMusic;
        private System.Windows.Forms.Button stopMusic;
        private System.Windows.Forms.Button changeShuffle;
        private System.Windows.Forms.CheckBox shuffleCheckBox;
    }
}

