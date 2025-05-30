using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent2024
{
    public class Day25CodeChronicle : SolverBase
    {
        private List<int[]> _locks = new List<int[]>();
        private List<int[]> _keys = new List<int[]>();

        public Day25CodeChronicle(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                if (line == "#####")
                {
                    int[] lck = { 0, 0, 0, 0, 0 };
                    _locks.Add(lck);
                    line = inputData.ReadLine();
                    while (line != null && line != "")
                    {
                        for (int i = 0; i < 5; i++)
                            if (line[i] == '#')
                                lck[i]++;
                        line = inputData.ReadLine();
                    }
                }
                else
                {
                    int[] key = { -1, -1, -1, -1, -1 };
                    _keys.Add(key);
                    line = inputData.ReadLine();
                    while (line != null && line != "")
                    {
                        for (int i = 0; i < 5; i++)
                            if (line[i] == '#')
                                key[i]++;
                        line = inputData.ReadLine();
                    }
                }
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            var count = 0L;
            foreach (var lck in _locks)
                foreach (var key in _keys)
                    if (Match(lck, key))
                        count++;
            return count.ToString();
        }

        private bool Match(int[] lck, int[] key)
        {
            for (int i = 0; i < 5; i++)
                if (lck[i] + key[i] > 5)
                    return false;
            return true;
        }

        public override string GetPartTwoAnswer()
        {
            return "0";
        }
    }
}
