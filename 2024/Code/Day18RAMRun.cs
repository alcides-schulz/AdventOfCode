using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day18RAMRun : SolverBase
    {
        private List<Node> _bytes = new List<Node>();
        private int _rows;
        private int _cols;
        private int _fallCount;
        private List<char[]> _map = new List<char[]>();

        public Day18RAMRun(string dataFileName, int rows, int cols, int fallCount)
        {
            _rows = rows;
            _cols = cols;
            _fallCount = fallCount;
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _bytes.Add(new Node(int.Parse(line.Split(',')[1]), int.Parse(line.Split(',')[0])));
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            var grid = CreateGrid(_rows, _cols);
            DropBytes(grid, _bytes, _fallCount);
            var path = FindPath(grid, new Node(0, 0), new Node(_rows - 1, _cols - 1));
            return Convert.ToString(path.Count - 1);
        }

        public override string GetPartTwoAnswer()
        {
            var grid = CreateGrid(_rows, _cols);
            DropBytes(grid, _bytes, _fallCount);
            var start = new Node(0, 0);
            var end = new Node(_rows - 1, _cols - 1);
            for (int i = _fallCount; i < _bytes.Count; i++)
            {
                grid[_bytes[i].Row][_bytes[i].Col] = '#';
                if (FindPath(grid, start, end).Count == 0)
                    return _bytes[i].Col + "," + _bytes[i].Row; // return as (x,y)
            }
            return "";
        }

        private void DropBytes(List<char[]> grid, List<Node> bytes, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                grid[bytes[i].Row][bytes[i].Col] = '#';
            }
        }

        private List<char[]> CreateGrid(int rows, int cols)
        {
            var grid = new List<char[]>();
            for (int i = 0; i < rows; i++)
                grid.Add(new string('.', cols).ToCharArray());
            return grid;
        }

        public List<Node> FindPath(List<char[]> grid, Node start, Node goal)
        {
            var closedSet = new HashSet<Node>();
            var openSet = new HashSet<Node>();
            var cameFrom = new Dictionary<Node, Node>();
            var gScore = new Dictionary<Node, int>();
            var fScore = new Dictionary<Node, int>();

            openSet.Add(start);
            gScore[start] = 0;
            fScore[start] = CalculateHeuristic(start, goal);
            while (openSet.Count > 0)
            {
                var current = GetLowestFScoreNode(openSet, fScore);
                if (current.Equals(goal))
                    return ReconstructPath(cameFrom, current);
                openSet.Remove(current);
                closedSet.Add(current);
                foreach (var neighbor in GetNeighbors(grid, current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;
                    int tentativeGScore = gScore[current] + 1;
                    if (!openSet.Contains(neighbor) || !gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + CalculateHeuristic(neighbor, goal);
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
            return new List<Node>();
        }

        private int CalculateHeuristic(Node a, Node b)
        {
            return Math.Abs(a.Row - b.Row) + Math.Abs(a.Col - b.Col);
        }

        private Node GetLowestFScoreNode(HashSet<Node> openSet, Dictionary<Node, int> fScore)
        {
            Node lowest = null;
            int lowestFScore = int.MaxValue;

            foreach (Node node in openSet)
            {
                if (fScore.ContainsKey(node))
                {
                    if (fScore[node] < lowestFScore)
                    {
                        lowest = node;
                        lowestFScore = fScore[node];
                    }
                }
                else
                {
                    if (lowest == null)
                    {
                        lowest = node;
                    }
                }
            }
            return lowest;
        }

        private List<Node> GetNeighbors(List<char[]> grid, Node node)
        {
            var neighbors = new List<Node>();
            int rows = grid.Count;
            int cols = grid[0].Length;
            int[] dr = { 0, 0, 1, -1 };
            int[] dc = { 1, -1, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                int new_row = node.Row + dr[i];
                int new_col = node.Col + dc[i];
                if (new_row >= 0 && new_row < rows && new_col >= 0 && new_col < cols)
                    if (grid[new_row][new_col] == '.')
                        neighbors.Add(new Node(new_row, new_col));
            }
            return neighbors;
        }

        private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            var path = new List<Node>();
            path.Add(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }
            return path;
        }
    }
}

