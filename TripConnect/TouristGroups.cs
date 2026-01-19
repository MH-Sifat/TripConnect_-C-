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
    public partial class TouristGroups : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
        string query;
        public TouristGroups(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;

            query = @"SELECT 
            tg.GroupID,
            creator.UserName AS CreatedBy,
            tg.Destination,
            tg.BudgetPerPerson,
            tg.TravelDate,
            tg.Status,
            guide.UserName AS GuideName,
            COUNT(gm.UserID) AS MemberCount,
            STRING_AGG(member.UserName, ', ') AS MemberNames
          FROM TourGroups tg
          JOIN Users creator ON tg.CreatedBy = creator.UserID
          LEFT JOIN Users guide ON tg.GuideID = guide.UserID
          LEFT JOIN GroupMembers gm ON tg.GroupID = gm.GroupID
          LEFT JOIN Users member ON gm.UserID = member.UserID
          WHERE tg.Status <> 'Finished'
          GROUP BY tg.GroupID,
                   creator.UserName,
                   tg.Destination,
                   tg.BudgetPerPerson,
                   tg.TravelDate,
                   tg.Status,
                   guide.UserName";

            LoadTourGroups(query);

        }
        private void TG_Load(object sender, EventArgs e)
        {

        }

        // load tourist groups
        private void LoadTourGroups(string query)
        {
            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Panel post = CreatePostCard(reader);
                    flowLayoutPanel1.Controls.Add(post);
                }
            }
        }

        // design card
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

            if (UserRole == "Tourist")  // show this for tourist
            {
                Button btnJoin = new Button();
                btnJoin.Text = "Join Group";
                btnJoin.Width = 120;
                btnJoin.Height = 35;
                btnJoin.Location = new Point(10, y);
                btnJoin.Font = new Font("Modern No. 20", 16.2f, FontStyle.Bold);
                btnJoin.BackColor = Color.SeaGreen;
                btnJoin.ForeColor = Color.White;
                btnJoin.Tag = reader["GroupID"]; // store GroupID
                btnJoin.Click += JoinGroup_Click;
                panel.Controls.Add(btnJoin);
            }
            if (UserRole == "Guide")  // show this for guide
            {
                Button btnReq = new Button();
                btnReq.Text = "Send Request";
                btnReq.Width = 150;
                btnReq.Height = 35;
                btnReq.Location = new Point(10, y);
                btnReq.Font = new Font("Modern No. 20", 12.2f, FontStyle.Bold);
                btnReq.BackColor = Color.Red;
                btnReq.ForeColor = Color.White;
                btnReq.Tag = reader["GroupID"]; // store GroupID
                btnReq.Click += SendReuquest_Click;

                panel.Controls.Add(btnReq);
            }

            return panel;
        }

        // create label
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

        // join group as tourist
        private void JoinGroup_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int groupId = Convert.ToInt32(btn.Tag);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @" IF NOT EXISTS ( SELECT 1 FROM GroupMembers 
                               WHERE GroupID = @GroupID AND UserID = @UserID )
                               BEGIN
                                   INSERT INTO GroupMembers (GroupID, UserID)
                                   VALUES (@GroupID, @UserID)
                               END";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GroupID", groupId);
                cmd.Parameters.AddWithValue("@UserID", UserId);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    MessageBox.Show("You have joined the group!",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("You are already a member of this group.",
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

           
            LoadTourGroups(query); // refresh UI
        }

        // send request as guide
        private void SendReuquest_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int groupId = Convert.ToInt32(btn.Tag);
            string Statuts = "Pending";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // ekhn theke korbo
                string query = @" IF NOT EXISTS ( SELECT 1 FROM GuideRequests 
                               WHERE GroupID = @GroupID AND GuideUserID = @GuideUserID )
                               BEGIN
                                   INSERT INTO GuideRequests (GroupID, GuideUserID,Status)
                                   VALUES (@GroupID, @GuideUserID,@Status)
                               END";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GroupID", groupId);
                cmd.Parameters.AddWithValue("@GuideUserID", UserId);
                cmd.Parameters.AddWithValue("@Status", Statuts);


                con.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    MessageBox.Show("Your Request is Sended to the group!",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("You Have Already Sended Request To The Group.",
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }


            LoadTourGroups(query); // refresh 
        }

        // search groups with destination, budget, group id
        private void button1_Click(object sender, EventArgs e)
        {
            string searchValue = richTextBox1.Text.Trim();


            string query = @"SELECT 
                           tg.GroupID,
                           creator.UserName AS CreatedBy,
                           tg.Destination,
                           tg.BudgetPerPerson,
                           tg.TravelDate,
                           tg.Status,
                           guide.UserName AS GuideName,
                           COUNT(gm.UserID) AS MemberCount,
                           STRING_AGG(member.UserName, ', ') AS MemberNames
                       FROM TourGroups tg
                       JOIN Users creator ON tg.CreatedBy = creator.UserID
                       LEFT JOIN Users guide ON tg.GuideID = guide.UserID
                       LEFT JOIN GroupMembers gm ON tg.GroupID = gm.GroupID
                       LEFT JOIN Users member ON gm.UserID = member.UserID
                       WHERE 
                           tg.Status <> 'Finished'
                           AND (
                            tg.Destination LIKE @search
                               OR CAST(tg.GroupID AS NVARCHAR) LIKE @search
                               OR CAST(tg.BudgetPerPerson AS NVARCHAR) LIKE @search
                        )
                       GROUP BY 
                           tg.GroupID,
                           creator.UserName,
                           tg.Destination,
                           tg.BudgetPerPerson,
                           tg.TravelDate,
                           tg.Status,
                           guide.UserName";
              

            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@search", "%" + searchValue + "%");

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                bool found = false;

                while (reader.Read())
                {
                    found = true;
                    Panel post = CreatePostCard(reader);
                    flowLayoutPanel1.Controls.Add(post);
                }

                if (!found)
                {
                    MessageBox.Show("No matching tour groups found.",
                                    "Search Result",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        // back button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();

            Home h1 = new Home(UserId, UserRole);
            h1.Show();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            // by mistake
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // by mistake

        }
    }
}
