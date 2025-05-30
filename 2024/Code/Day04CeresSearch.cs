using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day04CeresSearch : SolverBase
    {
        private readonly List<string> _list = new List<string>();
        private readonly int[,] _direction = { { -1, 0 }, { -1, +1 }, { 0, +1 }, { +1, +1 }, { +1, 0 }, { +1, -1 }, { 0, -1 }, { -1, -1 } };
        private readonly string[] _xmas_pattern = { "M MS MS SM S",
                                                    " A  A  A  A ",
                                                    "S SS MM MM S"};

        public Day04CeresSearch(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            _list.Clear();
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null) break;
                _list.Add(line);
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            int total = 0;
            for (int row = 0; row < _list.Count; row++)
            {
                for (int col = 0; col < _list[row].Length; col++)
                {
                    total += CountWords(row, col, "XMAS");
                }

            }
            return total.ToString();
        }

        private int CountWords(int row, int col, string word)
        {
            int count = 0;
            for (int i = 0; i < _direction.GetLength(0); i++)
            {
                if (HasWord(row, col, _direction[i, 0], _direction[i, 1], word))
                    count++;
            }
            return count;
        }

        private bool HasWord(int row, int col, int rowDirection, int colDirection, string word)
        {
            var match_count = 0;
            for (int i = 0; i < word.Length; i++)
            {
                if (_list[row][col] == word[i])
                    match_count++;
                row += rowDirection;
                col += colDirection;
                if (row < 0 || row >= _list.Count)
                    break;
                if (col < 0 || col >= _list[row].Length)
                    break; ;
            }
            return match_count == word.Length;
        }

        public override string GetPartTwoAnswer()
        {
            int count = 0;
            for (int row = 0; row <= _list.Count - 3; row++)
            {
                for (int col = 0; col <= _list[row].Length - 3; col++)
                {
                    if (HasXmasPattern(row, col))
                        count++;
                }
            }
            return count.ToString();
        }

        private bool HasXmasPattern(int row, int col)
        {
            for (int i = 0; i < 4; i++)
            {
                var match_count = 0;
                for (int pr = 0; pr < 3; pr++)
                {
                    for (int pc = 0; pc < 3; pc++)
                    {
                        if (_xmas_pattern[pr][i * 3 + pc] == ' ')
                            match_count++;
                        if (_xmas_pattern[pr][i * 3 + pc] == _list[row + pr][col + pc])
                            match_count++;
                    }
                }
                if (match_count == 9)
                    return true;
            }
            return false;
        }
    }
}
