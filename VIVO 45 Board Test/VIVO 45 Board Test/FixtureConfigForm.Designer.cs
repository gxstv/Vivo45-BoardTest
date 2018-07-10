namespace VIVO_45_Board_Test
{
    partial class FixtureConfigForm
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
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxPowerSource = new System.Windows.Forms.GroupBox();
            this.comboBoxIntBatt = new System.Windows.Forms.ComboBox();
            this.comboBoxExtBatt = new System.Windows.Forms.ComboBox();
            this.comboBoxExtDC = new System.Windows.Forms.ComboBox();
            this.comboBoxMainsDC = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxScope = new System.Windows.Forms.GroupBox();
            this.comboBoxSupercapReturnScope = new System.Windows.Forms.ComboBox();
            this.comboBoxSupercapMidScope = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxSupercapHiScope = new System.Windows.Forms.ComboBox();
            this.comboBoxSpeakerScope2 = new System.Windows.Forms.ComboBox();
            this.comboBoxSpeakerScope1 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxMainScope = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxSecondaryOut = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxPrimaryOut = new System.Windows.Forms.ComboBox();
            this.groupBoxPowerSource.SuspendLayout();
            this.groupBoxScope.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(320, 376);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(96, 40);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save changes";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(464, 376);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 40);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBoxPowerSource
            // 
            this.groupBoxPowerSource.Controls.Add(this.comboBoxIntBatt);
            this.groupBoxPowerSource.Controls.Add(this.comboBoxExtBatt);
            this.groupBoxPowerSource.Controls.Add(this.comboBoxExtDC);
            this.groupBoxPowerSource.Controls.Add(this.comboBoxMainsDC);
            this.groupBoxPowerSource.Controls.Add(this.label4);
            this.groupBoxPowerSource.Controls.Add(this.label3);
            this.groupBoxPowerSource.Controls.Add(this.label2);
            this.groupBoxPowerSource.Controls.Add(this.label1);
            this.groupBoxPowerSource.Location = new System.Drawing.Point(32, 32);
            this.groupBoxPowerSource.Name = "groupBoxPowerSource";
            this.groupBoxPowerSource.Size = new System.Drawing.Size(240, 200);
            this.groupBoxPowerSource.TabIndex = 2;
            this.groupBoxPowerSource.TabStop = false;
            this.groupBoxPowerSource.Text = "Power source channel setup";
            // 
            // comboBoxIntBatt
            // 
            this.comboBoxIntBatt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIntBatt.FormattingEnabled = true;
            this.comboBoxIntBatt.Items.AddRange(new object[] {
            "CH1",
            "CH2",
            "CH3",
            "CH4"});
            this.comboBoxIntBatt.Location = new System.Drawing.Point(152, 144);
            this.comboBoxIntBatt.Name = "comboBoxIntBatt";
            this.comboBoxIntBatt.Size = new System.Drawing.Size(64, 21);
            this.comboBoxIntBatt.TabIndex = 7;
            // 
            // comboBoxExtBatt
            // 
            this.comboBoxExtBatt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExtBatt.FormattingEnabled = true;
            this.comboBoxExtBatt.Items.AddRange(new object[] {
            "CH1",
            "CH2",
            "CH3",
            "CH4"});
            this.comboBoxExtBatt.Location = new System.Drawing.Point(152, 104);
            this.comboBoxExtBatt.Name = "comboBoxExtBatt";
            this.comboBoxExtBatt.Size = new System.Drawing.Size(64, 21);
            this.comboBoxExtBatt.TabIndex = 6;
            // 
            // comboBoxExtDC
            // 
            this.comboBoxExtDC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExtDC.FormattingEnabled = true;
            this.comboBoxExtDC.Items.AddRange(new object[] {
            "CH1",
            "CH2",
            "CH3",
            "CH4"});
            this.comboBoxExtDC.Location = new System.Drawing.Point(152, 64);
            this.comboBoxExtDC.Name = "comboBoxExtDC";
            this.comboBoxExtDC.Size = new System.Drawing.Size(64, 21);
            this.comboBoxExtDC.TabIndex = 5;
            // 
            // comboBoxMainsDC
            // 
            this.comboBoxMainsDC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMainsDC.FormattingEnabled = true;
            this.comboBoxMainsDC.Items.AddRange(new object[] {
            "CH1",
            "CH2",
            "CH3",
            "CH4"});
            this.comboBoxMainsDC.Location = new System.Drawing.Point(152, 24);
            this.comboBoxMainsDC.Name = "comboBoxMainsDC";
            this.comboBoxMainsDC.Size = new System.Drawing.Size(64, 21);
            this.comboBoxMainsDC.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Internal battery channel:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "External battery channel:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "External DC channel:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mains DC channel:";
            // 
            // groupBoxScope
            // 
            this.groupBoxScope.Controls.Add(this.comboBoxSupercapReturnScope);
            this.groupBoxScope.Controls.Add(this.comboBoxSupercapMidScope);
            this.groupBoxScope.Controls.Add(this.label12);
            this.groupBoxScope.Controls.Add(this.label13);
            this.groupBoxScope.Controls.Add(this.comboBoxSupercapHiScope);
            this.groupBoxScope.Controls.Add(this.comboBoxSpeakerScope2);
            this.groupBoxScope.Controls.Add(this.comboBoxSpeakerScope1);
            this.groupBoxScope.Controls.Add(this.label8);
            this.groupBoxScope.Controls.Add(this.label7);
            this.groupBoxScope.Controls.Add(this.label6);
            this.groupBoxScope.Controls.Add(this.comboBoxMainScope);
            this.groupBoxScope.Controls.Add(this.label5);
            this.groupBoxScope.Location = new System.Drawing.Point(320, 32);
            this.groupBoxScope.Name = "groupBoxScope";
            this.groupBoxScope.Size = new System.Drawing.Size(240, 279);
            this.groupBoxScope.TabIndex = 3;
            this.groupBoxScope.TabStop = false;
            this.groupBoxScope.Text = "Scope channel setup";
            // 
            // comboBoxSupercapReturnScope
            // 
            this.comboBoxSupercapReturnScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSupercapReturnScope.FormattingEnabled = true;
            this.comboBoxSupercapReturnScope.Items.AddRange(new object[] {
            "CH A",
            "CH B",
            "CH C",
            "CH D",
            "CH E",
            "CH F",
            "CH G",
            "CH H"});
            this.comboBoxSupercapReturnScope.Location = new System.Drawing.Point(152, 224);
            this.comboBoxSupercapReturnScope.Name = "comboBoxSupercapReturnScope";
            this.comboBoxSupercapReturnScope.Size = new System.Drawing.Size(64, 21);
            this.comboBoxSupercapReturnScope.TabIndex = 21;
            // 
            // comboBoxSupercapMidScope
            // 
            this.comboBoxSupercapMidScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSupercapMidScope.FormattingEnabled = true;
            this.comboBoxSupercapMidScope.Items.AddRange(new object[] {
            "CH A",
            "CH B",
            "CH C",
            "CH D",
            "CH E",
            "CH F",
            "CH G",
            "CH H"});
            this.comboBoxSupercapMidScope.Location = new System.Drawing.Point(152, 184);
            this.comboBoxSupercapMidScope.Name = "comboBoxSupercapMidScope";
            this.comboBoxSupercapMidScope.Size = new System.Drawing.Size(64, 21);
            this.comboBoxSupercapMidScope.TabIndex = 20;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 192);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(116, 13);
            this.label12.TabIndex = 17;
            this.label12.Text = "Supercap mid channel:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 232);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(127, 13);
            this.label13.TabIndex = 18;
            this.label13.Text = "Supercap return channel:";
            // 
            // comboBoxSupercapHiScope
            // 
            this.comboBoxSupercapHiScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSupercapHiScope.FormattingEnabled = true;
            this.comboBoxSupercapHiScope.Items.AddRange(new object[] {
            "CH A",
            "CH B",
            "CH C",
            "CH D",
            "CH E",
            "CH F",
            "CH G",
            "CH H"});
            this.comboBoxSupercapHiScope.Location = new System.Drawing.Point(152, 144);
            this.comboBoxSupercapHiScope.Name = "comboBoxSupercapHiScope";
            this.comboBoxSupercapHiScope.Size = new System.Drawing.Size(64, 21);
            this.comboBoxSupercapHiScope.TabIndex = 15;
            // 
            // comboBoxSpeakerScope2
            // 
            this.comboBoxSpeakerScope2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpeakerScope2.FormattingEnabled = true;
            this.comboBoxSpeakerScope2.Items.AddRange(new object[] {
            "CH A",
            "CH B",
            "CH C",
            "CH D",
            "CH E",
            "CH F",
            "CH G",
            "CH H"});
            this.comboBoxSpeakerScope2.Location = new System.Drawing.Point(152, 104);
            this.comboBoxSpeakerScope2.Name = "comboBoxSpeakerScope2";
            this.comboBoxSpeakerScope2.Size = new System.Drawing.Size(64, 21);
            this.comboBoxSpeakerScope2.TabIndex = 14;
            // 
            // comboBoxSpeakerScope1
            // 
            this.comboBoxSpeakerScope1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpeakerScope1.FormattingEnabled = true;
            this.comboBoxSpeakerScope1.Items.AddRange(new object[] {
            "CH A",
            "CH B",
            "CH C",
            "CH D",
            "CH E",
            "CH F",
            "CH G",
            "CH H"});
            this.comboBoxSpeakerScope1.Location = new System.Drawing.Point(152, 64);
            this.comboBoxSpeakerScope1.Name = "comboBoxSpeakerScope1";
            this.comboBoxSpeakerScope1.Size = new System.Drawing.Size(64, 21);
            this.comboBoxSpeakerScope1.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Main scope channel:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Speaker channel 1:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Speaker channel 2:";
            // 
            // comboBoxMainScope
            // 
            this.comboBoxMainScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMainScope.FormattingEnabled = true;
            this.comboBoxMainScope.Items.AddRange(new object[] {
            "CH A",
            "CH B",
            "CH C",
            "CH D",
            "CH E",
            "CH F",
            "CH G",
            "CH H"});
            this.comboBoxMainScope.Location = new System.Drawing.Point(152, 24);
            this.comboBoxMainScope.Name = "comboBoxMainScope";
            this.comboBoxMainScope.Size = new System.Drawing.Size(64, 21);
            this.comboBoxMainScope.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Supercap high channel:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxSecondaryOut);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.comboBoxPrimaryOut);
            this.groupBox1.Location = new System.Drawing.Point(608, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 125);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Digital channel setup";
            // 
            // comboBoxSecondaryOut
            // 
            this.comboBoxSecondaryOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSecondaryOut.FormattingEnabled = true;
            this.comboBoxSecondaryOut.Items.AddRange(new object[] {
            "Slot 2",
            "Slot 3",
            "Slot 4",
            "Slot 5"});
            this.comboBoxSecondaryOut.Location = new System.Drawing.Point(152, 64);
            this.comboBoxSecondaryOut.Name = "comboBoxSecondaryOut";
            this.comboBoxSecondaryOut.Size = new System.Drawing.Size(64, 21);
            this.comboBoxSecondaryOut.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Primary out channel:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(120, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Secondary out channel:";
            // 
            // comboBoxPrimaryOut
            // 
            this.comboBoxPrimaryOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPrimaryOut.FormattingEnabled = true;
            this.comboBoxPrimaryOut.Items.AddRange(new object[] {
            "Slot 2",
            "Slot 3",
            "Slot 4",
            "Slot 5"});
            this.comboBoxPrimaryOut.Location = new System.Drawing.Point(152, 24);
            this.comboBoxPrimaryOut.Name = "comboBoxPrimaryOut";
            this.comboBoxPrimaryOut.Size = new System.Drawing.Size(64, 21);
            this.comboBoxPrimaryOut.TabIndex = 12;
            // 
            // FixtureConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 446);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxScope);
            this.Controls.Add(this.groupBoxPowerSource);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FixtureConfigForm";
            this.Text = "Configure fixture hardware";
            this.groupBoxPowerSource.ResumeLayout(false);
            this.groupBoxPowerSource.PerformLayout();
            this.groupBoxScope.ResumeLayout(false);
            this.groupBoxScope.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxPowerSource;
        private System.Windows.Forms.GroupBox groupBoxScope;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxIntBatt;
        private System.Windows.Forms.ComboBox comboBoxExtBatt;
        private System.Windows.Forms.ComboBox comboBoxExtDC;
        private System.Windows.Forms.ComboBox comboBoxMainsDC;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxMainScope;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxSupercapHiScope;
        private System.Windows.Forms.ComboBox comboBoxSpeakerScope2;
        private System.Windows.Forms.ComboBox comboBoxSpeakerScope1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxSecondaryOut;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxPrimaryOut;
        private System.Windows.Forms.ComboBox comboBoxSupercapReturnScope;
        private System.Windows.Forms.ComboBox comboBoxSupercapMidScope;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}