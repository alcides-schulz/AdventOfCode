using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day06GuardGallivant : SolverBase
    {
        private readonly List<string> _grid = new List<string>();
        private int _startingGuardRow = -1;
        private int _startingGuardCol = -1;

        private readonly int[,] _direction = { { -1, 0 }, { 0, +1 }, { +1, 0 }, { 0, -1 } };

        public Day06GuardGallivant(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            _grid.Clear();
            _startingGuardRow = 0;
            _startingGuardCol = 0;
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                var col = line.IndexOf('^');
                if (col != -1)
                {
                    _startingGuardCol = col;
                    _startingGuardRow = _grid.Count - 1;
                    line = line.Replace("^", ".");
                }
                _grid.Add(line);
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            return GetVisitedNodes().Count.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            int answer = 0;
            var visited = GetVisitedNodes();
            foreach (var node in visited)
            {
                var obstacleRow = node.Item1;
                var obstacleCol = node.Item2;
                if (_grid[obstacleRow][obstacleCol] == '#')
                    continue;
                if (obstacleRow == _startingGuardRow && obstacleCol == _startingGuardCol)
                    continue;
                if (CreatesLoop(obstacleRow, obstacleCol))
                    answer++;
            }
            return answer.ToString();
        }

        private bool CreatesLoop(int obstacleRow, int obstacleCol)
        {
            var currentDirectionIndex = 0;
            var walkRow = _startingGuardRow;
            var walkCol = _startingGuardCol;
            var turns = new List<Tuple<int, int, int, int>>();
            while (true)
            {
                var newRow = walkRow + _direction[currentDirectionIndex, 0];
                var newCol = walkCol + _direction[currentDirectionIndex, 1];
                if (!IsInsideGrid(newRow, newCol))
                    break;
                if (_grid[newRow][newCol] == '#' || (newRow == obstacleRow && newCol == obstacleCol))
                {
                    var turn = new Tuple<int, int, int, int>(walkRow, walkCol, newRow, newCol);
                    if (turns.Contains(turn))
                        return true;
                    turns.Add(turn);
                    currentDirectionIndex++;
                    if (currentDirectionIndex == 4)
                        currentDirectionIndex = 0;
                    continue;
                }
                walkRow = newRow;
                walkCol = newCol;
            }
            return false;
        }

        public List<Tuple<int,int>> GetVisitedNodes()
        {
            var visited = new List<Tuple<int, int>>();
            visited.Add(new Tuple<int, int>(_startingGuardRow, _startingGuardCol));
            var currentDirectionIndex = 0;
            var walkRow = _startingGuardRow;
            var walkCol = _startingGuardCol;
            while (true)
            {
                var newRow = walkRow + _direction[currentDirectionIndex, 0];
                var newCol = walkCol + _direction[currentDirectionIndex, 1];
                if (!IsInsideGrid(newRow, newCol))
                    break;
                if (_grid[newRow][newCol] == '#')
                {
                    currentDirectionIndex = (currentDirectionIndex + 1) % 4;
                    if (currentDirectionIndex == 4)
                        currentDirectionIndex = 0;
                    continue;
                }
                var position = new Tuple<int, int>(newRow, newCol);
                if (!visited.Contains(position))
                    visited.Add(position);
                walkRow = newRow;
                walkCol = newCol;
            }
            return visited;
        }

        private bool IsInsideGrid(int row, int col)
        {
            if (row < 0 || row >= _grid.Count) 
                return false;
            if (col < 0 || col >= _grid[row].Length)
                return false;
            return true;
        }

    }
}
