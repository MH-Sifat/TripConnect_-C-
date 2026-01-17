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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            TouristSpots t1 = new TouristSpots(UserId, UserRole);
            t1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminDashBoardTourists t2 = new AdminDashBoardTourists();
            t2.Show();
        }

        private void AdminDashBoard_Load(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            TripNotes t3 = new TripNotes(UserId, UserRole);
            t3.Show();
        }
    }
}
