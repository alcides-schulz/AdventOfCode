using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day13ClawContraption : SolverBase
    {
        public class Coordinate : Tuple<int, int>
        {
            public Coordinate(int x, int y) : base(x, y) { }

            public int X { get { return Item1; } }
            public int Y { get { return Item2; } }
        }

        private class Machine
        {
            public Coordinate ButtonA;
            public Coordinate ButtonB;
            public Coordinate Prize;
        }

        private readonly List<Machine> _machine = new List<Machine>();

        public Day13ClawContraption(string dataFileName)
        {
            _machine.Clear();
            var inputData = new StreamReader(dataFileName);
            Machine machine = null;
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                if (line == "")
                    continue;
                if (line.StartsWith("Button A"))
                {
                    machine = new Machine();
                    _machine.Add(machine);
                }
                var values = line.Split(':')[1];
                var x = values.Split(',')[0].Replace("X", "").Replace("=", "").Trim();
                var y = values.Split(',')[1].Replace("Y", "").Replace("=", "").Trim();
                if (line.StartsWith("Button A"))
                    machine.ButtonA = new Coordinate(int.Parse(x), int.Parse(y));
                else if (line.StartsWith("Button B"))
                    machine.ButtonB = new Coordinate(int.Parse(x), int.Parse(y));
                else
                    machine.Prize = new Coordinate(int.Parse(x), int.Parse(y));
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            long total = 0;
            foreach(var machine in _machine)
            {
                total += CalculateTokens(machine, 0);
            }
            return total.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            long total = 0;
            foreach (var machine in _machine)
            {
                total += CalculateTokens(machine, 10000000000000);
            }
            return total.ToString();
        }

        /// <summary>
        /// Define formula for each button times:
        ///     ATimes * ButtonA.X + BTimes * ButtonB.X = Prize.X
        ///     ATimes * ButtonA.Y + BTimes * ButtonB.Y = Prize.Y
        /// Isolate ATimes and BTimes calculation:
        ///     ATimes = (Prize.X - (BTimes * ButtonB.X)) / ButtonA.X
        ///     BTimes = (Prize.Y - (BTimes * ButtonB.Y)) / ButtonA.Y
        /// Substitute BTimes in ATimes formula and simplify.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="prizeAdjustment"></param>
        /// <returns></returns>
        private long CalculateTokens(Machine machine, long prizeAdjustment)
        {
            var buttonATimes = CalculateButtonATimes(machine, prizeAdjustment);
            var buttonBTimes = CalculateButtonBTimes(machine, buttonATimes, prizeAdjustment);
            var totalX = machine.ButtonA.X * buttonATimes + machine.ButtonB.X * buttonBTimes;
            var totalY = machine.ButtonA.Y * buttonATimes + machine.ButtonB.Y * buttonBTimes;
            if (totalX == machine.Prize.X + prizeAdjustment && totalY == machine.Prize.Y + prizeAdjustment)
                return buttonATimes * 3 + buttonBTimes * 1;
            else
                return 0;
        }

        private long CalculateButtonATimes(Machine machine, long prizeAdjustment)
        {
            var prizeX = machine.Prize.X + prizeAdjustment;
            var prizeY = machine.Prize.Y + prizeAdjustment;
            var part1 = machine.ButtonB.Y * prizeX - machine.ButtonB.X * prizeY;
            var part2 = machine.ButtonA.X * machine.ButtonB.Y - machine.ButtonB.X * machine.ButtonA.Y;
            return part1 / part2;
        }

        private long CalculateButtonBTimes(Machine machine, long buttonATimes, long prizeAdjustment)
        {
            var prizeY = machine.Prize.Y + prizeAdjustment;
            var part1 = prizeY - buttonATimes * machine.ButtonA.Y;
            var part2 = machine.ButtonB.Y;
            return part1 / part2;
        }
    }
}
