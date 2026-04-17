using CommomLib;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Helper
{
    public class DbManagement : IDbManagement
    {
        private IDbConnection? _dbConnection;
        private readonly IServiceProvider _serviceProvider;
        public DbManagement(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IDbConnection GetConnectSingle()
        {
            if (_dbConnection is null)
            {
                _dbConnection = new NpgsqlConnection(ConfigData.ConnectionString);
            }
            return _dbConnection;
        }

        public IDbConnection GetDbConnection()
        {
            var dbConnection = new NpgsqlConnection(ConfigData.ConnectionString);
            dbConnection.Open();
            return dbConnection;
        }

        public T? GetRequiredService<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public object? GetRequiredService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public T? GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public object? GetService(Type serviceType)
        {
            return _serviceProvider.GetRequiredService(serviceType);
        }
    }
}
