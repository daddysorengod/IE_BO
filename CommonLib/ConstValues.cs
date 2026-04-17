using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommomLib
{
    public class ConstValues
    {
        #region Database type: Oracle, Postgres, MySql
        public static string DB_ORACLE = "Oracle"; 
        public static string DB_POSTGRES = "Postgres";
        public static string DB_MYSQL = "MySql";
        #endregion

        #region Database prefix
        public static string PREFIX_ORACLE = ":";
        public static string PREFIX_POSTGRES = "@";
        public static string PREFIX_MYSQL = "@";
        #endregion
    }
}
