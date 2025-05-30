using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day12GardenGroups : SolverBase
    {
        private readonly List<string> _map = new List<string>();

        private readonly int[,] _allDirections = { { -1, 0 }, { 0, +1 }, { +1, 0 }, { 0, -1 } };
        private readonly Node _directionUp = new Node(-1, 0);
        private readonly Node _directionDown = new Node(+1, 0);
        private readonly Node _directionRight = new Node(0, +1);
        private readonly Node _directionLeft = new Node(0, -1);

        public Day12GardenGroups(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            while(true)
            {
                var line = inputData.ReadLine();
                if (line == null)
                    break;
                _map.Add(line);
            }
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            return CalculateTotalPrice(false).ToString();
        }

        public override string GetPartTwoAnswer()
        {
            return CalculateTotalPrice(true).ToString();
        }

        private long CalculateTotalPrice(bool useNumberOfSides)
        {
            long totalPrice = 0;
            var regionNodes = new List<Node>();
            for (int row = 0; row < _map.Count; row++)
            {
                for (int col = 0; col < _map[row].Length; col++)
                {
                    var currentLocation = new Node(row, col);
                    if (regionNodes.Contains(currentLocation))
                        continue;
                    var region = GetRegion(currentLocation);
                    regionNodes.AddRange(region);
                    var area = region.Count;
                    if (useNumberOfSides)
                    {
                        var numberOfSides = CountNumberOfSides(region);
                        totalPrice += area * numberOfSides;
                    }
                    else
                    { 
                        var perimeter = CountPerimeter(region);
                        totalPrice += area * perimeter;
                    }
                }
            }
            return totalPrice;
        }

        private int CountPerimeter(List<Node> region)
        {
            var count = 0;
            foreach (Node position in region)
            {
                for (int i = 0; i < _allDirections.GetLength(0); i++)
                {
                    var fencePosition = new Node(position.Row + _allDirections[i, 0], position.Col + _allDirections[i, 1]);
                    if (CanPlaceFence(_map[position.Row][position.Col], fencePosition))
                        count++;
                }
            }
            return count;
        }

        private int CountNumberOfSides(List<Node> region)
        {
            var count = 0;
            var sidePositionsUp = new List<Node>();
            var sidePositionsDown = new List<Node>();
            var sidePositionsRight = new List<Node>();
            var sidePositionsLeft = new List<Node>();
            foreach (Node position in region)
            {
                if (HasSide(position, _directionUp, _directionRight, _directionLeft, sidePositionsUp))
                    count++;
                if (HasSide(position, _directionDown, _directionRight, _directionLeft, sidePositionsDown))
                    count++;
                if (HasSide(position, _directionRight, _directionUp, _directionDown, sidePositionsRight))
                    count++;
                if (HasSide(position, _directionLeft, _directionUp, _directionDown, sidePositionsLeft))
                    count++;
            }
            return count;
        }

        private bool HasSide(Node plantPosition, Node fenceDirection, Node side1Direction, Node side2Direction, List<Node> sidePositions)
        {
            var plant = _map[plantPosition.Row][plantPosition.Col];
            var fencePosition = new Node(plantPosition.Row + fenceDirection.Row, plantPosition.Col + fenceDirection.Col);
            if (!CanPlaceFence(plant, fencePosition))
                return false;
            if (sidePositions.Contains(fencePosition))
                return false;
            sidePositions.Add(fencePosition);
            VerifyAlignedNeighbours(plantPosition, fenceDirection, side1Direction, sidePositions);
            VerifyAlignedNeighbours(plantPosition, fenceDirection, side2Direction, sidePositions);
            return true;
        }

        private void VerifyAlignedNeighbours(Node plantPosition, Node fenceDirection, Node neighbourDirection, List<Node> sidePositions)
        {
            var plant = _map[plantPosition.Row][plantPosition.Col];
            var neighbour = new Node(plantPosition.Row + neighbourDirection.Row, plantPosition.Col + neighbourDirection.Col);
            while (IsValidPosition(neighbour) && _map[neighbour.Row][neighbour.Col] == plant)
            {
                var fencePosition = new Node(neighbour.Row + fenceDirection.Row, neighbour.Col + fenceDirection.Col);
                if (!CanPlaceFence(plant, fencePosition))
                    break;
                sidePositions.Add(fencePosition);
                neighbour = new Node(neighbour.Row + neighbourDirection.Row, neighbour.Col + neighbourDirection.Col);
            }
        }

        private bool CanPlaceFence(char plant, Node position)
        {
            return !IsValidPosition(position) || _map[position.Row][position.Col] != plant;
        }

        private List<Node> GetRegion(Node position)
        {
            var region = new List<Node>();
            var visited = new List<Node>();
            Search(region, visited, position, _map[position.Row][position.Col]);
            return region;
        }

        private void Search(List<Node> region, List<Node> visited, Node position, char plant) 
        {
            if (visited.Contains(position)) 
                return;
            visited.Add(position);
            if (_map[position.Row][position.Col] != plant)
                return;
            if (!region.Contains(position))
                region.Add(position);
            for (int i = 0; i < _allDirections.GetLength(0); i++)
            {
                var neighbour = new Node(position.Row + _allDirections[i, 0], position.Col + _allDirections[i, 1]);
                if (IsValidPosition(neighbour))
                    Search(region, visited, neighbour, plant);
            }
        }

        private bool IsValidPosition(Node position)
        {
            if (position.Row < 0 || position.Row >= _map.Count)
                return false;
            if (position.Col < 0 || position.Col >= _map[position.Row].Length)
                return false;
            return true;
        }

    }
}
