namespace F16Gaming.SwitchBladeSteam.App
{
	partial class FriendsWindow
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
			this.FriendsLabel = new System.Windows.Forms.Label();
			this.HelpLabel = new System.Windows.Forms.Label();
			this.FriendList = new System.Windows.Forms.ListView();
			this.NameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.StatusHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// FriendsLabel
			// 
			this.FriendsLabel.BackColor = System.Drawing.Color.Transparent;
			this.FriendsLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.FriendsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FriendsLabel.ForeColor = System.Drawing.Color.Gray;
			this.FriendsLabel.Location = new System.Drawing.Point(0, 0);
			this.FriendsLabel.Name = "FriendsLabel";
			this.FriendsLabel.Size = new System.Drawing.Size(800, 39);
			this.FriendsLabel.TabIndex = 0;
			this.FriendsLabel.Text = "Friends";
			this.FriendsLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// HelpLabel
			// 
			this.HelpLabel.BackColor = System.Drawing.Color.Transparent;
			this.HelpLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.HelpLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.HelpLabel.Location = new System.Drawing.Point(0, 39);
			this.HelpLabel.Name = "HelpLabel";
			this.HelpLabel.Size = new System.Drawing.Size(800, 24);
			this.HelpLabel.TabIndex = 1;
			this.HelpLabel.Text = "Click on a friend to start a chat with them";
			this.HelpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FriendList
			// 
			this.FriendList.BackColor = System.Drawing.Color.Black;
			this.FriendList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.FriendList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameHeader,
            this.StatusHeader});
			this.FriendList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FriendList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FriendList.ForeColor = System.Drawing.Color.White;
			this.FriendList.FullRowSelect = true;
			this.FriendList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.FriendList.Location = new System.Drawing.Point(0, 63);
			this.FriendList.MultiSelect = false;
			this.FriendList.Name = "FriendList";
			this.FriendList.Size = new System.Drawing.Size(800, 417);
			this.FriendList.TabIndex = 2;
			this.FriendList.UseCompatibleStateImageBehavior = false;
			this.FriendList.View = System.Windows.Forms.View.Details;
			this.FriendList.SelectedIndexChanged += new System.EventHandler(this.FriendListSelectedIndexChanged);
			// 
			// NameHeader
			// 
			this.NameHeader.Text = "Name";
			this.NameHeader.Width = 300;
			// 
			// StatusHeader
			// 
			this.StatusHeader.Text = "Status";
			this.StatusHeader.Width = 500;
			// 
			// FriendsWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImage = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.tp_splash;
			this.ClientSize = new System.Drawing.Size(800, 480);
			this.Controls.Add(this.FriendList);
			this.Controls.Add(this.HelpLabel);
			this.Controls.Add(this.FriendsLabel);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FriendsWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SwitchBlade Steam App - Friends";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FriendsWindowFormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label FriendsLabel;
		private System.Windows.Forms.Label HelpLabel;
		private System.Windows.Forms.ListView FriendList;
		private System.Windows.Forms.ColumnHeader NameHeader;
		private System.Windows.Forms.ColumnHeader StatusHeader;
	}
}