namespace DatabaseHelper
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnSaveTo = new System.Windows.Forms.Button();
            this.txtSaveTo = new System.Windows.Forms.TextBox();
            this.lblSaveTo = new System.Windows.Forms.Label();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.btnGenerateCode = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.richTxtLog = new System.Windows.Forms.RichTextBox();
            this.gpConnection = new System.Windows.Forms.GroupBox();
            this.cmbInstances = new System.Windows.Forms.ComboBox();
            this.btnDetect = new System.Windows.Forms.Button();
            this.checkTrustConnection = new System.Windows.Forms.CheckBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkGenTblClass = new System.Windows.Forms.CheckBox();
            this.radioGenSpCode = new System.Windows.Forms.RadioButton();
            this.radioGenSp = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gpConnection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "data.png");
            this.imageList1.Images.SetKeyName(1, "table_sql.png");
            this.imageList1.Images.SetKeyName(2, "table_sql_run.png");
            this.imageList1.Images.SetKeyName(3, "data_connection.png");
            this.imageList1.Images.SetKeyName(4, "data_copy.png");
            this.imageList1.Images.SetKeyName(5, "garbage_empty.png");
            this.imageList1.Images.SetKeyName(6, "column.png");
            this.imageList1.Images.SetKeyName(7, "Start.ico");
            this.imageList1.Images.SetKeyName(8, "Bulb Off.ico");
            this.imageList1.Images.SetKeyName(9, "Bulb Idea.ico");
            this.imageList1.Images.SetKeyName(10, "Bulb On.ico");
            this.imageList1.Images.SetKeyName(11, "Bulb Electric.ico");
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Please Select a folder to Generate code into it";
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClearLog.ImageIndex = 5;
            this.btnClearLog.ImageList = this.imageList1;
            this.btnClearLog.Location = new System.Drawing.Point(10, 504);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(840, 31);
            this.btnClearLog.TabIndex = 35;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnSaveTo
            // 
            this.btnSaveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveTo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveTo.Location = new System.Drawing.Point(817, 135);
            this.btnSaveTo.Name = "btnSaveTo";
            this.btnSaveTo.Size = new System.Drawing.Size(37, 25);
            this.btnSaveTo.TabIndex = 34;
            this.btnSaveTo.Text = "...";
            this.btnSaveTo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveTo.UseVisualStyleBackColor = true;
            this.btnSaveTo.Click += new System.EventHandler(this.btnSaveTo_Click);
            // 
            // txtSaveTo
            // 
            this.txtSaveTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSaveTo.Location = new System.Drawing.Point(251, 138);
            this.txtSaveTo.Name = "txtSaveTo";
            this.txtSaveTo.Size = new System.Drawing.Size(560, 20);
            this.txtSaveTo.TabIndex = 33;
            // 
            // lblSaveTo
            // 
            this.lblSaveTo.AutoSize = true;
            this.lblSaveTo.Location = new System.Drawing.Point(195, 141);
            this.lblSaveTo.Name = "lblSaveTo";
            this.lblSaveTo.Size = new System.Drawing.Size(50, 13);
            this.lblSaveTo.TabIndex = 32;
            this.lblSaveTo.Text = "Save to :";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Items.AddRange(new object[] {
            "C#",
            "VB"});
            this.cmbLanguage.Location = new System.Drawing.Point(141, 136);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(46, 21);
            this.cmbLanguage.TabIndex = 31;
            this.cmbLanguage.Text = "C#";
            // 
            // btnGenerateCode
            // 
            this.btnGenerateCode.Enabled = false;
            this.btnGenerateCode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenerateCode.ImageIndex = 7;
            this.btnGenerateCode.ImageList = this.imageList1;
            this.btnGenerateCode.Location = new System.Drawing.Point(10, 131);
            this.btnGenerateCode.Name = "btnGenerateCode";
            this.btnGenerateCode.Size = new System.Drawing.Size(125, 33);
            this.btnGenerateCode.TabIndex = 30;
            this.btnGenerateCode.Text = "Generate Code";
            this.btnGenerateCode.UseVisualStyleBackColor = true;
            this.btnGenerateCode.Click += new System.EventHandler(this.btnGenerateCode_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(10, 170);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTxtLog);
            this.splitContainer1.Size = new System.Drawing.Size(844, 327);
            this.splitContainer1.SplitterDistance = 261;
            this.splitContainer1.TabIndex = 29;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(261, 327);
            this.treeView1.TabIndex = 2;
            // 
            // richTxtLog
            // 
            this.richTxtLog.BackColor = System.Drawing.SystemColors.Control;
            this.richTxtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTxtLog.Location = new System.Drawing.Point(0, 0);
            this.richTxtLog.Name = "richTxtLog";
            this.richTxtLog.ReadOnly = true;
            this.richTxtLog.Size = new System.Drawing.Size(579, 327);
            this.richTxtLog.TabIndex = 15;
            this.richTxtLog.Text = "";
            // 
            // gpConnection
            // 
            this.gpConnection.Controls.Add(this.cmbInstances);
            this.gpConnection.Controls.Add(this.btnDetect);
            this.gpConnection.Controls.Add(this.checkTrustConnection);
            this.gpConnection.Controls.Add(this.btnConnect);
            this.gpConnection.Controls.Add(this.txtPass);
            this.gpConnection.Controls.Add(this.lblPass);
            this.gpConnection.Controls.Add(this.txtUser);
            this.gpConnection.Controls.Add(this.lblUser);
            this.gpConnection.Location = new System.Drawing.Point(10, 28);
            this.gpConnection.Name = "gpConnection";
            this.gpConnection.Size = new System.Drawing.Size(576, 97);
            this.gpConnection.TabIndex = 28;
            this.gpConnection.TabStop = false;
            this.gpConnection.Text = "Connection";
            // 
            // cmbInstances
            // 
            this.cmbInstances.FormattingEnabled = true;
            this.cmbInstances.Location = new System.Drawing.Point(180, 26);
            this.cmbInstances.Name = "cmbInstances";
            this.cmbInstances.Size = new System.Drawing.Size(211, 21);
            this.cmbInstances.TabIndex = 10;
            this.cmbInstances.Text = ".";
            // 
            // btnDetect
            // 
            this.btnDetect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDetect.ImageIndex = 4;
            this.btnDetect.ImageList = this.imageList1;
            this.btnDetect.Location = new System.Drawing.Point(13, 20);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(153, 31);
            this.btnDetect.TabIndex = 9;
            this.btnDetect.Text = "Detect Data Sources";
            this.btnDetect.UseVisualStyleBackColor = true;
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // checkTrustConnection
            // 
            this.checkTrustConnection.AutoSize = true;
            this.checkTrustConnection.Location = new System.Drawing.Point(180, 67);
            this.checkTrustConnection.Name = "checkTrustConnection";
            this.checkTrustConnection.Size = new System.Drawing.Size(119, 17);
            this.checkTrustConnection.TabIndex = 8;
            this.checkTrustConnection.Text = "Trusted Connection";
            this.checkTrustConnection.UseVisualStyleBackColor = true;
            this.checkTrustConnection.CheckedChanged += new System.EventHandler(this.checkTrustConnection_CheckedChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.ImageIndex = 3;
            this.btnConnect.ImageList = this.imageList1;
            this.btnConnect.Location = new System.Drawing.Point(13, 57);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(153, 31);
            this.btnConnect.TabIndex = 7;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(484, 65);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '*';
            this.txtPass.Size = new System.Drawing.Size(77, 20);
            this.txtPass.TabIndex = 6;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(443, 68);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(36, 13);
            this.lblPass.TabIndex = 5;
            this.lblPass.Text = "Pass :";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(348, 65);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(77, 20);
            this.txtUser.TabIndex = 4;
            this.txtUser.Text = "sa";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(307, 68);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(35, 13);
            this.lblUser.TabIndex = 1;
            this.lblUser.Text = "User :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkGenTblClass);
            this.groupBox1.Controls.Add(this.radioGenSpCode);
            this.groupBox1.Controls.Add(this.radioGenSp);
            this.groupBox1.Location = new System.Drawing.Point(594, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 97);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output Type";
            // 
            // checkGenTblClass
            // 
            this.checkGenTblClass.AutoSize = true;
            this.checkGenTblClass.Location = new System.Drawing.Point(19, 69);
            this.checkGenTblClass.Name = "checkGenTblClass";
            this.checkGenTblClass.Size = new System.Drawing.Size(172, 17);
            this.checkGenTblClass.TabIndex = 9;
            this.checkGenTblClass.Text = "Generate Tables Classes (BLL)";
            this.checkGenTblClass.UseVisualStyleBackColor = true;
            this.checkGenTblClass.CheckedChanged += new System.EventHandler(this.checkGenTblClass_CheckedChanged);
            // 
            // radioGenSpCode
            // 
            this.radioGenSpCode.AutoSize = true;
            this.radioGenSpCode.Location = new System.Drawing.Point(19, 46);
            this.radioGenSpCode.Name = "radioGenSpCode";
            this.radioGenSpCode.Size = new System.Drawing.Size(233, 17);
            this.radioGenSpCode.TabIndex = 1;
            this.radioGenSpCode.TabStop = true;
            this.radioGenSpCode.Text = "Generate Code for Stored Procedures (DAL)";
            this.radioGenSpCode.UseVisualStyleBackColor = true;
            this.radioGenSpCode.CheckedChanged += new System.EventHandler(this.radioGenSpCode_CheckedChanged);
            // 
            // radioGenSp
            // 
            this.radioGenSp.AutoSize = true;
            this.radioGenSp.Location = new System.Drawing.Point(19, 23);
            this.radioGenSp.Name = "radioGenSp";
            this.radioGenSp.Size = new System.Drawing.Size(160, 17);
            this.radioGenSp.TabIndex = 0;
            this.radioGenSp.TabStop = true;
            this.radioGenSp.Text = "Generate Stored Procedures";
            this.radioGenSp.UseVisualStyleBackColor = true;
            this.radioGenSp.CheckedChanged += new System.EventHandler(this.radioGenSp_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(864, 24);
            this.menuStrip1.TabIndex = 37;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 548);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnSaveTo);
            this.Controls.Add(this.txtSaveTo);
            this.Controls.Add(this.lblSaveTo);
            this.Controls.Add(this.cmbLanguage);
            this.Controls.Add(this.btnGenerateCode);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.gpConnection);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Db Helper v1.0.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.gpConnection.ResumeLayout(false);
            this.gpConnection.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnSaveTo;
        private System.Windows.Forms.TextBox txtSaveTo;
        private System.Windows.Forms.Label lblSaveTo;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Button btnGenerateCode;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.GroupBox gpConnection;
        private System.Windows.Forms.ComboBox cmbInstances;
        private System.Windows.Forms.Button btnDetect;
        private System.Windows.Forms.CheckBox checkTrustConnection;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.RichTextBox richTxtLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioGenSp;
        private System.Windows.Forms.CheckBox checkGenTblClass;
        private System.Windows.Forms.RadioButton radioGenSpCode;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}

