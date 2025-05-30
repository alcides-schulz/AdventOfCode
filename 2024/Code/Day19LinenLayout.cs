using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent2024
{
    public class Day19LinenLayout : SolverBase
    {
        private readonly List<string> _patterns = new List<string>();
        private readonly List<string> _designs = new List<string>();
        private readonly Dictionary<string, long> _design_cache = new Dictionary<string, long>();

        public Day19LinenLayout(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            var patternLine = inputData.ReadLine();
            _patterns.AddRange(patternLine.Replace(" ", "").Split(',').ToArray());
            inputData.ReadLine(); // skip blank line
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _designs.Add(line);
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            var possible = 0L;
            foreach (var layout in _designs)
            {
                if (CountDesigns(layout) != 0L)
                    possible++;
            }
            return possible.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            var totalCount = 0L;
            foreach (var design in _designs)
            {
                totalCount += CountDesigns(design);
            }
            return totalCount.ToString();
        }

        private long CountDesigns(string design)
        {
            if (design == "")
                return 1L;
            if (_design_cache.ContainsKey(design))
                return _design_cache[design];
            var count = 0L;
            foreach (var pattern in _patterns)
            {
                if (design.StartsWith(pattern))
                    count += CountDesigns(design.Substring(pattern.Length));
            }
            _design_cache.Add(design, count);
            return count;
        }

    }
}

