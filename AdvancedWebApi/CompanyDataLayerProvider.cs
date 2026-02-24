// CompanyDataLayerProvider - Fortgeschrittenes Beispiel fuer Multi-Mandant-Szenarien
// Cached DataLayer pro Mandant in einem ConcurrentDictionary fuer performanten Zugriff.
// Fuer einfache Szenarien stattdessen AddMesoXPOServices() verwenden (siehe MinimalWebApi).

using System.Collections.Concurrent;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using MesoXPO;
using MesoXPO.Helper;

namespace AdvancedWebApi;

/// <summary>
/// Interface fuer einen mandantenspezifischen DataLayer-Provider.
/// Ermoeglicht den Zugriff auf verschiedene Mandanten-Datenbanken in einer Anwendung.
/// </summary>
public interface ICompanyDataLayerProvider
{
    /// <summary>
    /// Liefert einen gecachten ThreadSafeDataLayer fuer den angegebenen Mandanten.
    /// </summary>
    ThreadSafeDataLayer GetForCompany(string mesocomp);

    /// <summary>
    /// Liefert einen MesoObjectLayer fuer den angegebenen Mandanten.
    /// </summary>
    MesoObjectLayer GetMesoObjectLayerForCompany(string mesocomp);
}

/// <summary>
/// Implementierung des ICompanyDataLayerProvider.
/// Cached DataLayer pro Mandant fuer bessere Performance bei wiederholten Zugriffen.
/// </summary>
public sealed class CompanyDataLayerProvider(IServiceProvider sp) : ICompanyDataLayerProvider
{
    readonly ConcurrentDictionary<string, ThreadSafeDataLayer> _cache = new();

    public ThreadSafeDataLayer GetForCompany(string mesocomp)
    {
        return _cache.GetOrAdd(mesocomp, key =>
        {
            using var scope = sp.CreateScope();
            var sysUow = scope.ServiceProvider.GetRequiredService<SystemUnitOfWork>();

            var conn = sysUow.GetConnectionStringForCompany(key)
                       + ";Application Name=MesoXPO.AdvancedWebApi;Max Pool Size=300;Min Pool Size=5";

            var dictData = new ReflectionDictionary();
            var baseTypes = ConnectionHelper.GetPersistentTypesWinLineData().ToArray();
            dictData.GetDataStoreSchema(baseTypes);

            // Benutzerdefinierte Tabellen/Spalten laden (optional)
            DataLayerManager.LoadUserdefinedTablesAndColumns(conn, dictData, true, true);

            var pooled = XpoDefault.GetConnectionPoolString(conn);
            var store = XpoDefault.GetConnectionProvider(pooled, AutoCreateOption.SchemaAlreadyExists);
            return new ThreadSafeDataLayer(dictData, store);
        });
    }

    public MesoObjectLayer GetMesoObjectLayerForCompany(string mesocomp)
    {
        return new MesoObjectLayer(GetForCompany(mesocomp), mesocomp);
    }
}
