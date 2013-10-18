using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public class GameController
    {
        #region Variables
        public static int TILE_WIDTH = 8;

        SnakeForm gameView;

        public GameStick gameStick { get; private set; }

        Timer gameTimer;

        int MAX_WIDTH;
        int MAX_HEIGHT;

        int score = 0;
        bool running = true;
        Snake snake;
        SnakePart food;
        #endregion

        public GameController(SnakeForm form)
        {
            gameView = form;
            gameStick = new GameStick(this);
            gameStick.OpenPort("COM5");

            MAX_WIDTH = gameView.getCanvas().Width / TILE_WIDTH;
            MAX_HEIGHT = gameView.getCanvas().Height / TILE_WIDTH;

            gameTimer = new Timer();
            gameTimer.Interval = 100;
            gameTimer.Tick += new EventHandler(Step);
        }

        #region GameLogic
        private void gameOver()
        {
            gameTimer.Stop();
            running = false;
        }

        private void Step(object sender, EventArgs e)
        {
            if (running)
            {
                updateSnake();
            }
            else
            {
                gameOver();
            }

            gameView.getCanvas().Invalidate();
        }

        private void updateSnake()
        {
            snake.moveSnake();

            if (snake.checkFood(food))
            {
                score++;
                generateFood();
            }
            else if (snake.checkForCollisions(MAX_HEIGHT, MAX_WIDTH))
            {
                gameOver();
            }
        }

        private void generateFood()
        {
            Random random = new Random();
            food = new SnakePart(
                        random.Next(0, MAX_HEIGHT),
                        random.Next(0, MAX_WIDTH)
                        );
        }
        #endregion

        #region Public Methods
        public void start()
        {
            // Generate Game Objects
            snake = new Snake();
            generateFood();
            
            //Update System Vars and Start Game
            score = 0;
            running = true;
            gameTimer.Start();
        }

        public void pause() { gameTimer.Stop(); }
        public void resume() { gameTimer.Start(); }

        public bool isRunning()     { return running; }
        public int  getScore()      { return score; }
        public Snake getSnake()     { return snake; }
        public SnakePart getFood()  { return food; }
        #endregion

        #region Controls
        public void updateDirection()
        {
            if (Input.Pressed(Keys.Right)) { turnRight(); }
            else if (Input.Pressed(Keys.Left)) { turnLeft(); }
        }

        public void updateDirection(int direction)
        {
            if (direction == GameStick.TURN_LEFT) { turnLeft(); }
            if (direction == GameStick.TURN_RIGHT) { turnRight(); }
            gameStick.resetState();
        }

        private void turnRight() { 
            snake.setDirection(snake.getDireciton() + 1);
        }
        private void turnLeft() { 
            snake.setDirection(snake.getDireciton() - 1);
        }
        #endregion
    }
}
