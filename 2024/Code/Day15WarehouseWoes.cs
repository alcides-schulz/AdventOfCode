using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Advent2024
{
    public class Day15WarehouseWoes : SolverBase
    {
        private readonly List<string> _map = new List<string>();
        private readonly List<string> _moves = new List<string>();
        private readonly Dictionary<char, Node> _direction = new Dictionary<char, Node>();

        public Day15WarehouseWoes(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                if (line == "")
                    break;
                _map.Add(line);
            }
            while (true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _moves.Add(line);
            }
            inputData.Close();

            _direction.Add('^', new Node(-1, 0));
            _direction.Add('>', new Node(0, +1));
            _direction.Add('v', new Node(+1, 0));
            _direction.Add('<', new Node(0, -1));
        }

        public override string GetPartOneAnswer()
        {
            var map = GetSmallBoxesLayout();
            ProcessSmallBoxesLayout(map);
            return CountSmallBoxes(map).ToString();
        }

        public override string GetPartTwoAnswer()
        {
            var map = GetLargeBoxesLayout();
            ProcessLargeBoxesLayout(map);
            return CountLargeBoxes(map).ToString();
        }

        private void ProcessSmallBoxesLayout(List<List<char>> map)
        {
            var robotPosition = GetRobotPosition(map);
            foreach(var line in _moves)
            {
                foreach(var move in line.ToArray())
                {
                    robotPosition = ExecuteSmallBoxesMove(map, robotPosition, move);
                }
            }
        }

        private void ProcessLargeBoxesLayout(List<List<char>> map)
        {
            var robotPosition = GetRobotPosition(map);
            foreach (var line in _moves)
            {
                foreach (var move in line.ToArray())
                {
                    robotPosition = ExecuteLargeBoxesMove(map, robotPosition, move);
                }
            }
        }

        private long CountSmallBoxes(List<List<char>> map)
        {
            var count = 0L;
            for (int row = 0; row < map.Count; row++)
            {
                for (int col = 0; col < map[row].Count; col++)
                {
                    if (map[row][col] == 'O')
                        count += 100 * row + col;
                }
            }
            return count;
        }
        private long CountLargeBoxes(List<List<char>> map)
        {
            var count = 0L;
            for (int row = 0; row < map.Count; row++)
            {
                for (int col = 0; col < map[row].Count; col++)
                {
                    if (map[row][col] == '[')
                        count += 100 * row + col;
                }
            }
            return count;
        }

        private List<List<char>> GetSmallBoxesLayout()
        {
            var map = new List<List<char>>();
            foreach (var line in _map)
                map.Add(new List<char>(line.ToCharArray()));
            return map;
        }

        private List<List<char>> GetLargeBoxesLayout()
        {
            var map = new List<List<char>>();
            foreach (var line in _map)
            {
                var list = new List<char>();
                foreach(var character in line.ToArray())
                {
                    switch (character)
                    {
                        case '#': list.AddRange("##".ToCharArray()); break;
                        case '.': list.AddRange("..".ToCharArray()); break;
                        case 'O': list.AddRange("[]".ToCharArray()); break;
                        case '@': list.AddRange("@.".ToCharArray()); break;
                    }
                }
                map.Add(list);
            }
            return map;
        }

        private Node ExecuteLargeBoxesMove(List<List<char>> map, Node robotPosition, char move)
        {
            var direction = _direction[move];
            var targetPosition = new Node(robotPosition.Row + direction.Row, robotPosition.Col + direction.Col);
            if (map[targetPosition.Row][targetPosition.Col] == '#')
                return robotPosition;
            if (map[targetPosition.Row][targetPosition.Col] == '.')
            {
                map[robotPosition.Row][robotPosition.Col] = '.';
                map[targetPosition.Row][targetPosition.Col] = '@';
                return targetPosition;
            }
            if (move == '>')
                return SearchSpotOnRight(map, robotPosition);
            if (move == '<')
                return SearchSpotOnLeft(map, robotPosition);
            return SearchSpotUpDown(map, robotPosition, direction.Row);
        }

        private Node SearchSpotUpDown(List<List<char>> map, Node robotPosition, int rowDirection)
        {
            var boxRowList = GetAllDependentBoxes(map, robotPosition, rowDirection);
            if (IsAllBoxesMoveable(map, boxRowList, rowDirection))
            {
                MoveAllBoxes(map, boxRowList, rowDirection);
                var newRobotPosition = new Node(robotPosition.Row + rowDirection, robotPosition.Col);
                map[robotPosition.Row][robotPosition.Col] = '.';
                map[newRobotPosition.Row][newRobotPosition.Col] = '@';
                return newRobotPosition;
            }
            return robotPosition;
        }

        private void MoveAllBoxes(List<List<char>> map, List<List<Node>> boxRowList, int rowDirection)
        {
            foreach (var rowBox in boxRowList)
            {
                foreach (var box in rowBox)
                {
                    map[box.Row + rowDirection][box.Col] = '[';
                    map[box.Row + rowDirection][box.Col + 1] = ']';
                    map[box.Row][box.Col] = '.';
                    map[box.Row][box.Col + 1] = '.';
                }
            }
        }

        private bool IsAllBoxesMoveable(List<List<char>> map, List<List<Node>> boxRowList, int rowDirection)
        {
            var movableTiles = new List<Node>();
            foreach (var rowBox in boxRowList)
            {
                foreach(var box in rowBox)
                {
                    if (!CanBoxMove(map, box, rowDirection, movableTiles))
                        return false;
                    movableTiles.Add(new Node(box.Row, box.Col));
                    movableTiles.Add(new Node(box.Row, box.Col + 1));
                }
            }
            return true;
        }

        private bool CanBoxMove(List<List<char>> map, Node boxPosition, int rowDirection, List<Node> movableTiles)
        {
            for (int colPosition = 0; colPosition < 2; colPosition++)
            {
                var tile = new Node(boxPosition.Row + rowDirection, boxPosition.Col + colPosition);
                var item = map[tile.Row][tile.Col];
                if (item == '#')
                    return false;
                if ("[]".IndexOf(item) != -1 && !movableTiles.Contains(tile))
                    return false;
            }
            return true;
         }

        private List<List<Node>> GetAllDependentBoxes(List<List<char>> map, Node robotPosition, int rowDirection)
        {
            var boxRowList = new List<List<Node>>();
            var boxes = new List<Node>();
            GetDependentBoxes(map, robotPosition, rowDirection, boxes);
            boxRowList.Add(boxes);
            while (true)
            {
                var newBoxes = new List<Node>();
                foreach (var box in boxes)
                {
                    var tempBoxes = new List<Node>();
                    GetDependentBoxes(map, box, rowDirection, tempBoxes);
                    foreach (var tempBox in tempBoxes)
                        if (!newBoxes.Contains(tempBox))
                            newBoxes.Add(tempBox);
                }
                if (newBoxes.Count == 0)
                    break;
                boxRowList.Insert(0, newBoxes);
                boxes = newBoxes;
            }
            return boxRowList;
        }

        private void GetDependentBoxes(List<List<char>> map, Node position, int rowDirection, List<Node> boxes)
        {
            if (map[position.Row + rowDirection][position.Col] == '[')
                boxes.Add(new Node(position.Row + rowDirection, position.Col));
            if (map[position.Row + rowDirection][position.Col] == ']')
                boxes.Add(new Node(position.Row + rowDirection, position.Col - 1));
            if (map[position.Row][position.Col] == '[')
                if (map[position.Row + rowDirection][position.Col + 1] == '[')
                    boxes.Add(new Node(position.Row + rowDirection, position.Col + 1));
        }

        private Node SearchSpotOnRight(List<List<char>> map, Node robotPosition)
        {
            var searchPosition = new Node(robotPosition.Row, robotPosition.Col + 1);
            while (map[searchPosition.Row][searchPosition.Col] != '#')
            {
                if (map[searchPosition.Row][searchPosition.Col] == '.')
                {
                    for (var i = searchPosition.Col; i > robotPosition.Col; i--)
                    {
                        map[searchPosition.Row][i] = map[searchPosition.Row][i - 1];
                    }
                    map[robotPosition.Row][robotPosition.Col] = '.';
                    return new Node(robotPosition.Row, robotPosition.Col + 1);
                }
                searchPosition = new Node(searchPosition.Row, searchPosition.Col + 1);
            }
            return robotPosition;
        }

        private Node SearchSpotOnLeft(List<List<char>> map, Node robotPosition)
        {
            var searchPosition = new Node(robotPosition.Row, robotPosition.Col - 1);
            while (map[searchPosition.Row][searchPosition.Col] != '#')
            {
                if (map[searchPosition.Row][searchPosition.Col] == '.')
                {
                    for (var i = searchPosition.Col; i < robotPosition.Col; i++)
                    {
                        map[searchPosition.Row][i] = map[searchPosition.Row][i + 1];
                    }
                    map[robotPosition.Row][robotPosition.Col] = '.';
                    return new Node(robotPosition.Row, robotPosition.Col - 1);
                }
                searchPosition = new Node(searchPosition.Row, searchPosition.Col - 1);
            }
            return robotPosition;
        }

        private Node ExecuteSmallBoxesMove(List<List<char>> map, Node robotPosition, char move)
        {
            var direction = _direction[move];
            var targetPosition = new Node(robotPosition.Row + direction.Row, robotPosition.Col + direction.Col);
            if (map[targetPosition.Row][targetPosition.Col] == '#')
                return robotPosition;
            if (map[targetPosition.Row][targetPosition.Col] == '.')
            {
                map[robotPosition.Row][robotPosition.Col] = '.';
                map[targetPosition.Row][targetPosition.Col] = '@';
                return targetPosition;
            }
            var searchPosition = new Node(targetPosition.Row + direction.Row, targetPosition.Col + direction.Col);
            while (map[searchPosition.Row][searchPosition.Col] != '#')
            {
                if (map[searchPosition.Row][searchPosition.Col] == '.')
                {
                    map[searchPosition.Row][searchPosition.Col] = 'O';
                    map[targetPosition.Row][targetPosition.Col] = '@';
                    map[robotPosition.Row][robotPosition.Col] = '.';
                    return targetPosition;
                }
                searchPosition = new Node(searchPosition.Row + direction.Row, searchPosition.Col + direction.Col);
            }
            return robotPosition;
        }

        private Node GetRobotPosition(List<List<char>> map)
        {
            for (int row = 0; row < _map.Count; row++)
            {
                var col = map[row].IndexOf('@');
                if (col != -1)
                    return new Node(row, col);
            }
            return null;
        }
    }
}
