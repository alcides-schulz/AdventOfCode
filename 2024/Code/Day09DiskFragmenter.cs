using System;
using System.Collections.Generic;
using System.IO;

namespace Advent2024
{
    public class Day09DiskFragmenter : SolverBase
    {
        private readonly string _diskMap = "";

        public Day09DiskFragmenter(string dataFileName)
        {
            var inputData = new StreamReader(dataFileName);
            _diskMap = inputData.ReadLine();
            inputData.Close();
        }

        public override string GetPartOneAnswer()
        {
            var disk = GetDiskFromMap();
            CompactSingleBlocks(disk);
            return GetCheckSum(disk).ToString();
        }

        public override string GetPartTwoAnswer()
        {
            var disk = GetDiskFromMap();
            CompactWholeFile(disk);
            return GetCheckSum(disk).ToString();
        }

        private long GetCheckSum(List<int> disk)
        {
            long checkSum = 0;
            for (int i = 0; i < disk.Count; i++)
            {
                if (disk[i] != -1)
                    checkSum += disk[i] * i;
            }
            return checkSum;
        }

        private void CompactWholeFile(List<int> disk)
        {
            var lastFileIndex = disk.Count;
            while (lastFileIndex > 0)
            {
                var file = GetFile(disk, lastFileIndex - 1);
                lastFileIndex = file.Item1;
                var space = GetSpace(disk, lastFileIndex, file.Item2);
                if (file.Item2 <= space.Item2)
                    MoveFile(file, space, disk);
            }
        }

        private void MoveFile(Tuple<int, int> file, Tuple<int, int> space, List<int> disk)
        {
            for (int i = 0; i < file.Item2; i++)
            {
                disk[space.Item1 + i] = disk[file.Item1 + i];
                disk[file.Item1 + i] = -1;
            }
        }

        private Tuple<int, int> GetFile(List<int> disk, int lastFileBeforeIndex)
        {
            var index = lastFileBeforeIndex;
            while (index > 0 && disk[index] == -1)
                index--;
            var fileId = disk[index];
            var end = index;
            while (index > 0 && disk[index] == fileId)
                index--;
            var start = index == 0 ? index : index + 1;
            return Tuple.Create(start, end - start + 1);
        }

        private Tuple<int,int> GetSpace(List<int> disk, int fileIndex, int fileSize)
        {
            int index = 0;
            while (index < fileIndex)
            {
                while (disk[index] != -1 && index < fileIndex)
                    index++;
                var start = index;
                while (disk[index] == -1 && index < fileIndex)
                    index++;
                var size = index - start;
                if (size >= fileSize)
                    return Tuple.Create(start, size);
            }
            return Tuple.Create(0, 0);
        }

        private void CompactSingleBlocks(List<int> disk)
        {
            var space_index = 0;
            var file_index = disk.Count - 1;
            while(file_index > space_index)
            {
                while (space_index < file_index && disk[space_index] != -1)
                    space_index++;
                while (file_index > space_index && disk[file_index] == -1)
                    file_index--;
                if (file_index > space_index)
                {
                    disk[space_index] = disk[file_index];
                    disk[file_index] = -1;
                }
            }
        }

        private List<int> GetDiskFromMap()
        {
            var list = new List<int>();
            var fileId = 0;
            var isFileBlock = true;
            foreach(var item in _diskMap.ToCharArray())
            {
                if (isFileBlock)
                {
                    for (var count = item - '0'; count > 0; count--)
                        list.Add(fileId);
                    fileId++;
                }
                else
                {
                    for (var count = item - '0'; count > 0; count--)
                        list.Add(-1);
                }
                isFileBlock = !isFileBlock;
            }
            return list;
        }
    }
}
