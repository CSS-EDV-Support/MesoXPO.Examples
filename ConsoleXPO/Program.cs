using DevExpress.Xpo;
using MesoXPO;
using MesoXPO.Models;
using MesoXPO.Models.Enums;
using MesoXPO.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleXPO
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlServer = GetParamFromConsole("SQL Server");
            var systemDb = GetParamFromConsole("Name der Systemdatenbank");
            var mesoPasswort = GetParamFromConsole("Passwort für den meso-User", true);


            // Systemverbindung
            var systemConnection = new SystemUnitOfWork(sqlServer, 0, systemDb, "meso", mesoPasswort);


            var mandanten = new XPQuery<Datenbankverbindung>(systemConnection).Select(d => d.Mandant).ToList();

            var mandant = GetSelectionFromConsole(mandanten, "Mandantenauswahl");


            // Mandantenverbindung
            var mesoDal = systemConnection.GetMesoObjectLayer(mandant);
            var companyConnection = mesoDal.GetDataUnitOfWork();

            // optional: statische Systemverbindung für Zugriff aus den Datenobjekten (z.B. auf Archivdokumente, Eigenschaften etc., die nicht in derselben Datenbank und daher nicht im selben DataLayer liegen)
            SystemUnitOfWork.SessionSystem = systemConnection;

            var artikelMitLagerstand = new XPQuery<ArtikelStammdatei>(companyConnection).Where(a =>
                a.Lagereinstellungen.Artikeltyp == ProductTypeEnum.ProductWithStock
                && a.Lagerwerte.Lagerbestand > 0);

            foreach (var artikel in artikelMitLagerstand)
            {
                Console.WriteLine(
                    $"Artikel {artikel.Bezeichnung} der Artikelgruppe {artikel.Preisstamm?.ArtikelgruppenStamm?.Gruppentext} hat einen Bestand von {artikel.Lagerwerte.Lagerbestand}" +
                    $" {artikel.Preisstamm.ColliVerkauf}, Verfügbarer Bestand {artikel.Lagerwerte.VerfuegbarerBestand()}");
            }

            var workflowsMitKunden = new XPQuery<CrmIncidencesUndSchritte>(companyConnection).Where(c =>
                c.Kunde.Adresse.Staat == "D" && c.Id > 0 && c.FlagFuerLetztenEintrag == 1);

            foreach (var workflow in workflowsMitKunden)
            {
                Console.WriteLine(
                    $"Kunde {workflow.Kunde.Kontoname}, Fall {workflow.Id} - {workflow.Kurzbeschreibung}. {workflow.Uploads.Count()} Anhänge");

                if (!workflow.Uploads.Any())
                {
                    continue;
                }

                foreach (var upload in workflow.Uploads)
                {
                    Console.WriteLine($"     Anhang: {upload.Dateiname} ({upload.UploadId})");
                }
            }

            var aktuelleAuftrage = new XPQuery<BestelldateiKopf>(companyConnection).Where(a =>
                a.Belegstufe == 2 && a.EinkaufsVerkaufsflag == 2);


            Console.ReadLine();
        }

        static string GetSelectionFromConsole(List<string> optionen, string optionsName)
        {
            Console.WriteLine($"Mögliche Werte für {optionsName}:");
            for (var i = 1; i <= optionen.Count; i++)
            {
                Console.WriteLine($"{i} - {optionen[i - 1]}");
            }

            Console.WriteLine($"Bitte gewünschten Wert für {optionsName} auswählen:");
            var input = string.Empty;
            var validNumber = int.TryParse(input, out var selection);

            while (string.IsNullOrWhiteSpace(input) || !validNumber)
            {
                Console.WriteLine($"Bitte gültige Auswahl für {optionsName} eingeben:");
                input = Console.ReadLine();
                validNumber = int.TryParse(input, out selection);
                if (optionen.Count <= selection)
                {
                    break;
                }
            }

            return optionen[selection - 1];
        }

        static string GetParamFromConsole(string paramName, bool passwordMask = false)
        {
            var input = string.Empty;
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"Bitte Wert für {paramName} eingeben:");
                input = passwordMask ? ConsoleHelper.ReadPassword() : Console.ReadLine();
            }

            return input;
        }
    }

    public static class ConsoleHelper
    {
        public static string ReadPassword(char mask = '*')
        {
            const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
            int[] FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

            var pass = new Stack<char>();
            var chr = (char)0;

            while ((chr = Console.ReadKey(true).KeyChar) != ENTER)
            {
                if (chr == BACKSP)
                {
                    if (pass.Count > 0)
                    {
                        Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (chr == CTRLBACKSP)
                {
                    while (pass.Count > 0)
                    {
                        Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => chr == x) > 0)
                {
                }
                else
                {
                    pass.Push(chr);
                    Console.Write(mask);
                }
            }

            Console.WriteLine();

            return new string(pass.Reverse().ToArray());
        }
    }
}
