using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.DapperLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DbTableAttribute : Attribute
    {
        public DbTableAttribute()
        {
        }
        public DbTableAttribute(string name, string viewName = "")
        {
            Name = name;
            ViewName = viewName;
        }

        public string Name { get; init; } = string.Empty;
        public string ViewName { get; init; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class DbFieldAttribute : Attribute
    {
        public string Name { get; init; } = string.Empty;
        public ColumnKey Key { get; init; } = ColumnKey.None;
        public bool IsKey { get; init; } = false;
        public bool Ignore { get; init; } = false;
        public bool IgnoreInsert { get; init; } = false;
        public bool IgnoreUpdate { get; init; } = false;
        public bool IsClob { get; init; } = false;
        public bool IsDetailTable { get; init; } = false;
    }

    public enum ColumnKey
    {
        None,
        Primary,
        Quniue,
        Unique,
        Foreign
    }
}
