using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Advent2024
{
    public class Day16ReindeerMaze : SolverBase
    {
        private readonly List<string> _map = new List<string>();

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

        public override string GetPartOneAnswer()
        {
            return CalculateScore(FindAllShortestPathsDijkstra(_map, FindStart(), FindGoal())[0]).ToString();
        }

        public override string GetPartTwoAnswer()
        {
            return CalculateBestSpots(FindAllShortestPathsDijkstra(_map, FindStart(), FindGoal())).ToString();
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
        private long CalculateBestSpots(List<List<Position>> paths)
        {
            var allNodes = new List<Position>();
            foreach (var path in paths)
            {
                foreach (var node in path)
                {
                    if (!allNodes.Contains(node))
                        allNodes.Add(node);
                }
            }
            return allNodes.Count;
        }

        private class Position : Tuple<int, int>
        {
            public Position(int row, int col) : base(row, col) { }
            public int Row { get { return Item1; } }
            public int Col { get { return Item2; } }
        }

        private class Node
        {
            public Position Position;
            public int Distance;
            public char Direction;
            public List<Node> Parents;

            public Node(Position position, int distance, char direction)
            {
                Position = position;
                Distance = distance;
                Direction = direction;
                Parents = new List<Node>();
            }
        }

        private List<List<Position>> FindAllShortestPathsDijkstra(List<string> map, Position start, Position end)
        {
            var nodeCost = new Dictionary<(Position, char), Node>();
            var openSet = new List<Node>();

            char[] direction = { 'E', 'W', 'S', 'N' };
            int[] dirRow = { 0, 0, 1, -1 };
            int[] dirCol = { 1, -1, 0, 0 };

            var startNode = new Node(start, 0, 'E');

            nodeCost[(start, 'E')] = startNode;
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(node => node.Distance).First();
                openSet.Remove(current);
                
                if (current.Position.Equals(end))
                    return ReconstructPaths(current);

                for (int i = 0; i < 4; i++)
                {
                    var nextRow = current.Position.Row + dirRow[i];
                    var nextCol = current.Position.Col + dirCol[i];
                    if (!IsValid(map, nextRow, nextCol) || map[nextRow][nextCol] == '#')
                        continue;
                    var neighborPosition = new Position(nextRow, nextCol);
                    int newDistance = current.Distance + 1;
                    char newDirection = current.Direction;
                    if (newDirection != direction[i])
                    {
                        newDistance += 1000;
                        newDirection = direction[i];
                    }
                    if (!nodeCost.ContainsKey((neighborPosition, newDirection)))
                    {
                        var newNeighbor = new Node(neighborPosition, newDistance, newDirection);
                        newNeighbor.Parents.Add(current);
                        nodeCost[(neighborPosition, newDirection)] = newNeighbor;
                        openSet.Add(newNeighbor);
                        continue;
                    }
                    var existingNeighbor = nodeCost[(neighborPosition, newDirection)];
                    if (newDistance < existingNeighbor.Distance)
                    {
                        existingNeighbor.Distance = newDistance;
                        existingNeighbor.Direction = newDirection;
                        existingNeighbor.Parents.Clear();
                        existingNeighbor.Parents.Add(current);
                        if (!openSet.Contains(existingNeighbor))
                            openSet.Add(existingNeighbor);
                        continue;
                    }
                    if (newDistance == existingNeighbor.Distance)
                    {
                        existingNeighbor.Parents.Add(current);
                        if (!openSet.Contains(existingNeighbor))
                            openSet.Add(existingNeighbor);
                        continue;
                    }
                }
            }

            return new List<List<Position>>();
        }

        private bool IsValid(List<string> map, int row, int col)
        {
            return row >= 0 && row < map.Count && col >= 0 && col < map[row].Length;
        }

        private List<List<Position>> ReconstructPaths(Node current)
        {
            var paths = new List<List<Position>>();
            if (current.Parents.Count == 0)
            {
                paths.Add(new List<Position> { current.Position });
                return paths;
            }
            foreach (Node parent in current.Parents)
            {
                var parentPaths = ReconstructPaths(parent);
                foreach (var parentPath in parentPaths)
                {
                    var newPath = new List<Position>(parentPath);
                    newPath.Add(current.Position);
                    paths.Add(newPath);
                }
            }
            return paths;
        }

    }
}

