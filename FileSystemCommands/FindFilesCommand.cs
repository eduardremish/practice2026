using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CommandLib;

namespace FileSystemCommands
{
    public class FindFilesCommand : ICommand
    {
        private readonly string _dirPath;
        private readonly string _searchPattern;
        public List<string> FoundFiles { get; private set; } = new();

        public FindFilesCommand(string dirPath, string searchPattern)
        {
            _dirPath = dirPath;
            _searchPattern = searchPattern;
        }

        public void Execute()
        {
            if (!Directory.Exists(_dirPath))
            {
                Console.WriteLine($"Директория не найдена: {_dirPath}");
                return;
            }

            FoundFiles = Directory
                .GetFiles(_dirPath, _searchPattern)
                .ToList();

            Console.WriteLine($"Файлы по маске {_searchPattern}:");

            foreach (var file in FoundFiles)
            {
                Console.WriteLine(Path.GetFileName(file));
            }
        }
    }
}
