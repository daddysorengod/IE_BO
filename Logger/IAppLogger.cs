namespace AppLogger
{
    public interface IAppLogger
    {
        void Error(string message);
        void Warning(string message);
        void Info(string message);
        void Warn(string message);
        void Debug(string message);

        void DebugSql(string message);
    }
}
