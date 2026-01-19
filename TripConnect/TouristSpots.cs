
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace TripConnect
{
    public partial class TouristSpots : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
        public TouristSpots(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;   
            this.UserRole = role;
            string query = "SELECT * FROM TouristSpots";
            FillDataGridView(query);
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        // Tourist spot details load
        private void FillDataGridView(string query)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGridView1.DataSource = dataTable;
                    dataGridView1.ReadOnly = true;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    // Fixed columns
                    dataGridView1.Columns["SpotId"].Width = 60;
                    dataGridView1.Columns["Destination"].Width = 80;

                    // Auto columns
                    dataGridView1.Columns["SpotName"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;


                    dataGridView1.Columns["Description"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                        
                }
            }
        }

        // search button
        private void button1_Click(object sender, EventArgs e)
        {
            string searchValue = richTextBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchValue))
            {
                MessageBox.Show("Please enter a search term.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"SELECT * FROM TouristSpots
            WHERE SpotName LIKE @searchTerm
            OR Destination LIKE @searchTerm";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchTerm", "%" + searchValue + "%");

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGridView1.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No matching rows found.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
           // by mistake
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // by mistake
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // by mistake
        }

        // add tourist spot as admin
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDashBoardAddTourSpots t1 = new AdminDashBoardAddTourSpots(UserId, UserRole);
            t1.Show();
        }

        // show this button only for admin
        private void TouristSpots_Load(object sender, EventArgs e)
        {
            if (UserRole != "Admin")
            {
                button3.Visible = false;
                button4.Visible = false;
            }
        }
        // admin select row and delelte tourits spots
        private void button4_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a tourist spot to delete.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get SpotId from selected row
            int spotId = Convert.ToInt32(
                dataGridView1.SelectedRows[0].Cells["SpotId"].Value);

            // Confirmation
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this tourist spot?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
                return;

            // Delete query
            string query = "DELETE FROM TouristSpots WHERE SpotId = @SpotId";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SpotId", spotId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Tourist spot deleted successfully.",
                "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Refresh DataGridView
            FillDataGridView("SELECT * FROM TouristSpots");

        }

        // Back Button 
        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Hide();

            if (UserRole == "Admin")
            {
                AdminDashBoard a1 = new AdminDashBoard(UserId, UserRole);
                a1.Show();
            }
            else
            {
                Home h1 = new Home(UserId, UserRole);
                h1.Show();
            }
        }
    }
}
