using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame_v_2
{
    class Program
    {
        static int x = 80;
        static int y = 26;

        static char a = 'y';

        static void Menu()
        {
            Console.SetWindowSize(x + 1, y + 1);
            Console.SetBufferSize(x + 1, y + 1);
            Console.CursorVisible = false;

            Walls walls = new Walls(x, y);
            walls.Draw();

            string m1 = "Welcome to the Snake Game";
            string m2 = "Start game y/n?";
            string m3 = "Press n to exit";

            Console.SetCursorPosition((Console.WindowWidth - m1.Length) / 2, (Console.WindowHeight - 4) / 2);
            Console.WriteLine(m1);

            Console.SetCursorPosition((Console.WindowWidth - m2.Length) / 2, Console.WindowHeight / 2);
            Console.WriteLine(m2);

            Console.SetCursorPosition((Console.WindowWidth - m3.Length) / 2, (Console.WindowHeight + 4) / 2);
            Console.WriteLine(m3);

            Console.SetCursorPosition(Console.WindowWidth / 2, (Console.WindowHeight + 7) / 2);
            a = Convert.ToChar(Console.ReadLine());
            Console.Clear();

        }
        static void Main(string[] args)
        {
            int count = 0;

            Menu();

            while (a == 'y')
            {
                string _Score = $"Score: {count}";

                int _bottomY = Console.WindowHeight - 1;
                Console.SetCursorPosition(0, _bottomY);
                Console.Write(_Score);

                Console.SetWindowSize(x + 1, y + 1);
                Console.SetBufferSize(x + 1, y + 1);
                Console.CursorVisible = false;

                Walls walls = new Walls(x, y);
                walls.Draw();

                Point p = new Point(x / 2, y / 2, '*');
                Snake snake = new Snake(p, 4, Direction.UP);
                snake.Draw();

                FoodCreator foodCreator = new FoodCreator(x, y, '@');
                Point food = foodCreator.CreateFood();
                food.Draw();

                while (true)
                {
                    if (walls.IsHit(snake) || snake.IsHitTail())
                    {
                        break;
                    }

                    else if (snake.Eat(food))
                    {
                        food = foodCreator.CreateFood();
                        food.Draw();

                        count = count + 1;

                        string Score = $"Score: {count}";

                        int bottomY = Console.WindowHeight - 1;
                        Console.SetCursorPosition(0, bottomY);
                        Console.Write(Score);
                    }

                    else
                    {
                        snake.Move();
                    }

                    Thread.Sleep(100);

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        snake.HandleKey(key.Key);
                    }
                }

                if (true)
                {
                    string str = $"Game Over, Your Score: {count}";
                    Console.SetCursorPosition((Console.WindowWidth - str.Length) / 2, (Console.WindowHeight - 4) / 2);
                    Console.WriteLine(str);

                    string str1 = "Play Again y/n?";
                    Console.SetCursorPosition((Console.WindowWidth - str1.Length) / 2, Console.WindowHeight / 2);
                    Console.WriteLine(str1);

                    Console.SetCursorPosition(Console.WindowWidth / 2, (Console.WindowHeight + 3) / 2);
                    a = Convert.ToChar(Console.ReadLine());
                    Console.Clear();

                    count = 0;
                }
            }
        }
    }

    class Snake : Figure
    {
        public Direction direction;

        public Snake(Point tail, int length, Direction _direction)
        {
            direction = _direction;
            pList = new List<Point>();
            for (int i = 0; i < length; i++)
            {
                Point p = new Point(tail);
                p.Move(i, direction);
                pList.Add(p);
            }
        }

        public void Move()
        {
            Point tail = pList.First();
            pList.Remove(tail);
            Point head = GetNextPoint();
            pList.Add(head);

            tail.Clear();
            head.Draw();
        }

        public Point GetNextPoint()
        {
            Point head = pList.Last();
            Point nextPoint = new Point(head);
            nextPoint.Move(1, direction);
            return nextPoint;
        }

        public bool IsHitTail()
        {
            var head = pList.Last();

            for (int i = 0; i < pList.Count - 2; i++)
            {
                if (head.IsHit(pList[i]))
                    return true;
            }

            return false;
        }

        public void HandleKey(ConsoleKey key)
        {
            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                    if (key == ConsoleKey.DownArrow)
                        direction = Direction.DOWN;

                    else if (key == ConsoleKey.UpArrow)
                        direction = Direction.UP;

                    break;

                case Direction.UP:
                case Direction.DOWN:
                    if (key == ConsoleKey.LeftArrow)
                        direction = Direction.LEFT;

                    else if (key == ConsoleKey.RightArrow)
                        direction = Direction.RIGHT;

                    break;
            }
        }

        internal bool Eat(Point food)
        {
            Point head = GetNextPoint();

            if (head.IsHit(food))
            {
                food.sym = head.sym;
                pList.Add(food);
                return true;
            }

            else
            {
                return false;
            }
        }
    }

    enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
    }

    class Point
    {
        public int x;
        public int y;
        public char sym;

        public Point()
        {

        }

        public Point(int _x, int _y, char _sym)
        {
            x = _x;
            y = _y;
            sym = _sym;
        }

        public Point(Point p)
        {
            x = p.x;
            y = p.y;
            sym = p.sym;
        }

        public void Move(int offset, Direction direction)
        {
            if (direction == Direction.RIGHT)
            {
                x = x + offset;
            }
            else if (direction == Direction.LEFT)
            {
                x = x - offset;
            }
            else if (direction == Direction.UP)
            {
                y = y - offset;
            }
            else if (direction == Direction.DOWN)
            {
                y = y + offset;
            }
        }

        public bool IsHit(Point p)
        {
            return p.x == this.x && p.y == this.y;
        }

        public void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.Write(sym);
        }

        public override string ToString()
        {
            return x + ", " + y + ", " + sym;
        }

        public void Clear()
        {
            sym = ' ';
            Draw();
        }
    }

    class Figure
    {
        protected List<Point> pList;

        public void Draw()
        {
            foreach (Point p in pList)
            {
                p.Draw();
            }
        }

        public bool IsHit(Figure figure)
        {
            foreach (var p in pList)
            {
                if (figure.IsHit(p))
                    return true;
            }
            return false;
        }

        private bool IsHit(Point point)
        {
            foreach (var p in pList)
            {
                if (p.IsHit(point))
                    return true;
            }
            return false;
        }
    }
    class HorizontalLine : Figure
    {
        public HorizontalLine(int xLeft, int xRight, int y, char sym)
        {
            pList = new List<Point>();

            for (int x = xLeft; x <= xRight; x++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class VerticalLine : Figure
    {
        public VerticalLine(int yUp, int yDown, int x, char sym)
        {
            pList = new List<Point>();

            for (int y = yUp; y <= yDown; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Walls
    {
        List<Figure> wallList;

        public Walls(int mapWidth, int mapHeight)
        {
            wallList = new List<Figure>();

            HorizontalLine upLine = new HorizontalLine(0, mapWidth - 2, 0, '#');
            HorizontalLine downLine = new HorizontalLine(0, mapWidth - 2, mapHeight - 1, '#');
            VerticalLine leftLine = new VerticalLine(0, mapHeight - 1, 0, '#');
            VerticalLine rightLine = new VerticalLine(0, mapHeight - 1, mapWidth - 2, '#');

            wallList.Add(upLine);
            wallList.Add(downLine);
            wallList.Add(leftLine);
            wallList.Add(rightLine);
        }

        public bool IsHit(Figure figure)
        {
            foreach (var wall in wallList)
            {
                if (wall.IsHit(figure))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw()
        {
            foreach (var wall in wallList)
            {
                wall.Draw();
            }
        }
    }

    class FoodCreator
    {
        private int mapWidth;
        private int mapHeight;
        private char sym;

        Random random = new Random();

        public FoodCreator(int mapWidth, int mapHeight, char sym)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.sym = sym;
        }

        public Point CreateFood()
        {
            int x = random.Next(2, mapWidth - 2);
            int y = random.Next(2, mapHeight - 2);
            return new Point(x, y, sym);
        }
    }
}
