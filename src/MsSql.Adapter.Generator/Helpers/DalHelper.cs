using MsSql.Collector.Types;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MsSql.Adapter.Generator.helpers
{
    public static class DalHelper
    {
        public static string GetBaseClassName(string originalName)
        {
            return ToPascalCase($@"({originalName.Replace("nc_", "").Replace("sp_", "").Replace("prc_", "")}");
        }

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

        public static string ToPascalCase(string original)
        {
            var invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            var whiteSpace = new Regex(@"(?<=\s)");
            var startsWithLowerCaseChar = new Regex("^[a-z]");
            var firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$@");
            var lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            var upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }

        public static string ToProtoCase(string original)
        {
            var invalidCharsRgx = new Regex("[^a-zA-Z0-9]");
            var whiteSpace = new Regex(@"(?<=\s)");
            var startsWithLowerCaseChar = new Regex("^[a-z]");

            // replace white spaces with and all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, string.Empty), string.Empty).ToLower();

            // set first letter to uppercase
            pascalCase = startsWithLowerCaseChar.Replace(pascalCase, m => m.Value.ToUpper());

            return string.Concat(pascalCase);
        }

        public static string ToLowerFirst(string original)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                return original;
            }

            var charArray = original.ToCharArray();

            charArray[0] = char.ToLower(charArray[0]);

            return new string(charArray);
        }

        public static Type GetBaseCSharpType(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "bigint" => typeof(long),
                "binary" => typeof(byte[]),
                "bit" => typeof(bool),
                "char" => typeof(string),
                "date" => typeof(DateTime),
                "filestream" => typeof(byte[]),
                "image" => typeof(byte[]),
                "tinyint" => typeof(byte),
                "int" => typeof(int),
                "float" => typeof(double),
                "decimal" => typeof(decimal),
                "money" => typeof(decimal),
                "nchar" => typeof(string),
                "ntext" => typeof(string),
                "numeric" => typeof(decimal),
                "nvarchar" => typeof(string),
                "real" => typeof(Single),
                "rowversion" => typeof(byte[]),
                "smalldatetime" => typeof(DateTime),
                "smallint" => typeof(short),
                "smallmoney" => typeof(decimal),
                "sql_variant" => typeof(object),
                "text" => typeof(string),
                "time" => typeof(TimeSpan),
                "timestamp" => typeof(byte[]),
                "uniqueidentifier" => typeof(Guid),
                "varbinary" => typeof(byte[]),
                "varchar" => typeof(string),
                "xml" => typeof(string),
                "datetime" => typeof(DateTime),
                "datetime2" => typeof(DateTime),
                "datetimeoffset" => typeof(DateTimeOffset),
                _ => typeof(object),
            };
        }

        public static string GetCSharpFriendlyType(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "bigint" => "long",
                "binary" => "byte[]",
                "bit" => "bool",
                "char" => "string",
                "date" => "DateTime",
                "filestream" => "byte[]",
                "image" => "byte[]",
                "tinyint" => "byte",
                "int" => "int",
                "float" => "double",
                "decimal" => "decimal",
                "money" => "decimal",
                "nchar" => "string",
                "ntext" => "string",
                "numeric" => "decimal",
                "nvarchar" => "string",
                "real" => "Single",
                "rowversion" => "byte[]",
                "smalldatetime" => "DateTime",
                "smallint" => "short",
                "smallmoney" => "decimal",
                "sql_variant" => "object",
                "text" => "string",
                "time" => "TimeSpan",
                "timestamp" => "byte[]",
                "uniqueidentifier" => "Guid",
                "varbinary" => "byte[]",
                "varchar" => "string",
                "xml" => "string",
                "datetime" => "DateTime",
                "datetime2" => "DateTime",
                "datetimeoffset" => "DateTimeOffset",
                _ => "object",
            };
        }

        public static string GetCSharpNullableFriendlyType(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "bigint" => "long?",
                "binary" => "byte[]?",
                "bit" => "bool?",
                "char" => "string?",
                "date" => "DateTime?",
                "filestream" => "byte[]?",
                "image" => "byte[]?",
                "tinyint" => "byte?",
                "int" => "int?",
                "float" => "double?",
                "decimal" => "decimal?",
                "money" => "decimal?",
                "nchar" => "string?",
                "ntext" => "string?",
                "numeric" => "decimal?",
                "nvarchar" => "string?",
                "real" => "Single?",
                "rowversion" => "byte[]?",
                "smalldatetime" => "DateTime?",
                "smallint" => "short?",
                "smallmoney" => "decimal?",
                "sql_variant" => "object?",
                "text" => "string?",
                "time" => "TimeSpan?",
                "timestamp" => "byte[]?",
                "uniqueidentifier" => "Guid?",
                "varbinary" => "byte[]?",
                "varchar" => "string?",
                "xml" => "string?",
                "datetime" => "DateTime?",
                "datetime2" => "DateTime?",
                "datetimeoffset" => "DateTimeOffset?",
                _ => "object?",
            };
        }

        public static string GetJavascriptFriendlyType(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "bigint" => "number",
                "binary" => "Uint8Array",
                "bit" => "boolean",
                "char" => "string",
                "date" => "protobuf_net_bcl_pb.DateTime",
                "filestream" => "Uint8Array",
                "image" => "Uint8Array",
                "tinyint" => "number",
                "int" => "number",
                "float" => "number",
                "decimal" => "protobuf_net_bcl_pb.Decimal",
                "money" => "number",
                "nchar" => "string",
                "ntext" => "string",
                "numeric" => "number",
                "nvarchar" => "string",
                "real" => "number",
                "rowversion" => "Uint8Array",
                "smalldatetime" => "protobuf_net_bcl_pb.DateTime",
                "smallint" => "number",
                "smallmoney" => "number",
                "sql_variant" => "Uint8Array",
                "text" => "string",
                "time" => "protobuf_net_bcl_pb.TimeSpan",
                "timestamp" => "Uint8Array",
                "uniqueidentifier" => "protobuf_net_bcl_pb.Guid",
                "varbinary" => "Uint8Array",
                "varchar" => "string",
                "xml" => "string",
                "datetime" => "protobuf_net_bcl_pb.DateTime",
                "datetime2" => "protobuf_net_bcl_pb.DateTime",
                "datetimeoffset" => "number",
                _ => "Uint8Array",
            };
        }

        public static bool IsDatetime(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "smalldatetime" or "datetime" or "datetime2" => true,
                _ => false,
            };
        }

        public static bool IsDecimal(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "decimal" => true,
                _ => false,
            };
        }

        public static string GetSqlDbType(string type)
        {
            return type.ToLower() switch
            {
                "bigint" => "BigInt",
                "binary" => "VarBinary",
                "char" => "Char",
                "filestream" => "VarBinary",
                "float" => "Float",
                "geometry" or "hierarchyid" or "geography" => "Udt",
                "image" => "Image",
                "bit" => "Bit",
                "tinyint" => "TinyInt",
                "int" => "Int",
                "money" => "Money",
                "decimal" => "Decimal",
                "nchar" => "NChar",
                "ntext" => "NText",
                "numeric" => "Decimal",
                "nvarchar" => "NVarChar",
                "real" => "Real",
                "rowversion" => "Timestamp",
                "smalldatetime" => "SmallDateTime",
                "smallint" => "SmallInt",
                "smallmoney" => "SmallMoney",
                "sql_variant" => "Variant",
                "text" => "Text",
                "time" => "Time",
                "timestamp" => "Timestamp",
                "uniqueidentifier" => "UniqueIdentifier",
                "varbinary" => "VarBinary",
                "varchar" => "VarChar",
                "xml" => "Xml",
                "date" => "Date",
                "datetime" => "DateTime",
                "datetime2" => "DateTime2",
                "datetimeoffset" => "DateTimeOffset",
                _ => "Structured",
            };
        }

        public static string GenerateDataRowReadValue(string sqlType, string order)
        {
            return sqlType.ToLower() switch
            {
                "bit" => $@"dr.GetBoolean({order})",
                "tinyint" => $@"dr.GetByte({order})",
                "smallint" => $@"dr.GetInt16({order})",
                "int" => $@"dr.GetInt32({order})",
                "smallmoney" => $@"dr.GetDecimal({order})",
                "money" => $@"dr.GetSqlMoney({order})",
                "float" => $@"dr.GetFloat({order})",
                "numeric" or "decimal" => $@"dr.GetDecimal({order})",
                "bigint" => $@"dr.GetInt64({order})",
                "uniqueidentifier" => $@"dr.GetGuid({order})",
                "nvarchar" or "ntext" or "nchar" or "xml" or "varchar" or "text" or "char" => $@"dr.GetString({order})",
                "timestamp" or "filestream" or "varbinary" or "binary" => $@"dr.GetBytes({order})",
                "time" => $@"dr.GetTimeSpan({order})",
                "smalldatetime" or "datetime" or "datetime2" or "date" => $@"dr.GetDateTime({order})",
                "datetimeoffset" => $@"dr.GetDateTimeOffset({order})",
                _ => "not implemented",
            };
        }
    }
}