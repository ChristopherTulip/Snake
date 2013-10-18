using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace Snake
{
    public partial class OptionsForm : Form
    {
        GameController game;

        public OptionsForm(GameController g)
        {
            InitializeComponent();
            game = g;
            
            if (game.gameStick.isConnected())
            {
                game.gameStick.displayText(xAccelLabel, yAccelLabel, zAccelLabel, orientationLabel);
            }
        }

        ~OptionsForm()
        {
            game.gameStick.stopDisplayingText();
            game = null;

        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                game.gameStick.OpenPort(comboBox1.SelectedItem.ToString());
                game.gameStick.displayText(xAccelLabel, yAccelLabel, zAccelLabel, orientationLabel);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!game.gameStick.demoMode)
            {
                game.pause();
                game.gameStick.demoMode = true;
            }
            else
            {
                game.gameStick.demoMode = false;
            }
        }
    }
}
