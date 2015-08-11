namespace DiffBotMasterControl
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dataGridChannels = new System.Windows.Forms.DataGridView();
			this.Controller = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Cube = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Amoeba = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Appendage = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.listBoxCommands = new System.Windows.Forms.ListBox();
			this.textBoxLog = new System.Windows.Forms.RichTextBox();
			this.buttonExecute = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonControllerPorts = new System.Windows.Forms.Button();
			this.buttonConnectRobots = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.textBoxKey = new System.Windows.Forms.RichTextBox();
			this.timerUpdateLog = new System.Windows.Forms.Timer(this.components);
			this.timerUpdateGrid = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.dataGridChannels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGridChannels
			// 
			this.dataGridChannels.AllowUserToAddRows = false;
			this.dataGridChannels.AllowUserToDeleteRows = false;
			this.dataGridChannels.AllowUserToResizeColumns = false;
			this.dataGridChannels.AllowUserToResizeRows = false;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridChannels.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
			this.dataGridChannels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridChannels.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Controller,
            this.Cube,
            this.Amoeba,
            this.Appendage});
			this.dataGridChannels.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dataGridChannels.Location = new System.Drawing.Point(12, 12);
			this.dataGridChannels.MultiSelect = false;
			this.dataGridChannels.Name = "dataGridChannels";
			this.dataGridChannels.RowHeadersVisible = false;
			this.dataGridChannels.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.dataGridChannels.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridChannels.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridChannels.Size = new System.Drawing.Size(243, 243);
			this.dataGridChannels.TabIndex = 0;
			this.dataGridChannels.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridChannels_CellValidating);
			this.dataGridChannels.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridChannels_ColumnHeaderMouseClick);
			this.dataGridChannels.Leave += new System.EventHandler(this.dataGridChannels_Leave);
			// 
			// Controller
			// 
			this.Controller.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.Transparent;
			dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Transparent;
			this.Controller.DefaultCellStyle = dataGridViewCellStyle6;
			this.Controller.HeaderText = "Controller";
			this.Controller.Name = "Controller";
			this.Controller.ReadOnly = true;
			this.Controller.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Controller.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Controller.Width = 60;
			// 
			// Cube
			// 
			this.Cube.HeaderText = "Cube";
			this.Cube.Name = "Cube";
			this.Cube.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Cube.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Cube.Width = 50;
			// 
			// Amoeba
			// 
			this.Amoeba.HeaderText = "Amoeba";
			this.Amoeba.Name = "Amoeba";
			this.Amoeba.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Amoeba.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Amoeba.Width = 60;
			// 
			// Appendage
			// 
			this.Appendage.HeaderText = "Appendage";
			this.Appendage.Name = "Appendage";
			this.Appendage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Appendage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Appendage.Width = 70;
			// 
			// listBoxCommands
			// 
			this.listBoxCommands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxCommands.FormattingEnabled = true;
			this.listBoxCommands.Location = new System.Drawing.Point(261, 57);
			this.listBoxCommands.Name = "listBoxCommands";
			this.listBoxCommands.Size = new System.Drawing.Size(223, 173);
			this.listBoxCommands.TabIndex = 1;
			this.listBoxCommands.SelectedIndexChanged += new System.EventHandler(this.listBoxCommands_SelectedIndexChanged);
			this.listBoxCommands.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxCommands_MouseDoubleClick);
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxLog.Location = new System.Drawing.Point(12, 274);
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.Size = new System.Drawing.Size(472, 164);
			this.textBoxLog.TabIndex = 2;
			this.textBoxLog.Text = "";
			this.textBoxLog.WordWrap = false;
			// 
			// buttonExecute
			// 
			this.buttonExecute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonExecute.Enabled = false;
			this.buttonExecute.Location = new System.Drawing.Point(260, 232);
			this.buttonExecute.Name = "buttonExecute";
			this.buttonExecute.Size = new System.Drawing.Size(139, 24);
			this.buttonExecute.TabIndex = 3;
			this.buttonExecute.Text = "Execute";
			this.buttonExecute.UseVisualStyleBackColor = true;
			this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 258);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(25, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Log";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(261, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Commands";
			// 
			// buttonControllerPorts
			// 
			this.buttonControllerPorts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonControllerPorts.Location = new System.Drawing.Point(0, 0);
			this.buttonControllerPorts.Name = "buttonControllerPorts";
			this.buttonControllerPorts.Size = new System.Drawing.Size(111, 25);
			this.buttonControllerPorts.TabIndex = 6;
			this.buttonControllerPorts.Text = "Controller Ports";
			this.buttonControllerPorts.UseVisualStyleBackColor = true;
			this.buttonControllerPorts.Click += new System.EventHandler(this.buttonControllerPorts_Click);
			// 
			// buttonConnectRobots
			// 
			this.buttonConnectRobots.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonConnectRobots.Location = new System.Drawing.Point(0, 0);
			this.buttonConnectRobots.Name = "buttonConnectRobots";
			this.buttonConnectRobots.Size = new System.Drawing.Size(110, 25);
			this.buttonConnectRobots.TabIndex = 7;
			this.buttonConnectRobots.Text = "Connect Robots";
			this.buttonConnectRobots.UseVisualStyleBackColor = true;
			this.buttonConnectRobots.Click += new System.EventHandler(this.buttonConnectRobots_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(260, 11);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.buttonConnectRobots);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.buttonControllerPorts);
			this.splitContainer1.Size = new System.Drawing.Size(225, 25);
			this.splitContainer1.SplitterDistance = 110;
			this.splitContainer1.TabIndex = 8;
			// 
			// textBoxKey
			// 
			this.textBoxKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxKey.Cursor = System.Windows.Forms.Cursors.Default;
			this.textBoxKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxKey.Location = new System.Drawing.Point(405, 232);
			this.textBoxKey.Name = "textBoxKey";
			this.textBoxKey.ReadOnly = true;
			this.textBoxKey.Size = new System.Drawing.Size(79, 24);
			this.textBoxKey.TabIndex = 9;
			this.textBoxKey.Text = "[ ]";
			this.textBoxKey.SelectionChanged += new System.EventHandler(this.textBoxKey_SelectionChanged);
			this.textBoxKey.Enter += new System.EventHandler(this.textBoxKey_Enter);
			this.textBoxKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxKey_KeyDown);
			this.textBoxKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxKey_KeyUp);
			this.textBoxKey.Leave += new System.EventHandler(this.textBoxKey_Leave);
			this.textBoxKey.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxKey_HideCaret);
			this.textBoxKey.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textBoxKey_HideCaret);
			// 
			// timerUpdateLog
			// 
			this.timerUpdateLog.Enabled = true;
			this.timerUpdateLog.Interval = 200;
			this.timerUpdateLog.Tick += new System.EventHandler(this.timerUpdateLog_Tick);
			// 
			// timerUpdateGrid
			// 
			this.timerUpdateGrid.Enabled = true;
			this.timerUpdateGrid.Tick += new System.EventHandler(this.timerUpdateGrid_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(496, 450);
			this.Controls.Add(this.textBoxKey);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonExecute);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.listBoxCommands);
			this.Controls.Add(this.dataGridChannels);
			this.Name = "MainForm";
			this.Text = "Robot Runner v007";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.dataGridChannels_Leave);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseClick);
			((System.ComponentModel.ISupportInitialize)(this.dataGridChannels)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridChannels;
		private System.Windows.Forms.ListBox listBoxCommands;
		private System.Windows.Forms.RichTextBox textBoxLog;
		private System.Windows.Forms.Button buttonExecute;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonControllerPorts;
		private System.Windows.Forms.Button buttonConnectRobots;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.RichTextBox textBoxKey;
		private System.Windows.Forms.DataGridViewTextBoxColumn Controller;
		private System.Windows.Forms.DataGridViewTextBoxColumn Cube;
		private System.Windows.Forms.DataGridViewTextBoxColumn Amoeba;
		private System.Windows.Forms.DataGridViewTextBoxColumn Appendage;
		private System.Windows.Forms.Timer timerUpdateLog;
		private System.Windows.Forms.Timer timerUpdateGrid;
	}
}

