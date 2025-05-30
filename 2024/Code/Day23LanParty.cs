using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent2024
{
    public class Day23LanParty : SolverBase
    {
        private Dictionary<string, HashSet<string>> _connections = new Dictionary<string, HashSet<string>>();

        public Day23LanParty(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                var computer1 = line.Split('-')[0];
                var computer2 = line.Split('-')[1];
                if (!_connections.ContainsKey(computer1))
                    _connections.Add(computer1, new HashSet<string>());
                if (!_connections[computer1].Contains(computer2))
                    _connections[computer1].Add(computer2);
                if (!_connections.ContainsKey(computer2))
                    _connections.Add(computer2, new HashSet<string>());
                if (!_connections[computer2].Contains(computer1))
                    _connections[computer2].Add(computer1);
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            return CreateThreeComputersSets().Count.ToString();
        }

        public override string GetPartTwoAnswer()
        {
            var cliques = new List<HashSet<string>>();
            BronKerbosch(cliques, new HashSet<string>(), new HashSet<string>(_connections.Keys), new HashSet<string>());
            var largestSet = cliques.OrderByDescending(c => c.Count).First().OrderBy(x => x);
            return string.Join(",", largestSet);
        }

        private List<string> CreateThreeComputersSets()
        {
            var sets = new List<string>();
            foreach (var computer0 in _connections.Keys)
            {
                foreach (var computer1 in _connections[computer0])
                {
                    foreach (var computer2 in _connections[computer1])
                    {
                        if (!_connections[computer2].Contains(computer0))
                            continue;
                        if (computer0.StartsWith("t") || computer1.StartsWith("t") || computer2.StartsWith("t"))
                        {
                            var newSet = string.Join(",", new SortedSet<string>() { computer0, computer1, computer2 });
                            if (!sets.Contains(newSet))
                                sets.Add(newSet);
                        }
                    }
                }
            }
            return sets;
        }
         
        /// BronKerbosch algorithm to find largest set (clique)
        /// r: The current set of nodes forming a potential clique.
        /// p: The set of candidate nodes (P) that can extend the current clique.
        /// x: The set of excluded nodes (X) that have already been processed.
        private void BronKerbosch(List<HashSet<string>> cliques, HashSet<string> r, HashSet<string> p, HashSet<string> x)
        {
            if (!p.Any() && !x.Any())
            {
                cliques.Add(new HashSet<string>(r));
                return;
            }
            foreach (var node in p.ToList())
            {
                var new_r = new HashSet<string>(r);
                var new_p = new HashSet<string>(p);
                var new_x = new HashSet<string>(x);
                new_r.Add(node);
                new_p.IntersectWith(_connections[node]);
                new_x.IntersectWith(_connections[node]);

                BronKerbosch(cliques, new_r, new_p, new_x);

                p.Remove(node);
                x.Add(node);
            }
        }

    }
}
