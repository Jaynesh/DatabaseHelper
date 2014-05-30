using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SQLDMO;
using GetDatabaseSp;
using System.Data.Sql;
using NovinMedia.Data;
using DatabaseHelper.Properties;
using SP_Gen.Classes;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace DatabaseHelper
{
    public partial class frmMain : Form
    {
        // this dataTable object hold all information about all storedProcedures (include spName,parameters, ...)
        DataTable dtSPs;

        public frmMain()
        {
            InitializeComponent();            
        }

        // define an enameration for available language providers
        public enum Providers
        {
            VbProvider = 1,
            CsProvider = 2
        };   

        // get connection string for use to generate sql sps
        private string GetConnectionString()
        {
            string connStr = string.Empty;

            if (!this.checkTrustConnection.Checked)
            {
                connStr =
                    string.Format("Server={0:G};Database=master;User ID={1:G};Password={2:G};Trusted_Connection=False;",
                                  this.cmbInstances.Text, this.txtUser.Text, this.txtPass.Text);
            }
            else
                connStr =
                    string.Format("Data Source={0:G};Initial Catalog=master;Integrated Security=SSPI;",
                                  this.cmbInstances.Text);

            return connStr;
        }

        #region Events

        // initialize app
        private void Form1_Load(object sender, EventArgs e)
        {            
            this.PrintToLog("Program Started.");
            this.PrintToLog("---- Welcome to Database Helper version 1.0.0 ----");
            this.checkTrustConnection.Select();
            this.checkTrustConnection.Checked = true;
            this.LoadSettings();
        }

        // detect all dataSource(s) for current netWork
        private void btnDetect_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnDetect.Enabled = false;
                this.cmbInstances.Enabled = false;
                this.cmbInstances.Items.Clear();
                this.cmbInstances.Text = "Retrieving DataSource(s) ...";
                this.PrintToLog("Retrieving DataSource(s) ...");
                System.Windows.Forms.Application.DoEvents();

                DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();
                for (int i = 0; i < servers.Rows.Count; i++)
                {
                    cmbInstances.Items.Add(servers.Rows[i]["ServerName"] + "\\" + servers.Rows[i]["InstanceName"]);
                }
                if (this.cmbInstances.Items.Count > 0)
                    cmbInstances.SelectedIndex = 0;

                this.btnDetect.Enabled = true;
                this.cmbInstances.Enabled = true;
                this.PrintToLog(string.Format("Found {0} DataSource(s) in the network", this.cmbInstances.Items.Count));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        // connect to selected dataSource and retrieve all database,tables and sps
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (this.checkTrustConnection.Checked)
            {
                string dataSource = this.cmbInstances.Text;
                this.PrintToLog(string.Format("Retrieving object(s) from datasource : {0}", dataSource));
                this.btnConnect.Enabled = false;
                System.Windows.Forms.Application.DoEvents();
                this.backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                if (this.txtUser.Text == string.Empty || this.txtPass.Text == string.Empty)
                {
                    MessageBox.Show("You selected UnTrusted Connection, Please fill UserName and Password", "Login information required");
                    this.btnConnect.Enabled = true;
                    this.PrintToLog("Error in connection");
                }
                else
                {
                    string dataSource = this.cmbInstances.Text;
                    this.PrintToLog(string.Format("Retrieving object(s) from datasource : {0}", dataSource));
                    this.btnConnect.Enabled = false;
                    System.Windows.Forms.Application.DoEvents();
                    this.backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        // change connection mode
        private void checkTrustConnection_CheckedChanged(object sender, EventArgs e)
        {
            this.PrintToLog(string.Format("Trusted Connection set to : {0}", this.checkTrustConnection.Checked));
            this.lblUser.Enabled = !this.checkTrustConnection.Checked;
            this.lblPass.Enabled = !this.checkTrustConnection.Checked;

            this.txtUser.Enabled = !this.checkTrustConnection.Checked;
            this.txtPass.Enabled = !this.checkTrustConnection.Checked;
        }

        // generate code and sps
        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                MessageBox.Show("Please Select a Database", "No database selected");
            else
            {
                if (this.txtSaveTo.Text == string.Empty)
                {
                    MessageBox.Show("Please enter path to generate Bussiness layer(s)", "Path reqiuired");
                }
                else
                {
                    this.GenerateSqlSpAndClassCodeAndGetSps();
                }
            }
        }

        // select outPut folder(s)
        private void btnSaveTo_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.PrintToLog(string.Format("Selected path is {0}", this.folderBrowserDialog1.SelectedPath));
                this.txtSaveTo.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        // clear log
        private void btnClearLog_Click(object sender, EventArgs e)
        {
            this.richTxtLog.Text = string.Empty;
        }

        // get all server objects (include databases,tables and sps) in Async mode
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.GetServerObjects();
        }

        // release ui that freezed for retrieving data from dataSource and print result to log
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.btnConnect.Enabled = true;
            this.btnGenerateCode.Enabled = true;
            this.PrintToLog(string.Format("Database object(s) has been completed, Found {0} database(s) in the selected datasource.", this.treeView1.Nodes.Count));
        }

        private void radioGenSp_CheckedChanged(object sender, EventArgs e)
        {
            if(radioGenSp.Checked)
                this.PrintToLog("Generate Stored Procedures Selected");
        }

        private void radioGenSpCode_CheckedChanged(object sender, EventArgs e)
        {
            if(radioGenSpCode.Checked)
                this.PrintToLog("Generate Code for Stored Procedures Selected");
        }

        private void checkGenTblClass_CheckedChanged(object sender, EventArgs e)
        {
            this.PrintToLog(string.Format("Generate Class for Tables set to : {0}", this.checkGenTblClass.Checked));
        }        

        #endregion

        // load databases,tables and sps into treeView
        private void GetServerObjects()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(this.GetServerObjects), null);
            }
            else
            {
                this.treeView1.Nodes.Clear();

                SQLServerClass m_sqlServer = new SQLServerClass();
                string serverName = this.cmbInstances.Text;
                string userName = string.Empty;
                string pass = string.Empty;
                if (!this.checkTrustConnection.Checked)
                {
                    userName = this.txtUser.Text;
                    pass = this.txtPass.Text;
                    m_sqlServer.Connect(serverName, userName, pass);
                }
                else
                {
                    m_sqlServer.LoginSecure = true;
                    m_sqlServer.Connect(serverName, "", "");
                }

                TreeNode database;
                TreeNode procedures;
                TreeNode storedProcedure;
                TreeNode storedProcedureParameter;

                TreeNode tables;
                TreeNode table;
                TreeNode column;

                #region Iterate through all databases to extract sps and tables

                foreach (Database2 db in m_sqlServer.Databases)
                {
                    if (!db.SystemObject)
                    {
                        database = new TreeNode(db.Name);
                        database.ImageIndex = 0;

                        procedures = new TreeNode("Procedures");
                        procedures.ImageIndex = 8;
                        procedures.SelectedImageIndex = 8;

                        #region Get All stored procedures for each database
                        // Get all procedures for each database
                        foreach (StoredProcedure2 sp in db.StoredProcedures)
                        {
                            if (!sp.SystemObject && sp.Name != "sp_alterdiagram" && sp.Name != "sp_creatediagram" && sp.Name != "sp_dropdiagram" && sp.Name != "sp_helpdiagrams" && sp.Name != "sp_renamediagram" && sp.Name != "sp_upgraddiagrams" && sp.Name != "sp_helpdiagramdefinition")
                            {
                                storedProcedure = new TreeNode(sp.Name);
                                storedProcedure.ImageIndex = 10;
                                storedProcedure.SelectedImageIndex = 10;
                                QueryResults res = sp.EnumParameters();
                                StoredProcedureParameter spParameter;
                                StoredProcedureParameterCollection spParameterCollection = new StoredProcedureParameterCollection();
                                int row, col;
                                for (row = 1; row <= res.Rows; row++)
                                {
                                    for (col = 1; col <= res.Columns; col++)
                                    {
                                        string spParamName = res.GetColumnString(row, col);
                                        string spParamType = res.GetColumnString(row, col + 1);
                                        string spParamLength = res.GetColumnString(row, col + 2);
                                        string spIsOutput = res.GetColumnString(row, col + 4) != "0" ? ",OutPut" : "";
                                        bool checkBooleanExpression = res.GetColumnString(row, col + 4) != "0" ? true : false;
                                        storedProcedureParameter = new TreeNode(string.Format("{0},{1}({2}){3}", spParamName, spParamType, spParamLength, spIsOutput));
                                        storedProcedureParameter.ImageIndex = 9;
                                        storedProcedureParameter.SelectedImageIndex = 11;
                                        spParameter = new StoredProcedureParameter(spParamName, spParamType, int.Parse(spParamLength), checkBooleanExpression);
                                        spParameterCollection.Add(spParameter);
                                        storedProcedure.Nodes.Add(storedProcedureParameter);
                                        break;
                                    }
                                }
                                procedures.Nodes.Add(storedProcedure);                                
                            }
                        }                        
                        database.Nodes.Add(procedures);
                        #endregion

                        #region Get all tables for each database
                        // Get all tables for each database
                        tables = new TreeNode("Tables");
                        tables.ImageIndex = 1;
                        tables.SelectedImageIndex = 1;

                        foreach (Table2 tbl in db.Tables)
                        {
                            if (!tbl.SystemObject && tbl.Name != "sysdiagrams")
                            {
                                table = new TreeNode(tbl.Name);
                                table.ImageIndex = 1;
                                table.SelectedImageIndex = 1;
                                foreach (Column2 col in tbl.Columns)
                                {
                                    column = new TreeNode(string.Format("{0},{1}", col.Name, col.Datatype));
                                    column.ImageIndex = 6;
                                    column.SelectedImageIndex = 6;
                                    table.Nodes.Add(column);
                                }
                                tables.Nodes.Add(table);                                
                            }
                        }
                        database.Nodes.Add(tables);
                        #endregion

                        // Add database to treeView1.Nodes
                        this.treeView1.Nodes.Add(database);
                    }
                }
                #endregion
            }
        }        

        // print action information to log
        private void PrintToLog(string text)
        {
            this.richTxtLog.Text += text + "\n";
            this.richTxtLog.SelectionStart = this.richTxtLog.TextLength;
            this.richTxtLog.ScrollToCaret();
        }

        // generate sql sps and class code together
        private void GenerateSqlSpAndClassCodeAndGetSps()
        {            
            //this.btnGenerateCode.Enabled = false;

            #region initialize new some DataColumn objects for retreieve all informations about stored procedures in db            

            DataColumn dcSpName = new DataColumn("SpName");
            DataColumn dcParameterName = new DataColumn("ParameterName");
            DataColumn dcSystemType = new DataColumn("SystemType");
            DataColumn dcLength = new DataColumn("Length");
            DataColumn dcIsOutputParameter = new DataColumn("IsOutputParameter");

            this.dtSPs = new DataTable();
            this.dtSPs.Columns.Add(dcSpName);
            this.dtSPs.Columns.Add(dcParameterName);
            this.dtSPs.Columns.Add(dcSystemType);
            this.dtSPs.Columns.Add(dcLength);
            this.dtSPs.Columns.Add(dcIsOutputParameter);
            #endregion

            string dbName;
            string tableName;
            string colName;
            string spName;
            string sp_parameter;
            string sqlScript = string.Empty;
            
            TreeNode currentNode = this.treeView1.SelectedNode;
            if (currentNode.Parent != null)
            {
                // this node either Tables/Procedures or table/procedure or column/procedureParameter
                TreeNode parentNode1 = currentNode.Parent;
                if (parentNode1.Parent != null)
                {
                    // this node is table/procedure or column/procedureParameter
                    if (parentNode1.Parent.Parent != null)
                    {
                        // this node is column/procedureParameter
                        TreeNode dbNode = parentNode1.Parent.Parent;
                        dbName = dbNode.Text;
                        int tblCount = 0;
                        int spCount = 0;

                        #region Actions to Create code for Stored Procedures

                        if (radioGenSpCode.Checked)
                        {
                            foreach (TreeNode spNode in dbNode.Nodes[0].Nodes)
                            {
                                spCount++;
                                spName = spNode.Text;
                                foreach (TreeNode sp_paramNode in spNode.Nodes)
                                {
                                    string[] txts = sp_paramNode.Text.Split(new char[] { ',' });
                                    string colParameterName = "";
                                    string colSystemType = "";
                                    string colLength = "";
                                    int colIsOutputParameter = 0;

                                    if (txts.Length > 2)
                                    {
                                        colParameterName = txts[0];
                                        colIsOutputParameter = txts[2] == "OutPut" ? 1 : 0;

                                        string[] txts2 = txts[1].Split('(', ')');
                                        colSystemType = txts2[0];
                                        colLength = txts2[1];
                                        this.dtSPs.Rows.Add(spName, colParameterName, colSystemType, colLength, colIsOutputParameter);
                                    }
                                    else
                                    {
                                        colParameterName = txts[0];
                                        string[] txts2 = txts[1].Split('(', ')');
                                        colSystemType = txts2[0];
                                        colLength = txts2[1];
                                        this.dtSPs.Rows.Add(spName, colParameterName, colSystemType, colLength, colIsOutputParameter);
                                    }
                                }
                                this.dtSPs.Rows.Add(spName, "NULL", "NULL", "NULL", "NULL");
                            }
                            if (this.cmbLanguage.Text == "C#")
                                this.GenerateDocumentForSps(dbName, "DAL", Providers.CsProvider);
                            if (this.cmbLanguage.Text == "VB")
                                this.GenerateDocumentForSps(dbName, "DAL", Providers.VbProvider);
                        }
                        #endregion

                        #region Actions to Create class for tables or Cenerate Stored Procedures

                        // initialize objects for query through database to generate sql sps
                        string connStr = GetConnectionString();
                        DbObject dbo = new DbObject(connStr);

                        string x = string.Format(Resources.strTablesAndColumns, dbName);
                        DataSet dsTablesAndColumns = dbo.RunQuery(x, "TablesAndColumns");                        

                        foreach (TreeNode tblNode in dbNode.Nodes[1].Nodes)
                        {                                                        
                            tableName = tblNode.Text;
                            tblCount++;

                            if (this.radioGenSp.Checked)
                            {
                                // execute some actions, then generate sql sps
                                this.PrintToLog(string.Format("Generating Stored Procedures(SelectAll,SelectRow,Insert,Update,DeleteRow) for table '{0}' ...", tableName));
                                DataRow[] rows = dsTablesAndColumns.Tables[0].Select("Table_Name = '" + tableName + "'");
                                sqlScript += GenerateSQL(dbName, tableName, rows);                                
                            }

                            List<CodeDomDatabaseSQLDMO.Column> columnCollection = new List<CodeDomDatabaseSQLDMO.Column>();
                            foreach (TreeNode colNode in tblNode.Nodes)
                            {
                                string[] txts = colNode.Text.Split(new char[] { ',' });
                                string colName3 = txts[0];
                                string columnType = txts[1];

                                CodeDomDatabaseSQLDMO.Column col = new CodeDomDatabaseSQLDMO.Column(colName3, columnType);
                                columnCollection.Add(col);
                            }

                            // check if user select generate class for tables, then generate class for tables
                            if (this.checkGenTblClass.Checked)
                            {
                                this.PrintToLog(string.Format("Generating Class '{0}' for table '{1}' in Namespace '{2}' ...", this.ToUpperFirstChar(tableName), tableName, dbName));
                                System.Windows.Forms.Application.DoEvents();
                                this.GenerateDocumentForCode(dbName, tableName, columnCollection);                                
                            }
                        }
                        if (this.radioGenSp.Checked == true && sqlScript.Length > 0)
                            this.PrintToLog(string.Format("Sp generation successfully completed for {0} table(s).", tblCount));
                        if (tblCount > 0 && this.checkGenTblClass.Checked)
                            this.PrintToLog(string.Format("Code generation successfully completed for {0} table(s)", tblCount));
                        #endregion
                    }
                    else
                    {
                        // this node is table/procedure
                        TreeNode dbNode = parentNode1.Parent;
                        dbName = dbNode.Text;
                        int tblCount = 0;
                        int spCount = 0;

                        #region Actions to Create code for Stored Procedures

                        if (radioGenSpCode.Checked)
                        {
                            foreach (TreeNode spNode in dbNode.Nodes[0].Nodes)
                            {
                                spCount++;
                                spName = spNode.Text;
                                foreach (TreeNode sp_paramNode in spNode.Nodes)
                                {
                                    string[] txts = sp_paramNode.Text.Split(new char[] { ',' });
                                    string colParameterName = "";
                                    string colSystemType = "";
                                    string colLength = "";
                                    int colIsOutputParameter = 0;

                                    if (txts.Length > 2)
                                    {
                                        colParameterName = txts[0];
                                        colIsOutputParameter = txts[2] == "OutPut" ? 1 : 0;

                                        string[] txts2 = txts[1].Split('(', ')');
                                        colSystemType = txts2[0];
                                        colLength = txts2[1];
                                        this.dtSPs.Rows.Add(spName, colParameterName, colSystemType, colLength, colIsOutputParameter);
                                    }
                                    else
                                    {
                                        colParameterName = txts[0];
                                        string[] txts2 = txts[1].Split('(', ')');
                                        colSystemType = txts2[0];
                                        colLength = txts2[1];
                                        this.dtSPs.Rows.Add(spName, colParameterName, colSystemType, colLength, colIsOutputParameter);
                                    }
                                }
                                this.dtSPs.Rows.Add(spName, "NULL", "NULL", "NULL", "NULL");
                            }
                            if (this.cmbLanguage.Text == "C#")
                                this.GenerateDocumentForSps(dbName, "DAL", Providers.CsProvider);
                            if (this.cmbLanguage.Text == "VB")
                                this.GenerateDocumentForSps(dbName, "DAL", Providers.VbProvider);
                        }
                        #endregion

                        #region Actions to Create class for tables or Cenerate Stored Procedures

                        // initialize objects for query through database to generate sql sps
                        string connStr = GetConnectionString();
                        DbObject dbo = new DbObject(connStr);

                        string x = string.Format(Resources.strTablesAndColumns, dbName);
                        DataSet dsTablesAndColumns = dbo.RunQuery(x, "TablesAndColumns");                       

                        foreach (TreeNode tblNode in dbNode.Nodes[1].Nodes)
                        {                                                        
                            tableName = tblNode.Text;
                            tblCount++;

                            if (this.radioGenSp.Checked)
                            {
                                // execute some actions, then generate sql sps
                                this.PrintToLog(string.Format("Generating Stored Procedures(SelectAll,SelectRow,Insert,Update,DeleteRow) for table '{0}' ...", tableName));
                                DataRow[] rows = dsTablesAndColumns.Tables[0].Select("Table_Name = '" + tableName + "'");
                                sqlScript += GenerateSQL(dbName, tableName, rows);                                
                            }

                            List<CodeDomDatabaseSQLDMO.Column> columnCollection = new List<CodeDomDatabaseSQLDMO.Column>();
                            foreach (TreeNode colNode in tblNode.Nodes)
                            {
                                string[] txts = colNode.Text.Split(new char[] { ',' });
                                string colName3 = txts[0];
                                string columnType = txts[1];

                                CodeDomDatabaseSQLDMO.Column col = new CodeDomDatabaseSQLDMO.Column(colName3, columnType);
                                columnCollection.Add(col);
                            }

                            // check if user select generate class for tables, then generate class for tables
                            if (this.checkGenTblClass.Checked)
                            {
                                this.PrintToLog(string.Format("Generating Class '{0}' for table '{1}' in Namespace '{2}' ...", this.ToUpperFirstChar(tableName), tableName, dbName));
                                System.Windows.Forms.Application.DoEvents();
                                this.GenerateDocumentForCode(dbName, tableName, columnCollection);
                            }
                        }
                        if (this.radioGenSp.Checked == true && sqlScript.Length > 0)
                            this.PrintToLog(string.Format("Sp generation successfully completed for {0} table(s).", tblCount));
                        if (tblCount > 0 && this.checkGenTblClass.Checked)
                            this.PrintToLog(string.Format("Code generation successfully completed for {0} table(s)", tblCount));
                        #endregion
                    }
                }
                else
                {
                    // this node is Tables or Procedures node                                
                    dbName = parentNode1.Text;
                    TreeNode dbNode = currentNode.Parent;
                    int tblCount = 0;
                    int spCount = 0;

                    #region Actions to Create code for Stored Procedures

                    if (radioGenSpCode.Checked)
                    {
                        foreach (TreeNode spNode in dbNode.Nodes[0].Nodes)
                        {
                            spCount++;
                            spName = spNode.Text;
                            foreach (TreeNode sp_paramNode in spNode.Nodes)
                            {
                                string[] txts = sp_paramNode.Text.Split(new char[] { ',' });
                                string colParameterName = "";
                                string colSystemType = "";
                                string colLength = "";
                                int colIsOutputParameter = 0;

                                if (txts.Length > 2)
                                {
                                    colParameterName = txts[0];
                                    colIsOutputParameter = txts[2] == "OutPut" ? 1 : 0;

                                    string[] txts2 = txts[1].Split('(', ')');
                                    colSystemType = txts2[0];
                                    colLength = txts2[1];
                                    this.dtSPs.Rows.Add(spName, colParameterName, colSystemType, colLength, colIsOutputParameter);
                                }
                                else
                                {
                                    colParameterName = txts[0];
                                    string[] txts2 = txts[1].Split('(', ')');
                                    colSystemType = txts2[0];
                                    colLength = txts2[1];
                                    this.dtSPs.Rows.Add(spName, colParameterName, colSystemType, colLength, colIsOutputParameter);
                                }
                            }
                            this.dtSPs.Rows.Add(spName, "NULL", "NULL", "NULL", "NULL");
                        }
                        if(this.cmbLanguage.Text == "C#")
                            this.GenerateDocumentForSps(dbName, "DAL", Providers.CsProvider);
                        if(this.cmbLanguage.Text == "VB")
                            this.GenerateDocumentForSps(dbName, "DAL", Providers.VbProvider);
                    }
                    #endregion

                    #region Actions to Create class for tables or Cenerate Stored Procedures

                    // initialize objects for query through database to generate sql sps
                    string connStr = GetConnectionString();
                    DbObject dbo = new DbObject(connStr);
                    string x = string.Format(Resources.strTablesAndColumns, dbName);
                    DataSet dsTablesAndColumns = dbo.RunQuery(x, "TablesAndColumns");

                    foreach (TreeNode tblNode in currentNode.Nodes[1].Nodes)
                    {
                        tableName = tblNode.Text;
                        tblCount++;

                        if (radioGenSp.Checked)
                        {
                            // execute some actions, then generate sql sps
                            this.PrintToLog(string.Format("Generating Stored Procedures(SelectAll,SelectRow,Insert,Update,DeleteRow) for table '{0}' ...", tableName));
                            DataRow[] rows = dsTablesAndColumns.Tables[0].Select("Table_Name = '" + tableName + "'");
                            sqlScript += GenerateSQL(dbName, tableName, rows);                            
                        }

                        List<CodeDomDatabaseSQLDMO.Column> columnCollection = new List<CodeDomDatabaseSQLDMO.Column>();
                        foreach (TreeNode colNode in tblNode.Nodes)
                        {
                            string[] txts = colNode.Text.Split(new char[] { ',' });
                            string colName3 = txts[0];
                            string columnType = txts[1];

                            CodeDomDatabaseSQLDMO.Column col = new CodeDomDatabaseSQLDMO.Column(colName3, columnType);
                            columnCollection.Add(col);
                        }

                        // check if user select generate class for tables, then generate class for tables
                        if (this.checkGenTblClass.Checked)
                        {
                            this.PrintToLog(string.Format("Generating Class '{0}' for table '{1}' in Namespace '{2}' ...", this.ToUpperFirstChar(tableName), tableName, dbName));
                            System.Windows.Forms.Application.DoEvents();
                            this.GenerateDocumentForCode(dbName, tableName, columnCollection);                            
                        }
                    }
                    if (this.radioGenSp.Checked == true && sqlScript.Length > 0)
                        this.PrintToLog(string.Format("Sp generation successfully completed for {0} table(s).", tblCount));
                    if (tblCount > 0 && this.checkGenTblClass.Checked)
                        this.PrintToLog(string.Format("Code generation successfully completed for {0} table(s)", tblCount));
                    #endregion
                }
            }
            else
            {
                // this node is database node
                dbName = currentNode.Text;
                int tblCount = 0;
                int spCount = 0;

                #region Actions to Create code for Stored Procedures

                if (radioGenSpCode.Checked)
                {
                    foreach (TreeNode spNode in currentNode.Nodes[0].Nodes)
                    {
                        spCount++;
                        spName = spNode.Text;
                        foreach (TreeNode sp_paramNode in spNode.Nodes)
                        {
                            string[] txts = sp_paramNode.Text.Split(new char[] { ',' });
                            string colParameterName = "";
                            string colSystemType = "";
                            string colLength = "";
                            int colIsOutputParameter = 0;

                            if (txts.Length > 2)
                            {
                                colParameterName = txts[0];
                                colIsOutputParameter = txts[2] == "OutPut" ? 1 : 0;

                                string[] txts2 = txts[1].Split('(', ')');
                                colSystemType = txts2[0];
                                colLength = txts2[1];
                                this.dtSPs.Rows.Add(spName, colParameterName, colSystemType, colLength, colIsOutputParameter);
                            }
                            else
                            {
                                colParameterName = txts[0];
                                string[] txts2 = txts[1].Split('(', ')');
                                colSystemType = txts2[0];
                                colLength = txts2[1];
                                this.dtSPs.Rows.Add(spName, colParameterName, colSystemType, colLength, colIsOutputParameter);
                            }
                        }
                        this.dtSPs.Rows.Add(spName, "NULL", "NULL", "NULL", "NULL");
                    }
                    if (this.cmbLanguage.Text == "C#")
                        this.GenerateDocumentForSps(dbName, "DAL", Providers.CsProvider);
                    if (this.cmbLanguage.Text == "VB")
                        this.GenerateDocumentForSps(dbName, "DAL", Providers.VbProvider);
                }
                #endregion

                #region Actions to Create class for tables or Cenerate Stored Procedures

                // initialize objects for query through database to generate sql sps
                string connStr = GetConnectionString();
                DbObject dbo = new DbObject(connStr);
                string x = string.Format(Resources.strTablesAndColumns, dbName);
                DataSet dsTablesAndColumns = dbo.RunQuery(x, "TablesAndColumns");

                foreach (TreeNode tblNode in currentNode.Nodes[1].Nodes)
                {
                    tableName = tblNode.Text;
                    tblCount++;

                    if (radioGenSp.Checked)
                    {
                        // execute some actions, then generate sql sps
                        this.PrintToLog(string.Format("Generating Stored Procedures(SelectAll,SelectRow,Insert,Update,DeleteRow) for table '{0}' ...", tableName));
                        DataRow[] rows = dsTablesAndColumns.Tables[0].Select("Table_Name = '" + tableName + "'");
                        sqlScript += GenerateSQL(dbName, tableName, rows);                        
                    }                    

                    List<CodeDomDatabaseSQLDMO.Column> columnCollection = new List<CodeDomDatabaseSQLDMO.Column>();
                    foreach (TreeNode colNode in tblNode.Nodes)
                    {
                        string[] txts = colNode.Text.Split(new char[] { ',' });
                        string colName3 = txts[0];
                        string columnType = txts[1];

                        CodeDomDatabaseSQLDMO.Column col = new CodeDomDatabaseSQLDMO.Column(colName3, columnType);
                        columnCollection.Add(col);                        
                    }

                    // check if user select generate class for tables, then generate class for tables
                    if (this.checkGenTblClass.Checked)
                    {
                        this.PrintToLog(string.Format("Generating Class '{0}' for table '{1}' in Namespace '{2}' ...", this.ToUpperFirstChar(tableName), tableName, dbName));
                        System.Windows.Forms.Application.DoEvents();
                        this.GenerateDocumentForCode(dbName, tableName, columnCollection);                        
                    }
                }
                if (this.radioGenSp.Checked == true && sqlScript.Length > 0)
                    this.PrintToLog(string.Format("Sp generation successfully completed for {0} table(s).", tblCount));
                if (tblCount > 0 && this.checkGenTblClass.Checked)
                    this.PrintToLog(string.Format("Code generation successfully completed for {0} table(s)", tblCount));
                #endregion
            }

            if (this.radioGenSp.Checked)
            {
                // save sql sps to file
                DirectoryInfo dirInfo = Directory.CreateDirectory(this.txtSaveTo.Text + "\\Sql");
                StreamWriter sw = new StreamWriter(File.Create(dirInfo.FullName + "\\" + dbName + "_Sps.sql"));
                sw.Write(sqlScript);
                sw.Flush();
                sw.Close();
            }                
        }

        // initialize document to generate code for stored procedures and create DAL (Data Access Layer)
        private void GenerateDocumentForSps(string NameSpace, string Class, Providers languageProvider)
        {
            this.PrintToLog(string.Format("Generating some initialized code and methods ..."));
            System.Windows.Forms.Application.DoEvents();
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace ns = new CodeNamespace(NameSpace);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Text"));
            ns.Imports.Add(new CodeNamespaceImport("System.Data"));
            ns.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));

            // create our class
            CodeTypeDeclaration ctd = new CodeTypeDeclaration(Class);
            ctd.IsClass = true;
            ns.Types.Add(ctd);
            compileUnit.Namespaces.Add(ns);

            // create private field  for our class (connectionString)
            string strConnectionString = this.ToLowerFirstChar("connectionString");
            CodeMemberField class_field_connectionString = new CodeMemberField(typeof(string), strConnectionString);
            class_field_connectionString.Attributes = MemberAttributes.Private;
            ctd.Members.Add(class_field_connectionString);

            // encapsulate our field(connectionString)
            CodeMemberProperty propertyCon = new CodeMemberProperty();
            propertyCon.Name = this.ToUpperFirstChar(strConnectionString);
            propertyCon.Type = new CodeTypeReference(typeof(string));
            propertyCon.Attributes = MemberAttributes.Public;
            propertyCon.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), strConnectionString)));
            propertyCon.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), strConnectionString), new CodePropertySetValueReferenceExpression()));
            ctd.Members.Add(propertyCon);

            #region Create 3 constructor
            // create constructor for our class
            CodeConstructor constructor1 = new CodeConstructor();
            constructor1.Attributes = MemberAttributes.Public;
            ctd.Members.Add(constructor1);

            // add parameter to our constructor mothod
            CodeParameterDeclarationExpression constructor1_connectionString = new CodeParameterDeclarationExpression(class_field_connectionString.Type, strConnectionString);
            constructor1.Parameters.Add(constructor1_connectionString);

            // in body of constructor, assign value of parameter to field's property of class (ConnectionString)
            CodeAssignStatement fieldAssignment = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), strConnectionString), new CodeArgumentReferenceExpression(constructor1_connectionString.Name));
            constructor1.Statements.Add(fieldAssignment);

            // create constructor2 for our class with 2 args
            CodeConstructor constructor2 = new CodeConstructor();
            constructor2.Attributes = MemberAttributes.Public;
            CodeParameterDeclarationExpression constructor2_server = new CodeParameterDeclarationExpression(typeof(string), "server");
            CodeParameterDeclarationExpression constructor2_database = new CodeParameterDeclarationExpression(typeof(string), "database");
            constructor2.Parameters.Add(constructor2_server);
            constructor2.Parameters.Add(constructor2_database);
            CodeAssignStatement constructor2_assignment = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), class_field_connectionString.Name), new CodeSnippetExpression("string.Format(\"Server={0};Database={1};Trusted_Connection=True;\", server, database)"));
            constructor2.Statements.Add(constructor2_assignment);
            ctd.Members.Add(constructor2);

            // create constructor3 for our class with 4 args
            CodeConstructor constructor3 = new CodeConstructor();
            constructor3.Attributes = MemberAttributes.Public;
            CodeParameterDeclarationExpression constructor3_server = new CodeParameterDeclarationExpression(typeof(string), "server");
            CodeParameterDeclarationExpression constructor3_database = new CodeParameterDeclarationExpression(typeof(string), "database");
            CodeParameterDeclarationExpression constructor3_user = new CodeParameterDeclarationExpression(typeof(string), "user");
            CodeParameterDeclarationExpression constructor3_pass = new CodeParameterDeclarationExpression(typeof(string), "pass");
            constructor3.Parameters.Add(constructor3_server);
            constructor3.Parameters.Add(constructor3_database);
            constructor3.Parameters.Add(constructor3_user);
            constructor3.Parameters.Add(constructor3_pass);
            CodeAssignStatement constructor3_assignment = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), class_field_connectionString.Name), new CodeSnippetExpression("string.Format(\"Server={0};Database={1};User ID={2};Password={3};Trusted_Connection=False\", server, database,user,pass)"));
            constructor3.Statements.Add(constructor3_assignment);
            ctd.Members.Add(constructor3);
            #endregion

            #region Create some useful methods (GetData(SqlCommand cmd))
            // create method named GetData()
            CodeMemberMethod method_getData = new CodeMemberMethod();
            method_getData.Attributes = MemberAttributes.Public;
            method_getData.Name = "GetData";
            method_getData.ReturnType = new CodeTypeReference(typeof(DataTable));
            ctd.Members.Add(method_getData);

            // add a SqlCommand object as parameter in 'method_getData' method
            CodeParameterDeclarationExpression method_getData_p1 = new CodeParameterDeclarationExpression(typeof(System.Data.SqlClient.SqlCommand), "cmd");
            method_getData.Parameters.Add(method_getData_p1);

            // declare variable with type SqlConnection and assign value of field's property (ConnectionString) to it
            CodeVariableDeclarationStatement cvds_con_getData = new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlConnection), "con");
            method_getData.Statements.Add(cvds_con_getData);
            CodeAssignStatement assignment_newConnection_getData = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_con_getData.Name), new CodeObjectCreateExpression(typeof(System.Data.SqlClient.SqlConnection), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), propertyCon.Name)));
            method_getData.Statements.Add(assignment_newConnection_getData);

            // Assign con object to cmd.Connection property
            CodeAssignStatement assign_cmd_con_getData = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(method_getData_p1.Name), "Connection"), new CodeVariableReferenceExpression(cvds_con_getData.Name));
            method_getData.Statements.Add(assign_cmd_con_getData);

            // decalre a SqlDataReader object to hold data
            CodeVariableDeclarationStatement cvds_dr_getData = new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlDataReader), "dr");

            // declare a DataTable object to load data from SqlDataReader object
            CodeVariableDeclarationStatement cvds_dt_getData = new CodeVariableDeclarationStatement(typeof(System.Data.DataTable), "dt");
            CodeAssignStatement assignment_new_dt_getData = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_dt_getData.Name), new CodeObjectCreateExpression(typeof(System.Data.DataTable)));

            // open the SqlConnection object
            CodeMethodInvokeExpression invoke_con_open_getData = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_con_getData.Name), "Open");

            // run ExecuteReader method and pass resultset to SqlDataReader object
            CodeMethodInvokeExpression invoke_execute_reader_getData = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(method_getData_p1.Name), "ExecuteReader");
            CodeAssignStatement assignment_execReaderToReader_getData = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_dr_getData.Name), invoke_execute_reader_getData);

            CodeMethodInvokeExpression invoke_dt_load_getData = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_dt_getData.Name), "Load", new CodeVariableReferenceExpression(cvds_dr_getData.Name));

            // close SqlConnection object
            CodeMethodInvokeExpression invoke_con_close_getData = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_con_getData.Name), "Close");

            // return DataTable object
            CodeMethodReturnStatement return_method_getData = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(cvds_dt_getData.Name));

            CodeTryCatchFinallyStatement tryCatch_method_getData = new CodeTryCatchFinallyStatement();
            tryCatch_method_getData.TryStatements.Add(invoke_con_open_getData);
            tryCatch_method_getData.TryStatements.Add(cvds_dr_getData);
            tryCatch_method_getData.TryStatements.Add(cvds_dt_getData);
            tryCatch_method_getData.TryStatements.Add(assignment_new_dt_getData);
            tryCatch_method_getData.TryStatements.Add(assignment_execReaderToReader_getData);
            tryCatch_method_getData.TryStatements.Add(invoke_dt_load_getData);
            tryCatch_method_getData.TryStatements.Add(invoke_con_close_getData);
            tryCatch_method_getData.TryStatements.Add(return_method_getData);

            // catch 1 for sql errors (Note : this catch clause must define first)
            CodeCatchClause catchSql_getData = new CodeCatchClause("ex", new CodeTypeReference("System.Data.SqlClient.SqlException"));
            CodeMethodInvokeExpression error_showSql_getData = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("System.Windows.Forms.MessageBox"), "Show");
            CodeSnippetExpression error_messageSql_getData = new CodeSnippetExpression("ex.Message");
            CodeVariableDeclarationStatement titleSqlVar1_getData = new CodeVariableDeclarationStatement(typeof(string), "errorTitle", new CodePrimitiveExpression("Error "));
            CodeVariableDeclarationStatement titleSqlVar2_getData = new CodeVariableDeclarationStatement(typeof(string), "numError", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(catchSql_getData.LocalName), "Number.ToString"));
            CodeAssignStatement error_titleSqlAssignment_getData = new CodeAssignStatement(new CodeVariableReferenceExpression(titleSqlVar1_getData.Name), new CodeSnippetExpression(titleSqlVar1_getData.Name + " + " + titleSqlVar2_getData.Name));
            error_showSql_getData.Parameters.Add(error_messageSql_getData);
            error_showSql_getData.Parameters.Add(new CodeVariableReferenceExpression(titleSqlVar1_getData.Name));
            catchSql_getData.Statements.Add(titleSqlVar1_getData);
            catchSql_getData.Statements.Add(titleSqlVar2_getData);
            catchSql_getData.Statements.Add(error_titleSqlAssignment_getData);
            catchSql_getData.Statements.Add(error_showSql_getData);
            catchSql_getData.Statements.Add(new CodeSnippetExpression("return null"));
            tryCatch_method_getData.CatchClauses.Add(catchSql_getData);

            // catch 2 for general errors
            CodeCatchClause catchGeneral_getData = new CodeCatchClause("ex", new CodeTypeReference("System.Exception"));
            CodeMethodInvokeExpression error_showGeneral_getData = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("System.Windows.Forms.MessageBox"), "Show");
            CodeSnippetExpression error_messageGeneral_getData = new CodeSnippetExpression("ex.Message");
            CodePrimitiveExpression error_titleGeneral_getData = new CodePrimitiveExpression("Error!");
            error_showGeneral_getData.Parameters.Add(error_messageGeneral_getData);
            error_showGeneral_getData.Parameters.Add(error_titleGeneral_getData);
            catchGeneral_getData.Statements.Add(error_showGeneral_getData);
            catchGeneral_getData.Statements.Add(new CodeSnippetExpression("return null"));
            tryCatch_method_getData.CatchClauses.Add(catchGeneral_getData);
            method_getData.Statements.Add(tryCatch_method_getData);
            #endregion

            #region Create some useful methods (ExecNonQuery(SqlCommand cmd))
            // create method named ExecNonQuery
            CodeMemberMethod method_execNonQuery = new CodeMemberMethod();
            method_execNonQuery.Attributes = MemberAttributes.Public;
            method_execNonQuery.Name = "ExecNonQuery";
            method_execNonQuery.ReturnType = new CodeTypeReference(typeof(int));
            ctd.Members.Add(method_execNonQuery);

            // add a SqlCommand object as parameter in 'method_execNonQuery' method
            CodeParameterDeclarationExpression method_execNonQuery_p1 = new CodeParameterDeclarationExpression(typeof(System.Data.SqlClient.SqlCommand), "cmd");
            method_execNonQuery.Parameters.Add(method_execNonQuery_p1);

            // declare variable with type SqlConnection and assign value of field's property (ConnectionString) to it
            CodeVariableDeclarationStatement cvds_con_execNonQuery = new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlConnection), "con");
            method_execNonQuery.Statements.Add(cvds_con_execNonQuery);
            CodeAssignStatement assignment_execNonQuery = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_con_execNonQuery.Name), new CodeObjectCreateExpression(typeof(System.Data.SqlClient.SqlConnection), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), propertyCon.Name)));
            method_execNonQuery.Statements.Add(assignment_execNonQuery);

            // Assign con object to cmd.Connection property
            CodeAssignStatement assign_cmd_con_execNonQuery = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(method_execNonQuery_p1.Name), "Connection"), new CodeVariableReferenceExpression(cvds_con_execNonQuery.Name));
            method_execNonQuery.Statements.Add(assign_cmd_con_execNonQuery);

            // open the SqlConnection object
            CodeMethodInvokeExpression invoke_con_open_execNonQuery = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_con_execNonQuery.Name), "Open");

            // run ExecuteNoneQuery method and pass resultset to our int variable
            CodeVariableDeclarationStatement output_execNonQuery = new CodeVariableDeclarationStatement(typeof(int), "output");
            CodeMethodInvokeExpression invoke_executeNoneQuery_execNonQuery = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(method_execNonQuery_p1.Name), "ExecuteNonQuery");
            CodeAssignStatement assignment_noneQueryToOutput_execNonQuery = new CodeAssignStatement(new CodeVariableReferenceExpression(output_execNonQuery.Name), invoke_executeNoneQuery_execNonQuery);

            // close SqlConnection object
            CodeMethodInvokeExpression invoke_con_close_execNonQuery = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_con_execNonQuery.Name), "Close");

            // return DataTable object
            CodeMethodReturnStatement return_method_execNonQuery = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(output_execNonQuery.Name));

            CodeTryCatchFinallyStatement tryCatch_method_execNonQuery = new CodeTryCatchFinallyStatement();
            tryCatch_method_execNonQuery.TryStatements.Add(output_execNonQuery);
            tryCatch_method_execNonQuery.TryStatements.Add(invoke_con_open_execNonQuery);
            tryCatch_method_execNonQuery.TryStatements.Add(assignment_noneQueryToOutput_execNonQuery);
            tryCatch_method_execNonQuery.TryStatements.Add(invoke_con_close_execNonQuery);
            tryCatch_method_execNonQuery.TryStatements.Add(return_method_execNonQuery);

            // catch 1 for sql errors (Note : this catch clause must define first)
            CodeCatchClause catchSql_execNonQuery = new CodeCatchClause("ex", new CodeTypeReference("System.Data.SqlClient.SqlException"));
            CodeMethodInvokeExpression error_showSql_execNonQuery = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("System.Windows.Forms.MessageBox"), "Show");
            CodeSnippetExpression error_messageSql_execNonQuery = new CodeSnippetExpression("ex.Message");
            CodeVariableDeclarationStatement titleSqlVar1_execNonQuery = new CodeVariableDeclarationStatement(typeof(string), "errorTitle", new CodePrimitiveExpression("Error "));
            CodeVariableDeclarationStatement titleSqlVar2_execNonQuery = new CodeVariableDeclarationStatement(typeof(string), "numError", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(catchSql_execNonQuery.LocalName), "Number.ToString"));
            CodeAssignStatement error_titleSqlAssignment_execNonQuery = new CodeAssignStatement(new CodeVariableReferenceExpression(titleSqlVar1_execNonQuery.Name), new CodeSnippetExpression(titleSqlVar1_execNonQuery.Name + " + " + titleSqlVar2_execNonQuery.Name));
            error_showSql_execNonQuery.Parameters.Add(error_messageSql_execNonQuery);
            error_showSql_execNonQuery.Parameters.Add(new CodeVariableReferenceExpression(titleSqlVar1_execNonQuery.Name));
            catchSql_execNonQuery.Statements.Add(titleSqlVar1_execNonQuery);
            catchSql_execNonQuery.Statements.Add(titleSqlVar2_execNonQuery);
            catchSql_execNonQuery.Statements.Add(error_titleSqlAssignment_execNonQuery);
            catchSql_execNonQuery.Statements.Add(error_showSql_execNonQuery);
            catchSql_execNonQuery.Statements.Add(new CodeSnippetExpression("return -1"));
            tryCatch_method_execNonQuery.CatchClauses.Add(catchSql_execNonQuery);

            // catch 2 for general errors
            CodeCatchClause catchGeneral_execNonQuery = new CodeCatchClause("ex", new CodeTypeReference("System.Exception"));
            CodeMethodInvokeExpression error_showGeneral_execNonQuery = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("System.Windows.Forms.MessageBox"), "Show");
            CodeSnippetExpression error_messageGeneral_execNonQuery = new CodeSnippetExpression("ex.Message");
            CodePrimitiveExpression error_titleGeneral_execNonQuery = new CodePrimitiveExpression("Error!");
            error_showGeneral_execNonQuery.Parameters.Add(error_messageGeneral_execNonQuery);
            error_showGeneral_execNonQuery.Parameters.Add(error_titleGeneral_execNonQuery);
            catchGeneral_execNonQuery.Statements.Add(error_showGeneral_execNonQuery);
            catchGeneral_execNonQuery.Statements.Add(new CodeSnippetExpression("return -1"));
            tryCatch_method_execNonQuery.CatchClauses.Add(catchGeneral_execNonQuery);
            method_execNonQuery.Statements.Add(tryCatch_method_execNonQuery);
            #endregion

            #region Retrieve storedProcedure(s) and create method for it
            // create new DataTable object (dtHelper) to import unique records from dtSPs
            // because in dtSPs, each stored Procedure + parameter has a single record, therefore, each sp that has a multiple parameters
            // cause multiple rows insert into dtSPs object
            DataTable dtHelper = this.dtSPs.Clone();
            dtHelper.PrimaryKey = new DataColumn[] { dtHelper.Columns[0] };
            foreach (DataRow row in this.dtSPs.Rows)
            {
                if (!dtHelper.Rows.Contains(row["SpName"]))
                {
                    dtHelper.ImportRow(row);
                    DataRow[] rowCol = this.dtSPs.Select("SpName = '" + row["SpName"].ToString() + "'");
                    StoredProcedureParameterCollection parameterCollection = new StoredProcedureParameterCollection();
                    foreach (DataRow rowParameter in rowCol)
                    {
                        StoredProcedureParameter parameter = new StoredProcedureParameter();
                        parameter.ParameterName = rowParameter["ParameterName"].ToString();
                        parameter.ParameterType = rowParameter["SystemType"].ToString();
                        parameter.ParameterLength = rowParameter["Length"].ToString() != "NULL" ? int.Parse(rowParameter["Length"].ToString()) : -1;
                        if (rowParameter["IsOutputParameter"].ToString() != "NULL")
                        {
                            if (rowParameter["IsOutputParameter"].ToString() == "1")
                                parameter.IsParameterOutput = true;
                            else
                                parameter.IsParameterOutput = false;
                        }
                        else
                        {
                            parameter.IsParameterOutput = false;
                        }
                        parameterCollection.Add(parameter);
                    }

                    this.PrintToLog(string.Format("Generating method for {0} ...", row["SpName"]));
                    System.Windows.Forms.Application.DoEvents();
                    // create method for each storedProcedure in this loop and pass parameters to this method as StoredProcedureParameterCollection object
                    this.CreateMethodForExecuteSP(ctd, row["SpName"].ToString(), strConnectionString, parameterCollection);
                }
            }
            #endregion

            // define language provider and generate code
            CodeDomProvider provider;
            if (languageProvider == Providers.CsProvider)
            {
                // output is in csharp
                provider = new CSharpCodeProvider();
            }
            else
            {
                // output is in viusal basic
                provider = new VBCodeProvider();
            }
            // finally, generate our code to specified codeProvider
            this.GenerateCode(provider, compileUnit, this.txtSaveTo.Text + "\\" + Class);

            this.PrintToLog(string.Format("Code generation successfully completed for {0} Stored Procedure(s) in Database : {1}", dtHelper.Rows.Count, NameSpace));
        }

        // create document code for each stored procedure that retrieve from database
        private void CreateMethodForExecuteSP(CodeTypeDeclaration ctd, string spName, string connectionString, StoredProcedureParameterCollection parameterCollection)
        {
            // declaration method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(System.Data.DataTable));
            method.Name = spName;

            // declare variable with type SqlConnection and assign value of field's property (ConnectionString) to it
            CodeVariableDeclarationStatement cvds_con = new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlConnection), "con");
            CodeAssignStatement assignment_new_con = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_con.Name), new CodeObjectCreateExpression(typeof(System.Data.SqlClient.SqlConnection), new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "ConnectionString")));

            // declare variable with type SqlCommand and assign value of cvds_con to SqlCommand.Connection property
            CodeVariableDeclarationStatement cvds_cmd = new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlCommand), "cmd");
            CodeAssignStatement assignment_new_cmd = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_cmd.Name), new CodeObjectCreateExpression(typeof(System.Data.SqlClient.SqlCommand)));
            CodeAssignStatement assignment_cmd_commandText = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "CommandText"), new CodePrimitiveExpression(spName));
            CodeAssignStatement assignment_cmd_commandType = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "CommandType"), new CodeSnippetExpression("CommandType.StoredProcedure"));

            // Assign con object to cmd.Connection property
            CodeAssignStatement assign_cmd_con = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "Connection"), new CodeVariableReferenceExpression(cvds_con.Name));

            // decalre a SqlDataReader object to hold data
            CodeVariableDeclarationStatement cvds_dr = new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlDataReader), "dr");

            // declare a DataTable object to load data from SqlDataReader object
            CodeVariableDeclarationStatement cvds_dt = new CodeVariableDeclarationStatement(typeof(System.Data.DataTable), "dt");
            CodeAssignStatement assignment_new_dt = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_dt.Name), new CodeObjectCreateExpression(typeof(System.Data.DataTable)));

            // open the SqlConnection object
            CodeMethodInvokeExpression invoke_con_open = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_con.Name), "Open");

            // run ExecuteReader method and pass resultset to SqlDataReader object
            CodeMethodInvokeExpression invoke_execute_reader = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "ExecuteReader");
            CodeAssignStatement assignment_dr_cmdExecute = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_dr.Name), invoke_execute_reader);

            // load data from SqlDataReader to DataTable object
            CodeMethodInvokeExpression dt_load = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_dt.Name), "Load", new CodeVariableReferenceExpression(cvds_dr.Name));

            // close SqlConnection object
            CodeMethodInvokeExpression invoke_con_close = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_con.Name), "Close");

            // return DataTable object
            CodeMethodReturnStatement return_method = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(cvds_dt.Name));

            // insert our method statements into TryCatch clause
            CodeTryCatchFinallyStatement tryCatch_method = new CodeTryCatchFinallyStatement();

            tryCatch_method.TryStatements.Add(cvds_con);
            tryCatch_method.TryStatements.Add(assignment_new_con);
            tryCatch_method.TryStatements.Add(cvds_cmd);
            tryCatch_method.TryStatements.Add(assignment_new_cmd);
            tryCatch_method.TryStatements.Add(assignment_cmd_commandText);
            tryCatch_method.TryStatements.Add(assignment_cmd_commandType);
            tryCatch_method.TryStatements.Add(assign_cmd_con);

            if (parameterCollection.Count > 0)
            {
                foreach (StoredProcedureParameter parameter in parameterCollection)
                {
                    if (parameter.ParameterName != "NULL")
                    {
                        CodeParameterDeclarationExpression method_parameter = new CodeParameterDeclarationExpression(this.GetParameterType(parameter), parameter.ParameterName.Replace("@", ""));
                        method.Parameters.Add(method_parameter);

                        CodeVariableDeclarationStatement commandParameter = new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlParameter), "_" + parameter.ParameterName.Replace("@", ""), new CodeObjectCreateExpression(typeof(System.Data.SqlClient.SqlParameter)));
                        CodeAssignStatement assign_parameter_name = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "ParameterName"), new CodePrimitiveExpression(parameter.ParameterName));
                        CodeAssignStatement assign_parameter_size = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "Size"), new CodeSnippetExpression(parameter.ParameterLength.ToString()));
                        CodeAssignStatement assign_parameter_value = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "Value"), new CodeArgumentReferenceExpression(method_parameter.Name));
                        CodeMethodInvokeExpression add_parameter = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "Parameters.Add", new CodeVariableReferenceExpression(commandParameter.Name));
                        tryCatch_method.TryStatements.Add(commandParameter);
                        tryCatch_method.TryStatements.Add(assign_parameter_name);
                        tryCatch_method.TryStatements.Add(assign_parameter_size);
                        tryCatch_method.TryStatements.Add(assign_parameter_value);
                        tryCatch_method.TryStatements.Add(add_parameter);

                        if (parameter.IsParameterOutput)
                        {
                            CodeAssignStatement assign_parameter_direction = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "Direction"), new CodeSnippetExpression("ParameterDirection.Output"));
                            tryCatch_method.TryStatements.Add(assign_parameter_direction);
                        }
                    }
                }
            }

            tryCatch_method.TryStatements.Add(cvds_dr);
            tryCatch_method.TryStatements.Add(cvds_dt);
            tryCatch_method.TryStatements.Add(assignment_new_dt);
            tryCatch_method.TryStatements.Add(invoke_con_open);
            tryCatch_method.TryStatements.Add(assignment_dr_cmdExecute);
            tryCatch_method.TryStatements.Add(dt_load);
            tryCatch_method.TryStatements.Add(invoke_con_close);
            tryCatch_method.TryStatements.Add(return_method);

            // catch 1 for sql errors (Note : this catch clause must define first)
            CodeCatchClause catchSql = new CodeCatchClause("ex", new CodeTypeReference("System.Data.SqlClient.SqlException"));
            CodeMethodInvokeExpression error_showSql = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("System.Windows.Forms.MessageBox"), "Show");
            CodeSnippetExpression error_messageSql = new CodeSnippetExpression("ex.Message");
            CodeVariableDeclarationStatement titleSqlVar1 = new CodeVariableDeclarationStatement(typeof(string), "errorTitle", new CodePrimitiveExpression("Error "));
            CodeVariableDeclarationStatement titleSqlVar2 = new CodeVariableDeclarationStatement(typeof(string), "numError", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(catchSql.LocalName), "Number.ToString"));
            CodeAssignStatement error_titleSqlAssignment = new CodeAssignStatement(new CodeVariableReferenceExpression(titleSqlVar1.Name), new CodeSnippetExpression(titleSqlVar1.Name + " + " + titleSqlVar2.Name));
            error_showSql.Parameters.Add(error_messageSql);
            error_showSql.Parameters.Add(new CodeVariableReferenceExpression(titleSqlVar1.Name));
            catchSql.Statements.Add(titleSqlVar1);
            catchSql.Statements.Add(titleSqlVar2);
            catchSql.Statements.Add(error_titleSqlAssignment);
            catchSql.Statements.Add(error_showSql);
            catchSql.Statements.Add(new CodeSnippetExpression("return null"));
            tryCatch_method.CatchClauses.Add(catchSql);

            // catch 2 for general errors
            CodeCatchClause catchGeneral = new CodeCatchClause("ex", new CodeTypeReference("System.Exception"));
            CodeMethodInvokeExpression error_showGeneral = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("System.Windows.Forms.MessageBox"), "Show");
            CodeSnippetExpression error_messageGeneral = new CodeSnippetExpression("ex.Message");
            CodePrimitiveExpression error_titleGeneral = new CodePrimitiveExpression("Error!");
            error_showGeneral.Parameters.Add(error_messageGeneral);
            error_showGeneral.Parameters.Add(error_titleGeneral);
            catchGeneral.Statements.Add(error_showGeneral);
            catchGeneral.Statements.Add(new CodeSnippetExpression("return null"));
            tryCatch_method.CatchClauses.Add(catchGeneral);

            // add try-catch to our method
            method.Statements.Add(tryCatch_method);

            // add method to the class
            ctd.Members.Add(method);
        }

        // get type of each stored procedure parameter
        private Type GetParameterType(StoredProcedureParameter item)
        {
            Type fieldType;
            if (item.ParameterType.ToLower() == SqlDbType.TinyInt.ToString().ToLower())
                fieldType = typeof(System.Byte);
            else if (item.ParameterType.ToLower() == SqlDbType.SmallInt.ToString().ToLower())
                fieldType = typeof(System.Int16);
            else if (item.ParameterType.ToLower() == SqlDbType.Int.ToString().ToLower())
                fieldType = typeof(System.Int32);
            else if (item.ParameterType.ToLower() == SqlDbType.BigInt.ToString().ToLower())
                fieldType = typeof(System.Int64);
            else if (item.ParameterType.ToLower() == SqlDbType.Money.ToString().ToLower())
                fieldType = typeof(System.Single);
            else if (item.ParameterType.ToLower() == SqlDbType.Float.ToString().ToLower())
                fieldType = typeof(System.Double);
            else if (item.ParameterType.ToLower() == SqlDbType.Char.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.NChar.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.VarChar.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.NVarChar.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.Text.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.NText.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.Xml.ToString().ToLower())
                fieldType = typeof(System.String);
            else if (item.ParameterType.ToLower() == SqlDbType.Decimal.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.Real.ToString().ToLower())
                fieldType = typeof(System.Decimal);
            else if (item.ParameterType.ToLower() == SqlDbType.Image.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.VarBinary.ToString().ToLower())
                fieldType = typeof(System.Byte[]);
            else if (item.ParameterType.ToLower() == SqlDbType.SmallDateTime.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.Date.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.DateTime.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.DateTime2.ToString().ToLower())
                fieldType = typeof(System.DateTime);
            else if (item.ParameterType.ToLower() == SqlDbType.DateTimeOffset.ToString().ToLower())
                fieldType = typeof(System.DateTimeOffset);
            else if (item.ParameterType.ToLower() == SqlDbType.Bit.ToString().ToLower())
                fieldType = typeof(System.Boolean);
            else if (item.ParameterType.ToLower() == SqlDbType.UniqueIdentifier.ToString().ToLower())
                fieldType = typeof(System.Guid);
            else
                fieldType = typeof(System.Object);
            return fieldType;
        }

        // generate sql script (Stored Procedures)
        private string GenerateSQL(string DatabaseName, string tableName, DataRow[] rows)
        {
            string Prefix = "SP_";//Session.LoadFromSession("Prefix").ToString();
            string reult = string.Empty;

            bool GenerateSelectAll = bool.Parse(Session.LoadFromSession("GenerateSelectAllProc").ToString());
            bool GenerateSelectRow = bool.Parse(Session.LoadFromSession("GenerateSelectRowProc").ToString());
            bool GenerateInsert = bool.Parse(Session.LoadFromSession("GenerateInsertProc").ToString());
            bool GenerateUpdate = bool.Parse(Session.LoadFromSession("GenerateUpdateProc").ToString());
            bool GenerateDelete = bool.Parse(Session.LoadFromSession("GenerateDeleteRowProc").ToString());

            if (GenerateSelectAll)
            {
                reult = SQL_Generator.CreateSelectAllSP(Prefix + tableName + "_SelectAll", tableName, rows);
                reult += "\r\n\r\n";
            }

            if (GenerateSelectRow)
            {
                reult += SQL_Generator.CreateSelectRowSP(Prefix + tableName + "_SelectRow", tableName, rows);
                reult += "\r\n\r\n";
            }

            if (GenerateInsert)
            {
                reult += SQL_Generator.CreateInsertSP(Prefix + tableName + "_Insert", tableName, rows);
                reult += "\r\n\r\n";
            }

            if (GenerateUpdate)
            {
                reult += SQL_Generator.CreateUpdateSP(Prefix + tableName + "_Update", tableName, rows);
                reult += "\r\n\r\n";
            }

            if (GenerateDelete)
            {
                reult += SQL_Generator.CreateDeleteRowSP(Prefix + tableName + "_DeleteRow", tableName, rows);
            }
            return reult;
        }

        // generate document via CodeDom
        private void GenerateDocumentForCode(string NameSpace, string Class, List<CodeDomDatabaseSQLDMO.Column> Fields)
        {
            Class = this.ToUpperFirstChar(Class);            

            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace ns = new CodeNamespace(NameSpace);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Text"));
            ns.Imports.Add(new CodeNamespaceImport("System.Data"));
            ns.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));

            CodeTypeDeclaration ctd = new CodeTypeDeclaration(Class);
            ctd.IsClass = true;
            ns.Types.Add(ctd);
            compileUnit.Namespaces.Add(ns);

            // create constructor for our class
            CodeConstructor constructor1 = new CodeConstructor();
            constructor1.Attributes = MemberAttributes.Public;
            ctd.Members.Add(constructor1);

            foreach (CodeDomDatabaseSQLDMO.Column item in Fields)
            {
                // create private field                
                string fieldName = this.ToLowerFirstChar(item.ColumnName);
                CodeMemberField field = new CodeMemberField(this.GetColumnType(item), fieldName);
                field.Attributes = MemberAttributes.Private;
                ctd.Members.Add(field);

                // encapsulate our field(s) 
                CodeMemberProperty property = new CodeMemberProperty();
                property.Name = this.ToUpperFirstChar(item.ColumnName);
                property.Type = new CodeTypeReference(this.GetColumnType(item));
                property.Attributes = MemberAttributes.Public;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));
                ctd.Members.Add(property);
            }

            // Generate Collection Class for our class
            this.GenerateCollectionClass(ns, Class);

            // define language provider and generate code
            CodeDomProvider provider;
            if (this.cmbLanguage.Text == "C#")
            {
                provider = new CSharpCodeProvider();
                DirectoryInfo dirInfo = Directory.CreateDirectory(this.txtSaveTo.Text + "\\ BLL");
                string fileName;
                fileName = dirInfo.FullName + "\\" + NameSpace;
                this.GenerateCode(provider, compileUnit,fileName);
            }
            else if (this.cmbLanguage.Text == "VB")
            {
                provider = new VBCodeProvider();
                DirectoryInfo dirInfo = Directory.CreateDirectory(this.txtSaveTo.Text + "\\ BLL");
                string fileName;
                fileName = dirInfo.FullName + "\\" + NameSpace;
                this.GenerateCode(provider, compileUnit, fileName);
            }            
        }

        // get type of each column
        private Type GetColumnType(CodeDomDatabaseSQLDMO.Column item)
        {
            Type fieldType;
            if (item.ColumnType.ToLower() == SqlDbType.TinyInt.ToString().ToLower())
                fieldType = typeof(System.Byte);
            else if (item.ColumnType.ToLower() == SqlDbType.SmallInt.ToString().ToLower())
                fieldType = typeof(System.Int16);
            else if (item.ColumnType.ToLower() == SqlDbType.Int.ToString().ToLower())
                fieldType = typeof(System.Int32);
            else if (item.ColumnType.ToLower() == SqlDbType.BigInt.ToString().ToLower())
                fieldType = typeof(System.Int64);
            else if (item.ColumnType.ToLower() == SqlDbType.Money.ToString().ToLower())
                fieldType = typeof(System.Single);
            else if (item.ColumnType.ToLower() == SqlDbType.Float.ToString().ToLower())
                fieldType = typeof(System.Double);
            else if (item.ColumnType.ToLower() == SqlDbType.Char.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.NChar.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.VarChar.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.NVarChar.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.Text.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.NText.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.Xml.ToString().ToLower())
                fieldType = typeof(System.String);
            else if (item.ColumnType.ToLower() == SqlDbType.Decimal.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.Real.ToString().ToLower())
                fieldType = typeof(System.Decimal);
            else if (item.ColumnType.ToLower() == SqlDbType.Image.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.VarBinary.ToString().ToLower())
                fieldType = typeof(System.Byte[]);
            else if (item.ColumnType.ToLower() == SqlDbType.SmallDateTime.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.Date.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.DateTime.ToString().ToLower() || item.ColumnType.ToLower() == SqlDbType.DateTime2.ToString().ToLower())
                fieldType = typeof(System.DateTime);
            else if (item.ColumnType.ToLower() == SqlDbType.DateTimeOffset.ToString().ToLower())
                fieldType = typeof(System.DateTimeOffset);
            else if (item.ColumnType.ToLower() == SqlDbType.Bit.ToString().ToLower())
                fieldType = typeof(System.Boolean);
            else if (item.ColumnType.ToLower() == SqlDbType.UniqueIdentifier.ToString().ToLower())
                fieldType = typeof(System.Guid);
            else
                fieldType = typeof(System.Object);
            return fieldType;
        }

        // generate code to save on disk
        public void GenerateCode(CodeDomProvider provider, CodeCompileUnit compileUnit, string fileName)
        {
            // Build the source file name with the appropriate
            // language extension.
            String sourceFile;
            if (provider.FileExtension[0] == '.')
            {
                sourceFile = fileName + provider.FileExtension;
            }
            else
            {
                sourceFile = fileName + "." + provider.FileExtension;
            }
            
            // Create an IndentedTextWriter, constructed with
            // a StreamWriter to the source file.
            IndentedTextWriter tw = new IndentedTextWriter(new StreamWriter(sourceFile, true), "    ");

            // Generate source code using the code generator.
            provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());

            // Close the output file.
            tw.Close();            
        }

        // Generate Collection Class
        public void GenerateCollectionClass(CodeNamespace ns, string Class)
        {
            CodeTypeDeclaration ctdCollection = new CodeTypeDeclaration(Class + "Collection");
            ctdCollection.IsClass = true;
            ctdCollection.BaseTypes.Add(new CodeTypeReference("System.Collections.CollectionBase"));
            ns.Types.Add(ctdCollection);

            // create Add method for Collection
            CodeMemberMethod method_add = new CodeMemberMethod();
            method_add.Attributes = MemberAttributes.Public;
            method_add.Name = "Add";
            method_add.Parameters.Add(new CodeParameterDeclarationExpression(Class, "Item"));
            method_add.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("List"), "Add", new CodeVariableReferenceExpression("Item")));
            ctdCollection.Members.Add(method_add);

            // create Remove method for Collection
            CodeMemberMethod method_Remove = new CodeMemberMethod();
            method_Remove.Attributes = MemberAttributes.Public;
            method_Remove.Name = "Remove";
            method_Remove.Parameters.Add(new CodeParameterDeclarationExpression(Class, "Item"));
            method_Remove.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("List"), "Remove", new CodeVariableReferenceExpression("Item")));
            ctdCollection.Members.Add(method_Remove);

            // create indexer property for collection
            CodeMemberProperty ctdCollection_property = new CodeMemberProperty();
            ctdCollection_property.Type = new CodeTypeReference(Class);
            ctdCollection_property.Attributes = MemberAttributes.Public;
            ctdCollection_property.Name = "Item"; // Item is special name that use in create indexer property
            ctdCollection_property.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
            ctdCollection_property.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(Class, new CodeIndexerExpression(new CodeVariableReferenceExpression("List"), new CodeArgumentReferenceExpression("index")))));
            ctdCollection_property.SetStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("List"), new CodeArgumentReferenceExpression("index")), new CodePropertySetValueReferenceExpression()));
            ctdCollection.Members.Add(ctdCollection_property);
        }

        // load setting at startUp
        private void LoadSettings()
        {
            ConfigTemplate ct = ConfigUtils.ReadConfig();


            Session.SaveInSession("Prefix", ct.StoredProceduresPrefix);
            Session.SaveInSession("AuthorName", ct.AuthorName);
            Session.SaveInSession("NullParamDefaultValues", ct.PassNullAsDefaultParamaeterValue);
            Session.SaveInSession("GenerateWrapperClass", ct.GenerateWrapperClass);
            Session.SaveInSession("WrapperClass_GenerateStaticMethods", ct.WrapperClass_GenerateStaticMethods);
            Session.SaveInSession("WrapperClass_NameSpace", ct.WrapperClass_NameSpace);

            Session.SaveInSession("AutoSaveWrapperClass", ct.AutoSaveWrapperClass);
            Session.SaveInSession("AutoSaveScript", ct.AutoSaveScript);

            Session.SaveInSession("GenerateDeleteRowProc", ct.GenerateDeleteRowProc);
            Session.SaveInSession("GenerateInsertProc", ct.GenerateInsertProc);
            Session.SaveInSession("GenerateSelectAllProc", ct.GenerateSelectAllProc);
            Session.SaveInSession("GenerateSelectRowProc", ct.GenerateSelectRowProc);
            Session.SaveInSession("GenerateUpdateProc", ct.GenerateUpdateProc);
            this.radioGenSp.Checked = true;
        }

        // set first charachter to LowerCase 
        private string ToLowerFirstChar(string text)
        {
            char[] chars = text.ToCharArray();
            string s = chars[0].ToString();
            s = s.ToLower();
            chars[0] = s[0];
            StringBuilder sb = new StringBuilder();
            foreach (char ch in chars)
            {
                sb.Append(ch);
            }
            return sb.ToString();
        }

        // set first charachter to UpperCase 
        private string ToUpperFirstChar(string text)
        {
            char[] chars = text.ToCharArray();
            string s = chars[0].ToString();
            s = s.ToUpper();
            chars[0] = s[0];
            StringBuilder sb = new StringBuilder();
            foreach (char ch in chars)
            {
                sb.Append(ch);
            }
            return sb.ToString();
        }                    
    }
}