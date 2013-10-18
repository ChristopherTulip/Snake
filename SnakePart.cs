using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    public class SnakePart
    {
        public int X { get; set; } // X Coordinate for Snake Part
        public int Y { get; set; } // Y Coordinate for Snake Part

        // Constructor
        public SnakePart(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
