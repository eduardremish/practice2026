namespace task17
{
    public interface ILongCommand: ICommand
    {
        bool IsCompleted { get; }
    }
}
