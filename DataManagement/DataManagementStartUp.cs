using DataManagement.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement
{
    public static class DataManagementStartUp
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // Initialization code can be added here in the future
            services.AddScoped<IDbManagement, DbManagement>();
        }
    }
}
