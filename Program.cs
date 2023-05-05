using System;
using System.IO;
using BetterMaze;

public class Program
{
    public static void Main(string[] args)
    {
        int w = 32;
        int h = 150;
        Random random = new Random();
        //int seed = 1264992159;
        int seed = random.Next();

        Maze m = new Maze((w * 2) + 1, (h * 2) + 1, seed);

        if (w % 2 == 0 && h % 2 == 0)
        {
            w++;
            h++;
        }
        else if (w % 2 == 0)
        {
            w++;
        }
        else if (h % 2 == 0)
        {
            h++;
        }
        int[] init = new int[2] { w, h };
        m.generateMaze(init[0], init[1]);
        //m.punchHoles();
        m.autoStart();
        //m.resetExit((3*2)-1, (3*2)-1);

        m.solveMazeDepth();
        m.printMazeColor();
        //m.printMaze();
        //m.printArray();

        //UserMadeMaze();

    }

    private static void UserMadeMaze()
    {
        bool repeat = false;
        do
        {
            repeat = false;

            bool repeatPrompt = true;
            int w = 0;
            do
            {
                Console.WriteLine("Enter a width: ");
                string input = Console.ReadLine();
                if (input != null && int.Parse(input) > 2)
                {
                    w = int.Parse(input);
                    repeatPrompt = false;
                }

            } while (repeatPrompt);
            Console.WriteLine();

            int h = 0;
            do
            {
                Console.WriteLine("Enter a height: ");
                string input = Console.ReadLine();
                if (input != null && int.Parse(input) > 2)
                {
                    h = int.Parse(input);
                    repeatPrompt = false;
                }

            } while (repeatPrompt);
            Console.WriteLine();

            repeatPrompt = true;
            int seed = 0;
            do
            {
                Console.WriteLine("Enter a seed? (y/n)");
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Y)
                {
                    Console.WriteLine("\nEnter a seed:");
                    string input = Console.ReadLine();
                    if (input != null)
                    {
                        if (!double.IsNaN(int.Parse(input)))
                        {
                            seed = int.Parse(input);
                            repeatPrompt = false;
                        }

                    }
                }
                else if (key == ConsoleKey.N)
                {
                    Random random = new Random();
                    seed = random.Next();
                    repeatPrompt = false;
                }
            } while (repeatPrompt);
            Console.WriteLine();
            Console.WriteLine();

            Maze m = new Maze((w * 2) + 1, (h * 2) + 1, seed);

            int[] init = new int[2] { 1, 1 };
            int[] start = new int[2] { w, h };
            int[] exit = new int[2] { (w * 2) - 1, (h * 2) - 1 };
            if (w % 2 == 0 && h % 2 == 0)
            {
                w++;
                h++;
                init = new int[2] { 1, 1 };
                start = new int[2] { w, h };
                exit = new int[2] { (w * 2) - 3, (h * 2) - 3 };
            }
            else if (w % 2 == 0)
            {
                w++;
                init = new int[2] { 1, 1 };
                start = new int[2] { w, h };
                exit = new int[2] { (w * 2) - 3, (h * 2) - 1 };
            }
            else if (h % 2 == 0)
            {
                h++;
                init = new int[2] { 1, 1 };
                start = new int[2] { w, h };
                exit = new int[2] { (w * 2) - 1, (h * 2) - 3 };
            }


            m.generateMaze(init[0], init[1], exit[0], exit[1]);
            m.resetStart(start[0], start[1]);
            m.solveMazeDepth();
            m.printMazeColor();

            repeatPrompt = true;
            do
            {
                Console.WriteLine("Save this maze? (y/n)");
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Y)
                {
                    repeatPrompt = false;
                    using (StreamWriter sw = new StreamWriter("C:\\Users\\kevdc\\source\\repos\\demo\\BetterMaze\\MyMazes\\Mazes.txt", true))
                    {
                        sw.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8}", w, h, seed, init[0], init[1], start[0], start[1], exit[0], exit[1]);
                    }
                }
                else if (key == ConsoleKey.N)
                {
                    repeatPrompt = false;
                }
            } while (repeatPrompt);
            Console.WriteLine();

            repeatPrompt = true;
            do
            {
                Console.WriteLine("Create a new maze? (y/n)");
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Y)
                {
                    repeatPrompt = false;
                    repeat = true;
                }
                else if (key == ConsoleKey.N)
                {
                    repeatPrompt = false;
                }
            } while (repeatPrompt);
            Console.WriteLine();

        } while (repeat);
    }
}