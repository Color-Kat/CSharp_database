using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using System.Data;

namespace MyDatabase
{
    internal class DBService
    {

        private readonly string _connectionString;
        private SqlConnection _connection;

        public DBService(string connectionString)
        {
            _connectionString = connectionString;

            // Auto start DB connection
            // It will be closed in the destructor
            OpenConnection();
        }

        ~DBService()
        {
            // Close connection in destructor
            CloseConnection();
        }

        /**
         * Start SQL DB connection
         */
        public void OpenConnection()
        {
            if(_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            } 

            if(_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        /**
         * Close SQL DB connection
         */
        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        // ----- Methods for interact with db ----- //

        /**
         * For queries that return result
         */
        public SqlDataReader ExecuteReader(string query)
        {
            SqlCommand command = new SqlCommand(query, _connection);
            return command.ExecuteReader();
        }

        /**
         * For queries that do not return a result
         */
        public int ExecuteNonQuery(string query)
        {
            SqlCommand command = new SqlCommand(query, _connection);
            return command.ExecuteNonQuery();
        }


        /*
        // Метод для выполнения SQL-запроса с параметрами
        public int ExecuteNonQueryWithParams(string query, Dictionary<string, object> parameters)
        {
            SqlCommand command = new SqlCommand(query, _connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
            return command.ExecuteNonQuery();
        }

        // Метод для выполнения BulkCopy (быстрая вставка большого объема данных)
        public void BulkInsert(DataTable dataTable, string tableName)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_connection))
            {
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.BatchSize = 1000; // размер пакета
                bulkCopy.BulkCopyTimeout = 600; // таймаут
                bulkCopy.WriteToServer(dataTable);
            }
        }

        // Метод для выполнения транзакции (несколько запросов в одном атомарном блоке)
        public void ExecuteTransaction(List<string> queries)
        {
            SqlTransaction transaction = _connection.BeginTransaction();

            try
            {
                foreach (var query in queries)
                {
                    SqlCommand command = new SqlCommand(query, _connection, transaction);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        // Асинхронный метод для выполнения запроса на выборку
        public async Task<SqlDataReader> ExecuteReaderAsync(string query)
        {
            SqlCommand command = new SqlCommand(query, _connection);
            return await command.ExecuteReaderAsync();
        }

        // Асинхронный метод для выполнения запроса на изменение данных
        public async Task<int> ExecuteNonQueryAsync(string query)
        {
            SqlCommand command = new SqlCommand(query, _connection);
            return await command.ExecuteNonQueryAsync();
        }

        // Асинхронный метод для выполнения запроса с параметрами
        public async Task<int> ExecuteNonQueryWithParamsAsync(string query, Dictionary<string, object> parameters)
        {
            SqlCommand command = new SqlCommand(query, _connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
            return await command.ExecuteNonQueryAsync();
        }

        */

        // -------

        /*
         using Microsoft.Data.SqlClient;
using System;
using System.Data;

public static class SqlExtensions
{
    public static void ExecuteQuery(this SqlConnection connection, string query, Action<SqlDataReader> handleRow)
    {
        using (var command = new SqlCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                handleRow(reader);  // Передаем обработку строки
            }
        }
    }

    public static int ExecuteNonQuery(this SqlConnection connection, string query, params SqlParameter[] parameters)
    {
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddRange(parameters);
            return command.ExecuteNonQuery();
        }
    }
}

class Program
{
    static void Main()
    {
        string connectionString = "Server=your_server_name;Database=your_database_name;User Id=your_username;Password=your_password;";

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Пример SELECT-запроса
            string selectQuery = "SELECT Id, Name FROM Users";
            connection.ExecuteQuery(selectQuery, reader =>
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                Console.WriteLine($"Id: {id}, Name: {name}");
            });

            // Пример INSERT-запроса
            string insertQuery = "INSERT INTO Users (Name, Age) VALUES (@name, @age)";
            var parameters = new[]
            {
                new SqlParameter("@name", "John Doe"),
                new SqlParameter("@age", 30)
            };
            int rowsAffected = connection.ExecuteNonQuery(insertQuery, parameters);
            Console.WriteLine($"Rows affected: {rowsAffected}");
        }
    }
}

         */
    }

}
