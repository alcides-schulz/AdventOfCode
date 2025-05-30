using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent2024
{
    public class Day24CrossedWires : SolverBase
    {
        private class Operation
        {
            public string Wire1 { get; set; }
            public string Gate { get; set; }
            public string Wire2 { get; set; }
            public string WireResult { get; set; }
            public int Level { get; set; }

            public Operation(string wire1, string gate, string wire2, string wireResult, int level)
            {
                Wire1 = wire1;
                Gate = gate;
                Wire2 = wire2;
                WireResult = wireResult;
                Level = level;
            }

            public override string ToString()
            {
                return $"{Wire1} {Gate} {Wire2} {WireResult}";
            }
        }

        private SortedDictionary<string, int> _wireValues = new SortedDictionary<string, int>();
        private List<Operation> _operations = new List<Operation>();
        private string _highest_ZWire = "";

        public Day24CrossedWires(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null || line == "")
                    break;
                var wire = line.Split(':')[0];
                var value = int.Parse(line.Split(':')[1].Trim());
                _wireValues.Add(wire, value);
            }
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null || line == "")
                    break;
                var tokens = line.Replace(" -> ", " ").Split(' ');
                _operations.Add(new Operation(tokens[0], tokens[1], tokens[2], tokens[3], 0));
                if (tokens[3].StartsWith("z") && tokens[3].CompareTo(_highest_ZWire) > 0)
                    _highest_ZWire = tokens[3];
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            var unsolvedOperationCount = 1;
            while (unsolvedOperationCount != 0)
            {
                unsolvedOperationCount = 0;
                foreach (var operation in _operations)
                {
                    if (_wireValues.ContainsKey(operation.WireResult))
                        continue;
                    if (_wireValues.ContainsKey(operation.Wire1) && _wireValues.ContainsKey(operation.Wire2))
                    {
                        var wireValue1 = _wireValues[operation.Wire1];
                        var wireValue2 = _wireValues[operation.Wire2];
                        if (operation.Gate == "AND")
                            _wireValues[operation.WireResult] = wireValue1 & wireValue2;
                        if (operation.Gate == "OR")
                            _wireValues[operation.WireResult] = wireValue1 | wireValue2;
                        if (operation.Gate == "XOR")
                            _wireValues[operation.WireResult] = wireValue1 ^ wireValue2;
                        continue;
                    }
                    if (!_wireValues.ContainsKey(operation.Wire1))
                        unsolvedOperationCount += 1;
                    if (!_wireValues.ContainsKey(operation.Wire2))
                        unsolvedOperationCount += 1;
                }
            }
            var zValue = 0L;
            foreach (var wire in _wireValues.Reverse())
            {
                if (wire.Key.StartsWith("z"))
                    zValue = (zValue << 1) + wire.Value;
            }
            return zValue.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            var wrongWires = new SortedSet<string>();
            foreach (var operation in _operations)
            {
                if (operation.WireResult[0] == 'z' && operation.Gate != "XOR" && operation.WireResult != _highest_ZWire)
                    wrongWires.Add(operation.WireResult);
                if (operation.Gate == "XOR")
                    if ("xyz".IndexOf(operation.WireResult[0]) == -1 && "xyz".IndexOf(operation.Wire1[0]) == -1 && "xyz".IndexOf(operation.Wire2[0]) == -1)
                        wrongWires.Add(operation.WireResult);
                if (operation.Gate == "AND" && operation.Wire1 != "x00" && operation.Wire2 != "x00")
                    foreach (var operation2 in _operations)
                        if (operation.WireResult == operation2.Wire1 || operation.WireResult == operation2.Wire2)
                            if (operation2.Gate != "OR")
                                wrongWires.Add(operation.WireResult);
                if (operation.Gate == "XOR")
                    foreach (var operation2 in _operations)
                        if (operation.WireResult == operation2.Wire1 || operation.WireResult == operation2.Wire2)
                            if (operation2.Gate == "OR")
                                wrongWires.Add(operation.WireResult);
            }
            return string.Join(",", wrongWires);
        }
    }
}
