using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day01HistorianHysteria : SolverBase
    {
        private readonly List<int> _list1 = new List<int>();
        private readonly List<int> _list2 = new List<int>();
        
        public Day01HistorianHysteria(string dataFileName)
        {
            _list1.Clear();
            _list2.Clear();
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null) break;
                var numbers = line.Split(' ');
                _list1.Add(Int32.Parse(numbers[0]));
                _list2.Add(Int32.Parse(numbers[numbers.Length - 1]));
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            _list1.Sort();
            _list2.Sort();
            var totalDistance = 0;
            for (int i = 0; i < _list1.Count; i++)
            {
                totalDistance += Math.Abs(_list1[i] - _list2[i]);
            }
            return totalDistance.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            var countTable = new Dictionary<int, int>();
            foreach (var i in _list2)
            {
                if (countTable.ContainsKey(i))
                    countTable[i]++;
                else
                    countTable.Add(i, 1);
            }
            var score = 0;
            foreach (var i in _list1)
            {
                if (countTable.ContainsKey(i))
                    score += i * countTable[i];
            }
            return score.ToString();
        }

    }
}
