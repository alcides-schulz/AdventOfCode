using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day14RestroomRedoubt : SolverBase
    {
        private class Robot
        {
            public int PositionX;
            public int PositionY;
            public int VelocityX;
            public int VelocityY;
        }

        private List<string> _inputLines = new List<string>();
        private readonly int _rows = 0;
        private readonly int _cols = 0;

        public Day14RestroomRedoubt(string dataFileName, int rows, int cols)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _inputLines.Add(line);
            }
            inputData.Close();
            _rows = rows;
            _cols = cols;
        }

        public override string GetPartOneAnswer()
        {
            var robots = GetRobots();
            for (int i = 0; i < 100; i++)
                WalkRobots(robots);
            return SafetyFactor(robots).ToString();
        }

        public override string GetPartTwoAnswer()
        {
            var maxValue = 0;
            var seconds = 0;
            var robots = GetRobots();
            //int[,] savedMap = null;
            for (int i = 0; i < 10000; i++)
            {
                var map = CreateMap(robots);
                var value = GetValue(map);
                if (value > maxValue)
                {
                    maxValue = value;
                    //savedMap = map;
                    seconds = i;
                }
                WalkRobots(robots);
            }
            //PrintTree(savedMap);
            //Console.WriteLine("Value: " + maxValue);
            return seconds.ToString();
        }

        private List<Robot> GetRobots()
        {
            var robots = new List<Robot>();
            foreach(var line in _inputLines)
            {
                var positionPart = line.Split(' ')[0].Replace("p=", "");
                var velocityPart = line.Split(' ')[1].Replace("v=", "");
                var robot = new Robot
                {
                    PositionX = int.Parse(positionPart.Split(',')[0]),
                    PositionY = int.Parse(positionPart.Split(',')[1]),
                    VelocityX = int.Parse(velocityPart.Split(',')[0]),
                    VelocityY = int.Parse(velocityPart.Split(',')[1])
                };
                robots.Add(robot);
            }
            return robots;
        }

        private int GetValue(int[,] tree)
        {
            var value = 0;
            for (int y = 0; y < tree.GetLength(0); y++)
            {
                for (int x = 0; x < tree.GetLength(1); x++)
                {
                    var n = 0;
                    if (x > 0 && tree[y, x - 1] == 1) n++;
                    if (x < tree.GetLength(1) - 1 && tree[y, x + 1] == 1) n++;
                    if (y > 0 && tree[y - 1, x] == 1) n++;
                    if (y < tree.GetLength(0) - 1 && tree[y + 1, x] == 1) n++;
                    if (n >= 3) value += n;
                }
            }
            return value;
        }

        private int SafetyFactor(List<Robot> robots)
        {
            var excludeX = (_cols - 1) / 2;
            var excludeY = (_rows - 1) / 2;
            var quadrant1 = 0;
            var quadrant2 = 0;
            var quadrant3 = 0;
            var quadrant4 = 0;
            foreach (var robot in robots)
            {
                if (robot.PositionX == excludeX || robot.PositionY == excludeY)
                    continue;
                if (robot.PositionX < excludeX && robot.PositionY < excludeY)
                    quadrant1++;
                if (robot.PositionX > excludeX && robot.PositionY < excludeY)
                    quadrant2++;
                if (robot.PositionX < excludeX && robot.PositionY > excludeY)
                    quadrant3++;
                if (robot.PositionX > excludeX && robot.PositionY > excludeY)
                    quadrant4++;
            }
            return quadrant1 * quadrant2 * quadrant3 * quadrant4;
        }

        private void WalkRobots(List<Robot> robots)
        {
            foreach (var robot in robots)
            {
                robot.PositionX += robot.VelocityX;
                if (robot.PositionX < 0)
                    robot.PositionX += _cols;
                if (robot.PositionX >= _cols)
                    robot.PositionX -= _cols;
                robot.PositionY += robot.VelocityY;
                if (robot.PositionY < 0)
                    robot.PositionY += _rows;
                if (robot.PositionY >= _rows)
                    robot.PositionY -= _rows;
            }
        }

        private int[,] CreateMap(List<Robot> robots)
        {
            var map = new int[_rows, _cols];
            foreach (var robot in robots)
            {
                map[robot.PositionY, robot.PositionX] = 1;
            }
            return map;
        }

        private void PrintTree(int[,] tree)
        {
            for (int y = 0; y < tree.GetLength(0); y++)
            {
                for (int x = 0; x < tree.GetLength(1); x++)
                {
                    if (tree[y, x] != 0)
                        Console.Write('*');
                    else
                    {
                        if (x == 0 || x == tree.GetLength(1) - 1)
                            Console.Write('|');
                        else if (y == 0 || y == tree.GetLength(0) - 1)
                            Console.Write('-');
                        else 
                            Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
