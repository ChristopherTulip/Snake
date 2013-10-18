using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SnakeForm snakeForm = new SnakeForm();
            GameController game = new GameController(snakeForm);
            snakeForm.game = game;
            
            game.start();

            Application.Run(snakeForm);
        }
    }
}
