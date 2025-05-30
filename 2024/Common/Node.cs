using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Advent2024.Day18RAMRun;

namespace Advent2024
{
    public class Node : Tuple<int, int>
    {
        public Node(int row, int col) : base(row, col) { }

        public int Row { get { return Item1; } }
        public int Col { get { return Item2; } }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var other = (Node)obj;
            return Row == other.Row && Col == other.Col;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode() ^ Col.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + Row + "," + Col + ")";
        }
    }
}
