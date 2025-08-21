using DevExpress.Data.Async.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using MesoXPO;
using MesoXPO.Helper;
using System.Collections.Concurrent;

namespace NetCoreMinimalApi;


public interface ICompanyDataLayerProvider
{
    ThreadSafeDataLayer GetForCompany(string mesocomp);
    MesoObjectLayer GetMesoObjectLayerForCompany(string mesocomp);
}

public sealed class CompanyDataLayerProvider(IServiceProvider sp) : ICompanyDataLayerProvider
{
    private readonly ConcurrentDictionary<string, ThreadSafeDataLayer> _cache = new();

    public ThreadSafeDataLayer GetForCompany(string mesocomp)
    {
        return _cache.GetOrAdd(mesocomp, key => {
            using var scope = sp.CreateScope();
            var sysUow = scope.ServiceProvider.GetRequiredService<SystemUnitOfWork>();

            var conn = sysUow.GetConnectionStringForCompany(key)
                       + ";Application Name=MESOGanttplan.MesoApi;Max Pool Size=300;Min Pool Size=5";

            var dictData = new ReflectionDictionary();
            var baseTypes = ConnectionHelper.GetPersistentTypesWinLineData()
                .ToArray();
            dictData.GetDataStoreSchema(baseTypes);

            // Userdefined Tables/Columns laden:
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
