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
    public partial class DashBoardJoinedGroups : Form
    {
        int UserId;
        string UserRole;

        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
        string query;
        public DashBoardJoinedGroups(int userId, string role)
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
                     COUNT(gm2.UserID) AS MemberCount
                 FROM GroupMembers gm
                 JOIN TourGroups tg ON gm.GroupID = tg.GroupID
                 JOIN Users creator ON tg.CreatedBy = creator.UserID
                 LEFT JOIN Users guide ON tg.GuideID = guide.UserID
                 LEFT JOIN GroupMembers gm2 ON tg.GroupID = gm2.GroupID
                 WHERE 
                     gm.UserID = @UserId
                     AND tg.Status <> 'Finished'
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

        private Panel CreatePostCard(SqlDataReader reader)
        {
            Panel panel = new Panel();
            panel.Width = 450;
            panel.Height = 270;
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

            // LEAVE GROUP BUTTON
            Button btnLeave = new Button();
            btnLeave.Text = "Leave Group";
            btnLeave.Width = 120;
            btnLeave.Height = 35;
            btnLeave.Location = new Point(10, y);
            btnLeave.BackColor = Color.BlueViolet;
            btnLeave.ForeColor = Color.White;
            btnLeave.Tag = reader["GroupID"];
            btnLeave.Click += LeaveGroup_Click;

            panel.Controls.Add(btnLeave);

            return panel;
        }

        private void LeaveGroup_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int groupId = Convert.ToInt32(btn.Tag);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to leave this group?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "DELETE FROM GroupMembers WHERE GroupID = @GroupID AND UserID = @UserID",
                        con);

                    cmd.Parameters.AddWithValue("@GroupID", groupId);
                    cmd.Parameters.AddWithValue("@UserID", UserId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("You have left the group successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadTourGroups(query);
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Profile p1 = new Profile(UserId, UserRole);
            p1.Show();
            LoadTourGroups(query);
        }
    }
}
