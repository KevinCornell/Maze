using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BetterMaze
{
    public class Maze
    {
        /* Maze codes:
         * 0: Path
         * 1: Wall
         * 2: Start
         * 3: Exit
         * -1: ^
         * -2: v
         * -3: >
         * -4: <
         */
        int[,] maze;
        int width;
        int height;
        bool generated;
        int[] start;
        int[] currPos;
        int[] end;
        Random rand;
        int seed;

        public Maze(int w, int h, int seed)
        {
            if (w%2 == 0 || h%2 == 0)
            {
                throw new Exception("Invalid Maze: Maze width and height must both be odd. Sorry.");
            }
            if (w < 3 || h < 3)
            {
                throw new Exception("Invalid Maze: Maze must be at least 3x3. Sorry.");
            }
            width = w;
            height = h;
            maze = new int[w, h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    maze[x, y] = 1;
                }
            }
            generated = false;
            start = new int [2] {1, 1};
            currPos = new int[2] { 1, 1 };
            end = new int[2] { 1, 1 };
            rand = new Random(seed);
            this.seed = seed;
        }

        public void generateMaze(int xS, int yS, int xE, int yE)
        {
            if ((xS < 0 || xS >= width) || (yS < 0 || yS >= height))
            {
                throw new Exception("Invalid Start position: xS and yS must both be within bounds.");
            }
            if (xS%2 == 0 || yS%2 == 0)
            {
                throw new Exception("Invalid Start position: xS and yS must both be odd. Sorry.");
            }
            if ((xE < 0 || xE >= width) || (yE < 0 || yE >= height))
            {
                throw new Exception("Invalid Exit position: xE and yE must both be within bounds.");
            }
            if (xE % 2 == 0 || yE % 2 == 0)
            {
                throw new Exception("Invalid Exit position: xE and yE must both be odd. Sorry.");
            }

            generateHelper(xS, yS);
            maze[xS, yS] = 2;
            maze[xE, yE] = 3;
            start[0] = xS;
            start[1] = yS;
            end[0] = xE;
            end[1] = yE;
            currPos[0] = xS;
            currPos[1] = yS;
            generated = true;
        }

        private void generateHelper(int x, int y)
        {
            maze[x, y] = 0;
            int cnt = 0;
            do
            {
                int pos = rand.Next(4);
                if (pos == 0)
                {
                    if (canGenerate(x+2, y))
                    {
                        maze[x + 1, y] = 0;
                        generateHelper(x + 2, y);
                    }
                }
                else if (pos == 1)
                {
                    if (canGenerate(x - 2, y))
                    {
                        maze[x - 1, y] = 0;
                        generateHelper(x - 2, y);
                    }
                }
                else if (pos == 2)
                {
                    if (canGenerate(x, y + 2))
                    {
                        maze[x, y + 1] = 0;
                        generateHelper(x, y + 2);
                    }
                } 
                else
                {
                    if (canGenerate(x, y - 2))
                    {
                        maze[x, y - 1] = 0;
                        generateHelper(x, y - 2);
                    }
                }
                cnt++;
            } while (cnt < 20);
        }

        public void generateMaze(int xS, int yS)
        {
            if ((xS < 0 || xS >= width) || (yS < 0 || yS >= height))
            {
                throw new Exception("Invalid Start position: xS and yS must both be within bounds.");
            }
            if (xS % 2 == 0 || yS % 2 == 0)
            {
                throw new Exception("Invalid Start position: xS and yS must both be odd. Sorry.");
            }
            generateHelper(xS, yS);
            maze[xS, yS] = 2;
            start[0] = xS;
            start[1] = yS;
            currPos[0] = xS;
            currPos[1] = yS;

            //Find a dead end from the bottom right outward
            for (int y = height-1; y >= 0; y--)
            {
                for (int x = width-1; x >= 0; x--)
                {
                    if (x % 2 == 1 && y % 2 == 1)
                    {
                        int cnt = 0;
                        if (!canMoveTo(x + 1, y))
                        {
                            cnt++;
                        }
                        if (!canMoveTo(x - 1, y))
                        {
                            cnt++;
                        }
                        if (!canMoveTo(x, y + 1))
                        {
                            cnt++;
                        }
                        if (!canMoveTo(x, y - 1))
                        {
                            cnt++;
                        }
                        if (cnt >= 3)
                        {
                            if (x != xS || y != yS)
                            {
                                //This is the exit
                                maze[x, y] = 3;
                                end[0] = x;
                                end[1] = y;
                                generated = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private bool canGenerate(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < width && y < height && maze[x,y] == 1);
        }

        public void clearMaze()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    maze[x, y] = 1;
                }
            }
            generated = false;
        }

        public void resetStart(int x, int y)
        {
            maze[start[0], start[1]] = 0;
            start[0] = x;
            start[1] = y;
            currPos[0] = x;
            currPos[1] = y;
            maze[start[0], start[1]] = 2;
        }

        public void resetExit(int x, int y) {
            maze[end[0], end[1]] = 0;
            end[0] = x;
            end[1] = y;
            maze[end[0], end[1]] = 3;
        }

        public void printMazeColor()
        {
            Console.WriteLine("Maze: {0} x {1}, seed: {2}", (width-1)/2, (height-1)/2, seed);
            for (int y = -1; y <= height; y++)
            {
                for (int x = -1; x <= width; x++)
                {
                    //Default Color
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    if (y == -1 || y == height)
                    {
                        if (x != -1 && x != width && x%2 == 1)
                        {
                            if (((x+1)/2) < 10)
                            {
                                Console.Write(" {0} ", (x+1)/2);
                            } else
                            {
                                Console.Write(" {0}", (x+1)/2);
                            }
                            
                        }
                        else
                        {
                            Console.Write("   ");
                        }
                    }
                    else if (x == -1 || x == width)
                    {
                        if (y != -1 && y != height && y % 2 == 1)
                        {
                            if (((y+1)/2) < 10)
                            {
                                Console.Write("  {0} ", (y+1)/2);
                            }
                            else if (((y + 1) / 2) < 100)
                            {
                                Console.Write(" {0} ", (y+1)/2);
                            } else
                            {
                                Console.Write("{0} ", (y + 1) / 2);
                            }

                        }
                        else
                        {
                            Console.Write("    ");
                        }
                    }
                    else if (maze[x,y] == 1)
                    {
                        //Wall Color
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Black;

                        if (x%2 == 0 && y%2 == 0)
                        {
                            Console.Write(" + ");
                        } else if (x%2 == 0)
                        {
                            Console.Write(" | ");
                        } else
                        {
                            Console.Write("---");
                        }
                    }
                    else if (maze[x,y] == 2 || maze[x,y] == 3)
                    {
                        //Start and Exit Colors
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Cyan;

                        if (maze[x,y] == 2)
                        {
                            Console.Write(" S ");
                        } else
                        {
                            Console.Write(" E ");
                        }
                    }
                    else if (maze[x,y] < 0 && maze[x,y] >= -4)
                    {
                        //Correct Path Colors
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.BackgroundColor = ConsoleColor.Green;

                        if (maze[x,y] == -1)
                        {
                            //Up
                            Console.Write(" ^ ");
                        } 
                        else if (maze[x, y] == -2)
                        {
                            //Down
                            Console.Write(" v ");
                        }
                        else if (maze[x, y] == -3)
                        {
                            //Right
                            Console.Write(" > ");
                        } 
                        else
                        {
                            //Left
                            Console.Write(" < ");
                        }
                    }
                    else if (currPos[0] == x && currPos[1] == y)
                    {
                        //My Position Color

                        Console.Write(" O ");
                    }
                    else if (maze[x,y] == -10)
                    {
                        //Incorrect Path and My previous path Colors
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(" * ");
                    }
                    else
                    {
                        //Empty Path Colors
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.White;

                        Console.Write("   ");
                    }
                }

                
                Console.WriteLine();
            }
        }

        public void printMazeColor(int xP, int yP)
        {
            Console.WriteLine("Maze: {0} x {1}, seed: {2}", (width - 1) / 2, (height - 1) / 2, seed);
            for (int y = -1; y <= height; y++)
            {
                for (int x = -1; x <= width; x++)
                {
                    //Default Color
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    if (x == xP && y == yP) {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.Write("   ");
                    }
                    else if (y == -1 || y == height)
                    {
                        if (x != -1 && x != width && x % 2 == 1)
                        {
                            if (((x + 1) / 2) < 10)
                            {
                                Console.Write(" {0} ", (x + 1) / 2);
                            }
                            else
                            {
                                Console.Write(" {0}", (x + 1) / 2);
                            }

                        }
                        else
                        {
                            Console.Write("   ");
                        }
                    }
                    else if (x == -1 || x == width)
                    {
                        if (y != -1 && y != height && y % 2 == 1)
                        {
                            if (((y + 1) / 2) < 10)
                            {
                                Console.Write("  {0} ", (y + 1) / 2);
                            }
                            else if (((y + 1) / 2) < 100)
                            {
                                Console.Write(" {0} ", (y + 1) / 2);
                            }
                            else
                            {
                                Console.Write("{0} ", (y + 1) / 2);
                            }

                        }
                        else
                        {
                            Console.Write("    ");
                        }
                    }
                    else if (maze[x, y] == 1)
                    {
                        //Wall Color
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Black;

                        if (x % 2 == 0 && y % 2 == 0)
                        {
                            Console.Write(" + ");
                        }
                        else if (x % 2 == 0)
                        {
                            Console.Write(" | ");
                        }
                        else
                        {
                            Console.Write("---");
                        }
                    }
                    else if (maze[x, y] == 2 || maze[x, y] == 3)
                    {
                        //Start and Exit Colors
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Cyan;

                        if (maze[x, y] == 2)
                        {
                            Console.Write(" S ");
                        }
                        else
                        {
                            Console.Write(" E ");
                        }
                    }
                    else if (maze[x, y] < 0 && maze[x, y] >= -4)
                    {
                        //Correct Path Colors
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.BackgroundColor = ConsoleColor.Green;

                        if (maze[x, y] == -1)
                        {
                            //Up
                            Console.Write(" ^ ");
                        }
                        else if (maze[x, y] == -2)
                        {
                            //Down
                            Console.Write(" v ");
                        }
                        else if (maze[x, y] == -3)
                        {
                            //Right
                            Console.Write(" > ");
                        }
                        else
                        {
                            //Left
                            Console.Write(" < ");
                        }
                    }
                    else if (currPos[0] == x && currPos[1] == y)
                    {
                        //My Position Color
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.Write(" S ");
                    }
                    else if (maze[x, y] == -10)
                    {
                        //Incorrect Path and My previous path Colors
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(" * ");
                    }
                    else if (maze[x, y] == -20)
                    {
                        //Incorrect Path and My previous path Colors
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        Console.Write(" * ");
                    }
                    else
                    {
                        //Empty Path Colors
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.White;

                        Console.Write("   ");
                    }
                }


                Console.WriteLine();
            }
        }

        public void printMaze()
        {
            Console.WriteLine("Maze: {0} x {1}, seed: {2}", (width - 1) / 2, (height - 1) / 2, seed);
            for (int y = -1; y <= height; y++)
            {
                for (int x = -1; x <= width; x++)
                {
                    //Default
                    if (y == -1 || y == height)
                    {
                        if (x != -1 && x != width && x % 2 == 1)
                        {
                            if (((x + 1) / 2) < 10)
                            {
                                Console.Write(" {0} ", (x + 1) / 2);
                            }
                            else
                            {
                                Console.Write(" {0}", (x + 1) / 2);
                            }

                        }
                        else
                        {
                            Console.Write("   ");
                        }
                    }
                    else if (x == -1 || x == width)
                    {
                        if (y != -1 && y != height && y % 2 == 1)
                        {
                            if (((y + 1) / 2) < 10)
                            {
                                Console.Write("  {0} ", (y + 1) / 2);
                            }
                            else if (((y + 1) / 2) < 100)
                            {
                                Console.Write(" {0} ", (y + 1) / 2);
                            }
                            else
                            {
                                Console.Write("{0} ", (y + 1) / 2);
                            }

                        }
                        else
                        {
                            Console.Write("    ");
                        }
                    }
                    else if (maze[x, y] == 1)
                    {
                        //Wall

                        if (x % 2 == 0 && y % 2 == 0)
                        {
                            Console.Write(" + ");
                        }
                        else if (x % 2 == 0)
                        {
                            Console.Write(" | ");
                        }
                        else
                        {
                            Console.Write("---");
                        }
                    }
                    else if (maze[x, y] == 2 || maze[x, y] == 3)
                    {
                        //Start and Exit

                        if (maze[x, y] == 2)
                        {
                            Console.Write(" S ");
                        }
                        else
                        {
                            Console.Write(" E ");
                        }
                    }
                    else if (maze[x, y] < 0 && maze[x, y] >= -4)
                    {
                        //Correct Path

                        if (maze[x, y] == -1)
                        {
                            //Up
                            Console.Write(" ^ ");
                        }
                        else if (maze[x, y] == -2)
                        {
                            //Down
                            Console.Write(" v ");
                        }
                        else if (maze[x, y] == -3)
                        {
                            //Right
                            Console.Write(" > ");
                        }
                        else
                        {
                            //Left
                            Console.Write(" < ");
                        }
                        //Console.Write("   ");
                    }
                    else if (currPos[0] == x && currPos[1] == y)
                    {
                        //My Position Color

                        Console.Write(" O ");
                    }
                    else if (maze[x, y] == -10)
                    {
                        //Incorrect Path and My previous path 
                        Console.Write("   ");
                    }
                    else
                    {
                        //Empty Path
                        Console.Write("   ");
                    }
                }


                Console.WriteLine();
            }
        }

        public void solveMazeDepth()
        {
            if (!generated)
            {
                throw new Exception("Maze has not been generated.");
            }
            recursiveSolveDepth(start);
            maze[start[0], start[1]] = 2;
        }

        private bool recursiveSolveDepth(int[] pos)
        {
            //Check current position is at the end
            if (pos[0] == end[0] && pos[1] == end[1])
            {
                //Found the end
                return true;
            }

            //Mark current spot
            maze[pos[0], pos[1]] = -20;
            bool found = false;

            //printMazeColor(pos[0], pos[1]);
            //Thread.Sleep(500);
            //Console.Clear();

            //Go Down if possible
            if (canMoveTo(pos[0], pos[1] + 1) && !found)
            {
                found = recursiveSolveDepth(new int[2] { pos[0], pos[1] + 1 });
                if (found)
                {
                    //Mark this as the correct path;
                    maze[pos[0], pos[1]] = -2;
                }
            }


            //Go Right if possible
            if (canMoveTo(pos[0]+1, pos[1]) && !found)
            {
                found = recursiveSolveDepth(new int[2] { pos[0] + 1, pos[1] });
                if (found)
                {
                    //Mark this as the correct path;
                    maze[pos[0], pos[1]] = -3;
                }
            }


            //Go Left if possible
            if (canMoveTo(pos[0] - 1, pos[1]) && !found)
            {
                found = recursiveSolveDepth(new int[2] { pos[0] - 1, pos[1] });
                if (found)
                {
                    //Mark this as the correct path;
                    maze[pos[0], pos[1]] = -4;
                }
            }

            //Go Up if possible
            if (canMoveTo(pos[0], pos[1] - 1) && !found)
            {
                found = recursiveSolveDepth(new int[2] { pos[0], pos[1] - 1});
                if (found)
                {
                    //Mark this as the correct path;
                    maze[pos[0], pos[1]] = -1;
                }
            }

            if (!found)
            {
                maze[pos[0], pos[1]] = -10;
            }

            //printMazeColor(pos[0], pos[1]);
            //Thread.Sleep(500);
            //Console.Clear();

            return found;
        }

        private bool canMoveTo(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 && y < height && (maze[x, y] == 0 || maze[x, y] == 3 || maze[x,y] == 10));
        }

        public bool moveDir(int d)
        {
            if (d == 0 && canMoveTo(currPos[0], currPos[1]-1))
            {
                currPos[0] = currPos[0];
                currPos[1] = currPos[1] - 1;
                maze[currPos[0], currPos[1]] = 10;
            } 
            else if (d == 1 && canMoveTo(currPos[0], currPos[1] + 1))
            {
                currPos[0] = currPos[0];
                currPos[1] = currPos[1] + 1;
                maze[currPos[0], currPos[1]] = 10;
            }
            else if (d == 2 && canMoveTo(currPos[0]-1, currPos[1]))
            {
                currPos[0] = currPos[0]-1;
                currPos[1] = currPos[1];
                maze[currPos[0], currPos[1]] = 10;
            }
            else if (d == 3 && canMoveTo(currPos[0]+1, currPos[1]))
            {
                currPos[0] = currPos[0]+1;
                currPos[1] = currPos[1];
                maze[currPos[0], currPos[1]] = 10;
            }

            if (currPos[0] == end[0] && currPos[1] == end[1])
            {
                maze[currPos[0], currPos[1]] = 2;
                return true;
            } else
            {
                return false;
            }
        }

        public void autoStart()
        {
            //Find a dead end from the top left inward
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x % 2 == 1 && y % 2 == 1 && x != end[0] && y != end[1])
                    {
                        int cnt = 0;
                        if (!canMoveTo(x + 1, y))
                        {
                            cnt++;
                        }
                        if (!canMoveTo(x - 1, y))
                        {
                            cnt++;
                        }
                        if (!canMoveTo(x, y + 1))
                        {
                            cnt++;
                        }
                        if (!canMoveTo(x, y - 1))
                        {
                            cnt++;
                        }

                        if (cnt >= 3)
                        {
                            if (x != end[0] || y != end[1])
                            {
                                //Clear old start
                                maze[start[0], start[1]] = 0;
                                //This is the start
                                maze[x, y] = 2;
                                start[0] = x;
                                start[1] = y;
                                currPos[0] = start[0];
                                currPos[1] = start[1];
                                generated = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void printArray()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(maze[x, y]);
                }
                Console.WriteLine();
            }
        }

        public void punchHoles()
        {
            int chance = 70;
            for (int y = 1; y < height-1; y++)
            {
                for (int x = 1; x < width-1; x++)
                {
                    if (maze[x, y] == 1)
                    {
                        int temp = rand.Next(chance);
                        if (temp == 1 && ((maze[x+1, y] == 0 && maze[x-1, y] == 0) || (maze[x, y+1] == 0 && maze[x, y-1] == 0)))
                        {
                            maze[x, y] = 0;
                        }
                    }
                }
            }
        }
    }
}
