using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TripConnect
{
    public partial class Profile : Form
    {
        int UserId;
        string UserRole;
        string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

        public Profile(int userId, string role)
        {
            InitializeComponent();
            UserId = userId;
            UserRole = role;
            LoadDetails();
        }
        private void LoadDetails()
        {
            string query = "SELECT UserName, Email, PhoneNumber, Password, Role, EmergencyContact  FROM Users WHERE UserId = @UserId";
            //   string query = "INSERT INTO Users (UserName, Email, PhoneNumber,Password,Role,EmergencyContact) VALUES (@UserName, @Email, @PhoneNumber, @Password, @Role, @EmergencyContact)";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", UserId);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        // Populate the text boxes with the retrieved data
                        textBox1.Text = reader["UserName"].ToString();
                        textBox2.Text = reader["Email"].ToString();
                        textBox3.Text = reader["PhoneNumber"].ToString();
                        string Role = reader["Role"].ToString();

                        if (Role == "Tourist")
                        {
                            radioButton1.Checked = true;
                        }
                        else
                        {
                            radioButton2.Checked = true;
                        }


                        textBox4.Text = reader["EmergencyContact"].ToString();

                        PassTextBox.Text = reader["Password"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No details found for the given ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close(); // Close the form if no data is found
                    }
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            if (UserRole == "Guide")
            {
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
            }
            if (UserRole == "Tourist")
            {
                button8.Visible = false;
                button9.Visible = false;
                button10.Visible = false;
            }
        }

        // Back button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();

            Home h1 = new Home(UserId, UserRole);
            h1.Show();
        }

        // myGroups button
        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();

            DashBoardMyGroups d1 = new DashBoardMyGroups(UserId, UserRole);
            d1.Show();
        }

        // updtae
        private void button2_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text.Trim();
            string userEmail = textBox2.Text.Trim();
            string userPhnNo = textBox3.Text.Trim();
            string userRole = null;

            if (radioButton1.Checked)
            {
                userRole = radioButton1.Text.Trim();
            }
            else if (radioButton2.Checked)
            {
                userRole = radioButton2.Text.Trim();
            }

            string userEmergencyCon = textBox4.Text.Trim();
            string userPass = PassTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userEmail) ||
                string.IsNullOrWhiteSpace(userPhnNo) || string.IsNullOrWhiteSpace(userRole) ||
               string.IsNullOrWhiteSpace(userEmergencyCon) || string.IsNullOrWhiteSpace(userPass))
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE Users SET UserName = @UserName, Email = @Email, PhoneNumber = @PhoneNumber, Password = @Password, Role = @Role, EmergencyContact = @EmergencyContact WHERE UserId = @UserId";
            //  string query = "UPDATE InfoSignUp SET Name = @Name, Age = @Age, Address = @Address WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))

            {

                using (SqlCommand command = new SqlCommand(query, connection))

                {
                    command.Parameters.AddWithValue("@UserId", UserId);

                    command.Parameters.AddWithValue("@UserName", userName);

                    command.Parameters.AddWithValue("@Email", userEmail);

                    command.Parameters.AddWithValue("@PhoneNumber", userPhnNo);

                    command.Parameters.AddWithValue("@Password", userPass);

                    command.Parameters.AddWithValue("@Role", userRole);

                    command.Parameters.AddWithValue("@EmergencyContact", userEmergencyCon);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)

                    {

                        MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    }

                    else

                    {

                        MessageBox.Show("No record was updated. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "Are you sure you want to delete this profile?",
             "Confirm Deletion",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Warning);


            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Users WHERE UserId = @UserId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", UserId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Profile deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("No profile was found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

        }
        // log out 
        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show(
           "Are you sure you want to logout?",
           "Logout Confirmation",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Clear session values (optional but good practice)
                UserId = 0;
                UserRole = null;
                this.Hide();
                // Open Login Form
                LogIn l1 = new LogIn();
                l1.Show();

                // Close / hide current Profile form
               

            }
        }

        // Joined Groups
        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            DashBoardJoinedGroups d2 = new DashBoardJoinedGroups(UserId, UserRole);
            d2.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            DashBoardFinishedTrip d2 = new DashBoardFinishedTrip(UserId, UserRole);
            d2.Show();
        }

        private void SignUpPasscheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (SignUpPasscheckBox1.Checked)
            {
                PassTextBox.UseSystemPasswordChar = false;
            }
            else
            {
                PassTextBox.UseSystemPasswordChar = true;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Hide();
            GuideDashBoardSentReq g1 = new GuideDashBoardSentReq(UserId, UserRole);
            g1.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Hide();
            GuideDashBoardAcceptReq g2 = new GuideDashBoardAcceptReq(UserId, UserRole);
            g2.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Hide();
            GuideDashBoardFinishedTrip g3 = new GuideDashBoardFinishedTrip(UserId, UserRole);
            g3.Show();
        }
    }
}
