using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day07BridgeRepair : SolverBase
    {
        private readonly List<long> _values = new List<long>();
        private readonly List<List<int>> _numbers = new List<List<int>>();

        public Day07BridgeRepair(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _values.Add(long.Parse(line.Split(':')[0]));
                var stringNumbers = line.Split(':')[1].Trim().Split();
                var numberList = new List<int>();
                foreach (var stringNumber in stringNumbers)
                {
                    numberList.Add(int.Parse(stringNumber));
                }
                _numbers.Add(numberList);
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            long answer = 0;
            for (int i = 0; i < _values.Count; i++)
            {
                var operatorList = new List<string>();
                GenerateCombinations("+*".ToCharArray(), _numbers[i].Count - 1, "", operatorList);
                foreach (var operators in operatorList)
                {
                    var result = Resolve(_numbers[i], operators);
                    if (result == _values[i])
                    {
                        answer += result;
                        break;
                    }
                }
            }
            return answer.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            long answer = 0;
            for (int i = 0; i < _values.Count; i++)
            {
                var operatorList = new List<string>();
                GenerateCombinations("+*|".ToCharArray(), _numbers[i].Count - 1, "", operatorList);
                foreach (var operators in operatorList)
                {
                    var result = Resolve(_numbers[i], operators);
                    if (result == _values[i])
                    {
                        answer += result;
                        break;
                    }
                }
            }
            return answer.ToString();
        }

        private long Resolve(List<int> numbers, string operators)
        {
            long result = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                if (i == 0)
                    result = numbers[i];
                else
                    switch (operators[i - 1]) {
                        case '+': 
                            result += numbers[i];
                            break;
                        case '*':
                            result *= numbers[i];
                            break;
                        case '|':
                            result = long.Parse(result.ToString() + numbers[i].ToString());
                            break;
                    }
            }
            return result;
        }

        private static void GenerateCombinations(char[] operators, int targetLength, string currentCombination, List<string> combinations)
        {
            if (currentCombination.Length == targetLength)
            {
                combinations.Add(currentCombination);
                return;
            }
            for (int i = 0; i < operators.Length; i++)
            {
                GenerateCombinations(operators, targetLength, currentCombination + operators[i], combinations);
            }
        }

    }
}
