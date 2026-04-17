using System.Data;

namespace DataManagement.Helper
{
    public interface IDbManagement
    {
        IDbConnection GetConnectSingle();

        IDbConnection GetDbConnection();

        T? GetService<T>();
        object? GetService(Type serviceType);
        T? GetRequiredService<T>();
        object? GetRequiredService(Type serviceType);

    }
}
