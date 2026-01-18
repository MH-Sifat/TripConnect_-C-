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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TripConnect
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // No need it was by mistake
        }

        // password Hide and seek using checkBox 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }

        }

        // Log In
        private void button1_Click(object sender, EventArgs e)
        {

            string connectionString = "data source=DESKTOP-BHNO9D6; database=TripConnectDB; integrated security=SSPI";

            string userEmail = textBox1.Text;
            string userPassword = textBox2.Text;
            if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(userPassword))
            {

                MessageBox.Show("Please enter both Email and Password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //  MessageBox.Show( userEmail + "\n" + userPassword);
            string query = "SELECT UserID, Role FROM Users WHERE Email = @Email AND Password = @Password";
           
            using (SqlConnection connection = new SqlConnection(connectionString))

            {

                using (SqlCommand command = new SqlCommand(query, connection))

                {

                    command.Parameters.AddWithValue("@Email", userEmail);

                    command.Parameters.AddWithValue("@Password", userPassword);

                    connection.Open();

                   // int count = (int)command.ExecuteScalar();
                    SqlDataReader reader = command.ExecuteReader();
                    //count > 0 &&
                    if (reader.Read())
                    {
                        int userId = Convert.ToInt32(reader["UserID"]);
                       string role = reader["Role"].ToString();

                       MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       this.Hide();

                        /*  Home h1 = new Home(userId, role);
                          h1.Show(); */
                        // ROLE BASED REDIRECTION
                        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Hide();
                            AdminDashBoard admin = new AdminDashBoard(userId, role);
                            admin.Show();
                        }
                        else
                        {
                            this.Hide();
                            Home h1 = new Home(userId, role);
                            h1.Show();
                        }

                    }

                    else

                    {

                        MessageBox.Show("Invalid Email or userPassword.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }

            }

        }

        // Sign Up
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignUp s1 = new SignUp();
            s1.Show();

        }

        // Forget Password
        private void button3_Click(object sender, EventArgs e)
        {
            
            ForgetPassword fp = new ForgetPassword();
            fp.ShowDialog(); 
        }
    }
}
