using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.AzureCat.Samples.AlertClient
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.mainHeaderPanel = new Microsoft.AzureCat.Samples.AlertClient.HeaderPanel();
            this.btnClear = new System.Windows.Forms.Button();
            this.grouperEventHub = new Microsoft.AzureCat.Samples.AlertClient.Grouper();
            this.cboEventHub = new System.Windows.Forms.ComboBox();
            this.lblStorageAccountConnectionString = new System.Windows.Forms.Label();
            this.txtStorageAccountConnectionString = new System.Windows.Forms.TextBox();
            this.lblEventHub = new System.Windows.Forms.Label();
            this.lblConsumerGroup = new System.Windows.Forms.Label();
            this.txtConsumerGroup = new System.Windows.Forms.TextBox();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.txtServiceBusConnectionString = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.logHeaderPanel = new Microsoft.AzureCat.Samples.AlertClient.HeaderPanel();
            this.logPanel = new System.Windows.Forms.Panel();
            this.logTabControl = new System.Windows.Forms.TabControl();
            this.alertTabPage = new System.Windows.Forms.TabPage();
            this.alertDataGridView = new System.Windows.Forms.DataGridView();
            this.logTabPage = new System.Windows.Forms.TabPage();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.alertBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.mainHeaderPanel.SuspendLayout();
            this.grouperEventHub.SuspendLayout();
            this.logHeaderPanel.SuspendLayout();
            this.logPanel.SuspendLayout();
            this.logTabControl.SuspendLayout();
            this.alertTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertDataGridView)).BeginInit();
            this.logTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(896, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearLogToolStripMenuItem,
            this.saveLogToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // clearLogToolStripMenuItem
            // 
            this.clearLogToolStripMenuItem.Name = "clearLogToolStripMenuItem";
            this.clearLogToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.clearLogToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.clearLogToolStripMenuItem.Text = "Clear Log";
            this.clearLogToolStripMenuItem.Click += new System.EventHandler(this.clearLogToolStripMenuItem_Click);
            // 
            // saveLogToolStripMenuItem
            // 
            this.saveLogToolStripMenuItem.Name = "saveLogToolStripMenuItem";
            this.saveLogToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveLogToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveLogToolStripMenuItem.Text = "Save Log As...";
            this.saveLogToolStripMenuItem.Click += new System.EventHandler(this.saveLogToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logWindowToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // logWindowToolStripMenuItem
            // 
            this.logWindowToolStripMenuItem.Checked = true;
            this.logWindowToolStripMenuItem.CheckOnClick = true;
            this.logWindowToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logWindowToolStripMenuItem.Name = "logWindowToolStripMenuItem";
            this.logWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.logWindowToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.logWindowToolStripMenuItem.Text = "&Log Window";
            this.logWindowToolStripMenuItem.Click += new System.EventHandler(this.logWindowToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 739);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(896, 22);
            this.statusStrip.TabIndex = 20;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(16, 40);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.mainHeaderPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.logHeaderPanel);
            this.splitContainer.Size = new System.Drawing.Size(864, 696);
            this.splitContainer.SplitterDistance = 272;
            this.splitContainer.SplitterWidth = 8;
            this.splitContainer.TabIndex = 21;
            // 
            // mainHeaderPanel
            // 
            this.mainHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.mainHeaderPanel.Controls.Add(this.btnClear);
            this.mainHeaderPanel.Controls.Add(this.grouperEventHub);
            this.mainHeaderPanel.Controls.Add(this.btnStart);
            this.mainHeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainHeaderPanel.ForeColor = System.Drawing.Color.White;
            this.mainHeaderPanel.HeaderColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(205)))), ((int)(((byte)(219)))));
            this.mainHeaderPanel.HeaderColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.mainHeaderPanel.HeaderFont = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.mainHeaderPanel.HeaderHeight = 24;
            this.mainHeaderPanel.HeaderText = "Event Hub Configuration";
            this.mainHeaderPanel.Icon = global::Microsoft.AzureCat.Samples.AlertClient.Properties.Resources.SmallWorld;
            this.mainHeaderPanel.IconTransparentColor = System.Drawing.Color.White;
            this.mainHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.mainHeaderPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.mainHeaderPanel.Name = "mainHeaderPanel";
            this.mainHeaderPanel.Padding = new System.Windows.Forms.Padding(5, 28, 5, 4);
            this.mainHeaderPanel.Size = new System.Drawing.Size(864, 272);
            this.mainHeaderPanel.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btnClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClear.Location = new System.Drawing.Point(656, 232);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(88, 24);
            this.btnClear.TabIndex = 159;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // grouperEventHub
            // 
            this.grouperEventHub.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grouperEventHub.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.grouperEventHub.BackgroundGradientColor = System.Drawing.Color.White;
            this.grouperEventHub.BackgroundGradientMode = Microsoft.AzureCat.Samples.AlertClient.Grouper.GroupBoxGradientMode.None;
            this.grouperEventHub.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.grouperEventHub.BorderThickness = 1F;
            this.grouperEventHub.Controls.Add(this.cboEventHub);
            this.grouperEventHub.Controls.Add(this.lblStorageAccountConnectionString);
            this.grouperEventHub.Controls.Add(this.txtStorageAccountConnectionString);
            this.grouperEventHub.Controls.Add(this.lblEventHub);
            this.grouperEventHub.Controls.Add(this.lblConsumerGroup);
            this.grouperEventHub.Controls.Add(this.txtConsumerGroup);
            this.grouperEventHub.Controls.Add(this.lblConnectionString);
            this.grouperEventHub.Controls.Add(this.txtServiceBusConnectionString);
            this.grouperEventHub.CustomGroupBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.grouperEventHub.GroupImage = null;
            this.grouperEventHub.GroupTitle = "Event Hub";
            this.grouperEventHub.Location = new System.Drawing.Point(16, 32);
            this.grouperEventHub.Name = "grouperEventHub";
            this.grouperEventHub.Padding = new System.Windows.Forms.Padding(20);
            this.grouperEventHub.PaintGroupBox = true;
            this.grouperEventHub.RoundCorners = 4;
            this.grouperEventHub.ShadowColor = System.Drawing.Color.DarkGray;
            this.grouperEventHub.ShadowControl = false;
            this.grouperEventHub.ShadowThickness = 3;
            this.grouperEventHub.Size = new System.Drawing.Size(832, 187);
            this.grouperEventHub.TabIndex = 158;
            this.grouperEventHub.CustomPaint += new System.Action<System.Windows.Forms.PaintEventArgs>(this.grouperEventHub_CustomPaint);
            // 
            // cboEventHub
            // 
            this.cboEventHub.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEventHub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboEventHub.FormattingEnabled = true;
            this.cboEventHub.Location = new System.Drawing.Point(16, 148);
            this.cboEventHub.Name = "cboEventHub";
            this.cboEventHub.Size = new System.Drawing.Size(392, 21);
            this.cboEventHub.TabIndex = 92;
            // 
            // lblStorageAccountConnectionString
            // 
            this.lblStorageAccountConnectionString.AutoSize = true;
            this.lblStorageAccountConnectionString.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStorageAccountConnectionString.Location = new System.Drawing.Point(16, 32);
            this.lblStorageAccountConnectionString.Name = "lblStorageAccountConnectionString";
            this.lblStorageAccountConnectionString.Size = new System.Drawing.Size(177, 13);
            this.lblStorageAccountConnectionString.TabIndex = 61;
            this.lblStorageAccountConnectionString.Text = "Storage Account Connection String:";
            // 
            // txtStorageAccountConnectionString
            // 
            this.txtStorageAccountConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStorageAccountConnectionString.Location = new System.Drawing.Point(16, 48);
            this.txtStorageAccountConnectionString.Name = "txtStorageAccountConnectionString";
            this.txtStorageAccountConnectionString.Size = new System.Drawing.Size(800, 20);
            this.txtStorageAccountConnectionString.TabIndex = 59;
            // 
            // lblEventHub
            // 
            this.lblEventHub.AutoSize = true;
            this.lblEventHub.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblEventHub.Location = new System.Drawing.Point(16, 132);
            this.lblEventHub.Name = "lblEventHub";
            this.lblEventHub.Size = new System.Drawing.Size(61, 13);
            this.lblEventHub.TabIndex = 58;
            this.lblEventHub.Text = "Event Hub:";
            // 
            // lblConsumerGroup
            // 
            this.lblConsumerGroup.AutoSize = true;
            this.lblConsumerGroup.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConsumerGroup.Location = new System.Drawing.Point(424, 132);
            this.lblConsumerGroup.Name = "lblConsumerGroup";
            this.lblConsumerGroup.Size = new System.Drawing.Size(89, 13);
            this.lblConsumerGroup.TabIndex = 57;
            this.lblConsumerGroup.Text = "Consumer Group:";
            // 
            // txtConsumerGroup
            // 
            this.txtConsumerGroup.Location = new System.Drawing.Point(424, 148);
            this.txtConsumerGroup.Name = "txtConsumerGroup";
            this.txtConsumerGroup.Size = new System.Drawing.Size(392, 20);
            this.txtConsumerGroup.TabIndex = 56;
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConnectionString.Location = new System.Drawing.Point(16, 82);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(154, 13);
            this.lblConnectionString.TabIndex = 54;
            this.lblConnectionString.Text = "Service Bus Connection String:";
            // 
            // txtServiceBusConnectionString
            // 
            this.txtServiceBusConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServiceBusConnectionString.Location = new System.Drawing.Point(16, 100);
            this.txtServiceBusConnectionString.Name = "txtServiceBusConnectionString";
            this.txtServiceBusConnectionString.Size = new System.Drawing.Size(800, 20);
            this.txtServiceBusConnectionString.TabIndex = 48;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.btnStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btnStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btnStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStart.Location = new System.Drawing.Point(760, 232);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(88, 24);
            this.btnStart.TabIndex = 48;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            this.btnStart.MouseEnter += new System.EventHandler(this.button_MouseEnter);
            this.btnStart.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // logHeaderPanel
            // 
            this.logHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.logHeaderPanel.Controls.Add(this.logPanel);
            this.logHeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logHeaderPanel.ForeColor = System.Drawing.Color.White;
            this.logHeaderPanel.HeaderColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(205)))), ((int)(((byte)(219)))));
            this.logHeaderPanel.HeaderColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.logHeaderPanel.HeaderFont = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.logHeaderPanel.HeaderHeight = 24;
            this.logHeaderPanel.HeaderText = "Log";
            this.logHeaderPanel.Icon = global::Microsoft.AzureCat.Samples.AlertClient.Properties.Resources.SmallDocument;
            this.logHeaderPanel.IconTransparentColor = System.Drawing.Color.White;
            this.logHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.logHeaderPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.logHeaderPanel.Name = "logHeaderPanel";
            this.logHeaderPanel.Padding = new System.Windows.Forms.Padding(5, 28, 5, 4);
            this.logHeaderPanel.Size = new System.Drawing.Size(864, 416);
            this.logHeaderPanel.TabIndex = 0;
            // 
            // logPanel
            // 
            this.logPanel.Controls.Add(this.logTabControl);
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.ForeColor = System.Drawing.Color.Black;
            this.logPanel.Location = new System.Drawing.Point(5, 28);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(854, 384);
            this.logPanel.TabIndex = 0;
            // 
            // logTabControl
            // 
            this.logTabControl.Controls.Add(this.alertTabPage);
            this.logTabControl.Controls.Add(this.logTabPage);
            this.logTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.logTabControl.Location = new System.Drawing.Point(0, 0);
            this.logTabControl.Name = "logTabControl";
            this.logTabControl.SelectedIndex = 0;
            this.logTabControl.Size = new System.Drawing.Size(854, 384);
            this.logTabControl.TabIndex = 2;
            this.logTabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.logTabControl_DrawItem);
            // 
            // alertTabPage
            // 
            this.alertTabPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.alertTabPage.Controls.Add(this.alertDataGridView);
            this.alertTabPage.Location = new System.Drawing.Point(4, 22);
            this.alertTabPage.Name = "alertTabPage";
            this.alertTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.alertTabPage.Size = new System.Drawing.Size(846, 358);
            this.alertTabPage.TabIndex = 0;
            this.alertTabPage.Text = "Alerts";
            // 
            // alertDataGridView
            // 
            this.alertDataGridView.AllowUserToAddRows = false;
            this.alertDataGridView.AllowUserToDeleteRows = false;
            this.alertDataGridView.AllowUserToResizeRows = false;
            this.alertDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.alertDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.alertDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.alertDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.alertDataGridView.Location = new System.Drawing.Point(3, 3);
            this.alertDataGridView.Name = "alertDataGridView";
            this.alertDataGridView.ReadOnly = true;
            this.alertDataGridView.RowHeadersWidth = 24;
            this.alertDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.alertDataGridView.ShowCellErrors = false;
            this.alertDataGridView.ShowRowErrors = false;
            this.alertDataGridView.Size = new System.Drawing.Size(840, 352);
            this.alertDataGridView.TabIndex = 1;
            this.alertDataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView_RowsAdded);
            this.alertDataGridView.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView_RowsRemoved);
            this.alertDataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // logTabPage
            // 
            this.logTabPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.logTabPage.Controls.Add(this.lstLog);
            this.logTabPage.Location = new System.Drawing.Point(4, 22);
            this.logTabPage.Name = "logTabPage";
            this.logTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.logTabPage.Size = new System.Drawing.Size(846, 358);
            this.logTabPage.TabIndex = 1;
            this.logTabPage.Text = "Log";
            // 
            // lstLog
            // 
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(3, 3);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(840, 352);
            this.lstLog.TabIndex = 1;
            this.lstLog.Leave += new System.EventHandler(this.lstLog_Leave);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Warning.png");
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(228)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(896, 761);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alert Client";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.mainHeaderPanel.ResumeLayout(false);
            this.grouperEventHub.ResumeLayout(false);
            this.grouperEventHub.PerformLayout();
            this.logHeaderPanel.ResumeLayout(false);
            this.logPanel.ResumeLayout(false);
            this.logTabControl.ResumeLayout(false);
            this.alertTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertDataGridView)).EndInit();
            this.logTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem clearLogToolStripMenuItem;
        private ToolStripMenuItem saveLogToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem logWindowToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        private SplitContainer splitContainer;
        private HeaderPanel logHeaderPanel;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
        private HeaderPanel mainHeaderPanel;
        private Button btnStart;
        private ImageList imageList;
        private Panel logPanel;
        private TabControl logTabControl;
        private TabPage alertTabPage;
        private TabPage logTabPage;
        private ListBox lstLog;
        private DataGridView alertDataGridView;
        private BindingSource alertBindingSource;
        private Grouper grouperEventHub;
        private Label lblConnectionString;
        private TextBox txtServiceBusConnectionString;
        private TextBox txtConsumerGroup;
        private Label lblConsumerGroup;
        private TextBox txtStorageAccountConnectionString;
        private Label lblEventHub;
        private Label lblStorageAccountConnectionString;
        private Button btnClear;
        private ComboBox cboEventHub;
    }
}