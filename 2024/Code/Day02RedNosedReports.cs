using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day02RedNosedReports : SolverBase
    {
        private readonly List<List<int>> _list = new List<List<int>>();

        public Day02RedNosedReports(string dataFileName)
        {
            _list.Clear();
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null) break;
                var tokens = line.Split(' ');
                var numbers = new List<int>();
                _list.Add(numbers);
                for (int i = 0; i < tokens.Length; i++)
                {
                    var number = Int32.Parse(tokens[i]);
                    numbers.Add(number);
                }
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            int safeCount = 0;
            foreach (var numbers in _list)
            {
                if (IsReportSafe(numbers))
                    safeCount++;
            }
            return safeCount.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            int safeCount = 0;
            var new_numbers = new List<int>();
            foreach (var numbers in _list)
            {
                if (IsReportSafe(numbers))
                {
                    safeCount++;
                    continue;
                }
                for (int i = 0; i < numbers.Count; i++)
                {
                    new_numbers.Clear();
                    new_numbers.AddRange(numbers);
                    new_numbers.RemoveAt(i);
                    if (IsReportSafe(new_numbers))
                    {
                        safeCount++;
                        break;
                    }
                }
            }
            return safeCount.ToString();
        }
        private bool IsReportSafe(List<int> numbers)
        {
            if (numbers[0] < numbers[1] && IsSafeIncreasing(numbers))
                return true;
            if (numbers[0] > numbers[1] && IsSafeDecreasing(numbers))
                return true;
            return false;
        }

        private bool IsSafeIncreasing(List<int> numbers)
        {
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                var diff = numbers[i + 1] - numbers[i];
                if (diff < 1 || diff > 3)
                    return false;
            }
            return true;
        }

        private bool IsSafeDecreasing(List<int> numbers)
        {
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                var diff = numbers[i] - numbers[i + 1];
                if (diff < 1 || diff > 3)
                    return false;
            }
            return true;
        }

    }
}
