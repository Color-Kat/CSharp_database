using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDatabase
{
    internal class TaskMerger
    {

        private struct Task
        {
            public string id;
            public uint class_number;
            public string topic;
            public string subject;
            public uint complexity_level;
        }

        // Bucket is a struct that stores 
        // a bunch of students that have the same criterias.
        // (In this case - class number)
        private struct Bucket
        {
            public int class_number;
            public List<string> StudentIds; // List of student ids in the bucket grouped by class_number
        }

        private readonly DBService _dbService;

        private DataTable _virtualDataTable;
        private SqlBulkCopy _bulkCopy;

        private Bucket _currentBucket = new Bucket();
        private Task _currentTask;

        public TaskMerger(
            DBService dbService
        )
        {
            _dbService = dbService;

            _virtualDataTable = new DataTable();
            _virtualDataTable.Columns.Add("id", typeof(string));
            _virtualDataTable.Columns.Add("classNumber", typeof(int));
            _virtualDataTable.Columns.Add("subject", typeof(string));
            _virtualDataTable.Columns.Add("topic", typeof(string));
            _virtualDataTable.Columns.Add("complexity_level", typeof(int));

            _bulkCopy = new SqlBulkCopy(_dbService.Connection)
            {
                DestinationTableName = "Tasks"
                // DestinationTableName = "dbo.Tasks"
            };

            _bulkCopy.ColumnMappings.Add("id", "id");
            _bulkCopy.ColumnMappings.Add("class_number", "class");
            _bulkCopy.ColumnMappings.Add("subject", "subject");
            _bulkCopy.ColumnMappings.Add("topic", "topic");
            _bulkCopy.ColumnMappings.Add("complexity_level", "complexity_level");
        }

        private int CompareBucket(int classNumber)
        {
            string bucketKey = $"{_currentBucket.class_number:D2}";
            string taskKey = $"{classNumber:D2}";

            return bucketKey.CompareTo(taskKey);
        }

        private bool Scoop()
        {
            return 0;
        }

        public void MergeTasks()
        {
            using (var studentReader = _dbService.ExecuteReader("SELECT * FROM Students ORDER BY class"))
            using (var taskReader = _dbService.ExecuteReader("SELECT * FROM Tasks ORDER BY class"))
            {
                bool hasMoreStudents = studentReader.Read();
                bool hasMoreTasks = taskReader.Read();

                while(hasMoreStudents && hasMoreTasks) {
                    int compareResult = CompareBucket(taskReader.GetInt32(0));

                    if (compareResult < 0)
                    {
                        // Task.class_number is greater than currentBucket.class_number
                        // Keep reading the students
                        hasMoreStudents = Scoop();
                    }
                    else if (compareResult == 0)
                    {
                        // There are students that match the current task in the current bucket
                        // Add this students to the cirtual table for further copy
                        foreach(var studentId in _currentBucket.StudentIds) {
                            _virtualDataTable.Rows.Add(
                                studentId,
                                currentBucket.Grade,
                                currentBucket.Theme, 
                                currentBucket.ComplexityLevel, 
                                readerTasks.GetString(3)
                            );
                        }
                    }
                    else
                    {
                        // ? If tasks has a higher class..., keep reading tasks
                        hasMoreTasks = taskReader.Read();
                    }

                    // После завершения цикла вставляем все данные в таблицу
                    _bulkCopy.BatchSize = _virtualDataTable.Rows.Count;
                    _bulkCopy.BulkCopyTimeout = 600;
                    _bulkCopy.WriteToServer(_virtualDataTable);
                    _bulkCopy.Close();
                }

            }
        }

        /*public void _Init()
        {
            currentBucket = new Bucket();
            currentBucket.Students = new List<string>();

            // Set up database connections
            connectionStudents = new SqlConnection(connectionString);
            connectionStudents.Open();
            connectionTasks = new SqlConnection(connectionString);
            connectionTasks.Open();
            connectionStudentTasks = new SqlConnection(connectionString);
            connectionStudentTasks.Open();

            // Set up SQL commands
            commandStudents = new SqlCommand("SELECT * FROM Students ORDER BY Grade, Theme, ComplexityLevel;", connectionStudents);
            readerStudents = commandStudents.ExecuteReader();

            commandTasks = new SqlCommand("SELECT * FROM Tasks ORDER BY Grade, Theme, ComplexityLevel;", connectionTasks);
            readerTasks = commandTasks.ExecuteReader();

            // Set up DataTable for bulk copy
            dataTableStudentTasks = new DataTable();
            dataTableStudentTasks.Columns.Add("Student", typeof(string));
            dataTableStudentTasks.Columns.Add("Grade", typeof(int));
            dataTableStudentTasks.Columns.Add("Theme", typeof(string));
            dataTableStudentTasks.Columns.Add("ComplexityLevel", typeof(int));
            dataTableStudentTasks.Columns.Add("TaskNumber", typeof(string));

            // Set up bulk copy
            bulkCopyStudentTasks = new SqlBulkCopy(connectionStudentTasks)
            {
                DestinationTableName = "StudentTasks"  // Bulk copy destination table
            };
            bulkCopyStudentTasks.ColumnMappings.Add("Student", "Student");
            bulkCopyStudentTasks.ColumnMappings.Add("Grade", "Grade");
            bulkCopyStudentTasks.ColumnMappings.Add("Theme", "Theme");
            bulkCopyStudentTasks.ColumnMappings.Add("ComplexityLevel", "ComplexityLevel");
            bulkCopyStudentTasks.ColumnMappings.Add("TaskNumber", "TaskNumber");
        }

        public int CompareBucket(int grade, string theme, int complexityLevel)
        {
            // Compare the current bucket with the task data to determine if they match
            string bucketKey = $"{currentBucket.Grade:D2}{currentBucket.Theme.PadRight(50)}{currentBucket.ComplexityLevel:D2}";
            string taskKey = $"{grade:D2}{theme.PadRight(50)}{complexityLevel:D2}";

            return bucketKey.CompareTo(taskKey);
        }

        public bool _Scoop()
        {
            bool endOfStudentData = false;
            bool endOfTaskData = false;

            // Set current bucket data
            currentBucket.Grade = readerStudents.GetInt32(0);
            currentBucket.Theme = readerStudents.GetString(1);
            currentBucket.ComplexityLevel = readerStudents.GetInt32(2);
            currentBucket.Students.Clear();
            currentBucket.Students.Add(readerStudents.GetString(3));

            // Continue reading students while the current bucket is not empty
            while (!endOfStudentData && !endOfTaskData)
            {
                if (readerStudents.Read())
                {
                    if (CompareBucket(readerStudents.GetInt32(0), readerStudents.GetString(1), readerStudents.GetInt32(2)) == 0)
                        currentBucket.Students.Add(readerStudents.GetString(3));
                    else
                        endOfTaskData = true; // End of students in this bucket
                }
                else
                    endOfStudentData = true; // End of student data

                if (endOfStudentData) return false; // No more students in this bucket
                else return true;
            }
        }

        public void _Merge()
        {
            bool hasMoreData = readerStudents.Read();
            hasMoreData = Scoop();
            hasMoreData = readerTasks.Read();

            while (hasMoreData)
            {
                int compareResult = CompareBucket(readerTasks.GetInt32(0), readerTasks.GetString(2), readerTasks.GetInt32(4));

                if (compareResult < 0)
                    hasMoreData = Scoop(); // Move to the next bucket
                else if (compareResult == 0)
                {
                    // Add student tasks to DataTable
                    foreach (var student in currentBucket.Students)
                    {
                        dataTableStudentTasks.Rows.Add(student, currentBucket.Grade, currentBucket.Theme, currentBucket.ComplexityLevel, readerTasks.GetString(3));
                    }
                    hasMoreData = readerTasks.Read(); // Move to next task
                }
                else
                {
                    hasMoreData = readerTasks.Read(); // Move to next task if no match
                }
            }

            // Insert all data into the database
            bulkCopyStudentTasks.BatchSize = dataTableStudentTasks.Rows.Count;
            bulkCopyStudentTasks.BulkCopyTimeout = 600;
            bulkCopyStudentTasks.WriteToServer(dataTableStudentTasks);
            bulkCopyStudentTasks.Close();

            // Close readers and connections
            readerTasks.Close();
            readerStudents.Close();
            connectionTasks.Close();
            connectionStudents.Close();
        } */

    }
}
