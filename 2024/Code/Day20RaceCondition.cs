using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Advent2024
{
    public class Day20RaceCondition : SolverBase
    {
        private List<char[]> _map = new List<char[]>();
        private Node _startNode = null;
        private Node _endNode = null;
        private int _minSave1 = 0;
        private int _minSave2 = 0;

        public Day20RaceCondition(string dataFileName, int minSave1, int minSave2)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _map.Add(line.ToCharArray());
                if (line.IndexOf("S") != -1)
                    _startNode = new Node(_map.Count - 1, line.IndexOf("S"));
                if (line.IndexOf("E") != -1)
                    _endNode = new Node(_map.Count - 1, line.IndexOf("E"));
            }
            inputData.Close();
            _minSave1 = minSave1;
            _minSave2 = minSave2;
        }

        public override string GetPartOneAnswer()
        {
            return GetAnswer(2, _minSave1, false);
        }

        public override string GetPartTwoAnswer()
        {
            return GetAnswer(20, _minSave2, false);
        }

        public string GetAnswer(int maxCheatDistance, int minSavePicoseconds, bool printReport)
        {
            var path = FindPath(_map, _startNode, _endNode);
            var savedPicoseconds = CalculateCheats(path, maxCheatDistance, minSavePicoseconds);
            if (printReport)
                PrintReport(savedPicoseconds, path, minSavePicoseconds);
            var count = 0L;
            foreach (var saved in savedPicoseconds)
            {
                if (saved.Key >= minSavePicoseconds)
                    count += saved.Value;
            }
            return count.ToString();
        }

        private SortedDictionary<int, int> CalculateCheats(List<Node> path, int maxCheatDistance, int minSave)
        {
            var savedPicoseconds = new SortedDictionary<int, int>();
            for (int startIndex = 0; startIndex < path.Count - minSave; startIndex++)
            {
                for (int endIndex = startIndex + minSave; endIndex < path.Count; endIndex++)
                {
                    var cheatDistance = ManhattanDistance(path[startIndex], path[endIndex]);
                    if (cheatDistance <= maxCheatDistance)
                    {
                        if (cheatDistance < endIndex - startIndex)
                        {
                            var savedPicosend = endIndex - startIndex - cheatDistance;
                            if (savedPicoseconds.ContainsKey(savedPicosend))
                                savedPicoseconds[savedPicosend] += 1;
                            else
                                savedPicoseconds.Add(savedPicosend, 1);
                        }
                    }
                }
            }
            return savedPicoseconds;
        }

        private void PrintReport(SortedDictionary<int, int> savedPicosecond, List<Node> path, int minSave)
        {
            Console.WriteLine();
            Console.WriteLine(path);
            foreach (var node in path)
                Console.Write(node.ToString() + " ");
            Console.WriteLine();
            PrintMap(_map, path);
            foreach (var saved in savedPicosecond)
            {
                if (saved.Key >= minSave)
                    Console.WriteLine($"There are {saved.Value} cheats that save {saved.Key} picoseconds");
            }
        }

        private void PrintMap(List<char[]> map, List<Node> path)
        {
            for (int row = 0; row < map.Count; row++)
            {
                for (int col = 0;  col < map[row].Length; col++)
                {
                    if (path.Contains(new Node(row, col)))
                        Console.Write('O');
                    else
                        Console.Write(map[row][col]);
                }
                Console.WriteLine();
            }
        }

        public List<Node> FindPath(List<char[]> map, Node start, Node goal)
        {
            var closedSet = new HashSet<Node>();
            var openSet = new HashSet<Node>();
            var cameFrom = new Dictionary<Node, Node>();
            var gScore = new Dictionary<Node, int>();
            var fScore = new Dictionary<Node, int>();

            openSet.Add(start);
            gScore[start] = 0;
            fScore[start] = ManhattanDistance(start, goal);
            while (openSet.Count > 0)
            {
                var current = GetLowestFScoreNode(openSet, fScore);
                if (current.Equals(goal))
                    return ReconstructPath(cameFrom, current);
                openSet.Remove(current);
                closedSet.Add(current);
                foreach (var neighbor in GetNeighbors(map, current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;
                    int tentativeGScore = gScore[current] + 1;
                    if (!openSet.Contains(neighbor) || !gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + ManhattanDistance(neighbor, goal);
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
            return new List<Node>();
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

        private List<Node> GetNeighbors(List<char[]> map, Node node)
        {
            var neighbors = new List<Node>();
            int rows = map.Count;
            int cols = map[0].Length;
            int[] dr = { 0, 0, 1, -1 };
            int[] dc = { 1, -1, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                int new_row = node.Row + dr[i];
                int new_col = node.Col + dc[i];
                if (new_row >= 0 && new_row < rows && new_col >= 0 && new_col < cols)
                    if (map[new_row][new_col] == '.' || map[new_row][new_col] == 'E')
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

        private int ManhattanDistance(Node node1, Node node2)
        {
            return Math.Abs(node1.Row - node2.Row) + Math.Abs(node1.Col - node2.Col);
        }
    }
}
