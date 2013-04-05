namespace Sharparam.SwitchBladeSteam.App
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
			this.UpButton = new System.Windows.Forms.Button();
			this.OfflineButton = new System.Windows.Forms.Button();
			this.OnlineButton = new System.Windows.Forms.Button();
			this.FriendsButton = new System.Windows.Forms.Button();
			this.HomeButton = new System.Windows.Forms.Button();
			this.DownButton = new System.Windows.Forms.Button();
			this.ChatButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// UpButton
			// 
			this.UpButton.FlatAppearance.BorderSize = 0;
			this.UpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.UpButton.Image = global::Sharparam.SwitchBladeSteam.App.Properties.Resources.dk_up;
			this.UpButton.Location = new System.Drawing.Point(12, 254);
			this.UpButton.Name = "UpButton";
			this.UpButton.Size = new System.Drawing.Size(115, 115);
			this.UpButton.TabIndex = 4;
			this.UpButton.UseVisualStyleBackColor = true;
			this.UpButton.Click += new System.EventHandler(this.UpButtonClick);
			// 
			// OfflineButton
			// 
			this.OfflineButton.FlatAppearance.BorderSize = 0;
			this.OfflineButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.OfflineButton.Image = global::Sharparam.SwitchBladeSteam.App.Properties.Resources.dk_appear_offline;
			this.OfflineButton.Location = new System.Drawing.Point(133, 133);
			this.OfflineButton.Name = "OfflineButton";
			this.OfflineButton.Size = new System.Drawing.Size(115, 115);
			this.OfflineButton.TabIndex = 3;
			this.OfflineButton.UseVisualStyleBackColor = true;
			this.OfflineButton.Click += new System.EventHandler(this.OfflineButtonClick);
			// 
			// OnlineButton
			// 
			this.OnlineButton.FlatAppearance.BorderSize = 0;
			this.OnlineButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.OnlineButton.Image = global::Sharparam.SwitchBladeSteam.App.Properties.Resources.dk_appear_online;
			this.OnlineButton.Location = new System.Drawing.Point(12, 133);
			this.OnlineButton.Name = "OnlineButton";
			this.OnlineButton.Size = new System.Drawing.Size(115, 115);
			this.OnlineButton.TabIndex = 2;
			this.OnlineButton.UseVisualStyleBackColor = true;
			this.OnlineButton.Click += new System.EventHandler(this.OnlineButtonClick);
			// 
			// FriendsButton
			// 
			this.FriendsButton.FlatAppearance.BorderSize = 0;
			this.FriendsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.FriendsButton.Image = global::Sharparam.SwitchBladeSteam.App.Properties.Resources.dk_friends;
			this.FriendsButton.Location = new System.Drawing.Point(133, 12);
			this.FriendsButton.Name = "FriendsButton";
			this.FriendsButton.Size = new System.Drawing.Size(115, 115);
			this.FriendsButton.TabIndex = 1;
			this.FriendsButton.UseVisualStyleBackColor = true;
			this.FriendsButton.Click += new System.EventHandler(this.FriendsButtonClick);
			// 
			// HomeButton
			// 
			this.HomeButton.FlatAppearance.BorderSize = 0;
			this.HomeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.HomeButton.Image = global::Sharparam.SwitchBladeSteam.App.Properties.Resources.dk_home;
			this.HomeButton.Location = new System.Drawing.Point(12, 12);
			this.HomeButton.Name = "HomeButton";
			this.HomeButton.Size = new System.Drawing.Size(115, 115);
			this.HomeButton.TabIndex = 0;
			this.HomeButton.UseVisualStyleBackColor = true;
			this.HomeButton.Click += new System.EventHandler(this.HomeButtonClick);
			// 
			// DownButton
			// 
			this.DownButton.FlatAppearance.BorderSize = 0;
			this.DownButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DownButton.Image = global::Sharparam.SwitchBladeSteam.App.Properties.Resources.dk_down;
			this.DownButton.Location = new System.Drawing.Point(133, 254);
			this.DownButton.Name = "DownButton";
			this.DownButton.Size = new System.Drawing.Size(115, 115);
			this.DownButton.TabIndex = 5;
			this.DownButton.UseVisualStyleBackColor = true;
			this.DownButton.Click += new System.EventHandler(this.DownButtonClick);
			// 
			// ChatButton
			// 
			this.ChatButton.FlatAppearance.BorderSize = 0;
			this.ChatButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ChatButton.Image = global::Sharparam.SwitchBladeSteam.App.Properties.Resources.dk_chat;
			this.ChatButton.Location = new System.Drawing.Point(12, 375);
			this.ChatButton.Name = "ChatButton";
			this.ChatButton.Size = new System.Drawing.Size(115, 115);
			this.ChatButton.TabIndex = 6;
			this.ChatButton.UseVisualStyleBackColor = true;
			this.ChatButton.Click += new System.EventHandler(this.ChatButtonClick);
			// 
			// KeyDebugWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(262, 534);
			this.ControlBox = false;
			this.Controls.Add(this.ChatButton);
			this.Controls.Add(this.DownButton);
			this.Controls.Add(this.UpButton);
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
		private System.Windows.Forms.Button UpButton;
		private System.Windows.Forms.Button DownButton;
		private System.Windows.Forms.Button ChatButton;
	}
}