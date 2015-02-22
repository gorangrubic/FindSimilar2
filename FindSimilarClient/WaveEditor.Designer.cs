﻿namespace FindSimilar2
{
	partial class WaveEditor
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private CommonUtils.GUI.CustomWaveViewer customWaveViewer1;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnPause;
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.TextBox txtFilePath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.customWaveViewer1 = new CommonUtils.GUI.CustomWaveViewer();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnPause = new System.Windows.Forms.Button();
			this.btnPlay = new System.Windows.Forms.Button();
			this.txtFilePath = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// customWaveViewer1
			// 
			this.customWaveViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.customWaveViewer1.Location = new System.Drawing.Point(12, 12);
			this.customWaveViewer1.Name = "customWaveViewer1";
			this.customWaveViewer1.Size = new System.Drawing.Size(630, 265);
			this.customWaveViewer1.TabIndex = 0;
			// 
			// btnStop
			// 
			this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStop.Location = new System.Drawing.Point(512, 3);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 9;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.BtnStopClick);
			// 
			// btnPause
			// 
			this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPause.Location = new System.Drawing.Point(431, 3);
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new System.Drawing.Size(75, 23);
			this.btnPause.TabIndex = 8;
			this.btnPause.Text = "Pause";
			this.btnPause.UseVisualStyleBackColor = true;
			this.btnPause.Click += new System.EventHandler(this.BtnPauseClick);
			// 
			// btnPlay
			// 
			this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPlay.Location = new System.Drawing.Point(350, 3);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(75, 23);
			this.btnPlay.TabIndex = 7;
			this.btnPlay.Text = "Play";
			this.btnPlay.UseVisualStyleBackColor = true;
			this.btnPlay.Click += new System.EventHandler(this.BtnPlayClick);
			// 
			// txtFilePath
			// 
			this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtFilePath.Location = new System.Drawing.Point(3, 6);
			this.txtFilePath.Name = "txtFilePath";
			this.txtFilePath.Size = new System.Drawing.Size(260, 20);
			this.txtFilePath.TabIndex = 6;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnBrowse.Location = new System.Drawing.Point(269, 3);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 5;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.BtnBrowseClick);
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog1";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.txtFilePath);
			this.flowLayoutPanel1.Controls.Add(this.btnBrowse);
			this.flowLayoutPanel1.Controls.Add(this.btnPlay);
			this.flowLayoutPanel1.Controls.Add(this.btnPause);
			this.flowLayoutPanel1.Controls.Add(this.btnStop);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 283);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(654, 32);
			this.flowLayoutPanel1.TabIndex = 10;
			// 
			// WaveEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(654, 315);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.customWaveViewer1);
			this.KeyPreview = true;
			this.Name = "WaveEditor";
			this.Text = "Wave Editor";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}
	}
}