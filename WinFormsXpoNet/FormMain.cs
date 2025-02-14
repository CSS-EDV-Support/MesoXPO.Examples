using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using MesoXPO;
using MesoXPO.Models;
using MesoXPO.Models.Base;
using MesoXPO.Models.SystemData;
using System.Reflection;

namespace WinFormsXpoNet;

public record CompanyObjectInfo
{
    public CompanyObjectInfo(CompanyBase mesoBase)
    {
        TableName = mesoBase.PhysicalTableName();
        Name = mesoBase.GetType().Name;
        XpClassInfo = mesoBase.GetXPClassInfo();
        ColumnCount = ((List<XPMemberInfo>)XpClassInfo.PersistentProperties).Count;
        CompanyBaseObject = mesoBase;
    }

    public XPClassInfo XpClassInfo { get; init; }

    public string TableName { get; init; }
    public string Name { get; init; }
    public int ColumnCount { get; init; }
    public CompanyBase CompanyBaseObject { get; init; }
}

public partial class FormMain : Form
{
    public FormMain()
    {
        InitializeComponent();
    }

    MesoDataUnitOfWork CompanyConnection { get; set; }

    SystemUnitOfWork SystemConnection { get; set; }

    void comboCompanies_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(comboCompanies.SelectedItem.ToString()))
        {
            return;
        }

        // Mandantenverbindung
        CompanyConnection = SystemConnection.GetDataUnitOfWorkForCompany(comboCompanies.SelectedItem.ToString());

        var mesoBaseClasses = Assembly.GetAssembly(typeof(Mandantenstamm)).GetTypes()
            .Where(type => type.IsSubclassOf(typeof(CompanyBase)) && !type.IsAbstract)
            .Select(type => Activator.CreateInstance(type) as CompanyBase);

        var objectInfos = mesoBaseClasses.Select(t => new CompanyObjectInfo(t)).OrderBy(t => t.TableName).ToList();
        bindingSourceCompanyObjects.DataSource = objectInfos;
        gridDataObjects.DataSource = bindingSourceCompanyObjects;
    }

    void gridDataObjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        var senderGrid = (DataGridView)sender;

        if (senderGrid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn ||
            e.RowIndex < 0)
        {
            return;
        }

        try
        {
            if (bindingSourceCompanyObjects.Current is not CompanyObjectInfo currentCompanyObject)
            {
                return;
            }

            var asmMesoXPO = Assembly.GetAssembly(typeof(Mandantenstamm));
            var className = currentCompanyObject.Name;
            var wlXpoType = asmMesoXPO.GetTypes()
                .FirstOrDefault(t => t.Name.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            if (wlXpoType == null)
            {
                MessageBox.Show($"Meso-XPO-Klasse für {className} konnte nicht geladen werden");
                return;
            }

            var data = new XPCollection(CompanyConnection, wlXpoType);
            AddTabWithData($"{className} in {CompanyConnection.CompanyNr}", data);
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Error while loading persistent object. {exception.Message}");
        }
    }


    void AddTabWithData(string formCaption, object data)
    {
        if (data == null)
        {
            return;
        }

        try
        {
            var newTab = new TabPage(formCaption);

            var gridControl = new DataGridView();
            newTab.Controls.Add(gridControl);
            gridControl.Dock = DockStyle.Fill;
            gridControl.BringToFront();
            gridControl.ReadOnly = true;
            gridControl.DataSource = data;
            tabControl1.TabPages.Add(newTab);
            tabControl1.SelectedTab = newTab;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while adding Tab with Data. {ex.Message}");
        }
    }

    void ShowGridFormWithData(string formCaption, object data)
    {
        if (data == null)
        {
            return;
        }

        try
        {
            using var xtraForm = new Form();
            xtraForm.Text = formCaption;
            var gridControl = new DataGridView();
            xtraForm.Controls.Add(gridControl);
            gridControl.Dock = DockStyle.Fill;
            gridControl.BringToFront();
            gridControl.ReadOnly = true;
            gridControl.DataSource = data;
            xtraForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while showing GridForm with Data. {ex.Message}");
        }
    }

    void buttonConnectionSettings_Click(object sender, EventArgs e)
    {
        try
        {
            using var sqlConnectionDialog = new SqlConnectionDialog();
            var result = sqlConnectionDialog.ShowDialog(this);
            if (result != DialogResult.OK)
            {
                return;
            }

            // Systemverbindung
            SystemConnection = new SystemUnitOfWork(sqlConnectionDialog.ConnectionString);

            // optional: statische Systemverbindung für Zugriff aus den Datenobjekten (z.B. auf Archivdokumente, Eigenschaften etc., die nicht in derselben Datenbank und daher nicht im selben DataLayer liegen)
            SystemUnitOfWork.SessionSystem = SystemConnection;

            var companies = new XPQuery<Datenbankverbindung>(SystemConnection)
                .Select(d => d.Mandant);

            comboCompanies.Items.Clear();
            comboCompanies.Items.AddRange(companies.ToArray());
            comboCompanies.Enabled = true;
            comboCompanies.DroppedDown = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while establishing connection. {ex.Message}");
        }
    }
}
