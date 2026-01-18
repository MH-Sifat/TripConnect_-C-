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
    public partial class AdminDashBoardGuides : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

        public AdminDashBoardGuides(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;
            LoadGuides();
        }

        private void LoadGuides()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                UserID,
                UserName,
                Email,
                PhoneNumber
            FROM Users
            WHERE Role = 'guide'";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                // UI polish
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
                dataGridView1.ReadOnly = true;
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDashBoard a1 = new AdminDashBoard(UserId, UserRole);
            a1.Show();
        }
    }
}
