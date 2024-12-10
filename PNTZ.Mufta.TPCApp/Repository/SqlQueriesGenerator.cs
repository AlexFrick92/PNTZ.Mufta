using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public static class SqlQueriesGenerator
    {
        public static string CreateTable<T>(string tableName)
        {
            var properties = typeof(T).GetProperties();

            var columns = properties.Select(p =>
            {
                // Определение типа для каждого свойства
                var columnType = GetSQLiteColumnType(p.PropertyType);

                return $"{p.Name} {columnType}";
            });

            var sql = $"CREATE TABLE IF NOT EXISTS {tableName} ({string.Join(", ", columns)});";

            return sql;
        }

        private static string GetSQLiteColumnType(Type propertyType)
        {
            // Маппинг типов .NET в типы SQLite
            if (propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(short))
                return "INTEGER";
            if (propertyType == typeof(bool))
                return "INTEGER"; // SQLite не имеет отдельного типа для boolean, используем INTEGER
            if (propertyType == typeof(string))
                return "TEXT";
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTimeOffset))
                return "TEXT"; // В SQLite DateTime обычно сохраняется в текстовом формате (ISO 8601)
            if (propertyType == typeof(float) || propertyType == typeof(double) || propertyType == typeof(decimal))
                return "REAL";
            if (propertyType == typeof(byte[]) || propertyType == typeof(Guid))
                return "BLOB";

            return "TEXT"; // По умолчанию используем TEXT
        }

        public static string Insert<T>(string tableName)
        {
            var properties = typeof(T).GetProperties();

            var columns = properties.Select(p =>
            {
                return $"{p.Name}";
            });

            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(s => "@" + s))});";

            return sql;
        }

        public static string Update<T>(string tableName, string whereStatement)
        {
            var properties = typeof(T).GetProperties();

            var columns = properties.Select(p =>
            {
                return $"{p.Name} = @{p.Name}";
            });

            var sql = $"UPDATE {tableName} SET {string.Join(", ", columns)} {whereStatement};";

            return sql;
        }

        public static string SelectFrom<T>(string tableName)
        {
            return $"SELECT * FROM {tableName}";
        }

    }
}
