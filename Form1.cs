using Microsoft.Data.SqlClient;

namespace MyDatabase
{
    public partial class Form1 : Form
    {

        private readonly DBService _dbService;
        private readonly DataImporter _dataImporter;

        public Form1()
        {
            InitializeComponent();

            // Manually instantiate services (simple DI setup)
            _dbService = new DBService(@"Server=COLORKAT-LAPTOP;Database=MySchool;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");
            _dataImporter = new DataImporter(_dbService);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void uploadCSVButton_Click(object sender, EventArgs e)
        {
            /*
            string query = "SELECT * FROM Students";
            SqlDataReader reader = DBService.ExecuteReader(query);

            while (reader.Read())
            {
                string studentName = reader["student_name"].ToString();
                int grade = Convert.ToInt32(reader["grade"]);
                MessageBox.Show($"Student: {studentName}, Grade: {grade}");
            }

            reader.Close();
            */

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _dataImporter.importDataFromTxt(openFileDialog.FileName, "Tasks");
            }


        }

        private void mergeTablesButton_Click(object sender, EventArgs e)
        {

        }
    }
}
