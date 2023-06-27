using System.ComponentModel;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace WinFormsXpoNet;

public partial class SqlConnectionDialog : Form
{
    private readonly string IsValidPropertyName = "IsValid";

    public SqlConnectionDialog()
    {
        InitializeComponent();
        SetupControls();
    }


    public string ServerName
    {
        get => ServerNameTextBox.Text;
        set
        {
            ServerNameTextBox.Text = value;
            NotifyIsValid();
        }
    }

    public string DatabaseName
    {
        get => DatabaseTextBox.Text;
        set
        {
            DatabaseTextBox.Text = value;
            NotifyIsValid();
        }
    }


    public bool IsValid => ValidateParameters();


    public string ConnectionString { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;


    private void SetupControls()
    {
        ServerNameTextBox.Focus();
        MesoPasswordTextBox.TextChanged += (sender, args) => { NotifyIsValid(); };
        ButtonTest.Click += (sender, args) => { Test(); };
        ButtonOk.Click += (sender, args) => { Ok(); };
        ButtonCancel.Click += (sender, args) => { Cancel(); };
    }


    private void Ok()
    {
        ConnectionString = BuildConnectionString();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void Cancel()
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void Test()
    {
        var success = TestConnection(BuildConnectionString());

        MessageBox.Show(this, success ? "Verbindung erfolgreich" : "Verbindung fehlgeschlagen");
        ButtonOk.Enabled = success;
    }

    private string BuildConnectionString()
    {
        var builder = new SqlConnectionStringBuilder
        {
            ["Data Source"] = ServerName,
            ["Initial Catalog"] = DatabaseName,
            ["User Id"] = "meso",
            ["Password"] = MesoPasswordTextBox.Text
        };


        return builder.ConnectionString;
    }

    private void NotifyIsValid()
    {
        OnPropertyChanged(IsValidPropertyName);
        ButtonTest.Enabled = IsValid;
    }

    private bool ValidateParameters()
    {
        return !string.IsNullOrEmpty(DatabaseName)
               && !string.IsNullOrEmpty(ServerName)
               && !string.IsNullOrEmpty(MesoPasswordTextBox.Text);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    internal static bool TestConnection(string conn)
    {
        var success = true;

        try
        {
            using var connection = new SqlConnection(conn);
            connection.Open();
            connection.Close();
        }
        catch
        {
            success = false;
        }

        return success;
    }
}
