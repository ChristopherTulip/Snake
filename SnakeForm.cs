using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class SnakeForm : Form
    {
        #region Variables
        public GameController game { get; set; }
        #endregion

        #region Constructor
        public SnakeForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Drawing
        public PictureBox getCanvas() { return gameCanvas; }

        private void gameCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (game == null) { return; }

            if (!game.isRunning())
            {
                Font font = this.Font;
                string gameover_msg = "Gameover";
                string score_msg = "Score: " + game.getScore().ToString();
                int center_width = gameCanvas.Width / 2;
                SizeF msg_size = canvas.MeasureString(gameover_msg, font);
                PointF msg_point = new PointF(center_width - msg_size.Width / 2, 16);
                canvas.DrawString(gameover_msg, font, Brushes.White, msg_point);
                msg_size = canvas.MeasureString(score_msg, font);
                msg_point = new PointF(center_width - msg_size.Width / 2, 32);
                canvas.DrawString(score_msg, font, Brushes.White, msg_point);
            }
            else
            {
                for (int i = 0; i < game.getSnake().getLength(); i++)
                {
                    Brush snake_color = i == 0 ? Brushes.Red : Brushes.Black;
                    canvas.FillRectangle(snake_color, 
                        new Rectangle(
                            game.getSnake().getMember(i).X * GameController.TILE_WIDTH,
                            game.getSnake().getMember(i).Y * GameController.TILE_WIDTH, 
                            GameController.TILE_WIDTH, 
                            GameController.TILE_WIDTH)
                            );
                }

                canvas.FillRectangle(Brushes.Orange, 
                    new Rectangle(game.getFood().X * GameController.TILE_WIDTH,
                                    game.getFood().Y * GameController.TILE_WIDTH, 
                                    GameController.TILE_WIDTH, 
                                    GameController.TILE_WIDTH)
                                    );

                canvas.DrawString("Score: " + game.getScore().ToString(),
                                this.Font, Brushes.White, new PointF(4, 4));
            }
        }
        #endregion

        #region Keyboard Controls
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
            game.updateDirection();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        #endregion

        #region OptionsMenu
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.start();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsForm opts = new OptionsForm(game);
            opts.Show();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.pause();
        }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.resume();
        }

        #endregion

    }
}
