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
    public partial class GuideDashBoardSentReq : Form
    {

        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

        public GuideDashBoardSentReq(int userId, string role)
        {
            InitializeComponent();
            UserId = userId;
            UserRole = role;
            LoadSentRequests();
        }

        private void LoadSentRequests()
        {
            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
SELECT 
    gr.RequestID,
    tg.GroupID,
    tg.Destination,
    tg.BudgetPerPerson,
    tg.TravelDate,
    tg.Status AS GroupStatus,
    creator.UserName AS CreatedBy,
    gr.Status AS RequestStatus
FROM GuideRequests gr
JOIN TourGroups tg ON gr.GroupID = tg.GroupID
JOIN Users creator ON tg.CreatedBy = creator.UserID
WHERE 
    gr.GuideUserID = @GuideUserID
    AND gr.Status = 'Pending'", con);

                cmd.Parameters.AddWithValue("@GuideUserID", UserId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    flowLayoutPanel1.Controls.Add(CreateRequestCard(reader));
                }
            }
        }

        private Panel CreateRequestCard(SqlDataReader reader)
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
            panel.Controls.Add(CreateLabel($"Group Status: {reader["GroupStatus"]}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Request Status: {reader["RequestStatus"]}", y)); y += 30;

            // CANCEL REQUEST BUTTON (OPTIONAL)
            Button btnCancel = new Button
            {
                Text = "Cancel Request",
                Width = 140,
                Height = 35,
                Location = new Point(10, y),
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                Tag = reader["RequestID"]
            };

            btnCancel.Click += CancelRequest_Click;
            panel.Controls.Add(btnCancel);

            panel.Height = y + 55;
            return panel;
        }

        private void CancelRequest_Click(object sender, EventArgs e)
        {
            int requestId = Convert.ToInt32(((Button)sender).Tag);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to cancel this request?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM GuideRequests WHERE RequestID = @id", con);

                cmd.Parameters.AddWithValue("@id", requestId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadSentRequests();
        }

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
