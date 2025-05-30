using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day10HoofIt : SolverBase
    {
        private List<string> _map = new List<string>();
        private readonly int[,] _direction = { { -1, 0 }, { 0, +1 }, { +1, 0 }, { 0, -1 } };

        public Day10HoofIt(string dataFileName)
        {
            _map.Clear();
            var inputData = new StreamReader(dataFileName);
            while(true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _map.Add(line);
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            return GetTrailheadCount(true).ToString();
        }

        public override string GetPartTwoAnswer()
        {
            return GetTrailheadCount(false).ToString();
        }

        private int GetTrailheadCount(bool distinctCount)
        {
            var count = 0;
            for (int row = 0; row < _map.Count; row++)
            {
                for (int col = 0; col < _map[row].Length; col++)
                {
                    if (_map[row][col] == '0')
                    {
                        var reachable = new List<Tuple<int, int>>();
                        SearchTrail(Tuple.Create(row, col), reachable, distinctCount);
                        count += reachable.Count;
                    }
                }
            }
            return count;
        }

        private void SearchTrail(Tuple<int,int> position, List<Tuple<int,int>> reachable, bool distinctCount)
        {
            if (_map[position.Item1][position.Item2] == '9')
            {
                if (!distinctCount || !reachable.Contains(position))
                    reachable.Add(position);
                return;
            }
            foreach(var nextPosition in GetPossiblePositions(position))
            {
                SearchTrail(nextPosition, reachable, distinctCount);
            }
        }

        private List<Tuple<int,int>> GetPossiblePositions(Tuple<int,int> fromPosition)
        {
            var possible = new List<Tuple<int,int>>();
            var nextHeight = (char)(_map[fromPosition.Item1][fromPosition.Item2] + 1);
            for (int i = 0; i < _direction.GetLength(0); i++)
            {
                var nextPosition = Tuple.Create(fromPosition.Item1 + _direction[i, 0], fromPosition.Item2 + _direction[i, 1]);
                if (IsValidPosition(nextPosition) && _map[nextPosition.Item1][nextPosition.Item2] == nextHeight)
                    possible.Add(nextPosition);
            }
            return possible;
        }

        private bool IsValidPosition(Tuple<int, int> position)
        {
            if (position.Item1 < 0 || position.Item1 >= _map.Count)
                return false;
            if (position.Item2 < 0 || position.Item2 >= _map[position.Item1].Length)
                return false;
            return true;
        }
    }
}
