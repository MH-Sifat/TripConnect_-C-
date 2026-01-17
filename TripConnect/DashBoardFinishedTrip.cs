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
    public partial class DashBoardFinishedTrip : Form
    {

        int UserId;
        string UserRole;

        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
        string query;
        public DashBoardFinishedTrip(int userId, string role)
        {
            InitializeComponent();
            UserId = userId;
            UserRole = role;

            query = @"SELECT DISTINCT
    tg.GroupID,
    tg.Destination,
    tg.BudgetPerPerson,
    tg.TravelDate,
    tg.Status,
    creator.UserName AS CreatedBy,
    guide.UserName AS GuideName,
    COUNT(gm2.UserID) AS MemberCount
FROM TourGroups tg
LEFT JOIN Users creator ON tg.CreatedBy = creator.UserID
LEFT JOIN Users guide ON tg.GuideID = guide.UserID
LEFT JOIN GroupMembers gm ON tg.GroupID = gm.GroupID
LEFT JOIN GroupMembers gm2 ON tg.GroupID = gm2.GroupID
WHERE 
    tg.Status = 'Finished'
    AND (tg.CreatedBy = @UserId OR gm.UserID = @UserId)
GROUP BY
    tg.GroupID,
    tg.Destination,
    tg.BudgetPerPerson,
    tg.TravelDate,
    tg.Status,
    creator.UserName,
    guide.UserName";


            LoadFinishedTrips();
        }

        private void LoadFinishedTrips()
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
                Font = new Font("Segoe UI", 12)
            };
        }

        private DataTable GetTripNotes(int groupId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT NoteId, Content 
                         FROM TripNotes 
                         WHERE GroupID = @GroupID AND UserID = @UserID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GroupID", groupId);
                cmd.Parameters.AddWithValue("@UserID", UserId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        private Panel CreatePostCard(SqlDataReader reader)
        {
            Panel panel = new Panel();
            panel.Width = 520;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(10);
            panel.BackColor = Color.White;

            int y = 10;
            int groupId = Convert.ToInt32(reader["GroupID"]);

            // Destination
            Label lblDestination = new Label()
            {
                Text = reader["Destination"].ToString(),
                Font = new Font("Modern No. 20", 16.2f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, y)
            };
            panel.Controls.Add(lblDestination);
            y += 35;

            panel.Controls.Add(CreateLabel($"Group ID: {groupId}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Created By: {reader["CreatedBy"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Budget: {reader["BudgetPerPerson"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Travel Date: {Convert.ToDateTime(reader["TravelDate"]).ToShortDateString()}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Members: {reader["MemberCount"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Guide: {reader["GuideName"]}", y)); y += 25;
            panel.Controls.Add(CreateLabel($"Status: {reader["Status"]}", y)); y += 20;

            // Notes label
            panel.Controls.Add(new Label()
            {
                Text = "Your Trip Note:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, y),
                AutoSize = true
            });
            y += 25;

            // Note textbox
            TextBox txtNote = new TextBox()
            {
                Width = 480,
                Height = 60,
                Multiline = true,
                Location = new Point(10, y)
            };
            panel.Controls.Add(txtNote);
            y += 70;

            // Load existing note
            LoadUserNote(groupId, txtNote);

            // Save button
            Button btnSave = new Button()
            {
                Text = "Save / Update Note",
                Width = 150,
                Height = 35,
                Location = new Point(10, y),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                Tag = new Tuple<int, TextBox>(groupId, txtNote)
            };
            btnSave.Click += SaveNote_Click;
            panel.Controls.Add(btnSave);

            // Delete button
            Button btnDelete = new Button()
            {
                Text = "Delete Note",
                Width = 120,
                Height = 35,
                Location = new Point(170, y),
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                Tag = new Tuple<int, TextBox>(groupId, txtNote)
            };
            btnDelete.Click += DeleteNote_Click;
            panel.Controls.Add(btnDelete);

            y += 45;
            panel.Height = y + 10;

            return panel;
        }

        private void LoadUserNote(int groupId, TextBox txt)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT Content FROM TripNotes WHERE GroupID=@g AND UserID=@u", con);
                cmd.Parameters.AddWithValue("@g", groupId);
                cmd.Parameters.AddWithValue("@u", UserId);

                con.Open();
                var result = cmd.ExecuteScalar();
                txt.Text = result == null ? "" : result.ToString();
            }
        }

        private void SaveNote_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var data = (Tuple<int, TextBox>)btn.Tag;

            int groupId = data.Item1;
            TextBox txt = data.Item2;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                MessageBox.Show("Write something first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string q = @" IF EXISTS (SELECT 1 FROM TripNotes WHERE GroupID=@g AND UserID=@u)
                              UPDATE TripNotes SET Content=@c WHERE GroupID=@g AND UserID=@u
                          ELSE
                              INSERT INTO TripNotes (UserID, GroupID, Content)
                              VALUES (@u, @g, @c)";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@u", UserId);
                cmd.Parameters.AddWithValue("@g", groupId);
                cmd.Parameters.AddWithValue("@c", txt.Text.Trim());

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Note saved successfully!");
        }

        private void DeleteNote_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var data = (Tuple<int, TextBox>)btn.Tag;

            int groupId = data.Item1;
            TextBox txt = data.Item2;

            DialogResult dr = MessageBox.Show(
                "Are you sure you want to delete your note?",
                "Confirm", MessageBoxButtons.YesNo);

            if (dr != DialogResult.Yes) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM TripNotes WHERE GroupID=@g AND UserID=@u", con);
                cmd.Parameters.AddWithValue("@g", groupId);
                cmd.Parameters.AddWithValue("@u", UserId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            txt.Clear();
            MessageBox.Show("Note deleted.");
        }


        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Profile p1 = new Profile(UserId, UserRole);
            p1.Show();
        }
    }
}
