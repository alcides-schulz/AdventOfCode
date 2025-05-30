using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day11PlutonianPebbles : SolverBase
    {
        private readonly List<long> _stones = new List<long>();

        public Day11PlutonianPebbles(string dataFileName)
        {
            _stones.Clear();
            var inputData = new StreamReader(dataFileName);
            var line = inputData.ReadLine();
            var tokens = line.Split(' ');
            foreach (var token in tokens)
                _stones.Add(long.Parse(token));
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            return Blink(_stones, 25).ToString();
        }

        public override string GetPartTwoAnswer()
        {
            return Blink(_stones, 75).ToString();
        }

        private long Blink(List<long> stones, int times)
        {
            var currentStones = new Dictionary<long, long>();
            foreach(var stone in stones)
            {
                if (currentStones.ContainsKey(stone))
                    currentStones[stone] += 1;
                else
                    currentStones.Add(stone, 1);
            }
            for (int i = 0; i < times; i++)
            {
                var newStones = new Dictionary<long, long>();
                foreach (var stone in currentStones.Keys)
                {
                    var currentCount = currentStones[stone];
                    if (stone == 0)
                    {
                        if (newStones.ContainsKey(1))
                            newStones[1] += currentCount;
                        else
                            newStones.Add(1, currentCount);
                    }
                    else if (stone.ToString().Length % 2 == 0)
                    {
                        int len = stone.ToString().Length;
                        var leftStone = long.Parse(stone.ToString().Substring(0, len / 2));
                        var rightStone = long.Parse(stone.ToString().Substring(len / 2));
                        if (newStones.ContainsKey(leftStone))
                            newStones[leftStone] += currentCount;
                        else
                            newStones.Add(leftStone, currentCount);
                        if (newStones.ContainsKey(rightStone))
                            newStones[rightStone] += currentCount;
                        else
                            newStones.Add(rightStone, currentCount);
                    }
                    else
                    {
                        var newStone = stone * 2024;
                        if (newStones.ContainsKey(newStone))
                            newStones[newStone] += currentCount;
                        else
                            newStones.Add(newStone, currentCount);
                    }
                }
                currentStones = newStones;
            }
            long count = 0;
            foreach(var stone in currentStones.Keys)
            {
                count += currentStones[stone];
            }
            return count;
        }
    }
}
