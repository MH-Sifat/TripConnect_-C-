using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TripConnect
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        // Sign Up
        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";
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

            // MessageBox.Show(userName + "\n" + userEmail + "\n" + userPhnNo + "\n" + userRole + "\n" + userEmergencyCon + "\n" + userPass );

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userEmail) ||
                string.IsNullOrWhiteSpace(userPhnNo) || string.IsNullOrWhiteSpace(userRole) ||
               string.IsNullOrWhiteSpace(userEmergencyCon) || string.IsNullOrWhiteSpace(userPass))
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO Users (UserName, Email, PhoneNumber,Password,Role,EmergencyContact) VALUES (@UserName, @Email, @PhoneNumber, @Password, @Role, @EmergencyContact)";

            using (SqlConnection connection = new SqlConnection(connectionString))

            {

                using (SqlCommand command = new SqlCommand(query, connection))

                {

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

                        MessageBox.Show("TripConnect Profile created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();

                        LogIn l1 = new LogIn();
                        l1.Show();

                    }

                    else

                    {

                        MessageBox.Show("Failed to create the profile. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }

            }

        }

        // Back to Log In
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            LogIn l1 = new LogIn();
            l1.Show();
        }

        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // No need it was by mistake
        }

        // password Hide and seek using checkBox
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
    }
}
