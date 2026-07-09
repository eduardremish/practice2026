using System;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _dirPath;
        public long CalculatedSize { get; private set; }

        public DirectorySizeCommand(string dirPath)
        {
            _dirPath = dirPath;
        }

        public void Execute()
        {
            if (!Directory.Exists(_dirPath))
            {
                Console.WriteLine($"Директория не найдена: {_dirPath}");
                return;
            }

            CalculatedSize = Directory
                .GetFiles(_dirPath, "*.*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f).Length)
                .Sum();

            Console.WriteLine($"Размер директории '{_dirPath}': {CalculatedSize} байт.");
        }
    }
}
