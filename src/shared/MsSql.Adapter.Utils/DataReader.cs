using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.Adapter.Utils
{
    public sealed class DataReader : IDisposable
    {
        private string? _connectionString = null;
        private SqlCredential? _credential = null;

        private SqlConnection? DbConnection { get; set; }

        public SqlDataReader? Reader { get; private set; }

        public DataReader(string connectionString, SqlCredential? credential)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw (new InvalidOperationException("connectionString was not set"));
            }

            _connectionString = connectionString;
            _credential = credential;
        }

        private SqlConnection GetDbConnection()
        {
            if (DbConnection == null)
            {
                DbConnection = new SqlConnection(_connectionString, _credential);

                DbConnection.Open();
            }

            return DbConnection;
        }

        public DataTable GetSchema(string collection, string[] restrictionValues)
        {
            return GetDbConnection().GetSchema(collection, restrictionValues);
        }

        public async Task ExecuteSpAsync(SqlCommand cmd, int timeout = 30)
        {
            if (DbConnection == null)
            {
                DbConnection = new SqlConnection(_connectionString, _credential);

                await DbConnection.OpenAsync().ConfigureAwait(false);
            }

            cmd.Connection = DbConnection;
            cmd.CommandTimeout = timeout;
            cmd.CommandType = CommandType.StoredProcedure;

            Reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        }

        public void Execute(SqlCommand cmd)
        {
            cmd.Connection = GetDbConnection();
            Reader = cmd.ExecuteReader();
        }

        public Task<bool> ReadAsync()
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.ReadAsync();
        }

        public Task<bool> NextResultAsync()
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.NextResultAsync();
        }

        public List<string> ValidateColumns(DataReaderColumnDefinition[] columns, bool strict)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            var drColumns = new HashSet<string>();
            var missingColumns = new HashSet<string>();

            for (int i = 0; i < Reader.FieldCount; i++)
            {
                drColumns.Add(Reader.GetName(i));
            }
            var errors = new List<string>();
            foreach (var x in columns)
            {
                try
                {
                    if (drColumns.Remove(x.Name))
                    {
                        x.Order = Reader.GetOrdinal(x.Name);
                        var rowType = Reader.GetFieldType(x.Order);
                        if (rowType != x.Type)
                        {
                            errors.Add($"Column: '{x.Name}' Expected Type:'{x.Type.FullName}' Sql Data Type:'{rowType.FullName}'");
                        }
                    }
                    else
                    {
                        if (strict)
                        {
                            missingColumns.Add(x.Name);
                        }
                        x.Order = -1;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"[{x.Name}] {ex.Message}");
                }
            }
            if (strict)
            {
                if (drColumns.Count > 0)
                {
                    errors.Add($"Unexpected columns in sql data: {string.Join(",", drColumns)}.");
                }
                if (missingColumns.Count > 0)
                {
                    errors.Add($"Expected columns missing from sql data: {string.Join(",", missingColumns)}.");
                }
            }
            return errors;
        }

        public void CheckFieldType(string columnName, Type type, StringBuilder sb)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            var rowType = Reader.GetFieldType(Reader.GetOrdinal(columnName));
            if (rowType != type)
            {
                sb.Append($"[{columnName}] Expects:{type.FullName} row:{rowType.FullName}");
            };
        }

        public int GetOrdinal(string key)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.GetOrdinal(key);
        }

        public byte[]? GetBytes(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            byte[]? result = null;

            if (!Reader.IsDBNull(ordinal))
            {
                long size = Reader.GetBytes(ordinal, 0, null, 0, 0); //get the length of data
                result = new byte[size];
                int bufferSize = 1024;
                long bytesRead = 0;
                int curPos = 0;
                while (bytesRead < size)
                {
                    bytesRead += Reader.GetBytes(ordinal, curPos, result, curPos, bufferSize);
                    curPos += bufferSize;
                }
            }

            return result;
        }

        public byte? GetByte(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetByte(ordinal);
        }

        public bool? GetBoolean(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetBoolean(ordinal);
        }

        public short? GetInt16(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetInt16(ordinal);
        }

        public int? GetInt32(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetInt32(ordinal);
        }

        public long? GetInt64(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetInt64(ordinal);
        }

        public Guid? GetGuid(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetGuid(ordinal);
        }

        public float? GetFloat(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetFloat(ordinal);
        }

        public double? GetDouble(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetDouble(ordinal);
        }

        public decimal? GetDecimal(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetDecimal(ordinal);
        }

        public DateTime? GetDateTime(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetDateTime(ordinal);
        }

        public DateTimeOffset? GetDateTimeOffset(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetDateTimeOffset(ordinal);
        }

        public TimeSpan? GetTimeSpan(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetTimeSpan(ordinal);
        }

        public string? GetString(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : Reader.GetString(ordinal);
        }

        public decimal? GetSqlMoney(int ordinal)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("Reader is null. Was Execute() called?");
            }

            return Reader.IsDBNull(ordinal) ? null : (decimal)Reader.GetSqlMoney(ordinal);
        }

        #region >>>dispose...

        private bool disposed = false;

        ~DataReader()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Reader?.Dispose();
                    DbConnection?.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion >>>dispose...
    }
}