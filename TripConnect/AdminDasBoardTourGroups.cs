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
    public partial class AdminDasBoardTourGroups : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
        public AdminDasBoardTourGroups(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;
            LoadTourGroups();
        }

        // Load Tour group details
        private void LoadTourGroups()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                              tg.GroupID,
                              tg.Destination,
                              tg.TravelDate,
                              tg.BudgetPerPerson,
                              tg.Status,
                              creator.UserName AS CreatedBy,
                              ISNULL(guide.UserName, 'No Guide') AS GuideName,
                              COUNT(gm.UserID) AS MemberCount
                          FROM TourGroups tg
                          JOIN Users creator ON tg.CreatedBy = creator.UserID
                          LEFT JOIN Users guide ON tg.GuideID = guide.UserID
                          LEFT JOIN GroupMembers gm ON tg.GroupID = gm.GroupID
                          GROUP BY 
                              tg.GroupID,
                              tg.Destination,
                              tg.TravelDate,
                              tg.BudgetPerPerson,
                              tg.Status,
                              creator.UserName,
                              guide.UserName
                          ORDER BY tg.GroupID DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                // UI 
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
            }
        }

        // back button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDashBoard a1 = new AdminDashBoard(UserId, UserRole);
            a1.Show();
        }
    }
}
