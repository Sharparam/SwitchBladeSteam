namespace F16Gaming.SwitchBladeSteam.App
{
	partial class MainWindow
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
			this.DebugExitButton = new System.Windows.Forms.Button();
			this.ErrorLabel = new System.Windows.Forms.Label();
			this.ShowStatsButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// DebugExitButton
			// 
			this.DebugExitButton.ForeColor = System.Drawing.Color.Black;
			this.DebugExitButton.Location = new System.Drawing.Point(364, 135);
			this.DebugExitButton.Name = "DebugExitButton";
			this.DebugExitButton.Size = new System.Drawing.Size(102, 49);
			this.DebugExitButton.TabIndex = 0;
			this.DebugExitButton.Text = "EXIT";
			this.DebugExitButton.UseVisualStyleBackColor = true;
			this.DebugExitButton.Click += new System.EventHandler(this.DebugExitButtonClick);
			// 
			// ErrorLabel
			// 
			this.ErrorLabel.AutoEllipsis = true;
			this.ErrorLabel.BackColor = System.Drawing.Color.Transparent;
			this.ErrorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ErrorLabel.ForeColor = System.Drawing.Color.Red;
			this.ErrorLabel.Location = new System.Drawing.Point(12, 286);
			this.ErrorLabel.Name = "ErrorLabel";
			this.ErrorLabel.Size = new System.Drawing.Size(267, 115);
			this.ErrorLabel.TabIndex = 1;
			this.ErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ShowStatsButton
			// 
			this.ShowStatsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ShowStatsButton.ForeColor = System.Drawing.Color.Black;
			this.ShowStatsButton.Location = new System.Drawing.Point(251, 40);
			this.ShowStatsButton.Name = "ShowStatsButton";
			this.ShowStatsButton.Size = new System.Drawing.Size(276, 68);
			this.ShowStatsButton.TabIndex = 2;
			this.ShowStatsButton.Text = "SHOW RENDER STATS";
			this.ShowStatsButton.UseVisualStyleBackColor = true;
			this.ShowStatsButton.Click += new System.EventHandler(this.ShowStatsButtonClick);
			// 
			// MainWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImage = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.tp_splash;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(800, 480);
			this.ControlBox = false;
			this.Controls.Add(this.ShowStatsButton);
			this.Controls.Add(this.ErrorLabel);
			this.Controls.Add(this.DebugExitButton);
			this.DoubleBuffered = true;
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SwitchBlade Steam App";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button DebugExitButton;
		private System.Windows.Forms.Label ErrorLabel;
		private System.Windows.Forms.Button ShowStatsButton;

	}
}

