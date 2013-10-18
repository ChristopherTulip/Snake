using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snake;

namespace Snake
{
    public class Snake
    {
        #region variables
        const int NORTH     = 0;
        const int EAST      = 1;
        const int SOUTH     = 2;
        const int WEST      = 3;

        List<SnakePart> snake;

        public int direction = EAST;
        #endregion

        public Snake()
        {
            snake = new List<SnakePart>();
            snake.Add(new SnakePart(30, 30));
            snake.Add(new SnakePart(29, 30));
        }

        public void moveSnake()
        {
            // move body
            for (int i = snake.Count - 1; i >= 1; i--)
            {
                snake[i].X = snake[i - 1].X;
                snake[i].Y = snake[i - 1].Y;
            }

            // move head
            switch (direction)
            {
                case NORTH:
                    snake[0].Y--;
                    break;
                case EAST:
                    snake[0].X++;
                    break;
                case SOUTH:
                    snake[0].Y++;
                    break;
                case WEST:
                    snake[0].X--;
                    break;
            }
        }

        public bool checkFood(SnakePart food)
        {
            if (getHead().X == food.X && getHead().Y == food.Y) {
                snake.Add(new SnakePart(food.X, food.Y));
                return true;     
            }
            else {
                return false;
            }
        }

        public bool checkForCollisions(int maxHeight, int maxWidth)
        {
            for (int i = 1; i < snake.Count; i++) 
            {
                if (getHead().X == snake[i].X && getHead().Y == snake[i].Y)
                {
                    return true;
                }
            }
            // check max bounds
            if (getHead().X < 0 || getHead().X >= maxWidth || getHead().Y < 0 || getHead().Y >= maxHeight)
            {
                return true;
            }

            return false;
        }

        public int getLength() {
            return snake.Count;
        }

        public void setDirection(int i)
        {
            if (i > WEST)       direction = NORTH;
            else if (i < NORTH) direction = WEST;
            else                direction = i;
        }

        public int getDireciton() { return direction; }

        public SnakePart getMember(int index)
        {
            return snake[index];
        }

        private SnakePart getHead()
        {
            return snake[0];
        }

        private void addPart()
        {
            switch (direction) {
                case NORTH:
                    snake.Add(new SnakePart(
                                snake[snake.Count - 1].X,
                                snake[snake.Count - 1].Y + 1)
                                );
                    break;
                case EAST:
                    snake.Add(new SnakePart(
                                snake[snake.Count - 1].X - 1,
                                snake[snake.Count - 1].Y)
                                );
                    break;
                case SOUTH:
                    snake.Add(new SnakePart(
                                snake[snake.Count - 1].X,
                                snake[snake.Count - 1].Y - 1)
                                );
                    break;
                case WEST:
                    snake.Add(new SnakePart(
                                snake[snake.Count - 1].X + 1,
                                snake[snake.Count - 1].Y)
                                );
                    break;
            }
        }
    }
}
