using DevExpress.Xpo;
using MesoXPO;
using MesoXPO.Models.Enums;
using MesoXPO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfXpo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();



            // Systemverbindung
            SystemConnection = new MesoXPO.SystemUnitOfWork("DEMOSRV01\\DEMO", 0, "CWLSYS", "meso", "meso@demo");

            // Mandantenverbindung
            CompanyConnection = SystemConnection.GetDataUnitOfWorkForCompany("500M");

            // statische Systemverbindung für Zugriff aus den Datenobjekten (z.B. auf Archivdokumente, Eigenschaften etc., die nicht in derselben Datenbank liegen)
            SystemUnitOfWork.SessionSystem = SystemConnection;

            //var artikelliste = new XPCollection<ArtikelStammdatei>(CompanyConnection);

            var artikelMitLagerstand = new XPQuery<ArtikelStammdatei>(CompanyConnection).Where(a => a.Lagereinstellungen.Artikeltyp == ProductTypeEnum.ProductWithStock
                && (a.Lagerwerte.MengeZugang - a.Lagerwerte.MengeAbgang - a.Lagerwerte.MengeProduktion) > 0
                && a.Mesocomp == CompanyConnection.CompanyNr
                && a.Mesoyear == CompanyConnection.MesoYear);
            
            this.DataContext = artikelMitLagerstand;
        }

        public MesoDataUnitOfWork CompanyConnection { get; private set; }

        public SystemUnitOfWork SystemConnection { get; private set; }
    }
}
