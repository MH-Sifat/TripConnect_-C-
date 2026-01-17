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
    public partial class TripNotes : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

        public TripNotes(int userId, string role)
        {
            InitializeComponent();
            UserId = userId;
            UserRole = role;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();

            Home h1 = new Home(UserId, UserRole);
            h1.Show();
        }

        private void TripNotes_Load(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            LoadFinishedTrips();
        }

        private void LoadFinishedTrips()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT 
                                             tg.GroupID,
                                             tg.Destination,
                                             tg.TravelDate,
                                             tg.Status,
                                             creator.UserName AS CreatedBy,
                                             guide.UserName AS GuideName,
                                             COUNT(gm.UserID) AS MemberCount
                                         FROM TourGroups tg
                                         LEFT JOIN Users creator ON tg.CreatedBy = creator.UserID
                                         LEFT JOIN Users guide ON tg.GuideID = guide.UserID
                                         LEFT JOIN GroupMembers gm ON tg.GroupID = gm.GroupID
                                         WHERE tg.Status = 'Finished'
                                         GROUP BY 
                                             tg.GroupID,
                                             tg.Destination,
                                             tg.TravelDate,
                                             tg.Status,
                                             creator.UserName,
                                             guide.UserName", con);


                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    flowLayoutPanel1.Controls.Add(CreateTripNotesCard(reader));
                }
            }
        }

        private Panel CreateTripNotesCard(SqlDataReader reader)
        {
            Panel panel = new Panel();
            panel.Width = 600;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(10);
            panel.BackColor = Color.White;

            int y = 10;
            int groupId = Convert.ToInt32(reader["GroupID"]);

            panel.Controls.Add(CreateBoldLabel(reader["Destination"].ToString(), y));
            y += 35;

            panel.Controls.Add(CreateLabel($"Group ID: {groupId}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Created By: {reader["CreatedBy"]}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Travel Date: {Convert.ToDateTime(reader["TravelDate"]).ToShortDateString()}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Members: {reader["MemberCount"]}", y)); y += 22;
            panel.Controls.Add(CreateLabel($"Guide: {reader["GuideName"]}", y)); y += 30;

            // Notes title
            panel.Controls.Add(new Label()
            {
                Text = "Notes",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, y),
                AutoSize = true
            });
            y += 25;

            // Notes container
            Panel notesPanel = new Panel()
            {
                Location = new Point(10, y),
                Width = 560,
                AutoSize = true
            };

            LoadNotes(groupId, notesPanel);

            panel.Controls.Add(notesPanel);
            panel.Height = y + notesPanel.Height + 15;

            return panel;
        }

        private void LoadNotes(int groupId, Panel notesPanel)
        {
            int y = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@" SELECT u.UserName, n.Content FROM TripNotes n
                                                JOIN Users u ON n.UserID = u.UserID
                                                WHERE n.GroupID=@g", con);

                cmd.Parameters.AddWithValue("@g", groupId);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    notesPanel.Controls.Add(new Label()
                    {
                        Text = "No notes available.",
                        ForeColor = Color.Gray,
                        Location = new Point(0, 0),
                        AutoSize = true
                    });
                    notesPanel.Height = 25;
                    return;
                }

                while (reader.Read())
                {
                    Panel noteCard = new Panel()
                    {
                        Width = 540,
                        Height = 70,
                        BorderStyle = BorderStyle.FixedSingle,
                        Location = new Point(0, y),
                        BackColor = Color.AliceBlue
                    };

                    noteCard.Controls.Add(new Label()
                    {
                        Text = reader["UserName"].ToString(),
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        Location = new Point(5, 5),
                        AutoSize = true
                    });

                    noteCard.Controls.Add(new Label()
                    {
                        Text = reader["Content"].ToString(),
                        Location = new Point(5, 25),
                        MaximumSize = new Size(520, 0),
                        AutoSize = true
                    });

                    notesPanel.Controls.Add(noteCard);
                    y += noteCard.Height + 8;
                }

                notesPanel.Height = y;
            }
        }

        private Label CreateLabel(string text, int y)
        {
            return new Label()
            {
                Text = text,
                Location = new Point(10, y),
                AutoSize = true
            };
        }

        private Label CreateBoldLabel(string text, int y)
        {
            return new Label()
            {
                Text = text,
                Font = new Font("Modern No. 20", 16, FontStyle.Bold),
                Location = new Point(10, y),
                AutoSize = true
            };
        }


    }
}
