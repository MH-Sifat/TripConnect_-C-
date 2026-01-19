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
       // guide details
        private void LoadGuides()
        {
            string query = @"SELECT 
                                UserID,
                                UserName,
                                Email,
                                PhoneNumber,
                                Role
                             FROM Users
                             WHERE Role = 'guide'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }


        // back button

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDashBoard a1 = new AdminDashBoard(UserId, UserRole);
            a1.Show();
        }

        // delete guide
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a guide to delete.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedGuideId = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["UserID"].Value);

            DialogResult result = MessageBox.Show(
                "This will permanently delete the guide and all related data.\n\nAre you sure?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 1. Trip notes written by guide
                    Exec(con, tran,
                        "DELETE FROM TripNotes WHERE UserID = @uid", selectedGuideId);

                    // 2. Guide requests handled by guide
                    Exec(con, tran,
                        "DELETE FROM GuideRequests WHERE GuideUserID = @uid",
                        selectedGuideId);

                    // 3. Remove guide from groups
                    Exec(con, tran,
                        "UPDATE TourGroups SET GuideID = NULL WHERE GuideID = @uid",
                        selectedGuideId);

                    // 4. Remove guide from group members
                    Exec(con, tran,
                        "DELETE FROM GroupMembers WHERE UserID = @uid",
                        selectedGuideId);

                    // 5. Delete guide account
                    Exec(con, tran,
                        "DELETE FROM Users WHERE UserID = @uid",
                        selectedGuideId);

                    tran.Commit();

                    MessageBox.Show("Guide account deleted successfully.",
                        "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadGuides();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(
                        "Delete failed:\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // Helper method
        private void Exec(SqlConnection con, SqlTransaction tran, string sql, int uid)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con, tran))
            {
                cmd.Parameters.AddWithValue("@uid", uid);
                cmd.ExecuteNonQuery();
            }
        }

    }
}
