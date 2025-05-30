using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day08ResonantCollinearity : SolverBase
    {
        private readonly List<string> _map = new List<string>();

        public Day08ResonantCollinearity(string dataFileName)
        {
            _map.Clear();
            var inputData = new StreamReader(dataFileName);
            while (true)
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
            return GetAntinodes(GetAntenas(), false, true).Count.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            return GetAntinodes(GetAntenas(), true, false).Count.ToString();
        }

         private List<Tuple<int, int>> GetAntinodes(Dictionary<char, List<Tuple<int, int>>> antenas, bool countAntenas, bool onlyFirstAntinode)
        {
            var antinodes = new List<Tuple<int, int>>();
            foreach (var antena in antenas.Keys)
            {
                var locations = antenas[antena];
                for (int i = 0; i < locations.Count; i++)
                {
                    for (int j = i + 1; j < locations.Count; j++)
                    {
                        var location1 = locations[i];
                        var location2 = locations[j];
                        var rowDirection = location2.Item1 - location1.Item1;
                        var colDirection = location2.Item2 - location1.Item2;
                        DiscoverAntinodes(location1, -rowDirection, -colDirection, antinodes, countAntenas, onlyFirstAntinode);
                        DiscoverAntinodes(location2, +rowDirection, +colDirection, antinodes, countAntenas, onlyFirstAntinode);
                    }
                }
            }
            return antinodes;
        }

        private void DiscoverAntinodes(Tuple<int, int> antena, int rowDirection, int colDirection, List<Tuple<int, int>> antinodes, bool countAntenas, bool onlyFirstAntinode)
        {
            if (countAntenas && !antinodes.Contains(antena))
                antinodes.Add(antena);
            var antinode = Tuple.Create(antena.Item1 + rowDirection, antena.Item2 + colDirection);
            while (IsInsideMap(antinode.Item1, antinode.Item2))
            {
                if (!antinodes.Contains(antinode))
                    antinodes.Add(antinode);
                if (onlyFirstAntinode)
                    break;
                antinode = Tuple.Create(antinode.Item1 + rowDirection, antinode.Item2 + colDirection);
            }
        }

        private bool IsInsideMap(int row, int col)
        {
            return row >= 0 && row < _map.Count && col >= 0 && col < _map[row].Length;
        }

        private Dictionary<char, List<Tuple<int,int>>> GetAntenas()
        {
            var antenas = new Dictionary<char, List<Tuple<int, int>>>();
            for (int row = 0; row < _map.Count; row++)
            {
                for (int col = 0; col < _map[row].Length; col++)
                {
                    if (_map[row][col] != '.')
                    {
                        var antena = _map[row][col];
                        if (!antenas.ContainsKey(antena))
                            antenas.Add(antena, new List<Tuple<int, int>>());
                        antenas[antena].Add(Tuple.Create(row, col));
                    }
                }
            }
            return antenas;
        }

    }
}
