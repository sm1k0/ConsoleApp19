using System;
using System.Collections.Generic;
using System.Threading;

public enum MapSize
{
    MaxRightBorder = 31,
    MaxLowerBorder = 21
}

public class Snake
{
    public List<(int x, int y)> Body { get; set; }
    public (int x, int y) Direction { get; set; }

    public Snake((int x, int y) startPosition, (int x, int y) direction)
    {
        Body = new List<(int x, int y)> { startPosition };
        Direction = direction;
    }

    public void Move()
    {
        (int x, int y) newHead = (Body[0].x + Direction.x, Body[0].y + Direction.y);
        Body.Insert(0, newHead);
        Body.RemoveAt(Body.Count - 1);
    }

    public void Grow()
    {
        (int x, int y) newTail = Body[Body.Count - 1];
        Body.Add(newTail);
    }

    public bool IsSelfCollision()
    {
        (int x, int y) head = Body[0];
        return Body.FindAll(segment => segment.x == head.x && segment.y == head.y).Count > 1;
    }

    public bool IsOutOfBounds()
    {
        (int x, int y) head = Body[0];
        return head.x < 0 || head.x > (int)MapSize.MaxRightBorder || head.y < 0 || head.y > (int)MapSize.MaxLowerBorder;
    }

    public bool Eat(Food food)
    {
        if (Body[0].Equals(food.Position))
        {
            Grow();
            return true;
        }
        return false;
    }
}

public class Food
{
    public (int x, int y) Position { get; set; }

    public Food((int x, int y) position)
    {
        Position = position;
    }
}

public class Game
{
    private Snake _snake;
    private Random _random;
    private Food _food;

    public Game()
    {
        _snake = new Snake((10, 10), (1, 0));
        _random = new Random();
        GenerateFood();
    }

    public void GenerateFood()
    {
        int x = _random.Next(0, (int)MapSize.MaxRightBorder + 1);
        int y = _random.Next(0, (int)MapSize.MaxLowerBorder + 1);
        _food = new Food((x, y));
    }

    public void HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                _snake.Direction = (0, -1);
                break;
            case ConsoleKey.DownArrow:
                _snake.Direction = (0, 1);
                break;
            case ConsoleKey.LeftArrow:
                _snake.Direction = (-1, 0);
                break;
            case ConsoleKey.RightArrow:
                _snake.Direction = (1, 0);
                break;
        }
    }

    public void Draw()
    {
        Console.Clear();

        for (int i = 0; i <= (int)MapSize.MaxRightBorder + 1; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("#");
        }

        for (int i = 0; i <= (int)MapSize.MaxRightBorder + 1; i++)
        {
            Console.SetCursorPosition(i, (int)MapSize.MaxLowerBorder + 1);
            Console.Write("#");
        }

        for (int i = 1; i <= (int)MapSize.MaxLowerBorder; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("#");

            Console.SetCursorPosition((int)MapSize.MaxRightBorder + 1, i);
            Console.Write("#");
        }

        foreach (var segment in _snake.Body)
        {
            Console.SetCursorPosition(segment.x, segment.y);
            Console.Write("*");
        }

        Console.SetCursorPosition(_food.Position.x, _food.Position.y);
        Console.Write("$");
    }

    public bool IsGameOver()
    {
        return _snake.IsSelfCollision() || _snake.IsOutOfBounds();
    }

    public void Play()
    {
        Thread drawThread = new Thread(DrawSnake);
        drawThread.Start();

        while (!IsGameOver())
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                HandleInput(keyInfo.Key);
            }
            if (_snake.Eat(_food))
            {
                GenerateFood();
            }
            _snake.Move();
            Thread.Sleep(100);
        }

        drawThread.Join();
        Console.WriteLine("Game Over!");
        Console.ReadLine();
    }

    private void DrawSnake()
    {
        while (!IsGameOver())
        {
            Draw();
            Thread.Sleep(100);
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Game game = new Game();
        game.Play();
    }
}
