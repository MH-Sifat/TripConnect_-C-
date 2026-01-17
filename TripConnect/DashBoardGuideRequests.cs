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
    public partial class DashBoardGuideRequests : Form
    {
        int GroupId;
        int UserId;
        string UserRole;

        string connectionString =
            "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
        public DashBoardGuideRequests(int groupId, int userId, string role)
        {
            InitializeComponent();
            GroupId = groupId;
            UserId = userId;
            UserRole = role;

            LoadGuideRequests();
        }

        private void LoadGuideRequests()
        {
            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                               gr.RequestID,
                               gr.GuideUserID,
                               gr.Status,
                               u.UserName,
                               u.Email,
                               u.PhoneNumber
                           FROM GuideRequests gr
                           JOIN Users u ON gr.GuideUserID = u.UserID
                           WHERE gr.GroupID = @GroupID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GroupID", GroupId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Panel card = CreateGuideCard(reader);
                    flowLayoutPanel1.Controls.Add(card);
                }
            }
        }

        private Panel CreateGuideCard(SqlDataReader reader)
        {
            Panel panel = new Panel();
            panel.Width = 450;
            panel.Height = 180;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(10);
            panel.BackColor = Color.White;

            int y = 10;
            panel.Controls.Add(CreateLabel($"Guide ID: {reader["GuideUserID"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Name: {reader["UserName"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Email: {reader["Email"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Phone: {reader["PhoneNumber"]}", y)); y += 30;

            string status = reader["Status"].ToString();
            int guideId = Convert.ToInt32(reader["GuideUserID"]);
            int requestId = Convert.ToInt32(reader["RequestID"]);

            // ACCEPT BUTTON
            Button btnAccept = new Button();
            btnAccept.Text = "Accept Guide";
            btnAccept.Width = 130;
            btnAccept.Height = 35;
            btnAccept.Location = new Point(10, y);
            btnAccept.BackColor = Color.SeaGreen;
            btnAccept.ForeColor = Color.White;
            btnAccept.Tag = new Tuple<int, int>(requestId, guideId);
            btnAccept.Click += AcceptGuide_Click;

            // REMOVE BUTTON
            Button btnRemove = new Button();
            btnRemove.Text = "Remove Guide";
            btnRemove.Width = 130;
            btnRemove.Height = 35;
            btnRemove.Location = new Point(150, y);
            btnRemove.BackColor = Color.Firebrick;
            btnRemove.ForeColor = Color.White;
            btnRemove.Tag = guideId;
            btnRemove.Click += RemoveGuide_Click;
            btnRemove.Visible = false;

            // STATUS HANDLING
            if (status == "Confirm")
            {
                btnAccept.Text = "Accepted";
                btnAccept.Enabled = false;
                btnAccept.BackColor = Color.Gray;
                btnRemove.Visible = true;
            }
            else if (IsGuideAlreadyAssigned())
            {
                btnAccept.Enabled = false;
                btnAccept.BackColor = Color.Gray;
            }

            panel.Controls.Add(btnAccept);
            panel.Controls.Add(btnRemove);

            return panel;
        }

        private void AcceptGuide_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var data = (Tuple<int, int>)btn.Tag;

            int requestId = data.Item1;
            int guideId = data.Item2;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // assign guide to group
                SqlCommand cmd1 = new SqlCommand(
                    "UPDATE TourGroups SET GuideID = @GuideID WHERE GroupID = @GroupID", con);
                cmd1.Parameters.AddWithValue("@GuideID", guideId);
                cmd1.Parameters.AddWithValue("@GroupID", GroupId);
                cmd1.ExecuteNonQuery();

                // mark request as confirmed
                SqlCommand cmd2 = new SqlCommand(
                    "UPDATE GuideRequests SET Status = 'Confirm' WHERE RequestID = @ReqID", con);
                cmd2.Parameters.AddWithValue("@ReqID", requestId);
                cmd2.ExecuteNonQuery();
            }

            MessageBox.Show("Guide accepted successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadGuideRequests();
        }

        private void RemoveGuide_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int guideId = Convert.ToInt32(btn.Tag);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // remove guide from group
                SqlCommand cmd1 = new SqlCommand(
                    "UPDATE TourGroups SET GuideID = NULL WHERE GroupID = @GroupID", con);
                cmd1.Parameters.AddWithValue("@GroupID", GroupId);
                cmd1.ExecuteNonQuery();

                // reset guide request status
                SqlCommand cmd2 = new SqlCommand(
                    "UPDATE GuideRequests SET Status = 'Pending' WHERE GroupID = @GroupID AND GuideUserID = @GuideUserID", con);
                cmd2.Parameters.AddWithValue("@GroupID", GroupId);
                cmd2.Parameters.AddWithValue("@GuideUserID", guideId);
                cmd2.ExecuteNonQuery();
            }

            MessageBox.Show("Guide removed successfully!", "Removed",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadGuideRequests();
        }

        private bool IsGuideAlreadyAssigned()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT GuideID FROM TourGroups WHERE GroupID = @GroupID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GroupID", GroupId);

                con.Open();
                object result = cmd.ExecuteScalar();

                return result != DBNull.Value && result != null;
            }
        }

        private Label CreateLabel(string text, int y)
        {
            return new Label()
            {
                Text = text,
                AutoSize = true,
                Location = new Point(10, y),
                Font = new Font("Segoe UI", 11)
            };
        }


        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            DashBoardMyGroups d1 = new DashBoardMyGroups(UserId, UserRole);
            d1.Show();
        }
    }
}
