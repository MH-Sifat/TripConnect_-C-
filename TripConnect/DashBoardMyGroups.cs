using System;
using System.Collections;
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
    public partial class DashBoardMyGroups : Form
    {
        int UserId;
        string UserRole;

        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
        string query;

        public DashBoardMyGroups(int userId, string role)
        {
            InitializeComponent();
            UserId = userId;
            UserRole = role;
            query = @"SELECT 
                     tg.GroupID,
                     tg.Destination,
                     tg.BudgetPerPerson,
                     tg.TravelDate,
                     tg.Status,
                     creator.UserName AS CreatedBy,
                     guide.UserName AS GuideName,
                     COUNT(gm.UserID) AS MemberCount
                 FROM TourGroups tg
                 LEFT JOIN Users creator ON tg.CreatedBy = creator.UserID
                 LEFT JOIN Users guide ON tg.GuideID = guide.UserID
                 LEFT JOIN GroupMembers gm ON tg.GroupID = gm.GroupID
                 WHERE tg.CreatedBy = @UserId
                 GROUP BY 
                    tg.GroupID,
                    tg.Destination,
                    tg.BudgetPerPerson,
                    tg.TravelDate,
                    tg.Status,
                    creator.UserName,
                    guide.UserName";


            LoadTourGroups(query);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void LoadTourGroups(string query)
        {
            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", UserId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Panel post = CreatePostCard(reader);
                    flowLayoutPanel1.Controls.Add(post);
                }
            }
        }
        private Panel CreatePostCard(SqlDataReader reader)
        {
            Panel panel = new Panel();
            panel.Width = 450;
          //  panel.Height = 270;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(10);
            panel.BackColor = Color.White;

            int y = 10;

            // DESTINATION 
            Label lblDestination = new Label();
            lblDestination.Text = reader["Destination"].ToString();
            lblDestination.Font = new Font("Modern No. 20", 16.2f, FontStyle.Bold);
            lblDestination.AutoSize = true;
            lblDestination.Location = new Point(10, y);
            lblDestination.ForeColor = Color.Black;

            panel.Controls.Add(lblDestination);
            y += 35;

            // Other labels
            panel.Controls.Add(CreateLabel($"Group Id: {reader["GroupId"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Created By: {reader["CreatedBy"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Budget: {reader["BudgetPerPerson"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Travel Date: {Convert.ToDateTime(reader["TravelDate"]).ToShortDateString()}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Members: {reader["MemberCount"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Guide: {reader["GuideName"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Status: {reader["Status"]}", y)); y += 30;

            // FINISH TRIP BUTTON
            Button btnFinish = new Button();
            btnFinish.Text = "Finish Trip";
            btnFinish.Width = 120;
            btnFinish.Height = 35;
            btnFinish.Location = new Point(10, y);
            btnFinish.BackColor = Color.DarkOrange;
            btnFinish.ForeColor = Color.White;
            btnFinish.Tag = reader["GroupID"];
            btnFinish.Click += FinishTrip_Click;
            panel.Controls.Add(btnFinish);

            string status = reader["Status"].ToString();

            // DELETE BUTTON
            Button btnDelete = new Button();
            btnDelete.Text = "Delete Group";
            btnDelete.Width = 120;
            btnDelete.Height = 35;
            btnDelete.Location = new Point(140, y);
            btnDelete.BackColor = Color.Firebrick;
            btnDelete.ForeColor = Color.White;
            btnDelete.Tag = reader["GroupID"];
            btnDelete.Click += DeleteGroup_Click;
            if (status == "Finished")
            {
                btnDelete.Visible = false;
                btnFinish.Visible = false;
            }

            panel.Controls.Add(btnDelete);


            // Confirm BUTTON
            Button btnConfirm = new Button();
            btnConfirm.Text = "Confirm Trip";
            btnConfirm.Width = 120;
            btnConfirm.Height = 35;
            btnConfirm.Location = new Point(270, y);
            btnConfirm.BackColor = Color.Blue;
            btnConfirm.ForeColor = Color.White;
            btnConfirm.Tag = reader["GroupID"];
            btnConfirm.Click += ConfirmTrip_Click;

            if (status == "Confirm" || status == "Finished")
            {
                btnConfirm.Visible = false;
            }

            panel.Controls.Add(btnConfirm);

           // GUIDE REQUESTS BUTTON
            Button btnGuideRequests = new Button();
            btnGuideRequests.Text = "Guide Requests";
            btnGuideRequests.Width = 120;
            btnGuideRequests.Height = 35;
            btnGuideRequests.Location = new Point(10, y + 45);
            btnGuideRequests.BackColor = Color.SeaGreen;
            btnGuideRequests.ForeColor = Color.White;
            btnGuideRequests.Tag = reader["GroupID"];
            btnGuideRequests.Click += GuideRequests_Click;

            // hide if trip finished
            if (status == "Finished")
            {
                btnGuideRequests.Visible = false;
            }

            panel.Controls.Add(btnGuideRequests);

            panel.Height = y + 90;

            return panel;
        }

        // Finished trip
        private void FinishTrip_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int groupId = Convert.ToInt32(btn.Tag);

            string query = "UPDATE TourGroups SET Status = 'Finished' WHERE GroupID = @GroupID";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GroupID", groupId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Trip marked as Finished!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadTourGroups(this.query);
        }

        // confirm
        private void ConfirmTrip_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int groupId = Convert.ToInt32(btn.Tag);

            string query = "UPDATE TourGroups SET Status = 'Confirm' WHERE GroupID = @GroupID";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GroupID", groupId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Trip confirmed successfully!",
                "Confirmed", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadTourGroups(this.query);
        }

        // Delete Group
        private void DeleteGroup_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int groupId = Convert.ToInt32(btn.Tag);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this group?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Remove members first (FK safety)
                    SqlCommand cmd1 = new SqlCommand(
                        "DELETE FROM GroupMembers WHERE GroupID = @GroupID", con);
                    cmd1.Parameters.AddWithValue("@GroupID", groupId);
                    cmd1.ExecuteNonQuery();

                    // Remove group
                    SqlCommand cmd2 = new SqlCommand(
                        "DELETE FROM TourGroups WHERE GroupID = @GroupID", con);
                    cmd2.Parameters.AddWithValue("@GroupID", groupId);
                    cmd2.ExecuteNonQuery();
                }

                MessageBox.Show("Group deleted successfully!",
                    "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadTourGroups(this.query);
            }
        }

        // Guide Request
        private void GuideRequests_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int groupId = Convert.ToInt32(btn.Tag);
            this.Hide();
            DashBoardGuideRequests gr = new DashBoardGuideRequests(groupId, UserId, UserRole);
            gr.Show();
        }


        private Label CreateLabel(string text, int y)
        {
            return new Label()
            {
                Text = text,
                AutoSize = true,
                Location = new Point(10, y),
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
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
