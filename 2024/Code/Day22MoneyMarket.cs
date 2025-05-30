using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace Advent2024
{
    public class Day22MoneyMarket : SolverBase
    {
        private List<long> _numbers = new List<long>();

        public Day22MoneyMarket(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _numbers.Add(int.Parse(line));
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            return CalculateSecretNumberSum().ToString();
        }

        public override string GetPartTwoAnswer()
        {
            return CalculateBestBuy().ToString();
        }

        private long GetNewNumber(long number)
        {
            var secretNumber = number;
            secretNumber = ((secretNumber * 64) ^ secretNumber) % 16777216L;
            secretNumber = ((secretNumber / 32) ^ secretNumber) % 16777216L;
            secretNumber = ((secretNumber * 2048) ^ secretNumber) % 16777216L;
            return secretNumber;
        }

        private long CalculateSecretNumberSum()
        {
            var total = 0L;
            foreach (var number in _numbers)
            {
                var secretNumber = number;
                for (int i = 0; i < 2000; i++)
                    secretNumber = GetNewNumber(secretNumber);
                total += secretNumber;
            }
            return total;
        }

        private long CalculateBestBuy()
        {
            var priceTable = new Dictionary<string, int>();
            foreach (var number in _numbers)
            {
                var secretNumber = number;
                var prevPrice = (int)(number % 10);
                var diffList = new List<int>(2000);
                var firstUseSequenceList = new List<string>();
                for (int i = 0; i < 2000; i++)
                {
                    secretNumber = GetNewNumber(secretNumber);
                    var price = (int)(secretNumber % 10);
                    var diff = price - prevPrice;
                    diffList.Add(diff);
                    prevPrice = price;
                    if (i >= 3)
                    {
                        var changeSequence = $"{diffList[i - 3]} {diffList[i - 2]} {diffList[i - 1]} {diffList[i]}";
                        if (!firstUseSequenceList.Contains(changeSequence))
                        {
                            firstUseSequenceList.Add(changeSequence);
                            if (priceTable.ContainsKey(changeSequence))
                                priceTable[changeSequence] += price;
                            else
                                priceTable.Add(changeSequence, price);
                        }
                    }
                }
            }
            return priceTable.Values.Max();
        }
    }
}
