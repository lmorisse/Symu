using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymuExamples
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var home = new BeliefsAndInfluence.Home();
            home.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var home = new GroupAndInteraction.Home();
            home.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var home = new LearnAndForget.Home();
            home.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var home = new MessageAndTask.Home();
            home.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var home = new MurphiesAndBlockers.Home();
            home.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var home = new ScenariosAndEvents.Home();
            home.Show();
        }
    }
}
