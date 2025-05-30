using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace Advent2024
{
    /*
        +---+---+---+
        | 7 | 8 | 9 |
        +---+---+---+
        | 4 | 5 | 6 |
        +---+---+---+
        | 1 | 2 | 3 |
        +---+---+---+
            | 0 | A |
            +---+---+

            +---+---+
            | ^ | A |
        +---+---+---+
        | < | v | > |
        +---+---+---+
    */
    public class Day21KeypadConundrum : SolverBase
    {
        private readonly List<string> _codes = new List<string>();
        private readonly List<string> _numpadLayout = new List<string> { "789", "456", "123", " 0A" };
        private readonly List<string> _dirpadLayout = new List<string> { " ^A", "<v>" };
        private readonly Dictionary<Tuple<char, char>, List<string>> _numpadMoves = null;
        private readonly Dictionary<Tuple<char, char>, List<string>> _dirpadMoves = null;
        private readonly Dictionary<Tuple<string, int>, long> _searchCache = new Dictionary<Tuple<string, int>, long>();

        public Day21KeypadConundrum(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _codes.Add(line);
            }
            inputData.Close();
            _numpadMoves = PreparePaths(_numpadLayout);
            _dirpadMoves = PreparePaths(_dirpadLayout);
        }

        public override string GetPartOneAnswer()
        {
            return CalculateComplexitySum(2).ToString();
        }

        public override string GetPartTwoAnswer()
        {
            return CalculateComplexitySum(25).ToString();
        }

        private long CalculateComplexitySum(int depth)
        {
            var complexitySum = 0L;
            foreach (var code in _codes)
            {
                var prevChar = 'A';
                var totalLength = 0L;
                foreach (var currentChar in code)
                {
                    var lengthList = new List<long>();
                    foreach (var moveSequence in _numpadMoves[Tuple.Create(prevChar, currentChar)])
                        lengthList.Add(SearchMinLength(moveSequence, depth));
                    totalLength += lengthList.Min();
                    prevChar = currentChar;
                }
                complexitySum += int.Parse(code.Replace("A", "")) * totalLength;
            }
            return complexitySum;
        }

        private long SearchMinLength(string moveSequence, int depth)
        {
            var searchKey = Tuple.Create(moveSequence, depth);
            if (_searchCache.ContainsKey(searchKey))
                return _searchCache[searchKey];
            var totalLength = 0L;
            if (depth == 0)
            {
                totalLength = moveSequence.Length;
            }
            else
            {
                var prevChar = 'A';
                foreach (var currentChar in moveSequence)
                {
                    var lengthList = new List<long>();
                    foreach (var nextMoveSequence in _dirpadMoves[Tuple.Create(prevChar, currentChar)])
                        lengthList.Add(SearchMinLength(nextMoveSequence, depth - 1));
                    totalLength += lengthList.Min();
                    prevChar = currentChar;
                }
            }
            _searchCache[searchKey] = totalLength;
            return totalLength;
        }
  
        private Dictionary<Tuple<char, char>, List<string>> PreparePaths(List<string> keypad)
        {
            var pathList = new Dictionary<Tuple<char, char>, List<string>>();
            for (int rowFrom = 0; rowFrom < keypad.Count; rowFrom++)
            {
                for (int colFrom = 0; colFrom < keypad[rowFrom].Length; colFrom++)
                {
                    if (keypad[rowFrom][colFrom] == ' ')
                        continue;
                    for (int rowTo = 0; rowTo < keypad.Count; rowTo++)
                    {
                        for (int colTo = 0; colTo < keypad[rowTo].Length; colTo++)
                        {
                            if (keypad[rowTo][colTo] == ' ')
                                continue;
                            var nodeFrom = new Node(rowFrom, colFrom);
                            var nodeTo = new Node(rowTo, colTo);
                            var allPaths = FindAllPaths(keypad, nodeFrom, nodeTo);
                            var sequences = new List<string>();
                            foreach (var path in allPaths)
                            {
                                var sequence = "";
                                for (int i = 1; i < path.Count; i++)
                                    sequence += GetDirection(path[i - 1], path[i]);
                                sequences.Add(sequence + 'A');
                            }
                            pathList[Tuple.Create(keypad[rowFrom][colFrom], keypad[rowTo][colTo])] = sequences;
                        }
                    }
                }
            }
            return pathList;
        }

        private char GetDirection(Node from, Node to)
        {
            if (from.Row == to.Row && from.Col < to.Col) return '>';
            if (from.Row == to.Row && from.Col > to.Col) return '<';
            if (from.Row < to.Row && from.Col == to.Col) return 'v';
            if (from.Row > to.Row && from.Col == to.Col) return '^';
            return '?';
        }

        public List<List<Node>> FindAllPaths(List<string> keypad, Node start, Node end)
        {
            var path = new List<Node>();
            var allPaths = new List<List<Node>>();
            if (start != end)
            {
                path.Add(start);
                FindPaths(keypad, start, end, path, allPaths);
            }
            return allPaths;
        }

        private void FindPaths(List<string> keypad, Node current, Node end, List<Node> path, List<List<Node>> allPaths)
        {
            if (current.Equals(end))
            {
                allPaths.Add(new List<Node>(path));
                return;
            }
            int currentRow = current.Row;
            int currentCol = current.Col;
            int[] dr = { 0, 1, 0, -1 };
            int[] dc = { 1, 0, -1, 0 };
            for (int i = 0; i < 4; i++)
            {
                int newRow = currentRow + dr[i];
                int newCol = currentCol + dc[i];
                Node next = new Node(newRow, newCol);
                if (IsValid(keypad, next) && !path.Contains(next))
                {
                    if (keypad[newRow][newCol] == ' ')
                        continue;
                    path.Add(next);
                    FindPaths(keypad, next, end, path, allPaths);
                    path.RemoveAt(path.Count - 1);
                }
            }
        }

        private bool IsValid(List<string> keypad, Node p)
        {
            int rows = keypad.Count;
            int cols = keypad[0].Length;
            return p.Row >= 0 && p.Row < rows && p.Col >= 0 && p.Col < cols;
        }
    }
}
