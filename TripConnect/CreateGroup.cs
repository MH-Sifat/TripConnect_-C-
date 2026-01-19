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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TripConnect
{
    public partial class CreateGroup : Form
    {
        int UserId;
        string UserRole;
        public CreateGroup(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;
        }

        // create tour group
        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
            string Destination = richTextBox1.Text.Trim();
            string BudgetPerPerson = richTextBox2.Text.Trim();
            string TravelDate = dateTimePicker1.Text.Trim();
            string Status = null;
            if (radioButton1.Checked)
            {
                Status = radioButton1.Text.Trim();
            }
            else if (radioButton2.Checked)
            {
                Status = radioButton2.Text.Trim();
            }


            if (string.IsNullOrWhiteSpace(Destination) || string.IsNullOrWhiteSpace(BudgetPerPerson) ||
                string.IsNullOrWhiteSpace(TravelDate) || string.IsNullOrWhiteSpace(Status))
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO TourGroups (Destination,BudgetPerPerson,TravelDate,Status,CreatedBy) VALUES (@Destination,@BudgetPerPerson,@TravelDate,@Status,@CreatedBy)";

            using (SqlConnection connection = new SqlConnection(connectionString))

            {

                using (SqlCommand command = new SqlCommand(query, connection))

                {

                    command.Parameters.AddWithValue("@Destination", Destination);

                    command.Parameters.AddWithValue("@BudgetPerPerson", BudgetPerPerson);


                    command.Parameters.Add("@TravelDate", SqlDbType.Date).Value = dateTimePicker1.Value.Date;


                    command.Parameters.AddWithValue("@Status", Status);

                    command.Parameters.AddWithValue("@CreatedBy", UserId);


                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)

                    {
                        MessageBox.Show("TripConnect Tour Group created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetForm();

                    }

                    else

                    {

                        MessageBox.Show("Failed to create the Group. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }

            }


        }

        // reset text box
        private void ResetForm()
        {
            // Clear text fields
            richTextBox1.Clear();   // Destination
            richTextBox2.Clear();   // Budget

            // Uncheck radio buttons
            radioButton1.Checked = false;
            radioButton2.Checked = false;

            // Reset DateTimePicker
            dateTimePicker1.Value = DateTime.Today;
        }

        // back button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();

            Home h1 = new Home(UserId, UserRole);
            h1.Show();
        }
    }
}
