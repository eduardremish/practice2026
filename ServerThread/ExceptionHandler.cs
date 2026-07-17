using System;

namespace  task17
{
    public static class ExceptionHandler
    {
        public static ICommand LastCommand { get; set; }
        public static Exception LastException { get; set; }

        public static void Handle(ICommand _lastCommand, Exception _lastException)
        {
            LastCommand = _lastCommand; 
            LastException = _lastException;
            Console.WriteLine($"Ошибка {LastException.Message}. Команда {LastCommand.GetType().Name} ");
        }
        public static void Clear()
        {
            LastCommand = null;
            LastException = null;
        }
    }
}
