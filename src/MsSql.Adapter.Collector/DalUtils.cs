using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MsSql.Collector
{
    public class DalUtils
    {
        public static string StripUnderscorePrefix(string name)
        {
            int uderscoreIndex;
            if (string.IsNullOrWhiteSpace(name)
                || (uderscoreIndex = name.IndexOf('_')) < 0)
            {
                return name;
            }
            else
            {
                return name.Substring(uderscoreIndex + 1, name.Length - (uderscoreIndex + 1));
            }
        }

        public static Type GetBaseCSharpType(string sqlType)
        {
            switch ((sqlType ?? string.Empty).ToLower())
            {
                case "bigint": return typeof(long);
                case "binary": return typeof(byte[]);
                case "bit": return typeof(bool);
                case "char": return typeof(string);
                case "date": return typeof(DateTime);
                case "filestream": return typeof(byte[]);
                case "image": return typeof(byte[]);
                case "tinyint": return typeof(byte);
                case "int": return typeof(int);
                case "float": return typeof(double);
                case "decimal": return typeof(decimal);
                case "money": return typeof(decimal);
                case "nchar": return typeof(string);
                case "ntext": return typeof(string);
                case "numeric": return typeof(decimal);
                case "nvarchar": return typeof(string);
                case "real": return typeof(Single);
                case "rowversion": return typeof(byte[]);
                case "smalldatetime": return typeof(DateTime);
                case "smallint": return typeof(short);
                case "smallmoney": return typeof(decimal);
                case "sql_variant": return typeof(object);
                case "text": return typeof(string);
                case "time": return typeof(TimeSpan);
                case "timestamp": return typeof(byte[]);

                case "uniqueidentifier": return typeof(Guid);
                case "varbinary": return typeof(byte[]);
                case "varchar": return typeof(string);
                case "xml": return typeof(string);
                case "datetime": return typeof(DateTime);
                case "datetime2": return typeof(DateTime);
                case "datetimeoffset": return typeof(DateTimeOffset);

                default: return typeof(object);
            }
        }
    }
}