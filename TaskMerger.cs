﻿using Microsoft.Data.SqlClient;
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
            public int class_number;
            public string topic;
            public string subject;
            public int complexity_level;
        }

        private struct Student
        {
            public string id;
            public string name;
            public int class_number;
        }

        // Bucket is a struct that stores 
        // a bunch of students that have the same criterias.
        // (In this case - class number)
        private struct Bucket
        {
            public string student_id;
            public string task_id;
            public List<Task> Tasks; // List of tasks
        }

        private readonly DBService _dbService;

        private DataTable _studentTaskAssigmentsTable;
        private SqlBulkCopy _bulkCopy;

        private Bucket _currentBucket = new Bucket();
        private Task _currentTask;

        public TaskMerger(
            DBService dbService
        )
        {
            _dbService = dbService;

            // This table is a join of students and tasks
            _studentTaskAssigmentsTable = new DataTable();
            _studentTaskAssigmentsTable.Columns.Add("id", typeof(string));
            _studentTaskAssigmentsTable.Columns.Add("student_id", typeof(string));
            _studentTaskAssigmentsTable.Columns.Add("student_name", typeof(string));
            _studentTaskAssigmentsTable.Columns.Add("class", typeof(int));

            _studentTaskAssigmentsTable.Columns.Add("task_id", typeof(string));
            _studentTaskAssigmentsTable.Columns.Add("subject", typeof(string));
            _studentTaskAssigmentsTable.Columns.Add("topic", typeof(string));
            _studentTaskAssigmentsTable.Columns.Add("complexity_level", typeof(int));

            _bulkCopy = new SqlBulkCopy(_dbService.Connection)
            {
                DestinationTableName = "StudentTaskAssigments"
            };

            _bulkCopy.ColumnMappings.Add("id", "");
            _bulkCopy.ColumnMappings.Add("student_id", "");
            _bulkCopy.ColumnMappings.Add("student_name", "student_name");
            _bulkCopy.ColumnMappings.Add("class_number", "class");
            _bulkCopy.ColumnMappings.Add("task_id", "task_id");
            _bulkCopy.ColumnMappings.Add("subject", "subject");
            _bulkCopy.ColumnMappings.Add("topic", "topic");
            _bulkCopy.ColumnMappings.Add("complexity_level", "complexity_level");
        }

        private int CompareBucket(int classNumber)
        {
            string bucketKey = $"{_currentBucket.task_id.PadRight(10)}";
            string taskKey = $"{classNumber.ToString().PadRight(10)}";

            return bucketKey.CompareTo(taskKey);
        }

        private bool Scoop()
        {
            return false;
        }

        public void MergeTasks()
        {
            Student currentSudent = new Student();

            using (var studentTaskReader = _dbService.ExecuteReader("SELECT * FROM StudentTask ORDER BY student_id"))
            {
                while(studentTaskReader.Read()) {

                    string studentId = studentTaskReader.GetString(1);
                    string taskId = studentTaskReader.GetString(2);

                    using (var taskReader = _dbService.ExecuteReader($"SELECT * FROM Tasks WHERE id = {taskId}  ORDER BY id"))
                    {
                        Task task = new Task();
                        task.id = taskId;
                        task.class_number = taskReader.GetInt16(1);
                        task.subject = taskReader.GetString(2);
                        task.topic = taskReader.GetString(3);
                        task.complexity_level = taskReader.GetInt16(4);

                        _currentBucket.Tasks.Add(task);
                    }

                    // Reach new student
                    if (_currentBucket.student_id != studentId)
                    {
                        _currentBucket.student_id = studentId;

                        using (var studentReader = _dbService.ExecuteReader($"SELECT * FROM Students WHERE id = {studentId} ORDER BY id"))
                        {
                            currentSudent.id = studentReader.GetString(0);
                            currentSudent.name = studentReader.GetString(1);
                            currentSudent.class_number = studentReader.GetInt16(2);
                        }
                    }




                        if(!_currentBucket.student_id.Equals(studentId)) {
                            var students = studentReader.Read();
                        }

                        
                        foreach (var student in _currentBucket.Students) {
                            _studentTaskAssigmentsTable.Rows.Add(
                                student.id,
                                student.name,
                                student.class_number,
                                taskReader.GetString(0), // task_id
                                taskReader.GetString(2), // task subject
                                taskReader.GetString(3), // task topic
                                taskReader.GetString(4)  // task complexity level
                            );
                        }
                        

                        // После завершения цикла вставляем все данные в таблицу
                        _bulkCopy.BatchSize = _studentTaskAssigmentsTable.Rows.Count;
                        _bulkCopy.BulkCopyTimeout = 600;
                        _bulkCopy.WriteToServer(_studentTaskAssigmentsTable);
                        _bulkCopy.Close();
                    }
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
