using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent2024
{
    public class Day03MullItOver : SolverBase
    {

        private readonly List<string> _list = new List<string>();

        public Day03MullItOver(string dataFileName)
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
            var total = 0;
            foreach (var line in _list)
            {
                var mul_start = 0;
                while (true)
                {
                    mul_start = line.IndexOf("mul(", mul_start);
                    if (mul_start == -1)
                        break;
                    mul_start += 4;
                    var mul_end = line.IndexOf(")", mul_start);
                    if (mul_end == -1)
                        break;
                    var operands = line.Substring(mul_start, mul_end - mul_start).Split(',');
                    if (operands.Length != 2)
                        continue;
                    if (operands[0].All(Char.IsDigit) == false || operands[0].Length > 3)
                        continue;
                    if (operands[1].All(Char.IsDigit) == false || operands[1].Length > 3)
                        continue;
                    total += Int32.Parse(operands[0]) * Int32.Parse(operands[1]);
                }
            }
            return total.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            var total = 0;
            var do_calculation = true;
            foreach (var line in _list)
            {
                var mul_start = 0;
                while (true)
                {
                    var do_index = line.IndexOf("do()", mul_start);
                    var dont_index = line.IndexOf("don't()", mul_start);
                    mul_start = line.IndexOf("mul(", mul_start);
                    if (mul_start == -1)
                        break;
                    if (do_index != -1 && do_index < mul_start && (do_index < dont_index || dont_index == -1))
                        do_calculation = true;
                    if (dont_index != -1 && dont_index < mul_start && (dont_index < do_index || do_index == -1))
                        do_calculation = false;
                    mul_start += 4;
                    var mul_end = line.IndexOf(")", mul_start);
                    if (mul_end == -1)
                        break;
                    var operands = line.Substring(mul_start, mul_end - mul_start).Split(',');
                    if (operands.Length != 2)
                        continue;
                    if (operands[0].All(Char.IsDigit) == false || operands[0].Length > 3)
                        continue;
                    if (operands[1].All(Char.IsDigit) == false || operands[1].Length > 3)
                        continue;
                    if (do_calculation)
                        total += Int32.Parse(operands[0]) * Int32.Parse(operands[1]);
                }
            }
            return total.ToString();
        }
  
    }
}
