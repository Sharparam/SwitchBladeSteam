namespace F16Gaming.SwitchBladeSteam.App
{
	partial class KeyDebugWindow
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
			this.NextButton = new System.Windows.Forms.Button();
			this.PrevButton = new System.Windows.Forms.Button();
			this.OfflineButton = new System.Windows.Forms.Button();
			this.OnlineButton = new System.Windows.Forms.Button();
			this.FriendsButton = new System.Windows.Forms.Button();
			this.HomeButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// NextButton
			// 
			this.NextButton.FlatAppearance.BorderSize = 0;
			this.NextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NextButton.Image = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.dk_next_chat;
			this.NextButton.Location = new System.Drawing.Point(133, 254);
			this.NextButton.Name = "NextButton";
			this.NextButton.Size = new System.Drawing.Size(115, 115);
			this.NextButton.TabIndex = 5;
			this.NextButton.UseVisualStyleBackColor = true;
			// 
			// PrevButton
			// 
			this.PrevButton.FlatAppearance.BorderSize = 0;
			this.PrevButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PrevButton.Image = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.dk_prev_chat;
			this.PrevButton.Location = new System.Drawing.Point(12, 254);
			this.PrevButton.Name = "PrevButton";
			this.PrevButton.Size = new System.Drawing.Size(115, 115);
			this.PrevButton.TabIndex = 4;
			this.PrevButton.UseVisualStyleBackColor = true;
			// 
			// OfflineButton
			// 
			this.OfflineButton.FlatAppearance.BorderSize = 0;
			this.OfflineButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.OfflineButton.Image = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.dk_appear_offline;
			this.OfflineButton.Location = new System.Drawing.Point(133, 133);
			this.OfflineButton.Name = "OfflineButton";
			this.OfflineButton.Size = new System.Drawing.Size(115, 115);
			this.OfflineButton.TabIndex = 3;
			this.OfflineButton.UseVisualStyleBackColor = true;
			// 
			// OnlineButton
			// 
			this.OnlineButton.FlatAppearance.BorderSize = 0;
			this.OnlineButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.OnlineButton.Image = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.dk_appear_online;
			this.OnlineButton.Location = new System.Drawing.Point(12, 133);
			this.OnlineButton.Name = "OnlineButton";
			this.OnlineButton.Size = new System.Drawing.Size(115, 115);
			this.OnlineButton.TabIndex = 2;
			this.OnlineButton.UseVisualStyleBackColor = true;
			// 
			// FriendsButton
			// 
			this.FriendsButton.FlatAppearance.BorderSize = 0;
			this.FriendsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.FriendsButton.Image = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.dk_friends;
			this.FriendsButton.Location = new System.Drawing.Point(133, 12);
			this.FriendsButton.Name = "FriendsButton";
			this.FriendsButton.Size = new System.Drawing.Size(115, 115);
			this.FriendsButton.TabIndex = 1;
			this.FriendsButton.UseVisualStyleBackColor = true;
			this.FriendsButton.Click += new System.EventHandler(this.FriendsButton_Click);
			// 
			// HomeButton
			// 
			this.HomeButton.FlatAppearance.BorderSize = 0;
			this.HomeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.HomeButton.Image = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.dk_home;
			this.HomeButton.Location = new System.Drawing.Point(12, 12);
			this.HomeButton.Name = "HomeButton";
			this.HomeButton.Size = new System.Drawing.Size(115, 115);
			this.HomeButton.TabIndex = 0;
			this.HomeButton.UseVisualStyleBackColor = true;
			this.HomeButton.Click += new System.EventHandler(this.HomeButton_Click);
			// 
			// KeyDebugWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(262, 383);
			this.ControlBox = false;
			this.Controls.Add(this.NextButton);
			this.Controls.Add(this.PrevButton);
			this.Controls.Add(this.OfflineButton);
			this.Controls.Add(this.OnlineButton);
			this.Controls.Add(this.FriendsButton);
			this.Controls.Add(this.HomeButton);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "KeyDebugWindow";
			this.Text = "Dynamic Key Debugging";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button HomeButton;
		private System.Windows.Forms.Button FriendsButton;
		private System.Windows.Forms.Button OnlineButton;
		private System.Windows.Forms.Button OfflineButton;
		private System.Windows.Forms.Button PrevButton;
		private System.Windows.Forms.Button NextButton;
	}
}