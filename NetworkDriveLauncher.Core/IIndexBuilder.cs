namespace NetworkDriveLauncher.Core
{
    public interface IIndexBuilder<out T> where T : IIndexConfiguration
    {
        T Configuration { get; }
        void BuildIndex();
    }
}