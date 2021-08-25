
namespace dmc3music
{
    partial class ShuffleRotation
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
            this.tracksRemaining = new System.Windows.Forms.ListBox();
            this.tracksShuffling = new System.Windows.Forms.ListBox();
            this.confirm = new System.Windows.Forms.Button();
            this.addTrack = new System.Windows.Forms.Button();
            this.removeTrack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tracksRemaining
            // 
            this.tracksRemaining.FormattingEnabled = true;
            this.tracksRemaining.Location = new System.Drawing.Point(12, 12);
            this.tracksRemaining.Name = "tracksRemaining";
            this.tracksRemaining.Size = new System.Drawing.Size(170, 147);
            this.tracksRemaining.TabIndex = 0;
            // 
            // tracksShuffling
            // 
            this.tracksShuffling.FormattingEnabled = true;
            this.tracksShuffling.Location = new System.Drawing.Point(310, 12);
            this.tracksShuffling.Name = "tracksShuffling";
            this.tracksShuffling.Size = new System.Drawing.Size(170, 147);
            this.tracksShuffling.TabIndex = 1;
            // 
            // confirm
            // 
            this.confirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.confirm.Location = new System.Drawing.Point(209, 175);
            this.confirm.Name = "confirm";
            this.confirm.Size = new System.Drawing.Size(75, 23);
            this.confirm.TabIndex = 2;
            this.confirm.Text = "OK";
            this.confirm.UseVisualStyleBackColor = true;
            this.confirm.Click += new System.EventHandler(this.confirm_Click);
            // 
            // addTrack
            // 
            this.addTrack.Location = new System.Drawing.Point(209, 44);
            this.addTrack.Name = "addTrack";
            this.addTrack.Size = new System.Drawing.Size(75, 23);
            this.addTrack.TabIndex = 3;
            this.addTrack.Text = ">";
            this.addTrack.UseVisualStyleBackColor = true;
            this.addTrack.Click += new System.EventHandler(this.addTrack_Click);
            // 
            // removeTrack
            // 
            this.removeTrack.Location = new System.Drawing.Point(209, 101);
            this.removeTrack.Name = "removeTrack";
            this.removeTrack.Size = new System.Drawing.Size(75, 23);
            this.removeTrack.TabIndex = 4;
            this.removeTrack.Text = "<";
            this.removeTrack.UseVisualStyleBackColor = true;
            this.removeTrack.Click += new System.EventHandler(this.removeTrack_Click);
            // 
            // ShuffleRotation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 210);
            this.Controls.Add(this.removeTrack);
            this.Controls.Add(this.addTrack);
            this.Controls.Add(this.confirm);
            this.Controls.Add(this.tracksShuffling);
            this.Controls.Add(this.tracksRemaining);
            this.Name = "ShuffleRotation";
            this.Text = "ShuffleRotation";
            this.Load += new System.EventHandler(this.ShuffleRotation_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox tracksRemaining;
        private System.Windows.Forms.ListBox tracksShuffling;
        private System.Windows.Forms.Button confirm;
        private System.Windows.Forms.Button addTrack;
        private System.Windows.Forms.Button removeTrack;
    }
}