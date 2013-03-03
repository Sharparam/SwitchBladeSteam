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
			this.FriendList = new System.Windows.Forms.ListView();
			this.NameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.StatusHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// FriendList
			// 
			this.FriendList.BackColor = System.Drawing.Color.Black;
			this.FriendList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.FriendList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameHeader,
            this.StatusHeader});
			this.FriendList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FriendList.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FriendList.ForeColor = System.Drawing.Color.White;
			this.FriendList.FullRowSelect = true;
			this.FriendList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.FriendList.HideSelection = false;
			this.FriendList.Location = new System.Drawing.Point(0, 0);
			this.FriendList.MultiSelect = false;
			this.FriendList.Name = "FriendList";
			this.FriendList.Size = new System.Drawing.Size(800, 480);
			this.FriendList.TabIndex = 2;
			this.FriendList.UseCompatibleStateImageBehavior = false;
			this.FriendList.View = System.Windows.Forms.View.Details;
			this.FriendList.SelectedIndexChanged += new System.EventHandler(this.FriendListSelectedIndexChanged);
			this.FriendList.DoubleClick += new System.EventHandler(this.FriendListDoubleClick);
			this.FriendList.Enter += new System.EventHandler(this.FriendListEnter);
			// 
			// NameHeader
			// 
			this.NameHeader.Text = "Name";
			this.NameHeader.Width = 460;
			// 
			// StatusHeader
			// 
			this.StatusHeader.Text = "Status";
			this.StatusHeader.Width = 340;
			// 
			// FriendsWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(800, 480);
			this.Controls.Add(this.FriendList);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FriendsWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SwitchBlade Steam App - Friends";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FriendsWindowFormClosing);
			this.Load += new System.EventHandler(this.FriendsWindowLoad);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView FriendList;
		private System.Windows.Forms.ColumnHeader NameHeader;
		private System.Windows.Forms.ColumnHeader StatusHeader;
	}
}