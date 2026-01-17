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
    public partial class GuideDashBoardAcceptReq : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

        public GuideDashBoardAcceptReq(int userId, string role)
        {
            InitializeComponent();
            UserId = userId;
            UserRole = role;

            LoadAcceptedGroups();

        }

        private void LoadAcceptedGroups()
        {
            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT 
                                                tg.GroupID,
                                                tg.Destination,
                                                tg.BudgetPerPerson,
                                                tg.TravelDate,
                                                tg.Status,
                                                creator.UserName AS CreatedBy
                                            FROM TourGroups tg
                                            JOIN Users creator ON tg.CreatedBy = creator.UserID
                                            WHERE tg.GuideID = @GuideID", con);

                cmd.Parameters.AddWithValue("@GuideID", UserId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    flowLayoutPanel1.Controls.Add(CreateGroupCard(reader));
                }
            }
        }

        private Panel CreateGroupCard(SqlDataReader reader)
        {
            Panel panel = new Panel
            {
                Width = 520,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10),
                BackColor = Color.White
            };

            int y = 10;

            panel.Controls.Add(CreateTitle(reader["Destination"].ToString(), y));
            y += 35;

            panel.Controls.Add(CreateLabel($"Group ID: {reader["GroupID"]}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Created By: {reader["CreatedBy"]}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Budget: {reader["BudgetPerPerson"]}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Travel Date: {Convert.ToDateTime(reader["TravelDate"]).ToShortDateString()}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Status: {reader["Status"]}", y)); y += 30;

            Button btnLeave = new Button
            {
                Text = "Leave Group",
                Width = 140,
                Height = 35,
                Location = new Point(10, y),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Tag = reader["GroupID"]
            };

            btnLeave.Click += LeaveGroup_Click;
            panel.Controls.Add(btnLeave);

            panel.Height = y + 55;
            return panel;
        }

        private void LeaveGroup_Click(object sender, EventArgs e)
        {
            int groupId = Convert.ToInt32(((Button)sender).Tag);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to leave this group?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction tx = con.BeginTransaction();

                try
                {
                    // 1️⃣ Remove guide from the group
                    SqlCommand cmd1 = new SqlCommand(
                        "UPDATE TourGroups SET GuideID = NULL WHERE GroupID = @gid AND GuideID = @uid",
                        con, tx);
                    cmd1.Parameters.AddWithValue("@gid", groupId);
                    cmd1.Parameters.AddWithValue("@uid", UserId);
                    cmd1.ExecuteNonQuery();

                    // 2️⃣ DELETE guide request completely
                    SqlCommand cmd2 = new SqlCommand(
                        "DELETE FROM GuideRequests WHERE GroupID = @gid AND GuideUserID = @uid",
                        con, tx);
                    cmd2.Parameters.AddWithValue("@gid", groupId);
                    cmd2.Parameters.AddWithValue("@uid", UserId);
                    cmd2.ExecuteNonQuery();

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    MessageBox.Show("Something went wrong. Please try again.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            LoadAcceptedGroups();

            MessageBox.Show("You have left the group successfully.",
                "Left Group", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /*  private void LeaveGroup_Click(object sender, EventArgs e)
          {
              int groupId = Convert.ToInt32(((Button)sender).Tag);

              DialogResult result = MessageBox.Show(
                  "Are you sure you want to leave this group?",
                  "Confirm",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Warning);

              if (result != DialogResult.Yes) return;

              using (SqlConnection con = new SqlConnection(connectionString))
              {
                  con.Open();

                  // Remove guide from group
                  SqlCommand cmd1 = new SqlCommand(
                      "UPDATE TourGroups SET GuideID = NULL WHERE GroupID = @gid", con);
                  cmd1.Parameters.AddWithValue("@gid", groupId);
                  cmd1.ExecuteNonQuery();

                  // Optional: update guide request status
                  SqlCommand cmd2 = new SqlCommand(
                      "UPDATE GuideRequests SET Status = 'Left' WHERE GroupID = @gid AND GuideUserID = @uid", con);
                  cmd2.Parameters.AddWithValue("@gid", groupId);
                  cmd2.Parameters.AddWithValue("@uid", UserId);
                  cmd2.ExecuteNonQuery();
              }

              LoadAcceptedGroups();

              MessageBox.Show("You have left the group successfully.",
                  "Left Group", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }*/

        private Label CreateLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
        }

        private Label CreateTitle(string text, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Modern No. 20", 16, FontStyle.Bold)
            };
        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Profile p1 = new Profile(UserId, UserRole);
            p1.Show();
        }
    }
}
