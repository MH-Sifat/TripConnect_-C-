using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TripConnect
{
    public partial class Home : Form
    {
        int UserId;
        string UserRole;
        public Home(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;
        }
        // open tourist spot page
        private void button1_Click(object sender, EventArgs e)
        {
           this.Hide();
            TouristSpots t1 = new TouristSpots(UserId, UserRole);
            t1.Show();
        }

        // check user role hide this button for tourist
        private void Home_Load(object sender, EventArgs e)
        {
            if (UserRole == "Guide")
            {
                button4.Visible = false;
                button5.Visible = false;
            }
        }

        // open Tourist group page
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            TouristGroups t1 = new TouristGroups(UserId, UserRole);
            t1.Show();
        }

        // open my profile page
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();    
            Profile p1 = new Profile(UserId, UserRole);
            p1.Show();  
        }

        // open create group page
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            CreateGroup c1 = new CreateGroup(UserId, UserRole);
            c1.Show();

        }

        // open trip notes page
        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            TripNotes t1 = new TripNotes(UserId, UserRole);
            t1.Show();
        }
    }
}
