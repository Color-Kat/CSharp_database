using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
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

    }

}
