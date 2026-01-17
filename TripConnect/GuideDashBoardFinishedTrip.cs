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
using static System.Collections.Specialized.BitVector32;

namespace TripConnect
{
    public partial class GuideDashBoardFinishedTrip : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

        public GuideDashBoardFinishedTrip(int userId, string role)
        {
            InitializeComponent();
            UserId = userId;
            UserRole = role;
        }

        private void LoadFinishedTrips()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            tg.GroupID,
            tg.Destination,
            tg.TravelDate,
            tg.BudgetPerPerson,
            creator.UserName AS CreatedBy,
            guide.UserName AS GuideName,
            COUNT(gm.UserID) AS MemberCount
        FROM TourGroups tg
        JOIN Users creator ON tg.CreatedBy = creator.UserID
        LEFT JOIN Users guide ON tg.GuideID = guide.UserID
        LEFT JOIN GroupMembers gm ON tg.GroupID = gm.GroupID
        WHERE 
            tg.Status = 'Finished'
            AND tg.GuideID = @GuideID
        GROUP BY 
            tg.GroupID,
            tg.Destination,
            tg.TravelDate,
            tg.BudgetPerPerson,
            creator.UserName,
            guide.UserName";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GuideID", UserId); // logged in guide

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }

   

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Profile p1 = new Profile(UserId, UserRole);
            p1.Show();
        }

        private void GuideDashBoardFinishedTrip_Load(object sender, EventArgs e)
        {
            LoadFinishedTrips();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
