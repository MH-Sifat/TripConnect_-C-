using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TripConnect
{
    public partial class Home : Form
    {
        int UserId;
        string UserRole;
        public Home(int userId, string role)
        {
            InitializeComponent();
            UserId = userId;
            UserRole = role;
        }
        private void button1_Click(object sender, EventArgs e)
        {
          /*  this.Hide();
            TouristSpots t1 = new TouristSpots(UserId, UserRole);
            t1.Show();*/
        }

        private void Home_Load(object sender, EventArgs e)
        {
            if (UserRole == "Guide")
            {
                button4.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();    
            Profile p1 = new Profile();
            p1.Show();  
        }

        private void button4_Click(object sender, EventArgs e)
        {
          
        }
    }
}
