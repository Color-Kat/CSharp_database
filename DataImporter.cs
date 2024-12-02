using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDatabase
{
    internal class DataImporter
    {
        private readonly DBService _dbService;

        public DataImporter(
            DBService dbService
        ) {
            _dbService = dbService;
        }

        public void importDataFromTxt(string filename, string tableName)
        {
            using (var reader = new StreamReader(filename))
            {
                // Clear db table before
                _dbService.ExecuteNonQuery($"DELETE FROM {tableName}");

                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(';');

                    // Escape the values to prevent SQL injection (if necessary)
                    var escapedValues = new List<string>();
                    foreach (var value in values)
                    {
                        // Wrap values in single quotes for string types
                        escapedValues.Add($"'{value.Replace("'", "''")}'");
                    }

                    // Construct the INSERT INTO query
                    string valuePlaceholders = string.Join(", ", escapedValues);

                    MessageBox.Show($"INSERT INTO {tableName} VALUES ({valuePlaceholders})");

                    _dbService.ExecuteNonQuery($"INSERT INTO {tableName} VALUES ({valuePlaceholders})");
                }
            }
        }
    }
}
