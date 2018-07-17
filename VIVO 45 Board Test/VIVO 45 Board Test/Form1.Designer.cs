namespace VIVO_45_Board_Test
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonStartSelected = new System.Windows.Forms.Button();
            this.buttonStartAll = new System.Windows.Forms.Button();
            this.linkLabelClearTests = new System.Windows.Forms.LinkLabel();
            this.buttonStop = new System.Windows.Forms.Button();
            this.checkedListBoxTests = new System.Windows.Forms.CheckedListBox();
            this.linkLabelSelectAll = new System.Windows.Forms.LinkLabel();
            this.textBoxSaveDirectory = new System.Windows.Forms.TextBox();
            this.textBoxSerialNumber = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelSelectDirectory = new System.Windows.Forms.Label();
            this.labelEnterSN = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonTest = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.scanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputPlaintextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.generateSubfolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editFixtureSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startWiFiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanForHWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.TestName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TestRes = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStartSelected
            // 
            this.buttonStartSelected.Location = new System.Drawing.Point(491, 532);
            this.buttonStartSelected.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonStartSelected.Name = "buttonStartSelected";
            this.buttonStartSelected.Size = new System.Drawing.Size(213, 79);
            this.buttonStartSelected.TabIndex = 0;
            this.buttonStartSelected.Text = "Start Selected Tests";
            this.buttonStartSelected.UseVisualStyleBackColor = true;
            this.buttonStartSelected.Click += new System.EventHandler(this.buttonStartSelected_Click);
            // 
            // buttonStartAll
            // 
            this.buttonStartAll.Location = new System.Drawing.Point(779, 532);
            this.buttonStartAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonStartAll.Name = "buttonStartAll";
            this.buttonStartAll.Size = new System.Drawing.Size(213, 79);
            this.buttonStartAll.TabIndex = 1;
            this.buttonStartAll.Text = "Start All Tests";
            this.buttonStartAll.UseVisualStyleBackColor = true;
            this.buttonStartAll.Click += new System.EventHandler(this.buttonStartAll_Click);
            // 
            // linkLabelClearTests
            // 
            this.linkLabelClearTests.AutoSize = true;
            this.linkLabelClearTests.Location = new System.Drawing.Point(491, 492);
            this.linkLabelClearTests.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabelClearTests.Name = "linkLabelClearTests";
            this.linkLabelClearTests.Size = new System.Drawing.Size(132, 17);
            this.linkLabelClearTests.TabIndex = 3;
            this.linkLabelClearTests.TabStop = true;
            this.linkLabelClearTests.Text = "Clear selected tests";
            this.linkLabelClearTests.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClearTests_LinkClicked);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(629, 532);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(213, 79);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "Cancel";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Visible = false;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // checkedListBoxTests
            // 
            this.checkedListBoxTests.CheckOnClick = true;
            this.checkedListBoxTests.FormattingEnabled = true;
            this.checkedListBoxTests.Location = new System.Drawing.Point(491, 89);
            this.checkedListBoxTests.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkedListBoxTests.Name = "checkedListBoxTests";
            this.checkedListBoxTests.ScrollAlwaysVisible = true;
            this.checkedListBoxTests.Size = new System.Drawing.Size(500, 378);
            this.checkedListBoxTests.TabIndex = 5;
            // 
            // linkLabelSelectAll
            // 
            this.linkLabelSelectAll.AutoSize = true;
            this.linkLabelSelectAll.Location = new System.Drawing.Point(885, 492);
            this.linkLabelSelectAll.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabelSelectAll.Name = "linkLabelSelectAll";
            this.linkLabelSelectAll.Size = new System.Drawing.Size(99, 17);
            this.linkLabelSelectAll.TabIndex = 6;
            this.linkLabelSelectAll.TabStop = true;
            this.linkLabelSelectAll.Text = "Select all tests";
            this.linkLabelSelectAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSelectAll_LinkClicked);
            // 
            // textBoxSaveDirectory
            // 
            this.textBoxSaveDirectory.Location = new System.Drawing.Point(43, 187);
            this.textBoxSaveDirectory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxSaveDirectory.Name = "textBoxSaveDirectory";
            this.textBoxSaveDirectory.ReadOnly = true;
            this.textBoxSaveDirectory.Size = new System.Drawing.Size(340, 22);
            this.textBoxSaveDirectory.TabIndex = 7;
            // 
            // textBoxSerialNumber
            // 
            this.textBoxSerialNumber.Location = new System.Drawing.Point(43, 325);
            this.textBoxSerialNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxSerialNumber.Name = "textBoxSerialNumber";
            this.textBoxSerialNumber.Size = new System.Drawing.Size(340, 22);
            this.textBoxSerialNumber.TabIndex = 8;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 664);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1045, 26);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 10;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(133, 20);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 21);
            // 
            // labelSelectDirectory
            // 
            this.labelSelectDirectory.AutoSize = true;
            this.labelSelectDirectory.Location = new System.Drawing.Point(43, 167);
            this.labelSelectDirectory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSelectDirectory.Name = "labelSelectDirectory";
            this.labelSelectDirectory.Size = new System.Drawing.Size(154, 17);
            this.labelSelectDirectory.TabIndex = 11;
            this.labelSelectDirectory.Text = "Report output directory";
            // 
            // labelEnterSN
            // 
            this.labelEnterSN.AutoSize = true;
            this.labelEnterSN.Location = new System.Drawing.Point(43, 305);
            this.labelEnterSN.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelEnterSN.Name = "labelEnterSN";
            this.labelEnterSN.Size = new System.Drawing.Size(214, 17);
            this.labelEnterSN.TabIndex = 12;
            this.labelEnterSN.Text = "Enter or scan PCB serial number";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(43, 219);
            this.buttonBrowse.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(96, 30);
            this.buttonBrowse.TabIndex = 13;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(85, 443);
            this.buttonTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(128, 59);
            this.buttonTest.TabIndex = 14;
            this.buttonTest.Text = "Dev Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Visible = false;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1045, 28);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // scanToolStripMenuItem
            // 
            this.scanToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportOptionsToolStripMenuItem,
            this.editFixtureSetupToolStripMenuItem,
            this.startWiFiToolStripMenuItem,
            this.scanForHWToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            this.scanToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.scanToolStripMenuItem.Text = "Options";
            // 
            // reportOptionsToolStripMenuItem
            // 
            this.reportOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outputXMLToolStripMenuItem,
            this.outputJSONToolStripMenuItem,
            this.outputPlaintextToolStripMenuItem,
            this.toolStripSeparator1,
            this.generateSubfolderToolStripMenuItem});
            this.reportOptionsToolStripMenuItem.Name = "reportOptionsToolStripMenuItem";
            this.reportOptionsToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.reportOptionsToolStripMenuItem.Text = "Report options";
            // 
            // outputXMLToolStripMenuItem
            // 
            this.outputXMLToolStripMenuItem.Checked = true;
            this.outputXMLToolStripMenuItem.CheckOnClick = true;
            this.outputXMLToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputXMLToolStripMenuItem.Name = "outputXMLToolStripMenuItem";
            this.outputXMLToolStripMenuItem.Size = new System.Drawing.Size(211, 26);
            this.outputXMLToolStripMenuItem.Text = "Output XML";
            this.outputXMLToolStripMenuItem.Click += new System.EventHandler(this.outputXMLToolStripMenuItem_Click);
            // 
            // outputJSONToolStripMenuItem
            // 
            this.outputJSONToolStripMenuItem.Checked = true;
            this.outputJSONToolStripMenuItem.CheckOnClick = true;
            this.outputJSONToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputJSONToolStripMenuItem.Name = "outputJSONToolStripMenuItem";
            this.outputJSONToolStripMenuItem.Size = new System.Drawing.Size(211, 26);
            this.outputJSONToolStripMenuItem.Text = "Output JSON";
            this.outputJSONToolStripMenuItem.Click += new System.EventHandler(this.outputJSONToolStripMenuItem_Click);
            // 
            // outputPlaintextToolStripMenuItem
            // 
            this.outputPlaintextToolStripMenuItem.Checked = true;
            this.outputPlaintextToolStripMenuItem.CheckOnClick = true;
            this.outputPlaintextToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputPlaintextToolStripMenuItem.Name = "outputPlaintextToolStripMenuItem";
            this.outputPlaintextToolStripMenuItem.Size = new System.Drawing.Size(211, 26);
            this.outputPlaintextToolStripMenuItem.Text = "Output plaintext";
            this.outputPlaintextToolStripMenuItem.Click += new System.EventHandler(this.outputPlaintextToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(208, 6);
            // 
            // generateSubfolderToolStripMenuItem
            // 
            this.generateSubfolderToolStripMenuItem.CheckOnClick = true;
            this.generateSubfolderToolStripMenuItem.Name = "generateSubfolderToolStripMenuItem";
            this.generateSubfolderToolStripMenuItem.Size = new System.Drawing.Size(211, 26);
            this.generateSubfolderToolStripMenuItem.Text = "Generate subfolder";
            this.generateSubfolderToolStripMenuItem.Click += new System.EventHandler(this.generateSubfolderToolStripMenuItem_Click);
            // 
            // editFixtureSetupToolStripMenuItem
            // 
            this.editFixtureSetupToolStripMenuItem.Name = "editFixtureSetupToolStripMenuItem";
            this.editFixtureSetupToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.editFixtureSetupToolStripMenuItem.Text = "Edit fixture setup";
            this.editFixtureSetupToolStripMenuItem.Click += new System.EventHandler(this.editFixtureSetupToolStripMenuItem_Click);
            // 
            // startWiFiToolStripMenuItem
            // 
            this.startWiFiToolStripMenuItem.Name = "startWiFiToolStripMenuItem";
            this.startWiFiToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.startWiFiToolStripMenuItem.Text = "Autoconfig WiFi";
            this.startWiFiToolStripMenuItem.Click += new System.EventHandler(this.startWiFiToolStripMenuItem_Click);
            // 
            // scanForHWToolStripMenuItem
            // 
            this.scanForHWToolStripMenuItem.Name = "scanForHWToolStripMenuItem";
            this.scanForHWToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.scanForHWToolStripMenuItem.Text = "Scan for HW";
            this.scanForHWToolStripMenuItem.Click += new System.EventHandler(this.scanForHWToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.label1.Location = new System.Drawing.Point(17, 532);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 36);
            this.label1.TabIndex = 17;
            this.label1.Text = "Test:";
            // 
            // TestName
            // 
            this.TestName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.TestName.Location = new System.Drawing.Point(115, 524);
            this.TestName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TestName.Name = "TestName";
            this.TestName.ReadOnly = true;
            this.TestName.Size = new System.Drawing.Size(340, 41);
            this.TestName.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.label2.Location = new System.Drawing.Point(17, 590);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 36);
            this.label2.TabIndex = 19;
            this.label2.Text = "Result:";
            // 
            // TestRes
            // 
            this.TestRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.TestRes.Location = new System.Drawing.Point(129, 585);
            this.TestRes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TestRes.Name = "TestRes";
            this.TestRes.ReadOnly = true;
            this.TestRes.Size = new System.Drawing.Size(109, 41);
            this.TestRes.TabIndex = 20;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(47, 59);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(240, 58);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 690);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TestRes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TestName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.labelEnterSN);
            this.Controls.Add(this.labelSelectDirectory);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.textBoxSerialNumber);
            this.Controls.Add(this.textBoxSaveDirectory);
            this.Controls.Add(this.linkLabelSelectAll);
            this.Controls.Add(this.checkedListBoxTests);
            this.Controls.Add(this.linkLabelClearTests);
            this.Controls.Add(this.buttonStartAll);
            this.Controls.Add(this.buttonStartSelected);
            this.Controls.Add(this.buttonStop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "VIVO45 Board Test v1.2";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStartSelected;
        private System.Windows.Forms.Button buttonStartAll;
        private System.Windows.Forms.LinkLabel linkLabelClearTests;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.CheckedListBox checkedListBoxTests;
        private System.Windows.Forms.LinkLabel linkLabelSelectAll;
        private System.Windows.Forms.TextBox textBoxSaveDirectory;
        private System.Windows.Forms.TextBox textBoxSerialNumber;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Label labelSelectDirectory;
        private System.Windows.Forms.Label labelEnterSN;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem scanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editFixtureSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanForHWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputPlaintextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem generateSubfolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startWiFiToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TestName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TestRes;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

