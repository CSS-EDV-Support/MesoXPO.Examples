// ConsoleXPO - Interaktives Konsolenbeispiel fuer MesoXPO
// Zeigt den manuellen Verbindungsaufbau ohne Dependency Injection.
// Fuer DI-basierte Beispiele siehe das MinimalWebApi-Projekt.

using DevExpress.Xpo;
using MesoXPO;
using MesoXPO.Models;
using MesoXPO.Models.Enums;
using MesoXPO.Models.SystemData;
using MesoXPO.Services;

// Verbindungsdaten interaktiv abfragen
var sqlServer = LeseEingabe("SQL Server");
var systemDb = LeseEingabe("Name der Systemdatenbank");
var mesoPasswort = LeseEingabe("Passwort fuer den meso-User", istPasswort: true);

try
{
    // SystemUnitOfWork: Zugriff auf die WinLine-Systemdatenbank.
    // Enthaelt Mandanteninformationen, Benutzer, Datenbankverbindungen etc.
    using var systemVerbindung = new SystemUnitOfWork(sqlServer, 0, systemDb, "meso", mesoPasswort);

    // Verfuegbare Mandanten aus der Systemdatenbank abfragen
    var mandanten = new XPQuery<Datenbankverbindung>(systemVerbindung)
        .Select(d => d.Mandant)
        .ToList();

    var mandant = WaehleAusListe(mandanten, "Mandantenauswahl");

    // MesoObjectLayer: Erzeugt einen mandantenspezifischen DataLayer.
    // Alle Abfragen werden automatisch nach Mandant und Wirtschaftsjahr gefiltert.
    var mesoDal = systemVerbindung.GetMesoObjectLayer(mandant);
    using var mandantenVerbindung = mesoDal.GetDataUnitOfWork();

    // Optional: Systemdatenbank-Service fuer Zugriff aus XPO-Datenobjekten bereitstellen
    // (z.B. auf Archivdokumente, Eigenschaften etc., die in der Systemdatenbank liegen).
    // In DI-Szenarien (ASP.NET Core) wird dies automatisch durch AddMesoXPOServices() erledigt.
    SystemDatabaseServiceProvider.SetStaticService(new SystemDatabaseService(systemVerbindung));

    // --- Beispiel 1: Artikel mit Lagerbestand abfragen ---
    Console.WriteLine("\n--- Artikel mit Lagerbestand ---");
    var artikelMitLagerstand = new XPQuery<ArtikelStammdatei>(mandantenVerbindung)
        .Where(a =>
            a.Lagereinstellungen.Artikeltyp == ProductTypeEnum.ProductWithStock
            && a.Lagerwerte.Lagerbestand > 0);

    foreach (var artikel in artikelMitLagerstand)
    {
        Console.WriteLine(
            $"  Artikel: {artikel.Bezeichnung} " +
            $"| Gruppe: {artikel.Preisstamm?.ArtikelgruppenStamm?.Gruppentext} " +
            $"| Bestand: {artikel.Lagerwerte.Lagerbestand} {artikel.Preisstamm?.ColliVerkauf} " +
            $"| Verfuegbar: {artikel.Lagerwerte.VerfuegbarerBestand()}");
    }

    // --- Beispiel 2: CRM-Vorgaenge mit Kundeninformationen ---
    Console.WriteLine("\n--- CRM-Vorgaenge deutscher Kunden ---");
    var workflowsMitKunden = new XPQuery<CrmIncidencesUndSchritte>(mandantenVerbindung)
        .Where(c =>
            c.Kunde.Adresse.Staat == "D"
            && c.Id > 0
            && c.FlagFuerLetztenEintrag == 1); // 1 = nur den letzten Schritt jedes Vorgangs anzeigen

    foreach (var workflow in workflowsMitKunden)
    {
        Console.WriteLine(
            $"  Kunde: {workflow.Kunde.Kontoname} | Fall {workflow.Id}: {workflow.Kurzbeschreibung} " +
            $"| {workflow.Uploads.Count()} Anhaenge");

        foreach (var upload in workflow.Uploads)
        {
            Console.WriteLine($"       Anhang: {upload.Dateiname} ({upload.UploadId})");
        }
    }

    // --- Beispiel 3: Aktuelle Verkaufsauftraege ---
    Console.WriteLine("\n--- Aktuelle Verkaufsauftraege (Top 10) ---");
    var aktuelleAuftraege = new XPQuery<BestelldateiKopf>(mandantenVerbindung)
        .Where(a =>
            a.Belegstufe == 2              // 2 = Auftrag (nicht Angebot, Lieferschein etc.)
            && a.EinkaufsVerkaufsflag == 2) // 2 = Verkauf (nicht Einkauf)
        .Take(10);

    foreach (var auftrag in aktuelleAuftraege)
    {
        Console.WriteLine($"  Auftrag {auftrag.KontonummerLaufnummer}: {auftrag.Auftragsnummer}");
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nFehler: {ex.Message}");
    Console.ResetColor();
}

Console.WriteLine("\nBeliebige Taste zum Beenden...");
Console.ReadKey();

// --- Hilfsmethoden ---

/// <summary>
/// Liest eine Benutzereingabe von der Konsole. Bei Passwoertern wird die Eingabe maskiert.
/// </summary>
static string LeseEingabe(string paramName, bool istPasswort = false)
{
    string? eingabe = null;
    while (string.IsNullOrWhiteSpace(eingabe))
    {
        Console.Write($"{paramName}: ");
        eingabe = istPasswort ? LesePasswort() : Console.ReadLine();
    }
    return eingabe;
}

/// <summary>
/// Liest ein Passwort mit Maskierung (*) von der Konsole.
/// </summary>
static string LesePasswort()
{
    var passwort = new List<char>();
    ConsoleKeyInfo taste;

    while ((taste = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
    {
        if (taste.Key == ConsoleKey.Backspace && passwort.Count > 0)
        {
            passwort.RemoveAt(passwort.Count - 1);
            Console.Write("\b \b");
        }
        else if (!char.IsControl(taste.KeyChar))
        {
            passwort.Add(taste.KeyChar);
            Console.Write('*');
        }
    }
    Console.WriteLine();
    return new string(passwort.ToArray());
}

/// <summary>
/// Zeigt eine nummerierte Liste an und laesst den Benutzer einen Eintrag auswaehlen.
/// </summary>
static string WaehleAusListe(List<string> optionen, string titel)
{
    Console.WriteLine($"\n{titel}:");
    for (var i = 0; i < optionen.Count; i++)
    {
        Console.WriteLine($"  {i + 1} - {optionen[i]}");
    }

    while (true)
    {
        Console.Write("Auswahl: ");
        if (int.TryParse(Console.ReadLine(), out var auswahl)
            && auswahl >= 1
            && auswahl <= optionen.Count)
        {
            return optionen[auswahl - 1];
        }
        Console.WriteLine($"Bitte eine Zahl zwischen 1 und {optionen.Count} eingeben.");
    }
}
