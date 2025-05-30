using Microsoft.Win32;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace Advent2024
{
    public class Day17ChronospatialComputer : SolverBase
    {
        private class Computer
        {
            public long RegisterA = 0;
            public long RegisterB = 0;
            public long RegisterC = 0;
            public List<int> Program = new List<int>();
            public int InstructionPointer = 0;

            public string Run()
            {
                var output = "";

                while (InstructionPointer < Program.Count() - 1)
                {
                    var opcode = Program[InstructionPointer];
                    var operand = Program[InstructionPointer + 1];
                    switch (opcode)
                    {
                        case 0:
                            RegisterA /= (long)Math.Pow(2, ComboOperand(operand));
                            break;
                        case 1:
                            RegisterB ^= operand;
                            break;
                        case 2:
                            RegisterB = ComboOperand(operand) % 8;
                            break;
                        case 3:
                            if (RegisterA != 0)
                                InstructionPointer = (operand - 2);
                            break;
                        case 4:
                            RegisterB ^= RegisterC;
                            break;
                        case 5:
                            output += (output == "" ? "" : ",") + (ComboOperand(operand) % 8);
                            break;
                        case 6:
                            RegisterB = RegisterA / (long)Math.Pow(2, ComboOperand(operand));
                            break;
                        case 7:
                            RegisterC = RegisterA / (long)Math.Pow(2, ComboOperand(operand));
                            break;
                    }
                    InstructionPointer += 2;
                }
                return output;
            }

            private long ComboOperand(int operand)
            {
                if (operand == 4)
                    return RegisterA;
                if (operand == 5)
                    return RegisterB;
                if (operand == 6)
                    return RegisterC;
                return operand;
            }
        }

        private Computer _computer = new Computer();
        private string _testProgram = "";

        public Day17ChronospatialComputer(string dataFileName, string testProgram)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                if (line.StartsWith("Register A:"))
                    _computer.RegisterA = int.Parse(line.Substring(11));
                if (line.StartsWith("Register B:"))
                    _computer.RegisterB = int.Parse(line.Substring(11));
                if (line.StartsWith("Register C:"))
                    _computer.RegisterC = int.Parse(line.Substring(11));
                if (line.StartsWith("Program"))
                    _computer.Program.AddRange(line.Substring(8).Split(',').Select(s => int.Parse(s)).ToList());
            }
            inputData.Close();
            _testProgram = testProgram;
        }

        public override string GetPartOneAnswer()
        {
            return _computer.Run();
        }

        public override string GetPartTwoAnswer()
        {
            if (_testProgram != "")
            {
                _computer.Program.Clear();
                _computer.Program.AddRange(_testProgram.Split(',').Select(s => int.Parse(s)).ToList());
            }
            var programString = string.Join("", _computer.Program.ToArray());
            long AValue = 0;
            bool searching = true;
            for (int i = 0; i < _computer.Program.Count() && searching; i++)
            {
                var partialProgram = programString.Substring(programString.Length - i - 1);
                for (int j = 0; j <= 64; j++)
                {
                    var TestA = AValue * 8 + j;
                    _computer.RegisterA = TestA;
                    _computer.RegisterB = 0;
                    _computer.RegisterC = 0;
                    _computer.InstructionPointer = 0;
                    var result = _computer.Run().Replace(",", "");
                    if (result == partialProgram)
                    {
                        AValue = TestA;
                        if (result == programString)
                            searching = false;
                        break;
                    }
                }
            }
            return AValue.ToString();
        }
    }
}

