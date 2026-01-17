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
        private void button1_Click(object sender, EventArgs e)
        {
           this.Hide();
            TouristSpots t1 = new TouristSpots(UserId, UserRole);
            t1.Show();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            if (UserRole == "Guide")
            {
                button4.Visible = false;
                button5.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            TouristGroups t1 = new TouristGroups(UserId, UserRole);
            t1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();    
            Profile p1 = new Profile(UserId, UserRole);
            p1.Show();  
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            /*  TG1 t1 = new TG1(UserId, UserRole);
             t1.Show();*/
            CreateGroup c1 = new CreateGroup(UserId, UserRole);
            c1.Show();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            TripNotes t1 = new TripNotes(UserId, UserRole);
            t1.Show();
        }
    }
}
