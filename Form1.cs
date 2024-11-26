using Microsoft.Data.SqlClient;

namespace MyDatabase
{
    public partial class Form1 : Form
    {

        DBService DBService = new DBService(
            @"Server=COLORKAT-LAPTOP;Database=MySchool;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
//            @"Server=COLORKAT-LAPTOP;Database=MySchool;Encrypt=True;TrustServerCertificate=True "
        );

        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void uploadCSVButton_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM Students";
            SqlDataReader reader = DBService.ExecuteReader(query);

            while (reader.Read())
            {
                string studentName = reader["student_name"].ToString();
                int grade = Convert.ToInt32(reader["grade"]);
                MessageBox.Show($"Student: {studentName}, Grade: {grade}");
            }

            reader.Close();
        }

        private void mergeTablesButton_Click(object sender, EventArgs e)
        {

        }
    }
}
