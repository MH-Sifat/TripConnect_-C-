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
    public partial class TG1 : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
        public TG1(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;
            string query = @" SELECT 
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
                             GROUP BY tg.GroupID,
                             creator.UserName,
                             tg.Destination,
                             tg.BudgetPerPerson,
                             tg.TravelDate,
                             tg.Status,
                             guide.UserName";
            FillDataGridView(query);
        }

        private void FillDataGridView(string query)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGridView1.DataSource = dataTable;

                    /* dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                     dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                     dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                     // Fixed columns
                     dataGridView1.Columns["GroupId"].Width = 60;

                     dataGridView1.Columns["CreatedBy"].AutoSizeMode =
                         DataGridViewAutoSizeColumnMode.Fill;

                     dataGridView1.Columns["Destination"].Width = 80;

                     // Auto columns
                     dataGridView1.Columns["BudgetPerPerson"].AutoSizeMode =
                         DataGridViewAutoSizeColumnMode.AllCells;


                     dataGridView1.Columns["TravelDate"].AutoSizeMode =
                         DataGridViewAutoSizeColumnMode.Fill;

                     dataGridView1.Columns["Members"].AutoSizeMode =
                         DataGridViewAutoSizeColumnMode.Fill;

                     dataGridView1.Columns["Status"].AutoSizeMode =
                         DataGridViewAutoSizeColumnMode.Fill;

                     dataGridView1.Columns["GuideId"].AutoSizeMode =
                         DataGridViewAutoSizeColumnMode.Fill; */
                 /*   dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    dataGridView1.Columns["GroupID"].Width = 60;
                    dataGridView1.Columns["CreatedBy"].Width = 120;
                    dataGridView1.Columns["Destination"].Width = 100;
                    dataGridView1.Columns["BudgetPerPerson"].Width = 80;
                    dataGridView1.Columns["TravelDate"].Width = 100;
                    dataGridView1.Columns["Status"].Width = 80;
                    dataGridView1.Columns["GuideName"].Width = 120;
                    dataGridView1.Columns["MemberCount"].Width = 80;
                    dataGridView1.Columns["MemberNames"].Width = 200;*/

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();

            Home h1 = new Home(UserId, UserRole);
            h1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
