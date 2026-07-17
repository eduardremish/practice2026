using System;

namespace task17
{
    public static class ExceptionHandler
    {
        public static ICommand FailedCommand { get; set; }
        public static Exception CapturedError { get; set; }

        public static void Handler(ICommand command, Exception error)
        {
            FailedCommand = command;
            CapturedError = error;
            Console.WriteLine($"{CapturedError.Message} {FailedCommand.GetType().Name}");
        }

        public static void Reset()
        {
            FailedCommand = null;
            CapturedError = null;
        }
    }
}
