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
    public partial class AdminDashBoardTourists : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

        public AdminDashBoardTourists(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;
            LoadTourists();
        }

        private void LoadTourists()
        {
            string query = @"SELECT 
                                UserID,
                                UserName,
                                Email,
                                PhoneNumber,
                                Role
                             FROM Users
                             WHERE Role = 'tourist'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    dataGridView1.DataSource = dt;

                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
        }

        // delete user
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDashBoard a1 =new  AdminDashBoard(UserId, UserRole);
            a1.Show();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            // no need

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a tourist to delete.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedUserId = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["UserID"].Value);

            DialogResult result = MessageBox.Show(
                "This will permanently delete the user and ALL related data.\n\nAre you sure?",
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
                    // 1. Trip notes written by user
                    Exec(con, tran,
                        "DELETE FROM TripNotes WHERE UserID = @uid", selectedUserId);

                    // 2. Trip notes of groups created by user
                    Exec(con, tran,
                        @"DELETE FROM TripNotes 
                  WHERE GroupID IN (
                      SELECT GroupID FROM TourGroups WHERE CreatedBy = @uid
                  )", selectedUserId);

                    // 3. Guide requests involving user
                    Exec(con, tran,
                        @"DELETE FROM GuideRequests 
                  WHERE GuideUserID = @uid",
                        selectedUserId);

                    // 4. Guide requests of groups created by user  ✅ FIX
                    Exec(con, tran,
                        @"DELETE FROM GuideRequests 
                  WHERE GroupID IN (
                      SELECT GroupID FROM TourGroups WHERE CreatedBy = @uid
                  )", selectedUserId);

                    // 5. Group members
                    Exec(con, tran,
                        "DELETE FROM GroupMembers WHERE UserID = @uid", selectedUserId);

                    Exec(con, tran,
                        @"DELETE FROM GroupMembers 
                  WHERE GroupID IN (
                      SELECT GroupID FROM TourGroups WHERE CreatedBy = @uid
                  )", selectedUserId);

                    // 6. Remove user as guide
                    Exec(con, tran,
                        "UPDATE TourGroups SET GuideID = NULL WHERE GuideID = @uid",
                        selectedUserId);

                    // 7. Delete groups created by user
                    Exec(con, tran,
                        "DELETE FROM TourGroups WHERE CreatedBy = @uid",
                        selectedUserId);

                    // 8. Delete user
                    Exec(con, tran,
                        "DELETE FROM Users WHERE UserID = @uid",
                        selectedUserId);

                    tran.Commit();

                    MessageBox.Show("Tourist account deleted successfully.",
                        "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadTourists();
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

        // Helper method (clean + reusable)
        private void Exec(SqlConnection con, SqlTransaction tran, string sql, int uid)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con, tran))
            {
                cmd.Parameters.AddWithValue("@uid", uid);
                cmd.ExecuteNonQuery();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // by mistake
        }
    }
}
