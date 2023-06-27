namespace WinFormsXpoNet
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.labelCompanies = new System.Windows.Forms.ToolStripLabel();
            this.comboCompanies = new System.Windows.Forms.ToolStripComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageObjects = new System.Windows.Forms.TabPage();
            this.gridDataObjects = new System.Windows.Forms.DataGridView();
            this.tableNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShowData = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bindingSourceCompanyObjects = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageObjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDataObjects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceCompanyObjects)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonConnect,
            this.toolStripSeparator1,
            this.labelCompanies,
            this.comboCompanies});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1317, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonConnect
            // 
            this.buttonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.buttonConnect.Image = ((System.Drawing.Image)(resources.GetObject("buttonConnect.Image")));
            this.buttonConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(134, 22);
            this.buttonConnect.Text = "Verbindung herstellen";
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnectionSettings_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // labelCompanies
            // 
            this.labelCompanies.Name = "labelCompanies";
            this.labelCompanies.Size = new System.Drawing.Size(71, 22);
            this.labelCompanies.Text = "Mandanten:";
            // 
            // comboCompanies
            // 
            this.comboCompanies.Enabled = false;
            this.comboCompanies.Name = "comboCompanies";
            this.comboCompanies.Size = new System.Drawing.Size(121, 25);
            this.comboCompanies.Sorted = true;
            this.comboCompanies.SelectedIndexChanged += new System.EventHandler(this.comboCompanies_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageObjects);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1317, 425);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPageObjects
            // 
            this.tabPageObjects.Controls.Add(this.gridDataObjects);
            this.tabPageObjects.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabPageObjects.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPageObjects.Location = new System.Drawing.Point(4, 24);
            this.tabPageObjects.Name = "tabPageObjects";
            this.tabPageObjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageObjects.Size = new System.Drawing.Size(1309, 397);
            this.tabPageObjects.TabIndex = 0;
            this.tabPageObjects.Text = "Datenobjekte";
            // 
            // gridDataObjects
            // 
            this.gridDataObjects.AllowUserToAddRows = false;
            this.gridDataObjects.AllowUserToDeleteRows = false;
            this.gridDataObjects.AllowUserToOrderColumns = true;
            this.gridDataObjects.AutoGenerateColumns = false;
            this.gridDataObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridDataObjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tableNameDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.columnCountDataGridViewTextBoxColumn,
            this.ShowData});
            this.gridDataObjects.DataSource = this.bindingSourceCompanyObjects;
            this.gridDataObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDataObjects.Location = new System.Drawing.Point(3, 3);
            this.gridDataObjects.MultiSelect = false;
            this.gridDataObjects.Name = "gridDataObjects";
            this.gridDataObjects.ReadOnly = true;
            this.gridDataObjects.RowTemplate.Height = 25;
            this.gridDataObjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDataObjects.Size = new System.Drawing.Size(1303, 391);
            this.gridDataObjects.TabIndex = 0;
            this.gridDataObjects.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDataObjects_CellContentClick);
            // 
            // tableNameDataGridViewTextBoxColumn
            // 
            this.tableNameDataGridViewTextBoxColumn.DataPropertyName = "TableName";
            this.tableNameDataGridViewTextBoxColumn.HeaderText = "Tabellenname";
            this.tableNameDataGridViewTextBoxColumn.Name = "tableNameDataGridViewTextBoxColumn";
            this.tableNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 250;
            // 
            // columnCountDataGridViewTextBoxColumn
            // 
            this.columnCountDataGridViewTextBoxColumn.DataPropertyName = "ColumnCount";
            this.columnCountDataGridViewTextBoxColumn.HeaderText = "Anzahl Spalten";
            this.columnCountDataGridViewTextBoxColumn.Name = "columnCountDataGridViewTextBoxColumn";
            this.columnCountDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ShowData
            // 
            this.ShowData.HeaderText = "Daten anzeigen";
            this.ShowData.Name = "ShowData";
            this.ShowData.ReadOnly = true;
            this.ShowData.Text = "Daten anzeigen";
            this.ShowData.UseColumnTextForButtonValue = true;
            // 
            // bindingSourceCompanyObjects
            // 
            this.bindingSourceCompanyObjects.DataSource = typeof(WinFormsXpoNet.CompanyObjectInfo);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 450);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormMain";
            this.Text = "WinForms Xpo";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageObjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDataObjects)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceCompanyObjects)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripComboBox comboCompanies;
        private TabControl tabControl1;
        private TabPage tabPageObjects;
        private DataGridView gridDataObjects;
        private ToolStripLabel labelCompanies;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton buttonConnect;
        private BindingSource bindingSourceCompanyObjects;
        private DataGridViewTextBoxColumn tableNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn columnCountDataGridViewTextBoxColumn;
        private DataGridViewButtonColumn ShowData;
    }
}