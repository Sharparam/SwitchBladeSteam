namespace F16Gaming.SwitchBladeSteam.App
{
	partial class ChatWindow
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
			this.ChatTitle = new System.Windows.Forms.Label();
			this.EntryBox = new System.Windows.Forms.TextBox();
			this.ChatHistory = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// ChatTitle
			// 
			this.ChatTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.ChatTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ChatTitle.ForeColor = System.Drawing.Color.DimGray;
			this.ChatTitle.Location = new System.Drawing.Point(0, 0);
			this.ChatTitle.Name = "ChatTitle";
			this.ChatTitle.Size = new System.Drawing.Size(800, 20);
			this.ChatTitle.TabIndex = 0;
			this.ChatTitle.Text = "Chatting with $(FRIEND_NAME)";
			this.ChatTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// EntryBox
			// 
			this.EntryBox.BackColor = System.Drawing.Color.Black;
			this.EntryBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.EntryBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.EntryBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EntryBox.ForeColor = System.Drawing.Color.Gray;
			this.EntryBox.Location = new System.Drawing.Point(0, 400);
			this.EntryBox.Multiline = true;
			this.EntryBox.Name = "EntryBox";
			this.EntryBox.Size = new System.Drawing.Size(800, 80);
			this.EntryBox.TabIndex = 1;
			this.EntryBox.TextChanged += new System.EventHandler(this.EntryBoxTextChanged);
			this.EntryBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EntryBoxKeyDown);
			// 
			// ChatHistory
			// 
			this.ChatHistory.BackColor = System.Drawing.Color.Black;
			this.ChatHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ChatHistory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChatHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ChatHistory.ForeColor = System.Drawing.Color.DarkGray;
			this.ChatHistory.Location = new System.Drawing.Point(0, 20);
			this.ChatHistory.Name = "ChatHistory";
			this.ChatHistory.ReadOnly = true;
			this.ChatHistory.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.ChatHistory.Size = new System.Drawing.Size(800, 380);
			this.ChatHistory.TabIndex = 2;
			this.ChatHistory.Text = "";
			// 
			// ChatWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImage = global::F16Gaming.SwitchBladeSteam.App.Properties.Resources.tp_splash;
			this.ClientSize = new System.Drawing.Size(800, 480);
			this.Controls.Add(this.ChatHistory);
			this.Controls.Add(this.EntryBox);
			this.Controls.Add(this.ChatTitle);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChatWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ChatWindow";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChatWindowFormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label ChatTitle;
		private System.Windows.Forms.TextBox EntryBox;
		private System.Windows.Forms.RichTextBox ChatHistory;
	}
}