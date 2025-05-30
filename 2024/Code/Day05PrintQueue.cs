using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day05PrintQueue : SolverBase
    {
        private readonly List<Tuple<int, int>> _rules = new List<Tuple<int, int>>();
        private readonly List<List<int>> _updates = new List<List<int>>();

        public Day05PrintQueue(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            _rules.Clear();
            _updates.Clear();
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == "")
                    break;
                var parts = line.Split('|');
                _rules.Add(new Tuple<int, int>(int.Parse(parts[0]), int.Parse(parts[1])));
            }
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                var pages = line.Split(',');
                var pageList = new List<int>();
                foreach (var page in pages)
                {
                    pageList.Add(int.Parse(page));
                }
                _updates.Add(pageList);
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            int answer = 0;
            foreach(var update in _updates)
            {
                if (!IsValid(update))
                    continue;
                answer += update[update.Count / 2];
            }
            return answer.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            int answer = 0;
            foreach (var update in _updates)
            {
                if (IsValid(update))
                    continue;
                do
                {
                    Fix(update);
                } while(!IsValid(update));
                answer += update[update.Count / 2];
            }
            return answer.ToString();
        }

        public bool IsValid(List<int> update)
        {
            foreach (var rule in _rules)
            {
                var index1 = update.IndexOf(rule.Item1);
                var index2 = update.IndexOf(rule.Item2);
                if (index1 == -1 || index2 == -1)
                    continue;
                if (index1 > index2)
                    return false;
            }
            return true;
        }

        public void Fix(List<int> update)
        {
            foreach (var rule in _rules)
            {
                var index1 = update.IndexOf(rule.Item1);
                var index2 = update.IndexOf(rule.Item2);
                if (index1 == -1 || index2 == -1)
                    continue;
                if (index1 > index2)
                {
                    update.Insert(index1 + 1, rule.Item2);
                    update.RemoveAt(index2);
                }
            }
        }
    }
}
