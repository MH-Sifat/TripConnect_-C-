using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TripConnect
{
    public partial class AdminDashBoardAddTourSpots : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

        public AdminDashBoardAddTourSpots(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;
        }

        // add spots
        private void button1_Click(object sender, EventArgs e)
        {
            string spotName = richTextBox1.Text.Trim();
            string destination = richTextBox2.Text.Trim();
            string description = richTextBox3.Text.Trim();

            // Validation
            if (string.IsNullOrWhiteSpace(spotName) ||
                string.IsNullOrWhiteSpace(destination) ||
                string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("All fields are required.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"INSERT INTO TouristSpots 
                            (SpotName, Destination, Description)
                            VALUES (@SpotName, @Destination, @Description)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SpotName", spotName);
                    cmd.Parameters.AddWithValue("@Destination", destination);
                    cmd.Parameters.AddWithValue("@Description", description);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Tourist spot added successfully!",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearFields();
        }

           // CLEAR INPUT FIELDS
        private void ClearFields()
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            richTextBox3.Clear();
            richTextBox1.Focus();
        }

        // back button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            TouristSpots admin = new TouristSpots(UserId, UserRole);
            admin.Show();
        }
    }
}
