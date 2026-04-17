using Microsoft.Extensions.Configuration;

namespace CommomLib
{
    public static class ConfigData
    {
        public static string ConnectionString = "";
        public static string DatabaseType = "";

        public static void InitConfig(IConfigurationRoot configuration)
        {
            if (configuration != null)
            {
                ConnectionString = configuration["ConnectionString"]?.Trim() ?? "";
                DatabaseType = configuration["DatabaseType"]?.Trim() ?? "";
            }
        }
    }
}
