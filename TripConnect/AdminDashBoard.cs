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
    public partial class AdminDashBoard : Form
    {
        int UserId;
        string UserRole;
        public AdminDashBoard(int userId, string role)
        {
            InitializeComponent();
            this.UserId = userId;
            this.UserRole = role;
        }

        // Tourist spot page
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            TouristSpots t1 = new TouristSpots(UserId, UserRole);
            t1.Show();
        }

        // Tourist Page
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDashBoardTourists t2 = new AdminDashBoardTourists(UserId, UserRole);
            t2.Show();
        }

        private void AdminDashBoard_Load(object sender, EventArgs e)
        {
            // no need by mistake
        }

        // NOte Page
        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            TripNotes t3 = new TripNotes(UserId, UserRole);
            t3.Show();
        }

        // Tour Groups page
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDasBoardTourGroups tg = new AdminDasBoardTourGroups(UserId, UserRole);
            tg.Show();

        }

        // Guides Page
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDashBoardGuides a1 = new AdminDashBoardGuides(UserId, UserRole);
            a1.Show();
        }

        // admin profile
        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Profile p1 = new Profile(UserId, UserRole);
            p1.Show();
        }
    }
}
