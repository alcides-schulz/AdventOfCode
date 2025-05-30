using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Advent2024
{
    public class Day16ReindeerMaze : SolverBase
    {
        private readonly List<string> _map = new List<string>();
        private readonly Dictionary<char, PathNode> _direction = new Dictionary<char, PathNode>();

        public Day16ReindeerMaze(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                if (line == "")
                    break;
                _map.Add(line);
            }
            inputData.Close();
        }

        public override long GetPartOneAnswer()
        {
            var path = SearchPath(FindStart(), FindGoal());
            //foreach (var point in path)
            //    Console.WriteLine("(" + point.Row + "," + point.Col + ")");
            //PrintMap(path);
            return CalculateScore(path[0]);
        }

        public override long GetPartTwoAnswer()
        {
            return 0;
        }

        private long CalculateScore(List<Position> path)
        {
            var score = 0L;
            var direction = 'E';
            for (int i = 1; i < path.Count; i++)
            {
                score += 1;
                var current = path[i];
                var previous = path[i - 1];
                if (current.Row == previous.Row + 1 && current.Col == previous.Col && direction != 'N')
                {
                    direction = 'N';
                    score += 1000;
                }
                if (current.Row == previous.Row - 1 && current.Col == previous.Col && direction != 'S')
                {
                    direction = 'S';
                    score += 1000;
                }
                if (current.Row == previous.Row && current.Col == previous.Col + 1 && direction != 'E')
                {
                    direction = 'E';
                    score += 1000;
                }
                if (current.Row == previous.Row && current.Col == previous.Col - 1 && direction != 'W')
                {
                    direction = 'W';
                    score += 1000;
                }
            }

            return score;
        }

        private void PrintMap(List<Position> path)
        {
            Console.WriteLine();
            var temp = new List<char[]>();
            foreach (var line in _map)
            {
                temp.Add(line.ToCharArray());
            }
            foreach (var position in path)
            {
                if (".SE".IndexOf(temp[position.Row][position.Col]) == -1)
                {
                    Console.WriteLine("ERROR: " + position.Row + "," + position.Col);
                    break;
                }
                if (temp[position.Row][position.Col] == '.')
                    temp[position.Row][position.Col] = '*';
            }
            for (int row = 0; row < temp.Count; row++)
            {
                Console.Write(string.Format("{0,2} ", row));
                for (int col = 0; col < temp[row].Length; col++)
                {
                    Console.Write(temp[row][col]);
                }
                Console.WriteLine();
            }
        }

        private Position FindStart()
        {
            for (int row = 0; row < _map.Count; row++)
            {
                for (int col = 0; col < _map[row].Length; col++)
                {
                    if (_map[row][col] == 'S')
                        return new Position(row, col);
                }
            }
            return null;
        }

        private Position FindGoal()
        {
            for (int row = 0; row < _map.Count; row++)
            {
                for (int col = 0; col < _map[row].Length; col++)
                {
                    if (_map[row][col] == 'E')
                        return new Position(row, col);
                }
            }
            return null;
        }

        private List<List<Position>> SearchPath(Position start, Position goal)
        {
            var openSet = new List<PathNode>();
            var closedSet = new List<PathNode>();

            var startNode = new PathNode();
            startNode.Row = start.Row;
            startNode.Col = start.Col;
            startNode.Direction = 'E';
            startNode.Cost = 0;
            startNode.Heuristic = HeuristicCost(start, goal);
            startNode.TotalCost = startNode.Cost + startNode.Heuristic;

            openSet.Add(startNode);

            while (openSet.Count != 0)
            {
                PathNode current = FindLowestCost(openSet);
                if (current.Row == goal.Row && current.Col == goal.Col)
                    return ReconstructPaths(current);
                openSet.Remove(current);
                closedSet.Add(current);
                // N, S, E, W
                int[] dirRow = { -1, 1, 0, 0 };
                int[] dirCol = { 0, 0, 1, -1 };
                for (int i = 0; i < 4; i++)
                {
                    int neighborRow = current.Row + dirRow[i];
                    int neighborCol = current.Col + dirCol[i];
                    if (!IsValid(neighborRow, neighborCol) || _map[neighborRow][neighborCol] == '#' || _map[neighborRow][neighborCol] == 'S')
                        continue;
                    var neighbor = new PathNode();
                    neighbor.Row = neighborRow;
                    neighbor.Col = neighborCol;
                    neighbor.Cost = current.Cost + 1;
                    neighbor.Direction = current.Direction;
                    if (i == 0 && current.Direction != 'N')
                    {
                        neighbor.Cost += 1000;
                        neighbor.Direction = 'N';
                    }
                    if (i == 1 && current.Direction != 'S')
                    {
                        neighbor.Cost += 1000;
                        neighbor.Direction = 'S';
                    }
                    if (i == 2 && current.Direction != 'E')
                    {
                        neighbor.Cost += 1000;
                        neighbor.Direction = 'E';
                    }
                    if (i == 3 && current.Direction != 'W')
                    {
                        neighbor.Cost += 1000;
                        neighbor.Direction = 'W';
                    }
                    
                    neighbor.Heuristic = HeuristicCost(new Position(neighborRow, neighborCol), goal);
                    neighbor.TotalCost = neighbor.Cost + neighbor.Heuristic;

                    var existingOpenNode = FindOpenNode(neighbor, openSet);
                    if (existingOpenNode == null)
                    {
                        neighbor.Parents.Add(current);
                        openSet.Add(neighbor);
                    }
                    else if (neighbor.Cost == existingOpenNode.Cost)
                    {
                        existingOpenNode.Parents.Add(current);
                    }
                    else if (neighbor.Cost < existingOpenNode.Cost)
                    {
                        existingOpenNode.Cost = neighbor.Cost;
                        existingOpenNode.Parents.Clear();
                        existingOpenNode.Parents.Add(current);
                    }


                    //neighbor.Parent.Add(current);
                    //bool inClosedSet = false;
                    //foreach (var closedNode in closedSet)
                    //{
                    //    if (closedNode.Row == neighborRow && closedNode.Col == neighborCol)
                    //    {
                    //        if (neighbor.Cost >= closedNode.Cost)
                    //        {
                    //            inClosedSet = true;
                    //            break;
                    //        }
                    //    }
                    //}
                    //if (inClosedSet)
                    //    continue;
                    //bool inOpenSet = false;
                    //foreach (var openNode in openSet)
                    //{
                    //    if (openNode.Row == neighborRow && openNode.Col == neighborCol)
                    //    {
                    //        if (neighbor.Cost >= openNode.Cost)
                    //        {
                    //            inOpenSet = true;
                    //            break;
                    //        }
                    //    }
                    //}
                    //if (inOpenSet)
                    //    continue;

                    //openSet.Add(neighbor);
                }
            }

            return new List<List<Position>>(); // no path
        }

        private PathNode FindOpenNode(PathNode searchNode, List<PathNode> openNodes)
        {
            foreach (var node in openNodes)
            {
                if (node.Row == searchNode.Row && node.Col == searchNode.Col)
                    return node;
            }
            return null;
        }

        private class PathNode
        {
            public int Row;
            public int Col;
            public char Direction = 'E';
            public int Cost;        // Cost from start to current node
            public int Heuristic;   // Heuristic (estimated cost to goal)
            public int TotalCost;   // Total cost (g + h)
            public List<PathNode> Parents = new List<PathNode>(); // Pointer to the parent node
        };

        private class Position
        {
            public int Row = 0;
            public int Col = 0;
            public Position(int row, int col)
            {
                Row = row;
                Col = col;
            }
        };

        private int HeuristicCost(Position current, Position goal)
        {
            return Math.Abs(current.Row - goal.Row) + Math.Abs(current.Col - goal.Col);
        }

        private bool IsValid(int row, int col)
        {
            return row >= 0 && row < _map.Count && col >= 0 && col < _map[row].Length;
        }

        private PathNode FindLowestCost(List<PathNode> openSet)
        {
            if (openSet.Count == 0)
                return null;
            var lowest = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].TotalCost < lowest.TotalCost)
                    lowest = openSet[i];
            }
            return lowest;
        }

        List<List<Position>> ReconstructPaths(PathNode current)
        {
            var paths = new List<List<Position>>();
            if (current.Parents.Count == 0)
            {
                paths.Add(new List<Position> { new Position(current.Row, current.Col) });
                return paths;
            }

            foreach (var parent in current.Parents)
            {
                var parentPaths = ReconstructPaths(parent);
                foreach(var parentPath in parentPaths)
                {
                    var newPath = new List<Position>(parentPath);
                    newPath.Add(new Position(current.Row, current.Col));
                    paths.Add(newPath);
                }
            }
            return paths;

            //var path = new List<Position>();
            //while (current != null)
            //{
            //    Position p = new Position(current.Row, current.Col);
            //    path.Insert(0, p);
            //    current = current.Parent;
            //}
            //return path;
        }
    }
}
